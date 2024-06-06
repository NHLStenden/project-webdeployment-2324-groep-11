using DieselBrandstofCafe.Components.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace DieselBrandstofCafe.Components.Data
{
    public interface IVoorraadOverviewService
    {
        Task<IEnumerable<OverzichtPerProduct>> GetOverzichtVoorraadAsync();
        Task UpdateVoorraadAsync(int productId, int overzichtId, int nieuweVoorraad);
        Task<OverzichtPerProduct> GetVoorraadVoorProductAsync(int productId);
    }

    public class VoorraadOverviewService : IVoorraadOverviewService
    {
        // Dit legt de configuration voor de connectie
        private readonly string _connectionString;

        public VoorraadOverviewService(IConfiguration configuration)
        {
            _connectionString = configuration?.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");
        }

        // Dit haalt de overzicht voorraad op van alle producten
        public async Task<IEnumerable<OverzichtPerProduct>> GetOverzichtVoorraadAsync()
        {
            using(var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM OverzichtPerProduct";
                return await connection.QueryAsync<OverzichtPerProduct>(sql);
            }
        }

        // Dit haalt de overzicht van voorraad voor een specifiek product
        public async Task<OverzichtPerProduct> GetVoorraadVoorProductAsync(int productId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM OverzichtPerProduct WHERE ProductId = @ProductId";
                return await connection.QueryFirstOrDefaultAsync<OverzichtPerProduct>(sql, new { ProductId = productId });
            }
        }

        //Dit update de voorraad van alle producten
        public async Task UpdateVoorraadAsync(int productId, int overzichtId, int nieuweVoorraad)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "UPDATE OverzichtPerProduct SET VoorraadPP = @NieuweVoorraad WHERE ProductId = @ProductId AND OverzichtID = @OverzichtID";
                await connection.ExecuteAsync(sql, new { NieuweVoorraad = nieuweVoorraad, ProductId = productId, OverzichtID = overzichtId });
            }
        }
    }
}