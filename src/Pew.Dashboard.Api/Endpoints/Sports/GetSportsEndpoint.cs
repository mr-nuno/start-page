using FastEndpoints;
using MediatR;
using Pew.Dashboard.Api.Extensions;
using Pew.Dashboard.Application.Common.Models;
using Pew.Dashboard.Application.Features.Sports;
using Pew.Dashboard.Application.Features.Sports.GetSports;

namespace Pew.Dashboard.Api.Endpoints.Sports;

public sealed class GetSportsEndpoint(ISender sender) : EndpointWithoutRequest<ApiResponse<SportsResponse>>
{
    public override void Configure()
    {
        Get("/api/sports");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get sports data";
            s.Description = "Returns Lule\u00e5 HF standings, recent games, and next game from EliteProspects.";
        });
        Options(x => x.WithTags("Sports"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await sender.Send(new GetSportsQuery(), ct);
        var response = result.ToApiResponse();
        var statusCode = result.ToHttpStatusCode();
        await Send.ResponseAsync(response, statusCode, ct);
    }
}
