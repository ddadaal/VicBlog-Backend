using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.Models;

namespace VicBlogServer.ViewModels
{
    public class ArticleViewModel
    {
        public int ArticleId { get; set; }

        public string Author { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastEditedTime { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }

    public class ArticleListViewModel
    {
   
        public PagingInfo PagingInfo { get; set; }

        public IEnumerable<ArticleBriefViewModel> List { get; set; }
    }

    public class ArticleBriefViewModel
    {
        public int ArticleId { get; set; }

        public string Author { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastEditedTime { get; set; }

        public string Title { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public long LikeCount { get; set; }

        public long CommentCount { get; set; }
    }

    public class ArticleMinimal
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
