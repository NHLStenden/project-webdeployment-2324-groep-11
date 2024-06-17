using DieselBrandstofCafe.Components.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace DieselBrandstofCafe.Components.Data
{
    // Interface definition for EmployeeService to handle various operations related to orders and products
    public interface IEmployeeService
    {
        // Retrieve a list of new orders with every status
        Task<IEnumerable<Bestelling>> GetOrdersByStatusAsync(string status);


        // Retrieve a list of completed or deleted orders (order history)
        Task<IEnumerable<Bestelling>> GetOrderHistoryAsync();

        // Update the status of a specific order
        Task UpdateOrderStatusAsync(int orderId, string status);

        // Mark a task (product) as completed and update the status of the associated order
        Task CompleteTaskAndUpdateOrderStatusAsync(int orderId, int productId);

        // Retrieve a list of products associated with a specific order
        Task<IEnumerable<ProductPerBestelronde>> GetProductsForOrderAsync(int bestellingId);

        // Mark a task (product) as completed and update its status
        Task CompleteTaskAndUpdateProductStatusAsync(int orderId, int productId, string status);

        // Update the status of a specific product in an order
        Task UpdateProductStatusAsync(int productId, int orderId, string status);

        // Check if all products in a specific order are completed
        Task<bool> AreAllProductsCompletedAsync(int bestellingId);



    }

    // Implementation of the IEmployeeService interface
    public class EmployeeService : IEmployeeService
    {
        // Connection string to the MySQL database
        private readonly string _connectionString;

        // Constructor to initialize the connection string from configuration settings
        public EmployeeService(IConfiguration configuration)
        {
            _connectionString = configuration?.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");
        }

        // Retrieve a list of new orders with every status
        public async Task<IEnumerable<Bestelling>> GetOrdersByStatusAsync(string status)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Bestelling WHERE StatusBestelling = @Status AND TijdBestelling >= NOW() - INTERVAL 1 DAY";
                return await connection.QueryAsync<Bestelling>(sql, new { Status = status });
            }
        }

        // Retrieve a list of completed or deleted orders (order history)
        public async Task<IEnumerable<Bestelling>> GetOrderHistoryAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Bestelling WHERE StatusBestelling = 'Completed' OR StatusBestelling = 'Cancelled' OR StatusBestelling = 'Served'";
                return await connection.QueryAsync<Bestelling>(sql);
            }
        }

        // Retrieve a list of products associated with a specific order
        public async Task<IEnumerable<ProductPerBestelronde>> GetProductsForOrderAsync(int bestellingId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                /*
                    The GetProductsForOrderAsync method retrieves products linked to a specific order.
                    It joins the Product_per_Bestelronde table with the Product, Bestelronde, and Bestelling tables
                    to get the necessary details including product names and their statuses.
                */
                var sql = @"
                SELECT ppb.ProductID, ppb.BestelrondeID, ppb.AantalProduct, ppb.AantalBetaald, ppb.StatusBesteldeProduct, ppb.VerkoopDatumProduct, p.ProductNaam
                FROM Product_per_Bestelronde ppb
                INNER JOIN Product p ON ppb.ProductID = p.ProductID
                INNER JOIN Bestelronde br ON ppb.BestelrondeID = br.BestelrondeID
                INNER JOIN Bestelling b ON br.BestelrondeID = b.BestelrondeID
                WHERE b.BestellingID = @BestellingID";

                var result = await connection.QueryAsync<ProductPerBestelronde>(sql, new { BestellingID = bestellingId });
                Console.WriteLine($"Retrieved {result.Count()} products for order {bestellingId}"); // Log the result count
                return result;
            }
        }

        // Update the status of a specific order
        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "UPDATE Bestelling SET StatusBestelling = @Status WHERE BestellingID = @BestellingID";
                await connection.ExecuteAsync(sql, new { Status = status, BestellingID = orderId });
            }
        }

        // Update the status of a specific product in an order
        public async Task UpdateProductStatusAsync(int productId, int bestelrondeId, string status)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                UPDATE Product_per_Bestelronde
                SET StatusBesteldeProduct = @Status
                WHERE ProductID = @ProductID AND BestelrondeID = @BestelrondeID";
                await connection.ExecuteAsync(sql, new { Status = status, ProductID = productId, BestelrondeID = bestelrondeId });
            }
        }

        // Mark a task (product) as completed and update the order status to 'In Progress' if necessary
        public async Task CompleteTaskAndUpdateOrderStatusAsync(int orderId, int productId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    UPDATE Product_per_Bestelronde
                    SET AantalBetaald = AantalBetaald + 1
                    WHERE ProductID = @ProductID AND BestelrondeID IN (
                        SELECT BestelrondeID FROM Bestelronde WHERE BestelrondeID IN (
                            SELECT BestelrondeID FROM Bestelling WHERE BestellingID = @BestellingID
                        )
                    )";
                await connection.ExecuteAsync(sql, new { ProductID = productId, BestellingID = orderId });

                var updateOrderStatusSql = @"
                    UPDATE Bestelling
                    SET StatusBestelling = 'In Progress'
                    WHERE BestellingID = @BestellingID";
                await connection.ExecuteAsync(updateOrderStatusSql, new { BestellingID = orderId });
            }
        }

        // Mark a task (product) as completed and update its status, then update the order status to 'In Progress'
        public async Task CompleteTaskAndUpdateProductStatusAsync(int orderId, int productId, string status)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                UPDATE Product_per_Bestelronde
                SET StatusBesteldeProduct = @Status
                WHERE ProductID = @ProductID AND BestelrondeID IN (
                SELECT BestelrondeID FROM Bestelronde WHERE BestelrondeID IN (
                SELECT BestelrondeID FROM Bestelling WHERE BestellingID = @BestellingID ))";
                await connection.ExecuteAsync(sql, new { Status = status, ProductID = productId, BestellingID = orderId });

                var updateOrderStatusSql = @"
                UPDATE Bestelling
                SET StatusBestelling = 'In Progress'
                WHERE BestellingID = @BestellingID";
                await connection.ExecuteAsync(updateOrderStatusSql, new { BestellingID = orderId });
            }
        }

        // Check if all products in a specific order are completed
        public async Task<bool> AreAllProductsCompletedAsync(int bestellingId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                SELECT COUNT(*)
                FROM Product_per_Bestelronde
                WHERE BestelrondeID IN (
                SELECT BestelrondeID
                FROM Bestelronde
                WHERE BestelrondeID IN (
                    SELECT BestelrondeID
                    FROM Bestelling
                    WHERE BestellingID = @BestellingID
                )) AND StatusBesteldeProduct != 'Completed'";

                var incompleteProductCount = await connection.ExecuteScalarAsync<int>(sql, new { BestellingID = bestellingId });
                return incompleteProductCount == 0;
            }
        }
    }
}


