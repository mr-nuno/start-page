using Ardalis.Result;
using FastEndpoints;
using FluentValidation;
using MediatR;

namespace Pew.Dashboard.Application.Features.Obsidian.AppendNote;

public sealed record AppendNoteCommand(string Text) : IRequest<Result>
{
    public sealed class Handler(IVaultService vaultService) : IRequestHandler<AppendNoteCommand, Result>
    {
        public async Task<Result> Handle(AppendNoteCommand request, CancellationToken ct)
        {
            await vaultService.AppendNoteAsync(request.Text, ct);
            return Result.Success();
        }
    }

    public sealed class Validator : Validator<AppendNoteCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Text).NotEmpty().WithMessage("Note text is required");
        }
    }
}
