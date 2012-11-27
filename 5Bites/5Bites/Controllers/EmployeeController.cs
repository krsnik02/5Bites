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
         * GET /Employee/Account
         * Edit your account (username, password)
         */
        [HttpGet]
        public ActionResult Account()
        {
            return View();
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
                    @"INSERT INTO Employee(Username, Password)
                    VALUES (@Username, @Password)", con);
                command.Parameters.AddWithValue("@Username", m.Username);
                command.Parameters.AddWithValue("@Password", hashed);
                command.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Manage", "Employee");
        }

        /**
         * GET /Employee/Permissions/{EmployeeId}
         * Edit employee permissions
         */
        [HttpGet]
        public ActionResult Permissions(int id)
        {
            if (!((bool?)Session.Contents["EmployeeAdmin"] ?? false))
                return RedirectToAction("Index", "Home");

            var m = new EmployeePermissionsViewModel();
            var con = new SqlConnection(
                @"Integrated Security = true;
                Data Source = (local)\SqlExpress;
                Initial Catalog = 5Bites;");

            {
                con.Open();
                var command = new SqlCommand(
                    @"SELECT e.Username, e.IsAdmin FROM Employee e WHERE e.Id = @EmployeeId", con);
                command.Parameters.AddWithValue("@EmployeeId", id);
                var reader = command.ExecuteReader();
                reader.Read();
                m.Id = id;
                m.Username = reader["Username"].ToString();
                m.IsAdmin = bool.Parse(reader["IsAdmin"].ToString());
                con.Close();
            }

            {
                con.Open();
                var command = new SqlCommand(
                    @"SELECT s.Id, l.Name, (SELECT CONVERT(BIT, COUNT(*)) FROM EmployeeStore es
                                            WHERE es.EmployeeId = @EmployeeId
                                            AND es.StoreId = s.Id) AS HasAccess FROM Store s
                    LEFT OUTER JOIN Location l ON l.Id = s.LocationId", con);
                command.Parameters.AddWithValue("@EmployeeId", id);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    m.Stores.Add(new EmployeePermissionsStoreModel
                    {
                        Id = int.Parse(reader["Id"].ToString()),
                        Name = reader["Name"].ToString(),
                        HasAccess = bool.Parse(reader["HasAccess"].ToString())
                    });
                }
                con.Close();
            }

            {
                con.Open();
                var command = new SqlCommand(
                    @"SELECT l.Id, l.Name, (SELECT CONVERT(BIT, COUNT(*)) FROM EmployeeLocation el
                                            WHERE el.EmployeeId = @EmployeeId
                                            AND el.LocationId = l.Id) AS HasAccess FROM Location l", con);
                command.Parameters.AddWithValue("@EmployeeId", id);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    m.Locations.Add(new EmployeePermissionsLocationModel
                    {
                        Id = int.Parse(reader["Id"].ToString()),
                        Name = reader["Name"].ToString(),
                        HasAccess = bool.Parse(reader["HasAccess"].ToString())
                    });
                }
                con.Close();
            }

            return View(m);
        }

        /**
         * POST /Employee/Permissions
         * Update permissions of an employee.
         */
        [HttpPost]
        public ActionResult Permissions(EmployeePermissionsViewModel m)
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

            foreach (var store in m.Stores)
            {
                con.Open();
                SqlCommand command;
                if (store.HasAccess)
                {
                    command = new SqlCommand(
                        @"BEGIN TRANSACTION
                        IF NOT EXISTS (SELECT * FROM EmployeeStore es WHERE es.EmployeeId = @EmployeeId AND es.StoreId = @StoreId)
                        BEGIN
                            INSERT INTO EmployeeStore (EmployeeId, StoreId) VALUES (@EmployeeId, @StoreId)
                        END
                        COMMIT TRANSACTION", con);
                }
                else
                {
                    command = new SqlCommand(
                        @"DELETE FROM EmployeeStore WHERE EmployeeId = @EmployeeId AND StoreId = @StoreId", con);
                }
                command.Parameters.AddWithValue("@EmployeeId", m.Id);
                command.Parameters.AddWithValue("@StoreId", store.Id);
                command.ExecuteNonQuery();
                con.Close();
            }

            foreach (var location in m.Locations)
            {
                con.Open();
                SqlCommand command;
                if (location.HasAccess)
                {
                    command = new SqlCommand(
                        @"BEGIN TRANSACTION
                        IF NOT EXISTS (SELECT * FROM EmployeeLocation WHERE EmployeeId = @EmployeeId AND LocationId = @LocationId)
                        BEGIN
                            INSERT INTO EmployeeLocation (EmployeeId, LocationId) VALUES (@EmployeeId, @LocationId)
                        END
                        COMMIT TRANSACTION", con);
                }
                else
                {
                    command = new SqlCommand(
                        @"DELETE FROM EmployeeLocation WHERE EmployeeId = @EmployeeId AND LocationId = @LocationId", con);
                }
                command.Parameters.AddWithValue("@EmployeeId", m.Id);
                command.Parameters.AddWithValue("@LocationId", location.Id);
                command.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Manage", "Employee");
        }
       

        /**
         * GET /Employee/Fire/{EmployeeId}
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
