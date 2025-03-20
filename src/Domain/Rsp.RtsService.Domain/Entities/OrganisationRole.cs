namespace Rsp.RtsService.Domain.Entities;

public class OrganisationRole
{
    public string Id { get; set; } = null!;
    public string OrganisationId { get; set; } = null!;
    public int Scoper { get; set; } = -1!; // Is this supposed to be non-nullable
    public string Status { get; set; } = null!;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
    public DateTime? LastUpdated { get; set; }
    public DateTime SystemUpdated { get; set; }
    public DateTime Imported { get; set; }
    public DateTime CreatedDate { get; set; }
}