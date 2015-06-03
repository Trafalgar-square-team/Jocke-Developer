namespace TrafalgarSquare.Web.ViewModels
{
    using System;
    using Automapper;
    using Models;
    using User;

    public class NotificationViewModel : IMapFrom<Notification>
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string RecepientId { get; set; }

        public string SenderId { get; set; }

        public UserViewModel Sender { get; set; }

        public DateTime SendDateTime { get; set; }

        public bool IsSeen { get; set; }
    }
}