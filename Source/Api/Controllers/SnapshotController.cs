using System;
using System.Linq;
using Api.Core;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Snapshots;
using ROOT.Zfs.Public.Data;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    public class SnapshotController : ApiControllerBase
    {
        private readonly IZfs _zfs;

        public SnapshotController(IZfsAccessor zfsAccessor)
        {
            _zfs = zfsAccessor.Zfs;
        }

        /// <summary>
        /// Gets a list of snapshots in the given dataset
        /// </summary>
        /// <param name="dataset">The dataset to return snapshots for</param>
        /// <param name="includeChildren">Whether or not to also return child datasets for the given root dataset - defaults to true</param>
        [HttpGet("/api/zfs/datasets/{dataset}/snapshots")]
        public Response<Snapshot[]> GetSnapshots(string dataset, [FromQuery] bool includeChildren = true)
        {
            var args = new SnapshotListArgs { Root = dataset, IncludeChildren = includeChildren };
            if (!args.Validate(out var errors))
            {
                return ToErrorResponse<Snapshot[]>(errors);
            }
            var snapshots = _zfs.Snapshots.List(args);

            return new Response<Snapshot[]> { Data = snapshots.ToArray() };
        }

        /// <summary>
        /// Deletes the snapshot in the given dataset
        /// </summary>
        /// <param name="dataset">The dataset where the snapshot resides</param>
        /// <param name="snapshot">The snapshot to delete</param>
        /// <param name="isExactName">Whether or not the snapshot name is to be used for exact match, or as a prefix.
        /// Optional to pass this, defaults to true, which means exact match on snapshot name</param>
        [HttpDelete("/api/zfs/datasets/{dataset}/snapshots/{snapshot}")]
        public Response DeleteSnapshot(string dataset, string snapshot, [FromQuery] bool isExactName = true)
        {
            var args = new SnapshotDestroyArgs { Dataset = dataset, Snapshot = snapshot, IsExactName = isExactName };
            if (!args.Validate(out var errors))
            {
                return ToErrorResponse(errors);
            }
            _zfs.Snapshots.Destroy(args);

            return new Response();
        }

        /// <summary>
        /// Creates a new snapshot in the given dataset
        /// </summary>
        /// <param name="dataset">The dataset to create a snapshot for</param>
        /// <param name="snapshot">The name of the snapshot</param>
        [HttpPost("/api/zfs/datasets/{dataset}/snapshots")]
        public Response CreateSnapshot(string dataset, [FromBody] string snapshot)
        {
            var args = new SnapshotCreateArgs{ Dataset = dataset, Snapshot = snapshot };
            if (!args.Validate(out var errors))
            {
                return ToErrorResponse(errors);
            }

            _zfs.Snapshots.Create(args);

            return new Response();
        }
    }
}
