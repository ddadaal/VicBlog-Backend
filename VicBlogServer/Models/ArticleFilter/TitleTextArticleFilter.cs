using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace VicBlogServer.Models.ArticleFilter
{
    public class TitleTextArticleFilter : IArticleFilter
    {
        public string TitleText { get; set; }

        public IQueryable<ArticleModel> Filter(IQueryable<ArticleModel> models)
        {
            return from article in models
                where TitleText == null || article.Title.Contains(TitleText)
                select article;
        }
    }
}