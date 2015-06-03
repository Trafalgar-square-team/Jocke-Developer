namespace TrafalgarSquare.Web.ViewModels
{
    using System;
    using Automapper;
    using Models;
    using User;

    public class PostViewModel : IMapFrom<Post>
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string Title { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public int Likes { get; set; }

        public bool IsReported { get; set; }

        public UserViewModel User { get; set; }

        public PostResources Resource { get; set; }

        public int CommentsCount { get; set; }
    }
}