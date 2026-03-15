using Pew.Dashboard.Application.Features.Comic;

namespace Pew.Dashboard.Application.Common.Interfaces;

public interface IComicService
{
    Task<ComicResponse> GetComicAsync(CancellationToken ct);
}
