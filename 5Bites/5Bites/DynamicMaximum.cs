using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _5Bites
{
    public class DynamicMaximum : ValidationAttribute
    {
        private String _maxPropertyName;
        public DynamicMaximum(string maxPropertyName)
            : base("'{0}' must be less than '{1}'")
        {
            _maxPropertyName = maxPropertyName;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _maxPropertyName);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var maxPropInfo = validationContext.ObjectType.GetProperty(_maxPropertyName);
            var max = (int)maxPropInfo.GetValue(validationContext.ObjectInstance, null);
            var val = (int)value;
            if (val > max)
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }
            return null;
        }
    }
}