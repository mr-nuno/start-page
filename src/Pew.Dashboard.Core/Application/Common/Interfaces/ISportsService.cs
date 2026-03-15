using Pew.Dashboard.Application.Features.Sports;

namespace Pew.Dashboard.Application.Common.Interfaces;

public interface ISportsService
{
    Task<SportsResponse> GetSportsAsync(CancellationToken ct);
}
