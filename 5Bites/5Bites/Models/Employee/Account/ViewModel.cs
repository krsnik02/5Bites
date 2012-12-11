using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Employee_.Account
{
    public class ViewModel
    {
        [Required]
        [Display(Name="Old Password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name="New Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}