using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using VicBlogServer.Configs;
using VicBlogServer.Data;
using VicBlogServer.DataService;
using VicBlogServer.Filters;
using VicBlogServer.Models;

namespace VicBlogServer
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("Configs/appsettings.json");


            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = Configuration["ApiInfo:Title"],
                    Version = Configuration["ApiInfo:Version"],
                    Description = Configuration["ApiInfo:Description"],
                    Contact = new Contact
                    {
                        Name = Configuration["ApiInfo:Contact:Name"],
                        Email = Configuration["ApiInfo:Contact:Email"]
                    },
                    License = new License
                    {
                        Name = "Apache 2.0",
                        Url = "http://www.apache.org/licenses/LICENSE-2.0.html"
                    }
                });
                c.AddSecurityDefinition("JWT Bearer", 
                    new ApiKeyScheme() {
                        In = "header",
                        Description = "Please insert JWT with \"Bearer\" into field",
                        Name = "Authorization",
                        Type = "apiKey"
                    });
            });
            services.AddCors();

            services.AddDbContext<BlogContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            JwtConfig.Initialize(Configuration);
            DataInitializer.RootPassword = Configuration["RootPassword"];

            services.AddIdentity<UserModel, Role>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 0;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<BlogContext>()
                .AddDefaultTokenProviders();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(config =>
                {
                    config.RequireHttpsMetadata = false;
                    config.SaveToken = true;
                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = JwtConfig.Config.Issuer,
                        ValidAudience = JwtConfig.Config.Issuer,
                        IssuerSigningKey = JwtConfig.Config.KeyObject,
                        ClockSkew = TimeSpan.Zero, // remove delay of token when expire
                        NameClaimType = ClaimTypes.NameIdentifier,
                        RoleClaimType = ClaimTypes.Role
                    };
                });

            services.AddScoped<IAccountDataService, AccountDataController>();
            services.AddScoped<IArticleDataService, ArticleDataController>();
            services.AddScoped<ITagDataService, ArticleTagDataController>();
            services.AddScoped<ILikeDataService, ArticleLikeDataController>();
            services.AddScoped<ICommentDataService, CommentDataController>();
            services.AddScoped<DataInitializer, DataInitializer>();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DataInitializer dataInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseAuthentication();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "VicBlog API"));

            app.UseMvc();

            if (env.IsDevelopment())
            {
                dataInitializer.InitializeDev().Wait();
            } else
            {
                dataInitializer.InitializeProduction().Wait();
            }
            
        }

    }
}
