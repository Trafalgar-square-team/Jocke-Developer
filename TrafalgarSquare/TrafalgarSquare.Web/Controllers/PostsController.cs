
namespace TrafalgarSquare.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
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
            var category = this.GetCategoryByMachineName(categoryMachineName);
            if (category == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            ViewBag.Title = category.Name;
            ViewBag.CategoryMachineName = categoryMachineName;
            ViewBag.CurrentPage = page;
            var posts = this.GetPostsByCategory(category.Id, page).OrderByDescending(p => p.CreatedDateTime).ToList();

            return this.View(posts);
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

            return this.View(post);
        }

        [Authorize]
        [HttpGet]
        [Route("post/create")]
        public ActionResult CreatePost()
        {
            ViewBag.CategoryId = new SelectList(Data.Categories.All().Where(c=>!c.IsDisabled), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("post/create")]
        public ActionResult CreatePost([Bind(Include = "Id,Title,Text,Resource,CategoryId,CreatedDateTime,IsReported")] Post post)
        {
            if (ModelState.IsValid)
            {
                post.PostOwnerId = User.Identity.GetUserId();
                Data.Posts.Add(post);
                Data.SaveChanges();
                return this.RedirectToAction("Index", "Home");
            }

            ViewBag.CategoryId = new SelectList(Data.Categories.All().Where(c => !c.IsDisabled), "Id", "Name", post.CategoryId);
            return this.View(post);
        }

        [HttpGet]
        [Authorize]
        [Route("post/edit/{id}")]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return this.RedirectToAction("Index", "Home");
            }
            var post = Data.Posts.GetById(id);
            if (post == null || post.PostOwnerId != User.Identity.GetUserId())
            {
                return this.RedirectToAction("Index", "Home");
            }

            ViewBag.CategoryId = new SelectList(Data.Categories.All().Where(c => !c.IsDisabled), "Id", "Name", post.CategoryId);
            return this.View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [Route("post/edit/{id}")]
        public ActionResult EditPost([Bind(Include = "Id,Title,Text,Resource,CategoryId,CreatedDateTime,IsReported")] Post post)
        {
            if (ModelState.IsValid)
            {
                if (post.PostOwnerId != User.Identity.GetUserId())
                {
                    return this.RedirectToAction("Index", "Home");
                }

                Data.Posts.Update(post);
                Data.SaveChanges();
                return this.RedirectToAction("Index", "Home");
            }
            ViewBag.CategoryId = new SelectList(Data.Categories.All().Where(c => !c.IsDisabled), "Id", "Name", post.CategoryId);
            return this.View(post);
        }

        [Route("post/delete/{id}")]
        public ActionResult DeletePost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var post = Data.Posts.GetById(id);
            if (post == null || post.PostOwnerId != User.Identity.GetUserId())
            {
                return this.RedirectToAction("Index", "Home");
            }

            return this.View(post);
        }

        [HttpPost]
        [Route("post/delete/{id}")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePostConfirmed(int id)
        {
            var post = Data.Posts.GetById(id);
            if (post == null || post.PostOwnerId != User.Identity.GetUserId())
            {
                return this.RedirectToAction("Index", "Home");
            }

            Data.Posts.Delete(post);
            Data.SaveChanges();
            return this.RedirectToAction("Index", "Home");
        }


        private Category GetCategoryByMachineName(string machineName)
        {
            return Data.Categories.All().FirstOrDefault(c => !c.IsDisabled && c.MachineName.ToLower().Equals(machineName));
        }

        private IEnumerable<PostViewModel> GetPostsByCategory(int categoryId, int page)
        {

            var getPageFromDb = (page - 1) * PageSize;

            if (getPageFromDb < 0)
            {
                getPageFromDb = 1;
            }

            // TODO Когато заявката иска Пост, който е извън колекцията, да се хвърля правилна грешка, иначе гърми
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