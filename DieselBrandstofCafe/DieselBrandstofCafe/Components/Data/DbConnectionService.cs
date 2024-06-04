using DieselBrandstofCafe.Components.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace DieselBrandstofCafe.Components.Data
{
    // Interface definition for DbConnectionService to handle database connection checks
    public interface IDbConnectionService
    {
        // Method to check if the database connection is successful
        Task<bool> CheckDatabaseConnectionAsync();
    }

    // Implementation of the IDbConnectionService interface
    public class DbConnectionService : IDbConnectionService
    {
        // Connection string to the MySQL database
        private readonly string _connectionString;

        // Constructor to initialize the connection string from configuration settings
        public DbConnectionService(IConfiguration configuration)
        {
            _connectionString = configuration?.GetConnectionString("DefaultConnection")
                // Throws an ArgumentNullException if the configuration is null, ensuring that the service cannot operate without valid configuration.
                ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");
        }

        // Method to check if the database connection is successful
        public async Task<bool> CheckDatabaseConnectionAsync()
        {
            try
            {
                // Open a connection to the database
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                // Execute a simple query to test the connection
                var command = new MySqlCommand("Select 1", connection);
                await command.ExecuteScalarAsync();

                // If no exceptions were thrown, the connection is successful
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


/*
 * How to Use the DbConnectionService Class:
 *
 * 1. Initialize the DbConnectionService:
 * 
 * var configuration = new ConfigurationBuilder()
 *     .AddJsonFile("appsettings.json")
 *     .Build();
 * var dbConnectionService = new DbConnectionService(configuration);
 *
 * 2. Check the database connection:
 * 
 * bool isConnected = await dbConnectionService.CheckDatabaseConnectionAsync();
 * if (isConnected)
 * {
 *     Console.WriteLine("Database connection successful.");
 * }
 * else
 * {
 *     Console.WriteLine("Database connection failed.");
 * }
 * 
 * Explanation:
 * 
 * - Step 1: Initialize the `DbConnectionService` with the configuration settings. The `ConfigurationBuilder` is used to load the configuration from the `appsettings.json` file.
 * - Step 2: Use the `CheckDatabaseConnectionAsync` method to verify if the database connection is successful. The method returns a boolean value indicating the connection status.
 * - The example logs the result to the console, but in a real application, you might handle it differently, such as showing a message to the user or retrying the connection.
 */