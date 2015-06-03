namespace TrafalgarSquare.Web.ViewModels.User
{
    using Automapper;
    using Models;
    using System;

    public class UserViewModel : IMapFrom<User>
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string AvatarUrl { get; set; }

        public string Email { get; set; }
        public string City { get; set; }

        public DateTime? Birthday { get; set; }

        public Gender? Gender { get; set; }
    }
}