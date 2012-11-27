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
        /**
         * GET /Home/Index
         * Employee Home Page
         * If there is no logged in user, redirect to /Store/Inventory
         */
        [HttpGet]
        public ActionResult Index()
        {
            int? EmployeeId = (int?)Session.Contents["EmployeeId"];
            if (EmployeeId == null)
                return RedirectToAction("Inventory", "Store");

            var m = new HomeIndexViewModel();
            var con = new SqlConnection(@"
                Integrated Security = true;
                Data Source = (local)\SQLExpress;
                Initial Catalog = 5Bites;");

            { 
                con.Open();
                var command = new SqlCommand(@"
                    SELECT e.Username, e.IsAdmin FROM Employee e
                    WHERE e.Id = @EmployeeId", con);
                command.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                var reader = command.ExecuteReader();
                reader.Read();
                m.EmployeeName = reader["Username"].ToString();
                m.IsAdmin = bool.Parse(reader["IsAdmin"].ToString());
                con.Close();
            }

            return View(m);
        }
    }
}
