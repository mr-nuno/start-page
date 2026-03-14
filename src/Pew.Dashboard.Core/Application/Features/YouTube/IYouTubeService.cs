namespace Pew.Dashboard.Application.Features.YouTube;

public interface IYouTubeService
{
    Task<YouTubeResponse> GetVideoAsync(CancellationToken ct);
}
