using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlog.Models;

namespace VicBlog
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UserAuthenticationAttribute : Attribute
    {
        public Role LeastRole { get; set; }
        public UserAuthenticationAttribute(Role leastRole)
        {
            LeastRole = leastRole;
        }
    }
}
