using Pew.Dashboard.Application.Features.Weather;

namespace Pew.Dashboard.Application.Common.Interfaces;

public interface IWeatherService
{
    Task<WeatherResponse> GetWeatherAsync(CancellationToken ct);
}
