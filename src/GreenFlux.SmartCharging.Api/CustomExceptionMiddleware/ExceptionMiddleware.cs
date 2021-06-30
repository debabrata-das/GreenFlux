using System;
using System.Net;
using System.Threading.Tasks;
using GreenFlux.SmartCharging.Api.CustomExceptionMiddleware.ProblemDetails;
using GreenFlux.SmartCharging.Api.Exceptions;
using GreenFlux.SmartCharging.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace GreenFlux.SmartCharging.Api.CustomExceptionMiddleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;
        public ExceptionMiddleware(RequestDelegate next, ILoggerManager logger)
        {
            _logger = logger;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.Error($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            if (exception is GreenFluxBaseException)
            {
                var greenFluxDomainProblemDetail= new GreenFluxProblemDetail(exception.Message, exception.InnerException?.Message);
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var json = JsonConvert.SerializeObject(greenFluxDomainProblemDetail);
                await httpContext.Response.WriteAsync(json);
            }
            else
            {
                Microsoft.AspNetCore.Mvc.ProblemDetails errorDetail = new Microsoft.AspNetCore.Mvc.ProblemDetails();
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorDetail.Title = "Internal server Error from the custom middleware";
                errorDetail.Status = 500;
                var json = JsonConvert.SerializeObject(errorDetail);
                await httpContext.Response.WriteAsync(json);
            }
        }
    }
}
