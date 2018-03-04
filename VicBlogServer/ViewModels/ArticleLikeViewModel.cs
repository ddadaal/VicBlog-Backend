using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VicBlogServer.ViewModels
{

    public class ArticleLikeHistoryViewModel
    {
        public IEnumerable<ArticleLikeViewModel> List { get; set; }
    }
    
    public class ArticleLikeViewModel
    {
        public DateTime LikeTime { get; set; }
        public string Username { get; set; }
    }
}
