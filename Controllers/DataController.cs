using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using DataSentinel.DataLayer;

namespace DataSentinel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        protected IDataRepository _dataRepository;
        public DataController(IDataRepository dataRepository)
        {
            this._dataRepository = dataRepository;   
        }

        [HttpGet("Get/{collection}/{keyColumn}/{value}")]
        public async Task<ActionResult> Get(string collection, string keyColumn, string value)
        {
            var result = await this._dataRepository.Get(collection, keyColumn, value);
            return new OkObjectResult(result);
        }
        [HttpPost("Add/{collection}")]
        public async Task<ActionResult> Add(string collection, [FromBody] string jsonObj)
        {
            await this._dataRepository.Add(collection, jsonObj);
            return Ok();
        }

        // DELETE api/values/5
        [HttpDelete("Delete/{collection}/{keyColumn}/{value}")]
        public async Task Delete(string collection, string keyColumn, string value)
        {
            await this._dataRepository.Delete(collection, keyColumn, value);
        }
    }
}
