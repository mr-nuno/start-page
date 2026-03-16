using Pew.Dashboard.Application.Common.Interfaces;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Pew.Dashboard.Core;
using Serilog;

using Pew.Dashboard.Application.Features.News;

namespace Pew.Dashboard.Infrastructure.Services.News;

public sealed partial class NewsService(
    HttpClient httpClient,
    IMemoryCache cache,
    IOptions<NewsOptions> options) : INewsService
{
    private static readonly ILogger Log = Serilog.Log.ForContext<NewsService>();
    private const string CacheKey = "news";

    public async Task<NewsResponse> GetNewsAsync(CancellationToken ct)
    {
        if (cache.TryGetValue(CacheKey, out NewsResponse? cached) && cached is not null)
        {
            return cached;
        }

        var opts = options.Value;

        var localTask = FetchFeedAsync(opts.LocalFeedUrl, opts.MaxItems, ct);
        var globalTask = FetchFeedAsync(opts.GlobalFeedUrl, opts.MaxItems, ct);

        await Task.WhenAll(localTask, globalTask);

        var response = new NewsResponse(
            Local: await localTask,
            Global: await globalTask);

        cache.Set(CacheKey, response, TimeSpan.FromMinutes(opts.CacheMinutes));
        Log.Information("News data refreshed with {LocalCount} local and {GlobalCount} global items",
            response.Local.Count, response.Global.Count);

        return response;
    }

    private async Task<List<NewsItem>> FetchFeedAsync(string feedUrl, int maxItems, CancellationToken ct)
    {
        try
        {
            var html = await httpClient.GetStringAsync(feedUrl, ct);
            using var reader = XmlReader.Create(new StringReader(html));
            var feed = SyndicationFeed.Load(reader);

            return feed.Items
                .Take(maxItems)
                .Select(item =>
                {
                    string? thumbnail = null;
                    var mediaExtension = item.ElementExtensions
                        .FirstOrDefault(e => e.OuterName == "thumbnail" && e.OuterNamespace == "http://search.yahoo.com/mrss/");

                    if (mediaExtension is not null)
                    {
                        using var extReader = mediaExtension.GetReader();
                        thumbnail = extReader.GetAttribute("url");
                    }

                    var summary = item.Summary?.Text ?? string.Empty;
                    summary = StripHtml(summary);

                    return new NewsItem(
                        Title: item.Title?.Text ?? string.Empty,
                        Summary: summary,
                        Link: item.Links.FirstOrDefault()?.Uri.ToString() ?? string.Empty,
                        Published: item.PublishDate.ToString("o"),
                        Thumbnail: thumbnail);
                })
                .ToList();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to fetch news feed from {FeedUrl}", feedUrl);
            return [];
        }
    }

    private static string StripHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var result = HtmlTagRegex().Replace(html, string.Empty);
        result = System.Net.WebUtility.HtmlDecode(result);
        return result.Trim();
    }

    [GeneratedRegex("<[^>]+>")]
    private static partial Regex HtmlTagRegex();
}
