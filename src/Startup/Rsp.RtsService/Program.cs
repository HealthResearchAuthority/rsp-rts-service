using System.Net.Mime;
using System.Text.Json;
using Rsp.RtsService.Application.Settings;
using Rsp.RtsService.Configuration.Auth;
using Rsp.RtsService.Configuration.Database;
using Rsp.RtsService.Configuration.Dependencies;
using Rsp.RtsService.Configuration.Swagger;
using Rsp.RtsService.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Rsp.ServiceDefaults;
using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;
using Rsp.RtsService.Application.Constants;
using Rsp.Logging.ActionFilters;

// TODO: If you using .NET Aspire provide the namespace for the project
// using ServiceDefaults

var builder = WebApplication.CreateBuilder(args);

//Add logger
builder
    .Configuration
    .AddJsonFile("logsettings.json")
    .AddEnvironmentVariables(); ;

builder.AddServiceDefaults();

var services = builder.Services;
var configuration = builder.Configuration;

if (!builder.Environment.IsDevelopment())
{
    services.AddLogging(builder => builder.AddConsole());
}

if (!builder.Environment.IsDevelopment())
{
    var azureAppConfigSection = configuration.GetSection(nameof(AppSettings));
    var azureAppConfiguration = azureAppConfigSection.Get<AppSettings>();

    // Load configuration from Azure App Configuration
    builder.Configuration.AddAzureAppConfiguration(
        options =>
        {
            options.Connect
            (
                new Uri(azureAppConfiguration!.AzureAppConfiguration.Endpoint),
                new ManagedIdentityCredential(azureAppConfiguration.AzureAppConfiguration.IdentityClientID)
            )
            .Select(KeyFilter.Any)
            .Select(KeyFilter.Any, AppSettings.ServiceLabel)
            .ConfigureRefresh(refreshOptions =>
                refreshOptions
                .Register("AppSettings:Sentinel", AppSettings.ServiceLabel, refreshAll: true)
                .SetCacheExpiration(new TimeSpan(0, 0, 15))
            );
        }
    );

    services.AddAzureAppConfiguration();
}

builder
    .Host
    .UseSerilog
    (
        (host, logger) =>
            logger
                .ReadFrom.Configuration(host.Configuration)
                .Enrich.WithCorrelationIdHeader()
    );

// this method is called by multiple projects
// serilog settings has been moved here, as all projects
// would need it
// TODO: if you using .NET Aspire and have added the ServiceDefaults project
// uncomment the following line
// builder.AddServiceDefaults()

var appSettingsSection = configuration.GetSection(nameof(AppSettings));
var appSettings = appSettingsSection.Get<AppSettings>();

// adds sql server database context
services.AddDatabase(configuration);

// Add services to the container.
services.AddServices();

services.AddHttpContextAccessor();

// routing configuration
services.AddRouting(options => options.LowercaseUrls = true);

// configures the authentication and authorization
services.AddAuthenticationAndAuthorization(appSettings!);

// Creating a feature manager without the use of DI. Injecting IFeatureManager
// via DI is appropriate in consturctor methods. At the startup, it's
// not recommended to call services.BuildServiceProvider and retreive IFeatureManager
// via provider. Instead, the follwing approach is recommended by creating FeatureManager
// with ConfigurationFeatureDefinitionProvider using the existing configuration.
var featureManager = new FeatureManager(new ConfigurationFeatureDefinitionProvider(configuration));

services
    .AddControllers(async options =>
    {
        options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
        options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status200OK));
        options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
        options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status403Forbidden));
        options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
        options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status503ServiceUnavailable));

        // add LogActionFilter if InterceptedLogging feature is enabled.
        if (await featureManager.IsEnabledAsync(Features.InterceptedLogging))
        {
            options.Filters.Add<LogActionFilter>();
        }
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    });

// add default health checks
services.Configure<HealthCheckPublisherOptions>(options => options.Period = TimeSpan.FromSeconds(300));

services.AddHealthChecks();

// adds the Swagger for the Api Documentation
services.AddSwagger();

var app = builder.Build();

// TODO: if you using .NET Aspire and have added the ServiceDefaults project
// uncomment the following line
// app.MapDefaultEndpoints()

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseAzureAppConfiguration();
}

app.UseHttpsRedirection();

// TODO: If you have a specific package or project
// referenced that allows injecting the correlationId in the request
// pipeline, uncomment the following and rename the method if necessary.
// app.UseCorrelationId()

app.UseRouting();

app.UseAuthentication();

// TODO: If you have a specific package or project
// referenced that allows the request tracing like Serilog request tracing
// uncomment the following and rename the method if necessary.
// NOTE: Needs to be after UseAuthentication in the pipeline so it can extract the claims values
// if needed
// app.UseRequestTracing()

app.UseAuthorization();

app.MapControllers();

// run the database migration and seed the data
await app.MigrateAndSeedDatabaseAsync();

await app.RunAsync();