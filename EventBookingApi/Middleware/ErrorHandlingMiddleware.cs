using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;

namespace EventBookingApi.Middleware
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var error = new
                {
                    message = "An unexpected error occurred.",
                    detail = ex.Message
                };

                var json = JsonSerializer.Serialize(error);
                await context.Response.WriteAsync(json);
            }
        }       
    }
}