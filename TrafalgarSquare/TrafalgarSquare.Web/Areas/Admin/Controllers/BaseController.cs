namespace TrafalgarSquare.Web.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Data;
    using Models;

    [Authorize(Roles = "Administrator")]
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

        protected override void OnException(ExceptionContext filterContext)
        {
            // base.OnException(filterContext);
            this.RedirectToAction("Index", "Error");
        }
    }
}