using AngleSharp;
using Microsoft.Extensions.Caching.Memory;
using Pew.Dashboard.Application.Common.Interfaces;
using ILogger = Serilog.ILogger;

namespace Pew.Dashboard.Application.Features.Comic;

public sealed class ComicService(HttpClient httpClient, IMemoryCache cache, IDateTimeProvider dateTimeProvider) : IComicService
{
    private static readonly ILogger Log = Serilog.Log.ForContext<ComicService>();
    private const string CacheKey = "comic";
    private const string GoComicsBase = "https://www.gocomics.com/calvinandhobbes";

    public async Task<ComicResponse> GetComicAsync(CancellationToken ct)
    {
        if (cache.TryGetValue(CacheKey, out ComicResponse? cached) && cached is not null)
            return cached;

        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow.DateTime);
        var url = $"{GoComicsBase}/{today.Year}/{today.Month:00}/{today.Day:00}";

        try
        {
            var html = await httpClient.GetStringAsync(url, ct);

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(html), ct);

            string? imageUrl = null;

            // Try picture.item-comic-image img
            var img = document.QuerySelector("picture.item-comic-image img");
            if (img is not null)
            {
                imageUrl = img.GetAttribute("src") ?? img.GetAttribute("data-srcset") ?? img.GetAttribute("srcset");
            }

            // Try img.item-comic-image
            if (imageUrl is null)
            {
                img = document.QuerySelector("img.item-comic-image");
                if (img is not null)
                {
                    imageUrl = img.GetAttribute("src") ?? img.GetAttribute("data-srcset") ?? img.GetAttribute("srcset");
                }
            }

            // Fallback: og:image meta tag
            if (imageUrl is null)
            {
                var og = document.QuerySelector("meta[property='og:image']");
                if (og is not null)
                {
                    imageUrl = og.GetAttribute("content");
                }
            }

            // Take first URL if srcset
            if (imageUrl is not null && imageUrl.Contains(' '))
            {
                imageUrl = imageUrl.Split(' ')[0];
            }

            var response = new ComicResponse(imageUrl, url, today.ToString("yyyy-MM-dd"), "Calvin and Hobbes", null);
            cache.Set(CacheKey, response, TimeSpan.FromHours(24));
            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Comic fetch failed");
            return new ComicResponse(null, url, today.ToString("yyyy-MM-dd"), "Calvin and Hobbes", ex.Message);
        }
    }
}
