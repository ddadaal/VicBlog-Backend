using System;
using System.Collections;
using System.Collections.Generic;
using VicBlog.Data;
using VicBlog.Models;
using System.Linq;

namespace VicBlog
{
    public static class DbInitializer
    {
        public static void Initialize(BlogContext context, bool development)
        {
            context.Database.EnsureCreated();

            if (!development)
            {
                return;
            }
            if (context.Users.Any())
            {
                return;
            }


            User root = new User()
            {

                Username = "root",
                Password = "root".ComputeMD5(),
                RegisterTime = DateTime.Now.ToUniversalTime(),
                Role = Role.Admin

            };

            context.Users.Add(root);

            string guid = Guid.NewGuid().ToString();

            ArticleBrief brief = new ArticleBrief()
            {
                ID = guid,
                Username = root.Username,
                Category = "test",
                SubmitTime = DateTime.Now.ToUniversalTime(),
                LastEditedTime = DateTime.Now.ToUniversalTime(),
                Title = "test Title"
            };

            context.ArticleBriefs.Add(brief);

            Article article = new Article()
            {
                ID = guid,
                Content = "test content 233 hahahahahha"
            };

            Comment comment = new Comment()
            {
                ID = Guid.NewGuid().ToString(),
                ArticleID = guid,
                Content = "test comment",
                ReplyTo = "",
                SubmitTime = DateTime.Now.ToUniversalTime(),
                Username = "test replier"
            };

            Rate rate = new Rate()
            {
                ArticleID = guid,
                Score = 4,
                Username = "root"
            };
            context.Rates.Add(rate);


            context.Articles.Add(article);
            context.Comments.Add(comment);

            context.SaveChanges();

            TagLink.Create("tag1", guid, context);
            TagLink.Create("tag2", guid, context);



        }
    }
}