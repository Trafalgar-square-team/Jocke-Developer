
namespace TrafalgarSquare.Web.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Data;
    using Models;

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
            var category = GetCategory(categoryMachineName);
            if (category == null)
            {
                //todo: redirect and add error message
            }

            ViewBag.Title = category.Name;

            var posts = GetPostsByCategory(category.Id, page).ToList();

            return View(posts);
        }



        /* public ActionResult GetPostById(int id)
         {
             var posts = Data.Posts.All()
                 .Where(p => p.Category.Name.Equals("Jokes"))
                 .Select(p => new JokesViewModel
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
                 .FirstOrDefault(post => post.Id == id);

             return this.View(posts);
         }    */

        public Category GetCategory(string machineName)
        {
            return Data.Categories.All().FirstOrDefault(c => !c.IsDisabled && c.MachineName.ToLower().Equals(machineName));
        }
    }
}