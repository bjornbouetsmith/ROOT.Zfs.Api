using System;
using System.Collections.Generic;
using System.Linq;
using Api.Core;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Shared.Utils.OS;
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


        [HttpPost("/api/zfs/datasets/{name}")]
        public Response<DataSet> CreateDataSet(string name, [FromBody] PropertyData[] properties)
        {
            // TODO: validate incoming properties for name/value - if not zfs will throw exception
            var propertyValues = properties.Select(p => new PropertyValue(p.Name, p.Source, p.Value)).ToArray();
            try
            {
                var dataset = Zfs.DataSets.CreateDataSet(name, propertyValues, _remoteConnection.RemoteProcessCall);

                return new Response<DataSet> { Data = dataset };
            }
            catch (ProcessCallException e)
            {
                Response.StatusCode = 500;
                return new Response<DataSet> { Status = ResponseStatus.Failure, ErrorText = e.ToString() };
            }

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
