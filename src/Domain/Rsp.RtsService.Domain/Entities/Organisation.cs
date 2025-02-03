namespace Rsp.RtsService.Domain.Entities;

public class Organisation
{
    public string Id { get; set; } = null!;
    public string? Name { get; set; } = null!;
    public string? Address { get; set; } = null!;
    public string? CountryName { get; set; } = null!;
    public string? CountryIdentifier { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string? Status { get; set; } = null!;
    public DateTime? LastUpdated { get; set; }
    public DateTime SystemUpdated { get; set; }
    public DateTime Imported { get; set; }

    // Navigation properties
    public ICollection<OrganisationRole> Roles { get; set; } = [];
    public OrganisationTermset TypeEntity { get; set; }
}