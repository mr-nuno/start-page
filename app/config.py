LULEA_LAT = 65.5848
LULEA_LON = 22.1547

WEATHER_URL = (
    "https://api.open-meteo.com/v1/forecast"
    f"?latitude={LULEA_LAT}&longitude={LULEA_LON}"
    "&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code,wind_speed_10m"
    "&daily=temperature_2m_max,temperature_2m_min,weather_code,sunrise,sunset"
    "&timezone=Europe/Stockholm&forecast_days=5"
)

NEWS_FEEDS = {
    "local": "https://www.svt.se/nyheter/lokalt/norrbotten/rss.xml",
    "global": "https://www.dn.se/rss/",
}

NEWS_ITEMS_LIMIT = 8

CACHE_TTL = {
    "weather": 900,
    "news": 600,
    "sports": 1800,
    "system": 30,
}

WMO_CODES = {
    0: ("Clear sky", "☀️"),
    1: ("Mainly clear", "🌤️"),
    2: ("Partly cloudy", "⛅"),
    3: ("Overcast", "☁️"),
    45: ("Foggy", "🌫️"),
    48: ("Rime fog", "🌫️"),
    51: ("Light drizzle", "🌦️"),
    53: ("Moderate drizzle", "🌦️"),
    55: ("Dense drizzle", "🌦️"),
    56: ("Freezing drizzle", "🌧️"),
    57: ("Dense freezing drizzle", "🌧️"),
    61: ("Light rain", "🌧️"),
    63: ("Moderate rain", "🌧️"),
    65: ("Heavy rain", "🌧️"),
    66: ("Freezing rain", "🌧️"),
    67: ("Heavy freezing rain", "🌧️"),
    71: ("Light snow", "🌨️"),
    73: ("Moderate snow", "🌨️"),
    75: ("Heavy snow", "🌨️"),
    77: ("Snow grains", "🌨️"),
    80: ("Light showers", "🌦️"),
    81: ("Moderate showers", "🌦️"),
    82: ("Violent showers", "🌦️"),
    85: ("Light snow showers", "🌨️"),
    86: ("Heavy snow showers", "🌨️"),
    95: ("Thunderstorm", "⛈️"),
    96: ("Thunderstorm with hail", "⛈️"),
    99: ("Thunderstorm with heavy hail", "⛈️"),
}
