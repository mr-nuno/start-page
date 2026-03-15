using Pew.Dashboard.Application.Features.Guitar;

namespace Pew.Dashboard.Application.Common.Interfaces;

public interface IGuitarService
{
    GuitarResponse GetChordOfTheDay();
}
