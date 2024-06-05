using DieselBrandstofCafe.Components.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace DieselBrandstofCafe.Components.Data
{
    public interface IProductTotBestelrondeService
    {
        Task<IEnumerable<ProductTotBestelronde>> GetProductsPerBestelrondeAsync(int bestelrondeId);
        Task UpdateAantalBetaaldAsync(int productId, int bestelrondeId, int aantalBetaald);
        Task<int> GetTotaalBetaaldAsync(int bestelrondeId);
    }

    public class ProductTotBestelrondeService : IProductTotBestelrondeService
    {
        private readonly string _connectionString;

        // Configuration voor connectie
        public ProductTotBestelrondeService(IConfiguration configuration)
        {
            _connectionString = configuration?.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");
        }

        // Dit haalt de bestelrondeID op
        public async Task<IEnumerable<ProductTotBestelronde>> GetProductsPerBestelrondeAsync(int bestelrondeId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM ProductTotBestelronde WHERE BestelrondeID = @BestelrondeID";
                return await connection.QueryAsync<ProductTotBestelronde>(sql, new { BestelrondeID = bestelrondeId });
            }
        }

        // Dit update de hoeveelheid betaalde producten in de database
        public async Task UpdateAantalBetaaldAsync(int productId, int bestelrondeId, int aantalBetaald)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE ProductTotBestelronde SET AantalBetaald = @AantalBetaald WHERE ProductID = @ProductID AND BestelrondeID = @BestelrondeID";
                await connection.ExecuteAsync(sql, new { ProductID = productId, BestelrondeID = bestelrondeId, AantalBetaald = aantalBetaald });
            }
        }

        // Haalt het totaal betaald hoeveelheid op
        public async Task<int> GetTotaalBetaaldAsync(int bestelrondeId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT SUM(AantalBetaald) FROM ProductTotBestelronde WHERE BestelrondeID = @BestelrondeID";
                return await connection.ExecuteScalarAsync<int>(sql, new { BestelrondeID = bestelrondeId });
            }
        }
    }
}