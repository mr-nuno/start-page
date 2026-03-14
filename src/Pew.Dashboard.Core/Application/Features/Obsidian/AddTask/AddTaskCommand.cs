using Ardalis.Result;
using FastEndpoints;
using FluentValidation;
using MediatR;

namespace Pew.Dashboard.Application.Features.Obsidian.AddTask;

public sealed record AddTaskCommand(string Text) : IRequest<Result>
{
    public sealed class Handler(IVaultService vaultService) : IRequestHandler<AddTaskCommand, Result>
    {
        public async Task<Result> Handle(AddTaskCommand request, CancellationToken ct)
        {
            await vaultService.AddTaskAsync(request.Text, ct);
            return Result.Success();
        }
    }

    public sealed class Validator : Validator<AddTaskCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Text).NotEmpty().WithMessage("Task text is required");
        }
    }
}
