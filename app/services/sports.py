import json
import re
import logging
import requests
from bs4 import BeautifulSoup

log = logging.getLogger(__name__)

EP_BASE = "https://www.eliteprospects.com"
TEAM_URL = f"{EP_BASE}/team/7/lulea-hf"
REQ_HEADERS = {
    "User-Agent": "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
}


def fetch():
    try:
        return _fetch_eliteprospects()
    except Exception as e:
        log.error("Sports fetch failed: %s", e)
        return {"error": str(e), "team": "Luleå HF", "league": "SHL"}


def _find_nested(obj, key, results=None):
    """Recursively find all values for a given key in nested dicts/lists."""
    if results is None:
        results = []
    if isinstance(obj, dict):
        if key in obj:
            results.append(obj[key])
        for v in obj.values():
            _find_nested(v, key, results)
    elif isinstance(obj, list):
        for item in obj:
            _find_nested(item, key, results)
    return results


def _find_games(obj, results=None):
    """Recursively find game-like objects (dicts with homeTeam and visitingTeam)."""
    if results is None:
        results = []
    if isinstance(obj, dict):
        if "homeTeam" in obj and "visitingTeam" in obj:
            results.append(obj)
        else:
            for v in obj.values():
                _find_games(v, results)
    elif isinstance(obj, list):
        for item in obj:
            _find_games(item, results)
    return results


def _fetch_eliteprospects():
    resp = requests.get(TEAM_URL, headers=REQ_HEADERS, timeout=15)
    resp.raise_for_status()
    html = resp.text
    soup = BeautifulSoup(html, "html.parser")

    data = {
        "team": "Luleå HF",
        "league": "SHL",
        "standings": None,
        "recent_games": [],
        "next_game": None,
    }

    # --- Extract standings from the season history table ---
    tables = soup.find_all("table")
    for table in tables:
        ths = table.find_all("th")
        headers = [th.get_text(strip=True).lower() for th in ths]
        if "gp" in headers and "w" in headers and "season" in headers:
            rows = table.find_all("tr")
            for row in rows:
                cells = row.find_all("td")
                if not cells:
                    continue
                cell_text = [c.get_text(strip=True) for c in cells]
                row_text = " ".join(cell_text).lower()
                if "2025" in row_text or "2024" in row_text:
                    record = {}
                    for h, v in zip(headers, cell_text):
                        record[h] = v
                    data["standings"] = record
                    break
            break

    # --- Extract games from __NEXT_DATA__ ---
    next_data_tag = soup.find("script", id="__NEXT_DATA__")
    if next_data_tag and next_data_tag.string:
        try:
            next_data = json.loads(next_data_tag.string)
            games = _find_games(next_data)
            log.info("Found %d game objects in __NEXT_DATA__", len(games))

            completed = []
            upcoming = []

            for g in games:
                home_team = g.get("homeTeam") or {}
                visiting_team = g.get("visitingTeam") or {}

                # Get team names - handle both nested object and string
                if isinstance(home_team, dict):
                    home_name = home_team.get("name", "")
                else:
                    home_name = str(home_team)
                if isinstance(visiting_team, dict):
                    visiting_name = visiting_team.get("name", "")
                else:
                    visiting_name = str(visiting_team)

                is_home = "lule" in home_name.lower()
                is_away = "lule" in visiting_name.lower()
                if not is_home and not is_away:
                    continue

                opponent = visiting_name if is_home else home_name
                prefix = "vs" if is_home else "@"

                date_raw = g.get("dateTime") or g.get("date", "")
                short_date = str(date_raw)[:10]

                # Extract time
                time_str = ""
                time_match = re.search(r'T(\d{2}:\d{2})', str(date_raw))
                if time_match:
                    time_str = time_match.group(1)

                # Score
                home_score = g.get("homeTeamScore")
                visiting_score = g.get("visitingTeamScore")
                status = g.get("status", "")

                game_entry = {
                    "date": short_date,
                    "time": time_str,
                    "opponent": f"{prefix} {opponent}",
                    "score": "",
                }

                if home_score is not None and visiting_score is not None:
                    game_entry["score"] = f"{home_score}-{visiting_score}"
                    if is_home:
                        game_entry["result"] = "W" if home_score > visiting_score else "L"
                    else:
                        game_entry["result"] = "W" if visiting_score > home_score else "L"
                    completed.append(game_entry)
                else:
                    upcoming.append(game_entry)

            completed.sort(key=lambda x: x["date"])
            upcoming.sort(key=lambda x: x["date"])

            if completed:
                data["recent_games"] = completed[-5:]
            if upcoming:
                data["next_game"] = upcoming[0]

        except (json.JSONDecodeError, KeyError, TypeError) as e:
            log.error("Failed to parse __NEXT_DATA__: %s", e)

    return data
