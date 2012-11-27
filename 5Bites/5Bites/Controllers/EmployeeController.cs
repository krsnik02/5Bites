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
        /**
         * GET /Employee/Login
         * Employee Login Page
         */
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /**
         * POST /Employee/Login
         * Validate employee login
         */
        [HttpPost]
        public ActionResult Login(EmployeeLoginModel m)
        {
            SHA256 sha256 = new SHA256Managed();
            byte[] hashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(m.Password));

            var con = new SqlConnection(
                @"Integrated Security = true;
                Data Source = (local)\SqlExpress;
                Initial Catalog = 5Bites;");

            {   /* Get Employee Id */
                con.Open();
                var command = new SqlCommand(
                    @"SELECT e.Id, e.IsAdmin FROM Employee e
                    WHERE e.Username = @Username AND e.Password = @Password", con);
                command.Parameters.AddWithValue("@Username", m.Username);
                command.Parameters.AddWithValue("@Password", hashed);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Session.Contents["EmployeeId"] = int.Parse(reader["Id"].ToString());
                    Session.Contents["EmployeeName"] = m.Username;
                    Session.Contents["EmployeeAdmin"] = bool.Parse(reader["IsAdmin"].ToString());
                }
                con.Close();
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

        /**
         * GET /Employee/Logout
         * Log the current employee out
         */
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Contents["EmployeeId"] = null;
            Session.Contents["EmployeeName"] = null;
            Session.Contents["EmployeeAdmin"] = null;
            return RedirectToAction("Index", "Home");
        }

        /**
         * GET /Employee/Manage
         * Hire or Fire employees
         */
        [HttpGet]
        public ActionResult Manage()
        {
            // Enforce admin priviledges
            if (!((bool?)Session.Contents["EmployeeAdmin"] ?? false))
                return RedirectToAction("Index", "Home");

            var m = new EmployeeManageViewModel();

            var con = new SqlConnection(
                @"Integrated Security = true;
                Data Source = (local)\SQLExpress;
                Initial Catalog = 5Bites;");

            {
                con.Open();
                var command = new SqlCommand(
                    @"SELECT e.Id, e.Username FROM Employee e
                    WHERE e.Id <> @EmployeeId", con);
                command.Parameters.AddWithValue("@EmployeeId", (int)Session.Contents["EmployeeId"]);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    m.Employees.Add(new EmployeeManageModel
                    {
                        Id = int.Parse(reader["Id"].ToString()),
                        Username = reader["Username"].ToString()
                    });
                }
            }

            return View(m);
        }

        /**
         * POST /Employee/Hire
         * Hire an employee
         */
        [HttpPost]
        public ActionResult Hire(EmployeeManageHireModel m)
        {
            SHA256 sha256 = new SHA256Managed();
            byte[] hashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(m.Password));

            var con = new SqlConnection(
                @"Integrated Security = true;
                Data Source = (local)\SqlExpress;
                Initial Catalog = 5Bites;");

            {   
                con.Open();
                var command = new SqlCommand(
                    @"INSERT INTO Employee(Username, Password, IsAdmin)
                    VALUES (@Username, @Password, @IsAdmin)", con);
                command.Parameters.AddWithValue("@Username", m.Username);
                command.Parameters.AddWithValue("@Password", hashed);
                command.Parameters.AddWithValue("@IsAdmin", m.IsAdmin);
                command.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Manage", "Employee");
        }

        
        /**
         * POST /Employee/SetAdmin
         * Change admin status of an employee
         */
        /*
        [HttpPost]
        public ActionResult SetAdmin(EmployeeManageEditModel m)
        {
            var con = new SqlConnection(
                @"Integrated Security = true;
                Data Source = (local)\SqlExpress;
                Initial Catalog = 5Bites;");

            {
                con.Open();
                var command = new SqlCommand(
                    @"UPDATE Employee SET IsAdmin = @IsAdmin WHERE Id = @EmployeeId", con);
                command.Parameters.AddWithValue("@EmployeeId", m.Id);
                command.Parameters.AddWithValue("@IsAdmin", m.IsAdmin);
                command.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Manage", "Employee");
        }
        */

        /**
         * POST /Employee/Fire/{EmployeeId}
         * Fire an employee
         */
        [HttpGet]
        public ActionResult Fire(int id)
        {
            // Enforce admin priviledges
            if (!((bool?)Session.Contents["EmployeeAdmin"] ?? false))
                return RedirectToAction("Index", "Home");

            var con = new SqlConnection(
                @"Integrated Security = true;
                Data Source = (local)\SqlExpress;
                Initial Catalog = 5Bites;");

            {
                con.Open();
                var command = new SqlCommand(
                    @"DELETE FROM Employee WHERE Id = @EmployeeId", con);
                command.Parameters.AddWithValue("@EmployeeId", id);
                command.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Manage", "Employee");
        }
    }
}
