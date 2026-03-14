namespace Pew.Dashboard.Application.Features.Guitar;

public sealed record ChordDefinition(
    string Name,
    string Short,
    int[] Frets,
    int[] Fingers,
    int? Barre = null,
    int StartFret = 1);

public static class GuitarChords
{
    public static readonly ChordDefinition[] Chords =
    [
        new("C Major", "C", [-1, 3, 2, 0, 1, 0], [0, 3, 2, 0, 1, 0]),
        new("D Major", "D", [-1, -1, 0, 2, 3, 2], [0, 0, 0, 1, 3, 2]),
        new("E Major", "E", [0, 2, 2, 1, 0, 0], [0, 2, 3, 1, 0, 0]),
        new("G Major", "G", [3, 2, 0, 0, 0, 3], [2, 1, 0, 0, 0, 3]),
        new("A Major", "A", [-1, 0, 2, 2, 2, 0], [0, 0, 1, 2, 3, 0]),
        new("F Major", "F", [1, 1, 2, 3, 3, 1], [1, 1, 2, 4, 3, 1], Barre: 1),
        new("B Major", "B", [-1, 2, 4, 4, 4, 2], [0, 1, 3, 3, 3, 1], Barre: 2, StartFret: 2),
        new("A Minor", "Am", [-1, 0, 2, 2, 1, 0], [0, 0, 2, 3, 1, 0]),
        new("D Minor", "Dm", [-1, -1, 0, 2, 3, 1], [0, 0, 0, 2, 3, 1]),
        new("E Minor", "Em", [0, 2, 2, 0, 0, 0], [0, 2, 3, 0, 0, 0]),
        new("B Minor", "Bm", [-1, 2, 4, 4, 3, 2], [0, 1, 3, 4, 2, 1], Barre: 2, StartFret: 2),
        new("F Minor", "Fm", [1, 1, 3, 3, 1, 1], [1, 1, 3, 4, 1, 1], Barre: 1),
        new("G7", "G7", [3, 2, 0, 0, 0, 1], [3, 2, 0, 0, 0, 1]),
        new("C7", "C7", [-1, 3, 2, 3, 1, 0], [0, 3, 2, 4, 1, 0]),
        new("D7", "D7", [-1, -1, 0, 2, 1, 2], [0, 0, 0, 2, 1, 3]),
        new("A7", "A7", [-1, 0, 2, 0, 2, 0], [0, 0, 2, 0, 3, 0]),
        new("E7", "E7", [0, 2, 0, 1, 0, 0], [0, 2, 0, 1, 0, 0]),
        new("B7", "B7", [-1, 2, 1, 2, 0, 2], [0, 2, 1, 3, 0, 4]),
        new("C Major 7", "Cmaj7", [-1, 3, 2, 0, 0, 0], [0, 3, 2, 0, 0, 0]),
        new("A Minor 7", "Am7", [-1, 0, 2, 0, 1, 0], [0, 0, 2, 0, 1, 0]),
        new("D Sus 4", "Dsus4", [-1, -1, 0, 2, 3, 3], [0, 0, 0, 1, 2, 3]),
        new("A Sus 2", "Asus2", [-1, 0, 2, 2, 0, 0], [0, 0, 1, 2, 0, 0]),
        new("E Sus 4", "Esus4", [0, 2, 2, 2, 0, 0], [0, 2, 3, 4, 0, 0]),
        new("F Major 7", "Fmaj7", [-1, -1, 3, 2, 1, 0], [0, 0, 3, 2, 1, 0]),
        new("G Sus 4", "Gsus4", [3, 3, 0, 0, 1, 3], [2, 3, 0, 0, 1, 4]),
        new("C Add 9", "Cadd9", [-1, 3, 2, 0, 3, 0], [0, 2, 1, 0, 3, 0]),
        new("E Minor 7", "Em7", [0, 2, 0, 0, 0, 0], [0, 1, 0, 0, 0, 0]),
        new("D Minor 7", "Dm7", [-1, -1, 0, 2, 1, 1], [0, 0, 0, 2, 1, 1]),
        new("Power Chord E5", "E5", [0, 2, 2, -1, -1, -1], [0, 1, 2, 0, 0, 0]),
        new("Power Chord A5", "A5", [-1, 0, 2, 2, -1, -1], [0, 0, 1, 2, 0, 0]),
    ];
}
