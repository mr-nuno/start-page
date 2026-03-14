using FastEndpoints;
using MediatR;
using Pew.Dashboard.Api.Extensions;
using Pew.Dashboard.Application.Common.Models;
using Pew.Dashboard.Application.Features.Guitar;
using Pew.Dashboard.Application.Features.Guitar.GetGuitar;

namespace Pew.Dashboard.Api.Endpoints.Guitar;

public sealed class GetGuitarEndpoint(ISender sender) : EndpointWithoutRequest<ApiResponse<GuitarResponse>>
{
    public override void Configure()
    {
        Get("/api/guitar");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get chord of the day";
            s.Description = "Returns a daily rotating guitar chord with SVG diagram, fingering, and practice tip.";
        });
        Options(x => x.WithTags("Guitar"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await sender.Send(new GetGuitarQuery(), ct);
        var response = result.ToApiResponse();
        var statusCode = result.ToHttpStatusCode();
        await Send.ResponseAsync(response, statusCode, ct);
    }
}
