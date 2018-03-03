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
        public List<IArticleFilter> Filters { get; set; } = new List<IArticleFilter>();

        public ArticleFilter(ArticleFilterViewModel viewModel = null)
        {
            if (viewModel != null)
            {
                AddFromViewModel(viewModel);
            }
        }

        public IQueryable<ArticleModel> Filter(IQueryable<ArticleModel> models)
        {
            Filters.ForEach(filter => filter.Filter(models));
            return models;
        }

        public void AddFromViewModel(ArticleFilterViewModel viewModel)
        {
            Filters.Add(new LikesArticleFilter()
            {
                MaxLike = viewModel.MaxLike,
                MinLike = viewModel.MinLike
            });
            Filters.Add(new TagsArticleFilter()
            {
                TagsCondition = viewModel.Tags
            });
            Filters.Add(new TitleTextArticleFilter()
            {
                TitleText = viewModel.TitleText
            });
            Filters.Add(new TimeArticleFilter()
            {
                CreatedTimeBegin = viewModel.CreatedTimeBegin,
                CreatedTimeEnd = viewModel.CreateTimeEnd,
                EditedTimeBegin = viewModel.EditedTimeBegin,
                EditedTimeEnd = viewModel.EditedTimeEnd
            });
        }
        
       
    }


}
