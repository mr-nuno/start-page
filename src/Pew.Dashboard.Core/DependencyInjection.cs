using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pew.Dashboard.Application.Common.Interfaces;
using Pew.Dashboard.Application.Features.Comic;
using Pew.Dashboard.Application.Features.Guitar;
using Pew.Dashboard.Application.Features.News;
using Pew.Dashboard.Application.Features.Obsidian;
using Pew.Dashboard.Application.Features.Quote;
using Pew.Dashboard.Application.Features.Seinfeld;
using Pew.Dashboard.Application.Features.Song;
using Pew.Dashboard.Application.Features.Sports;
using Pew.Dashboard.Application.Features.SystemInfo;
using Pew.Dashboard.Application.Features.Weather;
using Pew.Dashboard.Application.Features.YouTube;
using Pew.Dashboard.Infrastructure.BackgroundServices;
using Pew.Dashboard.Infrastructure.Services;
using Pew.Dashboard.Infrastructure.Services.Comic;
using Pew.Dashboard.Infrastructure.Services.Guitar;
using Pew.Dashboard.Infrastructure.Services.News;
using Pew.Dashboard.Infrastructure.Services.Obsidian;
using Pew.Dashboard.Infrastructure.Services.Quote;
using Pew.Dashboard.Infrastructure.Services.Seinfeld;
using Pew.Dashboard.Infrastructure.Services.Song;
using Pew.Dashboard.Infrastructure.Services.Sports;
using Pew.Dashboard.Infrastructure.Services.SystemInfo;
using Pew.Dashboard.Infrastructure.Services.Weather;
using Pew.Dashboard.Infrastructure.Services.YouTube;

namespace Pew.Dashboard.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Application
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Infrastructure
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddMemoryCache();

        // Options
        services.Configure<WeatherOptions>(configuration.GetSection("Weather"));
        services.Configure<NewsOptions>(configuration.GetSection("News"));
        services.Configure<VaultOptions>(configuration.GetSection("Vault"));
        services.Configure<DashboardOptions>(configuration.GetSection("Dashboard"));

        // HttpClient registrations with resilience
        services.AddHttpClient<IWeatherService, WeatherService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
        }).AddStandardResilienceHandler();

        services.AddHttpClient<INewsService, NewsService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
        }).AddStandardResilienceHandler();

        services.AddHttpClient<ISportsService, SportsService>(client =>
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            client.Timeout = TimeSpan.FromSeconds(15);
        }).AddStandardResilienceHandler();

        services.AddHttpClient<IComicService, ComicService>(client =>
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            client.Timeout = TimeSpan.FromSeconds(15);
        }).AddStandardResilienceHandler();

        services.AddHttpClient<ISongService, SongService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
        }).AddStandardResilienceHandler();

        services.AddHttpClient<IYouTubeService, YouTubeService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
        }).AddStandardResilienceHandler();

        // Services without HttpClient
        services.AddSingleton<IGuitarService, GuitarService>();
        services.AddSingleton<ISeinfeldService, SeinfeldService>();
        services.AddSingleton<IQuoteService, QuoteService>();
        services.AddSingleton<ISystemService, SystemService>();
        services.AddSingleton<IVaultService, VaultService>();

        // Background refresh
        services.AddHostedService<DashboardRefreshService>();

        return services;
    }
}

public sealed class DashboardOptions
{
    public int BackgroundRefreshMinutes { get; set; } = 5;
}

public sealed class WeatherOptions
{
    public double Latitude { get; set; } = 65.5848;
    public double Longitude { get; set; } = 22.1547;
    public int CacheMinutes { get; set; } = 15;
}

public sealed class NewsOptions
{
    public string LocalFeedUrl { get; set; } = "https://www.svt.se/nyheter/lokalt/norrbotten/rss.xml";
    public string GlobalFeedUrl { get; set; } = "https://www.dn.se/rss/";
    public int MaxItems { get; set; } = 8;
    public int CacheMinutes { get; set; } = 10;
}

public sealed class VaultOptions
{
    public string VaultPath { get; set; } = "/vault";
    public string DailyNotesFolder { get; set; } = "Daily Notes";
}
