using Pew.Dashboard.Application.Features.YouTube;

namespace Pew.Dashboard.Application.Common.Interfaces;

public interface IYouTubeService
{
    Task<YouTubeResponse> GetVideoAsync(CancellationToken ct);
}
