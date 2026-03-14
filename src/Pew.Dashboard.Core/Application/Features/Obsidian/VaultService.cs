using Ardalis.Result;
using Microsoft.Extensions.Options;
using Pew.Dashboard.Application.Common.Interfaces;
using Pew.Dashboard.Core;
using ILogger = Serilog.ILogger;

namespace Pew.Dashboard.Application.Features.Obsidian;

public sealed class VaultService(IOptions<VaultOptions> options, IDateTimeProvider dateTimeProvider) : IVaultService
{
    private static readonly ILogger Log = Serilog.Log.ForContext<VaultService>();

    private string VaultPath => options.Value.VaultPath;
    private string DailyNotesFolder => options.Value.DailyNotesFolder;

    private string GetDailyNotePath()
    {
        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow.DateTime);
        return Path.Combine(VaultPath, DailyNotesFolder, $"{today:yyyy-MM-dd}.md");
    }

    public Task<ObsidianResponse> GetDashboardAsync(CancellationToken ct)
    {
        var dailyNotePath = GetDailyNotePath();
        var dailyNote = File.Exists(dailyNotePath)
            ? File.ReadAllText(dailyNotePath)
            : "";

        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow.DateTime);
        var tasks = FindTasks(15);
        var recentNotes = GetRecentNotes(5);

        var response = new ObsidianResponse(dailyNote, today.ToString("yyyy-MM-dd"), tasks, recentNotes);
        return Task.FromResult(response);
    }

    public Task AppendNoteAsync(string text, CancellationToken ct)
    {
        var dailyNotePath = GetDailyNotePath();
        var dir = Path.GetDirectoryName(dailyNotePath)!;

        Directory.CreateDirectory(dir);

        if (!File.Exists(dailyNotePath))
        {
            var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow.DateTime);
            File.WriteAllText(dailyNotePath, $"# {today:yyyy-MM-dd}\n\n");
        }

        File.AppendAllText(dailyNotePath, text.TrimEnd() + "\n");
        return Task.CompletedTask;
    }

    public Task AddTaskAsync(string text, CancellationToken ct)
        => AppendNoteAsync($"- [ ] {text}", ct);

    public Task<Result> ToggleTaskAsync(string filePath, int lineNumber, CancellationToken ct)
    {
        var fullPath = Path.GetFullPath(Path.Combine(VaultPath, filePath));
        var normalizedVaultPath = Path.GetFullPath(VaultPath);

        if (!fullPath.StartsWith(normalizedVaultPath, StringComparison.Ordinal))
        {
            Log.Warning("Path traversal attempt detected: {FilePath}", filePath);
            return Task.FromResult(Result.Invalid(new ValidationError("Path traversal detected")));
        }

        if (!File.Exists(fullPath))
            return Task.FromResult(Result.NotFound("File not found"));

        var lines = File.ReadAllLines(fullPath);
        var index = lineNumber - 1;

        if (index < 0 || index >= lines.Length)
            return Task.FromResult(Result.NotFound("Line not found"));

        var line = lines[index];

        if (line.Contains("- [ ] "))
        {
            lines[index] = line.Replace("- [ ] ", "- [x] ");
        }
        else if (line.Contains("- [x] "))
        {
            lines[index] = line.Replace("- [x] ", "- [ ] ");
        }

        File.WriteAllLines(fullPath, lines);
        return Task.FromResult(Result.Success());
    }

    private List<VaultTask> FindTasks(int limit)
    {
        var tasks = new List<VaultTask>();

        if (!Directory.Exists(VaultPath))
            return tasks;

        var files = Directory.EnumerateFiles(VaultPath, "*.md", SearchOption.AllDirectories)
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.LastWriteTimeUtc);

        foreach (var file in files)
        {
            var relativePath = Path.GetRelativePath(VaultPath, file.FullName);

            // Skip directories starting with "."
            if (relativePath.Split(Path.DirectorySeparatorChar).Any(part => part.StartsWith('.')))
                continue;

            try
            {
                var lines = File.ReadAllLines(file.FullName);

                for (var i = 0; i < lines.Length; i++)
                {
                    var stripped = lines[i].Trim();

                    if (!stripped.StartsWith("- [ ] ", StringComparison.Ordinal))
                        continue;

                    tasks.Add(new VaultTask(stripped[6..], relativePath, i + 1));

                    if (tasks.Count >= limit)
                        return tasks;
                }
            }
            catch (Exception)
            {
                continue;
            }
        }

        return tasks;
    }

    private List<RecentNote> GetRecentNotes(int limit)
    {
        if (!Directory.Exists(VaultPath))
            return [];

        return Directory.EnumerateFiles(VaultPath, "*.md", SearchOption.AllDirectories)
            .Select(f => new FileInfo(f))
            .Where(f =>
            {
                var rel = Path.GetRelativePath(VaultPath, f.FullName);
                return !rel.Split(Path.DirectorySeparatorChar).Any(part => part.StartsWith('.'));
            })
            .OrderByDescending(f => f.LastWriteTimeUtc)
            .Take(limit)
            .Select(f => new RecentNote(
                Path.GetFileNameWithoutExtension(f.Name),
                Path.GetRelativePath(VaultPath, f.FullName)))
            .ToList();
    }
}