/*
 * How to Use the EmployeeService Class:
 *
 * 1. Retrieve a list of new orders:
 * 
 * var employeeService = new EmployeeService(configuration);
 * var newOrders = await employeeService.GetNewOrdersAsync();
 * // The newOrders variable will contain a list of all new orders with status 'Pending'.
 * 
 * 2. Retrieve a list of completed or deleted orders (order history):
 * 
 * var orderHistory = await employeeService.GetOrderHistoryAsync();
 * // The orderHistory variable will contain a list of all orders with status 'Completed' or 'Deleted'.
 * 
 * 3. Retrieve a list of products associated with a specific order:
 * 
 * int bestellingId = 1;
 * var products = await employeeService.GetProductsForOrderAsync(bestellingId);
 * // The products variable will contain a list of products associated with the specified order.
 * 
 * 4. Update the status of a specific order:
 * 
 * int orderId = 1;
 * string newStatus = "Completed";
 * await employeeService.UpdateOrderStatusAsync(orderId, newStatus);
 * // The status of the order with the specified orderId will be updated to "Completed".
 * 
 * 5. Complete a task (product) and update the order status:
 * 
 * int orderId = 1;
 * int productId = 1;
 * await employeeService.CompleteTaskAndUpdateOrderStatusAsync(orderId, productId);
 * // The specified product will be marked as completed and the order status will be updated if necessary.
 * 
 * 6. Update the status of a specific product in an order:
 * 
 * int productId = 1;
 * int bestelrondeId = 1;
 * string status = "Completed";
 * await employeeService.UpdateProductStatusAsync(productId, bestelrondeId, status);
 * // The status of the specified product in the order will be updated to "Completed".
 * 
 * 7. Check if all products in a specific order are completed:
 * 
 * int bestellingId = 1;
 * bool allProductsCompleted = await employeeService.AreAllProductsCompletedAsync(bestellingId);
 * // The allProductsCompleted variable will be true if all products in the specified order are completed, otherwise false.
 */