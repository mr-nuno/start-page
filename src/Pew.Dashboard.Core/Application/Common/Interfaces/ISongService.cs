using Pew.Dashboard.Application.Features.Song;

namespace Pew.Dashboard.Application.Common.Interfaces;

public interface ISongService
{
    Task<SongResponse> GetSongAsync(CancellationToken ct);
}
