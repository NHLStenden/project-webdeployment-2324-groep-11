using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DieselBrandstofCafe.Components.Models;

namespace DieselBrandstofCafe.Components.Data
{
    public interface ICategorieService
    {
        Task<IEnumerable<Categorie>> GetCategorieënAsync();
    }

    public class CategorieService : ICategorieService
    {
        private readonly string? _connectionString;

        public CategorieService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Categorie>> GetCategorieënAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Categorie";
                return await connection.QueryAsync<Categorie>(query);
            }
        }
    }
}