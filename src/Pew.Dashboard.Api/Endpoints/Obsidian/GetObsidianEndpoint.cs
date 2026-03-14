using FastEndpoints;
using MediatR;
using Pew.Dashboard.Api.Extensions;
using Pew.Dashboard.Application.Common.Models;
using Pew.Dashboard.Application.Features.Obsidian;
using Pew.Dashboard.Application.Features.Obsidian.GetObsidian;

namespace Pew.Dashboard.Api.Endpoints.Obsidian;

public sealed class GetObsidianEndpoint(ISender sender) : EndpointWithoutRequest<ApiResponse<ObsidianResponse>>
{
    public override void Configure()
    {
        Get("/api/obsidian");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get Obsidian dashboard";
            s.Description = "Returns today's daily note, unchecked tasks, and recently modified notes from the vault.";
        });
        Options(x => x.WithTags("Obsidian"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await sender.Send(new GetObsidianQuery(), ct);
        var response = result.ToApiResponse();
        var statusCode = result.ToHttpStatusCode();
        await Send.ResponseAsync(response, statusCode, ct);
    }
}
