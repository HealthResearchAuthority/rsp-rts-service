namespace Rsp.RtsImport.Application.DTO.Responses;

public class RtsTermset
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null;
    public string Identifier { get; set; } = null!;
    public string? ParentIdentifier { get; set; } = null;
    public string? ParentOid { get; set; } = null;
    public string Status { get; set; } = null!;
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public DateTime? EffectiveStartDate { get; set; }
    public DateTime? EffectiveEndDate { get; set; }
    public DateTime SystemUpdated { get; set; }
    public DateTime Imported { get; set; }
}