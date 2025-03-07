namespace Rsp.RtsImport.Application.DTO;

public class DbOperationResult
{
    public int RecordsUpdated { get; set; }
    public int RecordsAdded { get; set; }
    public bool Complete { get; set; }
    public string ErrorMessage { get; set; } = null!;
}