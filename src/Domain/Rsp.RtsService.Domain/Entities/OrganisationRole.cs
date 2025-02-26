namespace Rsp.RtsService.Domain.Entities;

public class OrganisationRole
{
    public string Id { get; set; } = null!;
    public string OrganisationId { get; set; } = null!;
    public string Scoper { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? LastUpdated { get; set; }
    public DateTime SystemUpdated { get; set; }
    public DateTime Imported { get; set; }
    public DateTime CreatedDate { get; set; }

    // Navigation properties
    //public Organisation Organisation { get; set; } = null!;
}