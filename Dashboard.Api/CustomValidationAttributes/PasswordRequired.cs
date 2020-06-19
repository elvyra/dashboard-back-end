using Dashboard.Api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Api.CustomValidationAttributes
{
    public class PasswordRequired : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (PortalFullViewModel)validationContext.ObjectInstance;

            if (!model.BasicAuth && !string.IsNullOrWhiteSpace(model.PasswordHashed))
                return new ValidationResult("Password field must be empty, if basic authorization is set to FALSE");

            if (model.BasicAuth && !string.IsNullOrWhiteSpace(model.UserName) && string.IsNullOrEmpty(model.PasswordHashed))
                return ValidationResult.Success;
       
            if (model.BasicAuth && !string.IsNullOrEmpty(model.PasswordHashed) && model.PasswordHashed.Length < 6)
                return new ValidationResult("Password must be at least 6 characters long");

            return ValidationResult.Success;
        }
    }
}
