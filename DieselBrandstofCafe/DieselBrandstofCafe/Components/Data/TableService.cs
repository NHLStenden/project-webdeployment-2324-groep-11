using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DieselBrandstofCafe.Components.Models;
using Dapper;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;


namespace DieselBrandstofCafe.Components.Data
{

    public interface ITableService
    {
        Task<IEnumerable<Tafel>> GetTablesAsync();
        Task<Tafel> GetTableAsync(int id);
        Task<Tafel> AddTableAsync(Tafel tafel);
        Task<Tafel> UpdateTableAsync(Tafel tafel);
        Task<Tafel> DeleteTableAsync(int id);
    }

    public class TableService : ITableService
    {
        private readonly string _connectionString;

        public TableService(IConfiguration configuration)
        {
            _connectionString = configuration?.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");
        }

        // Retrieves all tables from the database.
        public async Task<IEnumerable<Tafel>> GetTablesAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Tafel";
                return await connection.QueryAsync<Tafel>(query);
            }
        }

        // Retrieves a single table by its ID.
        public async Task<Tafel> GetTableAsync(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Tafel WHERE TafelId = @Id";
                return await connection.QueryFirstOrDefaultAsync<Tafel>(query, new { Id = id });
            }
        }

        // Adds a new table to the database and returns the added table with its new ID.
        public async Task<Tafel> AddTableAsync(Tafel tafel)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var query = "INSERT INTO Tafel (TafelSectie, TafelAfbeelding) VALUES (@TafelSectie, @TafelAfbeelding); SELECT LAST_INSERT_ID();";
                var id = await connection.ExecuteScalarAsync<int>(query, tafel);
                tafel.TafelId = id;
                return tafel;
            }
        }


        // Updates an existing table and returns the updated table.
        public async Task<Tafel> UpdateTableAsync(Tafel tafel)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var query = "UPDATE Tafel SET TafelSectie = @TafelSectie, TafelAfbeelding = @TafelAfbeelding WHERE TafelId = @TafelId";
                await connection.ExecuteAsync(query, tafel);
                return tafel;
            }
        }

        // Deletes a table by its ID and returns the deleted table.
        public async Task<Tafel> DeleteTableAsync(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var query = "DELETE FROM Tafel WHERE TafelId = @Id";
                var tafel = await GetTableAsync(id);
                await connection.ExecuteAsync(query, new { Id = id });
                return tafel;
            }
        }



    }
}