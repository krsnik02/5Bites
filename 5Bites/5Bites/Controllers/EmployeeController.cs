﻿using _5Bites.Models;
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
        public ActionResult Login(Models.Employee_.Login.ViewModel m)
        {
            SHA256 sha256 = new SHA256Managed();
            byte[] hashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(m.Password));

            using (var db = new DBEntities())
            {
                var employee = db.Employees.SingleOrDefault(
                    e => e.Username == m.Username && e.Password == hashed);
                Session.Contents["EmployeeId"] = employee.Id;
                Session.Contents["EmployeeName"] = employee.Username;
                Session.Contents["EmployeeAdmin"] = employee.IsAdmin;
            }

            if (Session.Contents["EmployeeId"] == null)
                return RedirectToAction("Login", "Employee");
            else
                return RedirectToAction("Index", "Home");
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

            int EmployeeId = (int)Session.Contents["EmployeeId"];
            var m = new Models.Employee_.Manage.ViewModel();

            using (var db = new DBEntities())
            {
                var employees = db.Employees.Where(e => e.Id != EmployeeId);
                var stores = db.Stores;
                var locations = db.Locations;

                foreach (var employee in employees)
                {
                    m.Employees.Add(new Models.Employee_.Manage.EmployeeModel
                    {
                        Id = employee.Id,
                        Username = employee.Username
                    });
                }

                foreach (var store in stores)
                {
                    m.Hire.Stores.Add(new Models.Employee_.Hire.StoreModel
                    {
                        Id = store.Id,
                        Name = store.Location.Name
                    });
                }

                foreach (var location in locations)
                {
                    m.Hire.Locations.Add(new Models.Employee_.Hire.LocationModel
                    {
                        Id = location.Id,
                        Name = location.Name
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
        public ActionResult Hire(_5Bites.Models.Employee_.Hire.ViewModel m)
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

            int EmployeeId;
            {
                con.Open();
                var command = new SqlCommand(
                    @"SELECT Id FROM Employee WHERE Username = @Username", con);
                command.Parameters.AddWithValue("@Username", m.Username);
                EmployeeId = int.Parse(command.ExecuteScalar().ToString());
                con.Close();
            }

            foreach (var store in m.Stores)
            {
                if (store.HasAccess)
                {
                    con.Open();
                    var command = new SqlCommand(
                        @"INSERT INTO EmployeeStore(EmployeeId, StoreId) VALUES (@EmployeeId, @StoreId)", con);
                    command.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    command.Parameters.AddWithValue("@StoreId", store.Id);
                    command.ExecuteNonQuery();
                    con.Close();
                }
            }

            foreach (var location in m.Locations)
            {
                if (location.HasAccess)
                {
                    con.Open();
                    var command = new SqlCommand(
                        @"INSERT INTO EMployeeLocation(EmployeeId, LocationId) VALUES (@EmployeeId, @LocationId)", con);
                    command.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    command.Parameters.AddWithValue("@LocationId", location.Id);
                    command.ExecuteNonQuery();
                    con.Close();
                }
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

            var m = new _5Bites.Models.Employee_.Permissions.ViewModel();
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
                    m.Stores.Add(new _5Bites.Models.Employee_.Permissions.StoreModel
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
                    m.Locations.Add(new _5Bites.Models.Employee_.Permissions.LocationModel
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
        public ActionResult Permissions(_5Bites.Models.Employee_.Permissions.ViewModel m)
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
    }
}
