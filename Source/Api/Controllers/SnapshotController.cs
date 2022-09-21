using System.Collections.Generic;
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
        public Response<IEnumerable<Snapshot>> GetSnapshots(string dataset)
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
                var snapshots = SnapshotParser.Parse(response.StdOut);
                return new Response<IEnumerable<Snapshot>> { Data = snapshots, Status = ResponseStatus.Success };
            }

            return new Response<IEnumerable<Snapshot>> { Status = ResponseStatus.Failure, ErrorText = response.StdError };
        }
    }
}
