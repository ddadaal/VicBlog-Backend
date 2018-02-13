using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using VicBlogServer.Configs;
using VicBlogServer.DataService;
using VicBlogServer.ViewModels;
using VicBlogServer.ViewModels.Dto;

namespace VicBlogServer.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public abstract class AccountControllerSpec : Controller
    {
        [HttpGet("Login")]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(UserLoginDto), description: "Login successful. Returns token, username, registerTime and role.")]
        [SwaggerResponse(400, description: "Credential provided is not valid.")]
        public abstract Task<IActionResult> Login([FromQuery]UserLoginDto model);

        [HttpPost("Register")]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(UserLoginDto), description: "Login successful. Returns token, username, registerTime and role.")]
        [SwaggerResponse(409, description: "Username already exists.")]
        [SwaggerResponse(400, description: "Problems occurred.")]
        public abstract Task<IActionResult> Register([FromQuery]UserRegisterDto model);

        [HttpPost("Test")]
        [Authorize]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(UserLoginDto), description: "Login successful. Returns token, username, registerTime and role.")]
        public abstract Task<IActionResult> TestAuth();

        [HttpGet("{username}")]
        [Authorize]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(UserViewModel), description: "Returns the user.")]
        [SwaggerResponse(404, description: "User specified doesn't exist.")]
        public abstract Task<IActionResult> GetAUser([FromRoute]string username);
    }

    public class AccountController : AccountControllerSpec
    {
        private readonly IAccountDataService accountService;

        public AccountController(IAccountDataService accountService)
        {
            this.accountService = accountService;

        }

        public override async Task<IActionResult> TestAuth()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var user = accountService.GetUser(HttpContext.User.Identity.Name);
                return Json($"Authenticated.{user.UserName}");
            }
            else
            {
                return Json("Not authenticated");
            }
        }

        public override async Task<IActionResult> Login([FromQuery]UserLoginDto model)
        {
            string username = model.Username;
            var result = await accountService.Login(username, model.Password);
            if (result)
            {
                var role = await accountService.GetRole(username);
                return Json(new UserLoginResponseDto()
                {
                    Token = Models.UserModel.GenerateToken(username, role),
                    Role = role,
                    Username = username
                });
            }
            else
            {
                return BadRequest();
            }

        }

        public override async Task<IActionResult> Register([FromQuery]UserRegisterDto model)
        {
            var username = model.Username;
            var result = await accountService.Register(username, model.Password);
            
            if (result.Succeeded)
            {
                var role = await accountService.GetRole(username);
                return Json(new UserLoginResponseDto()
                {
                    Token = Models.UserModel.GenerateToken(username, role),
                    Role = role,
                    Username = username
                });
            } else
            {
                if (result.ContainsDuplicateUsernameError)
                {
                    return StatusCode(StatusCodes.Status409Conflict);
                }
                else
                {
                    return BadRequest(result.Errors);
                }

            }
        }

        public override async Task<IActionResult> GetAUser([FromRoute] string username)
        {
            var user = accountService.GetUser(username);
            var role = await accountService.GetRole(username);

            if (user == null)
            {
                return NotFound();
            }

            var userVM = new UserViewModel()
            {
                Role = role,
                Username = username
            };

            return Json(userVM);

        }
    }
}
