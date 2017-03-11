using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Runtime.Serialization;

namespace VicBlog.Models
{
    [DataContract]
    public class Rate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DataMember(Name = "articleID")]
        public string ArticleID { get; set; }

        [DataMember(Name = "rate")]
        public double Score { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "submitTime")]
        public long SubmitTimeInUnix
        {
            get
            {
                return this.SubmitTime.ToUnixUTCTime();
            }
        }

        public DateTime SubmitTime { get; set; }

    }
}