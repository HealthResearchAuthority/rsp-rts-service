namespace Rsp.RtsService.Domain.Entities;

public class Audit
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Description { get; set; } = null!;
}