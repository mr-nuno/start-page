using Pew.Dashboard.Application.Common.Interfaces;
using Ardalis.Result;
using MediatR;

namespace Pew.Dashboard.Application.Features.SystemInfo.GetSystem;

public sealed record GetSystemQuery : IRequest<Result<SystemResponse>>
{
    public sealed class Handler(ISystemService systemService) : IRequestHandler<GetSystemQuery, Result<SystemResponse>>
    {
        public async Task<Result<SystemResponse>> Handle(GetSystemQuery request, CancellationToken ct)
        {
            var response = await systemService.GetSystemAsync(ct);
            return Result<SystemResponse>.Success(response);
        }
    }
}
