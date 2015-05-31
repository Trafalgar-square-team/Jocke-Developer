using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrafalgarSquare.Web.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public class AddCommentViewModel
    {
        [Required]
        public int PostId { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Comment must be at least 5 characters,")]
        public string Text { get; set; }
    }
}