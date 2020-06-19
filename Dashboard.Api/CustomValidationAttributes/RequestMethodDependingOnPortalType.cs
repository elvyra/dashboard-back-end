using Dashboard.Api.Models;
using Dashboard.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Api.CustomValidationAttributes
{
    public class RequestMethodDependingOnPortalType : ValidationAttribute
    {
        public string GetErrorMessage() =>
            "Request method not supported with this portal type";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (PortalFullViewModel)validationContext.ObjectInstance;

            if (model.Type == PortalType.WebApp && model.Method == RequestMethod.POST || model.Type == PortalType.ServiceSOAP && model.Method == RequestMethod.GET)
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }
    }
}
