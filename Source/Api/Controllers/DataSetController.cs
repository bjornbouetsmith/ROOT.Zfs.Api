using System;
using System.Collections.Generic;
using System.Linq;
using Api.Core;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Datasets;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/zfs/datasets")]
    [ApiController]
    public class DataSetController : ApiControllerBase
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
        public Response<IEnumerable<Dataset>> GetDataSets()
        {
            var dataSets = _zfs.Datasets.List(new DatasetListArgs());
            return new Response<IEnumerable<Dataset>> { Data = dataSets };
        }

        /// <summary>
        /// Deletes the given dataset
        /// </summary>
        /// <param name="name">The dataset to delete</param>
        /// <param name="flags">How to delete the dataset</param>
        [HttpDelete("/api/zfs/datasets/{name}")]
        public Response<string[]> DeleteDataSet(string name, [FromQuery] DatasetDestroyFlags flags)
        {
            var args = new DatasetDestroyArgs { Dataset = name, DestroyFlags = flags };
            if (!args.Validate(out var errors))
            {
                return ToErrorResponse<string[]>(errors);
            }

            var response = _zfs.Datasets.Destroy(args);

            var data = flags.HasFlag(DatasetDestroyFlags.DryRun) ? response.DryRun : null;

            return new Response<string[]> { Data = data?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries) };
        }

        /// <summary>
        /// Creates a dataset with the given properties if any
        /// </summary>
        /// <param name="name">The name of the dataset.
        /// This must include the name of the pool in the format
        /// [pool]/[preceedingdataset]/[dataset]</param>
        /// <param name="properties">Any properties that should be set when creating the dataset. This is not required and an empty array can be sent</param>
        [HttpPost("/api/zfs/datasets/{name}")]
        public Response<Dataset> CreateDataSet(string name, [FromBody] PropertyData[] properties)
        {
            var propertyValues = properties.Select(p => new PropertyValue { Property = p.Name, Source = p.Source, Value = p.Value }).ToArray();

            var args = new DatasetCreationArgs { DatasetName = name, Properties = propertyValues, Type = DatasetTypes.Filesystem };

            if (!args.Validate(out var errors))
            {
                return ToErrorResponse<Dataset>(errors);
            }

            try
            {
                var dataset = _zfs.Datasets.Create(args);

                return new Response<Dataset> { Data = dataset };
            }
            catch (ProcessCallException e)
            {
                Response.StatusCode = 500;
                return new Response<Dataset> { Status = ResponseStatus.Failure, ErrorText = e.ToString() };
            }

        }

        /// <summary>
        /// Gets information about the given dataset
        /// </summary>
        /// <param name="name">The name of the dataset</param>
        [HttpGet("/api/zfs/datasets/{name}")]
        public Response<Dataset> GetDataSet(string name)
        {
            var args = new DatasetListArgs { Root = name };
            if (!args.Validate(out var errors))
            {
                return ToErrorResponse<Dataset>(errors);
            }

            var dataset = _zfs.Datasets.List(args);

            if (dataset == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return new Response<Dataset> { Data = dataset.FirstOrDefault() };
        }
    }
}
