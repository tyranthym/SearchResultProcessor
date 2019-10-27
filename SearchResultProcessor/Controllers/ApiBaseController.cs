using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SearchResultProcessor.Models.Constants;
using SearchResultProcessor.Models.Responses;
using Serilog;

namespace SearchResultProcessor.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        private string modelName = "Model";
        protected string ModelName
        {
            get
            {
                return modelName;
            }
            set
            {
                modelName = value;
                errorMessageIdNotProvided = $"Id not provided for {modelName}";
                errorMessageIdNotMatched = $"Id in query path and in body are not matched for {modelName}";
                errorMessageModelNotFound = $"{modelName} not found";
            }
        }
        private string errorMessageIdNotProvided = "Id not provided";
        private string errorMessageIdNotMatched = "Id in query path and in body are not matched";
        private string errorMessageModelNotFound = "Model not found";
        private readonly string errorMessageErrorResponse = "Request was received successfully but an error occurred while processing";

        /// <summary>
        /// Log 400BadRequest, 
        /// Creates an Microsoft.AspNetCore.Mvc.BadRequestObjectResult that produces a Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest response.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.BadRequestObjectResult for the response.</returns>
        protected BadRequestObjectResult IdNotProvidedBadRequest(ILogger logger)
        {
            logger.Warning($"{errorMessageIdNotProvided}.");
            ErrorResponse errorResponse = new ErrorResponse(errorMessageIdNotProvided, ErrorType.IdNotProvided);
            return BadRequest(errorResponse);
        }

        /// <summary>
        /// Log 400BadRequest,
        /// Creates an Microsoft.AspNetCore.Mvc.BadRequestObjectResult that produces a Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest response.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="idInPath">id provided by query string</param>
        /// <param name="idInBody">id provided by body</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.BadRequestObjectResult for the response.</returns>
        protected BadRequestObjectResult IdNotMatchedBadRequest(ILogger logger, int? idInPath, int idInBody)
        {
            logger.Warning($"{errorMessageIdNotMatched}. Id in path: {idInPath}, id in body: {idInBody}.");
            string errorMessage = $"{errorMessageIdNotMatched}. Id in path: {idInPath}, id in body: {idInBody}";
            ErrorResponse errorResponse = new ErrorResponse(errorMessage, ErrorType.IdNotMatched);
            return BadRequest(errorResponse);
        }
        /// <summary>
        /// Log 400BadRequest,
        /// Creates an Microsoft.AspNetCore.Mvc.BadRequestObjectResult that produces a Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest response.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="idInPath">id provided by query string</param>
        /// <param name="idInBody">id provided by body</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.BadRequestObjectResult for the response.</returns>
        protected BadRequestObjectResult IdNotMatchedBadRequest(ILogger logger, string idInPath, string idInBody)
        {
            logger.Warning($"{errorMessageIdNotMatched}. Id in path: {idInPath}, id in body: {idInBody}.");
            string errorMessage = $"{errorMessageIdNotMatched}. Id in path: {idInPath}, id in body: {idInBody}";
            ErrorResponse errorResponse = new ErrorResponse(errorMessage, ErrorType.IdNotMatched);
            return BadRequest(errorResponse);
        }


        /// <summary>
        /// Log 404NotFound response, 
        /// Creates an Microsoft.AspNetCore.Mvc.NotFoundObjectResult that produces a Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound response.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="id">model primary key</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.NotFoundObjectResult for the response.</returns>
        protected NotFoundObjectResult ModelNotFound(ILogger logger, int? id = null)
        {
            if (id == null)
            {
                logger.Warning($"{errorMessageModelNotFound}.");
            }
            else
            {
                logger.Warning($"{errorMessageModelNotFound}. Id passed: {id.ToString()}");
            }
            return NotFound(errorMessageModelNotFound);
        }
        /// <summary>
        /// Log 404NotFound response, 
        /// Creates an Microsoft.AspNetCore.Mvc.NotFoundObjectResult that produces a Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound response.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="id">model primary key</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.NotFoundObjectResult for the response.</returns>
        protected NotFoundObjectResult ModelNotFound(ILogger logger, string id = null)
        {
            if (id == null)
            {
                logger.Warning($"{errorMessageModelNotFound}.");
            }
            else
            {
                logger.Warning($"{errorMessageModelNotFound}. Id passed: {id}");
            }
            return NotFound(errorMessageModelNotFound);
        }


        /// <summary>
        /// Log 400BadRequest,
        /// Creates an Microsoft.AspNetCore.Mvc.BadRequestObjectResult that produces a Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest response.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="errorMessage"></param>
        /// <param name="errorType">application error type</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.BadRequestObjectResult for the response.</returns>
        protected BadRequestObjectResult ErrorResponseBadRequest(ILogger logger, string errorMessage, string errorType = ErrorType.BusinessRuleViolation)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                logger.Warning($"{errorMessageErrorResponse}.");
                ErrorResponse errorResponse = new ErrorResponse(errorMessageErrorResponse, errorType);
                return BadRequest(errorResponse);
            }
            else
            {
                logger.Warning($"{errorMessageErrorResponse}. Error: {errorMessage}.");
                ErrorResponse errorResponse = new ErrorResponse(errorMessage, errorType, errorMessageErrorResponse);
                return BadRequest(errorResponse);
            }
        }

    }
}