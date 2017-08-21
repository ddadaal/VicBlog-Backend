using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlog.Models;

namespace VicBlog.Controllers
{
    public partial class DefaultApiController : Controller
    {



        [HttpGet]
        [Route("/login")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerOperation("LoginGet")]
        [SwaggerResponse(200, type: typeof(User), description: "Login successful. Returns token, username, registerTime and role.")]
        [SwaggerResponse(401, description: "Credential provided is not valid.")]
        public IActionResult LoginGet([FromQuery]UserLoginModel data)
        {
            var user = context.Users.Find(data.Username);
            if (user == null || user.Password != data.Password)
            {
                return Unauthorized();
            }

            var now = DateTime.Now;

            var returnData = new UserLoginSuccessModel()
            {
                Username = user.Username,
                Role = user.Role,
                Token = user.ComputeToken(now),
                LoginTime = now
            };

            return Json(returnData);
        }

        [HttpPost]
        [Route("/register")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerOperation("RegisterPost")]
        [SwaggerResponse(409, description: "Username exists.")]
        [SwaggerResponse(201, type: typeof(UserLoginSuccessModel), description: "User registered successfully. Returns user info.")]
        public async Task<IActionResult> RegisterPost([FromBody]UserRegisterModel data)
        {
            var user = await context.Users.FindAsync(data.Username);
            if (user != null)
            {
                return StatusCode(409);
            }

            User newUser = new Models.User()
            {
                Password = data.Password.ComputeMD5(),
                Username = data.Username,
                RegisterTime = DateTime.Now,
                Role = Role.User,
            };


            context.Users.Add(newUser);
            await context.SaveChangesAsync();

            UserLoginSuccessModel model = new UserLoginSuccessModel()
            {
                Username = newUser.Username,
                Role = newUser.Role,
                LoginTime = DateTime.Now,
                Token = newUser.ComputeToken(DateTime.Now)
            };

            return Created("/users/" + data.Username, model);


        }

        [HttpGet]
        [Route("/users/{username}")]
        [SwaggerOperation("RegisterPost")]
        [SwaggerResponse(200, type: typeof(User), description: "Returns user profile.")]
        [SwaggerResponse(404, description: "Username not found!")]
        public IActionResult UsersUsernameGet([FromRoute]string username)
        {
            var user = context.Users.Find(username);
            if (user != null)
            {
                return Json(user);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
