using System;
using System.Linq;
using Api.Core;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    public class SnapshotController : ControllerBase
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
        [HttpGet("/api/zfs/datasets/{dataset}/snapshots")]
        public Response<Snapshot[]> GetSnapshots(string dataset)
        {
            var snapshots = _zfs.Snapshots.GetSnapshots(dataset);

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
        public Response DeleteSnapshot(string dataset, string snapshot, [FromQuery]bool isExactName = true)
        {
            _zfs.Snapshots.DestroySnapshot(dataset, snapshot, isExactName);

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
            // Trim values before @ if there, so we only pass on the raw name to zfs
            var atIndex = snapshot.IndexOf("@", StringComparison.InvariantCultureIgnoreCase);
            var rawSnapshot = atIndex > -1 ? snapshot.Substring(atIndex + 1) : snapshot;

            _zfs.Snapshots.CreateSnapshot(dataset, rawSnapshot);

            return new Response();
        }
    }
}
