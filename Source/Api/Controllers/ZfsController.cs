using Api.Core;
using Api.Models;
using Api.Models.Zfs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Zfs.Core;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    //[Route("[controller]")]
    [ApiController]
    public class ZfsController : ControllerBase
    {
        private readonly IRemoteConnection _remote;

        public ZfsController(IRemoteConnection remote)
        {
            _remote = remote;
        }

        // GET: api/<ZfsController>
        [HttpGet]
        public Response<ZfsInfo> Get()
        {
            var versionCall = Zfs.ProcessCalls.GetVersion();
            if (_remote.RemoteProcessCall != null)
            {
                versionCall = _remote.RemoteProcessCall | versionCall;
            }
            var response = versionCall.LoadResponse();
            if (response.Success)
            {
                return new Response<ZfsInfo> { Data = new ZfsInfo { Version = response.StdOut } };
            }


            return new Response<ZfsInfo> { Status = ResponseStatus.Failure, ErrorText = response.StdError };

        }
    }
}
