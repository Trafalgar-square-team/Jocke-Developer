using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using TrafalgarSquare.Models;

namespace TrafalgarSquare.Web.ViewModels
{
    public class PostLikesViewModel
    {

        public static Expression<Func<PostLikes, PostLikesViewModel>> ViewModel
        {
            get
            {
                return x => new PostLikesViewModel()
                {
                    PostId = x.PostId,
                    UserId = x.User.Id,
                    User = x.User,
                    Post = x.Post,
                    LikedDateTime = x.LikedDateTime
                };
            }
        }
        public string UserId { get; set; }

        public Models.User User { get; set; }

        public int PostId { get; set; }

        public Post Post { get; set; }

        public DateTime LikedDateTime { get; set; }
    }
}