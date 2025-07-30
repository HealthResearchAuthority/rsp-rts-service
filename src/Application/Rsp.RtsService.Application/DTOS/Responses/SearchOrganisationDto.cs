namespace Rsp.RtsService.Application.DTOS.Responses;

public record SearchOrganisationDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? CountryName { get; set; }
    public string Type { get; set; } = null!;
}