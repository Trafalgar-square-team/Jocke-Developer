

namespace TrafalgarSquare.Web.ViewModels.User
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using TrafalgarSquare.Web.Automapper;

    public class TopUserViewModel : IMapFrom<Models.User>
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string AvatarUrl { get; set; }

        public int TotalLikes { get; set; }

        public int Rank { get; set; }
    }
}