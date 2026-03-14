using FastEndpoints;
using MediatR;
using Pew.Dashboard.Api.Extensions;
using Pew.Dashboard.Application.Common.Models;
using Pew.Dashboard.Application.Features.SystemInfo;
using Pew.Dashboard.Application.Features.SystemInfo.GetSystem;

namespace Pew.Dashboard.Api.Endpoints.System;

public sealed class GetSystemEndpoint(ISender sender) : EndpointWithoutRequest<ApiResponse<SystemResponse>>
{
    public override void Configure()
    {
        Get("/api/system");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get system metrics";
            s.Description = "Returns current memory, disk, and CPU usage from the host system.";
        });
        Options(x => x.WithTags("System"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await sender.Send(new GetSystemQuery(), ct);
        var response = result.ToApiResponse();
        var statusCode = result.ToHttpStatusCode();
        await Send.ResponseAsync(response, statusCode, ct);
    }
}
