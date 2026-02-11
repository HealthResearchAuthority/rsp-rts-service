using System.ComponentModel.DataAnnotations;

namespace Rsp.RtsService.Domain.Entities;

public class SponsorOrganisation
{
    public Guid Id { get; set; }

    public string RtsId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; } = null!;
    public string? UpdatedBy { get; set; }
}