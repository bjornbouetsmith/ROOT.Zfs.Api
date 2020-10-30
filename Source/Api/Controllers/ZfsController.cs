using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.Models.Zfs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Info;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    //[Route("[controller]")]
    [ApiController]
    public class ZfsController : ControllerBase
    {
        // GET: api/<ZfsController>
        [HttpGet]
        public Response<ZfsInfo> Get()
        {



            var versionCall = Zfs.ProcessCalls.GetVersion();

            var response = versionCall.LoadResponse();
            if (response.Success)
            {
                return new Response<ZfsInfo> { Data = new ZfsInfo { Version = response.StdOut } };
            }


            return new Response<ZfsInfo> { Status = ResponseStatus.Failure, ErrorText = response.StdError };

        }
       


        // POST api/<ZfsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ZfsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ZfsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
