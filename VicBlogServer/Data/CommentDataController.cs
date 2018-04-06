using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.DataService;
using VicBlogServer.Models;

namespace VicBlogServer.Data
{
    public class CommentDataController : DefaultCrudDataController<CommentModel, int>, ICommentDataService
    {
        public CommentDataController(BlogContext context)
            : base(context, context.Comments) { }

        public IQueryable<CommentModel> FindByArticleId(int articleId)
        {
            var comments = dbSet
                .Include(x => x.Article)
                .Where(x => x.Article.ArticleId == articleId);

            return comments.AsQueryable();
        }
    }
}
