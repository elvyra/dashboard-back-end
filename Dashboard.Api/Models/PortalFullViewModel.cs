using Dashboard.Api.CustomValidationAttributes;
using Dashboard.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dashboard.Api.Models
{
    public class PortalFullViewModel : PortalInListViewModel
    {
        [ParametersDependingOnRequestMethodAndPortalType]
        public string CallParameters { get; set; }

        [EnumDataType(typeof(RequestMethod))]
        [RequestMethodDependingOnPortalType]
        public RequestMethod Method { get; set; }
                
        [BasicAuthRequired]
        public bool BasicAuth { get; set; }

        [StringLength(50)]
        [RegularExpression("[ -~ąčęėįšųūžĄČĘĖĮŠŲŪŽ]+", ErrorMessage = "Please use only printable English characters")]
        [UserNameRequired]
        public string UserName { get; set; }

      //  [JsonIgnore]
        [PasswordRequired]
        [DataType(DataType.Password)]      
        public string PasswordHashed { get; set; }
    }
}
