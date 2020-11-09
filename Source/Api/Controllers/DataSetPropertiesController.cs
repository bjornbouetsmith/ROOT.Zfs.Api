﻿using System;
using System.Collections.Generic;
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
        public Response<IEnumerable<PropertyValue>> GetProperties(string dataset)
        {
            var props = Zfs.Properties.GetProperties(dataset,_remote.RemoteProcessCall);
            return new Response<IEnumerable<PropertyValue>> { Data = props };
        }

        [HttpGet("/api/zfs/datasets/{dataset}/properties/{property}")]
        public Response<PropertyValue> GetProperty(string dataset, string property)
        {
            var value = Zfs.Properties.GetProperty(dataset, property, _remote.RemoteProcessCall);
            return new Response<PropertyValue> { Data = value };
        }

        [HttpPut("/api/zfs/datasets/{dataset}/properties/{property}")]
        public Response<PropertyValue> SetProperty(string dataset, string property, [FromBody] string value)
        {
            try
            {
                var newValue = Zfs.Properties.SetProperty(dataset, property, value, _remote.RemoteProcessCall);
                //Zfs.
                return new Response<PropertyValue> { Data = newValue };
            }
            catch (Exception e)
            {
                return new Response<PropertyValue> { ErrorText = e.ToString(), Status = ResponseStatus.Failure };
            }
        }
    }
}