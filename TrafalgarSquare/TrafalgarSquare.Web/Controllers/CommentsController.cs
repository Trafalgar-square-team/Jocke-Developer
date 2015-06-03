using System;
using TrafalgarSquare.Models;

namespace TrafalgarSquare.Web.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.UI.WebControls;
    using Data;
    using ViewModels;
    using ViewModels.User;

    public class CommentsController : BaseController
    {
        public CommentsController(ITrafalgarSquareData data)
            : base(data)
        {
        }

        [HttpGet]
        [Route("Comments/DisplayById/{postId}")]
        public ActionResult DisplayById(int postId)
        {
            if (Request.IsAjaxRequest())
            {
                var commentsFromAjax = Data.Comments.All()
                   .Where(c => c.PostId == postId)
                   .OrderByDescending(c => c.CreatedOn)
                   .Select(c => new CommentViewModel
                   {
                       Id = c.Id,
                       CreatedOn = c.CreatedOn,
                       Text = c.Text,
                       User = new UserViewModel
                       {
                           Id = c.UserId,
                           Username = c.User.UserName,
                           AvatarUrl = c.User.AvatarUrl
                       }
                   })
/*                   .Skip((2 - 1) * 2)
                   .Take(2)*/
                   .ToList();

                return this.PartialView("_CommentsDetailsPartial", commentsFromAjax);
            }

            var comments = Data.Comments.All()
               .Where(c => c.PostId == postId)
               .OrderByDescending(c => c.CreatedOn)
               .Select(c => new CommentViewModel
               {
                   Id = c.Id,
                   CreatedOn = c.CreatedOn,
                   Text = c.Text,
                   User = new UserViewModel
                   {
                       Id = c.UserId,
                       Username = c.User.UserName,
                       AvatarUrl = c.User.AvatarUrl
                   }
               })
               .Take(2)
               .ToList();

            return this.View(comments);         
        }


         [HttpGet]
         [Route("Comments/{postId}")]
         public ActionResult Comments(int postId)
         {
             var comments = Data.Comments.All()
                 .Where(c => c.PostId == postId)
                 .OrderByDescending(c => c.CreatedOn)
                 .Select(c => new CommentViewModel
                 {
                     Id = c.Id,
                     CreatedOn = c.CreatedOn,
                     Text = c.Text,
                     User = new UserViewModel
                     {
                         Id = c.UserId,
                         Username = c.User.UserName,
                         AvatarUrl = c.User.AvatarUrl
                     }
                 })
                 .ToList();

             return this.View(comments);
         }

         [HttpPost]
         public ActionResult CreateComment(string newCommentText, string userId, string userName, string AvatarUrl, int postId)
         {
            /* var commentToCreate = new CommentCreateModel()
             {
                 Text = newCommentText,
                 CreatedOn = DateTime.Now,
                 User = new UserViewModel
                 {
                     Id = userId,
                     Username = userName,
                     AvatarUrl = AvatarUrl
                 },
                 Post = new PostViewModel
                 {
                     Text = "dd",
                     Title = "dd",
                     CreatedDateTime = DateTime.Now,
                      Likes = 2,
                     IsReported = true,
                     User = 

                 }

             };


             Data.Comments.Add(commentToCreate);*/
             Data.SaveChanges();
      

             return RedirectToAction("Index", "Home");
         }
    }
}