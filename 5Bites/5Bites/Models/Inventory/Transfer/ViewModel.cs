using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Inventory_.Transfer
{
    public class ViewModel
    {
        [TransferValidation(ErrorMessage="Transfer cannot create nor destroy inventory")]
        public List<LocationModel> Locations { get; set; }

        public ViewModel()
        {
            Locations = new List<LocationModel>();
        }
    }
}