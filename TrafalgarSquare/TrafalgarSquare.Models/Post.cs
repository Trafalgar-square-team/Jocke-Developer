namespace TrafalgarSquare.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Post
    {
        private ICollection<ReportedPosts> _usersReportedPosts;
        private ICollection<PostLikes> _likesPost;
        private ICollection<Comment> comments;

        public Post()
        {
            _usersReportedPosts = new HashSet<ReportedPosts>();
            _likesPost = new HashSet<PostLikes>();
            comments = new HashSet<Comment>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string Text { get; set; }

        [Required]
        public PostResources Resource { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        [Required]
        public DateTime CreatedDateTime { get; set; }

        public string PostOwnerId { get; set; }

        public virtual User PostOwner { get; set; }

        public virtual ICollection<ReportedPosts> UsersReportedPosts
        {
            get { return this._usersReportedPosts; }
            set { this._usersReportedPosts = value; }
        }

        public virtual ICollection<PostLikes> LikesPost
        {
            get { return this._likesPost; }
            set { this._likesPost = value; }
        }

        public virtual ICollection<Comment> Comments
        {
            get { return this.comments; }
            set { this.comments = value; }
        }

        public bool? IsReported { get; set; }
    }
}
