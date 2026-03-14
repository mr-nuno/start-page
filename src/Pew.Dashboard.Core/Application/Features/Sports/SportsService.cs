using System.Text.Json;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Caching.Memory;
using ILogger = Serilog.ILogger;

namespace Pew.Dashboard.Application.Features.Sports;

public sealed class SportsService(HttpClient httpClient, IMemoryCache cache) : ISportsService
{
    private static readonly ILogger Log = Serilog.Log.ForContext<SportsService>();
    private const string CacheKey = "sports";
    private const string TeamUrl = "https://www.eliteprospects.com/team/7/lulea-hf";

    public async Task<SportsResponse> GetSportsAsync(CancellationToken ct)
    {
        if (cache.TryGetValue(CacheKey, out SportsResponse? cached) && cached is not null)
            return cached;

        try
        {
            var response = await FetchFromEliteProspectsAsync(ct);
            cache.Set(CacheKey, response, TimeSpan.FromMinutes(30));
            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Sports fetch failed");
            return new SportsResponse("Lule\u00e5 HF", "SHL", null, [], null);
        }
    }

    private async Task<SportsResponse> FetchFromEliteProspectsAsync(CancellationToken ct)
    {
        var html = await httpClient.GetStringAsync(TeamUrl, ct);

        var context = BrowsingContext.New(Configuration.Default);
        var document = await context.OpenAsync(req => req.Content(html), ct);

        var standings = ExtractStandings(document);
        var (recentGames, nextGame) = ExtractGames(html);

        return new SportsResponse("Lule\u00e5 HF", "SHL", standings, recentGames, nextGame);
    }

    private static Dictionary<string, string>? ExtractStandings(IDocument document)
    {
        var tables = document.QuerySelectorAll("table");

        foreach (var table in tables)
        {
            var ths = table.QuerySelectorAll("th");
            var headers = ths.Select(th => th.TextContent.Trim().ToLowerInvariant()).ToList();

            if (!headers.Contains("gp") || !headers.Contains("w") || !headers.Contains("season"))
                continue;

            var rows = table.QuerySelectorAll("tr");

            foreach (var row in rows)
            {
                var cells = row.QuerySelectorAll("td");
                if (cells.Length == 0)
                    continue;

                var cellTexts = cells.Select(c => c.TextContent.Trim()).ToList();
                var rowText = string.Join(" ", cellTexts).ToLowerInvariant();

                if (!rowText.Contains("2025") && !rowText.Contains("2024"))
                    continue;

                var record = new Dictionary<string, string>();
                for (var i = 0; i < Math.Min(headers.Count, cellTexts.Count); i++)
                {
                    record[headers[i]] = cellTexts[i];
                }

                return record;
            }

            break;
        }

        return null;
    }

    private static (List<GameEntry> Recent, GameEntry? Next) ExtractGames(string html)
    {
        var scriptMatch = Regex.Match(html, @"<script\s+id=""__NEXT_DATA__""[^>]*>(.*?)</script>", RegexOptions.Singleline);
        if (!scriptMatch.Success)
            return ([], null);

        try
        {
            using var jsonDoc = JsonDocument.Parse(scriptMatch.Groups[1].Value);
            var games = new List<JsonElement>();
            FindGames(jsonDoc.RootElement, games);

            Log.Information("Found {GameCount} game objects in __NEXT_DATA__", games.Count);

            var completed = new List<GameEntry>();
            var upcoming = new List<GameEntry>();

            foreach (var g in games)
            {
                var homeName = GetTeamName(g, "homeTeam");
                var visitingName = GetTeamName(g, "visitingTeam");

                var isHome = homeName.Contains("lule", StringComparison.OrdinalIgnoreCase);
                var isAway = visitingName.Contains("lule", StringComparison.OrdinalIgnoreCase);

                if (!isHome && !isAway)
                    continue;

                var opponent = isHome ? visitingName : homeName;
                var prefix = isHome ? "vs" : "@";

                var dateRaw = g.TryGetProperty("dateTime", out var dt) ? dt.ToString()
                    : g.TryGetProperty("date", out var d) ? d.ToString()
                    : "";

                var shortDate = dateRaw.Length >= 10 ? dateRaw[..10] : dateRaw;

                var timeStr = "";
                var timeMatch = Regex.Match(dateRaw, @"T(\d{2}:\d{2})");
                if (timeMatch.Success)
                    timeStr = timeMatch.Groups[1].Value;

                var hasHomeScore = g.TryGetProperty("homeTeamScore", out var hs) && hs.ValueKind == JsonValueKind.Number;
                var hasVisitingScore = g.TryGetProperty("visitingTeamScore", out var vs) && vs.ValueKind == JsonValueKind.Number;

                if (hasHomeScore && hasVisitingScore)
                {
                    var homeScore = hs.GetInt32();
                    var visitingScore = vs.GetInt32();
                    var score = $"{homeScore}-{visitingScore}";
                    var result = isHome
                        ? (homeScore > visitingScore ? "W" : "L")
                        : (visitingScore > homeScore ? "W" : "L");

                    completed.Add(new GameEntry(shortDate, timeStr, $"{prefix} {opponent}", score, result));
                }
                else
                {
                    upcoming.Add(new GameEntry(shortDate, timeStr, $"{prefix} {opponent}", "", null));
                }
            }

            completed.Sort((a, b) => string.Compare(a.Date, b.Date, StringComparison.Ordinal));
            upcoming.Sort((a, b) => string.Compare(a.Date, b.Date, StringComparison.Ordinal));

            var recentGames = completed.Count > 5 ? completed[^5..] : completed;
            var nextGame = upcoming.Count > 0 ? upcoming[0] : null;

            return (recentGames, nextGame);
        }
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or InvalidOperationException)
        {
            Log.Error(ex, "Failed to parse __NEXT_DATA__");
            return ([], null);
        }
    }

    private static string GetTeamName(JsonElement game, string propertyName)
    {
        if (!game.TryGetProperty(propertyName, out var team))
            return "";

        if (team.ValueKind == JsonValueKind.Object)
            return team.TryGetProperty("name", out var name) ? name.GetString() ?? "" : "";

        return team.ToString();
    }

    private static void FindGames(JsonElement element, List<JsonElement> results)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                if (element.TryGetProperty("homeTeam", out _) && element.TryGetProperty("visitingTeam", out _))
                {
                    results.Add(element);
                }
                else
                {
                    foreach (var property in element.EnumerateObject())
                    {
                        FindGames(property.Value, results);
                    }
                }
                break;

            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                {
                    FindGames(item, results);
                }
                break;
        }
    }
}
