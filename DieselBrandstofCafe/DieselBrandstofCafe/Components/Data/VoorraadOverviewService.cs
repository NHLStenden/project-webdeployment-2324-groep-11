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
        Task<OverzichtPerProduct> GetVoorraadVoorProductAsync(int productId);
        Task UpdateVoorraadAsync(int productId, int overzichtId, int nieuweVoorraad);
        Task VerhoogVoorraadAsync(int productId, int overzichtId, int hoeveelheid);
        Task VerkochteProductenAsync(int productId, int overzichtId, int hoeveelheid);
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
                var sql = @"
                SELECT opp.ProductID, opp.OverzichtID, opp.VoorraadPP, p.ProductNaam
                FROM Overzicht_per_product opp
                INNER JOIN Product p ON opp.ProductID = p.ProductID;
                ";
                return await connection.QueryAsync<OverzichtPerProduct>(sql);
            }
        }

        // Dit haalt de overzicht van voorraad voor een specifiek product
        public async Task<OverzichtPerProduct> GetVoorraadVoorProductAsync(int productId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM OverzichtPerProduct WHERE ProductId = @ProductId";
                var result = await connection.QueryFirstOrDefaultAsync<OverzichtPerProduct>(sql, new { ProductId = productId });

                if (result == null)
                {
                    throw new ArgumentException("Result is null");
                }
                else
                {
                    return result;
                }
            }
        }

        //Dit update de voorraad van alle producten ongeacht de huidige voorraad, dus je kan voorraad op 50 instellen ongeacht wat de vorige was bijvoorbeeld.
        public async Task UpdateVoorraadAsync(int productId, int overzichtId, int nieuweVoorraad)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql ="UPDATE OverzichtPerProduct SET VoorraadPP = @NieuweVoorraad WHERE ProductId = @ProductId AND OverzichtId = @OverzichtId"; 
                await connection.ExecuteAsync(sql, new { NieuweVoorraad = nieuweVoorraad, ProductId = productId, OverzichtId = overzichtId });
            }
        }

        /*Dit verhoogt de VoorraadPP tijdens inkoop, de voorraad wordt verhoogd met de nieuwe hoeveelheid. Bijvoorbeeld er was 40 stuks voorraad kan je 10 erbij
          en dan wordt het 50*/
        public async Task VerhoogVoorraadAsync(int productId, int overzichtId, int hoeveelheid)
        {
            using(var connection = new MySqlConnection(_connectionString))
            {
                var sql = "UPDATE OverzichtPerProduct SET VoorraadPP = VoorraadPP + @Hoeveelheid WHERE ProductId = @ProductId AND OverzichtId = @OverzichtId";
                await connection.ExecuteAsync(sql, new {Hoeveelheid = hoeveelheid, ProductId = productId, OverzichtID = overzichtId });
            }
        }

        /*Dit verlaagd de VoorraadPP als er producten verkocht worden. Dus als er een 50 cola in de voorraad is en er worden 2 verkocht wordt de voorraad verlaagd
         naar 48*/
        public async Task VerkochteProductenAsync(int productId, int overzichtId, int hoeveelheid)
        {
            using(var connection = new MySqlConnection(_connectionString))
            {
                var sql = "UPDATE OverzichtPerProduct SET VoorraadPP = VoorraadPP - @Hoeveelheid WHERE ProductId = @ProductId AND OverzichtId = @OverzichtId";
                await connection.ExecuteAsync(sql, new { Hoeveelheid = hoeveelheid, ProductId = productId, OverzichtID = overzichtId });
            }
        }
    }
}