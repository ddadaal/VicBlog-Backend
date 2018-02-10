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
    public class AccountData : IAccountDataService
    {
        private readonly SignInManager<UserModel> signInManager;
        private readonly UserManager<UserModel> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IConfiguration configuration;

        public IQueryable<UserModel> Users => userManager.Users;

        public IQueryable<Role> Roles => roleManager.Roles;

        public AccountData(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, 
            IConfiguration configuration, RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.roleManager = roleManager;

            InitializeRole().Wait();
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


        public async Task<UserModel> GetUser(string username)
        {
            return userManager.Users.FirstOrDefault(x => x.UserName == username);
        }

        public async Task<bool> Login(string username, string password)
        {
            var result = await signInManager.PasswordSignInAsync(username, password, false, false);
            return result.Succeeded;
        }

        public async Task<RegisterResult> Register(string username, string password, string roleName = Role.User)
        {
            var user = new UserModel
            {
                UserName = username,
            };
            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var registeredUser = userManager.Users.FirstOrDefault(x => x.UserName == username);
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

        public async Task<string> GetRole(string username)
        {
            var roles = await userManager.GetRolesAsync(await GetUser(username));
            return roles.FirstOrDefault();
        }
    }
}
