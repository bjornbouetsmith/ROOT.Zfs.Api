using System;
using System.Linq;
using Api.Core;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;

namespace Api.Controllers
{
    /// <summary>
    /// Contains access to the pool as a resource
    /// </summary>
    [Authorize]
    [ApiController]
    public class ZPoolController : ControllerBase
    {
        private readonly IZfs _zfs;

        /// <summary>
        /// Creates a zpool controller
        /// </summary>
        public ZPoolController(IZfsAccessor zfsAccessor)
        {
            _zfs = zfsAccessor.Zfs;
        }

        /// <summary>
        /// Gets the command history for the given pool.
        /// </summary>
        /// <param name="pool">The name of the zpool to get the command history from</param>
        /// <param name="skipLines">Number of lines to skip - if you want to page</param>
        /// <param name="greaterThan">Only return history newer than this date</param>
        /// <returns>An array of command history objects</returns>
        [HttpGet("/api/zfs/pools/{pool}/history")]
        public Response<CommandHistory[]> GetCommandHistory(string pool, [FromQuery] int skipLines = 0, [FromQuery] DateTime greaterThan = default)
        {
            var history = _zfs.Pool.GetHistory(pool, skipLines);

            return new Response<CommandHistory[]> { Data = history.ToArray() };
        }

        [HttpGet("/api/zfs/pools")]
        public Response<PoolInfo[]> GetPoolInfos()
        {
            var infos = _zfs.Pool.GetAllPoolInfos();

            return new Response<PoolInfo[]> { Data = infos?.ToArray() };
        }

        [HttpGet("/api/zfs/pools/{pool}/info")]
        public Response<PoolInfo> GetPoolInfo(string pool)
        {
            var info = _zfs.Pool.GetPoolInfo(pool);

            return new Response<PoolInfo> { Data = info };
        }

        [HttpGet("/api/zfs/pools/{pool}/status")]
        public Response<PoolStatus> GetPoolStatus(string pool)
        {
            var status = _zfs.Pool.GetStatus(pool);
            return new Response<PoolStatus> { Data = status };
        }

        /// <summary>
        /// Creates a zfs pool with the given name and args
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost("/api/zfs/pools")]
        public Response<PoolStatus> CreatePool([FromBody]PoolCreationArgs args)
        {
            var status = _zfs.Pool.CreatePool(args);

            return new Response<PoolStatus> { Data = status };
        }
    }
}
