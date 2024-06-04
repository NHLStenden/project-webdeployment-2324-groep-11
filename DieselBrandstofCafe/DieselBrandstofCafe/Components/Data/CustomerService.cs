using DieselBrandstofCafe.Components.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace DieselBrandstofCafe.Components.Data
{
    // Interface definition for CustomerService to handle various customer-related operations
    public interface ICustomerService
    {
        // Method to retrieve a list of products
        Task<IEnumerable<Product>> GetProductsAsync();

        // Method to place a new order and return the ID of the newly created order
        Task<int> PlaceOrderAsync(Bestelling order);

        // Method to retrieve a list of orders placed by a specific customer
        Task<IEnumerable<Bestelling>> GetCustomerOrdersAsync(int customerId);

        // Method to update the status of a specific order
        Task UpdateOrderAsync(int orderId, string status);

        // Additional methods as needed...
    }

    // Implementation of the ICustomerService interface
    public class CustomerService : ICustomerService
    {
        // Connection string to the MySQL database
        private readonly string _connectionString;

        // Constructor to initialize the connection string from configuration settings
        public CustomerService(IConfiguration configuration)
        {
            _connectionString = configuration?.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");
        }

        // Method to retrieve a list of products
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                // Execute a query to retrieve all products from the Product table
                return await connection.QueryAsync<Product>("SELECT * FROM Product");
            }
        }

        // Method to place a new order and return the ID of the newly created order
        public async Task<int> PlaceOrderAsync(Bestelling order)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                INSERT INTO Bestelling (TafelID, Status, Tijd, Kostenplaatsnummer, TotaalPrijs, BestelrondeID) 
                VALUES (@TafelID, @Status, @Tijd, @Kostenplaatsnummer, @TotaalPrijs, @BestelrondeID);
                SELECT CAST(SCOPE_IDENTITY() as int);";
                // Execute the insert statement and return the ID of the newly created order
                return await connection.QuerySingleAsync<int>(sql, order);
            }
        }

        // Method to retrieve a list of orders placed by a specific customer
        public async Task<IEnumerable<Bestelling>> GetCustomerOrdersAsync(int customerId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Bestelling WHERE CustomerID = @CustomerId";
                // Execute a query to retrieve all orders placed by the specified customer
                return await connection.QueryAsync<Bestelling>(sql, new { CustomerId = customerId });
            }
        }

        // Method to update the status of a specific order
        public async Task UpdateOrderAsync(int orderId, string status)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "UPDATE Bestelling SET Status = @Status WHERE BestellingID = @BestellingID";
                // Execute the update statement to change the status of the specified order
                await connection.ExecuteAsync(sql, new { BestellingID = orderId, Status = status });
            }
        }
    }
}


/*
 * How to Use the CustomerService Class:
 *
 * 1. Retrieve a list of products:
 * 
 * var customerService = new CustomerService(configuration);
 * var products = await customerService.GetProductsAsync();
 * // The products variable will contain a list of all products in the database.
 * 
 * 2. Place a new order:
 * 
 * var order = new Bestelling
 * {
 *     TafelID = 1,
 *     Status = "Pending",
 *     Tijd = DateTime.Now,
 *     Kostenplaatsnummer = 12345,
 *     TotaalPrijs = 2500,
 *     BestelrondeID = 1
 * };
 * var orderId = await customerService.PlaceOrderAsync(order);
 * // The orderId variable will contain the ID of the newly created order.
 * 
 * 3. Retrieve a list of orders placed by a specific customer:
 * 
 * int customerId = 1;
 * var orders = await customerService.GetCustomerOrdersAsync(customerId);
 * // The orders variable will contain a list of orders placed by the specified customer.
 * 
 * 4. Update the status of a specific order:
 * 
 * int orderId = 1;
 * string newStatus = "Completed";
 * await customerService.UpdateOrderAsync(orderId, newStatus);
 * // The status of the order with the specified orderId will be updated to "Completed".
 * 
 * Additional methods can be added to the ICustomerService interface and implemented in the CustomerService class as needed.
 */