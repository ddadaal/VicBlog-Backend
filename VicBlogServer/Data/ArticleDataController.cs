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
    public class ArticleDataController : DefaultCrudDataController<ArticleModel, int>, IArticleDataService
    {
        public ArticleDataController(BlogContext context): base(context, context.Articles)
        {
            
            
        }

        public async Task<ArticleModel> FindAFullyLoadArticleAsync(int id)
        {
            var article = await dbSet.FindAsync(id);

            if (article == null)
            {
                return null;
            }
            context.Entry(article).Collection(x => x.Tags).Load(); 
            context.Entry(article).Collection(x => x.Likes).Load();
            context.Entry(article).Collection(x => x.Comments).Load();

            return article;
        }

        public IQueryable<ArticleModel> FullyLoadedRaw
        {
            get { return dbSet.Include(x => x.Comments).Include(x => x.Likes).Include(x => x.Tags); }
        }
        
        
        
        
    }
}
