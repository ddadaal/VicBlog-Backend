using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.Models;
using VicBlogServer.ViewModels;

namespace VicBlogServer.DataService
{


    public class RegisterResult
    {
        public bool Succeeded { get; set; }
        public IEnumerable<IdentityError> Errors { get; set; }
        public bool ContainsDuplicateUsernameError =>
            Errors != null &&
            Errors.Any(x => x.Code == new IdentityErrorDescriber().DuplicateUserName("").Code);
    }

    public interface IAccountDataService
    {
        /// <summary>
        /// Gets the IQueryable Users for advanced usage.
        /// </summary>
        IQueryable<UserModel> Users { get; }

        /// <summary>
        /// Gets the IQueryable Roles for advanced usage
        /// </summary>
        IQueryable<Role> Roles { get; }

        /// <summary>
        /// Attempts to log in with provided username and password.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Whether login is successful</returns>
        Task<bool> LoginAsync(string username, string password);

        /// <summary>
        /// Attempts to register with provided username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<RegisterResult> RegisterAsync(string username, string password, string roleName = Role.User);

        /// <summary>
        /// Gets a User with username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>The user or null if no user with that username is found</returns>
        UserModel GetUser(string username);

        /// <summary>
        /// Gets a Role with username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>The Role of null if username doesn't exist</returns>
        Task<string> GetRoleAsync(string username);
    }
}
