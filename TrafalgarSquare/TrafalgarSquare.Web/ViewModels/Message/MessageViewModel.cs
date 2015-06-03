namespace TrafalgarSquare.Web.ViewModels.Message
{
    using System;
    using Automapper;
    using User;

    public class MessageViewModel : IMapFrom<Models.Message>
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string SenderId { get; set; }

        public DateTime SendDateTime { get; set; }

        public UserViewModel Sender { get; set; }

        public string RecepientId { get; set; }

        public UserViewModel Recepient { get; set; }
    }
}