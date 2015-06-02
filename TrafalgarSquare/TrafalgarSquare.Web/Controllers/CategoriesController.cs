using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrafalgarSquare.Data;
using TrafalgarSquare.Web.ViewModels;

namespace TrafalgarSquare.Web.Controllers
{
    [Authorize]
    public class CategoriesController : BaseController
    {
        public CategoriesController(ITrafalgarSquareData data)
            : base(data)
        {
        }


        [Route("Categories/DeletePost/{postId}")]
        public ActionResult DeletePost(int postId)
        {
            base.DeletePostInCategorie(postId);

            return this.Redirect("/");
        }

       
       
    }
}