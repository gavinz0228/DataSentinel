using System;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using DataSentinel.Infrastructure;
using DataSentinel.Utilities;
using DataSentinel.DataLayer;
namespace DataSentinel.Services{
    public class UserService : IUserService
    {
        protected IOptions<AppConfig> _appConfig;
        protected IDataRepository _repository;
        protected IHttpContextAccessor _contextAccessor;
        public UserService(IOptions<AppConfig>  appConfig, IDataRepository repository, IHttpContextAccessor contextAccessor)
        {
            this._appConfig = appConfig;
            this._repository = repository;
            this._contextAccessor = contextAccessor;
        }
        public async Task<string> Authenticate(string userName, string password)
        {
            var clientIp = this._contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var isBlocked = await this._repository.IsBlacklisted(clientIp);
            if(isBlocked)
                throw new Exception("You've been blocked. Please contact our tech support.");
            if(userName == SystemUtility.GetEnvironmentVariableAsString(this._appConfig.Value.LoginUserNameKey)
                &&password == SystemUtility.GetEnvironmentVariableAsString(this._appConfig.Value.LoginPasswordKey))
            {
                await this._repository.RemoveWrongPassword(clientIp);
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[] 
                    {
                        new Claim(ClaimTypes.Name, userName)

                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(this._appConfig.Value.SecretKey), SecurityAlgorithms.HmacSha256Signature)
                };
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                string token = tokenHandler.WriteToken(securityToken);
                
                return token;
            }
            await this._repository.LogWrongPassword(clientIp);
            throw new Exception("User name or password is wrong!");
        }

    }
}