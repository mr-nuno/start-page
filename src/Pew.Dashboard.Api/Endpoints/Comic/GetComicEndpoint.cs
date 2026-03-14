using FastEndpoints;
using MediatR;
using Pew.Dashboard.Api.Extensions;
using Pew.Dashboard.Application.Common.Models;
using Pew.Dashboard.Application.Features.Comic;
using Pew.Dashboard.Application.Features.Comic.GetComic;

namespace Pew.Dashboard.Api.Endpoints.Comic;

public sealed class GetComicEndpoint(ISender sender) : EndpointWithoutRequest<ApiResponse<ComicResponse>>
{
    public override void Configure()
    {
        Get("/api/comic");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get daily comic";
            s.Description = "Returns today's Calvin and Hobbes comic strip from GoComics.";
        });
        Options(x => x.WithTags("Comic"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await sender.Send(new GetComicQuery(), ct);
        var response = result.ToApiResponse();
        var statusCode = result.ToHttpStatusCode();
        await Send.ResponseAsync(response, statusCode, ct);
    }
}
