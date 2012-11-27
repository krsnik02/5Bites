using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5Bites.Models
{
    public class EmployeePermissionsViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public List<EmployeePermissionsStoreModel> Stores { get; set; }
        public List<EmployeePermissionsLocationModel> Locations { get; set; }

        public EmployeePermissionsViewModel()
        {
            Stores = new List<EmployeePermissionsStoreModel>();
            Locations = new List<EmployeePermissionsLocationModel>();
        }
    }
}