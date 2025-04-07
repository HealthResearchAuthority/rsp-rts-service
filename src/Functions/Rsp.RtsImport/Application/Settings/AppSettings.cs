namespace Rsp.RtsImport.Application.Settings;

public class AppSettings
{
    public const string ServiceLabel = "rtsimportfunction";
    public string RtsApiClientId { get; set; } = null!;
    public string RtsApiClientSecret { get; set; } = null!;
    public Uri RtsApiBaseUrl { get; set; } = null!;

    public Uri OATRtsAuthApiBaseUrl { get; set; } = null!;
    public AzureAppConfiguration AzureAppConfiguration { get; set; } = null!;
}