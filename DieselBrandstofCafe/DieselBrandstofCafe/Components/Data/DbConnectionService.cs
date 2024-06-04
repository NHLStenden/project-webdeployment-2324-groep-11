using DieselBrandstofCafe.Components.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace DieselBrandstofCafe.Components.Data
{
    public interface IDbConnectionService 
    {
        Task<bool> CheckDatabaseConnectionAsync();

    }
    public class DbConnectionService(IConfiguration configuration) : IDbConnectionService
    {
        private readonly string _connectionString = configuration?.GetConnectionString("DefaultConnection")
                // Throws an ArgumentNullException if the configuration is null, ensuring that the service cannot operate without valid configuration.
                ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");

        public async Task<bool> CheckDatabaseConnectionAsync()
        {

            try
            {

                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                var command = new MySqlCommand("Select 1", connection);
                await command.ExecuteScalarAsync();

                return true;

            }
            catch (Exception ex)
            {
                // Log the exception (ensure you have a logging mechanism in place)
                Console.WriteLine($"Error connecting to the database: {ex.Message}");
                return false;
            }

        }
    }
}
