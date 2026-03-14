using FastEndpoints;
using MediatR;
using Pew.Dashboard.Api.Extensions;
using Pew.Dashboard.Application.Common.Models;
using Pew.Dashboard.Application.Features.Song;
using Pew.Dashboard.Application.Features.Song.GetSong;

namespace Pew.Dashboard.Api.Endpoints.Song;

public sealed class GetSongEndpoint(ISender sender) : Endpoint<EmptyRequest, ApiResponse<SongResponse>>
{
    public override void Configure()
    {
        Get("/api/song");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get song of the day";
            s.Description = "Returns the daily song with chords, strumming pattern, and optional lyrics";
        });
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new GetSongQuery(), ct);
        await Send.ResponseAsync(result.ToApiResponse(), result.ToHttpStatusCode(), ct);
    }
}
