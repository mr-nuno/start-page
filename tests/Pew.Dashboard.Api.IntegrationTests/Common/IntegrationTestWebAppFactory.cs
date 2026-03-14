using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
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

namespace Pew.Dashboard.Api.IntegrationTests.Common;

public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            ReplaceService<IWeatherService>(services, CreateFakeWeatherService());
            ReplaceService<INewsService>(services, CreateFakeNewsService());
            ReplaceService<ISportsService>(services, CreateFakeSportsService());
            ReplaceService<ISystemService>(services, CreateFakeSystemService());
            ReplaceService<IGuitarService>(services, CreateFakeGuitarService());
            ReplaceService<IComicService>(services, CreateFakeComicService());
            ReplaceService<ISeinfeldService>(services, CreateFakeSeinfeldService());
            ReplaceService<ISongService>(services, CreateFakeSongService());
            ReplaceService<IQuoteService>(services, CreateFakeQuoteService());
            ReplaceService<IYouTubeService>(services, CreateFakeYouTubeService());
            ReplaceService<IVaultService>(services, CreateFakeVaultService());
        });
    }

    private static void ReplaceService<TService>(IServiceCollection services, TService implementation)
        where TService : class
    {
        var descriptors = services.Where(d => d.ServiceType == typeof(TService)).ToList();
        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }

        services.AddSingleton(implementation);
    }

    private static IWeatherService CreateFakeWeatherService()
    {
        var fake = Substitute.For<IWeatherService>();
        fake.GetWeatherAsync(Arg.Any<CancellationToken>())
            .Returns(new WeatherResponse(
                Temperature: 22.5,
                FeelsLike: 21.0,
                Humidity: 55,
                WindSpeed: 3.2,
                Description: "Clear sky",
                Icon: "01d",
                WeatherCode: 0,
                Forecast: [new ForecastDay("2026-03-14", 25.0, 15.0, "Sunny", "01d", "06:00", "18:00")]));
        return fake;
    }

    private static INewsService CreateFakeNewsService()
    {
        var fake = Substitute.For<INewsService>();
        fake.GetNewsAsync(Arg.Any<CancellationToken>())
            .Returns(new NewsResponse(
                Local: [new NewsItem("Local headline", "Local summary", "https://example.com/local", "2026-03-14", null)],
                Global: [new NewsItem("Global headline", "Global summary", "https://example.com/global", "2026-03-14", null)]));
        return fake;
    }

    private static ISportsService CreateFakeSportsService()
    {
        var fake = Substitute.For<ISportsService>();
        fake.GetSportsAsync(Arg.Any<CancellationToken>())
            .Returns(new SportsResponse(
                Team: "Lulea HF",
                League: "SHL",
                Standings: new Dictionary<string, string> { ["Position"] = "1st" },
                RecentGames: [new GameEntry("2026-03-10", "19:00", "Skelleftea", "4-2", "W")],
                NextGame: new GameEntry("2026-03-20", "19:00", "Frolunda", "", null)));
        return fake;
    }

    private static ISystemService CreateFakeSystemService()
    {
        var fake = Substitute.For<ISystemService>();
        fake.GetSystemAsync(Arg.Any<CancellationToken>())
            .Returns(new SystemResponse(
                Memory: new MemoryInfo(TotalGb: 16, UsedGb: 8, Percent: 50),
                Disk: new DiskInfo(TotalGb: 500, UsedGb: 250, FreeGb: 250, Percent: 50),
                Cpu: new CpuInfo(Percent: 25, TempC: 45)));
        return fake;
    }

    private static IGuitarService CreateFakeGuitarService()
    {
        var fake = Substitute.For<IGuitarService>();
        fake.GetChordOfTheDay()
            .Returns(new GuitarResponse(
                Name: "C Major",
                Short: "C",
                Svg: "<svg xmlns=\"http://www.w3.org/2000/svg\"></svg>",
                Strings: ["E", "A", "D", "G", "B", "e"],
                Frets: [0, 3, 2, 0, 1, 0],
                Tip: "Open chord"));
        return fake;
    }

    private static IComicService CreateFakeComicService()
    {
        var fake = Substitute.For<IComicService>();
        fake.GetComicAsync(Arg.Any<CancellationToken>())
            .Returns(new ComicResponse(
                ImageUrl: "https://example.com/comic.png",
                PageUrl: "https://example.com/comic",
                Date: "2026-03-14",
                Title: "Test Comic",
                Error: null));
        return fake;
    }

    private static ISeinfeldService CreateFakeSeinfeldService()
    {
        var fake = Substitute.For<ISeinfeldService>();
        fake.GetQuoteOfTheDay()
            .Returns(new SeinfeldResponse(
                Quote: "No soup for you!",
                Character: "Soup Nazi",
                Episode: "The Soup Nazi"));
        return fake;
    }

    private static ISongService CreateFakeSongService()
    {
        var fake = Substitute.For<ISongService>();
        fake.GetSongAsync(Arg.Any<CancellationToken>())
            .Returns(new SongResponse(
                Title: "Wonderwall",
                Artist: "Oasis",
                Chords: ["Em", "G", "D", "A"],
                Pattern: "D DU UDU",
                Lyrics: "Today is gonna be the day"));
        return fake;
    }

    private static IQuoteService CreateFakeQuoteService()
    {
        var fake = Substitute.For<IQuoteService>();
        fake.GetQuoteOfTheDay()
            .Returns(new QuoteResponse(
                Quote: "The only way to do great work is to love what you do.",
                Source: "Steve Jobs"));
        return fake;
    }

    private static IYouTubeService CreateFakeYouTubeService()
    {
        var fake = Substitute.For<IYouTubeService>();
        fake.GetVideoAsync(Arg.Any<CancellationToken>())
            .Returns(new YouTubeResponse(
                Title: "Test Video",
                VideoId: "dQw4w9WgXcQ",
                Published: "2026-03-14",
                Link: "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                Error: null));
        return fake;
    }

    private static IVaultService CreateFakeVaultService()
    {
        var fake = Substitute.For<IVaultService>();
        fake.GetDashboardAsync(Arg.Any<CancellationToken>())
            .Returns(new ObsidianResponse(
                DailyNote: "Today's note content",
                Date: "2026-03-14",
                Tasks: [new VaultTask("Buy groceries", "Daily Notes/2026-03-14.md", 5)],
                RecentNotes: [new RecentNote("Meeting Notes", "Notes/Meeting Notes.md")]));
        fake.AppendNoteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        fake.AddTaskAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        fake.ToggleTaskAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Ardalis.Result.Result.Success());
        return fake;
    }
}
