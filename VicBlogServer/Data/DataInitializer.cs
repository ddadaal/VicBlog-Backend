using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.DataService;
using VicBlogServer.Models;

namespace VicBlogServer.Data
{
    public class DataInitializer
    {

        public static string RootPassword { get; set; }

        private BlogContext context;
        private RoleManager<Role> roleManager;
        private UserManager<UserModel> userManager;
        private IAccountDataService accountDataService;

        public DataInitializer(BlogContext context, 
            RoleManager<Role> roleManager,
            UserManager<UserModel> userManager, 
            IAccountDataService accountDataService
            )
        {
            this.context = context;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.accountDataService = accountDataService;
        }

        public async Task Initialize()
        {
            context.Database.EnsureCreated();
            await roleManager.CreateAsync(new Role(Role.Admin));
            await roleManager.CreateAsync(new Role(Role.User));


            if (accountDataService.GetUser("root") == null)
            {
                await accountDataService.RegisterAsync("root", RootPassword, Role.Admin);
            }

        }

        public async Task InitializeProduction()
        {
            await Initialize();

        }

        public async Task InitializeDev()
        {
            await Initialize();
        }
    }
}
