using System.Reflection;
using FastEndpoints;
using Pew.Dashboard.Application.Common.Models;

namespace Pew.Dashboard.Api.Endpoints.Version;

public sealed class GetVersionEndpoint : EndpointWithoutRequest<ApiResponse<VersionResponse>>
{
    public override void Configure()
    {
        Get("/api/version");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get API version";
            s.Description = "Returns the current version of the API.";
        });
        Options(x => x.WithTags("Version"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknown";

        var response = new ApiResponse<VersionResponse>
        {
            Success = true,
            Data = new VersionResponse(version)
        };

        await Send.OkAsync(response, ct);
    }
}

public sealed record VersionResponse(string Version);
