using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Pew.Dashboard.Application.Common.Interfaces;
using Serilog;

using Pew.Dashboard.Application.Features.Song;

namespace Pew.Dashboard.Infrastructure.Services.Song;

public sealed class SongService(
    HttpClient httpClient,
    IMemoryCache cache,
    IDateTimeProvider dateTimeProvider) : ISongService
{
    private static readonly ILogger Log = Serilog.Log.ForContext<SongService>();
    private const string CacheKey = "song";

    public async Task<SongResponse> GetSongAsync(CancellationToken ct)
    {
        if (cache.TryGetValue(CacheKey, out SongResponse? cached) && cached is not null)
        {
            return cached;
        }

        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow.DateTime);
        var index = today.DayNumber % SongLibrary.Songs.Length;
        var song = SongLibrary.Songs[index];

        var lyrics = await FetchLyricsAsync(song.Artist, song.Title, ct);

        var response = new SongResponse(
            Title: song.Title,
            Artist: song.Artist,
            Chords: song.Chords,
            Pattern: song.Pattern,
            Lyrics: lyrics);

        cache.Set(CacheKey, response, TimeSpan.FromHours(24));
        Log.Information("Song of the day refreshed: {Title} by {Artist}", song.Title, song.Artist);

        return response;
    }

    private async Task<string?> FetchLyricsAsync(string artist, string title, CancellationToken ct)
    {
        try
        {
            var encodedArtist = Uri.EscapeDataString(artist);
            var encodedTitle = Uri.EscapeDataString(title);
            var url = $"https://api.lyrics.ovh/v1/{encodedArtist}/{encodedTitle}";

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            var json = await httpClient.GetStringAsync(url, cts.Token);
            var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("lyrics", out var lyricsElement))
            {
                return lyricsElement.GetString();
            }

            return null;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to fetch lyrics for {Title} by {Artist}", title, artist);
            return null;
        }
    }
}
