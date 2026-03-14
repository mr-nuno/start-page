using FastEndpoints;
using MediatR;
using Pew.Dashboard.Api.Extensions;
using Pew.Dashboard.Application.Common.Models;
using Pew.Dashboard.Application.Features.Quote;
using Pew.Dashboard.Application.Features.Quote.GetQuote;

namespace Pew.Dashboard.Api.Endpoints.Quote;

public sealed class GetQuoteEndpoint(ISender sender) : EndpointWithoutRequest<ApiResponse<QuoteResponse>>
{
    public override void Configure()
    {
        Get("/api/quote");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get quote of the day";
            s.Description = "Returns a daily rotating quote from Lord of the Rings, Star Wars, or philosophy.";
        });
        Options(x => x.WithTags("Quote"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await sender.Send(new GetQuoteQuery(), ct);
        var response = result.ToApiResponse();
        var statusCode = result.ToHttpStatusCode();
        await Send.ResponseAsync(response, statusCode, ct);
    }
}
