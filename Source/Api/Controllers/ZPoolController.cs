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
    [Authorize]
    [ApiController]
    public class ZPoolController : ControllerBase
    {
        private readonly IZfs _zfs;

        public ZPoolController(IZfsAccessor zfsAccessor)
        {
            _zfs = zfsAccessor.Zfs;
        }

        [HttpGet("/api/zfs/pools/{pool}/history")]
        public Response<CommandHistory[]> GetHistory(string pool, [FromQuery] int skipLines = 0, [FromQuery] DateTime greaterThan = default)
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
    }
}
