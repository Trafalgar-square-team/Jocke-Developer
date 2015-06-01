namespace TrafalgarSquare.Web.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Data;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.SignalR;
    using Models;
    using Newtonsoft.Json;


    public class Chat : Hub
    {
        private ITrafalgarSquareData data;

        public Chat()
            : this(new TrafalgarSquareData(new TrafalgarSquareDbContext()))
        {
        }

        public Chat(ITrafalgarSquareData data)
        {
            this.Data = data;
        }


        protected ITrafalgarSquareData Data
        {
            get { return this.data; }
            private set { this.data = value; }
        }

        [Authorize]
        public void SendPersonMessage(string userId, string message)
        {
            // Clients.User(userId).addMessage(message);
            var user = Context.User;
            var senderId = user.Identity.GetUserId();
            var sender = this.Data.Users.All().FirstOrDefault(x => x.Id == senderId);

            // TODO: Check if recipient is Friend and then send message
            var isSenderFriend = this.Data.UsersFriends
                .All()
                .Count(x => (x.UserId == userId && x.Friend.Id == senderId && x.IsAccepted == true) ||
                            (x.User.Id == senderId && x.Friend.Id == userId && x.IsAccepted == true)) == 2;
            if (isSenderFriend)
            {
                Clients.User(userId).addMessage(JsonConvert.SerializeObject(new
                {
                    userId = senderId,
                    message
                }));

                Clients.User(userId).addMessageNotification();

                this.Data.Messages.Add(new Message()
                {
                    RecepientId = userId,
                    SenderId = senderId,
                    IsSeen = false,
                    Text = message,
                    SendDateTime = DateTime.Now,
                });

                this.Data.SaveChanges();
            }
            else
            {
                //Clients.User(senderId).invalidRecipient();
            }
        }

        [Authorize]
        public void SendPersonalNotifications(string userId)
        {
            Clients.User(userId).addNotification();
        }

        public void SendMessage(string message)
        {
            var msg = string.Format("{0}: {1}", Context.ConnectionId, message);
            Clients.All.addMessage(msg);
        }

        public void JoinRoom(string room)
        {
            Groups.Add(Context.ConnectionId, room);
            Clients.Caller.joinRoom(room);
        }

        public void SendMessageToRoom(string message, string[] rooms)
        {
            var msg = string.Format("{0}: {1}", Context.ConnectionId, message);

            for (int i = 0; i < rooms.Length; i++)
            {
                Clients.Group(rooms[i]).addMessage(msg);
            }
        }
    }
}