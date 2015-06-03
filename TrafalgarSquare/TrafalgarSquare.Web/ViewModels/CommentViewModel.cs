namespace TrafalgarSquare.Web.ViewModels
{
    using System;
    using Automapper;
    using Controllers;
    using Models;
    using User;

    public class CommentViewModel : IMapFrom<Comment>
    {

        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime CreatedOn { get; set; }

        public UserViewModel User { get; set; }
    }
}