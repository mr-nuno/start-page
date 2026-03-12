from datetime import date

# Chord library: name, frets (0=open, -1=muted, N=fret), fingers, start_fret
# Strings: E A D G B e (low to high)
CHORDS = [
    {"name": "C Major", "short": "C", "frets": [-1, 3, 2, 0, 1, 0], "fingers": [0, 3, 2, 0, 1, 0]},
    {"name": "D Major", "short": "D", "frets": [-1, -1, 0, 2, 3, 2], "fingers": [0, 0, 0, 1, 3, 2]},
    {"name": "E Major", "short": "E", "frets": [0, 2, 2, 1, 0, 0], "fingers": [0, 2, 3, 1, 0, 0]},
    {"name": "G Major", "short": "G", "frets": [3, 2, 0, 0, 0, 3], "fingers": [2, 1, 0, 0, 0, 3]},
    {"name": "A Major", "short": "A", "frets": [-1, 0, 2, 2, 2, 0], "fingers": [0, 0, 1, 2, 3, 0]},
    {"name": "F Major", "short": "F", "frets": [1, 1, 2, 3, 3, 1], "fingers": [1, 1, 2, 4, 3, 1], "barre": 1},
    {"name": "B Major", "short": "B", "frets": [-1, 2, 4, 4, 4, 2], "fingers": [0, 1, 3, 3, 3, 1], "barre": 2, "start_fret": 2},
    {"name": "A Minor", "short": "Am", "frets": [-1, 0, 2, 2, 1, 0], "fingers": [0, 0, 2, 3, 1, 0]},
    {"name": "D Minor", "short": "Dm", "frets": [-1, -1, 0, 2, 3, 1], "fingers": [0, 0, 0, 2, 3, 1]},
    {"name": "E Minor", "short": "Em", "frets": [0, 2, 2, 0, 0, 0], "fingers": [0, 2, 3, 0, 0, 0]},
    {"name": "B Minor", "short": "Bm", "frets": [-1, 2, 4, 4, 3, 2], "fingers": [0, 1, 3, 4, 2, 1], "barre": 2, "start_fret": 2},
    {"name": "F Minor", "short": "Fm", "frets": [1, 1, 3, 3, 1, 1], "fingers": [1, 1, 3, 4, 1, 1], "barre": 1},
    {"name": "G7", "short": "G7", "frets": [3, 2, 0, 0, 0, 1], "fingers": [3, 2, 0, 0, 0, 1]},
    {"name": "C7", "short": "C7", "frets": [-1, 3, 2, 3, 1, 0], "fingers": [0, 3, 2, 4, 1, 0]},
    {"name": "D7", "short": "D7", "frets": [-1, -1, 0, 2, 1, 2], "fingers": [0, 0, 0, 2, 1, 3]},
    {"name": "A7", "short": "A7", "frets": [-1, 0, 2, 0, 2, 0], "fingers": [0, 0, 2, 0, 3, 0]},
    {"name": "E7", "short": "E7", "frets": [0, 2, 0, 1, 0, 0], "fingers": [0, 2, 0, 1, 0, 0]},
    {"name": "B7", "short": "B7", "frets": [-1, 2, 1, 2, 0, 2], "fingers": [0, 2, 1, 3, 0, 4]},
    {"name": "C Major 7", "short": "Cmaj7", "frets": [-1, 3, 2, 0, 0, 0], "fingers": [0, 3, 2, 0, 0, 0]},
    {"name": "A Minor 7", "short": "Am7", "frets": [-1, 0, 2, 0, 1, 0], "fingers": [0, 0, 2, 0, 1, 0]},
    {"name": "D Sus 4", "short": "Dsus4", "frets": [-1, -1, 0, 2, 3, 3], "fingers": [0, 0, 0, 1, 2, 3]},
    {"name": "A Sus 2", "short": "Asus2", "frets": [-1, 0, 2, 2, 0, 0], "fingers": [0, 0, 1, 2, 0, 0]},
    {"name": "E Sus 4", "short": "Esus4", "frets": [0, 2, 2, 2, 0, 0], "fingers": [0, 2, 3, 4, 0, 0]},
    {"name": "F Major 7", "short": "Fmaj7", "frets": [-1, -1, 3, 2, 1, 0], "fingers": [0, 0, 3, 2, 1, 0]},
    {"name": "G Sus 4", "short": "Gsus4", "frets": [3, 3, 0, 0, 1, 3], "fingers": [2, 3, 0, 0, 1, 4]},
    {"name": "C Add 9", "short": "Cadd9", "frets": [-1, 3, 2, 0, 3, 0], "fingers": [0, 2, 1, 0, 3, 0]},
    {"name": "E Minor 7", "short": "Em7", "frets": [0, 2, 0, 0, 0, 0], "fingers": [0, 1, 0, 0, 0, 0]},
    {"name": "D Minor 7", "short": "Dm7", "frets": [-1, -1, 0, 2, 1, 1], "fingers": [0, 0, 0, 2, 1, 1]},
    {"name": "Power Chord E5", "short": "E5", "frets": [0, 2, 2, -1, -1, -1], "fingers": [0, 1, 2, 0, 0, 0]},
    {"name": "Power Chord A5", "short": "A5", "frets": [-1, 0, 2, 2, -1, -1], "fingers": [0, 0, 1, 2, 0, 0]},
]

STRING_NAMES = ["E", "A", "D", "G", "B", "e"]


