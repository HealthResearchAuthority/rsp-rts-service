using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;
public class Coding
{
    public IList<Code> coding {get;set;}
    public string text { get; set; }
}
