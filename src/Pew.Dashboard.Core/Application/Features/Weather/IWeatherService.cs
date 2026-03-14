namespace Pew.Dashboard.Application.Features.Weather;

public interface IWeatherService
{
    Task<WeatherResponse> GetWeatherAsync(CancellationToken ct);
}
