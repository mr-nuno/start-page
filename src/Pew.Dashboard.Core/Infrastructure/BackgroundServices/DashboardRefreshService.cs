using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Pew.Dashboard.Application.Features.Comic;
using Pew.Dashboard.Application.Features.Guitar;
using Pew.Dashboard.Application.Features.News;
using Pew.Dashboard.Application.Features.Song;
using Pew.Dashboard.Application.Features.Sports;
using Pew.Dashboard.Application.Features.Weather;
using Pew.Dashboard.Application.Features.YouTube;
using Pew.Dashboard.Core;
using ILogger = Serilog.ILogger;

namespace Pew.Dashboard.Infrastructure.BackgroundServices;

public sealed class DashboardRefreshService(
    IServiceScopeFactory scopeFactory,
    IOptions<DashboardOptions> options,
    IGuitarService guitarService) : BackgroundService
{
    private static readonly ILogger Log = Serilog.Log.ForContext<DashboardRefreshService>();

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), ct);

        while (!ct.IsCancellationRequested)
        {
            Log.Information("Starting background refresh cycle");

            await RefreshService("Weather", async () =>
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IWeatherService>();
                await service.GetWeatherAsync(ct);
            });

            await RefreshService("News", async () =>
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<INewsService>();
                await service.GetNewsAsync(ct);
            });

            await RefreshService("Sports", async () =>
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<ISportsService>();
                await service.GetSportsAsync(ct);
            });

            await RefreshService("Comic", async () =>
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IComicService>();
                await service.GetComicAsync(ct);
            });

            await RefreshService("Song", async () =>
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<ISongService>();
                await service.GetSongAsync(ct);
            });

            await RefreshService("YouTube", async () =>
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IYouTubeService>();
                await service.GetVideoAsync(ct);
            });

            RefreshService("Guitar", () =>
            {
                guitarService.GetChordOfTheDay();
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();

            Log.Information("Background refresh cycle complete");

            await Task.Delay(TimeSpan.FromMinutes(options.Value.BackgroundRefreshMinutes), ct);
        }
    }

    private static async Task RefreshService(string name, Func<Task> action)
    {
        try
        {
            await action();
            Log.Information("Refreshed {ServiceName}", name);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Background refresh failed for {ServiceName}", name);
        }
    }
}
