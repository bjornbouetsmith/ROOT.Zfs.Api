using System;
using System.Linq;
using Api.Core;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Info;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    public class SnapshotController : ControllerBase
    {
        private readonly IRemoteConnection _remote;

        public SnapshotController(IRemoteConnection remote)
        {
            _remote = remote;
        }

        /// <summary>
        /// Gets a list of snapshots in the given dataset
        /// </summary>
        /// <param name="dataset">The dataset to return snapshots for</param>
        [HttpGet("/api/zfs/datasets/{dataset}/snapshots")]
        public Response<Snapshot[]> GetSnapshots(string dataset)
        {
            ProcessCall pc;
            if (_remote.RemoteProcessCall != null)
            {
                pc = _remote.RemoteProcessCall | Snapshots.ProcessCalls.ListSnapshots(dataset);
            }
            else
            {
                pc = Snapshots.ProcessCalls.ListSnapshots(dataset);
            }

            var response = pc.LoadResponse();
            if (response.Success)
            {
                var snapshots = SnapshotParser.Parse(response.StdOut).ToArray();
                return new Response<Snapshot[]> { Data = snapshots, Status = ResponseStatus.Success };
            }

            return new Response<Snapshot[]> { Status = ResponseStatus.Failure, ErrorText = response.StdError };
        }

        /// <summary>
        /// Deletes the snapshot in the given dataset
        /// </summary>
        /// <param name="dataset">The dataset where the snapshot resides</param>
        /// <param name="snapshot">The snapshot to delete</param>
        [HttpDelete("/api/zfs/datasets/{dataset}/snapshots/{snapshot}")]
        public Response DeleteSnapshot(string dataset, string snapshot)
        {
            ProcessCall pc;
            var atIndex = snapshot.IndexOf("@", StringComparison.InvariantCultureIgnoreCase);
            var rawSnapshot = atIndex > -1 ? snapshot.Substring(atIndex + 1) : snapshot;
            if (_remote.RemoteProcessCall != null)
            {
                pc = _remote.RemoteProcessCall | Snapshots.ProcessCalls.DestroySnapshot(dataset, rawSnapshot);
            }
            else
            {
                pc = Snapshots.ProcessCalls.DestroySnapshot(dataset, rawSnapshot);
            }

            var response = pc.LoadResponse();
            if (response.Success)
            {
                return new Response { Status = ResponseStatus.Success };
            }

            return new Response<Snapshot[]> { Status = ResponseStatus.Failure, ErrorText = response.StdError };
        }

        /// <summary>
        /// Creates a new snapshot in the given dataset
        /// </summary>
        /// <param name="dataset">The dataset to create a snapshot for</param>
        /// <param name="snapshot">The name of the snapshot</param>
        [HttpPost("/api/zfs/datasets/{dataset}/snapshots")]
        public Response CreateSnapshot(string dataset, [FromBody] string snapshot)
        {
            ProcessCall pc;

            // Trim values before @ if there, so we only pass on the raw name to zfs
            var atIndex = snapshot.IndexOf("@", StringComparison.InvariantCultureIgnoreCase);
            var rawSnapshot = atIndex > -1 ? snapshot.Substring(atIndex + 1) : snapshot;
            if (_remote.RemoteProcessCall != null)
            {
                pc = _remote.RemoteProcessCall | Snapshots.ProcessCalls.CreateSnapshot(dataset, rawSnapshot);
            }
            else
            {
                pc = Snapshots.ProcessCalls.CreateSnapshot(dataset, rawSnapshot);
            }

            var response = pc.LoadResponse();
            if (response.Success)
            {
                return new Response { Status = ResponseStatus.Success };
            }

            return new Response<Snapshot[]> { Status = ResponseStatus.Failure, ErrorText = response.StdError };
        }
    }
}
