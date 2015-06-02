
namespace TrafalgarSquare.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Data;
    using Microsoft.AspNet.Identity;
    using Models;
    using ViewModels;
    using ViewModels.User;

    public class PostsController : BaseController
    {
        public PostsController(ITrafalgarSquareData data)
            : base(data)
        {
        }

        [Route("posts/{categoryMachineName}/{page:int?}")]
        public ActionResult Index(string categoryMachineName, int page = 1)
        {
            page = page < 1 ? 1 : page;
            var category = GetCategoryByMachineName(categoryMachineName);
            if (category == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Title = category.Name;
            ViewBag.CategoryMachineName = categoryMachineName;
            ViewBag.CurrentPage = page;
            var posts = GetPostsByCategory(category.Id, page).ToList();

            return View(posts);
        }

        [Route("post/{postId:int}")]
        public ActionResult PostById(int postId)
        {
            var post = Data.Posts.All()
                .Where(p => p.Id == postId)
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
                }).FirstOrDefault();

            return View(post);
        }

        [Authorize]
        [HttpGet]
        [Route("posts/{categoryMachineName}/add")]
        public ActionResult CreatePost(string categoryMachineName)
        {
            var category = GetCategoryByMachineName(categoryMachineName);
            if (category == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Title = category.Name;
            return View();
        }

        [Authorize]
        [HttpPost]
        [Route("post/add")]
        public ActionResult CreatePost(PostCreateBindModel post)
        {
            var currentUserId = User.Identity.GetUserId();

            var newPost = new Post
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

            Data.Posts.Add(newPost);
            Data.Posts.SaveChanges();

            return View(newPost);
        }


        private Category GetCategoryByMachineName(string machineName)
        {
            return Data.Categories.All().FirstOrDefault(c => !c.IsDisabled && c.MachineName.ToLower().Equals(machineName));
        }

        private IEnumerable<PostViewModel> GetPostsByCategory(int categoryId, int page)
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
    }
}