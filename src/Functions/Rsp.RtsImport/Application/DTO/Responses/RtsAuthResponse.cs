using System.Text.Json.Serialization;

namespace Rsp.RtsImport.Application.DTO.Responses;

public class RtsAuthResponse
{
    [JsonPropertyName("token_type")] 
    public string TokenType { get; set; } = null!;

    [JsonPropertyName("scope")] 
    public string Scope { get; set; } = null!;

    [JsonPropertyName("access_token")] 
    public string AccessToken { get; set; } = null!;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}