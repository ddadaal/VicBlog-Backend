using System;
using System.Collections.Generic;
using System.Linq;

namespace VicBlogServer.Models.ArticleFilter
{
    public class TimeArticleFilter : IArticleFilter
    {
        public DateTime? CreatedTimeBegin { get; set; }

        public DateTime? CreatedTimeEnd { get; set; }
        
        public DateTime? EditedTimeBegin { get; set; }

        public DateTime? EditedTimeEnd { get; set; }
        
        public IQueryable<ArticleModel> Filter(IQueryable<ArticleModel> model)
        {
            return from article in model
                where (CreatedTimeBegin == null) ||
                      (CreatedTimeBegin <= article.CreateTime)
                where (CreatedTimeEnd == null) ||
                      (article.CreateTime <= CreatedTimeEnd)
                where (EditedTimeBegin == null) ||
                      (EditedTimeBegin <= article.LastEditedTime)
                where (EditedTimeEnd == null) ||
                      (article.LastEditedTime <= EditedTimeEnd)
                select article;
        }
    }
}