using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using DataSentinel.DataLayer;
using DataSentinel.ViewModels;
using DataSentinel.Services;
namespace DataSentinel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        protected IUserService _userService;
        public AuthController(IUserService userService)
        {
            this._userService = userService;
        }
        [HttpPost("login")]
        public string Login([FromBody] UserLoginViewModel userLogin)
        {
            string token;
            this._userService.Authenticate(userLogin.UserName, userLogin.Password, out token );
            return token;
        }
    }
    
}