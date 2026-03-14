namespace Pew.Dashboard.Application.Features.Guitar;

public sealed record GuitarResponse(
    string Name,
    string Short,
    string Svg,
    string[] Strings,
    int[] Frets,
    string Tip);
