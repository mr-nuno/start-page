using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Pew.Dashboard.Application.Common.Interfaces;
using Pew.Dashboard.Application.Features.Quote;
using Serilog;

namespace Pew.Dashboard.Infrastructure.Services.Quote;

public sealed class QuoteService(
    HttpClient httpClient,
    IMemoryCache cache,
    IDateTimeProvider dateTimeProvider) : IQuoteService
{
    private static readonly ILogger Log = Serilog.Log.ForContext<QuoteService>();
    private const string CacheKey = "quote";

    public async Task<QuoteResponse> GetQuoteOfTheDayAsync(CancellationToken ct)
    {
        if (cache.TryGetValue(CacheKey, out QuoteResponse? cached) && cached is not null)
        {
            return cached;
        }

        try
        {
            var json = await httpClient.GetStringAsync("https://zenquotes.io/api/today", ct);
            var doc = JsonDocument.Parse(json);
            var arr = doc.RootElement;

            if (arr.ValueKind == JsonValueKind.Array && arr.GetArrayLength() > 0)
            {
                var item = arr[0];
                var quote = item.GetProperty("q").GetString() ?? string.Empty;
                var author = item.GetProperty("a").GetString() ?? "Unknown";

                var response = new QuoteResponse(Quote: quote, Source: author);
                cache.Set(CacheKey, response, TimeSpan.FromHours(24));
                Log.Information("Quote refreshed from ZenQuotes API");
                return response;
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to fetch quote from ZenQuotes API, falling back to local library");

            if (cache.TryGetValue(CacheKey, out QuoteResponse? fallback) && fallback is not null)
            {
                return fallback;
            }
        }

        return GetFallbackQuote();
    }

    private QuoteResponse GetFallbackQuote()
    {
        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow.DateTime);
        var index = today.DayNumber % QuoteLibrary.Quotes.Length;
        var quote = QuoteLibrary.Quotes[index];

        return new QuoteResponse(Quote: quote.Quote, Source: quote.Source);
    }
}
