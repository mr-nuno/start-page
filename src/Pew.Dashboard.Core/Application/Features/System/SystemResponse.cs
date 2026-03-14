namespace Pew.Dashboard.Application.Features.SystemInfo;

public sealed record SystemResponse(
    MemoryInfo Memory,
    DiskInfo Disk,
    CpuInfo Cpu);

public sealed record MemoryInfo(double TotalGb, double UsedGb, double Percent);
public sealed record DiskInfo(double TotalGb, double UsedGb, double FreeGb, double Percent);
public sealed record CpuInfo(double Percent, double? TempC);
