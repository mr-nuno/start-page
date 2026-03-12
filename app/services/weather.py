import requests
from app.config import WEATHER_URL, WMO_CODES


def fetch():
    resp = requests.get(WEATHER_URL, timeout=10)
    resp.raise_for_status()
    data = resp.json()

    current = data.get("current", {})
    daily = data.get("daily", {})

    weather_code = current.get("weather_code", 0)
    desc, icon = WMO_CODES.get(weather_code, ("Unknown", "❓"))

    forecast = []
    for i in range(len(daily.get("time", []))):
        day_code = daily["weather_code"][i]
        day_desc, day_icon = WMO_CODES.get(day_code, ("Unknown", "❓"))
        forecast.append({
            "date": daily["time"][i],
            "temp_max": daily["temperature_2m_max"][i],
            "temp_min": daily["temperature_2m_min"][i],
            "description": day_desc,
            "icon": day_icon,
            "sunrise": daily["sunrise"][i],
            "sunset": daily["sunset"][i],
        })

    return {
        "temperature": current.get("temperature_2m"),
        "feels_like": current.get("apparent_temperature"),
        "humidity": current.get("relative_humidity_2m"),
        "wind_speed": current.get("wind_speed_10m"),
        "description": desc,
        "icon": icon,
        "weather_code": weather_code,
        "forecast": forecast,
    }
