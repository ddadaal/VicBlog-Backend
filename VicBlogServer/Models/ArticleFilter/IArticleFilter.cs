using System;
using System.Collections.Generic;
using System.Linq;
using VicBlogServer.DataService;
using VicBlogServer.ViewModels;

namespace VicBlogServer.Models.ArticleFilter
{
    public interface IArticleFilter
    {
        IQueryable<ArticleModel> Filter (IQueryable<ArticleModel> model);
    }
    
    public class ArticleFilter
    {
        private List<IArticleFilter> Filters { get; set; } = new List<IArticleFilter>();

        public ArticleFilter(ArticleFilterModel model = null)
        {

            if (model != null)
            {
                AddFromViewModel(model);
            }
        }

        public IQueryable<ArticleModel> Filter(IQueryable<ArticleModel> models)
        {
            foreach (var filter in Filters)
            {
                models = filter.Filter(models);
            }
            return models;
        }

        private void AddFromViewModel(ArticleFilterModel model)
        {
            Filters.Add(new LikesArticleFilter()
            {
                MaxLike = model.MaxLike,
                MinLike = model.MinLike
            });
            Filters.Add(new TagsArticleFilter()
            {
                TagsCondition = model.Tags
            });
            Filters.Add(new TitleTextArticleFilter()
            {
                TitleText = model.TitleText
            });
            Filters.Add(new TimeArticleFilter()
            {
                CreatedTimeBegin = model.CreatedTimeBegin,
                CreatedTimeEnd = model.CreateTimeEnd,
                EditedTimeBegin = model.EditedTimeBegin,
                EditedTimeEnd = model.EditedTimeEnd
            });
        }
        
       
    }


    public static class ArticleFilterExtension
    {
        
        public static IQueryable<ArticleModel> FilterArticleList(this IQueryable<ArticleModel> models, ArticleFilterModel filter)
        {
            return new ArticleFilter(filter).Filter(models);
        }
    }
}
