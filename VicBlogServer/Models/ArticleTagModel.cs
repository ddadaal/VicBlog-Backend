using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VicBlogServer.Models
{
    public class ArticleTagModel
    {
        [Key]
        public int TagId { get; set; }

        [Required]
        public ArticleModel Article { get; set; }

        public string Tag { get; set; }
    }
}
