namespace TrafalgarSquare.Web.ViewModels.User
{
    using Automapper;
    using Models;

    public class UserViewModel : IMapFrom<User>
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string AvatarUrl { get; set; }

        public string Email { get; set; }

    }
}