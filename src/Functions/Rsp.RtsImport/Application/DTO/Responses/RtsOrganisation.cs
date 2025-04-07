namespace Rsp.RtsImport.Application.DTO.Responses;

public class RtsOrganisation
{
    public string UKCountryIdentifier { get; set; }
    public string UKCountryName { get; set; }

    public string Name { get; set; }
    public string Identifier { get; set; }

    public string Type { get; set; }
    public object ParentOrganisation { get; set; }
    public string Status { get; set; }
    public DateTime EffectiveStartDate { get; set; }
    public object EffectiveEndDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressLine3 { get; set; }
    public string AddressLine4 { get; set; }
    public string AddressLine5 { get; set; }
    public string Postcode { get; set; }

    [JsonIgnore]
    public string Address => $"{AddressLine1} {AddressLine2} {AddressLine3} {AddressLine4} {AddressLine5}".Trim(' ');
}