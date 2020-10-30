using System.Collections.Generic;
using System.Linq;
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
        [HttpGet]
        public Response<IEnumerable<DataSet>> GetDataSets()
        {
            var dataSets = Zfs.GetDataSets();
            return new Response<IEnumerable<DataSet>> { Data = dataSets };
        }
        

        [HttpGet("/api/zfs/datasets/{name}")]
        public Response<DataSet> GetDataSet(string name)
        {
            var dataset = Zfs.GetDataSets().FirstOrDefault(ds=>ds.Name.Equals(name,System.StringComparison.OrdinalIgnoreCase));
            if (dataset == null)
            {
                Response.StatusCode = 404;
            }

            return new Response<DataSet> { Data = dataset };
        }
    }
}
