namespace TrafalgarSquare.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using System.Web.Routing;
    using AutoMapper;
    using Data;
    using Microsoft.AspNet.Identity;
    using TrafalgarSquare.Models;
    using TrafalgarSquare.Web.ViewModels.User;
    using ViewModels;

    public abstract class BaseController : Controller
    {
        public readonly int PageSize;
        private ITrafalgarSquareData data;
        private User userProfile;

        protected BaseController()
        {
        }


        protected BaseController(ITrafalgarSquareData data)
        {
            this.Data = data;
            ViewBag.Categories = data.Categories.All().Where(c => !c.IsDisabled).ToList();
            this.PageSize = int.Parse(WebConfigurationManager.AppSettings["PageSize"]);
        }

        protected ITrafalgarSquareData Data { get; set; }

        protected User UserProfile { get; private set; }

        protected IEnumerable<TopPostViewModel> Top10Jokes()
        {
            return this.TopJokes(10);
        }

        protected IEnumerable<TopPostViewModel> TopJokes(int showNumber)
        {
            // GrouJoin prevents from changing order unlike groupBy
            //var topJokes = this.Data.Posts.All()
            //    .GroupJoin(
            //       this.Data.PostsLikes.All(),
            //        x => x.Id,
            //        postLikes => postLikes.PostId,
            //        (post, postLikes) => new
            //        {
            //            post = post,
            //            postLikes = postLikes.Count()
            //        })
            //    .OrderByDescending(x => x.postLikes)
            //    .ThenByDescending(x => x.post.CreatedDateTime)
            //    .Take(showNumber)
            //    .AsEnumerable()
            //    .Select(x => new TopPostViewModel()
            //    {
            //        Id = x.post.Id,
            //        Title = x.post.Title,
            //        PostResources = x.post.Resource,
            //        PostOwnerId = x.post.PostOwnerId,
            //        PostOwner = Mapper.Map<UserViewModel>(x.post.PostOwner),
            //        LikesCount = x.postLikes,
            //    }).ToList();

            var topJokes = this.Data.Posts.All()
                 .GroupJoin(
                    this.Data.PostsLikes.All(),
                     x => x.Id,
                     postLikes => postLikes.PostId,
                     (post, postLikes) => new
                     {
                         post = post,
                         postLikes = postLikes.Count()
                     })
                 .OrderByDescending(x => x.postLikes)
                 .ThenByDescending(x => x.post.CreatedDateTime)
                 .Take(showNumber)
                 .AsEnumerable()
                 .Select(x => Mapper.Map<Post, TopPostViewModel>(
                     x.post, 
                     opts =>
                     {
                         opts.AfterMap((s, d) => d.LikesCount = x.postLikes);
                         opts.BeforeMap((s, d) => d.Owner = Mapper.Map<User, UserViewModel>(x.post.PostOwner));
                     }))
                 .ToList();

            return topJokes;
        }

        [Authorize]
        public void DeletePostInCategorie(int postId)
        {
            Data.Posts.DeleteById(postId);
            Data.Posts.SaveChanges();
        }

        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            if (requestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var userName = requestContext.HttpContext.User.Identity.Name;
                var user = this.Data.Users.All().FirstOrDefault(x => x.UserName == userName);
                this.UserProfile = user;
            }

            if (this.UserProfile == null)
            {
                //throw new InstanceNotFoundException("shit");
                return base.BeginExecute(requestContext, callback, state);
            }

            return base.BeginExecute(requestContext, callback, state);
        }
    }
}