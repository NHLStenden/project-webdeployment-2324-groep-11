using Dapper;
using DieselBrandstofCafe.Components.Models;
using MySql.Data.MySqlClient;


namespace DieselBrandstofCafe.Components.Data
{

        public interface IProductPerBestelrondeService
        {
            Task<IEnumerable<ProductPerBestelronde>> GetProductsPerBestelrondeAsync(int bestelrondeId);
            Task UpdateAantalBetaaldAsync(int productId, int bestelrondeId, int aantalBetaald);
            Task<int> GetTotaalBetaaldAsync(int bestelrondeId);
        }

        public class ProductPerBestelrondeService (IConfiguration configuration) : IProductPerBestelrondeService
        {
            private readonly string _connectionString = configuration?.GetConnectionString("DefaultConnection")
                        ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");

            // Dit haalt de bestelrondeID op
            public async Task<IEnumerable<ProductPerBestelronde>> GetProductsPerBestelrondeAsync(int bestelrondeId)
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var sql = "SELECT * FROM ProductTotBestelronde WHERE BestelrondeID = @BestelrondeID";
                    return await connection.QueryAsync<ProductPerBestelronde>(sql, new { BestelrondeID = bestelrondeId });
                }
            }

            // Dit update de hoeveelheid betaalde producten in de database
            public async Task UpdateAantalBetaaldAsync(int productId, int bestelrondeId, int aantalBetaald)
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var sql = "UPDATE ProductTotBestelronde SET AantalBetaald = @AantalBetaald WHERE ProductID = @ProductID AND BestelrondeID = @BestelrondeID";
                    await connection.ExecuteAsync(sql, new { ProductID = productId, BestelrondeID = bestelrondeId, AantalBetaald = aantalBetaald });
                }
            }

            // Haalt het totaal betaald hoeveelheid op
            public async Task<int> GetTotaalBetaaldAsync(int bestelrondeId)
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var sql = "SELECT SUM(AantalBetaald) FROM ProductTotBestelronde WHERE BestelrondeID = @BestelrondeID";
                    return await connection.ExecuteScalarAsync<int>(sql, new { BestelrondeID = bestelrondeId });
                }
            }
        }
}
