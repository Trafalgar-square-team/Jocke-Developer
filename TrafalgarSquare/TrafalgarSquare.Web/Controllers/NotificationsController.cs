namespace TrafalgarSquare.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Data;
    using Microsoft.AspNet.Identity;
    using TrafalgarSquare.Web.ViewModels;
    using TrafalgarSquare.Web.ViewModels.User;

    public class NotificationsController : BaseController
    {
        public NotificationsController(ITrafalgarSquareData data)
            : base(data)
        {
        }

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var notifications = this.Data.Notifications
                .All()
                .Where(x => x.RecepientId == userId)
                .OrderByDescending(x => x.SendDateTime)
                .Select(x => new NotificationViewModel()
                {
                    Id = x.Id,
                    Text = x.Text,
                    SendDateTime = x.SendDateTime,
                    RecepientId = x.RecepientId,
                    SenderId = x.SenderId,
                    Sender = new UserViewModel()
                    {
                        Id = x.Sender.Id,
                        AvatarUrl = x.Sender.AvatarUrl,
                        Username = x.Sender.UserName
                    },
                    IsSeen = x.IsSeen
                })
                .ToList();

            return this.View(notifications);
        }

        [HttpPost]
        [Authorize]
        public ActionResult MarkNotificationAsRead(int id)
        {
            var notification = this.Data.Notifications
                .All()
                .FirstOrDefault(x => x.Id == id);

            if (notification == null)
            {
                throw new ArgumentNullException("id", "Invalid notification id!");
            }

            if (notification.RecepientId != User.Identity.GetUserId() && !User.IsInRole("Administrator"))
            {
                throw new ArgumentNullException("id", "That's not your notification. You cannot mark it as read!");
            }

            notification.IsSeen = true;
            this.Data.SaveChanges();

            return new HttpStatusCodeResult(200);
        }
    }
}