using System.Globalization;
using System.Text;
using Pew.Dashboard.Application.Common.Interfaces;

using Pew.Dashboard.Application.Features.Guitar;

namespace Pew.Dashboard.Infrastructure.Services.Guitar;

public sealed class GuitarService(IDateTimeProvider dateTimeProvider) : IGuitarService
{
    private static readonly string[] StringNames = ["E", "A", "D", "G", "B", "e"];

    public GuitarResponse GetChordOfTheDay()
    {
        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow.DateTime);
        var index = today.DayNumber % GuitarChords.Chords.Length;
        var chord = GuitarChords.Chords[index];

        return new GuitarResponse(
            Name: chord.Name,
            Short: chord.Short,
            Svg: GenerateSvg(chord),
            Strings: StringNames,
            Frets: chord.Frets,
            Tip: GetTip(chord));
    }

    private static string GetTip(ChordDefinition chord)
    {
        if (chord.Barre.HasValue)
            return $"Barre chord \u2014 lay your index finger flat across fret {chord.Barre.Value}";
        if (chord.Name.Contains("Minor"))
            return "Minor chord \u2014 has a darker, sadder sound";
        if (chord.Name.Contains('7'))
            return "Seventh chord \u2014 adds tension and wants to resolve";
        if (chord.Name.Contains("Sus"))
            return "Suspended chord \u2014 neither major nor minor, creates anticipation";
        if (chord.Name.Contains("Add"))
            return "Add chord \u2014 enriches the basic triad with an extra color tone";
        if (chord.Name.Contains("Power"))
            return "Power chord \u2014 just root and fifth, great for rock and punk";

        return "Open chord \u2014 one of the essential shapes every guitarist should know";
    }

    private static string GenerateSvg(ChordDefinition chord)
    {
        var frets = chord.Frets;
        var fingers = chord.Fingers;
        var startFret = chord.StartFret;
        var barre = chord.Barre;

        const int w = 180;
        const int h = 220;
        const int padTop = 45;
        const int padLeft = 35;
        const int padRight = 15;
        const int numFretsShown = 5;
        const int numStrings = 6;

        var fw = (double)(w - padLeft - padRight);
        var fh = (double)(h - padTop - 30);
        var stringGap = fw / (numStrings - 1);
        var fretGap = fh / numFretsShown;

        var svg = new StringBuilder();
        svg.Append(CultureInfo.InvariantCulture, $"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 {w} {h}\" width=\"{w}\" height=\"{h}\">");
        svg.Append("<style>text { font-family: Inter, -apple-system, sans-serif; }</style>");
        svg.Append("<defs><linearGradient id=\"dotGrad\" x1=\"0%\" y1=\"0%\" x2=\"100%\" y2=\"100%\">");
        svg.Append("<stop offset=\"0%\" style=\"stop-color:#6c8cff\"/><stop offset=\"100%\" style=\"stop-color:#22d3ee\"/>");
        svg.Append("</linearGradient></defs>");
        svg.Append(CultureInfo.InvariantCulture, $"<rect width=\"{w}\" height=\"{h}\" fill=\"#12141e\" rx=\"14\"/>");
        svg.Append(CultureInfo.InvariantCulture, $"<rect x=\"0.5\" y=\"0.5\" width=\"{w - 1}\" height=\"{h - 1}\" fill=\"none\" stroke=\"rgba(255,255,255,0.06)\" rx=\"14\"/>");

        // Chord name
        var cx = w / 2.0;
        svg.Append(CultureInfo.InvariantCulture, $"<text x=\"{cx}\" y=\"24\" text-anchor=\"middle\" fill=\"#eaf0f6\" font-size=\"17\" font-weight=\"700\" letter-spacing=\"1\">{chord.Short}</text>");

        // Nut or fret indicator
        if (startFret == 1)
        {
            svg.Append(CultureInfo.InvariantCulture, $"<rect x=\"{padLeft - 1}\" y=\"{padTop - 3}\" width=\"{fw + 2}\" height=\"5\" fill=\"#eaf0f6\" rx=\"2\"/>");
        }
        else
        {
            var fy = padTop + fretGap / 2.0 + 5;
            svg.Append(CultureInfo.InvariantCulture, $"<text x=\"{padLeft - 14}\" y=\"{fy}\" text-anchor=\"middle\" fill=\"#6b7a90\" font-size=\"11\" font-weight=\"600\">{startFret}</text>");
        }

        // Fret lines
        for (var i = 0; i <= numFretsShown; i++)
        {
            var y = padTop + i * fretGap;
            svg.Append(CultureInfo.InvariantCulture, $"<line x1=\"{padLeft}\" y1=\"{y}\" x2=\"{padLeft + fw}\" y2=\"{y}\" stroke=\"rgba(255,255,255,0.08)\" stroke-width=\"1\"/>");
        }

        // String lines
        for (var i = 0; i < numStrings; i++)
        {
            var x = padLeft + i * stringGap;
            svg.Append(CultureInfo.InvariantCulture, $"<line x1=\"{x}\" y1=\"{padTop}\" x2=\"{x}\" y2=\"{padTop + fh}\" stroke=\"#6b7a90\" stroke-width=\"1.5\"/>");
        }

        // Barre
        if (barre.HasValue)
        {
            var barreStrings = new List<int>();
            for (var i = 0; i < frets.Length; i++)
            {
                if (frets[i] == barre.Value)
                    barreStrings.Add(i);
            }

            if (barreStrings.Count >= 2)
            {
                var x1 = padLeft + barreStrings[0] * stringGap;
                var x2 = padLeft + barreStrings[^1] * stringGap;
                var y = padTop + (barre.Value - startFret + 0.5) * fretGap;
                svg.Append(CultureInfo.InvariantCulture, $"<line x1=\"{x1}\" y1=\"{y}\" x2=\"{x2}\" y2=\"{y}\" stroke=\"url(#dotGrad)\" stroke-width=\"8\" stroke-linecap=\"round\" opacity=\"0.85\"/>");
            }
        }

        // Finger dots and open/muted markers
        for (var i = 0; i < frets.Length; i++)
        {
            var fretNum = frets[i];
            var x = padLeft + i * stringGap;

            if (fretNum == -1)
            {
                svg.Append(CultureInfo.InvariantCulture, $"<text x=\"{x}\" y=\"{padTop - 10}\" text-anchor=\"middle\" fill=\"#ff6b6b\" font-size=\"13\" font-weight=\"700\">X</text>");
            }
            else if (fretNum == 0)
            {
                svg.Append(CultureInfo.InvariantCulture, $"<circle cx=\"{x}\" cy=\"{padTop - 14}\" r=\"5\" fill=\"none\" stroke=\"#51cf66\" stroke-width=\"2\"/>");
            }
            else
            {
                var displayFret = fretNum - startFret + 1;
                if (displayFret >= 1 && displayFret <= numFretsShown)
                {
                    var y = padTop + (displayFret - 0.5) * fretGap;

                    // Skip drawing individual dots for barre frets (already drawn)
                    if (barre.HasValue && fretNum == barre.Value)
                    {
                        var minBarreIndex = -1;
                        for (var j = 0; j < frets.Length; j++)
                        {
                            if (frets[j] == barre.Value)
                            {
                                minBarreIndex = j;
                                break;
                            }
                        }

                        if (i != minBarreIndex)
                            continue;
                    }

                    svg.Append(CultureInfo.InvariantCulture, $"<circle cx=\"{x}\" cy=\"{y}\" r=\"9\" fill=\"url(#dotGrad)\"/>");
                    svg.Append(CultureInfo.InvariantCulture, $"<circle cx=\"{x}\" cy=\"{y}\" r=\"9\" fill=\"none\" stroke=\"rgba(255,255,255,0.15)\" stroke-width=\"1\"/>");
                    if (fingers[i] > 0)
                    {
                        svg.Append(CultureInfo.InvariantCulture, $"<text x=\"{x}\" y=\"{y + 4}\" text-anchor=\"middle\" fill=\"#08090d\" font-size=\"10\" font-weight=\"700\">{fingers[i]}</text>");
                    }
                }
            }
        }

        // String names at bottom
        for (var i = 0; i < StringNames.Length; i++)
        {
            var x = padLeft + i * stringGap;
            svg.Append(CultureInfo.InvariantCulture, $"<text x=\"{x}\" y=\"{h - 5}\" text-anchor=\"middle\" fill=\"#6b7a90\" font-size=\"10\" font-weight=\"500\">{StringNames[i]}</text>");
        }

        svg.Append("</svg>");
        return svg.ToString();
    }
}
