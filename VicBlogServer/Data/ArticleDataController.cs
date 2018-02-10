using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.DataService;
using VicBlogServer.Models;
using VicBlogServer.Utils;
using VicBlogServer.ViewModels;

namespace VicBlogServer.Data
{
    public class ArticleDataController : 
        DefaultCrudDataController<ArticleModel, string>,
        IArticleDataService
    {
        public ArticleDataController(BlogContext context): base(context, context.Articles)
        {
        }

    }
}
