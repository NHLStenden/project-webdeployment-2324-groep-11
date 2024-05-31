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
        Task CompleteOrderAsync(int orderId);
        Task DeleteOrderAsync(int orderId);
    }


    public class EmployeeService(IConfiguration configuration) : IEmployeeService
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

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
    }
}
