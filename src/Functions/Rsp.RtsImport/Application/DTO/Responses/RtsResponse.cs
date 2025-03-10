namespace Rsp.RtsImport.Application.DTO.Responses;

public class RtsResponse
{
    public string Version { get; set; }
    public int StatusCode { get; set; }
    public object ErrorMessage { get; set; }
    public RtsResponseResult Result { get; set; }
}