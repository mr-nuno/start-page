using Pew.Dashboard.Application.Common.Interfaces;

namespace Pew.Dashboard.Infrastructure.Services;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
