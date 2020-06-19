using Dashboard.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Api.Models
{
    public class PortalInListViewModel : PortalMainViewModel
    {
        [Required]
        [EnumDataType(typeof(PortalType))]
        public PortalType Type { get; set; }

        [Required]
        [Url]
        public string URL { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
