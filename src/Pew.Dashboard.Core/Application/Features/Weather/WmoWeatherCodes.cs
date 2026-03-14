namespace Pew.Dashboard.Application.Features.Weather;

public static class WmoWeatherCodes
{
    private static readonly Dictionary<int, (string Description, string Emoji)> Codes = new()
    {
        [0] = ("Clear sky", "\u2600\ufe0f"),
        [1] = ("Mainly clear", "\ud83c\udf24\ufe0f"),
        [2] = ("Partly cloudy", "\u26c5"),
        [3] = ("Overcast", "\u2601\ufe0f"),
        [45] = ("Foggy", "\ud83c\udf2b\ufe0f"),
        [48] = ("Rime fog", "\ud83c\udf2b\ufe0f"),
        [51] = ("Light drizzle", "\ud83c\udf26\ufe0f"),
        [53] = ("Moderate drizzle", "\ud83c\udf26\ufe0f"),
        [55] = ("Dense drizzle", "\ud83c\udf26\ufe0f"),
        [56] = ("Freezing drizzle", "\ud83c\udf27\ufe0f"),
        [57] = ("Dense freezing drizzle", "\ud83c\udf27\ufe0f"),
        [61] = ("Light rain", "\ud83c\udf27\ufe0f"),
        [63] = ("Moderate rain", "\ud83c\udf27\ufe0f"),
        [65] = ("Heavy rain", "\ud83c\udf27\ufe0f"),
        [66] = ("Freezing rain", "\ud83c\udf27\ufe0f"),
        [67] = ("Heavy freezing rain", "\ud83c\udf27\ufe0f"),
        [71] = ("Light snow", "\ud83c\udf28\ufe0f"),
        [73] = ("Moderate snow", "\ud83c\udf28\ufe0f"),
        [75] = ("Heavy snow", "\ud83c\udf28\ufe0f"),
        [77] = ("Snow grains", "\ud83c\udf28\ufe0f"),
        [80] = ("Light showers", "\ud83c\udf26\ufe0f"),
        [81] = ("Moderate showers", "\ud83c\udf26\ufe0f"),
        [82] = ("Violent showers", "\ud83c\udf26\ufe0f"),
        [85] = ("Light snow showers", "\ud83c\udf28\ufe0f"),
        [86] = ("Heavy snow showers", "\ud83c\udf28\ufe0f"),
        [95] = ("Thunderstorm", "\u26c8\ufe0f"),
        [96] = ("Thunderstorm with hail", "\u26c8\ufe0f"),
        [99] = ("Thunderstorm with heavy hail", "\u26c8\ufe0f"),
    };

    public static (string Description, string Emoji) GetDescription(int code) =>
        Codes.TryGetValue(code, out var result) ? result : ("Unknown", "\u2753");
}
