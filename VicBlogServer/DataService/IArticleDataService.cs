using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.Models;
using VicBlogServer.ViewModels;

namespace VicBlogServer.DataService
{
    public interface IArticleDataService : ICrudDataService<ArticleModel, string>
    {
    }
}
