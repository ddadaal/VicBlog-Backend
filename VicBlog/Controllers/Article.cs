using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlog.Data;
using VicBlog.Models;

namespace VicBlog.Controllers
{
    public partial class DefaultApiController : Controller
    {
        [HttpGet]
        [Route("/tags")]
        [SwaggerOperation("TagsGet")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(200, type: typeof(List<string>), description: "Returns all existing tags.")]
        public IActionResult TagsGet()
        {
            return Json(context.TagLinks.Select(x => x.TagName).Distinct());
        }

        [HttpGet]
        [Route("/categories")]
        [SwaggerOperation("CategoriesGet")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(200, type: typeof(List<string>), description: "Returns all categories existing.")]
        public IActionResult CategoriesGet()
        {
            return Json(context.ArticleBriefs.Select(x => x.Category).Distinct());
        }

        /// <summary>
        /// Get the article by ID
        /// </summary>
        /// <remarks>Returns an Article according to the ID provided</remarks>
        /// <param name="articleID">Article ID</param>
        /// <response code="200">Success</response>
        /// <response code="404">Article Not found</response>
        /// <response code="0">Failed</response>
        [HttpGet]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("/articles/{articleID}")]
        [SwaggerOperation("ArticlesArticleIDGet")]
        [SwaggerResponse(200, type: typeof(ArticleRequestModel), description: "Returns the article specified by the articleID")]
        [SwaggerResponse(404, description: "Article specified by the articleID is not found.")]

        public IActionResult ArticlesArticleIDGet([FromRoute]string articleID)
        {
            ArticleBrief brief = context.ArticleBriefs.Find(articleID);

            if (brief == null)
            {
                return NotFound();
            }

            PV.Add(articleID, Request.GetIPAddress(), context);

            var result = new ArticleRequestModel(
                brief.LoadTheRest(context),
                context.Articles.Find(articleID).Content
            );

            return Json(result);

        }

        [HttpGet]
        [Produces("application/json")]
        [Route("/articles")]
        [SwaggerOperation("ArticlesGet")]
        [SwaggerResponse(200, type: typeof(List<ArticleBriefRequestModel>), description: "Returns all filtered ArticleBriefs.")]
        public virtual IActionResult ArticlesGet()
        {
            return Json(context.ArticleBriefs.ToList().Select(x =>
            {
                return new ArticleBriefRequestModel(x.LoadTheRest(context));
            }));
        }

