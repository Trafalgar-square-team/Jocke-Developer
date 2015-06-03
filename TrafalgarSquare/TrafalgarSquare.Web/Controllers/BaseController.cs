using System;
using Microsoft.AspNet.Identity;
using TrafalgarSquare.Models;
using TrafalgarSquare.Web.ViewModels.User;

namespace TrafalgarSquare.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using Data;
    using ViewModels;
    using System.Web.Routing;

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
                .Select(x => new TopPostViewModel()
                {
                    Id = x.post.Id,
                    Title = x.post.Title,
                    PostResources = x.post.Resource,
                    PostOwnerId = x.post.PostOwnerId,
                    // TODO: add UserViewModel
                    PostOwner = x.post.PostOwner,
                    LikesCount = x.post.LikesPost.Count,
                }).ToList();
            return topJokes;
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

        [Authorize]
        public void DeletePostInCategorie(int postId)
        {
            Data.Posts.DeleteById(postId);
            Data.Posts.SaveChanges();
        }
    }
}