using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using DataSentinel.DataLayer;
using DataSentinel.Services;
using DataSentinel.Infrastructure;
using DataSentinel.Infrastructure.InputFormaters;
using DataSentinel.Infrastructure.Middlewares;
using DataSentinel.Utilities;
namespace DataSentinel
{
    public class Startup
    {
        public AppConfig _appConfig;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var authSecretKey =  System.Text.Encoding.UTF8.GetBytes(
                SystemUtility.GetEnvironmentVariableAsString(Configuration.GetSectionAsString("Constants:KEY_TOKEN_SECRET"))
            );
            services.Configure<AppConfig>(options=>{
                options.ConnectionString = SystemUtility.GetEnvironmentVariableAsString(Configuration.GetSectionAsString("Constants:KEY_DB_CONNECTION")) ;
                options.DatabaseName = SystemUtility.GetEnvironmentVariableAsString(Configuration.GetSectionAsString("Constants:KEY_DB_NAME"));
                options.LoginUserNameKey = Configuration.GetSectionAsString("Constants:KEY_LOGIN_USER_NAME");
                options.LoginPasswordKey = Configuration.GetSectionAsString("Constants:KEY_LOGIN_PASSWORD");
                options.TokenSecretKey = Configuration.GetSectionAsString("Constants:KEY_TOKEN_SECRET");
                options.SecretKey = authSecretKey;
                _appConfig = options;
            });

            services.AddMvc( options =>{
                options.InputFormatters.Insert(0, new JsonStringBodyInputFormatter());
            }
            ).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(authSecretKey),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddScoped<IDataRepository, MongoDbDataRepository>();
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMvc();

        }
    }
}
