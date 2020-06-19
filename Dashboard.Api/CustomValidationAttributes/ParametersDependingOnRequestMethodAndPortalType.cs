using Dashboard.Api.Models;
using Dashboard.Models.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Dashboard.Api.CustomValidationAttributes
{
    public class ParametersDependingOnRequestMethodAndPortalType : ValidationAttribute
    {      
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (PortalFullViewModel)validationContext.ObjectInstance;    
            
            // POST method parameters validation for RESTful API

            if (model.Method == RequestMethod.POST && model.Type == PortalType.ServiceREST)
            {
                try
                {
                    var json = JObject.Parse(model.CallParameters);
                    return ValidationResult.Success;
                }
                catch (Exception ex)
                {
                    return new ValidationResult("Not a valid JSON, parsing failed: " + ex.Message);
                }
            }

            // POST method parameters validation for SOAP API

            if (model.Method == RequestMethod.POST && model.Type == PortalType.ServiceSOAP)
            {
                try
                {
                    var doc =  XDocument.Parse(model.CallParameters);
                    return ValidationResult.Success;
                }
                catch (Exception ex)
                {
                    return new ValidationResult("Not a valid XML, parsing failed: " + ex.Message);
                }
            }

            // GET method validation for portal and Webapp

            if (model.Method == RequestMethod.GET && model.Type == PortalType.WebApp) 
            {
                if (model.CallParameters == null)
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult("Call parameters must be null for WebApp method GET");
            }

            // GET method validation for portal and RESTful

            if (model.Method == RequestMethod.GET && model.Type == PortalType.ServiceREST)
            {
                try
                {
                    var json = JObject.Parse(model.CallParameters);
                    return new ValidationResult("JSON format can not be provided for request method GET");
                }
                catch (Exception)
                {
                } 
                
                try
                {
                    var doc = XDocument.Parse(model.CallParameters);
                    return new ValidationResult("XML format can not be provided for request method GET");
                }
                catch (Exception)
                {
                }
                
                try
                {
                    var query = HttpUtility.ParseQueryString(model.CallParameters);
                    return ValidationResult.Success;
                }
                catch (Exception ex)
                {
                    return new ValidationResult("Not a valid query string, parsing failed: " + ex.Message);
                }
            }

            return new ValidationResult("Validation failed with unknown error");
        }
    }
}
