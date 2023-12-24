using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace AirportAutomationServices.Middlewares
{
	public class GlobalExceptionHandler
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionHandler> _logger;

		public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext httpContext)
		{
			try
			{
				await _next(httpContext);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(httpContext, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";
			var response = context.Response;
			ErrorResponseModel model = new();

			switch (exception)
			{
				case ApplicationException:
					model.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.StatusCode = (int)HttpStatusCode.BadRequest;
					model.ResponseMessage = "Bad Request";
					break;
				case ValidationException:
					model.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.StatusCode = (int)HttpStatusCode.BadRequest;
					model.ResponseMessage = "Validation failed.";
					break;
				case ArgumentException argumentException:
					model.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.StatusCode = (int)HttpStatusCode.BadRequest;
					model.ResponseMessage = "Bad Request. " + argumentException.Message;
					break;
				case DbUpdateException dbUpdateException when dbUpdateException.InnerException is SqlException sqlException:
					if (sqlException.Number == 2601 || sqlException.Number == 2627)
					{
						model.ResponseCode = (int)HttpStatusCode.Conflict;
						response.StatusCode = (int)HttpStatusCode.Conflict;
						model.ResponseMessage = "Conflict: Duplicate key violation.";
					}
					else if (sqlException.Number == 547)
					{
						model.ResponseCode = (int)HttpStatusCode.Conflict;
						response.StatusCode = (int)HttpStatusCode.Conflict;
						model.ResponseMessage = "Conflict: This entity is referenced in other records and cannot be deleted.";
					}
					else
					{
						model.ResponseCode = (int)HttpStatusCode.InternalServerError;
						response.StatusCode = (int)HttpStatusCode.InternalServerError;
						model.ResponseMessage = "Database error: " + sqlException.Message;
					}
					break;
				case InvalidOperationException invalidOperationException:
					model.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.StatusCode = (int)HttpStatusCode.BadRequest;
					model.ResponseMessage = "Bad Request. " + invalidOperationException.Message;
					break;
				case UnauthorizedAccessException:
					model.ResponseCode = (int)HttpStatusCode.Unauthorized;
					response.StatusCode = (int)HttpStatusCode.Unauthorized;
					model.ResponseMessage = "Unauthorized access..";
					break;
				default:
					model.ResponseCode = (int)HttpStatusCode.InternalServerError;
					response.StatusCode = (int)HttpStatusCode.InternalServerError;
					model.ResponseMessage = "Internal Server Error";
					break;
			}
			_logger.LogError("Response Code: {responseCode}, Message: {responseMessage} \n Stack Trace: {stackTrace}",
				model.ResponseCode, model.ResponseMessage, exception);
			var result = JsonSerializer.Serialize(model);
			await context.Response.WriteAsync(result);
		}
	}
}