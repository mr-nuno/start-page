using System.Globalization;
using Microsoft.Extensions.Caching.Memory;
using ILogger = Serilog.ILogger;

namespace Pew.Dashboard.Application.Features.SystemInfo;

public sealed class SystemService(IMemoryCache cache) : ISystemService
{
    private static readonly ILogger Log = Serilog.Log.ForContext<SystemService>();
    private const string CacheKey = "system";

    public async Task<SystemResponse> GetSystemAsync(CancellationToken ct)
    {
        if (cache.TryGetValue(CacheKey, out SystemResponse? cached) && cached is not null)
            return cached;

        var memory = GetMemoryInfo();
        var disk = GetDiskInfo();
        var cpu = await GetCpuInfoAsync(ct);

        var response = new SystemResponse(memory, disk, cpu);
        cache.Set(CacheKey, response, TimeSpan.FromSeconds(30));
        return response;
    }

    private static MemoryInfo GetMemoryInfo()
    {
        try
        {
            var lines = File.ReadAllLines("/proc/meminfo");
            long totalKb = 0;
            long availableKb = 0;

            foreach (var line in lines)
            {
                if (line.StartsWith("MemTotal:", StringComparison.Ordinal))
                {
                    totalKb = ParseMemInfoValue(line);
                }
                else if (line.StartsWith("MemAvailable:", StringComparison.Ordinal))
                {
                    availableKb = ParseMemInfoValue(line);
                }

                if (totalKb > 0 && availableKb > 0)
                    break;
            }

            var totalGb = Math.Round(totalKb / (1024.0 * 1024.0), 1);
            var usedGb = Math.Round((totalKb - availableKb) / (1024.0 * 1024.0), 1);
            var percent = totalKb > 0 ? Math.Round((totalKb - availableKb) * 100.0 / totalKb, 1) : 0;

            return new MemoryInfo(totalGb, usedGb, percent);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to read memory info from /proc/meminfo");
            return new MemoryInfo(0, 0, 0);
        }
    }

    private static long ParseMemInfoValue(string line)
    {
        var parts = line.Split(':', 2);
        if (parts.Length < 2)
            return 0;

        var value = parts[1].Trim().Replace("kB", "", StringComparison.OrdinalIgnoreCase).Trim();
        return long.TryParse(value, CultureInfo.InvariantCulture, out var result) ? result : 0;
    }

    private static DiskInfo GetDiskInfo()
    {
        try
        {
            var path = Directory.Exists("/host") ? "/host" : "/";
            var drive = new DriveInfo(path);

            var totalGb = Math.Round(drive.TotalSize / (1024.0 * 1024.0 * 1024.0), 1);
            var freeGb = Math.Round(drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0), 1);
            var usedGb = Math.Round((drive.TotalSize - drive.AvailableFreeSpace) / (1024.0 * 1024.0 * 1024.0), 1);
            var percent = drive.TotalSize > 0 ? Math.Round((drive.TotalSize - drive.AvailableFreeSpace) * 100.0 / drive.TotalSize, 1) : 0;

            return new DiskInfo(totalGb, usedGb, freeGb, percent);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to read disk info");
            return new DiskInfo(0, 0, 0, 0);
        }
    }

    private static async Task<CpuInfo> GetCpuInfoAsync(CancellationToken ct)
    {
        try
        {
            var (idle1, total1) = ReadCpuJiffies();
            await Task.Delay(500, ct);
            var (idle2, total2) = ReadCpuJiffies();

            var idleDelta = idle2 - idle1;
            var totalDelta = total2 - total1;
            var cpuPercent = totalDelta > 0 ? Math.Round((1.0 - (double)idleDelta / totalDelta) * 100.0, 1) : 0;

            var temp = GetCpuTemperature();

            return new CpuInfo(cpuPercent, temp);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to read CPU info");
            return new CpuInfo(0, null);
        }
    }

    private static (long Idle, long Total) ReadCpuJiffies()
    {
        var line = File.ReadLines("/proc/stat").First();
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // cpu user nice system idle iowait irq softirq steal
        if (parts.Length < 5)
            return (0, 0);

        long total = 0;
        for (var i = 1; i < parts.Length; i++)
        {
            if (long.TryParse(parts[i], CultureInfo.InvariantCulture, out var val))
                total += val;
        }

        var idle = long.TryParse(parts[4], CultureInfo.InvariantCulture, out var idleVal) ? idleVal : 0;

        return (idle, total);
    }

    private static double? GetCpuTemperature()
    {
        for (var i = 0; i < 10; i++)
        {
            var path = $"/sys/class/thermal/thermal_zone{i}/temp";
            try
            {
                var content = File.ReadAllText(path).Trim();
                if (int.TryParse(content, CultureInfo.InvariantCulture, out var millidegrees))
                    return Math.Round(millidegrees / 1000.0, 1);
            }
            catch (FileNotFoundException)
            {
                continue;
            }
            catch (DirectoryNotFoundException)
            {
                continue;
            }
        }

        return null;
    }
}
