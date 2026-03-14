using FastEndpoints;
using MediatR;
using Pew.Dashboard.Api.Extensions;
using Pew.Dashboard.Application.Common.Models;
using Pew.Dashboard.Application.Features.News;
using Pew.Dashboard.Application.Features.News.GetNews;

namespace Pew.Dashboard.Api.Endpoints.News;

public sealed class GetNewsEndpoint(ISender sender) : Endpoint<EmptyRequest, ApiResponse<NewsResponse>>
{
    public override void Configure()
    {
        Get("/api/news");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get latest news";
            s.Description = "Returns local and global news items from RSS feeds";
        });
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new GetNewsQuery(), ct);
        await Send.ResponseAsync(result.ToApiResponse(), result.ToHttpStatusCode(), ct);
    }
}
