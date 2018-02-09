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
using VicBlogServer.Models;
using VicBlogServer.Models.Dto;

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
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IConfiguration configuration;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,IConfiguration configuration, RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.roleManager = roleManager;

            InitializeRole().Wait();
        }


        public override async Task<IActionResult> TestAuth()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var user = userManager.Users.FirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name);
                return Json($"Authenticated.{user.UserName}");
            }
            else
            {
                return Json("Not authenticated");
            }
        }

        private async Task InitializeRole()
        {
            await CreateRoleIfNotExist(Role.Admin);
            await CreateRoleIfNotExist(Role.User);
        }

        private async Task CreateRoleIfNotExist(string roleName)
        {
            if (!(await roleManager.RoleExistsAsync(roleName)))
            {
                var role = new Role(roleName);
                await roleManager.CreateAsync(role);
            }
        }

        

        public override async Task<IActionResult> Login([FromQuery]UserLoginDto model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

            if (result.Succeeded)
            {
                var user = userManager.Users.FirstOrDefault(x => x.UserName == model.Username);
                string role = (await userManager.GetRolesAsync(user)).FirstOrDefault();
                return Json(GenerateResponseDto(model.Username, role));
            }
            else
            {
                return BadRequest();
            }

        }

        private UserLoginResponseDto GenerateResponseDto(string username, string role)
        {
            return new UserLoginResponseDto()
            {
                Token = GenerateJwtToken(username),
                Role = role,
                Username = username
            };
        }

        private string GenerateJwtToken(string username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, username)
            };

            var key = JwtConfig.Config.KeyObject;
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(JwtConfig.Config.ExpireDays);

            var token = new JwtSecurityToken(
                JwtConfig.Config.Issuer,
                JwtConfig.Config.Issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public override async Task<IActionResult> Register([FromQuery]UserRegisterDto model)
        {
            var user = new User
            {
                UserName = model.Username,
            };
            var result = await userManager.CreateAsync(user, model.Password);
            
            if (result.Succeeded)
            {
                var registeredUser = userManager.Users.FirstOrDefault(x => x.UserName == model.Username);
                await userManager.AddToRoleAsync(registeredUser, Role.User);
                await signInManager.SignInAsync(user, false);
                return Json(GenerateResponseDto(model.Username, Role.User));
            } else
            {
                IEnumerable<string> s = result.Errors.Select(x => x.Code);
                var describer = new IdentityErrorDescriber();
                if (s.Contains(describer.DuplicateUserName(model.Username).Code))
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
