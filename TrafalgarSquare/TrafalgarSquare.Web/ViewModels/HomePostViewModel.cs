namespace TrafalgarSquare.Web.ViewModels
{
    using System;
    using Automapper;
    using Models;
    using User;

    public class HomePostViewModel : IMapFrom<Post>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public PostResources PostResources { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public string PostOwnerId { get; set; }

        public virtual UserViewModel Owner { get; set; }

        public int CommentsCount { get; set; }

        public int LikesCount { get; set; }
    }
}