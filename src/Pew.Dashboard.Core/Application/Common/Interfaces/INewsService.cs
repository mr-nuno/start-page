using Pew.Dashboard.Application.Features.News;

namespace Pew.Dashboard.Application.Common.Interfaces;

public interface INewsService
{
    Task<NewsResponse> GetNewsAsync(CancellationToken ct);
}
