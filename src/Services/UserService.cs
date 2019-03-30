using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using DataSentinel.Infrastructure;
using DataSentinel.Utilities;
namespace DataSentinel.Services{
    public class UserService : IUserService
    {
        protected IOptions<AppConfig> _appConfig;
        public UserService(IOptions<AppConfig>  appConfig)
        {
            this._appConfig = appConfig;
        }
        public bool Authenticate(string userName, string password, out string token)
        {
            token = null;
            if(userName == SystemUtility.GetEnvironmentVariableAsString(this._appConfig.Value.LoginUserNameKey)
                &&password == SystemUtility.GetEnvironmentVariableAsString(this._appConfig.Value.LoginPasswordKey))
            {
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
                token = tokenHandler.WriteToken(securityToken);
                return true;
            }
            return false;
        }
    }
}