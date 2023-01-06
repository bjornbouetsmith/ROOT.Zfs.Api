using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Api.Controllers
{
    /// <summary>
    /// Base class for all controllers
    /// </summary>
    [Authorize]
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {

        /// <summary>
        /// Converts a list of errors to a proper errors response and ends the request
        /// </summary>
        protected Response<T> ToErrorResponse<T>(IList<string> errors)
        {
            Response.StatusCode = 400;
            return new Response<T> { ErrorText = string.Join(Environment.NewLine, errors), Status = ResponseStatus.InputError };
        }

        /// <summary>
        /// Converts a list of errors to a proper errors response and ends the request
        /// </summary>
        protected Response ToErrorResponse(IList<string> errors)
        {
            Response.StatusCode = 400;
            return new Response { ErrorText = string.Join(Environment.NewLine, errors), Status = ResponseStatus.InputError };
        }
    }
}
