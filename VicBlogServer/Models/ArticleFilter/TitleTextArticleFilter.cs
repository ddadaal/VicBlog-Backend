using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace VicBlogServer.Models.ArticleFilter
{
    public class TitleTextArticleFilter : IArticleFilter
    {
        public string TitleText { get; set; }

        public IQueryable<ArticleModel> Filter(IQueryable<ArticleModel> model)
        {
            return from article in model
                where article.Title.Contains(TitleText)
                select article;
        }
    }
}