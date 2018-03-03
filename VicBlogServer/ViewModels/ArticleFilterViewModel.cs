using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.Models;
using VicBlogServer.Utils;

namespace VicBlogServer.ViewModels
{
    public class ArticleFilterViewModel
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
