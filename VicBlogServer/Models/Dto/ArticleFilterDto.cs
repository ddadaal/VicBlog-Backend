using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.Utils;

namespace VicBlogServer.Models.Dto
{
    public class ArticleFilterDto
    {
        public List<string> Tags { get; set; }

        public string TitleText { get; set; }

        public long[] CreatedTimeRange { get; set; }

        public long[] EditedTimeRange { get; set; }

        public long MinLike { get; set; }

        public List<Article> Execute(DbSet<Article> dbSet)
        {
            var articles = from x in dbSet
                           where (TitleText == null) || x.Title.Contains(TitleText)
                           where (Tags == null) || x.Tags.Intersect(Tags).Any()
                           where (CreatedTimeRange == null) || 
                                (CreatedTimeRange[0].ToLocalDateTime() <= x.CreateTime
                                    && x.CreateTime <= CreatedTimeRange[1].ToLocalDateTime())
                           where (EditedTimeRange == null) || 
                                (EditedTimeRange[0].ToLocalDateTime() <= x.LastEditedTime 
                                    && x.LastEditedTime <= EditedTimeRange[1].ToLocalDateTime())
                           where x.Like >= MinLike
                           select x;
            return articles.ToList();
            

        }
    }
}
