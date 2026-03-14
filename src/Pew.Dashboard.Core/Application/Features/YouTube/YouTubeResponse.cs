namespace Pew.Dashboard.Application.Features.YouTube;

public sealed record YouTubeResponse(
    string Title,
    string VideoId,
    string Published,
    string Link,
    string? Error);
