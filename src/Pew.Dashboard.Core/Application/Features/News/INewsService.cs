namespace Pew.Dashboard.Application.Features.News;

public interface INewsService
{
    Task<NewsResponse> GetNewsAsync(CancellationToken ct);
}
