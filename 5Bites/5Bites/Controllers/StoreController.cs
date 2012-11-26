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
        [HttpPost]
        public ActionResult Transfer(HomeIndexViewModel model)
        {
            return RedirectToAction("Transfer", "Store", new { toId = model.PostTransfer.ToId, fromId = model.PostTransfer.FromId });
        }

        [HttpGet]
        public ActionResult Transfer(int toId, int fromId)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Sell(int id)
        {
            var model = new StoreSellViewModel();
            var connection = new SqlConnection(@"
                Integrated Security = true;
                Data Source = (local)\SQLExpress;
                Initial Catalog = 5Bites;");

            {
                connection.Open();
                var command = new SqlCommand(@"
                    SELECT l.Name FROM Store s
                    LEFT OUTER JOIN Location l ON l.Id = s.LocationId
                    WHERE s.Id = @StoreId", connection);
                command.Parameters.AddWithValue("@StoreId", id);
                model.StoreName = (string)command.ExecuteScalar();
                connection.Close();
            }

            {
                connection.Open();
                var command = new SqlCommand(@"
                    SELECT p.Id, p.Name, p.RetailPrice, i.Quantity FROM Store s
                    LEFT OUTER JOIN Inventory i ON i.LocationId = s.LocationId
                    LEFT OUTER JOIN Product p ON p.Id = i.ProductId
                    WHERE s.Id = @StoreId", connection);
                command.Parameters.AddWithValue("@StoreId", id);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    model.Inventory.Add(new ProductSaleModel
                    {
                        Id = int.Parse(reader["Id"].ToString()),
                        Name = reader["Name"].ToString(),
                        Price = float.Parse(reader["RetailPrice"].ToString()),
                        Quantity = int.Parse(reader["Quantity"].ToString())
                    });
                }
                connection.Close();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Sell(int id, StoreSellViewModel model)
        {
            int EmployeeId = (int)Session.Contents["EmployeeId"];

            var connection = new SqlConnection(@"
                Integrated Security = true;
                Data Source = (local)\SQLExpress;
                Initial Catalog = 5Bites;");

            float TotalPrice = 0;
            foreach (var product in model.Inventory)
            {
                if (product.Quantity != 0)
                {
                    TotalPrice += product.Price * product.Quantity;

                    {
                        connection.Open();
                        var command = new SqlCommand(@"
                        INSERT INTO [Transaction](ProductId, StoreId, EmployeeId, Quantity, Timestamp)
                        VALUES (@ProductId, @StoreId, @EmployeeId, @Quantity, GETDATE())", connection);
                        command.Parameters.AddWithValue("@ProductId", product.Id);
                        command.Parameters.AddWithValue("@StoreId", id);
                        command.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        command.Parameters.AddWithValue("@Quantity", product.Quantity);
                        command.ExecuteNonQuery();
                        connection.Close();
                    }

                    {
                        connection.Open();
                        var command = new SqlCommand(@"
                        UPDATE Inventory SET Quantity = Quantity - @QuantitySold WHERE LocationId IN (SELECT LocationId FROM Store WHERE Id = @StoreId) AND ProductId = @ProductId", connection);
                        command.Parameters.AddWithValue("@StoreId", id);
                        command.Parameters.AddWithValue("@ProductId", product.Id);
                        command.Parameters.AddWithValue("@QuantitySold", product.Quantity);
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }

            {
                connection.Open();
                var command = new SqlCommand(@"
                    UPDATE Store SET Bank = Bank + @TotalSale WHERE Id = @StoreId", connection);
                command.Parameters.AddWithValue("@StoreId", id);
                command.Parameters.AddWithValue("@TotalSale", TotalPrice);
                command.ExecuteNonQuery();
                connection.Close();
            }

            return RedirectToAction("Sell", "Store", new { id = id });
        }
    }
}
