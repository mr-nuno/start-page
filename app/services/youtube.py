import logging
import feedparser

log = logging.getLogger(__name__)

# The Daily Show YouTube channel
CHANNEL_ID = "UCwWhs_6x42TyRM4Wstoq8HA"
FEED_URL = f"https://www.youtube.com/feeds/videos.xml?channel_id={CHANNEL_ID}"


def fetch():
    try:
        feed = feedparser.parse(FEED_URL)
        if not feed.entries:
            return {"error": "No videos found"}

        entry = feed.entries[0]
        video_id = entry.get("yt_videoid", "")
        if not video_id and "watch?v=" in entry.get("link", ""):
            video_id = entry["link"].split("watch?v=")[-1]

        return {
            "title": entry.get("title", ""),
            "video_id": video_id,
            "published": entry.get("published", ""),
            "link": entry.get("link", ""),
        }
    except Exception as e:
        log.error("YouTube fetch failed: %s", e)
        return {"error": str(e)}
