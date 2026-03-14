using System.Reflection;
using System.Text.Json;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pew.Dashboard.Api;
using Pew.Dashboard.Api.Middleware;
using Pew.Dashboard.Application.Common.Models;
using Pew.Dashboard.Core;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services
    .AddCoreServices(builder.Configuration)
    .AddApiServices();

var app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, _, _) =>
        httpContext.Request.Path.StartsWithSegments("/health")
            ? Serilog.Events.LogEventLevel.Verbose
            : Serilog.Events.LogEventLevel.Information;
});
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseFastEndpoints(c =>
{
    c.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
    {
        return new ApiResponse<object>
        {
            Success = false,
            ValidationErrors = failures
                .Select(f => new ValidationError(f.PropertyName, f.ErrorMessage))
                .ToList()
        };
    };
    c.Errors.StatusCode = 400;
});

var healthOptions = new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        string version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3) ?? "0.0.0";
        var response = new { status = report.Status.ToString(), version };
        await JsonSerializer.SerializeAsync(context.Response.Body, response, cancellationToken: context.RequestAborted);
    }
};

app.MapHealthChecks("/health/live", healthOptions).AllowAnonymous();
app.MapHealthChecks("/health/ready", healthOptions).AllowAnonymous();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
    app.MapScalarApiReference(options =>
    {
        options.OpenApiRoutePattern = "/swagger/{documentName}/swagger.json";
    });
}

app.MapFallbackToFile("index.html");

app.Run();

public partial class Program;
