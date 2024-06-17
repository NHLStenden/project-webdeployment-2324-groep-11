using Dapper;
using DieselBrandstofCafe.Components.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DieselBrandstofCafe.Components.Data
{
    public interface IDashboardDataService
    {
        Task<int> GetTotalRevenueAsync();
        Task<int> GetTotalOrdersAsync();
        Task<int> GetTotalProductsSoldAsync();
        Task<int> GetTotalMenuItemsAsync();
        Task<List<ProductPerBestelronde>> GetPopularProductsAsync();
        Task<Dictionary<string, int>> GetWeeklySalesDataAsync();
    }

    public class DashboardDataService : IDashboardDataService
    {
        private readonly string _connectionString;

        public DashboardDataService(IConfiguration configuration)
        {
            _connectionString = configuration?.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");
        }

        public async Task<int> GetTotalRevenueAsync()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var sql = "SELECT SUM(TotaalPrijs) FROM Bestelling";
                    return await connection.ExecuteScalarAsync<int>(sql);
                }
            }
            catch (Exception ex)
            {
                // Log de fout hier
                Console.WriteLine($"Error in GetTotalRevenueAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<int> GetTotalOrdersAsync()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var sql = "SELECT COUNT(*) FROM Bestelling;";
                    return await connection.ExecuteScalarAsync<int>(sql);
                }
            }
            catch (Exception ex)
            {
                // Log de fout hier
                Console.WriteLine($"Error in GetTotalOrdersAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<int> GetTotalProductsSoldAsync()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var sql = "SELECT SUM(AantalProduct) FROM Product_per_Bestelronde WHERE BestelrondeID IN (SELECT BestelrondeID FROM Bestelronde)";
                    return await connection.ExecuteScalarAsync<int>(sql);
                }
            }
            catch (Exception ex)
            {
                // Log de fout hier
                Console.WriteLine($"Error in GetTotalProductsSoldAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<int> GetTotalMenuItemsAsync()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var sql = "SELECT COUNT(*) FROM Product";
                    return await connection.ExecuteScalarAsync<int>(sql);
                }
            }
            catch (Exception ex)
            {
                // Log de fout hier
                Console.WriteLine($"Error in GetTotalMenuItemsAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<ProductPerBestelronde>> GetPopularProductsAsync()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var sql = @"
                    SELECT ppb.ProductID, p.ProductNaam, SUM(ppb.AantalProduct) AS TotaleAantalProduct
                    FROM Product_Per_Bestelronde ppb
                    INNER JOIN Product p ON ppb.ProductID = p.ProductID
                    GROUP BY ppb.ProductID, p.ProductNaam
                    ORDER BY TotaleAantalProduct DESC;";
                        
                    return (await connection.QueryAsync<ProductPerBestelronde>(sql)).ToList();
                 }
            }
            catch (Exception ex)
            {
                // Log de fout hier
                Console.WriteLine($"Error in GetPopularProductsAsync: {ex.Message}");
                throw;
            }
        }
        public async Task<Dictionary<string, int>> GetWeeklySalesDataAsync()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var sql = @"
                    SELECT 
                    DAYNAME(DATE(VerkoopDatumProduct)) AS DayOfWeek, 
                    SUM(AantalProduct) AS TotalProductsSold
                    FROM Product_per_Bestelronde
                    WHERE VerkoopDatumProduct >= DATE_SUB(CURDATE(), INTERVAL 7 DAY)
                    GROUP BY DayOfWeek
                    ORDER BY FIELD(DayOfWeek, 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday');";

                    var salesData = await connection.QueryAsync<ProductPerBestelronde>(sql);
                    return salesData.ToDictionary(sd => sd.DayOfWeek, sd => sd.TotalProductsSold);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetWeeklySalesDataAsync: {ex.Message}");
                throw;
            }
        }
    }
}