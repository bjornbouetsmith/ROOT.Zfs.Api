using System.Collections.Generic;
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
    [Route("api/zfs/datasets")]
    [ApiController]
    public class DataSetController : ControllerBase
    {
        private readonly IZfs _zfs;

        public DataSetController(IZfsAccessor zfsAccessor)
        {
            _zfs = zfsAccessor.Zfs;
            
        }

        /// <summary>
        /// Gets a list of all datasets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Response<IEnumerable<DataSet>> GetDataSets()
        {
            var dataSets = _zfs.DataSets.GetDataSets();
            return new Response<IEnumerable<DataSet>> { Data = dataSets };
        }

        /// <summary>
        /// Deletes the given dataset
        /// </summary>
        [HttpDelete("/api/zfs/datasets/{name}")]
        public Response DeleteDataSet(string name)
        {
            _zfs.DataSets.DestroyDataSet(name);

            return new Response();
        }

        /// <summary>
        /// Creates a dataset with the given properties if any
        /// </summary>
        /// <param name="name">The name of the dataset.
        /// This must include the name of the pool in the format
        /// [pool]/[preceedingdataset]/[dataset]</param>
        /// <param name="properties">Any properties that should be set when creating the dataset. This is not required and an empty array can be sent</param>
        [HttpPost("/api/zfs/datasets/{name}")]
        public Response<DataSet> CreateDataSet(string name, [FromBody] PropertyData[] properties)
        {
            // TODO: validate incoming properties for name/value - if not zfs will throw exception
            var propertyValues = properties.Select(p => new PropertyValue(p.Name, p.Source, p.Value)).ToArray();
            try
            {
                var dataset = _zfs.DataSets.CreateDataSet(name, propertyValues);

                return new Response<DataSet> { Data = dataset };
            }
            catch (ProcessCallException e)
            {
                Response.StatusCode = 500;
                return new Response<DataSet> { Status = ResponseStatus.Failure, ErrorText = e.ToString() };
            }

        }

        /// <summary>
        /// Gets information about the given dataset
        /// </summary>
        /// <param name="name">The name of the dataset</param>
        [HttpGet("/api/zfs/datasets/{name}")]
        public Response<DataSet> GetDataSet(string name)
        {
            var dataset =_zfs.DataSets.GetDataSet(name);

            if (dataset == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return new Response<DataSet> { Data = dataset };
        }
    }
}
