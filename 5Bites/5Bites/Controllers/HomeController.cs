using _5Bites.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _5Bites.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            int? EmployeeId = (int?)Session.Contents["EmployeeId"];
            if (EmployeeId == null)
                return RedirectToAction("Public", "Home");

            var connection = new SqlConnection(@"
                Integrated Security = true;
                Data Source = (local)\SQLExpress;
                Initial Catalog = 5Bites;");

            {
                connection.Open();
                var command = new SqlCommand(@"
                    SELECT e.Username FROM Employee e
                    WHERE e.Id = @EmployeeId", connection);
                command.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                ViewBag.EmployeeName = command.ExecuteScalar().ToString();
                connection.Close();
            }

            {
                ViewBag.Stores = new List<StoreModel>();
                connection.Open();
                var command = new SqlCommand(@"
                    SELECT s.Id, l.Name FROM EmployeeStore es
                    LEFT OUTER JOIN Store s ON s.Id = es.StoreId
                    LEFT OUTER JOIN Location l ON l.Id = s.LocationId
                    WHERE es.EmployeeId = @EmployeeId", connection);
                command.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ViewBag.Stores.Add(new StoreModel
                    {
                        Id = int.Parse(reader["Id"].ToString()),
                        Name = reader["Name"].ToString()
                    });
                }
                connection.Close();
            }

            return View();
        }

        public ActionResult Public()
        {
            var connection = new SqlConnection(@"
                Integrated Security = true;
                Data Source = (local)\SQLExpress;
                Initial Catalog = 5Bites;");

            {
                ViewBag.Stores = new List<StoreModel>();
                connection.Open();
                var command = new SqlCommand(@"
                    SELECT s.Id, l.Name FROM Store s
                    LEFT OUTER JOIN Location l ON l.Id = s.LocationId", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ViewBag.Stores.Add(new StoreModel
                    {
                        Id = int.Parse(reader["Id"].ToString()),
                        Name = reader["Name"].ToString()
                    });
                }
                connection.Close();
            }

            return View();
        }
    }
}
