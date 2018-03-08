using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VicBlogServer.Models.ArticleFilter
{
    public class TagsArticleFilter: IArticleFilter
    {
        
        public IEnumerable<string> TagsCondition { get; set; }

        public IQueryable<ArticleModel> Filter(IQueryable<ArticleModel> models)
        {

            if (TagsCondition == null)
            {
                return models;
            }


            return TagsCondition.Aggregate(models, 
                (current, tag) => current.Where(article => article.Tags.Select(x => x.Tag).Any(x => TagsCondition.Contains(x))));
            
            // select those articles, each of which contains at least a tag in TagsCondition.
        }
    }
}