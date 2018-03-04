using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VicBlogServer.Models.ArticleListOrder
{
    public enum ArticleListOrder
    {
        LastEditTimeLatestFirst,
        LastEditTimeEarliestFirst,
        CreateTimeLatestFirst,
        CreateTimeEarliestFirst,
        LikeLeastFirst,
        LikeMostFirst
    }

    public static class ArticleListOrderExtensions
    {
        public static IQueryable<ArticleModel> OrderArticles(this IQueryable<ArticleModel> list, ArticleListOrder? order)
        {
            var actualOrder = order.GetValueOrDefault(ArticleListOrder.LastEditTimeLatestFirst);
            switch (actualOrder)
            {
                case ArticleListOrder.LastEditTimeEarliestFirst:
                    return list.OrderBy(x => x.LastEditedTime);
                case ArticleListOrder.LastEditTimeLatestFirst:
                    return list.OrderByDescending(x => x.LastEditedTime);
                case ArticleListOrder.CreateTimeEarliestFirst:
                    return list.OrderBy(x => x.CreateTime);
                case ArticleListOrder.CreateTimeLatestFirst:
                    return list.OrderByDescending(x => x.CreateTime);
                case ArticleListOrder.LikeMostFirst:
                    return list.Include(x => x.Likes).OrderByDescending(x => x.Likes.Count);
                case ArticleListOrder.LikeLeastFirst:
                    return list.Include(x => x.Likes).OrderBy(x => x.Likes.Count);
                default:
                    throw new ArgumentOutOfRangeException(nameof(actualOrder), actualOrder, null);
            }
        }
    }

}
