using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VicBlogServer.Controllers;
using VicBlogServer.DataService;
using VicBlogServer.Models;
using VicBlogServer.Models.ArticleFilter;
using Xunit;

namespace VicBlogServer.Test
{
    public class ArticleFilterTest
    {
        private void AssertCount(ArticleFilterModel filter, int expected)
        {
            Assert.Equal(expected, MockObjProvider.GetMockArticleModels().Filter(filter).Count());
        }

        [Fact]
        public void AllNullFilter()
        {
            var filter = new ArticleFilterModel();
            AssertCount(filter, 2);
        }

        [Fact]
        public void Like()
        {
            var filter = new ArticleFilterModel()
            {
                MinLike = 0
            };
            
            AssertCount(filter,2);

            var filter2 = new ArticleFilterModel()
            {
                MinLike = 2
            };
            AssertCount(filter2, 1);

            var filter3 = new ArticleFilterModel()
            {
                MinLike = 3
            };
            AssertCount(filter3,0);
            
            AssertCount(new ArticleFilterModel()
            {
                MinLike = 2,
                MaxLike = 4
            },1);
        }

        [Fact]
        public void Tags()
        {
            var filter = new ArticleFilterModel()
            {
                Tags = new List<string> {"123"}
            };
            AssertCount(filter, 2);

            var filter2 = new ArticleFilterModel()
            {
                Tags = new List<string> {"1234"}
            };
            AssertCount(filter2,1);

            var filter3 = new ArticleFilterModel()
            {
                Tags = new List<string> {"123456"}
            };
            
            AssertCount(filter3, 0);
        }
        
        [Fact]
        public void EditTime()
        {
            AssertCount(new ArticleFilterModel()
            {
                EditedTimeBegin = new DateTime(2018,3,7,9,0,0),
                EditedTimeEnd = new DateTime(2018,3,7,12,0,0)
            },2);
            
            AssertCount(new ArticleFilterModel()
            {
                EditedTimeBegin = new DateTime(2018,3,7,11,0,0),
                EditedTimeEnd = new DateTime(2018,3,7,12,0,0)
            },1);
            
            AssertCount(new ArticleFilterModel()
            {
                EditedTimeBegin = new DateTime(2018,3,7,9,0,0),
            },2);
            
                        
            AssertCount(new ArticleFilterModel()
            {
                EditedTimeEnd = new DateTime(2018,3,7,9,0,0),
            },0);
        }

        [Fact]
        public void CreateTime()
        {
            //2018 2 2 10 0 0
            // 2018 3 7 10 0 0
            AssertCount(new ArticleFilterModel()
            {
                CreatedTimeBegin = new DateTime(2018,2,1,9,0,0),
                CreateTimeEnd = new DateTime(2018,3,7,12,0,0)
            },2);
            
            AssertCount(new ArticleFilterModel()
            {
                CreatedTimeBegin = new DateTime(2018,2,3,11,0,0),
                CreateTimeEnd = new DateTime(2018,3,7,12,0,0)
            },1);
            
            AssertCount(new ArticleFilterModel()
            {
                CreatedTimeBegin = new DateTime(2018,2,2,9,0,0),
            },2);
            
                        
            AssertCount(new ArticleFilterModel()
            {
                CreateTimeEnd = new DateTime(2018,3,7,9,0,0),
            },0);
        }

        [Fact]
        public void TitleText()
        {
            AssertCount(new ArticleFilterModel()
            {
                TitleText = "45",
            },2);
            AssertCount(new ArticleFilterModel()
            {
                TitleText = "123"
            },1);
        }
    }
}
