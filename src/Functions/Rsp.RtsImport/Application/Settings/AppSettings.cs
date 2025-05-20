using System.Diagnostics.CodeAnalysis;

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
}