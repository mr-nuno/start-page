namespace Pew.Dashboard.Application.Features.Song;

public sealed record SongDefinition(string Title, string Artist, string[] Chords, string Pattern);

public static class SongLibrary
{
    public static readonly SongDefinition[] Songs =
    [
        new("Knockin' on Heaven's Door", "Bob Dylan",
            ["G", "D", "Am", "G", "D", "C"],
            "G - D - Am / G - D - C (repeat for each verse and chorus)"),

        new("Wish You Were Here", "Pink Floyd",
            ["Em", "G", "Em", "G", "Em", "A", "Em", "A", "G"],
            "Intro: Em - G / Verse: Em - G - Em - G - Em - A - Em - A - G"),

        new("Redemption Song", "Bob Marley",
            ["G", "Em", "C", "G/B", "Am", "D"],
            "Verse: G - Em - C - G/B - Am / Chorus: G - C - D - G - C - D - Em - C - D"),

        new("Hallelujah", "Leonard Cohen",
            ["C", "Am", "F", "G", "E7"],
            "Verse: C - Am - C - Am - F - G - C - G / Chorus: F - Am - F - C - G - C"),

        new("House of the Rising Sun", "The Animals",
            ["Am", "C", "D", "F", "E"],
            "Am - C - D - F / Am - C - E - E / Am - C - D - F / Am - E - Am - E"),

        new("Hurt", "Johnny Cash",
            ["Am", "C", "D", "Am", "G"],
            "Verse: Am - C - D - Am - C - D / Chorus: G - Am - F - C - G - Am"),

        new("Wonderwall", "Oasis",
            ["Em7", "G", "Dsus4", "A7sus4", "C"],
            "Verse: Em7 - G - Dsus4 - A7sus4 (repeat) / Chorus: C - D - Em (repeat)"),

        new("Hotel California", "Eagles",
            ["Am", "E7", "G", "D", "F", "C", "Dm"],
            "Am - E7 - G - D - F - C - Dm - E7"),

        new("Sound of Silence", "Simon & Garfunkel",
            ["Am", "G", "F", "C"],
            "Verse: Am - G / Am - F - C / F - C (repeat pattern)"),

        new("Nothing Else Matters", "Metallica",
            ["Em", "D", "C", "G", "B7", "Am"],
            "Verse: Em - D - C / Em - D - C / Chorus: G - B7 - Em / C - Am - D"),

        new("Stairway to Heaven", "Led Zeppelin",
            ["Am", "E+", "C", "D", "Fmaj7", "G"],
            "Intro: Am - E+ - C - D - Fmaj7 - G - Am / Verse continues same pattern"),

        new("Blackbird", "The Beatles",
            ["G", "Am7", "G/B", "C", "D", "Em"],
            "G - Am7 - G/B - G / C - C#dim - D - D#dim - Em"),

        new("Fast Car", "Tracy Chapman",
            ["C", "G", "Em", "D"],
            "C - G - Em - D (repeat for all sections)"),

        new("Behind Blue Eyes", "The Who",
            ["Em", "G", "D", "Dsus4", "C", "A", "Asus2"],
            "Verse: Em - G - D - Dsus4 - C / A - Asus2 / Chorus: C - D - G"),

        new("Losing My Religion", "R.E.M.",
            ["Am", "Em", "Dm", "G", "F", "C"],
            "Verse: Am - Em (repeat) / Chorus: F - Dm - G - Am - G - F"),

        new("Creep", "Radiohead",
            ["G", "B", "C", "Cm"],
            "G - B - C - Cm (repeat for entire song)"),

        new("Mad World", "Gary Jules",
            ["Em", "G", "D", "A"],
            "Em - G - D - A (repeat for verse and chorus)"),

        new("Zombie", "The Cranberries",
            ["Em", "C", "G", "D"],
            "Em - C - G - D (repeat for all sections)"),

        new("Every Breath You Take", "The Police",
            ["G", "Em", "C", "D", "A7"],
            "Verse: G - Em - C - D / Bridge: A7 - D"),

        new("Hey There Delilah", "Plain White T's",
            ["D", "F#m", "Bm", "G", "A"],
            "Verse: D - F#m (repeat) / Chorus: Bm - G - A - Bm - G - A - D"),

        new("Horse with No Name", "America",
            ["Em", "D6/9"],
            "Em - D6/9 (two chords for the entire song, alternating)"),

        new("Wish You Were Here", "Incubus",
            ["Am", "F", "C", "G"],
            "Verse: Am - F - C - G (repeat)"),

        new("The Man Who Sold the World", "Nirvana",
            ["A", "Dm", "F", "C"],
            "Verse: A - Dm / Chorus: F - C - A - Dm"),

        new("Under the Bridge", "Red Hot Chili Peppers",
            ["D", "F#m", "E", "B", "A", "G#m", "C#m"],
            "Intro: D - F#m / Verse: E - B - C#m - G#m - A / Chorus: F#m - E - B"),

        new("Tears in Heaven", "Eric Clapton",
            ["A", "E", "F#m", "D", "E7", "A"],
            "Verse: A - E - F#m - D - A - E / Chorus: F#m - D - A - E - A"),

        new("Let It Be", "The Beatles",
            ["C", "G", "Am", "F"],
            "C - G - Am - F / C - G - F - C (repeat for verse and chorus)"),

        new("No Woman No Cry", "Bob Marley",
            ["C", "G", "Am", "F"],
            "C - G - Am - F (repeat) / Chorus same pattern"),

        new("Yellow", "Coldplay",
            ["B", "Badd11", "F#6", "Emaj7", "G#m", "E"],
            "Verse: B - Badd11 - F#6 - Emaj7 / Chorus: G#m - F#6 - E - B"),

        new("Hey Joe", "Jimi Hendrix",
            ["C", "G", "D", "A", "E"],
            "C - G - D - A - E (repeat for all sections)"),

        new("Time of Your Life", "Green Day",
            ["G", "C", "D", "Em"],
            "Verse: G - C - D / Chorus: Em - D - C - G / Em - D - C - G"),

        new("Have You Ever Seen the Rain", "Creedence Clearwater Revival",
            ["Am", "F", "C", "G"],
            "Verse: Am - F - C - G / Chorus: F - G - C"),
    ];
}
