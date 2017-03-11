using System;
using Microsoft.EntityFrameworkCore;
using VicBlog.Models;

namespace VicBlog.Data
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {

        }


        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ArticleBrief> ArticleBriefs { get; set; }
        public DbSet<TagLink> TagLinks { get; set; }
        public DbSet<ArticlePVModel> ArticlePVs { get; set; }

        public DbSet<Rate> Rates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>().ToTable("Article");
            modelBuilder.Entity<Comment>().ToTable("Comment");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<ArticleBrief>().ToTable("ArticleBrief");
            modelBuilder.Entity<TagLink>().ToTable("TagLink");
            modelBuilder.Entity<Rate>().ToTable("Rate");
            modelBuilder.Entity<ArticlePVModel>().ToTable("ArticlePV");
        }



    }
}