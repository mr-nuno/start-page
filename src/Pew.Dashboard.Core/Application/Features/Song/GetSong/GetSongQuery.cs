using Pew.Dashboard.Application.Common.Interfaces;
using Ardalis.Result;
using MediatR;

namespace Pew.Dashboard.Application.Features.Song.GetSong;

public sealed record GetSongQuery : IRequest<Result<SongResponse>>
{
    public sealed class Handler(ISongService songService) : IRequestHandler<GetSongQuery, Result<SongResponse>>
    {
        public async Task<Result<SongResponse>> Handle(GetSongQuery request, CancellationToken ct)
        {
            var response = await songService.GetSongAsync(ct);
            return Result.Success(response);
        }
    }
}
