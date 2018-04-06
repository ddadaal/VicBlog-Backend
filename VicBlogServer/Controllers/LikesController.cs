using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [SwaggerResponse(200, type: typeof(ArticleLikeHistoryViewModel), description: "Returns like history of the articleId.")]
        [SwaggerResponse(404, type: typeof(StandardErrorDto), description: "article id doesn't exist.")]
        public abstract Task<IActionResult> GetLikeHistory([FromQuery]int articleId);

        [HttpGet("QueryLiked")]
        [ArticleExists]
        [SwaggerOperation]
        [Authorize]
        [SwaggerResponse(200, type: typeof(QueryLikedResultViewModel), description: "Returns whether current user has liked the article.")]
        [SwaggerResponse(404, type: typeof(StandardErrorDto), description: "article id doesn't exist.")]
        public abstract Task<IActionResult> QueryLiked([FromQuery]int articleId);

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

            var article = await articleDataService.FindByIdAsync(articleId);

            if (likeService.Raw.Any(x => x.Article == article))
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }
           
            var like = new ArticleLikeModel()
            {
                LikeTime = DateTime.UtcNow,
                Username = username,
                Article = article
            };

            likeService.Add(like);
            await likeService.SaveChangesAsync();

            return Json(await CountAsync(articleId));

        }

        public override async Task<IActionResult> GetLikeCount([FromQuery] int articleId)
        {
            return Json(await CountAsync(articleId));
        }

        public override async Task<IActionResult> GetLikeHistory([FromQuery] int articleId)
        {
            var list = likeService.Raw
                .Where(x => x.Article.ArticleId == articleId)
                .Select(x => new ArticleLikeViewModel()
                {
                    Username = x.Username,
                    LikeTime = x.LikeTime
                });
            
            
            
            return Json(new ArticleLikeHistoryViewModel()
            {
                List =  list
            });
        }

        private async Task<int> CountAsync(int articleId)
        {
            return likeService.Raw.Count(x => x.Article.ArticleId == articleId);
        }

        public override async Task<IActionResult> RemoveALike([FromQuery] int articleId)
        {
            var username = HttpContext.User.Identity.Name;

            await likeService.RemoveWhereAsync(x => x.Article.ArticleId == articleId && x.Username == username);
            await likeService.SaveChangesAsync();

            return Json(likeService.Raw.Count(x => x.Article.ArticleId == articleId));
        }

        public override async Task<IActionResult> QueryLiked([FromQuery] int articleId)
        {
            var username = HttpContext.User.Identity.Name;

            var like = await likeService.Raw.SingleOrDefaultAsync(x => x.Article.ArticleId == articleId && x.Username == username);

            if (like == null)
            {
                return Json(new QueryLikedResultViewModel()
                {
                    DidLike = false
                });
            }
            else
            {
                return Json(new QueryLikedResultViewModel()
                {
                    DidLike = true,
                    LikeTime = like.LikeTime
                });
            }
        }
    }

}