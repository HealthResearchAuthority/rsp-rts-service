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
}