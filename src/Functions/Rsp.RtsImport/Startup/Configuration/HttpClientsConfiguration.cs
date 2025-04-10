namespace Rsp.RtsImport.Startup.Configuration;

[ExcludeFromCodeCoverage]
public static class HttpClientsConfiguration
{
    /// <summary>
    ///     Adds the Orchestration service http clients
    /// </summary>
    /// <param name="services">Specifies the contract for a collection of service descriptors</param>
    /// <param name="appSettings">Application settings from appsettings.json</param>
    public static IServiceCollection AddHttpClients(this IServiceCollection services, AppSettings appSettings)
    {
        services
            .AddRestClient<IRtsServiceClient>()
            .ConfigureHttpClient(client => client.BaseAddress = appSettings.RtsApiBaseUrl)
            .AddHttpMessageHandler<RtsAuthHeadersHandler>();

        services
            .AddRestClient<IRtsAuthorisationServiceClient>()
            .ConfigureHttpClient(client => client.BaseAddress = appSettings.OATRtsAuthApiBaseUrl);

        return services;
    }

    /// <summary>
    ///     Adds the rest client.
    /// </summary>
    /// <typeparam name="T">Interface to register as a Refit client</typeparam>
    /// <param name="services">Specifies the contract for a collection of service descriptors</param>
    public static IHttpClientBuilder AddRestClient<T>(this IServiceCollection services) where T : class
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        var refitSettings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(options),
            // Buffering enabled while we wait for this fix: https://github.com/reactiveui/refit/issues/1099
            // Otherwise the "Content-Length" won't be set and downstream requests with a body will fail
            Buffered = true
        };

        // add the http client, with retries, to the Ioc to call the down stream API
        return services
            .AddHttpClient(typeof(T).Name) // this name will be used as a SourceContext when logging request/response
            .AddTypedClient(client => RestService.For<T>(client, refitSettings));
    }
}