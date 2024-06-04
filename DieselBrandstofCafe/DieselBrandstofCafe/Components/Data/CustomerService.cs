using DieselBrandstofCafe.Components.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace DieselBrandstofCafe.Components.Data
{

    public interface ICustomerService
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<int> PlaceOrderAsync(Bestelling order);
        Task<IEnumerable<Bestelling>> GetCustomerOrdersAsync(int customerId);
        Task UpdateOrderAsync(int orderId, string status);
        // Additional methods as needed...
    }

    public class CustomerService(IConfiguration configuration) : ICustomerService
    {
        private readonly string _connectionString = configuration?.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Product>("SELECT * FROM Product");
            }
        }

        public async Task<int> PlaceOrderAsync(Bestelling order)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                INSERT INTO Bestelling (TafelID, Status, Tijd, Kostenplaatsnummer, TotaalPrijs, BestelrondeID) 
                VALUES (@TafelID, @Status, @Tijd, @Kostenplaatsnummer, @TotaalPrijs, @BestelrondeID);
                SELECT CAST(SCOPE_IDENTITY() as int);";
                return await connection.QuerySingleAsync<int>(sql, order);
            }
        }

        public async Task<IEnumerable<Bestelling>> GetCustomerOrdersAsync(int customerId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Bestelling WHERE CustomerID = @CustomerId";
                return await connection.QueryAsync<Bestelling>(sql, new { CustomerId = customerId });
            }
        }

        public async Task UpdateOrderAsync(int orderId, string status)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "UPDATE Bestelling SET Status = @Status WHERE BestellingID = @BestellingID";
                await connection.ExecuteAsync(sql, new { BestellingID = orderId, Status = status });
            }
        }
    }
}
