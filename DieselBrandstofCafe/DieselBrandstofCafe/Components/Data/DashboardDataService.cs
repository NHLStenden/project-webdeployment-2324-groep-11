using Dapper;
using DieselBrandstofCafe.Components.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
        Task<List<Overzicht>> GetDailySalesOverviewAsync();
    }

    public class DashboardDataService : IDashboardDataService
    {
        // Dit legt de configuration voor de connectie
        private readonly string _connectionString;

        public DashboardDataService(IConfiguration configuration)
        {
            _connectionString = configuration?.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");
        }

        // Dit haalt de totale winst (Revenue) op
        public async Task<int> GetTotalRevenueAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT SUM(TotaalPrijs) FROM Bestelling";
                return await connection.ExecuteScalarAsync<int>(sql);
            }
        }

        // Dit haalt de totale orders op
        public async Task<int> GetTotalOrdersAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT COUNT(*) FROM Bestelling;";
                return await connection.ExecuteScalarAsync<int>(sql);
            }
        }

        //Haalt totaal verkochte producten op
        public async Task<int> GetTotalProductsSoldAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT SUM(AantalProduct) FROM ProductPerBestelronde WHERE BestelrondeID IN (SELECT BestelrondeID FROM Bestelronde)";
                return await connection.ExecuteScalarAsync<int>(sql);
            }
        }

        // haalt alle menu items op
        public async Task<int> GetTotalMenuItemsAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT COUNT(*) FROM Product";
                return await connection.ExecuteScalarAsync<int>(sql);
            }
        }

        //Dagelijkse verkopen
        public async Task<List<Overzicht>> GetDailySalesOverviewAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Overzicht";
                return (await connection.QueryAsync<Overzicht>(sql)).ToList();
            }
        }
    }
}