namespace TrafalgarSquare.Web.Areas.Admin.Controllers
{
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using Data;
    using Models;

    public class PostsController : BaseController
    {
        public PostsController(ITrafalgarSquareData data)
            : base(data)
        {
        }
        // GET: AdminArea/Posts
        public ActionResult Index()
        {
            var posts = Data.Posts.All().Include(p => p.Category).Include(p => p.PostOwner);
            return View(posts.ToList());
        }

        // GET: AdminArea/Posts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = Data.Posts.GetById(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: AdminArea/Posts/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(Data.Categories.All(), "Id", "Name");
            ViewBag.PostOwnerId = new SelectList(Data.Users.All(), "Id", "Name");
            return View();
        }

        // POST: AdminArea/Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Text,Resource,CategoryId,CreatedDateTime,PostOwnerId,IsReported")] Post post)
        {
            if (ModelState.IsValid)
            {
                Data.Posts.Add(post);
                Data.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(Data.Categories.All(), "Id", "Name", post.CategoryId);
            ViewBag.PostOwnerId = new SelectList(Data.Users.All(), "Id", "Name", post.PostOwnerId);
            return View(post);
        }

        // GET: AdminArea/Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = Data.Posts.GetById(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(Data.Categories.All(), "Id", "Name", post.CategoryId);
            ViewBag.PostOwnerId = new SelectList(Data.Users.All(), "Id", "Name", post.PostOwnerId);
            return View(post);
        }

        // POST: AdminArea/Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Text,Resource,CategoryId,CreatedDateTime,PostOwnerId,IsReported")] Post post)
        {
            if (ModelState.IsValid)
            {
                Data.Posts.Update(post);
                Data.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(Data.Categories.All(), "Id", "Name", post.CategoryId);
            ViewBag.PostOwnerId = new SelectList(Data.Users.All(), "Id", "Name", post.PostOwnerId);
            return View(post);
        }

        // GET: AdminArea/Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = Data.Posts.GetById(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: AdminArea/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = Data.Posts.GetById(id);
            Data.Posts.Delete(post);
            Data.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
