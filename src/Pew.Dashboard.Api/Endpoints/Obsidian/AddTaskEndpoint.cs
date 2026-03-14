using FastEndpoints;
using MediatR;
using Pew.Dashboard.Api.Extensions;
using Pew.Dashboard.Application.Common.Models;
using Pew.Dashboard.Application.Features.Obsidian.AddTask;

namespace Pew.Dashboard.Api.Endpoints.Obsidian;

public sealed class AddTaskEndpoint(ISender sender) : Endpoint<AddTaskCommand, ApiResponse<object>>
{
    public override void Configure()
    {
        Post("/api/obsidian/task");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Add task to daily note";
            s.Description = "Adds a checkbox task to today's daily note.";
            s.ExampleRequest = new AddTaskCommand("Deploy the new version");
        });
        Options(x => x.WithTags("Obsidian"));
    }

    public override async Task HandleAsync(AddTaskCommand req, CancellationToken ct)
    {
        var result = await sender.Send(req, ct);
        var response = result.ToApiResponse();
        var statusCode = result.ToHttpStatusCode();
        await Send.ResponseAsync(response, statusCode, ct);
    }
}
