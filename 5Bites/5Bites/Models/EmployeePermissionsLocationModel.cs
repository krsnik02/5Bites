﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models
{
    public class EmployeePermissionsLocationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HasAccess { get; set; }
    }
}