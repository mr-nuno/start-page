using Pew.Dashboard.Application.Features.SystemInfo;

namespace Pew.Dashboard.Application.Common.Interfaces;

public interface ISystemService
{
    Task<SystemResponse> GetSystemAsync(CancellationToken ct);
}
