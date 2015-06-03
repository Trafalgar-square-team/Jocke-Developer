namespace TrafalgarSquare.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Regular Error";
            return this.View();
        }

        public ActionResult NotFound404()
        {
            ViewBag.Title = "Error 404 - File not Found";
            return this.View("Index");
        }
    }
}