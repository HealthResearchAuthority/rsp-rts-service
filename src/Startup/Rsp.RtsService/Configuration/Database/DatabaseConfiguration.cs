using Microsoft.EntityFrameworkCore;

namespace Rsp.RtsService.Configuration.Database;

/// <summary>
/// Adds DbContext to the application
/// </summary>
public static class DatabaseConfiguration
{
    /// <summary>
    /// Adds services to the IoC container
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: rename the database connection as needed
        services.AddDbContext<Infrastructure.DbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DatabaseConnection")));

        return services;
    }
}