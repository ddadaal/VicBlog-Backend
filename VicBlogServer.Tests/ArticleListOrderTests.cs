using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using VicBlogServer.Models;
using VicBlogServer.Models.ArticleListOrder;
using Xunit;

namespace VicBlogServer.Tests
{
    public class ArticleListOrderTests
    {
////                LastEditTimeLatestFirst,
//        LastEditTimeEarliestFirst,
//        CreateTimeLatestFirst,
//        CreateTimeEarliestFirst,
//        LikeLeastFirst,
//        LikeMostFirst

        private void AssertOrder(ArticleListOrder? order, int id)
        {
            var mock = MockObjProvider.GetMockArticleModels();
            Assert.Equal(mock.OrderArticles(order).First().ArticleId, id);
        }
        
        [Fact]
        public void LastEditTimeLatestFirst()
        {
            AssertOrder(ArticleListOrder.LastEditTimeLatestFirst, 2);
        }

        [Fact]
        public void CreateTimeLatestFirst()
        {
            AssertOrder(ArticleListOrder.CreateTimeLatestFirst, 1 );
        }

        [Fact]
        public void LikeMostFirst()
        {
            AssertOrder(ArticleListOrder.LikeMostFirst, 2);
        }

        [Fact]
        public void Default_ShouldBeLastEditTimeLatestFirst()
        {
            AssertOrder(null, 2);
        }
    }
}