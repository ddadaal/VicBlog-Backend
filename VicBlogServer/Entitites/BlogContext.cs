using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.Models;

namespace VicBlogServer.Entitites
{
    public class BlogContext : IdentityDbContext<User, Role, string>
    {
        public DbSet<Article> Articles { get; set; }

        public BlogContext(DbContextOptions<BlogContext> options): base(options) { }
    }
}
