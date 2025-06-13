namespace Rsp.RtsService.Application.DTOS.Responses;

/// <summary>
/// Response DTO for searching organisations by name.
/// </summary>
public record OrganisationSearchResponse
{
    /// <summary>
    /// Collection of organisations that match the search criteria.
    /// </summary>
    public IEnumerable<SearchOrganisationByNameDto> Organisations { get; set; } = null!;

    /// <summary>
    /// Total number of organisations that match the search criteria.
    /// </summary>
    public int TotalCount { get; set; }
}