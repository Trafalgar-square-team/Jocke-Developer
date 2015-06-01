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

    public abstract class BaseController : Controller
    {
        public readonly int PageSize;
        private User userProfile;  

        protected BaseController()
        {
        }


        protected BaseController(ITrafalgarSquareData data)
        {
            Data = data;
            ViewBag.Categories = data.Categories.All().Where(c=>!c.IsDisabled).ToList();
            PageSize = int.Parse(WebConfigurationManager.AppSettings["PageSize"]);
        }

        protected BaseController(ITrafalgarSquareData data, User userProfile)
            : this(data)
        {
            this.UserProfile = userProfile;
        }

        protected ITrafalgarSquareData Data { get; set; }
        protected User UserProfile { get; private set; }


        protected IEnumerable<TopPostViewModel> Top10Jokes()
        {
            return TopJokes(10);
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

        public IEnumerable<PostViewModel> GetPostsByCategory(int categoryId, int page)
        {

            var getPageFromDb = ((page - 1) * PageSize);

            if (getPageFromDb < 0)
            {
                getPageFromDb = 1;
            }

            //TODO Когато заявката иска Пост, който е извън колекцията, да се хвърля правилна грешка, иначе гърми
            var posts = Data.Posts.All()
                .Where(p => p.Category.Id == categoryId)
                .OrderByDescending(p => p.CreatedDateTime)
                .Select(p => new PostViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Text = p.Text,
                    Resource = p.Resource,
                    CreatedDateTime = p.CreatedDateTime,
                    IsReported = p.IsReported ?? false,
                    Likes = p.LikesPost.Count(),
                    CommentsCount = p.Comments.Count(),
                    User = new UserViewModel
                    {
                        Id = p.PostOwnerId,
                        Username = p.PostOwner.UserName,
                        AvatarUrl = p.PostOwner.AvatarUrl
                    }
                })
                .Skip(getPageFromDb)
                .Take(PageSize)
                .ToList();

            return posts;
        }

        [Authorize]
        public void BaseForAllCategoriesPostCreat(PostCreateBindModel post)
        {

            var currentUserId = User.Identity.GetUserId();

            var postToCreate = new Post
            {
                Text = post.Text,
                Title = post.Title,

                Resource = new PostResources()
                {
                    VideoUrl = post.Resource
                },

                PostOwnerId = currentUserId,
                CreatedDateTime = DateTime.Now,
                CategoryId = post.CategoryId

            };

            Data.Posts.Add(postToCreate);
            Data.Posts.SaveChanges();
        }

        [Authorize]
        public void DeletePostInCategorie(int postId)
        {   
            Data.Posts.DeleteById(postId);
            Data.Posts.SaveChanges();
        }
    }
}