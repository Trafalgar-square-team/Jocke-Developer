using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrafalgarSquare.Models;
using TrafalgarSquare.Web.ViewModels.User;

namespace TrafalgarSquare.Web.ViewModels
{
    public class PostCreateBindModel
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public string Resource { get; set; }
      
        public string PostOwnerId { get; set; }

        public int CategoryId { get; set; }

        public bool IsVideo { get; set; }


    }
}