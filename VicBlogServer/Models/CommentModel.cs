using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.Utils;

namespace VicBlogServer.Models
{
    public partial class CommentModel
    {
        [Key]
        public int ID { get; set; }

        public string ArticleId { get; set; }

        public string Username { get; set; }

        public DateTime SubmitTime { get; set; }

        [NotMapped]
        public long SubmitTimeInUnix => SubmitTime.ToUnixUTCTime();

        public string Content { get; set; }
    }
}
