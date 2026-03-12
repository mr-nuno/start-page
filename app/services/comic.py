import logging
from datetime import date
import requests
from bs4 import BeautifulSoup

log = logging.getLogger(__name__)

GOCOMICS_BASE = "https://www.gocomics.com/calvinandhobbes"
HEADERS = {
    "User-Agent": "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
}


def fetch():
    today = date.today()
    url = f"{GOCOMICS_BASE}/{today.year}/{today.month:02d}/{today.day:02d}"
    try:
        resp = requests.get(url, headers=HEADERS, timeout=15)
        resp.raise_for_status()
        soup = BeautifulSoup(resp.text, "html.parser")

        # GoComics embeds the strip image in a <picture> or <img> tag
        img = None

        # Try picture > source/img with comic class
        for tag in soup.select("picture.item-comic-image img, img.item-comic-image"):
            src = tag.get("src") or tag.get("data-srcset") or tag.get("srcset")
            if src:
                img = src.split()[0]  # Take first URL if srcset
                break

        # Fallback: look for og:image meta tag
        if not img:
            og = soup.find("meta", property="og:image")
            if og:
                img = og.get("content")

        return {
            "image_url": img,
            "page_url": url,
            "date": today.isoformat(),
            "title": "Calvin and Hobbes",
        }
    except Exception as e:
        log.error("Comic fetch failed: %s", e)
        return {
            "image_url": None,
            "page_url": url,
            "date": today.isoformat(),
            "title": "Calvin and Hobbes",
            "error": str(e),
        }
