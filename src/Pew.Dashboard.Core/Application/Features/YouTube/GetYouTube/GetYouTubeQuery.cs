using Ardalis.Result;
using MediatR;

namespace Pew.Dashboard.Application.Features.YouTube.GetYouTube;

public sealed record GetYouTubeQuery : IRequest<Result<YouTubeResponse>>
{
    public sealed class Handler(IYouTubeService youTubeService) : IRequestHandler<GetYouTubeQuery, Result<YouTubeResponse>>
    {
        public async Task<Result<YouTubeResponse>> Handle(GetYouTubeQuery request, CancellationToken ct)
        {
            var response = await youTubeService.GetVideoAsync(ct);
            return Result.Success(response);
        }
    }
}
