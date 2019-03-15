using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using DataSentinel.DataLayer;

namespace DataSentinel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DataController : ControllerBase
    {
        protected IDataRepository _dataRepository;
        public DataController(IDataRepository dataRepository)
        {
            this._dataRepository = dataRepository;   
        }

        [HttpGet("get/{collection}/{filter}")]
        public async Task<ActionResult> Get(string collection, string filter)
        {
            var result = await this._dataRepository.Get(collection, filter);
            return new OkObjectResult(result);
        }
        [HttpPut("save/{collection}/{filter}")]
        public async Task<ActionResult> Save(string collection, string filter, [FromBody] Stream stream)
        {
            await this._dataRepository.Save(collection, stream, filter);
            return Ok();
        }
        [HttpPost("add/{collection}")]
        public async Task<ActionResult> Add(string collection, [FromBody] Stream stream)
        {
            await this._dataRepository.Add(collection, stream);
            return Ok();
        }
        // DELETE api/values/5
        [HttpDelete("delete/{collection}/{filter}")]
        public async Task Delete(string collection, string filter)
        {
            await this._dataRepository.Delete(collection, filter);
        }
    }
}
