using _5Bites.Models.Inventory_.Transfer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _5Bites
{
    public class TransferValidation : ValidationAttribute
    {
        public TransferValidation()
            : base("Transfer can neither create nor destroy inventory!")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<LocationModel> val = (List<LocationModel>)value;
            for (int i = 0; i < val[0].Inventory.Count; ++i)
            {
                int old_ = val.Sum(l => l.Inventory[i].OldQuantity);
                int new_ = val.Sum(l => l.Inventory[i].Quantity);
                if (old_ != new_)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return null;
        }
    }
}