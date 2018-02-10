using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.Models;

namespace VicBlogServer.ViewModels
{
    public class ArticleViewModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastEditedTime { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public IEnumerable<ArticleLikeViewModel> Likes { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }

    public class ArticleBriefViewModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

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
