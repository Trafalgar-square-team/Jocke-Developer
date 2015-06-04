namespace TrafalgarSquare.Web.Areas.Admin.Controllers
{
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using System.Web.UI.WebControls;
    using Data;
    using Models;

    public class CommentsController : BaseController
    {
        public CommentsController(ITrafalgarSquareData data)
            : base(data)
        {
        }

        // GET: AdminArea/Comments
        public ActionResult Index()
        {
            var comments = Data.Comments.All().Include(c => c.Post).Include(c => c.User);
            return View(comments.ToList());
        }

        // GET: AdminArea/Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = Data.Comments.GetById(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: AdminArea/Comments/Create
        public ActionResult Create()
        {
            ViewBag.PostId = new SelectList(Data.Posts.All(), "Id", "Title");
            ViewBag.UserId = new SelectList(Data.Users.All(), "Id", "Name");
            return View();
        }

        // POST: AdminArea/Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Text,UserId,PostId,CreatedOn")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                Data.Comments.Add(comment);
                Data.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PostId = new SelectList(Data.Posts.All(), "Id", "Title", comment.PostId);
            ViewBag.UserId = new SelectList(Data.Users.All(), "Id", "Name", comment.UserId);
            return View(comment);
        }

        // GET: AdminArea/Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = Data.Comments.GetById(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.PostId = new SelectList(Data.Posts.All(), "Id", "Title", comment.PostId);
            ViewBag.UserId = new SelectList(Data.Users.All(), "Id", "Name", comment.UserId);
            return View(comment);
        }

        // POST: AdminArea/Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Text,UserId,PostId,CreatedOn")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                Data.Comments.Update(comment);
                Data.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PostId = new SelectList(Data.Posts.All(), "Id", "Title", comment.PostId);
            ViewBag.UserId = new SelectList(Data.Users.All(), "Id", "Name", comment.UserId);
            return View(comment);
        }

        // GET: AdminArea/Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = Data.Comments.GetById(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: AdminArea/Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = Data.Comments.GetById(id);
            Data.Comments.Delete(comment);
            Data.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
