using System.Net;
using System.Text.Json;
using Pew.Dashboard.Application.Common.Models;
using ILogger = Serilog.ILogger;

namespace Pew.Dashboard.Api.Middleware;

public sealed class GlobalExceptionHandlerMiddleware(RequestDelegate next)
{
    private static readonly ILogger Log = Serilog.Log.ForContext<GlobalExceptionHandlerMiddleware>();

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An unhandled exception occurred");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>
            {
                Success = false,
                Error = "An unexpected error occurred"
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
