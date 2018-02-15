using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VicBlogServer.Configs;
using VicBlogServer.DataService;
using VicBlogServer.Models;

namespace VicBlogServer.Data
{
    public class AccountDataController : IAccountDataService
    {
        private readonly SignInManager<UserModel> signInManager;
        private readonly UserManager<UserModel> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IConfiguration configuration;

        public IQueryable<UserModel> Users => userManager.Users;

        public IQueryable<Role> Roles => roleManager.Roles;

        public AccountDataController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, 
            IConfiguration configuration, RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.roleManager = roleManager;
        }

        public UserModel GetUser(string username)
        {
            return userManager.Users.SingleOrDefault(x => x.UserName == username);
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var result = await signInManager.PasswordSignInAsync(username, password, false, false);
            return result.Succeeded;
        }

        public async Task<RegisterResult> RegisterAsync(string username, string password, string roleName = Role.User)
        {
            var user = new UserModel
            {
                UserName = username,
                RegisterTime = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var registeredUser = userManager.Users.SingleOrDefault(x => x.UserName == username);
                await userManager.AddToRoleAsync(registeredUser, Role.User);
                return new RegisterResult()
                {
                    Succeeded = true,
                };
            }
            else
            {
                return new RegisterResult()
                {
                    Succeeded = false,
                    Errors = result.Errors
                };
            }
        }

        public async Task<string> GetRoleAsync(string username)
        {
            var roles = await userManager.GetRolesAsync(GetUser(username));
            return roles.SingleOrDefault();
        }
    }
}
