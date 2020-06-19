using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Api.Models
{
    public abstract class PortalMainViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        [RegularExpression("[ -~]+", ErrorMessage = "Please use only latin characters")]
        public string Name { get; set; }

        [Required]
        [Range(10, int.MaxValue)]
        public int CheckInterval { get; set; }

        [Required]
        [Range(10, int.MaxValue)]
        public int ResponseTimeThreshold { get; set; }
    }
}
