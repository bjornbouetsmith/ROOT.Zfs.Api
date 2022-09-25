using System;
using System.Linq;
using Api.Core;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;

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
    }
}
