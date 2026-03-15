using Pew.Dashboard.Application.Common.Interfaces;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Pew.Dashboard.Core;
using Serilog;

using Pew.Dashboard.Application.Features.Weather;

namespace Pew.Dashboard.Infrastructure.Services.Weather;

public sealed class WeatherService(
    HttpClient httpClient,
    IMemoryCache cache,
    IOptions<WeatherOptions> options) : IWeatherService
{
    private static readonly ILogger Log = Serilog.Log.ForContext<WeatherService>();
    private const string CacheKey = "weather";

    public async Task<WeatherResponse> GetWeatherAsync(CancellationToken ct)
    {
        if (cache.TryGetValue(CacheKey, out WeatherResponse? cached) && cached is not null)
            return cached;

        try
        {
            var opts = options.Value;
            var lat = opts.Latitude.ToString(CultureInfo.InvariantCulture);
            var lon = opts.Longitude.ToString(CultureInfo.InvariantCulture);

            var url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}" +
                      "&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code,wind_speed_10m" +
                      "&daily=temperature_2m_max,temperature_2m_min,weather_code,sunrise,sunset" +
                      "&timezone=Europe/Stockholm&forecast_days=5";

            var json = await httpClient.GetStringAsync(url, ct);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var current = root.GetProperty("current");
            var weatherCode = current.GetProperty("weather_code").GetInt32();
            var (description, icon) = WmoWeatherCodes.GetDescription(weatherCode);

            var daily = root.GetProperty("daily");
            var dates = daily.GetProperty("time");
            var maxTemps = daily.GetProperty("temperature_2m_max");
            var minTemps = daily.GetProperty("temperature_2m_min");
            var dailyCodes = daily.GetProperty("weather_code");
            var sunrises = daily.GetProperty("sunrise");
            var sunsets = daily.GetProperty("sunset");

            var forecast = new List<ForecastDay>();
            for (var i = 0; i < dates.GetArrayLength(); i++)
            {
                var dayCode = dailyCodes[i].GetInt32();
                var (dayDesc, dayIcon) = WmoWeatherCodes.GetDescription(dayCode);

                forecast.Add(new ForecastDay(
                    Date: dates[i].GetString()!,
                    TempMax: maxTemps[i].GetDouble(),
                    TempMin: minTemps[i].GetDouble(),
                    Description: dayDesc,
                    Icon: dayIcon,
                    Sunrise: sunrises[i].GetString()!,
                    Sunset: sunsets[i].GetString()!));
            }

            var response = new WeatherResponse(
                Temperature: current.GetProperty("temperature_2m").GetDouble(),
                FeelsLike: current.GetProperty("apparent_temperature").GetDouble(),
                Humidity: current.GetProperty("relative_humidity_2m").GetDouble(),
                WindSpeed: current.GetProperty("wind_speed_10m").GetDouble(),
                Description: description,
                Icon: icon,
                WeatherCode: weatherCode,
                Forecast: forecast);

            cache.Set(CacheKey, response, TimeSpan.FromMinutes(opts.CacheMinutes));
            Log.Information("Weather data refreshed successfully");

            return response;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to fetch weather data, returning cached data if available");

            if (cache.TryGetValue(CacheKey, out WeatherResponse? fallback) && fallback is not null)
                return fallback;

            throw;
        }
    }
}
