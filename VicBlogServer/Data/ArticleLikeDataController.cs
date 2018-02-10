using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.DataService;
using VicBlogServer.Models;

namespace VicBlogServer.Data
{
    public class ArticleLikeDataController : DefaultCrudDataController<ArticleLikeModel, int>, ILikeDataService
    {
        public ArticleLikeDataController(BlogContext context)
            : base(context, context.ArticleLikes) {  }
    }
}
