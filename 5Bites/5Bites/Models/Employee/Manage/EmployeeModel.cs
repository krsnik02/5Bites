using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Employee_.Manage
{
    public class EmployeeModel
    {
        public int Id { get; set; }

        [DataType(DataType.Password)]
        public string Username { get; set; }
    }
}