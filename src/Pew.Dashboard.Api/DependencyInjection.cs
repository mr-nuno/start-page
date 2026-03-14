using FastEndpoints;
using FastEndpoints.Swagger;

namespace Pew.Dashboard.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHealthChecks();
        services.AddFastEndpoints();

        services.SwaggerDocument(o =>
        {
            o.DocumentSettings = s =>
            {
                s.Title = "Pew Dashboard API";
                s.Version = "v1";
            };
        });

        return services;
    }
}
