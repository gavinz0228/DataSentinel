using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using DataSentinel.DataLayer;
using DataSentinel.Utilities;
namespace DataSentinel.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class ConnectionController: ControllerBase
    {
        
        [HttpGet("{host}/")]
        public ActionResult Index(string host)
        {
            bool result = NetworkUtility.CanPing(host, 10000);
            string message = result == true? "up and runing": "not reachable";
            return  new OkObjectResult($"Host {host} is {message}!");

        }
    }

}