using Ardalis.Result;
using MediatR;

namespace Pew.Dashboard.Application.Features.News.GetNews;

public sealed record GetNewsQuery : IRequest<Result<NewsResponse>>
{
    public sealed class Handler(INewsService newsService) : IRequestHandler<GetNewsQuery, Result<NewsResponse>>
    {
        public async Task<Result<NewsResponse>> Handle(GetNewsQuery request, CancellationToken ct)
        {
            var response = await newsService.GetNewsAsync(ct);
            return Result.Success(response);
        }
    }
}
