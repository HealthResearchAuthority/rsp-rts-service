using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rsp.RtsImport.Application.DTO.Responses;

public class ResponseDetails
{
    public bool IsSuccess { get; set; }
    public bool IsFailed { get; set; }
    public object Errors { get; set; }
    public object DetailedErrors { get; set; }
    public object DetailedErrors_V2 { get; set; }
    public object Entity { get; set; }
    public bool isStale { get; set; }
}