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
    }

    public class AccountController : AccountControllerSpec
    {
        private readonly IAccountDataService dataService;

        public AccountController(IAccountDataService dataService)
        {
            this.dataService = dataService;

        }


        public override async Task<IActionResult> TestAuth()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var user = await dataService.GetUser(HttpContext.User.Identity.Name);
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
            var result = await dataService.Login(username, model.Password);
            if (result)
            {
                var role = await dataService.GetRole(username);
                return Json(new UserLoginResponseDto()
                {
                    Token = Models.UserModel.GenerateToken(username),
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
            var result = await dataService.Register(username, model.Password);
            
            if (result.Succeeded)
            {
                var role = await dataService.GetRole(username);
                return Json(new UserLoginResponseDto()
                {
                    Token = Models.UserModel.GenerateToken(username),
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
    }
}
