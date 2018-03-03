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
    [Route("api/Comments")]
    public abstract class CommentsControllerSpec : Controller
    {
        [HttpPost]
        [Authorize]
        [ArticleExists]
        [SwaggerOperation]
        [SwaggerResponse(201, typeof(int), description: "Comment has been created.")]
        [SwaggerResponse(401, description: "Not logged in.")]
        [SwaggerResponse(404, typeof(StandardErrorDto), description: "Article id is not valid.")]
        public abstract Task<IActionResult> AddComment([FromQuery]int articleId, [FromBody]CommentMinimal comment);

        [HttpDelete]
        [Authorize]
        [SwaggerOperation]
        [SwaggerResponse(200, description: "The comment has been removed.")]
        [SwaggerResponse(401, description: "Not logged in")]
        [SwaggerResponse(403, description: "not an admin && not the author of the comment")]
        [SwaggerResponse(404, description: "Comment id is not valid.")]
        public abstract Task<IActionResult> RemoveComment([FromQuery]int commentId);

        [HttpGet]
        [ArticleExists]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(CommentListViewModel), description: "Returns the comments of the article.")]
        [SwaggerResponse(404, typeof(StandardErrorDto), description: "The article is not found.")]
        public abstract Task<IActionResult> GetComments([FromQuery]int articleId, [FromQuery]int? pageNumber, [FromQuery]int? pageSize);

        [HttpGet("{commentId}")]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(CommentViewModel), description: "Returns the comment.")]
        [SwaggerResponse(404, description: "The comment is not found.")]
        public abstract Task<IActionResult> GetComment([FromRoute]int id);
    }

    public class CommentsController : CommentsControllerSpec
    {

        private readonly IArticleDataService articleService;
        private readonly ICommentDataService commentService;
        private readonly IAccountDataService userService;

        private CommentViewModel ModelToViewModel(CommentModel model)
        {
            return new CommentViewModel()
            {
                Id = model.CommentId,
                ArticleId = model.Article.ArticleId,
                Content = model.Content,
                SubmitTime = model.SubmitTime,
                Username = model.Username
            };
        }

        public CommentsController(IArticleDataService articleService, ICommentDataService commentService, IAccountDataService userService)
        {
            this.articleService = articleService;
            this.commentService = commentService;
            this.userService = userService;
        }

        public override async Task<IActionResult> AddComment([FromQuery]int articleId, [FromBody] CommentMinimal comment)
        {
            var article = await articleService.FindByIdAsync(articleId);
            
            var newComment = new CommentModel()
            {
                Article = article,
                Content = comment.Content,
                SubmitTime = DateTime.UtcNow,
                Username = HttpContext.User.Identity.Name
            };

            commentService.Add(newComment);
            await commentService.SaveChangesAsync();

            return Created($"api/Comments/{newComment.CommentId}", newComment.CommentId);
        }

        public override async Task<IActionResult> GetComment([FromRoute] int id)
        {
            var commentModel = await commentService.FindByIdAsync(id);

            if (commentModel == null)
            {
                return NotFound();
            }

            return Json(ModelToViewModel(commentModel));


        }

        public override async Task<IActionResult> GetComments([FromQuery] int articleId, [FromQuery]int? pageNumber, [FromQuery]int? pageSize)
        {
            var comments = (await articleService.FindAFullyLoadArticleAsync(articleId)).Comments.Page(pageNumber, pageSize);

            var result = new CommentListViewModel()
            {
                PagingInfo = comments.ToPagingInfo(),
                List = comments.List.Select(ModelToViewModel)
            };

            return Json(result);
        }

        public override async Task<IActionResult> RemoveComment([FromQuery] int commentId)
        {
            var comment = await commentService.FindByIdAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            var username = HttpContext.User.Identity.Name;
            if (username != comment.Username && await userService.GetRoleAsync(username) != Role.Admin)
            {
                return Forbid();
            }

            await commentService.RemoveAsync(commentId);
            await commentService.SaveChangesAsync();

            return Ok();
        }
    }
}
