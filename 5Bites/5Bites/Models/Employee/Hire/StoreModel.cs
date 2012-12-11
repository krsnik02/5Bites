﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace _5Bites.Models.Employee_.Hire
{
    public class StoreModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Required]
        public bool HasAccess { get; set; }
    }
}
