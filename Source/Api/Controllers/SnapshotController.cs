using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Zfs.Core;
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
            var remote = _remote.RemoteProcessCall | Snapshots.ProcessCalls.ListSnapshots(dataset);
            var response = remote.LoadResponse();
            if (response.Success)
            {
                var snapshots = SnapshotParser.Parse(response.StdOut);
                return new Response<IEnumerable<Snapshot>> { Data = snapshots };
            }

            return new Response<IEnumerable<Snapshot>> { Status = ResponseStatus.Failure, ErrorText = response.StdError };
        }
    }
}
