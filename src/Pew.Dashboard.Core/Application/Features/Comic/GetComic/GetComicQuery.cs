using Pew.Dashboard.Application.Common.Interfaces;
using Ardalis.Result;
using MediatR;

namespace Pew.Dashboard.Application.Features.Comic.GetComic;

public sealed record GetComicQuery : IRequest<Result<ComicResponse>>
{
    public sealed class Handler(IComicService comicService) : IRequestHandler<GetComicQuery, Result<ComicResponse>>
    {
        public async Task<Result<ComicResponse>> Handle(GetComicQuery request, CancellationToken ct)
        {
            var response = await comicService.GetComicAsync(ct);
            return Result<ComicResponse>.Success(response);
        }
    }
}
