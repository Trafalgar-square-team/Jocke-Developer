using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrafalgarSquare.Models;
using TrafalgarSquare.Web.ViewModels.User;

namespace TrafalgarSquare.Web.ViewModels
{
    public class PostViewModel
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