using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.Utils;

namespace VicBlogServer.Models
{
    public class Article
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; }

        public string Username { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastEditedTime { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string FormattedTags { get; set; }

        [NotMapped]
        public IEnumerable<string> Tags
        {
            get
            {
                return FormattedTags.Split(",");
            }
            set
            {
                FormattedTags = string.Join(",", value);
            }
        }

        public long Like { get; set; }
    }

    public class ArticleBrief
    {
        public string ID { get; set; }

        public string Username { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastEditedTime { get; set; }

        public string Title { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public long Like { get; set; }

        public long CommentCount { get; set; }

        public ArticleBrief(Article article, long commentCount)
        {
            ID = article.ID;
            Username = article.Username;
            CreateTime = article.CreateTime;
            LastEditedTime = article.LastEditedTime;
            Title = article.Title;
            Tags = article.Tags;
            Like = article.Like;
            CommentCount = commentCount;

        }
    }
}
