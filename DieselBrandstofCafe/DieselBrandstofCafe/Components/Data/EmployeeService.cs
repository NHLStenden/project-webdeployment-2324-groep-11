using DieselBrandstofCafe.Components.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;


namespace DieselBrandstofCafe.Components.Data
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Bestelling>> GetNewOrdersAsync();
        Task<IEnumerable<Bestelling>> GetOrderHistoryAsync();
        Task UpdateOrderStatusAsync(int orderId, string status);
        Task CompleteTaskAndUpdateOrderStatusAsync(int orderId, int productId);
    }


    public class EmployeeService(IConfiguration configuration) : IEmployeeService
    {
        private readonly string _connectionString = configuration?.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");

        public async Task<IEnumerable<Bestelling>> GetNewOrdersAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Bestelling WHERE Status = 'New'";
                return await connection.QueryAsync<Bestelling>(sql);
            }
        }

        public async Task<IEnumerable<Bestelling>> UpdateOrdersAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Bestelling WHERE Status = 'New'";
                return await connection.QueryAsync<Bestelling>(sql);
            }
        }

        public async Task<IEnumerable<Bestelling>> GetOrderHistoryAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Bestelling WHERE Status = 'Completed' OR Status = 'Deleted'";
                return await connection.QueryAsync<Bestelling>(sql);
            }
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE Bestelling SET Status = @Status WHERE BestellingID = @BestellingID";
                await connection.ExecuteAsync(sql, new { BestellingID = orderId, Status = status });
            }
        }

        public async Task CompleteTaskAndUpdateOrderStatusAsync(int orderId, int productId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE ProductTotBestelronde SET AantalBetaald = 1 WHERE BestellingID = @BestellingID AND ProductID = @ProductID";
                await connection.ExecuteAsync(sql, new { BestellingID = orderId, ProductID = productId });

                // Check if all products are completed
                var incompleteProductsCount = await connection.QuerySingleAsync<int>(
                    "SELECT COUNT(*) FROM ProductTotBestelronde WHERE BestellingID = @BestellingID AND AantalBetaald = 0",
                    new { BestellingID = orderId }
                );

                if (incompleteProductsCount == 0)
                {
                    await UpdateOrderStatusAsync(orderId, "Completed");
                }
            }
        }


    }
}
