using FastEndpoints;
using MediatR;
using Pew.Dashboard.Api.Extensions;
using Pew.Dashboard.Application.Common.Models;
using Pew.Dashboard.Application.Features.YouTube;
using Pew.Dashboard.Application.Features.YouTube.GetYouTube;

namespace Pew.Dashboard.Api.Endpoints.YouTube;

public sealed class GetYouTubeEndpoint(ISender sender) : Endpoint<EmptyRequest, ApiResponse<YouTubeResponse>>
{
    public override void Configure()
    {
        Get("/api/youtube");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get latest YouTube video";
            s.Description = "Returns the most recent video from the configured YouTube channel";
        });
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new GetYouTubeQuery(), ct);
        await Send.ResponseAsync(result.ToApiResponse(), result.ToHttpStatusCode(), ct);
    }
}
