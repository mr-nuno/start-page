namespace Pew.Dashboard.Application.Features.Song;

public interface ISongService
{
    Task<SongResponse> GetSongAsync(CancellationToken ct);
}
