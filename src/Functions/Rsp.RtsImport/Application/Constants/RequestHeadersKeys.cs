using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rsp.RtsImport.Application.Constants;

public struct RequestHeadersKeys
{
    /// <summary>
    /// Header name for unique identifier assigned to a particular request
    /// </summary>
    public const string CorrelationId = "x-correlation-id";
}