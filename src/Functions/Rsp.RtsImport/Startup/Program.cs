using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Rsp.Logging.Extensions;
using Rsp.Logging.Interceptors;
using Rsp.RtsImport.Application.Constants;
using Rsp.RtsImport.Application.Settings;
using Rsp.RtsImport.Startup.Configuration;
using Rsp.RtsService.Infrastructure;

namespace Rsp.RtsImport.Startup;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = FunctionsApplication.CreateBuilder(args);
        builder.ConfigureFunctionsWebApplication();

        // 1) Development-only local config
        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration
                .AddJsonFile("local.settings.json", true, true)
                .AddUserSecrets<UserSecretsAnchor>(true);
        }

        // 2) Common config
        builder.Configuration
            .AddJsonFile("featuresettings.json", true, true)
            .AddEnvironmentVariables();

        // 3) Attach Azure App Configuration in non-Dev
        if (!builder.Environment.IsDevelopment())
        {
            builder.Services.AddAzureAppConfiguration(builder.Configuration);
        }

        // register dependencies
        builder.Services.AddMemoryCache();
        builder.Services.AddServices();
        builder.Services.AddDbContext<RtsDbContext>(options =>
        {
            options.EnableSensitiveDataLogging();
            options.UseSqlServer(builder.Configuration.GetConnectionString("RTSDatabaseConnection"));
        });

        builder.Services.AddDbContext<IrasContext>(options =>
        {
            options.EnableSensitiveDataLogging();
            options.UseSqlServer(builder.Configuration.GetConnectionString("IrasServiceDatabaseConnection"));
        });

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddHeaderPropagation(options => options.Headers.Add(RequestHeadersKeys.CorrelationId));

        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));
        var appSettings = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>()!;

        builder.Services.AddHttpClients(appSettings);

        // register configurationSettings as singleton
        builder.Services.AddSingleton(appSettings);

        // Creating a feature manager without the use of DI. Injecting IFeatureManager via DI is
        // appropriate in constructor methods. At the startup, it's not recommended to call
        // services.BuildServiceProvider and retrieve IFeatureManager via provider. Instead, the
        // following approach is recommended by creating FeatureManager with
        // ConfigurationFeatureDefinitionProvider using the existing configuration.
        var featureManager = new FeatureManager(new ConfigurationFeatureDefinitionProvider(builder.Configuration));

        if (await featureManager.IsEnabledAsync(Features.InterceptedLogging))
        {
            builder.Services.AddLoggingInterceptor<LoggingInterceptor>();
        }

        var app = builder.Build();

        await app.RunAsync();
    }
}