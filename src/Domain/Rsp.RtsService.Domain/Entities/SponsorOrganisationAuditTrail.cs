namespace Rsp.RtsService.Domain.Entities;

public class SponsorOrganisationAuditTrail
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SponsorOrganisationId { get; set; }
    public string RtsId { get; set; } = null!;
    public DateTime DateTimeStamp { get; set; }
    public string Description { get; set; } = null!;
    public string User { get; set; } = null!;
}