using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models
{
    public class EmployeeManageViewModel
    {
        public List<EmployeeManageModel> Employees { get; set; }

        public EmployeeManageViewModel()
        {
            Employees = new List<EmployeeManageModel>();
        }
    }
}