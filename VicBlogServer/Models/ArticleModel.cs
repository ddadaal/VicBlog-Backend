using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.Utils;

namespace VicBlogServer.Models
{
    public class ArticleModel : ISingleKey<int>
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastEditedTime { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }
    }


}
