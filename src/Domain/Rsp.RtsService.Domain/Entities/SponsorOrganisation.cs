namespace Rsp.RtsService.Domain.Entities;

public class SponsorOrganisation
{
    public Guid Id { get; set; }
    public string RtsId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? UpdatedDate { get; set; }
}