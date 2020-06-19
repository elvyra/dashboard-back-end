using Dashboard.Api.Models;
using Dashboard.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Api.CustomValidationAttributes
{
    public class BasicAuthRequired : ValidationAttribute
    {
        public string GetErrorMessage() =>
            "Authorization is not awailable for this type of portal";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (PortalFullViewModel)validationContext.ObjectInstance;

            if (model.Type == PortalType.WebApp && model.BasicAuth)
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }
    }
}
