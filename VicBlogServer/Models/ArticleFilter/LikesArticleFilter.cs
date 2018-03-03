using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VicBlogServer.Models.ArticleFilter
{
    public class LikesArticleFilter : IArticleFilter
    {
        
        public long? MinLike { get; set; }

        public long? MaxLike { get; set; }

        public IQueryable<ArticleModel> Filter(IQueryable<ArticleModel> model)
        {
            
            return from article in model
                where (!MinLike.HasValue) || article.Likes.Count() >= MinLike
                where (!MaxLike.HasValue) || article.Likes.Count() <= MaxLike
                select article;
        }
    }
}