namespace Rsp.RtsService.Application.DTOS.Responses;

public record GetOrganisationByIdDto
{
    public string Id { get; set; } = null!;
    public string OId { get; set; } = null!;
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? CountryName { get; set; }
    public string Type { get; set; } = null!;
    public bool? Status { get; set; } = null!;
    public DateTime? LastUpdated { get; set; } = null!;
    public DateTime SystemUpdated { get; set; }
    public DateTime Imported { get; set; }
    public string TypeId { get; set; } = null!;
    public string TypeName { get; set; } = null!;
    public List<OrganisationRoleDto> Roles { get; set; } = [];
}