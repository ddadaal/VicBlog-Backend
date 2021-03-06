﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.Utils;

namespace VicBlogServer.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }

        public int ArticleId { get; set; }

        public string Username { get; set; }

        public DateTime SubmitTime { get; set; }

        public string Content { get; set; }
    }

    public class CommentMinimal
    {    
        public string Content { get; set; }
    }

    public class CommentListViewModel
    {
        public PagingInfo PagingInfo { get; set; }
        
        public IEnumerable<CommentViewModel> List { get; set; }
    }


}
