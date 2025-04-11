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

var builder = FunctionsApplication.CreateBuilder(args);
builder.ConfigureFunctionsWebApplication();

var config = builder.Configuration;

config
    .AddJsonFile("local.settings.json", true)
    .AddJsonFile("featuresettings.json", true, true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

// register dependencies
builder.Services.AddServices();
builder.Services.AddDbContext<RtsDbContext>(options =>
{
    options.EnableSensitiveDataLogging();
    options.UseSqlServer(config.GetConnectionString("RtsDB"));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddHeaderPropagation(options => options.Headers.Add(RequestHeadersKeys.CorrelationId));

if (!builder.Environment.IsDevelopment())
{
    // Load configuration from Azure App Configuration
    builder.Services.AddAzureAppConfiguration(config);
}

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));

// instantiate the appsettings class and assign the values from the config file
var appSettingsSection = config.GetSection(nameof(AppSettings));
var appSettings = appSettingsSection.Get<AppSettings>()!;

builder.Services.AddHttpClients(appSettings!);

// register configurationSettings as singleton
builder.Services.AddSingleton(appSettings!);

// Creating a feature manager without the use of DI. Injecting IFeatureManager
// via DI is appropriate in consturctor methods. At the startup, it's
// not recommended to call services.BuildServiceProvider and retreive IFeatureManager
// via provider. Instead, the follwing approach is recommended by creating FeatureManager
// with ConfigurationFeatureDefinitionProvider using the existing configuration.
var featureManager = new FeatureManager(new ConfigurationFeatureDefinitionProvider(config));

if (await featureManager.IsEnabledAsync(Features.InterceptedLogging))
{
    builder.Services.AddLoggingInterceptor<LoggingInterceptor>();
}

var app = builder.Build();

await app.RunAsync();