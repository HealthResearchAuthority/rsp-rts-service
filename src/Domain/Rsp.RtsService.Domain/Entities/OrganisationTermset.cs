using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rsp.RtsService.Domain.Entities;
public class OrganisationTermset
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime SystemUpdated { get; set; }
    public DateTime Imported { get; set; }
}
