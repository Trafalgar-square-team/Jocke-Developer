namespace TrafalgarSquare.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Data;
    using Microsoft.AspNet.Identity;
    using TrafalgarSquare.Models;
    using TrafalgarSquare.Web.ViewModels;
    using TrafalgarSquare.Web.ViewModels.User;
    using System.Threading;

    public class UsersController : BaseController
    {
        public UsersController(ITrafalgarSquareData data)
            : base(data)
        {
        }

        [Authorize]
        public ActionResult Index(string username)
        {
            var user = this.UserProfileData(username);

            return this.View(user);
        }

        [Authorize]
        public ActionResult TopUsers(int? showNumber)
        {
            if (showNumber == null)
            {
                showNumber = 10;
            }

            var users = this.Data.Users
                .All()
                .GroupJoin(
                    this.Data.PostsLikes.All(),
                    x => x.Id,
                    postLikes => postLikes.Post.PostOwnerId,
                    (user, postLikes) => new
                    {
                        user = user,
                        postLikes = postLikes.Count()
                    })
                .OrderByDescending(x => x.postLikes)
                .ThenBy(x => x.user.UserName)
                .Take((int)showNumber)
                .Select(x => new TopUserViewModel()
                {
                    Id = x.user.Id,
                    AvatarUrl = x.user.AvatarUrl,
                    Username = x.user.UserName,
                    TotalLikes = x.postLikes
                }).ToList();

            return this.View(users);
        }

        [Authorize]
        [HttpPost]
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
                        Text = "Your friend request is waiting acceptance.",
                        SendDateTime = DateTime.Now
                    };
                }
                else
                {
                    // TODO use SignalR To send notifiacation (to Current User)
                    model = new Notification()
                    {
                        RecepientId = adderUserId,
                        Text = string.Format("You are now friends with {0}", friendToAdd.UserName),
                        SendDateTime = DateTime.Now
                    };
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
                    RecepientId = adderUserId,
                    Text = string.Format("Friend request sent to: {0}", friendToAdd.UserName),
                    SendDateTime = DateTime.Now
                };
            }

            this.Data.SaveChanges();
            return this.PartialView(model);
        }

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize]
        public JsonResult GetFriendStatus(string id)
        {
            var user = this.UserProfileData(id);

            return this.Json(user, JsonRequestBehavior.AllowGet);
        }

        private UserProfileViewModel UserProfileData(string username)
        {
            var userId = User.Identity.GetUserId();
            var user = this.Data.Users
                .All()
                .Where(x => x.UserName == username)
                .Select(x => new UserProfileViewModel()
                {
                    Id = x.Id,
                    AvatarUrl = x.AvatarUrl,
                    Username = x.UserName,
                    Email = x.Email,
                    Birthday = x.Birthday,
                    City = x.City,
                    Gender = x.Gender.ToString(),
                    Name = x.Name,
                    RegisterDate = x.RegisterDate,
                    PostCount = x.Posts.Count(),
                    CommentsCount = x.Comments.Count(),
                    IsOwned = userId == x.Id,
                })
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

        public ActionResult IsNameTaken(string username)
        {
            Thread.Sleep(3000);
            var users = this.Data
                .Users
                .All()
                .Where(x => x.UserName == username);

            return Json(!users.Any());
        }
    }
}