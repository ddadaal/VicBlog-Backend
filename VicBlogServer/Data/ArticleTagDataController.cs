using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.DataService;
using VicBlogServer.Models;

namespace VicBlogServer.Data
{
    public class ArticleTagDataController : DefaultCrudDataController<ArticleTagModel, int>, ITagDataService
    {
        public ArticleTagDataController(BlogContext context) : base(context, context.ArticleTags) { }

        public IEnumerable<string> GetAllTags()
        {
            return dbSet.Select(x => x.Tag).Distinct();
        }

    }
}
