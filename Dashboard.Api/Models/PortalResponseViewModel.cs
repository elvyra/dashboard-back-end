using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Api.Models
{
    public class PortalResponseViewModel
    {
        public int ResponseTime { get; set; }
        public DateTime RequestDateTime { get; set; }
        public DateTime? LastFailure { get; set; }
        public int Status { get; set; }
    }
}
