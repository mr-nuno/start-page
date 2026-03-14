namespace Pew.Dashboard.Application.Features.Sports;

public sealed record SportsResponse(
    string Team,
    string League,
    Dictionary<string, string>? Standings,
    List<GameEntry> RecentGames,
    GameEntry? NextGame);

public sealed record GameEntry(
    string Date,
    string Time,
    string Opponent,
    string Score,
    string? Result);
