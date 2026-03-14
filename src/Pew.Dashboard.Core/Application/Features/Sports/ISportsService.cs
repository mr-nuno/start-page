namespace Pew.Dashboard.Application.Features.Sports;

public interface ISportsService
{
    Task<SportsResponse> GetSportsAsync(CancellationToken ct);
}
