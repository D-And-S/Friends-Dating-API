using Friends_Date_API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;


using Microsoft.AspNetCore.Hosting;

namespace Friends_Date_API.Middleware
{
    // we are creating custom middleware for error handling
    public class ExcepitonMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExcepitonMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        //Request Delegate used for to mentionwhat's coming up next in the middleware pipeline
        //we want to show the exception log in the terminal what's 
        //IHostEnvrionment to use hosting lavel production, staging or development
        public ExcepitonMiddleware(RequestDelegate next,
                                   ILogger<ExcepitonMiddleware> logger,
                                   IWebHostEnvironment env
                                   )
        {
            
            _next = next;
            _logger = logger;
            _env = env;
        }

        // What to do if there is any exception
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                

                var response = _env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, "Internal Server Error");

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
