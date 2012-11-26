using _5Bites.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace _5Bites.Controllers
{
    public class EmployeeController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(EmployeeLoginModel model)
        {
            SHA256 sha256 = new SHA256Managed();
            byte[] hashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(model.Password));

            var connection = new SqlConnection(@"
                Integrated Security = true;
                Data Source = (local)\SqlExpress;
                Initial Catalog = 5Bites;");

            {
                connection.Open();
                var command = new SqlCommand(@"
                    SELECT e.Id FROM Employee e
                    WHERE e.Username = @Username AND e.Password = @Password", connection);
                command.Parameters.AddWithValue("@Username", model.Username);
                command.Parameters.AddWithValue("@Password", hashed);
                Session.Contents["EmployeeId"] = (int?)command.ExecuteScalar();
                connection.Close();
            }

            if (Session.Contents["EmployeeId"] == null)
            {
                return RedirectToAction("Login", "Employee");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
