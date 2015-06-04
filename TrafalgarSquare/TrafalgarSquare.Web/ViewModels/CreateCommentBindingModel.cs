namespace TrafalgarSquare.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using TrafalgarSquare.Models;

    public class CreateCommentBindingModel
    {

        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string CommentText { get; set; }

        [Required]
        public int PostId { get; set; }
    }
}