def fetch():
    today = date.today()
    idx = today.toordinal() % len(CHORDS)
    chord = CHORDS[idx]
    return {
        "name": chord["name"],
        "short": chord["short"],
        "svg": _generate_svg(chord),
        "strings": STRING_NAMES,
        "frets": chord["frets"],
        "tip": _get_tip(chord),
    }


def _get_tip(chord):
    if chord.get("barre"):
        return f"Barre chord — lay your index finger flat across fret {chord['barre']}"
    if "Minor" in chord["name"]:
        return "Minor chord — has a darker, sadder sound"
    if "7" in chord["name"]:
        return "Seventh chord — adds tension and wants to resolve"
    if "Sus" in chord["name"]:
        return "Suspended chord — neither major nor minor, creates anticipation"
    if "Add" in chord["name"]:
        return "Add chord — enriches the basic triad with an extra color tone"
    if "Power" in chord["name"]:
        return "Power chord — just root and fifth, great for rock and punk"
    return "Open chord — one of the essential shapes every guitarist should know"


def _generate_svg(chord):
    frets = chord["frets"]
    fingers = chord["fingers"]
    start_fret = chord.get("start_fret", 1)
    barre = chord.get("barre")

    # Dimensions
    w, h = 180, 220
    pad_top = 45
    pad_left = 35
    pad_right = 15
    num_frets_shown = 5
    num_strings = 6

    fw = w - pad_left - pad_right
    fh = h - pad_top - 30
    string_gap = fw / (num_strings - 1)
    fret_gap = fh / num_frets_shown

    svg = f'<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {w} {h}" width="{w}" height="{h}">'
    svg += '<style>text { font-family: Inter, -apple-system, sans-serif; }</style>'
    svg += '<defs><linearGradient id="dotGrad" x1="0%" y1="0%" x2="100%" y2="100%">'
    svg += '<stop offset="0%" style="stop-color:#6c8cff"/><stop offset="100%" style="stop-color:#22d3ee"/>'
    svg += '</linearGradient></defs>'
    svg += f'<rect width="{w}" height="{h}" fill="#12141e" rx="14"/>'
    svg += f'<rect x="0.5" y="0.5" width="{w-1}" height="{h-1}" fill="none" stroke="rgba(255,255,255,0.06)" rx="14"/>'

    # Chord name
    svg += f'<text x="{w/2}" y="24" text-anchor="middle" fill="#eaf0f6" font-size="17" font-weight="700" letter-spacing="1">{chord["short"]}</text>'

    # Nut or fret indicator
    if start_fret == 1:
        svg += f'<rect x="{pad_left-1}" y="{pad_top-3}" width="{fw+2}" height="5" fill="#eaf0f6" rx="2"/>'
    else:
        svg += f'<text x="{pad_left-14}" y="{pad_top + fret_gap/2 + 5}" text-anchor="middle" fill="#6b7a90" font-size="11" font-weight="600">{start_fret}</text>'

    # Fret lines
    for i in range(num_frets_shown + 1):
        y = pad_top + i * fret_gap
        svg += f'<line x1="{pad_left}" y1="{y}" x2="{pad_left + fw}" y2="{y}" stroke="rgba(255,255,255,0.08)" stroke-width="1"/>'

    # String lines
    for i in range(num_strings):
        x = pad_left + i * string_gap
        svg += f'<line x1="{x}" y1="{pad_top}" x2="{x}" y2="{pad_top + fh}" stroke="#6b7a90" stroke-width="1.5"/>'

    # Barre
    if barre:
        barre_strings = [i for i, f in enumerate(frets) if f == barre]
        if len(barre_strings) >= 2:
            x1 = pad_left + barre_strings[0] * string_gap
            x2 = pad_left + barre_strings[-1] * string_gap
            y = pad_top + (barre - start_fret + 0.5) * fret_gap
            svg += f'<line x1="{x1}" y1="{y}" x2="{x2}" y2="{y}" stroke="url(#dotGrad)" stroke-width="8" stroke-linecap="round" opacity="0.85"/>'

    # Finger dots and open/muted markers
    for i, fret_num in enumerate(frets):
        x = pad_left + i * string_gap

        if fret_num == -1:
            svg += f'<text x="{x}" y="{pad_top - 10}" text-anchor="middle" fill="#ff6b6b" font-size="13" font-weight="700">X</text>'
        elif fret_num == 0:
            svg += f'<circle cx="{x}" cy="{pad_top - 14}" r="5" fill="none" stroke="#51cf66" stroke-width="2"/>'
        else:
            display_fret = fret_num - start_fret + 1
            if 1 <= display_fret <= num_frets_shown:
                y = pad_top + (display_fret - 0.5) * fret_gap
                # Skip drawing individual dots for barre frets (already drawn)
                if barre and fret_num == barre and i != min(j for j, f in enumerate(frets) if f == barre):
                    pass
                else:
                    svg += f'<circle cx="{x}" cy="{y}" r="9" fill="url(#dotGrad)"/>'
                    svg += f'<circle cx="{x}" cy="{y}" r="9" fill="none" stroke="rgba(255,255,255,0.15)" stroke-width="1"/>'
                    if fingers[i] > 0:
                        svg += f'<text x="{x}" y="{y + 4}" text-anchor="middle" fill="#08090d" font-size="10" font-weight="700">{fingers[i]}</text>'

    # String names at bottom
    for i, name in enumerate(STRING_NAMES):
        x = pad_left + i * string_gap
        svg += f'<text x="{x}" y="{h - 5}" text-anchor="middle" fill="#6b7a90" font-size="10" font-weight="500">{name}</text>'

    svg += '</svg>'
    return svg
