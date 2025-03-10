using Microsoft.Extensions.DependencyInjection;
using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsImport.Infrastructure.HttpMessageHandlers;
using Rsp.RtsImport.Services;

namespace Rsp.RtsImport.Startup.Configuration;

public static class ServicesConfiguration
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IOrganisationService, OrganisationsService>();
        services.AddTransient<IOrganisationImportService, OrganisationImportService>();

        services.AddTransient<RtsAuthHeadersHandler>();
        services.AddTransient<RtsPreAuthHeadersHandler>();

        return services;
    }
}