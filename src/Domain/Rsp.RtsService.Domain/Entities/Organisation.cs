namespace Rsp.RtsService.Domain.Entities;

public class Organisation
{
    public string Id { get; set; } = null!;
    public string OId { get; set; } = null!;
    public string? Name { get; set; } = null!;
    public string? Address { get; set; } = null!;
    public string? CountryName { get; set; } = null!;
    public string Type { get; set; } = null!;
    public bool? Status { get; set; } = null!;
    public DateTime? LastUpdated { get; set; }
    public DateTime SystemUpdated { get; set; }
    public DateTime Imported { get; set; }

    public string TypeId { get; set; }

    public string TypeName { get; set; }

    public ICollection<OrganisationRole> Roles { get; set; } = [];

}