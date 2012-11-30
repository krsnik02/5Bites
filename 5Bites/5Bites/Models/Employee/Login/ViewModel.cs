﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Employee_.Login
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

    }
}