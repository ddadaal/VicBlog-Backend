using System;
using System.Collections.Generic;
using System.Linq;
using VicBlogServer.Models;

namespace VicBlogServer.Test
{
    public static class MockObjProvider
    {
        public static IQueryable<ArticleModel> GetMockArticleModels()
        {
            var list = new List<ArticleModel>
            {
                new ArticleModel()
                {
                    ArticleId = 1,
                    Comments = new List<CommentModel>(),
                    Content = "123",
                    Title = "123456",
                    LastEditedTime = new DateTime(2018, 3, 7, 10, 0, 0),
                    CreateTime = new DateTime(2018, 3, 7, 10, 0, 0),
                    Likes = new List<ArticleLikeModel>
                    {
                        new ArticleLikeModel()
                    },
                    Tags = new List<ArticleTagModel>()
                    {
                        new ArticleTagModel()
                        {
                            Tag = "123",
                            TagId = 2
                        },
                        new ArticleTagModel()
                        {
                            Tag = "1234",
                            TagId = 3
                        }
                    },
                    Username = "123"
                },
                new ArticleModel()
                {
                    ArticleId = 2,
                    Comments = new List<CommentModel>(),
                    Content = "123",
                    Title = "45",
                    LastEditedTime = new DateTime(2018, 3,7, 12,0,0),
                    CreateTime = new DateTime(2018,2,2,10,0,0),
                    Likes = new List<ArticleLikeModel>
                    {
                        new ArticleLikeModel(),
                        new ArticleLikeModel()
                    },
                    Tags = new List<ArticleTagModel>
                    {
                        new ArticleTagModel()
                        {
                            Tag = "123",
                            TagId = 1
                        }
                    },
                    Username = "123"
                }
            };
            return list.AsQueryable();
        }
    }
}