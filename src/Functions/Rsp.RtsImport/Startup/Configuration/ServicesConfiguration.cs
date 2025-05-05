using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsImport.Infrastructure.HttpMessageHandlers;
using Rsp.RtsImport.Services;
using IOrganisationService = Rsp.RtsImport.Application.Contracts.IOrganisationService;

namespace Rsp.RtsImport.Startup.Configuration;

[ExcludeFromCodeCoverage]
public static class ServicesConfiguration
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IAuditService, AuditService>();
        services.AddTransient<IMetadataService, MetadataService>();
        services.AddTransient<IOrganisationService, OrganisationsService>();
        services.AddTransient<IOrganisationImportService, OrganisationImportService>();
        services.AddTransient<ITokenService, TokenService>();

        services.AddTransient<RtsAuthHeadersHandler>();

        return services;
    }
}