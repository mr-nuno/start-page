import time
import threading
import logging
from flask import Flask, jsonify, request, send_from_directory

from app.config import CACHE_TTL
from app.services import weather, news, sports, system, guitar, comic, seinfeld, songofday, quotes, obsidian

logging.basicConfig(level=logging.INFO)
log = logging.getLogger(__name__)

app = Flask(__name__, static_folder="static")

_cache = {}


def _get_cached(key, ttl, fetch_fn):
    now = time.time()
    entry = _cache.get(key)
    if entry and (now - entry["time"]) < ttl:
        return entry["data"]
    try:
        data = fetch_fn()
        _cache[key] = {"data": data, "time": now}
        return data
    except Exception as e:
        log.error("Failed to fetch %s: %s", key, e)
        if entry:
            return entry["data"]
        return {"error": str(e)}


def _background_refresh():
    while True:
        for key, fn in [("weather", weather.fetch), ("news", news.fetch), ("sports", sports.fetch), ("guitar", guitar.fetch), ("comic", comic.fetch), ("song", songofday.fetch)]:
            try:
                data = fn()
                _cache[key] = {"data": data, "time": time.time()}
                log.info("Refreshed %s", key)
            except Exception as e:
                log.error("Background refresh %s failed: %s", key, e)
        time.sleep(300)


@app.route("/")
def index():
    return send_from_directory(app.static_folder, "index.html")


@app.route("/api/weather")
def api_weather():
    return jsonify(_get_cached("weather", CACHE_TTL["weather"], weather.fetch))


@app.route("/api/news")
def api_news():
    return jsonify(_get_cached("news", CACHE_TTL["news"], news.fetch))


@app.route("/api/sports")
def api_sports():
    return jsonify(_get_cached("sports", CACHE_TTL["sports"], sports.fetch))


@app.route("/api/system")
def api_system():
    return jsonify(_get_cached("system", CACHE_TTL["system"], system.fetch))


@app.route("/api/guitar")
def api_guitar():
    return jsonify(_get_cached("guitar", 86400, guitar.fetch))


@app.route("/api/comic")
def api_comic():
    return jsonify(_get_cached("comic", 86400, comic.fetch))


@app.route("/api/seinfeld")
def api_seinfeld():
    return jsonify(_get_cached("seinfeld", 86400, seinfeld.fetch))


@app.route("/api/song")
def api_song():
    return jsonify(_get_cached("song", 86400, songofday.fetch))


@app.route("/api/quote")
def api_quote():
    return jsonify(_get_cached("quote", 86400, quotes.fetch))


@app.route("/api/obsidian")
def api_obsidian():
    return jsonify(obsidian.fetch())


@app.route("/api/obsidian/note", methods=["POST"])
def api_obsidian_note():
    data = request.get_json(force=True)
    text = data.get("text", "").strip()
    if not text:
        return jsonify({"ok": False, "error": "Empty note"}), 400
    return jsonify(obsidian.append_note(text))


@app.route("/api/obsidian/task", methods=["POST"])
def api_obsidian_task():
    data = request.get_json(force=True)
    text = data.get("text", "").strip()
    if not text:
        return jsonify({"ok": False, "error": "Empty task"}), 400
    return jsonify(obsidian.add_task(text))


@app.route("/api/obsidian/toggle", methods=["POST"])
def api_obsidian_toggle():
    data = request.get_json(force=True)
    filepath = data.get("file", "")
    line = data.get("line", 0)
    if not filepath or not line:
        return jsonify({"ok": False, "error": "Missing file/line"}), 400
    return jsonify(obsidian.toggle_task(filepath, int(line)))


# Start background refresh thread
_refresh_thread = threading.Thread(target=_background_refresh, daemon=True)
_refresh_thread.start()
