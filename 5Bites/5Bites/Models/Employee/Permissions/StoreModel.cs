﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Employee_.Permissions
{
    public class StoreModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HasAccess { get; set; }
    }
}