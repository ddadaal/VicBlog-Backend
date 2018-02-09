using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VicBlogServer.Models
{
    public class User : IdentityUser
    {
        
    }

    public class Role: IdentityRole
    {
        public const string Admin = "Admin";
        public const string User = "User";

        public Role() { }
        public Role(string roleName) : base(roleName) { }
    }
}
