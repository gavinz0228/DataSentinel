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
    [Route("/")]
    public class HomeController:ControllerBase
    {
        [HttpGet("/")]
        public ActionResult Index()
        {
            string html = $"<html><body>My services are up and running. <a href='connection/bygavin.com'>Check other server connection</a></body></html>";
            return new ContentResult(){Content = html, ContentType = "text/html"};
        }
    }
}