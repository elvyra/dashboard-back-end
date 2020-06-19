using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dashboard.Models;
using Dashboard.Models.Enums;

namespace Dashboard.Api.Models
{
    public class QueryPortalViewModel : PortalMainViewModel
    {
        public int? ResponseTime { get; set; }               // Server response time
        public DateTime? LastFailure { get; set; }           // Last failure from server time
        public int? Status { get; set; }                     // Status which was get from server (200, 404 etc)
        public PortalType Type { get; set; }
    }
}
