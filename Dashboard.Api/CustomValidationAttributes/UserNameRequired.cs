using Dashboard.Api.Models;
using Dashboard.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Api.CustomValidationAttributes
{
    public class UserNameRequired : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (PortalFullViewModel)validationContext.ObjectInstance;

            if (!model.BasicAuth && !string.IsNullOrWhiteSpace(model.UserName))
                return new ValidationResult("User name field must be empty, if basic authorization is set to FALSE");
        
            if (model.BasicAuth && string.IsNullOrWhiteSpace(model.UserName))
                return new ValidationResult("Username is required, if basic authorization is set to TRUE");

            if (model.BasicAuth && string.IsNullOrWhiteSpace(model.UserName))
                return new ValidationResult("Username is empty or whitespace");

            if (model.BasicAuth && model.UserName.Length < 3)
                return new ValidationResult("Username must be at least 3 characters long");
     
            return ValidationResult.Success;
        }
    }
}
