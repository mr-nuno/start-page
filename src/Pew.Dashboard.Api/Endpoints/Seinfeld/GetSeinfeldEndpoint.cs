using FastEndpoints;
using MediatR;
using Pew.Dashboard.Api.Extensions;
using Pew.Dashboard.Application.Common.Models;
using Pew.Dashboard.Application.Features.Seinfeld;
using Pew.Dashboard.Application.Features.Seinfeld.GetSeinfeld;

namespace Pew.Dashboard.Api.Endpoints.Seinfeld;

public sealed class GetSeinfeldEndpoint(ISender sender) : EndpointWithoutRequest<ApiResponse<SeinfeldResponse>>
{
    public override void Configure()
    {
        Get("/api/seinfeld");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get Seinfeld quote of the day";
            s.Description = "Returns a daily rotating Seinfeld quote with character and episode information.";
        });
        Options(x => x.WithTags("Seinfeld"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await sender.Send(new GetSeinfeldQuery(), ct);
        var response = result.ToApiResponse();
        var statusCode = result.ToHttpStatusCode();
        await Send.ResponseAsync(response, statusCode, ct);
    }
}
