using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.SwaggerGen;
using VicBlogServer.Data;
using VicBlogServer.DataService;
using VicBlogServer.Filters;
using VicBlogServer.Models;
using VicBlogServer.Models.ArticleFilter;
using VicBlogServer.Models.ArticleListOrder;
using VicBlogServer.Utils;
using VicBlogServer.ViewModels;
using VicBlogServer.ViewModels.Dto;

namespace VicBlogServer.Controllers
{
    [Produces("application/json")]
    [Route("api/Articles")]
    public abstract class ArticlesControllerSpec : Controller
    {
        [HttpGet]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(ArticleListViewModel), description: "Filters articles")]
        public abstract Task<IActionResult> GetArticles
            ([FromQuery] ArticleFilterModel filter, [FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            [FromQuery] ArticleListOrder? order);


        [HttpGet("Tags")]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(List<string>), description: "Gets all tags")]
        public abstract Task<IActionResult> GetTags();

        [HttpGet("{articleId}")]
        [ArticleExists]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(ArticleModel), description: "Gets an Article from articleId.")]
        [SwaggerResponse(404, type: typeof(StandardErrorDto), description: "Article id doesn't exist.")]
        public abstract Task<IActionResult> GetAnArticle([FromRoute] int articleId);


        [HttpDelete("{articleId}")]
        [SwaggerOperation]
        [ArticleExists]
        [Authorize(Roles = Role.Admin)]
        [SwaggerResponse(200, description: "Deletion has been done.")]
        [SwaggerResponse(401, description: "Not authorized.")]
        [SwaggerResponse(403, description: "Not enough permission. At least Admin")]
        public abstract Task<IActionResult> DeleteAnArticle([FromRoute] int articleId);

        [HttpPatch("{articleId}")]
        [SwaggerOperation]
        [ArticleExists]
        [Authorize(Roles = Role.Admin)]
        [SwaggerResponse(200, description: "Patch/Update has been done.")]
        [SwaggerResponse(401, description: "Not authorized.")]
        public abstract Task<IActionResult>
            PatchAnArticle([FromRoute] int articleId, [FromBody] ArticleMinimal article);

        [HttpPost]
        [SwaggerOperation]
        [Authorize(Roles = Role.Admin)]
        [SwaggerResponse(201, typeof(int), description: "Creation has been done.")]
        [SwaggerResponse(401, description: "Not authorized.")]
        [SwaggerResponse(403, description: "Not enough permission. Need Admin")]
        public abstract Task<IActionResult> CreateAnArticle([FromBody] ArticleMinimal article);
    }

    public class ArticlesController : ArticlesControllerSpec
    {
        private readonly IArticleDataService articleService;
        private readonly ITagDataService tagService;

        public ArticlesController(IArticleDataService articleService, ITagDataService tagService)
        {
            this.articleService = articleService;
            this.tagService = tagService;
        }

        private static ArticleListViewModel ToListViewModel(IEnumerable<ArticleBriefViewModel> list, int? pageNumber,
            int? pageSize)
        {
            
            
            var paged = list.Page(pageNumber, pageSize);

            var articleListVm = new ArticleListViewModel()
            {
                PagingInfo = paged.ToPagingInfo(),
                List = paged.List
            };

            return articleListVm;
        }

        public override async Task<IActionResult> CreateAnArticle([FromBody] ArticleMinimal article)
        {
            var now = DateTime.UtcNow;
            var newArticle = new ArticleModel()
            {
                Content = article.Content,
                CreateTime = now,
                LastEditedTime = now,
                Title = article.Title,
                Username = HttpContext.User.Identity.Name,
                Tags = article.Tags.Select(x => new ArticleTagModel()
                {
                    Tag = x
                }).ToList()
            };

            articleService.Add(newArticle);
            await articleService.SaveChangesAsync();

            var id = newArticle.ArticleId;

            return Created($"api/Articles/{id}", id);
        }

        public override async Task<IActionResult> DeleteAnArticle([FromRoute] int articleId)
        {
            await articleService.RemoveAsync(articleId);

            await articleService.SaveChangesAsync();

            return Json(articleId);
        }

        public override async Task<IActionResult> GetAnArticle([FromRoute] int articleId)
        {
            var articleModel = await articleService.FindAFullyLoadArticleAsync(articleId);
            return Json(new ArticleViewModel()
            {
                ArticleId = articleModel.ArticleId,
                Content = articleModel.Content,
                CreateTime = articleModel.CreateTime,
                LastEditedTime = articleModel.LastEditedTime,
                Tags = articleModel.Tags.Select(x => x.Tag),
                Title = articleModel.Title,
                Author = articleModel.Username,
            });
        }

        public override async Task<IActionResult> GetArticles
            ([FromQuery] ArticleFilterModel filter, [FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            [FromQuery] ArticleListOrder? order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var articles = articleService.FullyLoadedRaw.Filter(filter)
                .Select(article => new ArticleBriefViewModel()
                {
                    ArticleId = article.ArticleId,
                    CommentCount = article.Comments.Count(),
                    CreateTime = article.CreateTime,
                    LastEditedTime = article.LastEditedTime,
                    LikeCount = article.Likes.Count(),
                    Tags = article.Tags.Select(x => x.Tag),
                    Title = article.Title,
                    Author = article.Username
                });


            return Json(ToListViewModel(articles, pageNumber, pageSize));
        }

        public override async Task<IActionResult> GetTags()
        {
            return Json(tagService.GetAllTags());
        }

        public override async Task<IActionResult> PatchAnArticle([FromRoute] int articleId,
            [FromBody] ArticleMinimal article)
        {
            var existentArticle = await articleService.FindAFullyLoadArticleAsync(articleId);

            existentArticle.Content = article.Content;
            existentArticle.Title = article.Title;
            existentArticle.LastEditedTime = DateTime.UtcNow;

            existentArticle.Tags = article.Tags.Select(tag => new ArticleTagModel()
            {
                Tag = tag
            }).ToList();
            await articleService.SaveChangesAsync();

            return Created($"api/Article/{articleId}", existentArticle.ArticleId);
        }
    }
}