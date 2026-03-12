import logging
import requests
from datetime import date

log = logging.getLogger(__name__)

# Curated list of songs with their chords mapped to sections
# Lyrics are fetched from lyrics.ovh API at runtime
SONGS = [
    {"title": "Knockin' on Heaven's Door", "artist": "Bob Dylan",
     "chords": ["G", "D", "Am", "G", "D", "C"],
     "pattern": "G - D - Am / G - D - C (repeat for each verse and chorus)"},
    {"title": "Wish You Were Here", "artist": "Pink Floyd",
     "chords": ["Em", "G", "Em", "G", "Em", "A", "Em", "A", "G"],
     "pattern": "Intro: Em - G / Verse: Em - G - Em - G - Em - A - Em - A - G"},
    {"title": "Redemption Song", "artist": "Bob Marley",
     "chords": ["G", "Em", "C", "G/B", "Am", "D"],
     "pattern": "Verse: G - Em - C - G/B - Am / Chorus: G - C - D - G - C - D - Em - C - D"},
    {"title": "Hallelujah", "artist": "Leonard Cohen",
     "chords": ["C", "Am", "F", "G", "E7"],
     "pattern": "Verse: C - Am - C - Am - F - G - C - G / Chorus: F - Am - F - C - G - C"},
    {"title": "House of the Rising Sun", "artist": "The Animals",
     "chords": ["Am", "C", "D", "F", "E"],
     "pattern": "Am - C - D - F / Am - C - E - E / Am - C - D - F / Am - E - Am - E"},
    {"title": "Hurt", "artist": "Johnny Cash",
     "chords": ["Am", "C", "D", "Am", "G"],
     "pattern": "Verse: Am - C - D - Am - C - D / Chorus: G - Am - F - C - G - Am"},
    {"title": "Wonderwall", "artist": "Oasis",
     "chords": ["Em7", "G", "Dsus4", "A7sus4", "C"],
     "pattern": "Verse: Em7 - G - Dsus4 - A7sus4 (repeat) / Chorus: C - D - Em (repeat)"},
    {"title": "Hotel California", "artist": "Eagles",
     "chords": ["Am", "E7", "G", "D", "F", "C", "Dm"],
     "pattern": "Am - E7 - G - D - F - C - Dm - E7"},
    {"title": "Sound of Silence", "artist": "Simon & Garfunkel",
     "chords": ["Am", "G", "F", "C"],
     "pattern": "Verse: Am - G / Am - F - C / F - C (repeat pattern)"},
    {"title": "Nothing Else Matters", "artist": "Metallica",
     "chords": ["Em", "D", "C", "G", "B7", "Am"],
     "pattern": "Verse: Em - D - C / Em - D - C / Chorus: G - B7 - Em / C - Am - D"},
    {"title": "Stairway to Heaven", "artist": "Led Zeppelin",
     "chords": ["Am", "E+", "C", "D", "Fmaj7", "G"],
     "pattern": "Intro: Am - E+ - C - D - Fmaj7 - G - Am / Verse continues same pattern"},
    {"title": "Blackbird", "artist": "The Beatles",
     "chords": ["G", "Am7", "G/B", "C", "D", "Em"],
     "pattern": "G - Am7 - G/B - G / C - C#dim - D - D#dim - Em"},
    {"title": "Fast Car", "artist": "Tracy Chapman",
     "chords": ["C", "G", "Em", "D"],
     "pattern": "C - G - Em - D (repeat for all sections)"},
    {"title": "Behind Blue Eyes", "artist": "The Who",
     "chords": ["Em", "G", "D", "Dsus4", "C", "A", "Asus2"],
     "pattern": "Verse: Em - G - D - Dsus4 - C / A - Asus2 / Chorus: C - D - G"},
    {"title": "Losing My Religion", "artist": "R.E.M.",
     "chords": ["Am", "Em", "Dm", "G", "F", "C"],
     "pattern": "Verse: Am - Em (repeat) / Chorus: F - Dm - G - Am - G - F"},
    {"title": "Creep", "artist": "Radiohead",
     "chords": ["G", "B", "C", "Cm"],
     "pattern": "G - B - C - Cm (repeat for entire song)"},
    {"title": "Mad World", "artist": "Gary Jules",
     "chords": ["Em", "G", "D", "A"],
     "pattern": "Em - G - D - A (repeat for verse and chorus)"},
    {"title": "Zombie", "artist": "The Cranberries",
     "chords": ["Em", "C", "G", "D"],
     "pattern": "Em - C - G - D (repeat for all sections)"},
    {"title": "Every Breath You Take", "artist": "The Police",
     "chords": ["G", "Em", "C", "D", "A7"],
     "pattern": "Verse: G - Em - C - D / Bridge: A7 - D"},
    {"title": "Hey There Delilah", "artist": "Plain White T's",
     "chords": ["D", "F#m", "Bm", "G", "A"],
     "pattern": "Verse: D - F#m (repeat) / Chorus: Bm - G - A - Bm - G - A - D"},
    {"title": "Horse with No Name", "artist": "America",
     "chords": ["Em", "D6/9"],
     "pattern": "Em - D6/9 (two chords for the entire song, alternating)"},
    {"title": "Wish You Were Here", "artist": "Incubus",
     "chords": ["Am", "F", "C", "G"],
     "pattern": "Verse: Am - F - C - G (repeat)"},
    {"title": "The Man Who Sold the World", "artist": "Nirvana",
     "chords": ["A", "Dm", "F", "C"],
     "pattern": "Verse: A - Dm / Chorus: F - C - A - Dm"},
    {"title": "Under the Bridge", "artist": "Red Hot Chili Peppers",
     "chords": ["D", "F#m", "E", "B", "A", "G#m", "C#m"],
     "pattern": "Intro: D - F#m / Verse: E - B - C#m - G#m - A / Chorus: F#m - E - B"},
    {"title": "Tears in Heaven", "artist": "Eric Clapton",
     "chords": ["A", "E", "F#m", "D", "E7", "A"],
     "pattern": "Verse: A - E - F#m - D - A - E / Chorus: F#m - D - A - E - A"},
    {"title": "Let It Be", "artist": "The Beatles",
     "chords": ["C", "G", "Am", "F"],
     "pattern": "C - G - Am - F / C - G - F - C (repeat for verse and chorus)"},
    {"title": "No Woman No Cry", "artist": "Bob Marley",
     "chords": ["C", "G", "Am", "F"],
     "pattern": "C - G - Am - F (repeat) / Chorus same pattern"},
    {"title": "Yellow", "artist": "Coldplay",
     "chords": ["B", "Badd11", "F#6", "Emaj7", "G#m", "E"],
     "pattern": "Verse: B - Badd11 - F#6 - Emaj7 / Chorus: G#m - F#6 - E - B"},
    {"title": "Hey Joe", "artist": "Jimi Hendrix",
     "chords": ["C", "G", "D", "A", "E"],
     "pattern": "C - G - D - A - E (repeat for all sections)"},
    {"title": "Time of Your Life", "artist": "Green Day",
     "chords": ["G", "C", "D", "Em"],
     "pattern": "Verse: G - C - D / Chorus: Em - D - C - G / Em - D - C - G"},
    {"title": "Have You Ever Seen the Rain", "artist": "Creedence Clearwater Revival",
     "chords": ["Am", "F", "C", "G"],
     "pattern": "Verse: Am - F - C - G / Chorus: F - G - C"},
]

LYRICS_API = "https://api.lyrics.ovh/v1"


def fetch():
    today = date.today()
    idx = today.toordinal() % len(SONGS)
    song = SONGS[idx]

    lyrics = _fetch_lyrics(song["artist"], song["title"])

    return {
        "title": song["title"],
        "artist": song["artist"],
        "chords": song["chords"],
        "pattern": song["pattern"],
        "lyrics": lyrics,
    }


def _fetch_lyrics(artist, title):
    try:
        resp = requests.get(f"{LYRICS_API}/{artist}/{title}", timeout=10)
        if resp.status_code == 200:
            data = resp.json()
            return data.get("lyrics", "")
    except Exception as e:
        log.warning("Lyrics fetch failed for %s - %s: %s", artist, title, e)
    return None
