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
        [HttpGet]
        public ActionResult Index()
        {
            int? EmployeeId = (int?)Session.Contents["EmployeeId"];
            if (EmployeeId == null)
                return RedirectToAction("Inventory", "Store");

            var model = new HomeIndexViewModel();
            var connection = new SqlConnection(@"
                Integrated Security = true;
                Data Source = (local)\SQLExpress;
                Initial Catalog = 5Bites;");

            {   /* Employee Name */
                connection.Open();
                var command = new SqlCommand(@"
                    SELECT e.Username FROM Employee e
                    WHERE e.Id = @EmployeeId", connection);
                command.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                model.EmployeeName = command.ExecuteScalar().ToString();
                connection.Close();
            }

            {   /* Administrator Flag */
                connection.Open();
                var command = new SqlCommand(@"
                    SELECT COUNT(*) FROM Admin a
                    WHERE a.EmployeeId = @EmployeeId", connection);
                command.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                model.IsAdmin = (int)command.ExecuteScalar() != 0;
                connection.Close();
            }

            {   /* Ascessible Stores */
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
                    model.Stores.Add(new StoreModel
                    {
                        Id = int.Parse(reader["Id"].ToString()),
                        Name = reader["Name"].ToString()
                    });
                }
                connection.Close();
            }

            {   /* Ascessible Locations */
                connection.Open();
                var command = new SqlCommand(@"
                    SELECT l.Id, l.Name FROM EmployeeLocation el
                    LEFT OUTER JOIN Location l ON l.Id = el.LocationId
                    WHERE el.EmployeeId = @EmployeeId", connection);
                command.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    model.Locations.Add(new LocationModel
                    {
                        Id = int.Parse(reader["Id"].ToString()),
                        Name = reader["Name"].ToString()
                    });
                }
                connection.Close();
            }

            return View(model);
        }
    }
}
