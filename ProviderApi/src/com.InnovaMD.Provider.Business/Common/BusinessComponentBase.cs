using com.InnovaMD.Provider.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace com.InnovaMD.Provider.Business.Common
{
    public abstract class BusinessComponentBase
    {
        protected BusinessComponentBase() { }

        protected virtual ValidationResultModel ValidateModel(object entity, object propertiesToIgnore = null)
        {
            if (entity == null)
            {
                return new ValidationResultModel
                {
                    IsValid = false,
                    ValidationResults = new List<ValidationResult>()
                    {
                        new ValidationResult("The request model cannot be null")
                    }
                };
            }

            var results = new List<ValidationResult>();
            var vc = new ValidationContext(entity);
            bool isValid = true;

            if (propertiesToIgnore == null)
            {
                isValid = Validator.TryValidateObject(entity, vc, results, true);
            }
            else
            {
                Type type = entity.GetType();
                Type propToIgnoreType = propertiesToIgnore.GetType();
                string[] propertyToIgnoreNames = (from p in propToIgnoreType.GetProperties() select p.Name).ToArray();

                var properties = type.GetProperties();
                foreach (PropertyInfo prop in properties)
                {
                    if (!propertyToIgnoreNames.Contains(prop.Name))
                    {
                        IEnumerable<ValidationAttribute> valAtrs = prop.GetCustomAttributes<ValidationAttribute>();
                        object value = prop.GetGetMethod().Invoke(entity, null);
                        isValid &= Validator.TryValidateValue(value, vc, results, valAtrs);
                    }
                }
            }
            return new ValidationResultModel
            {
                IsValid = isValid,
                ValidationResults = results
            };
        }
    }
}
