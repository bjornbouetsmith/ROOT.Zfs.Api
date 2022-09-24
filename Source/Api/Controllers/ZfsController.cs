using System.Linq;
using Api.Core;
using Api.Models;
using Api.Models.Zfs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Info;

namespace Api.Controllers
{
    [Route("api/zfs")]
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

        /// <summary>
        /// Gets ZFS version info
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Gets a list of properties that can be set on a dataset
        /// </summary>
        [HttpGet("/api/zfs/info/datasets/properties")]
        public Response<Property[]> GetAvailableDataSetProperties()
        {
            var properties = Zfs.Properties.GetAvailableDataSetProperties(_remote.RemoteProcessCall);


            return new Response<Property[]> { Data = properties.ToArray() };
        }

        /// <summary>
        /// Gets a list of properties that can be set on a pool
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/zfs/info/pools/properties")]
        public Response<Property[]> GetAvailablePoolProperties()
        {
            var properties = Zfs.Properties.GetAvailableDataSetProperties(_remote.RemoteProcessCall);


            return new Response<Property[]> { Data = properties.ToArray() };
        }
    }
}
