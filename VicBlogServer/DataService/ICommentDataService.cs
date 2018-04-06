using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.Models;

namespace VicBlogServer.DataService
{
    public interface ICommentDataService : ICrudDataService<CommentModel, int>
    {
        IQueryable<CommentModel> FindByArticleId(int articleId);
    }
}
