namespace Rsp.RtsService.Application.DTOS.Responses;

public record SearchOrganisationByNameDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
}