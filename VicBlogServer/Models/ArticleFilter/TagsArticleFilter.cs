using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VicBlogServer.Models.ArticleFilter
{
    public class TagsArticleFilter: IArticleFilter
    {
        
        public IEnumerable<string> TagsCondition { get; set; }

        public IQueryable<ArticleModel> Filter(IQueryable<ArticleModel> model)
        {
            return from article in model
                where (TagsCondition == null) 
                      || (TagsCondition.Intersect(article.Tags.Select(x => x.Tag))).Any()
                select article;
        }
    }
}