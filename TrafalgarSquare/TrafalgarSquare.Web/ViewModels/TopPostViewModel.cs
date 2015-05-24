﻿namespace TrafalgarSquare.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using TrafalgarSquare.Models;

    public class TopPostViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public PostResources PostResources { get; set; }

        public string PostOwnerId { get; set; }

        public virtual User PostOwner { get; set; }

        public int LikesCount { get; set; }
    }
}