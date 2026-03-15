using Pew.Dashboard.Application.Features.Seinfeld;

namespace Pew.Dashboard.Application.Common.Interfaces;

public interface ISeinfeldService
{
    SeinfeldResponse GetQuoteOfTheDay();
}
