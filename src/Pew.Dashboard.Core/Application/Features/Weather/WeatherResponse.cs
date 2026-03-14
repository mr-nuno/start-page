namespace Pew.Dashboard.Application.Features.Weather;

public sealed record WeatherResponse(
    double Temperature,
    double FeelsLike,
    double Humidity,
    double WindSpeed,
    string Description,
    string Icon,
    int WeatherCode,
    List<ForecastDay> Forecast);

public sealed record ForecastDay(
    string Date,
    double TempMax,
    double TempMin,
    string Description,
    string Icon,
    string Sunrise,
    string Sunset);
