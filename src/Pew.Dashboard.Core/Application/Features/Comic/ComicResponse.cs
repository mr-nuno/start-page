namespace Pew.Dashboard.Application.Features.Comic;

public sealed record ComicResponse(
    string? ImageUrl,
    string PageUrl,
    string Date,
    string Title,
    string? Error);
