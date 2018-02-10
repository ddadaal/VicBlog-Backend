using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VicBlogServer.Models
{
    public class ArticleLikeModel
    {
        [Key]
        public int Id { get; set; }

        public string ArticleId { get; set; }

        public string Username { get; set; }

        public DateTime LikeTime { get; set; }

    }
}
