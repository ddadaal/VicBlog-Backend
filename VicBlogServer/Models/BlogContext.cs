using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace VicBlogServer.Models
{
    public class BlogContext : IdentityDbContext<UserModel, Role, string>
    {
        public DbSet<ArticleModel> Articles { get; set; }
        public DbSet<ArticleTagModel> ArticleTags { get; set; }
        public DbSet<ArticleLikeModel> ArticleLikes { get; set; }
        public DbSet<CommentModel> Comments { get; set; }

        public BlogContext(DbContextOptions<BlogContext> options): base(options) { }
    }
}
