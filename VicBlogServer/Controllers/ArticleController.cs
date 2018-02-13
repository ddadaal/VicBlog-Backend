using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using VicBlogServer.Data;
using VicBlogServer.DataService;
using VicBlogServer.Filters;
using VicBlogServer.Models;
using VicBlogServer.Utils;
using VicBlogServer.ViewModels;
using VicBlogServer.ViewModels.Dto;

namespace VicBlogServer.Controllers
{
    [Produces("application/json")]
    [Route("api/Article")]
    public abstract class ArticleControllerSpec : Controller
    {
        [HttpGet]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(List<ArticleBriefViewModel>), description: "Filters articles")]
        public abstract Task<IActionResult> GetArticles();

        [HttpGet("Filter")]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(List<ArticleBriefViewModel>), description: "Filters articles")]
        public abstract Task<IActionResult> Filter([FromQuery]ArticleFiler filter);


        [HttpGet("Tags")]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(List<string>), description: "Gets all tags")]
        public abstract Task<IActionResult> GetTags();

        [HttpGet("{articleId}")]
        [ArticleExists]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(ArticleModel),description: "Gets an Article from articleId.")]
        [SwaggerResponse(404, type: typeof(StandardErrorDto), description: "Article id doesn't exist.")]
        public abstract Task<IActionResult> GetAnArticle([FromRoute]string articleId);


        [HttpDelete("{articleId}")]
        [SwaggerOperation]
        [ArticleExists]
        [Authorize(Roles = Role.Admin)]
        [SwaggerResponse(200, description: "Deletion has been done.")]
        [SwaggerResponse(401, description: "Not authorized.")]
        [SwaggerResponse(403, description: "Not enough permission. At least Admin")]
        public abstract Task<IActionResult> DeleteAnArticle([FromRoute]string articleId);

        [HttpPatch("{articleId}")]
        [SwaggerOperation]
        [ArticleExists]
        [Authorize(Roles = Role.Admin)]
        [SwaggerResponse(200, description: "Patch/Update has been done.")]
        [SwaggerResponse(401, description: "Not authorized.")]
        public abstract Task<IActionResult> PatchAnArticle([FromRoute]string articleId, [FromBody]ArticleMinimal article);

        [HttpPost]
        [SwaggerOperation]
        [Authorize(Roles = Role.Admin)]
        [SwaggerResponse(200, description: "Creation has been done.")]
        [SwaggerResponse(401, description: "Not authorized.")]
        [SwaggerResponse(403, description: "Not enough permission. At least Admin")]
        public abstract Task<IActionResult> CreateAnArticle([FromBody]ArticleMinimal article);
    }

    public class ArticleController : ArticleControllerSpec
    {
        private readonly IArticleDataService articleService;
        private readonly ILikeDataService likeService;
        private readonly ITagDataService tagService;
        private readonly ICommentDataService commentService;

        public ArticleController(IArticleDataService articleService, ILikeDataService likeService, ITagDataService tagService, ICommentDataService commentService)
        {
            this.articleService = articleService;
            this.likeService = likeService;
            this.tagService = tagService;
            this.commentService = commentService;
        }

        public override async Task<IActionResult> CreateAnArticle([FromBody] ArticleMinimal article)
        {
            string articleId = Guid.NewGuid().ToString();
            ArticleModel newArticle = new ArticleModel()
            {
                Id = articleId,
                Content = article.Content,
                CreateTime = DateTime.Now,
                LastEditedTime = DateTime.Now,
                Title = article.Title,
                Username = HttpContext.User.Identity.Name
            };

            articleService.Add(newArticle);

            tagService.AddRange(article.Tags.Select(x => new ArticleTagModel()
            {
                ArticleId = articleId,
                Tag = x
            }));

            await articleService.SaveChangesAsync();

            return Created($"api/Article/{articleId}", "");
            
        }

        public override async Task<IActionResult> DeleteAnArticle([FromRoute] string articleId)
        {
            await articleService.RemoveAsync(articleId);

            var ids = tagService.Raw.Where(x => x.ArticleId == articleId).Select(x => x.Id);
            await tagService.RemoveRangeAsync(ids);

            await articleService.SaveChangesAsync();

            return Json(articleId);
        }

        public override async Task<IActionResult> GetAnArticle([FromRoute] string articleId)
        {
            ArticleModel articleModel = await articleService.FindByIdAsync(articleId);
            var tags = tagService.Raw.Where(x => x.ArticleId == articleId).Select(x => x.Tag);
            var likes = likeService.Raw.Where(x => x.ArticleId == articleId)
                .Select(x => new ArticleLikeViewModel()
                {
                    LikeTime = x.LikeTime,
                    Username = x.Username
                });

            return Json(new ArticleViewModel()
            {
                Id = articleModel.Id,
                Content = articleModel.Content,
                CreateTime = articleModel.CreateTime,
                LastEditedTime = articleModel.LastEditedTime,
                Likes = likes,
                Tags = tags,
                Title = articleModel.Title,
                Username = articleModel.Username
            });


        }

        public override async Task<IActionResult> Filter([FromQuery]ArticleFiler filter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            
            var articles = from article in articleService.Raw
                           where (filter.TitleText != null) ||
                                (article.Title.Contains(filter.TitleText))
                           where (filter.CreatedTimeRange != null) ||
                                (filter.CreatedTimeRange[0].ToLocalDateTime() <= article.CreateTime
                                    && article.CreateTime <= filter.CreatedTimeRange[1].ToLocalDateTime())
                           where (filter.EditedTimeRange != null) ||
                                (filter.EditedTimeRange[0].ToLocalDateTime() <= article.LastEditedTime
                                    && article.LastEditedTime <= filter.EditedTimeRange[1].ToLocalDateTime())

                           join like in likeService.Raw on article.Id equals like.ArticleId into likes
                           where likes.Count() >= filter.MinLike

                           join tag in tagService.Raw on article.Id equals tag.ArticleId into tags
                           where (filter.Tags != null) || (filter.Tags.Intersect(tags.Select(x => x.Tag))).Any()

                           join comment in commentService.Raw on article.Id equals comment.ArticleId into comments
                           select new ArticleBriefViewModel()
                           {
                               Id = article.Id,
                               CommentCount = comments.Count(),
                               CreateTime = article.CreateTime,
                               LastEditedTime = article.LastEditedTime,
                               LikeCount = likes.Count(),
                               Tags = tags.Select(x => x.Tag),
                               Title = article.Title,
                               Username = article.Username
                           };
            return Json(articles);
        }

        public override async Task<IActionResult> GetArticles()
        {

            var articles = from article in articleService.Raw
                           join like in likeService.Raw on article.Id equals like.ArticleId into likes
                           join tag in tagService.Raw on article.Id equals tag.ArticleId into tags
                           join comment in commentService.Raw on article.Id equals comment.ArticleId into comments
                           select new ArticleBriefViewModel()
                           {
                               Id = article.Id,
                               CommentCount = comments.Count(),
                               CreateTime = article.CreateTime,
                               LastEditedTime = article.LastEditedTime,
                               LikeCount = likes.Count(),
                               Tags = tags.Select(x => x.Tag),
                               Title = article.Title,
                               Username = article.Username
                           };

            return Json(articles);
        }

        public override async Task<IActionResult> GetTags()
        {
            return Json(tagService.Raw.Select(x => x.Tag).Distinct());
        }

        public override async Task<IActionResult> PatchAnArticle([FromRoute] string articleId, [FromBody] ArticleMinimal article)
        {
            var existentArticle = await articleService.FindByIdAsync(articleId);

            existentArticle.Content = article.Content;
            existentArticle.Title = article.Title;
            existentArticle.LastEditedTime = DateTime.Now;
            articleService.Update(existentArticle);

            var existentTagIds = tagService.Raw.Where(x => x.ArticleId == articleId).Select(x => x.Id);
            await tagService.RemoveRangeAsync(existentTagIds);

            tagService.AddRange(article.Tags.Select(tag => new ArticleTagModel()
            {
                ArticleId = articleId,
                Tag = tag
            }));

            await articleService.SaveChangesAsync();

            return Created($"api/Article/{articleId}", "");

        }
    }
}
