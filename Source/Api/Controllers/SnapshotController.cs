using System;
using System.Collections.Generic;
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
    }
}
