namespace TrafalgarSquare.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Data;
    using Hubs;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.SignalR;
    using Models;
    using ViewModels.User;
    using System.Threading;

    public class UsersController : BaseController
    {
        public UsersController(ITrafalgarSquareData data)
            : base(data)
        {
        }

        [System.Web.Mvc.Authorize]
        [Route("User/Profile/{username}")]
        public ActionResult Index(string username)
        {
            var user = this.UserProfileData(username);

            return this.View(user);
        }

        [System.Web.Mvc.Authorize]
        [Route("users/topUsers/{showNumber:int?}")]
        public ActionResult TopUsers(int? showNumber)
        {
            if (showNumber == null)
            {
                showNumber = 10;
            }

            //var usersWithRank = this.Data.Users.All()
            //    .GroupBy(z => z.Posts.SelectMany(x => x.LikesPost).Count())
            //    .OrderByDescending(z => z.Key)
            //    .AsEnumerable()
            //    .SelectMany((grouping, i) => grouping.Select(s => new TopUserViewModel()
            //    {
            //        Id = s.Id,
            //        AvatarUrl = s.AvatarUrl,
            //        Username = s.UserName,
            //        Rank = i + 1,
            //        TotalLikes = grouping.Key
            //    }))
            //    .ToList();

            var usersWithRank = this.Data.Users.All()
                .GroupBy(z => z.Posts.SelectMany(x => x.LikesPost).Count())
                .OrderByDescending(z => z.Key)
                .AsEnumerable()
                .SelectMany(
                    (grouping, rank) => grouping.Select(user => AutoMapper.Mapper.Map<User, TopUserViewModel>(
                        user,
                        opt =>
                        {
                            opt.BeforeMap((src, dest) => dest.Rank = rank + 1);
                            opt.AfterMap((src, dest) => dest.TotalLikes = grouping.Key);
                        })))
                .ToList();

            return this.View(usersWithRank);
        }

        [System.Web.Mvc.Authorize]
        [HttpPost]
        [Route("users/addFriend")]
        public ActionResult AddFriend(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id", "Invalid user ID.");
            }

            var friendToAdd = this.Data.Users
                .All()
                .FirstOrDefault(x => x.Id == id);

            if (friendToAdd == null)
            {
                throw new ArgumentNullException("id", "Invalid user ID.");
            }

            var adderUserId = User.Identity.GetUserId();
            var userFriend = this.Data
                .UsersFriends.All()
                .FirstOrDefault(x => (x.UserId == adderUserId && x.FriendId == friendToAdd.Id));

            // Check if it is already added
            if (userFriend != null)
            {
                userFriend.IsAccepted = true;
                userFriend.SentFriendRequestDate = DateTime.Now;
            }
            else
            {
                this.Data.UsersFriends.Add(new UserFriends()
                {
                    UserId = adderUserId,
                    FriendId = id,
                    Friend = friendToAdd,
                    IsAccepted = true,
                    SentFriendRequestDate = DateTime.Now
                });
            }

            Notification model;
            var theChat = GlobalHost.ConnectionManager.GetHubContext<Chat>();

            // Send Friend Request.
            var frinedRequest = this.Data.UsersFriends
                .All()
                .FirstOrDefault(x => (x.UserId == friendToAdd.Id && x.FriendId == adderUserId));

            if (frinedRequest != null)
            {
                if (frinedRequest.IsAccepted == false)
                {
                    // TODO use SignalR To send notifiacation (to Current User)
                    model = new Notification()
                    {
                        RecepientId = adderUserId,
                        Text = string.Format("You sent friend request to {0}. It is waiting acceptance.", friendToAdd.UserName),
                        SendDateTime = DateTime.Now,
                        SenderId = adderUserId,
                        IsSeen = false
                    };
                }
                else
                {
                    // TODO use SignalR To send notifiacation  (to Current and Friend User)
                    model = new Notification()
                    {
                        RecepientId = adderUserId,
                        Text = string.Format("You are now friends with {0}", friendToAdd.UserName),
                        SendDateTime = DateTime.Now,
                        IsSeen = false,
                        SenderId = friendToAdd.Id
                    };

                    var not = new Notification()
                    {
                        RecepientId = friendToAdd.Id,
                        Text = string.Format("You are now friends with {0}", User.Identity.GetUserName()),
                        SendDateTime = DateTime.Now,
                        IsSeen = false,
                        SenderId = adderUserId
                    };

                    this.Data.Notifications.Add(not);
                    theChat.Clients.User(model.SenderId).addNotification();
                }
            }
            else
            {
                this.Data.UsersFriends.Add(new UserFriends()
                {
                    UserId = friendToAdd.Id,
                    FriendId = adderUserId,
                    IsAccepted = false,
                    SentFriendRequestDate = DateTime.Now
                });

                // TODO use SignalR To send notifiacation  (to Current and Friend User)
                model = new Notification()
                {
                    RecepientId = friendToAdd.Id,
                    Text = string.Format("You have friend request from <a href='/User/Profile/{0}'>{0}</a>", User.Identity.GetUserName()),
                    SendDateTime = DateTime.Now,
                    SenderId = adderUserId,
                    IsSeen = false
                };
            }

            theChat.Clients.User(model.RecepientId).addNotification();

            this.Data.Notifications.Add(model);
            this.Data.SaveChanges();
            return new HttpStatusCodeResult(200);
            return this.PartialView(model);
        }

        [Route("users/AcceptFriendRequest/{id}")]
        [System.Web.Mvc.Authorize]
        public ActionResult AcceptFriendRequest(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id", "Invalid user ID.");
            }

            var newFriend = this.Data.Users
                .All()
                .FirstOrDefault(x => x.Id == id);

            if (newFriend == null)
            {
                throw new ArgumentNullException("id", "Invalid user ID.");
            }

            var acceptorUserId = User.Identity.GetUserId();
            var userFriend = this.Data
                .UsersFriends.All()
                .FirstOrDefault(x => (x.UserId == acceptorUserId && x.FriendId == newFriend.Id));

            // Check if they are already friends
            if (userFriend != null)
            {
                if (userFriend.IsAccepted == true)
                {
                    throw new Exception("You are alredy friends.");
                }

                // accept request again for delted friend
                userFriend.IsAccepted = true;
                userFriend.SentFriendRequestDate = DateTime.Now;
            }
            else
            {
                var isValidRequest =
                    this.Data.UsersFriends
                    .All()
                    .Any(x => x.UserId == id && x.FriendId == acceptorUserId && x.IsAccepted == true);

                if (!isValidRequest)
                {
                    throw new Exception("Invalid friend request!");
                }
            }

            this.Data.SaveChanges();
            return this.View();
        }

        [Route("users/RemoveFriend/{id}")]
        [System.Web.Mvc.Authorize]
        public ActionResult RemoveFriend(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id", "Invalid user ID.");
            }

            var frinedForRemove = this.Data.Users
                .All()
                .FirstOrDefault(x => x.Id == id);

            if (frinedForRemove == null)
            {
                throw new ArgumentNullException("id", "Invalid user ID.");
            }

            var userId = User.Identity.GetUserId();
            var userFriend = this.Data
                .UsersFriends.All()
                .FirstOrDefault(x => (x.UserId == userId && x.FriendId == frinedForRemove.Id));

            // Check if they are already friends
            if (userFriend == null || userFriend.IsAccepted != true)
            {
                throw new Exception("You are not friends and cannot remove from friends.");
            }

            userFriend.IsAccepted = false;

            this.Data.SaveChanges();
            return new EmptyResult();
        }

        [Route("users/GetFriends/{id}")]
        [System.Web.Mvc.Authorize]
        public ActionResult GetFriends(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id", "Invalid username.");
            }

            var frinedForRemove = this.Data.Users
                .All()
                .FirstOrDefault(x => x.UserName == id);

            if (frinedForRemove == null)
            {
                throw new ArgumentNullException("id", "Invalid username.");
            }

            var friends = this.Data.UsersFriends.All()
                .Where(x => x.User.UserName == id && x.IsAccepted == true)
                .OrderBy(x => x.Friend.UserName)
                .AsEnumerable()
                .Select((x, i) => new FriendViewModel()
                {
                    Number = i,
                    Id = x.FriendId,
                    AvatarUrl = x.Friend.AvatarUrl,
                    Username = x.Friend.UserName,
                    IsAcceptedFriendShip = x.Friend.Friends.Any(z => z.FriendId == x.UserId && z.IsAccepted == true)
                });


            if (User.Identity.GetUserId() != id)
            {
                friends = friends.Where(x => x.IsAcceptedFriendShip == true);
            }

            friends = friends.ToList();

            if (Request.IsAjaxRequest())
            {
                return this.PartialView(friends);
            }

            return this.View(friends);
        }

        [Route("users/GetFriendStatus/{id}")]
        [System.Web.Mvc.Authorize]
        public JsonResult GetFriendStatus(string id)
        {
            var user = this.UserProfileData(id);

            return this.Json(user, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult IsNameTaken(string username)
        {
            var users = this.Data
                .Users
                .All()
                .Where(x => x.UserName == username);

            return this.Json(!users.Any());
        }

        [Route("users/UserProfileData/{username}")]
        private UserProfileViewModel UserProfileData(string username)
        {
            var userId = User.Identity.GetUserId();
            //var user = this.Data.Users
            //    .All()
            //    .Where(x => x.UserName == username)
            //    .Select(x => new UserProfileViewModel()
            //    {
            //        Id = x.Id,
            //        AvatarUrl = x.AvatarUrl,
            //        Username = x.UserName,
            //        Email = x.Email,
            //        Birthday = x.Birthday,
            //        City = x.City,
            //        Gender = x.Gender.ToString(),
            //        Name = x.Name,
            //        RegisterDate = x.RegisterDate,
            //        PostCount = x.Posts.Count(),
            //        CommentsCount = x.Comments.Count(),
            //        IsOwned = userId == x.Id,
            //    })
            //    .FirstOrDefault();

            var user = this.Data.Users
                .All()
                .Where(x => x.UserName == username)
                .AsEnumerable()
                .Select(x => AutoMapper.Mapper.Map<User, UserProfileViewModel>(
                        x,
                        opt =>
                        {
                            opt.AfterMap((src, dest) => dest.IsOwned = userId == x.Id);
                            opt.AfterMap((src, dest) => dest.Gender = x.Gender.ToString());
                        }))
               .FirstOrDefault();

            if (user == null)
            {
                throw new Exception("No user with such username.");
            }

            // Check if viewer is friend with this user
            if (User.Identity.GetUserName() != username)
            {
                user.IsViewerFriend = this.Data.UsersFriends
                    .All()
                    .Count(x => (x.UserId == userId && x.Friend.UserName == username && x.IsAccepted == true) ||
                                (x.User.UserName == username && x.Friend.Id == userId && x.IsAccepted == true)) == 2;
                if (!user.IsViewerFriend)
                {
                    user.IsViewerWaitingAcceptance = this.Data.UsersFriends
                        .All()
                        .Count(x => (x.UserId == userId && x.Friend.UserName == username && x.IsAccepted == true) ||
                                    (x.User.UserName == username && x.Friend.Id == userId && x.IsAccepted == false)) == 2;
                }
            }

            return user;
        }
    }
}