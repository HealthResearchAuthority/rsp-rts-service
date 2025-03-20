using System.Text.Json.Serialization;

namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class Resource
{
    public string resourceType { get; set; }
    public string id { get; set; }
    public Meta meta { get; set; }

    //[JsonPropertyName("extension")]
    public IList<dynamic> extension { get; set; }

    public IList<Identifier> identifier { get; set; }
    public bool active { get; set; }

    //[JsonPropertyName("type")]
    public IList<Coding> type { get; set; }

    public string name { get; set; }
    public IList<Address> address { get; set; }
}