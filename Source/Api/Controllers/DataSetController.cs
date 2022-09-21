using System.Collections.Generic;
using System.Linq;
using Api.Core;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Zfs.Core;
using ROOT.Zfs.Core.Info;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/zfs/datasets")]
    [ApiController]
    public class DataSetController : ControllerBase
    {
        private readonly IRemoteConnection _remoteConnection;

        public DataSetController(IRemoteConnection remoteConnection)
        {
            _remoteConnection = remoteConnection;
        }
        [HttpGet]
        public Response<IEnumerable<DataSet>> GetDataSets()
        {
            var dataSets = Zfs.DataSets.GetDataSets(_remoteConnection.RemoteProcessCall);
            return new Response<IEnumerable<DataSet>> { Data = dataSets };
        }

        [HttpDelete("/api/zfs/datasets/{name}")]
        public Response DeleteDataSet(string name)
        {
            Zfs.DataSets.DeleteDataSet(name, _remoteConnection.RemoteProcessCall);

            return new Response();
        }

        [HttpGet("/api/zfs/datasets/{name}")]
        public Response<DataSet> GetDataSet(string name)
        {
            var dataset = Zfs.DataSets.GetDataSet(name, _remoteConnection.RemoteProcessCall);
            if (dataset == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return new Response<DataSet> { Data = dataset };
        }
    }
}
