namespace TrafalgarSquare.Web.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using AutoMapper;
    using Data;
    using Microsoft.AspNet.Identity;
    using Models;
    using ViewModels;
    using ViewModels.User;

    public class HomeController : BaseController
    {
        public HomeController(ITrafalgarSquareData data)
            : base(data)
        {
        }

        public ActionResult Index()
        {
            // Takes latest Post for each category
            var latestPostByCategory = Data.Posts.All()
                .GroupBy(post => post.CategoryId)
                .Select(posts => posts.OrderByDescending(x => x.CreatedDateTime))
                .SelectMany(posts => posts.Take(1))
                .GroupJoin(
                    Data.Comments.All(),
                    x => x.Id,
                    postComments => postComments.PostId,
                    (post, postComments) => new
                    {
                        post,
                        postComments = postComments.Count()
                    })
                 .AsEnumerable()
                 .Select(x => Mapper.Map<Post, HomePostViewModel>(
                     x.post,
                     opts =>
                     {
                         //opts.CreateMissingTypeMaps = true;
                         opts.AfterMap((s, d) => d.LikesCount = x.post.LikesPost.Count);
                         opts.BeforeMap((s, d) => d.Owner = Mapper.Map<User, UserViewModel>(x.post.PostOwner));
                     }))
                .ToList();

            var model = new HomeViewModel()
            {
                LatestPostsByCategory = latestPostByCategory,
                TopJokes = TopJokes(10)
            };

            return this.View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return this.View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return this.View();
        }

        public ActionResult PartialTop10Jokes()
        {
            return this.PartialView("Partials/_Top10PostsPartial", this.Top10Jokes());
        }

        [Authorize]
        public int GlobalUnseenMessagesCount()
        {
            var userId = User.Identity.GetUserId();
            var unseenMessagesCount = this.Data.Messages
                .All()
                .Count(x => x.RecepientId == userId && x.IsSeen == false);
            return unseenMessagesCount;
        }

        [Authorize]
        public int GlobalUnseenNotificationsCount()
        {
            var userId = User.Identity.GetUserId();
            var unseenNotificationsCount = this.Data.Notifications
                .All()
                .Count(x => x.RecepientId == userId && x.IsSeen == false);
            return unseenNotificationsCount;
        }
    }
}