using Rsp.RtsService.Application;
using Rsp.RtsService.Application.Authentication.Helpers;
using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Application.Contracts.Services;
using Rsp.RtsService.Infrastructure.Repositories;
using Rsp.RtsService.Services;

namespace Rsp.RtsService.Configuration.Dependencies;

/// <summary>
///  User Defined Services Configuration
/// </summary>
public static class ServicesConfiguration
{
    /// <summary>
    /// Adds services to the IoC container
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // example of configuring the IoC container to inject the dependencies

        services.AddSingleton<ITokenHelper, TokenHelper>();

        services.AddTransient<IRtsService, Service>();
        services.AddTransient<IRepository, Repository>();
        services.AddMediatR(option => option.RegisterServicesFromAssemblyContaining<IApplication>());

        return services;
    }
}