        [HttpPatch]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("/articles/{articleID}")]
        [SwaggerOperation("ArticlesArticleIDPatch")]
        [SwaggerResponse(200, type: typeof(ArticleRequestModel), description: "Patched Successfully. Returns new Article.")]
        [SwaggerResponse(404, description: "Article specified by articleID is not found")]
        [SwaggerResponse(403, description: "Token outdated.")]
        [SwaggerResponse(401, description: "User token is invalid or operator is not the author or operator is not an admin.")]
        public async Task<IActionResult> ArticlesArticleIDPatch([FromHeader]string token, [FromRoute]string articleID, [FromBody]ArticlePatchModel newArticle)
        {
            User user = null;
            try
            {
                user = Utils.GetUser(token, context);
            }
            catch (TokenOutdatedException)
            {
                return Forbid();
            }
            catch
            {
                return Unauthorized();
            }

            ArticleBrief brief = await context.ArticleBriefs.FindAsync(articleID);
            Article article = await context.Articles.FindAsync(articleID);
            if (brief == null)
            {
                return NotFound();
            }
            if (user.Role != Role.Admin && user.Username != brief.Username)
            {
                return Unauthorized();
            }

            if (newArticle == null)
            {
                return BadRequest();
            }

            article.Content = newArticle.Content;
            brief.Title = newArticle.Title;
            brief.Category = newArticle.Category;
            context.TagLinks.RemoveRange(context.TagLinks.Where(x => x.ArticleID == articleID));
            context.TagLinks.AddRange(newArticle.Tags.Select(x => new TagLink() { ArticleID = articleID, TagName = x }));

            brief.LastEditedTime = DateTime.Now;
            await context.SaveChangesAsync();



            return Ok(new ArticleRequestModel(brief.LoadTheRest(context), article.Content));

        }

        [HttpPost]
        [Route("/articles/filter")]
        [SwaggerOperation("ArticleFilterPost")]
        [SwaggerResponse(200, type: typeof(List<ArticleBriefRequestModel>), description: "Returns filtered ArticleBriefs.")]
        public IActionResult ArticleFilterPost([FromBody]ArticleFilter filter)
        {
            if (filter == null) return BadRequest();

            var allData = context.ArticleBriefs.Select(x => x);

            if (filter.Categories != null && filter.Categories.Any())
            {
                allData = allData.Where(x => filter.Categories.Contains(x.Category));

            }
            if (!string.IsNullOrEmpty(filter.TitleText))
            {
                allData = allData.Where(x => x.Title.Contains(filter.TitleText));
            }

            if (filter.CreatedTimeEnabled && filter.CreatedTimeRange != null)
            {
                allData = allData.Where(x => x.SubmitTime.ToUnixUTCTime() >= filter.CreatedTimeRange[0] && x.SubmitTime.ToUnixUTCTime() <= filter.CreatedTimeRange[1]);
            }

            if (filter.EditedTimeEnabled && filter.EditedTimeRange != null)
            {
                allData = allData.Where(x => x.LastEditedTime.ToUnixUTCTime() >= filter.EditedTimeRange[0] && x.LastEditedTime.ToUnixUTCTime() <= filter.EditedTimeRange[1]);
            }

            var list = allData.ToList().Select(x => x.LoadTheRest(context));




            if (filter.Tags != null && filter.Tags.Any())
            {

                list = list.Where(x => x.Tags.Intersect(filter.Tags).Any());

            }

            switch (filter.Order)
            {
                case ArticleBriefListOrder.SubmitEarliestToLatest:
                    list = list.OrderBy(x => x.SubmitTime);
                    break;
                case ArticleBriefListOrder.SubmitLatestToEarliest:
                    list = list.OrderByDescending(x => x.SubmitTime);
                    break;
                case ArticleBriefListOrder.LastEditedEarliestToLatest:
                    list = list.OrderBy(x => x.LastEditedTime);
                    break;
                case ArticleBriefListOrder.LastEditedLatestToEarlist:
                    list = list.OrderByDescending(x => x.LastEditedTime);
                    break;
                case ArticleBriefListOrder.RankHighestToLowest:
                    list = list.OrderByDescending(x => x.Rate);
                    break;
                case ArticleBriefListOrder.RankLowestToHighest:
                    list = list.OrderBy(x => x.Rate);
                    break;
            }

            return Json(list.Select(x => new ArticleBriefRequestModel(x)));
        }



        [HttpDelete]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("/articles/{articleID}")]
        [SwaggerOperation("ArticlesArticleIDDelete")]
        [SwaggerResponse(200, type: typeof(Article), description: "Deleted successfully. Returns the article.")]
        [SwaggerResponse(401, description: "User token is invalid or operator is not the author or operator is not an admin.")]
        [SwaggerResponse(403, description: "Token outdated.")]
        [SwaggerResponse(404, description: "Article specified by the articleID is not found.")]
        public async Task<IActionResult> ArticlesArticleIDDelete([FromRoute]string articleID, [FromHeader]string token)
        {
            User user = null;
            try
            {
                user = Utils.GetUser(token, context);
            }
            catch (TokenOutdatedException)
            {
                return Forbid();
            }
            catch
            {
                return Unauthorized();
            }
            var article = context.Articles.Find(articleID);
            if (article == null)
            {
                return NotFound();
            }

            var brief = await context.ArticleBriefs.FindAsync(articleID);

            context.ArticleBriefs.Remove(brief);

            context.Articles.Remove(article);

            context.TagLinks.RemoveRange(context.TagLinks.Where(x => x.ArticleID == articleID));

            PV.DeleteAll(context, articleID);

            await context.SaveChangesAsync();



            return Json(article);
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("/articles")]
        [SwaggerOperation("ArticlesArticlePOST")]
        [SwaggerResponse(201, type: typeof(ArticleRequestModel), description: "Created successfully. Returns new article.")]
        [SwaggerResponse(401, description: "User token is not valid or user is not an admin.")]
        [SwaggerResponse(403, description: "Token outdated.")]
        public async Task<IActionResult> ArticlesArticlePOST([FromHeader]string token, [FromBody]ArticleCreationModel data)
        {
            User user = null;
            try
            {
                user = Utils.GetUser(token, context);
            }
            catch (TokenOutdatedException)
            {
                return Forbid();
            }
            catch
            {
                return Unauthorized();
            }

            if (user.Role != Role.Admin)
            {
                return Unauthorized();
            }

            string guid = Guid.NewGuid().ToString();

            DateTime now = DateTime.Now;

            ArticleBrief brief = new ArticleBrief()
            {
                ID = guid,
                Username = user.Username,
                LastEditedTime = now,
                SubmitTime = now,
                Title = data.Title,
                Category = data.Category
            };

            context.ArticleBriefs.Add(brief);

            Article article = new Article()
            {
                ID = guid,
                Content = data.Content
            };
            context.Articles.Add(article);


            context.TagLinks.AddRange(data.Tags.Select(x => new TagLink() { ArticleID = guid, TagName = x }));



            Rate rate = new Rate()
            {
                ArticleID = guid,
                Score = data.Rate,
                Username = user.Username,
                SubmitTime = now
            };

            context.Rates.Add(rate);



            await context.SaveChangesAsync();

            return Created("/articles/" + guid, new ArticleRequestModel(brief.LoadTheRest(context), article.Content));
        }
    }
}
