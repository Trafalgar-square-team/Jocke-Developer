namespace TrafalgarSquare.Web.ViewModels
{
    using Automapper;
    using Models;
    using User;

    public class TopPostViewModel : IMapFrom<Post>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public PostResources PostResources { get; set; }

        public string PostOwnerId { get; set; }

        public UserViewModel Owner { get; set; }

        public int LikesCount { get; set; }
    }
}