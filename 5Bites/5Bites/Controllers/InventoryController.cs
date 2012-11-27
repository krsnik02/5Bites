using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _5Bites.Controllers
{
    public class InventoryController : Controller
    {

        /**
         * GET /Inventory/Transfer
         * Transfer stock between stores
         */
        public ActionResult Transfer()
        {
            return View();
        }

        /**
         * GET /Inventory/Purchase
         * Purchase stock for stores
         */
        public ActionResult Purchase()
        {
            var m = new Models.Inventory.Purchase.ViewModel();

            var con = new SqlConnection(
                @"Integrated Security = true;
                Data Source = (local)\SqlExpress;
                Initial Catalog = 5Bites;");

            {
                con.Open();
                var command = new SqlCommand(
                    @"SELECT l.Id, l.Name FROM EmployeeLocation el
                    LEFT OUTER JOIN Location l ON l.Id = el.LocationId
                    WHERE el.EmployeeId = @EmployeeId", con);
                command.Parameters.AddWithValue("@EmployeeId", (int)Session.Contents["EmployeeId"]);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    m.Locations.Add(new Models.Inventory.Purchase.LocationModel
                    {
                        Id = int.Parse(reader["Id"].ToString()),
                        Name = reader["Name"].ToString()
                    });
                }
                con.Close();
            }

            for (int i = 0; i < m.Locations.Count; ++i)
            {
                con.Open();
                var command = new SqlCommand(
                    @"SELECT p.Id, p.Name, p.WholesalePrice, (SELECT i.Quantity FROM Inventory i WHERE i.ProductId = p.Id AND i.LocationId = @LocationId) AS Quantity FROM Product p", con);
                command.Parameters.AddWithValue("@LocationId", m.Locations[i].Id);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    m.Locations[i].Inventory.Add(new Models.Inventory.Purchase.ProductModel
                    {
                        Id = int.Parse(reader["Id"].ToString()),
                        Name = reader["Name"].ToString(),
                        Price = float.Parse(reader["WholesalePrice"].ToString()),
                        Quantity = int.Parse((reader["Quantity"] ?? 0).ToString())
                    });
                }
                con.Close();
            }

            return View(m);
        }

        /**
         * POST /Inventory/Purchase
         * Purchase supplies for stores
         */
        [HttpPost]
        public ActionResult Purchase(Models.Inventory.Purchase.LocationModel m)
        {
            var con = new SqlConnection(
                @"Integrated Security = true;
                Data Source = (local)\SQLExpress;
                Initial Catalog = 5Bites;");

            foreach (var product in m.Inventory)
            {
                con.Open();
                var command = new SqlCommand(
                    @"UPDATE Inventory SET Quantity = @Quantity
                    WHERE LocationId = @LocationId AND ProductId = @ProductId", con);
                command.Parameters.AddWithValue("@LocationId", m.Id);
                command.Parameters.AddWithValue("@ProductId", product.Id);
                command.Parameters.AddWithValue("@Quantity", product.Quantity);
                command.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Purchase", "Inventory");
        }

    }
}
