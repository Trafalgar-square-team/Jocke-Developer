﻿
namespace TrafalgarSquare.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PostsComments
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }

        public int PostId { get; set; }

        public virtual Post Post { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
