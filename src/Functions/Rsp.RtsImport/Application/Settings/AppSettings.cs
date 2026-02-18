using System.Diagnostics.CodeAnalysis;
using Rsp.MalwareScanEvent.Application.Configuration;

namespace Rsp.RtsImport.Application.Settings;

[ExcludeFromCodeCoverage]
public class AppSettings
{
    public const string ServiceLabel = "rtsimportfunction";
    public string RtsApiClientId { get; set; } = null!;
    public string RtsApiClientSecret { get; set; } = null!;
    public Uri RtsApiBaseUrl { get; set; } = null!;
    public Uri RtsAuthApiBaseUrl { get; set; } = null!;
    public AzureAppConfiguration AzureAppConfiguration { get; set; } = null!;
    public MicrosoftEntra MicrosoftEntra { get; set; } = null!;

    /// <summary>
    /// Database command timeout in seconds
    /// </summary>
    public int DatabaseCommandTimeout { get; set; }

    /// <summary>
    /// Timeout for bulk copy operations in seconds
    /// </summary>
    public int BulkCopyTimeout { get; set; }

    /// <summary>
    /// Number of records to request from the RTS API
    /// </summary>
    public int ApiRequestPageSize { get; set; }

    /// <summary>
    /// Maximum number of RTS API requests that can run in parallel
    /// </summary>
    public int ApiRequestMaxConcurrency { get; set; }

    /// <summary>
    /// Gets or sets the URI of the ApplicationsService microservice.
    /// </summary>
    public Uri ApplicationsServiceUri { get; set; } = null!;

    /// <summary>
    /// Gets or sets Managed Identity Client ID to enabling the framework to fetch a token for
    /// accessing Applications Service.
    /// </summary>
    public string ManagedIdentityRtsClientID { get; set; } = null!;
}