using Api.Core;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;
using System.Collections.Generic;
using ROOT.Zfs.Public.Data.Datasets;
using System.Linq;
using ROOT.Zfs.Public.Arguments.Snapshots;

namespace Api.Controllers
{
    /// <summary>
    /// Contains methods for manipulating clones of snapshots, i.e.
    /// datasets that is clones of a snapshot
    /// </summary>
    [Authorize]
    [ApiController]
    public class ClonesController : ApiControllerBase
    {
        private readonly IZfs _zfs;

        /// <inheritdoc />
        public ClonesController(IZfsAccessor zfsAccessor)
        {
            _zfs = zfsAccessor.Zfs;
        }

        /// <summary>
        /// Lists datasets that are clones of snapshots
        /// </summary>
        /// <returns>A list of datasets that are clones</returns>
        [HttpGet("/api/zfs/clones")]
        public Response<IEnumerable<Dataset>> ListClones()
        {
            DatasetListArgs args = new DatasetListArgs
            {
                DatasetTypes = DatasetTypes.Filesystem | DatasetTypes.Volume
            };
            var datasets = _zfs.Datasets.List(args);
            var clones = datasets.Where(d => d.IsClone);
            return new Response<IEnumerable<Dataset>> { Data = clones };
        }

        [HttpPost("/api/zfs/clones/{dataset}/{snapshot}")]
        public Response CreateClone(string dataset, string snapshot, [FromBody] CloneRequest request)
        {
            var args = new SnapshotCloneArgs { Dataset = dataset, Snapshot = snapshot, TargetDataset = request.Dataset };
            if (request.Properties != null && request.Properties.Length > 0)
            {
                var properties = request.Properties.Select(p => new PropertyValue { Property = p.Name, Value = p.Value }).ToList();
                args.Properties = properties;
            }
            _zfs.Snapshots.Clone(args);
            return new Response();
        }

        /// <summary>
        /// Promotes a clone into an independent dataset.
        /// When you promote a clone, it reverses the relationship between the original dataset and the clone.
        /// So the clone becomes the "master" and the original becomes a dependent dataset
        /// </summary>
        /// <param name="name">The name of the dataset that should be promoted from a clone to its own dataset</param>
        [HttpPut("/api/zfs/clones/{name}")]
        public Response PromoteClone(string name)
        {
            var args = new PromoteArgs { Name = name };
            _zfs.Datasets.Promote(args);
            return new Response();
        }
    }
}
