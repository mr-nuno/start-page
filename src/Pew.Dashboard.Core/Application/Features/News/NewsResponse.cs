namespace Pew.Dashboard.Application.Features.News;

public sealed record NewsResponse(
    List<NewsItem> Local,
    List<NewsItem> Global);

public sealed record NewsItem(
    string Title,
    string Summary,
    string Link,
    string Published,
    string? Thumbnail);
