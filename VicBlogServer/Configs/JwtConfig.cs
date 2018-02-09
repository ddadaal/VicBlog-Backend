using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicBlogServer.Configs
{


    public class JwtConfig
    {
        public static JwtConfig Config { get; private set; }

        public string Issuer { get; private set; }
        public string Key { get; private set; }
        public SymmetricSecurityKey KeyObject => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
        public int ExpireDays { get; private set; }

        internal static void Initialize(IConfiguration configuration)
        {
            Config = new JwtConfig()
            {
                Issuer = configuration["Jwt:Issuer"],
                Key = configuration["Jwt:Key"],
                ExpireDays = int.Parse(configuration["Jwt:ExpireDays"])
            };
        }
    }
}
