namespace Rsp.RtsService.Application.Settings;

public class AppSettings
{
    /// <summary>
    /// Label to use when reading App Configuration from AzureAppConfiguration
    /// </summary>
    public const string ServiceLabel = "rtsservice";
    /// <summary>
    /// Gets or sets authentication settings
    /// </summary>
    public AuthSettings AuthSettings { get; set; } = null!;

    /// <summary>
    /// Settings for Azure AppConfiguration
    /// </summary>
    public AzureAppConfigurations AzureAppConfiguration { get; set; } = null!;
}