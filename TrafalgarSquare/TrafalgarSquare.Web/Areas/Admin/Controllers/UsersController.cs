namespace TrafalgarSquare.Web.Areas.Admin.Controllers
{
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using Data;
    using Models;

    public class UsersController : BaseController
    {

        public UsersController(ITrafalgarSquareData data)
            : base(data)
        {
        }

        // GET: AdminArea/Users
        public ActionResult Index()
        {
            return View(Data.Users.All().ToList());
        }

        // GET: AdminArea/Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = Data.Users.GetById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: AdminArea/Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminArea/Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,AvatarUrl,Birthday,Gender,City,RegisterDate,NotificationId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] User user)
        {
            if (ModelState.IsValid)
            {
                Data.Users.Add(user);
                Data.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: AdminArea/Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = Data.Users.GetById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: AdminArea/Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,AvatarUrl,Birthday,Gender,City,RegisterDate,NotificationId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] User user)
        {
            if (ModelState.IsValid)
            {
                Data.Users.Update(user);
                Data.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: AdminArea/Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = Data.Users.GetById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: AdminArea/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = Data.Users.GetById(id);
            Data.Users.Delete(user);
            Data.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
