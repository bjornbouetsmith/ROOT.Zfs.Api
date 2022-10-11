using System.Linq;
using Api.Core;
using Api.Models;
using Api.Models.Zfs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;

namespace Api.Controllers
{
    [Route("api/zfs")]
    [Authorize]
    //[Route("[controller]")]
    [ApiController]
    public class ZfsController : ControllerBase
    {
        private readonly IZfs _zfs;

        public ZfsController(IZfsAccessor zfsAccessor)
        {
            _zfs = zfsAccessor.Zfs;
        }

        /// <summary>
        /// Gets ZFS version info
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Response<ZfsInfo> Get()
        {
            var info = _zfs.GetVersionInfo();

            return new Response<ZfsInfo> { Data = new ZfsInfo { Version = info.Lines } };
        }

        /// <summary>
        /// Gets a list of properties that can be set on a dataset
        /// </summary>
        [HttpGet("/api/zfs/info/datasets/properties")]
        public Response<Property[]> GetAvailableDataSetProperties()
        {
            var properties = _zfs.Properties.GetAvailableDataSetProperties();
            
            return new Response<Property[]> { Data = properties.ToArray() };
        }

        /// <summary>
        /// Gets a list of properties that can be set on a pool
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/zfs/info/pools/properties")]
        public Response<Property[]> GetAvailablePoolProperties()
        {
            var properties = _zfs.Properties.GetAvailableDataSetProperties();
            
            return new Response<Property[]> { Data = properties.ToArray() };
        }

        /// <summary>
        /// Gets a list of disks
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/zfs/info/disks")]
        public Response<DiskInfo[]> GetDisks()
        {
            var disks = _zfs.ListDisks();
            return new Response<DiskInfo[]> { Data = disks.ToArray() };
        }
    }
}
