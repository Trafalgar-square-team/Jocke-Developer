namespace TrafalgarSquare.Web.ViewModels
{
    using System;
    using Automapper;
    using Controllers;
    using Models;
    using TrafalgarSquare.Models;
    using TrafalgarSquare.Web.Automapper;
    using User;

    public class CommentCreateModel : IMapFrom<Comment>
    {
        public string Text { get; set; }

        public virtual UserViewModel User { get; set; }

        public virtual Post Post { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}