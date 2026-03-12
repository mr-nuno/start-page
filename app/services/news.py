import feedparser
from app.config import NEWS_FEEDS, NEWS_ITEMS_LIMIT


def _parse_feed(url, limit):
    feed = feedparser.parse(url)
    items = []
    for entry in feed.entries[:limit]:
        thumbnail = None
        if hasattr(entry, "media_thumbnail") and entry.media_thumbnail:
            thumbnail = entry.media_thumbnail[0].get("url")
        items.append({
            "title": entry.get("title", ""),
            "summary": entry.get("summary", ""),
            "link": entry.get("link", ""),
            "published": entry.get("published", ""),
            "thumbnail": thumbnail,
        })
    return items


def fetch():
    result = {}
    for key, url in NEWS_FEEDS.items():
        try:
            result[key] = _parse_feed(url, NEWS_ITEMS_LIMIT)
        except Exception:
            result[key] = []
    return result
