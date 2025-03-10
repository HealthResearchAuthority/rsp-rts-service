using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rsp.RtsImport.Application.DTO.Responses;

public class RtsResponseResult
{
    public List<RtsOrganisation> RtsOrganisations { get; set; }
    public object RtsOrganisationSites { get; set; }
    public List<RtsRole> RtsOrganisationRoles { get; set; }
    public object RtsOids { get; set; }
    public List<RtsTermset> RtsTermsets { get; set; }
    public int TotalRecords { get; set; }
    public int PageSize { get; set; }
    public int CurrentPageNumber { get; set; }
    public ResponseDetails Result { get; set; }
}