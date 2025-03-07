namespace Rsp.RtsImport.Application.DTO.Responses;

public class RtsRole
{
    public string OrgIdentifier { get; set; }
    public string RoleType { get; set; }
    public string Status { get; set; }
    public string ParentIdentifier { get; set; }
    public DateTime EffectiveStartDate { get; set; }
    public DateTime? EffectiveEndDate { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? SystemUpdated { get; set; }
    public DateTime? ModifiedDate { get; set; }
}