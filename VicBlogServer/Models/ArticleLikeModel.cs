using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VicBlogServer.Models
{
    public class ArticleLikeModel : ISingleKey<int>
    {
        [Key]
        public int Id { get; set; }

        public int ArticleId { get; set; }

        public string Username { get; set; }

        public DateTime LikeTime { get; set; }

    }
}
