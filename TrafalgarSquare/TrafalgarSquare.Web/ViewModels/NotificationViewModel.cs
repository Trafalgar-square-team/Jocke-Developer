
namespace TrafalgarSquare.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using TrafalgarSquare.Web.ViewModels.User;

    public class NotificationViewModel
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