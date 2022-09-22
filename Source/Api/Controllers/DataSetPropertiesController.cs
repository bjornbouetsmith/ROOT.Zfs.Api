using System;
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
    [ApiController]
    public class DataSetPropertiesController : ControllerBase
    {
        private readonly IRemoteConnection _remote;

        public DataSetPropertiesController(IRemoteConnection remote)
        {
            _remote = remote;
        }

        [HttpGet("/api/zfs/datasets/{dataset}/properties")]
        public Response<PropertyData[]> GetProperties(string dataset)
        {
            var props = Zfs.Properties.GetProperties(dataset, _remote.RemoteProcessCall);
            return new Response<PropertyData[]> { Data = props.Select(PropertyData.FromValue).ToArray() };
        }

        [HttpGet("/api/zfs/datasets/{dataset}/properties/{property}")]
        public Response<PropertyData> GetProperty(string dataset, string property)
        {
            var value = Zfs.Properties.GetProperty(dataset, property, _remote.RemoteProcessCall);
            return new Response<PropertyData> { Data = PropertyData.FromValue(value) };
        }

        [HttpPut("/api/zfs/datasets/{dataset}/properties/{property}")]
        public Response<PropertyData> SetProperty(string dataset, string property, [FromBody] string value)
        {
            try
            {
                var newValue = Zfs.Properties.SetProperty(dataset, property, value, _remote.RemoteProcessCall);
                //Zfs.
                return new Response<PropertyData> { Data = PropertyData.FromValue(newValue) };
            }
            catch (Exception e)
            {
                return new Response<PropertyData> { ErrorText = e.ToString(), Status = ResponseStatus.Failure };
            }
        }

        [HttpPost("/api/zfs/datasets/{dataset}/properties/")]
        public Response<PropertyValue[]> SetProperties(string dataset, [FromBody] PropertyData[] properties)
        {
            try
            {
                List<PropertyValue> responses = new List<PropertyValue>();
                foreach (var property in properties)
                {
                    var newValue = Zfs.Properties.SetProperty(dataset, property.Name, property.Value, _remote.RemoteProcessCall);
                    responses.Add(newValue);
                }

                return new Response<PropertyValue[]> { Data = responses.ToArray() };
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return new Response<PropertyValue[]> { ErrorText = e.ToString(), Status = ResponseStatus.Failure };
            }
        }
    }
}
