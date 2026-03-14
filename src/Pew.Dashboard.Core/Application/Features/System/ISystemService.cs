namespace Pew.Dashboard.Application.Features.SystemInfo;

public interface ISystemService
{
    Task<SystemResponse> GetSystemAsync(CancellationToken ct);
}
