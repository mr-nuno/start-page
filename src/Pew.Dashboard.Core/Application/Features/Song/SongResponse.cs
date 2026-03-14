namespace Pew.Dashboard.Application.Features.Song;

public sealed record SongResponse(
    string Title,
    string Artist,
    string[] Chords,
    string Pattern,
    string? Lyrics);
