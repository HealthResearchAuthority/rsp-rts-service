namespace Rsp.RtsImport.Startup.Configuration;

[ExcludeFromCodeCoverage]
public static class ServicesConfiguration
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IOrganisationService, OrganisationsService>();
        services.AddTransient<IOrganisationImportService, OrganisationImportService>();

        services.AddTransient<RtsAuthHeadersHandler>();

        return services;
    }
}