namespace TrafalgarSquare.Web.ViewModels.Message
{
    using System;
    using TrafalgarSquare.Web.ViewModels.User;

    public class MessageViewModel
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string SenderId { get; set; }

        public DateTime SendDateTime { get; set; }

        public virtual UserViewModel Sender { get; set; }

        public string RecepientId { get; set; }

        public virtual UserViewModel Recepient { get; set; }
    }
}