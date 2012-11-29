using _5Bites.Models;
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
        [HttpGet]
        public ActionResult Inventory()
        {
            var m = new _5Bites.Models.Store_.Inventory.ViewModel();
            using (var db = new DBContext())
            {
                var ss = db.Stores;
                foreach (var s in ss)
                {
                    var sm = new Models.Store_.Inventory.StoreModel();
                    sm.StoreId = s.Id;
                    sm.StoreName = s.Location.Name;
                    foreach (var i in s.Location.Inventories)
                    {
                        var pm = new Models.Store_.Inventory.ProductModel();
                        pm.Description = i.Product.Name;
                        pm.Price = i.Product.RetailPrice;
                        pm.Quantity = i.Quantity;
                        sm.Inventory.Add(pm);
                    }
                    m.Stores.Add(sm);
                }
            }

            return View(m);
        }

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
