using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.Models;
using VicBlogServer.Utils;

namespace VicBlogServer.ViewModels
{
    public class ArticleFiler
    {
        public List<string> Tags { get; set; }

        public string TitleText { get; set; }

        public long[] CreatedTimeRange { get; set; }

        public long[] EditedTimeRange { get; set; }

        public long MinLike { get; set; }
    }
}
