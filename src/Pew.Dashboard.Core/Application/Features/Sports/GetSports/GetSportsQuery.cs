using Ardalis.Result;
using MediatR;

namespace Pew.Dashboard.Application.Features.Sports.GetSports;

public sealed record GetSportsQuery : IRequest<Result<SportsResponse>>
{
    public sealed class Handler(ISportsService sportsService) : IRequestHandler<GetSportsQuery, Result<SportsResponse>>
    {
        public async Task<Result<SportsResponse>> Handle(GetSportsQuery request, CancellationToken ct)
        {
            var response = await sportsService.GetSportsAsync(ct);
            return Result<SportsResponse>.Success(response);
        }
    }
}
