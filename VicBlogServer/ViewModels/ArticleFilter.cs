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
        public List<string> Tags { get; }

        public string TitleText { get; }

        public List<long> CreatedTimeRange { get;  }

        public List<long> EditedTimeRange { get;  }

        public long MinLike { get; }
    }
}
