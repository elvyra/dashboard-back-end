using Dashboard.Api.CustomValidationAttributes;
using Dashboard.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Dashboard.Api.Models
{
    public class PortalQueryPostModel
    {
        [Required]
        [EnumDataType(typeof(PortalType))]
        public PortalType Type { get; set; }

        [Required]
        [Url]
        public string URL { get; set; }

        public string CallParameters { get; set; }

        [EnumDataType(typeof(RequestMethod))]
        public RequestMethod Method { get; set; }

        public bool BasicAuth { get; set; }

        [StringLength(50)]
        [RegularExpression("[ -~ąčęėįšųūžĄČĘĖĮŠŲŪŽ]+", ErrorMessage = "Please use only printable English characters")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string PasswordHashed { get; set; }
    }
}
