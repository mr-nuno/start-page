namespace Pew.Dashboard.Application.Features.Comic;

public interface IComicService
{
    Task<ComicResponse> GetComicAsync(CancellationToken ct);
}
