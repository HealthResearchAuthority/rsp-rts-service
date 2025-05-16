namespace Rsp.RtsService.Domain.Entities;

public class OrganisationRole
{
    public string Id { get; set; } = null!;
    public string OrganisationId { get; set; } = null!;
    public int Scoper { get; set; } = -1!;
    public string Status { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime SystemUpdated { get; set; }
    public DateTime Imported { get; set; }
}