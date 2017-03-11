using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using VicBlog.Data;
using System.Threading.Tasks;

namespace VicBlog.Models
{
    [DataContract]
    public class ArticlePatchingModel
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "content")]

        public string Content { get; set; }
        [DataMember(Name = "category")]
        public string Category { get; set; }
        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }
    }

    [DataContract]
    public class ArticleCreatingModel
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "content")]
        public string Content { get; set; }
        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }
        [DataMember(Name = "category")]
        public string Category { get; set; }
        [DataMember(Name = "rate")]
        public double Rate { get; set; }

    }

    [DataContract]
    public class CommentCreatingModel
    {
        [DataMember(Name = "articleID")]
        public string ArticleID { get; set; }
        [DataMember(Name = "content")]
        public string Content { get; set; }
        [DataMember(Name = "replyTo")]
        public string ReplyToCommentID { get; set; }
    }

    [DataContract]
    public class ArticleBriefRequestModel
    {
        [DataMember(Name = "id")]
        public string ID { get; set; }
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "submitTime")]
        public long SubmitTime { get; set; }
        [DataMember(Name = "rate")]
        public double Rate { get; set; }
        [DataMember(Name = "lastEditedTime")]
        public long LastEditedTime { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }
        [DataMember(Name = "category")]
        public string Category { get; set; }

        public ArticleBriefRequestModel(ArticleBrief brief)
        {
            ID = brief.ID;
            Username = brief.Username;
            SubmitTime = brief.SubmitTime.ToUnixUTCTime();
            LastEditedTime = brief.LastEditedTime.ToUnixUTCTime();
            Title = brief.Title;
            Category = brief.Category;
            Tags = brief.Tags;
            Rate = brief.Rate;
        }
    }

    [DataContract]
    public class UserLoginModel
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "password")]
        public string Password { get; set; }
    }

    [DataContract]
    public class UserLoginSuccessModel
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "role")]
        public Role Role { get; set; }

        public DateTime LoginTime { get; set; }
        [DataMember(Name = "loginTime")]
        public long UTCTimestampLoginTime
        {
            get
            {
                return this.LoginTime.ToUnixUTCTime();
            }
        }
        [DataMember(Name = "token")]
        public string Token { get; set; }
    }
    [DataContract]
    public class TokenModel
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }

        public DateTime LoginTime { get; set; }
        [DataMember(Name = "loginTime")]
        public long LoginTimeInUnixUTC
        {
            get
            {
                return this.LoginTime.ToUnixUTCTime();
            }
            set
            {
                LoginTime = value.ToLocalDateTime();
            }
        }
    }

    [DataContract]
    public class UserRegisterModel
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "password")]
        public string Password { get; set; }
    }

    [DataContract]
    public class RatePostModel
    {
        [DataMember(Name = "score")]
        public double Score { get; set; }
    }


    [DataContract]
    public class ArticleRequestModel
    {
        [DataMember(Name = "id")]
        public string ID { get; set; }
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "submitTime")]
        public long SubmitTime { get; set; }
        [DataMember(Name = "lastEditedTime")]
        public long LastEditedTime { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "category")]
        public string Category { get; set; }
        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }
        [DataMember(Name = "content")]
        public string Content { get; set; }
        [DataMember(Name = "rate")]
        public double Rate { get; set; }

        public ArticleRequestModel(ArticleBrief brief, string content)
        {
            ID = brief.ID;
                Username = brief.Username;
            SubmitTime = brief.SubmitTime.ToUnixUTCTime();
            LastEditedTime = brief.LastEditedTime.ToUnixUTCTime();
            Title = brief.Title;
            Tags = brief.Tags;
                Category = brief.Category;
            Content = content;
            Rate = brief.Rate;
        }


    }

}
