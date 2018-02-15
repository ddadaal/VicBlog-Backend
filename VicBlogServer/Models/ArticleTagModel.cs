using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VicBlogServer.Models
{
    public class ArticleTagModel : ISingleKey<int>
    {
        [Key]
        public int Id { get; set; }

        public int ArticleId { get; set; }

        public string Tag { get; set; }
    }
}
