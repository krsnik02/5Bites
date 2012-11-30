using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Employee_.Hire
{
    public class ViewModel
    {
        [Required]
        [Display(Name="Username")]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [Display(Name="Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name="Is Admin")]
        public bool IsAdmin { get; set; }

        public List<StoreModel> Stores { get; set; }
        public List<LocationModel> Locations { get; set; }

        public ViewModel()
        {
            Stores = new List<StoreModel>();
            Locations = new List<LocationModel>();
        }
    }
}