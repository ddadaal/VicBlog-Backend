using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VicBlogServer.Configs;

namespace VicBlogServer.Models
{
    public class UserModel : IdentityUser
    {
        public static string GenerateToken(string username)
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
    }

    public class Role: IdentityRole
    {
        public const string Admin = "Admin";
        public const string User = "User";

        public Role() { }
        public Role(string roleName) : base(roleName) { }
    }
}
