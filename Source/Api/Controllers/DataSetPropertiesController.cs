using System;
using System.Collections.Generic;
using System.Linq;
using Api.Core;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Data;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    public class DataSetPropertiesController : ControllerBase
    {
        private readonly IZfs _zfs;

        public DataSetPropertiesController(IZfsAccessor zfsAccessor)
        {
            _zfs = zfsAccessor.Zfs;
        }

        /// <summary>
        /// Gets all properties from the given dataset
        /// </summary>
        /// <param name="dataset">The dataset to retrieve properties from</param>
        [HttpGet("/api/zfs/datasets/{dataset}/properties")]
        public Response<PropertyData[]> GetProperties(string dataset)
        {
            var props = _zfs.Properties.GetProperties(dataset);
            return new Response<PropertyData[]> { Data = props.Select(PropertyData.FromValue).ToArray() };
        }

        /// <summary>
        /// Gets the single specific property from the given dataset
        /// </summary>
        /// <param name="dataset">The dataset to retrieve the property from</param>
        /// <param name="property">The property to retrieve</param>
        /// <returns></returns>
        [HttpGet("/api/zfs/datasets/{dataset}/properties/{property}")]
        public Response<PropertyData> GetProperty(string dataset, string property)
        {
            var value = _zfs.Properties.GetProperty(dataset, property);
            return new Response<PropertyData> { Data = PropertyData.FromValue(value) };
        }

        /// <summary>
        /// Sets the property for the given dataset
        /// </summary>
        /// <param name="dataset">The dataset where to set the property</param>
        /// <param name="property">The property to set</param>
        /// <param name="value">The value to set</param>
        [HttpPut("/api/zfs/datasets/{dataset}/properties/{property}")]
        public Response<PropertyData> SetProperty(string dataset, string property, [FromBody] string value)
        {
            try
            {
                var newValue = _zfs.Properties.SetProperty(dataset, property, value);
                //Zfs.
                return new Response<PropertyData> { Data = PropertyData.FromValue(newValue) };
            }
            catch (Exception e)
            {
                return new Response<PropertyData> { ErrorText = e.ToString(), Status = ResponseStatus.Failure };
            }
        }

        /// <summary>
        /// Deletes a local value of a property, by resetting it to its default value -
        /// which can either be inherited or just a default value depending on how the pool and dataset has been configured
        /// </summary>
        /// <param name="dataset">The dataset in which to reset the property</param>
        /// <param name="property">The property to reset</param>
        [HttpDelete("/api/zfs/datasets/{dataset}/properties/{property}")]
        public Response<PropertyData> ResetProperty(string dataset, string property)
        {
            try
            {
                _zfs.Properties.ResetPropertyToInherited(dataset, property);

                var value = _zfs.Properties.GetProperty(dataset, property);
                return new Response<PropertyData> { Data = PropertyData.FromValue(value) };
            }
            catch (Exception e)
            {
                return new Response<PropertyData> { ErrorText = e.ToString(), Status = ResponseStatus.Failure };
            }
        }

        /// <summary>
        /// Sets a range of properties in one go on the given dataset.
        /// This is meant for bulk updating of many properties in one call
        /// </summary>
        /// <param name="dataset">The dataset in which to set the properties</param>
        /// <param name="properties">The properties to set - this can be 1-n</param>
        /// <returns></returns>
        [HttpPost("/api/zfs/datasets/{dataset}/properties/")]
        public Response<PropertyValue[]> SetProperties(string dataset, [FromBody] PropertyData[] properties)
        {
            try
            {
                if (properties?.Length == 0)
                {
                    Response.StatusCode = 400;
                    return new Response<PropertyValue[]> { Status = ResponseStatus.Failure, ErrorText = "Please provide at least one property to set" };
                }

                List<PropertyValue> responses = new List<PropertyValue>();
                foreach (var property in properties)
                {
                    var newValue = _zfs.Properties.SetProperty(dataset, property.Name, property.Value);
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
