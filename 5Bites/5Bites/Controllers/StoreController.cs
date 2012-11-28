using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _5Bites.Controllers
{
    public class StoreController : Controller
    {
        /**
         * GET /Store/Inventory
         * Display the inventory of all stores
         */
        [HttpGet]
        public ActionResult Inventory()
        {
            var m = new _5Bites.Models.Store_.Inventory.ViewModel();

            var con = new SqlConnection(
                @"Integrated Security = true;
                Data Source = (local)\SQLExpress;
                Initial Catalog = 5Bites;");

            {
                con.Open();
                var command = new SqlCommand(
                    @"SELECT s.Id, l.Name FROM Store s
                    LEFT OUTER JOIN Location l ON l.Id = s.LocationId", con);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    m.Stores.Add(new _5Bites.Models.Store_.Inventory.StoreModel
                    {
                        StoreId = int.Parse(reader["Id"].ToString()),
                        StoreName = reader["Name"].ToString()
                    });
                }
                con.Close();
            }

            for (int i = 0; i < m.Stores.Count; ++i)
            {
                con.Open();
                var command = new SqlCommand(
                    @"SELECT p.Name, p.RetailPrice, i.Quantity FROM Store s
                    LEFT OUTER JOIN Inventory i ON i.LocationId = s.LocationId
                    LEFT OUTER JOIN Product p ON p.Id = i.ProductId
                    WHERE s.Id = @StoreId", con);
                command.Parameters.AddWithValue("@StoreId", m.Stores[i].StoreId);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    m.Stores[i].Inventory.Add(new _5Bites.Models.Store_.Inventory.ProductModel
                    {
                        Description = reader["Name"].ToString(),
                        Price = float.Parse(reader["RetailPrice"].ToString()),
                        Quantity = int.Parse(reader["Quantity"].ToString())
                    });
                }
                con.Close();
            }

            return View(m);
        }

        /**
         * GET /Store/Sell
         * Sale page for a logged in employee
         */
        [HttpGet]
        public ActionResult Sell()
        {
            var m = new _5Bites.Models.Store_.Sell.ViewModel();

            var con = new SqlConnection(
                @"Integrated Security = true;
                Data Source = (local)\SQLExpress;
                Initial Catalog = 5Bites;");

            {
                con.Open();
                var command = new SqlCommand(
                    @"SELECT s.Id, l.Name FROM EmployeeStore es
                    LEFT OUTER JOIN Store s ON s.Id = es.StoreId
                    LEFT OUTER JOIN Location l ON l.Id = s.LocationId
                    WHERE es.EmployeeId = @EmployeeId", con);
                command.Parameters.AddWithValue("@EmployeeId", Session.Contents["EmployeeId"]);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    m.Stores.Add(new _5Bites.Models.Store_.Sell.StoreModel
                    {
                        StoreId = int.Parse(reader["Id"].ToString()),
                        StoreName = reader["Name"].ToString()
                    });
                }
                con.Close();
            }

            for (int i = 0; i < m.Stores.Count; ++i)
            {
                con.Open();
                var command = new SqlCommand(
                    @"SELECT p.Id, p.Name, p.RetailPrice, i.Quantity FROM Store s
                    LEFT OUTER JOIN Inventory i ON i.LocationId = s.LocationId
                    LEFT OUTER JOIN Product p ON p.Id = i.ProductId
                    WHERE s.Id = @StoreId", con);
                command.Parameters.AddWithValue("@StoreId", m.Stores[i].StoreId);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    m.Stores[i].Inventory.Add(new _5Bites.Models.Store_.Sell.ProductModel
                    {
                        Id = int.Parse(reader["Id"].ToString()),
                        Description = reader["Name"].ToString(),
                        Price = float.Parse(reader["RetailPrice"].ToString()),
                        Quantity = int.Parse(reader["Quantity"].ToString())
                    });
                }
                con.Close();
            }

            return View(m);
        }

        /**
         * POST /Store/Sell
         * Handle sale action
         */
        [HttpPost]
        public ActionResult Sell(_5Bites.Models.Store_.Sell.StoreModel m)
        {
            var con = new SqlConnection(
                @"Integrated Security = true;
                Data Source = (local)\SQLExpress;
                Initial Catalog = 5Bites;");

            float TotalPrice = 0;
            foreach (var product in m.Inventory)
            {
                if (product.Quantity != 0)
                {
                    TotalPrice += product.Price * product.Quantity;

                    {
                        con.Open();
                        var command = new SqlCommand(@"
                            INSERT INTO [Transaction](ProductId, StoreId, EmployeeId, Quantity, Timestamp)
                            VALUES (@ProductId, @StoreId, @EmployeeId, @Quantity, GETDATE())", con);
                        command.Parameters.AddWithValue("@ProductId", product.Id);
                        command.Parameters.AddWithValue("@StoreId", m.StoreId);
                        command.Parameters.AddWithValue("@EmployeeId", (int)Session.Contents["EmployeeId"]);
                        command.Parameters.AddWithValue("@Quantity", product.Quantity);
                        command.ExecuteNonQuery();
                        con.Close();
                    }

                    {
                        con.Open();
                        var command = new SqlCommand(@"
                            UPDATE Inventory SET Quantity = Quantity - @QuantitySold 
                            WHERE LocationId IN (SELECT LocationId FROM Store WHERE Id = @StoreId) 
                            AND ProductId = @ProductId", con);
                        command.Parameters.AddWithValue("@StoreId", m.StoreId);
                        command.Parameters.AddWithValue("@ProductId", product.Id);
                        command.Parameters.AddWithValue("@QuantitySold", product.Quantity);
                        command.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }

            {
                con.Open();
                var command = new SqlCommand(@"
                    UPDATE Store SET Bank = Bank + @TotalSale WHERE Id = @StoreId", con);
                command.Parameters.AddWithValue("@StoreId", m.StoreId);
                command.Parameters.AddWithValue("@TotalSale", TotalPrice);
                command.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Sell", "Store");
        }

    }
}
