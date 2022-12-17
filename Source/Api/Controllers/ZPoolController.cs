using System;
using System.Linq;
using Api.Core;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;

namespace Api.Controllers
{
    /// <summary>
    /// Contains access to the pool as a resource
    /// </summary>
    [Authorize]
    [ApiController]
    public class ZPoolController : ApiControllerBase
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
            var args = new PoolHistoryArgs { PoolName = pool, SkipLines = skipLines, AfterDate = greaterThan };
            if (!args.Validate(out var errors))
            {
                return ToErrorResponse<CommandHistory[]>(errors);
            }

            var history = _zfs.Pool.History(args);

            return new Response<CommandHistory[]> { Data = history.ToArray() };
        }
        /// <summary>
        /// Gets pool information - data comes from zpool info
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/zfs/pools")]
        public Response<PoolInfo[]> GetPoolInfos()
        {
            var infos = _zfs.Pool.List();

            return new Response<PoolInfo[]> { Data = infos?.ToArray() };
        }

        [HttpGet("/api/zfs/pools/{pool}/info")]
        public Response<PoolInfo> GetPoolInfo(string pool)
        {
            var args = new PoolListArgs { PoolName = pool };
            if (!args.Validate(out var errors))
            {
                return ToErrorResponse<PoolInfo>(errors);
            }

            var info = _zfs.Pool.List(args);

            return new Response<PoolInfo> { Data = info };
        }

        [HttpGet("/api/zfs/pools/{pool}/status")]
        public Response<PoolStatus> GetPoolStatus(string pool)
        {
            var args = new PoolStatusArgs { PoolName = pool };
            if (!args.Validate(out var errors))
            {
                return ToErrorResponse<PoolStatus>(errors);
            }
            var status = _zfs.Pool.Status(args);;
            return new Response<PoolStatus> { Data = status };
        }

        /// <summary>
        /// Creates a zfs pool with the given arguments
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost("/api/zfs/pools")]
        public Response<PoolStatus> CreatePool([FromBody]PoolCreateArgs args)
        {
            if (!args.Validate(out var errors))
            {
                return ToErrorResponse<PoolStatus>(errors);
            }

            var status = _zfs.Pool.Create(args);

            return new Response<PoolStatus> { Data = status };
        }
        /// <summary>
        /// Destroy the given pool.
        /// Use with caution.
        /// </summary>
        /// <param name="pool">The name of the pool to destroy</param>
        /// <returns></returns>
        [HttpDelete("/api/zfs/pools/{pool}")]
        public Response DestroyPool(string pool)
        {
            var args = new PoolDestroyArgs { PoolName = pool };
            if (!args.Validate(out var errors))
            {
                return ToErrorResponse<Response>(errors);
            }
            _zfs.Pool.Destroy(args);
            return new Response();
        }
    }
}
