using Api.Core;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;
using System.Linq;

namespace Api.Controllers
{
    /// <summary>
    /// Contains methods for manipulating snapshot holds
    /// </summary>
    [Authorize]
    [ApiController]
    public class HoldsController : ApiControllerBase
    {
        private readonly IZfs _zfs;

        /// <inheritdoc />
        public HoldsController(IZfsAccessor zfsAccessor)
        {
            _zfs = zfsAccessor.Zfs;
        }

        [HttpGet("/api/zfs/holds/{dataset}/{snapshot}")]
        public Response<SnapshotHold[]> ListHolds(string dataset, string snapshot, [FromQuery] bool recursive)
        {
            SnapshotHoldsArgs args = new SnapshotHoldsArgs { Dataset = dataset, Snapshot = snapshot, Recursive = recursive };
            if (!args.Validate(out var errors))
            {
                return ToErrorResponse<SnapshotHold[]>(errors);
            }
            var result = _zfs.Snapshots.Holds(args);
            return new Response<SnapshotHold[]>(result.ToArray());
        }

        [HttpPost("/api/zfs/holds/{dataset}/{snapshot}")]
        public Response CreateHold(string dataset, string snapshot, [FromBody] string tag, [FromQuery] bool recursive)
        {
            SnapshotHoldArgs args = new SnapshotHoldArgs { Dataset = dataset, Snapshot = snapshot, Tag = tag, Recursive = recursive };
            if (!args.Validate(out var errors))
            {
                return ToErrorResponse(errors);
            }
            _zfs.Snapshots.Hold(args);
            return new Response();
        }

        [HttpDelete("/api/zfs/holds/{dataset}/{snapshot}/{tag}")]
        public Response ReleaseHold(string dataset, string snapshot, string tag, [FromQuery] bool recursive)
        {
            SnapshotReleaseArgs args = new SnapshotReleaseArgs { Dataset = dataset, Snapshot = snapshot, Tag = tag, Recursive = recursive };
            if (!args.Validate(out var errors))
            {
                return ToErrorResponse(errors);
            }
            _zfs.Snapshots.Release(args);
            return new Response();
        }
    }
}
