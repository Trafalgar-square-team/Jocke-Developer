namespace TrafalgarSquare.Web.ViewModels.User
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using TrafalgarSquare.Web.Automapper;

    public class ChatFriendViewModel : IMapFrom<Models.User>
    {
        public int Number { get; set; }

        public string Id { get; set; }

        public string Username { get; set; }

        public string AvatarUrl { get; set; }

        public bool IsAcceptedFriendShip { get; set; }

        public int UnSeenMessageCount { get; set; }
    }
}