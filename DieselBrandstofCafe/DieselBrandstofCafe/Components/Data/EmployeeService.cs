using DieselBrandstofCafe.Components.Models;
using MySql.Data.MySqlClient;
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
        Task<IEnumerable<ProductPerBestelronde>> GetProductsForOrderAsync(int bestellingId);
        Task CompleteTaskAndUpdateProductStatusAsync(int orderId, int productId, string status);

        Task UpdateProductStatusAsync(int productId, int orderId, string status);

        Task<bool> AreAllProductsCompletedAsync(int bestellingId);
    }


    public class EmployeeService(IConfiguration configuration) : IEmployeeService
    {
        private readonly string _connectionString = configuration?.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");

        public async Task<IEnumerable<Bestelling>> GetNewOrdersAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Bestelling WHERE StatusBestelling = 'Pending'";
                return await connection.QueryAsync<Bestelling>(sql);
            }
        }

        public async Task<IEnumerable<Bestelling>> UpdateOrdersAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Bestelling WHERE StatusBestelling = 'Pending'";
                return await connection.QueryAsync<Bestelling>(sql);
            }
        }

        public async Task<IEnumerable<Bestelling>> GetOrderHistoryAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Bestelling WHERE StatusBestelling = 'Completed' OR StatusBestelling = 'Deleted'";
                return await connection.QueryAsync<Bestelling>(sql);
            }
        }

        public async Task<IEnumerable<ProductPerBestelronde>> GetProductsForOrderAsync(int bestellingId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                /*
                     The GetProductsForOrderAsync method uses a JOIN to retrieve products associated with an order:

                    INNER JOIN Product_per_Bestelronde ppb ON p.ProductID = ppb.ProductID: Joins Product and Product_per_Bestelronde tables.
                    INNER JOIN Bestelronde br ON ppb.BestelrondeID = br.BestelrondeID: Joins Product_per_Bestelronde and Bestelronde tables.
                    INNER JOIN Bestelling b ON br.BestelrondeID = b.BestelrondeID: Joins Bestelronde and Bestelling tables.
                */
                var sql = @"
                SELECT ppb.ProductID, ppb.BestelrondeID, ppb.AantalProduct, ppb.AantalBetaald, ppb.StatusBesteldeProduct, p.ProductNaam
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

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "UPDATE Bestelling SET StatusBestelling = @Status WHERE BestellingID = @BestellingID";
                await connection.ExecuteAsync(sql, new { Status = status, BestellingID = orderId });
            }
        }

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

        public async Task CompleteTaskAndUpdateProductStatusAsync(int orderId, int productId, string status)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                PDATE Product_per_Bestelronde
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
                )
            ) AND StatusBesteldeProduct != 'Completed'";

                var incompleteProductCount = await connection.ExecuteScalarAsync<int>(sql, new { BestellingID = bestellingId });
                return incompleteProductCount == 0;
            }
        }
    }
}
