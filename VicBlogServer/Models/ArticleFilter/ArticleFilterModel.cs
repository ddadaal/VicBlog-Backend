using System;
using System.Collections.Generic;

namespace VicBlogServer.Models.ArticleFilter
{
    public class ArticleFilterModel
    {
        public IEnumerable<string> Tags { get; set; }

        public string TitleText { get; set; }

        public DateTime? CreatedTimeBegin { get; set; }

        public DateTime? CreateTimeEnd { get; set; }

        public DateTime? EditedTimeBegin { get; set; }

        public DateTime? EditedTimeEnd { get; set; }

        public long? MinLike { get; set; }

        public long? MaxLike { get; set; }


    }
}
