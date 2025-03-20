namespace Rsp.RtsImport.Application.Settings;

public class AppSettings
{
    public const string ServiceLabel = "rtsimportfunction";
    public string? RtsApiId { get; set; } = null!;
    public string? RtsApiSecret { get; set; } = null!;
    public string? AzureVaultUri { get; set; } = null!;
    public string? RtsAuthUrl { get; set; } = null!;
    public string? RtsOrgDataUrl { get; set; } = null!;
    public string? RtsRolesDataUrl { get; set; } = null!;
    public string? RtsTermsetDataUrl { get; set; } = null!;
    public Uri RtsApiBaseUrl { get; set; } = null!;

    public Uri OATRtsAuthApiBaseUrl { get; set; } = null!;
    public AzureAppConfiguration AzureAppConfiguration { get; set; } = null!;
}