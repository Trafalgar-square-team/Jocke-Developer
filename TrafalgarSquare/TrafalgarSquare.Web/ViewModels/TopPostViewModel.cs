namespace TrafalgarSquare.Web.ViewModels
{
    using Models;

    public class TopPostViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public PostResources PostResources { get; set; }

        public string PostOwnerId { get; set; }

        public virtual Models.User PostOwner { get; set; }

        public int LikesCount { get; set; }
    }
}