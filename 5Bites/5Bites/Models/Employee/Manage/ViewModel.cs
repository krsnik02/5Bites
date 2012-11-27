using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models.Employee.Manage
{
    public class ViewModel
    {
        public List<EmployeeModel> Employees { get; set; }

        public ViewModel()
        {
            Employees = new List<EmployeeModel>();
        }
    }
}