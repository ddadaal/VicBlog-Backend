using System;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VicBlog.Models
{
    [DataContract]
    public partial class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DataMember(Name = "id")]
        public string ID { get; set; }

        [DataMember(Name = "articleID")]
        public string ArticleID { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }

        public DateTime SubmitTime { get; set; }

        [DataMember(Name = "submitTime")]
        public long SubmitTimeInUnix
        {
            get
            {
                return SubmitTime.ToUnixUTCTime();
            }
        }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "replyTo")]
        public string ReplyTo { get; set; }
    }
}
