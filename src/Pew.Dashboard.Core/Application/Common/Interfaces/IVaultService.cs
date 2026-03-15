using Ardalis.Result;

using Pew.Dashboard.Application.Features.Obsidian;

namespace Pew.Dashboard.Application.Common.Interfaces;

public interface IVaultService
{
    Task<ObsidianResponse> GetDashboardAsync(CancellationToken ct);
    Task AppendNoteAsync(string text, CancellationToken ct);
    Task AddTaskAsync(string text, CancellationToken ct);
    Task<Result> ToggleTaskAsync(string filePath, int lineNumber, CancellationToken ct);
}
