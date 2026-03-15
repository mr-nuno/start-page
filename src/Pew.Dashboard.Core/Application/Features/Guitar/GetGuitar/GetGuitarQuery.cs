using Pew.Dashboard.Application.Common.Interfaces;
using Ardalis.Result;
using MediatR;

namespace Pew.Dashboard.Application.Features.Guitar.GetGuitar;

public sealed record GetGuitarQuery : IRequest<Result<GuitarResponse>>
{
    public sealed class Handler(IGuitarService guitarService) : IRequestHandler<GetGuitarQuery, Result<GuitarResponse>>
    {
        public Task<Result<GuitarResponse>> Handle(GetGuitarQuery request, CancellationToken ct)
        {
            var response = guitarService.GetChordOfTheDay();
            return Task.FromResult(Result<GuitarResponse>.Success(response));
        }
    }
}
