using System.Diagnostics.CodeAnalysis;
using Azure.Core;
using Azure.Identity;
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
using Rsp.RtsImport.Infrastructure.HttpMessageHandlers;
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
        else
        {   // Use DefaultAzureCredential in development environment
            // Need to give access to user's account on API - Or verify if this works with user's
            // identity You should have these set in your locall settings or environment variables: "AZURE_CLIENT_ID","AZURE_TENANT_ID","AZURE_CLIENT_SECRET"
            builder.Services.AddSingleton<TokenCredential>(new DefaultAzureCredential());
        }

        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));
        var appSettings = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>()!;

        builder.Services.AddSingleton<TokenCredential>(_ =>
        {
            var opts = new DefaultAzureCredentialOptions
            {
                ExcludeInteractiveBrowserCredential = true,
            };

            // If present, this pins DefaultAzureCredential to a user-assigned identity when in Azure
            if (!string.IsNullOrWhiteSpace(appSettings.ManagedIdentityClientID))
            {
                opts.ManagedIdentityClientId = appSettings.ManagedIdentityClientID;
            }

            return new DefaultAzureCredential(opts);
        });

        // register dependencies
        builder.Services.AddMemoryCache();
        builder.Services.AddServices();
        builder.Services.AddDbContext<RtsDbContext>(options =>
        {
            options.EnableSensitiveDataLogging();
            options.UseSqlServer(builder.Configuration.GetConnectionString("RTSDatabaseConnection"));
        });

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddHeaderPropagation(options => options.Headers.Add(RequestHeadersKeys.CorrelationId));

        builder.Services.AddTransient<AuthHeadersHandler>();
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