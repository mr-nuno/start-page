using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace Pew.Dashboard.Application.Features.YouTube;

public sealed class YouTubeService(
    HttpClient httpClient,
    IMemoryCache cache) : IYouTubeService
{
    private static readonly ILogger Log = Serilog.Log.ForContext<YouTubeService>();
    private const string CacheKey = "youtube";
    private const string FeedUrl = "https://www.youtube.com/feeds/videos.xml?channel_id=UCwWhs_6x42TyRM4Wstoq8HA";

    public async Task<YouTubeResponse> GetVideoAsync(CancellationToken ct)
    {
        if (cache.TryGetValue(CacheKey, out YouTubeResponse? cached) && cached is not null)
            return cached;

        try
        {
            var xml = await httpClient.GetStringAsync(FeedUrl, ct);
            using var reader = XmlReader.Create(new StringReader(xml));
            var feed = SyndicationFeed.Load(reader);

            var latest = feed.Items.FirstOrDefault();
            if (latest is null)
                return new YouTubeResponse(string.Empty, string.Empty, string.Empty, string.Empty, "No videos found");

            var videoId = ExtractVideoId(latest);
            var link = latest.Links.FirstOrDefault()?.Uri.ToString() ?? string.Empty;

            var response = new YouTubeResponse(
                Title: latest.Title?.Text ?? string.Empty,
                VideoId: videoId,
                Published: latest.PublishDate.ToString("o"),
                Link: link,
                Error: null);

            cache.Set(CacheKey, response, TimeSpan.FromHours(1));
            Log.Information("YouTube data refreshed with video {VideoId}", videoId);

            return response;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to fetch YouTube feed");

            if (cache.TryGetValue(CacheKey, out YouTubeResponse? fallback) && fallback is not null)
                return fallback;

            return new YouTubeResponse(string.Empty, string.Empty, string.Empty, string.Empty, "Failed to fetch YouTube data");
        }
    }

    private static string ExtractVideoId(SyndicationItem item)
    {
        var ytExtension = item.ElementExtensions
            .FirstOrDefault(e => e.OuterName == "videoId" && e.OuterNamespace == "http://www.youtube.com/xml/schemas/2015");

        if (ytExtension is not null)
        {
            using var extReader = ytExtension.GetReader();
            extReader.Read();
            return extReader.ReadContentAsString();
        }

        // Fallback: parse from link URL
        var link = item.Links.FirstOrDefault()?.Uri.ToString() ?? string.Empty;
        var queryIndex = link.IndexOf("v=", StringComparison.Ordinal);
        if (queryIndex >= 0)
            return link[(queryIndex + 2)..].Split('&')[0];

        return string.Empty;
    }
}
