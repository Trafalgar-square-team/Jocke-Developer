namespace TrafalgarSquare.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Data;
    using Microsoft.AspNet.Identity;
    using ViewModels;
    using ViewModels.Message;
    using ViewModels.User;
    using WebGrease.Css.Extensions;

    public class MessagesController : BaseController
    {
        public MessagesController(ITrafalgarSquareData data)
            : base(data)
        {
        }

        [Authorize]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var usersFriends = this.Data.UsersFriends.All()
                .Where(x => x.UserId == userId && x.IsAccepted == true && x.Friend.Friends.Any(z => z.FriendId == userId && z.IsAccepted == true))
                .Select(x => new ChatFriendViewModel()
                {
                    Id = x.FriendId,
                    Username = x.Friend.UserName,
                    AvatarUrl = x.Friend.AvatarUrl,
                    IsAcceptedFriendShip = x.Friend.Friends.Any(z => z.FriendId == userId && z.IsAccepted == true),
                    UnSeenMessageCount = x.Friend.Messages.Count(y => y.RecepientId == userId && y.IsSeen == false)
                }).ToList();


            return this.View(usersFriends);
        }

        [Authorize]
        [HttpPost]
        public ActionResult FromUser(string senderId)
        {
            var userId = User.Identity.GetUserId();
            var allMessages = this.Data.Messages.All()
                .Where(x => (x.SenderId == senderId && x.RecepientId == userId) || (x.SenderId == userId && x.RecepientId == senderId))
                .OrderBy(x => x.SendDateTime);
            var model = allMessages.Select(x => new MessageViewModel()
                {
                    Id = x.Id,
                    Sender = new UserViewModel()
                    {
                        Id = x.Sender.Id,
                        AvatarUrl = x.Sender.AvatarUrl,
                        Username = x.Sender.UserName
                    },
                    SendDateTime = x.SendDateTime,
                    Text = x.Text,
                    SenderId = x.SenderId,
                }).ToList();

            allMessages.Where(x => x.SenderId == senderId).ForEach(s => s.IsSeen = true);
            this.Data.SaveChanges();

            return this.PartialView(model);
        }
    }
}