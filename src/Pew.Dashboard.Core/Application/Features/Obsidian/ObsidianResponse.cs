namespace Pew.Dashboard.Application.Features.Obsidian;

public sealed record ObsidianResponse(
    string DailyNote,
    string Date,
    List<VaultTask> Tasks,
    List<RecentNote> RecentNotes);

public sealed record VaultTask(string Text, string File, int Line);
public sealed record RecentNote(string Name, string Path);
