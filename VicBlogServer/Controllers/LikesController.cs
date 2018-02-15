using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using VicBlogServer.DataService;
using VicBlogServer.Filters;
using VicBlogServer.Models;
using VicBlogServer.ViewModels;
using VicBlogServer.ViewModels.Dto;

namespace VicBlogServer.Controllers
{
    [Produces("application/json")]
    [Route("api/Likes")]
    public abstract class LikesControllerSpec : Controller
    {
        [HttpGet]
        [ArticleExists]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(int), description: "Returns like count of the articleId.")]
        [SwaggerResponse(404, type: typeof(StandardErrorDto), description: "article id doesn't exist.")]
        public abstract Task<IActionResult> GetLikeCount([FromQuery]int articleId);

        [HttpGet("History")]
        [ArticleExists]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(List<ArticleLikeViewModel>), description: "Returns like history of the articleId.")]
        [SwaggerResponse(404, description: "article id doesn't exist.")]
        public abstract Task<IActionResult> GetLikeHistory([FromQuery]int articleId);

        [HttpPost]
        [Authorize]
        [ArticleExists]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(int), description: "Likes an article. Returns new like count.")]
        [SwaggerResponse(401, description: "Not logged in.")]
        [SwaggerResponse(404, description: "Article id doesn't exist.")]
        [SwaggerResponse(409, description: "The user has already liked the article.")]
        public abstract Task<IActionResult> CreateALike([FromQuery]int articleId);

        [HttpDelete]
        [Authorize]
        [ArticleExists]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(int), description: "Like removed. Returns new like count.")]
        [SwaggerResponse(404, description: "Article id doesn't exist.")]
        [SwaggerResponse(400, description: "The user hasn't liked the article.")]
        [SwaggerResponse(401, description: "Not logged in.")]
        public abstract Task<IActionResult> RemoveALike([FromQuery]int articleId);
    }

    public class LikesController : LikesControllerSpec
    {
        private readonly ILikeDataService likeService;
        private readonly IArticleDataService articleDataService;

        public LikesController(ILikeDataService likeService, IArticleDataService articleDataService)
        {
            this.likeService = likeService;
            this.articleDataService = articleDataService;
        }

        public override async Task<IActionResult> CreateALike([FromQuery] int articleId)
        {
            var username = HttpContext.User.Identity.Name;

            if (likeService.Raw.Where(x => x.ArticleId == articleId && x.Username == username).Any())
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }

            var like = new ArticleLikeModel()
            {
                LikeTime = DateTime.UtcNow,
                Username = username,
                ArticleId = articleId
            };

            likeService.Add(like);
            await likeService.SaveChangesAsync();

            return Json(Count(articleId));

        }

        public override async Task<IActionResult> GetLikeCount([FromQuery] int articleId)
        {
            return Json(Count(articleId));
        }

        public override async Task<IActionResult> GetLikeHistory([FromQuery] int articleId)
        {

            var history = likeService.Raw.Select(x => new ArticleLikeViewModel()
            {
                LikeTime = x.LikeTime,
                Username = x.Username
            });

            return Json(history);
        }

        private int Count(int articleId)
        {
            return likeService.Raw.Where(x => x.ArticleId == articleId).Count();
        }

        public override async Task<IActionResult> RemoveALike([FromQuery] int articleId)
        {
            var username = HttpContext.User.Identity.Name;

            var like = likeService.Raw.Where(x => x.ArticleId == articleId && x.Username == username).SingleOrDefault();
            if (like == null)
            {
                return BadRequest();
            }

            await likeService.RemoveAsync(like.Id);
            await likeService.SaveChangesAsync();

            return Json(Count(articleId));
        }
    }

}