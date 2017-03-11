using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Jose;
using VicBlog.Data;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VicBlog.Models
{
    public enum Role { User, Admin, Unclear }

    [DataContract]
    public class User
    {
        [DataMember(Name = "username")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Username { get;set;}

        public string Password { get;set; }

        [DataMember(Name = "role")]
        public Role Role { get;set; }

        public DateTime RegisterTime { get;set;}

        [DataMember(Name = "registerTime")]
        public long UTCJSRegisterTime
        {
            get
            {
                return RegisterTime.ToUnixUTCTime();
            }
            set
            {
                RegisterTime = Utils.ToLocalDateTime(value);
            }
        }

        public string ComputeToken(DateTime? loginTime)
        {
            try
            {
                return JWT.Encode(new TokenModel() { Username = Username, LoginTime = loginTime ?? DateTime.Now }, Encoding.ASCII.GetBytes(Utils.USER_TOKEN_KEY), Utils.ALGORITHM);

            }
            catch
            {
                return null;
            }
        }

        

    }
}