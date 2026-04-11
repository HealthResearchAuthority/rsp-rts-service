namespace Rsp.RtsService.Application.DTOS.Responses;

/// <summary>
/// Response DTO for searching organisations by name.
/// </summary>
public record OrganisationsSearchRequest
{
    public string? SearchNameTerm { get; set; }
    public List<string> ExcludingRoles { get; set; } = [];
    public List<string> Countries { get; set; } = [];
    public List<string> OrganisationTypes { get; set; } = [];
    public List<bool> OrganisationStatuses { get; set; } = [];
    public List<string> ExcludedOrganisationIds { get; set; } = [];
}