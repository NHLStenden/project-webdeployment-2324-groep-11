using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DieselBrandstofCafe.Components.Models;
using MySql.Data.MySqlClient;
using Dapper;
using System.Collections;
using Microsoft.AspNetCore.Mvc;

namespace DieselBrandstofCafe.Components.Data
{
    public interface IOrderService
    {
        Task<int> PlaceOrderAsync(int tableId, List<OrderItem> orderItems);

        Task<IEnumerable<Models.Product>> GetAddOnsAsync();

        Task<int> AddBestelrondeToBestellingAsync(int bestellingId, List<OrderItem> orderItems);

        Task<int?> CheckLopendeBestellingVoorTafelID(int tableId);

        Task<int> CreateBestellingAsync(IDbConnection connection, int tableId, int bestelrondeId, IDbTransaction transaction);
    }

    public class OrderService : IOrderService
    {
        private readonly string _connectionString;

        public OrderService(IConfiguration configuration)
        {
            _connectionString = configuration?.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");
        }

        public async Task<int> PlaceOrderAsync(int tableId, List<OrderItem> orderItems)
        {
            var totalPrice = orderItems.Sum(item => item.Product?.ProductPrijs * item.AantalProduct);

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // Create new Bestelronde
                        var bestelrondeId = await CreateBestelrondeAsync(connection, transaction);
                        if (bestelrondeId <= 0)
                        {
                            throw new Exception("Failed to create Bestelronde");
                        }

                        // Create new Bestelling
                        var bestellingId = await CreateBestellingAsync(connection, tableId, bestelrondeId, transaction);
                        if (bestellingId <= 0)
                        {
                            throw new Exception("Failed to create Bestelling");
                        }

                        // Add products to the Product_per_Bestelronde table
                        foreach (var item in orderItems)
                        {
                            await AddProductToBestelrondeAsync(connection, bestelrondeId, item, transaction);
                        }

                        // Update totals
                        await UpdateBestellingTotalAsync(connection, bestellingId, totalPrice, transaction);

                        // Commit transaction
                        await transaction.CommitAsync();

                        return bestellingId;

                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction if there is an error
                        await transaction.RollbackAsync();
                        Console.Error.WriteLine($"Error: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public async Task<int> AddBestelrondeToBestellingAsync(int bestellingId, List<OrderItem> orderItems)
        {
            var totalPrice = orderItems.Sum(item => item.Product?.ProductPrijs * item.AantalProduct);

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // Voegt de producten toe voor de bestelronde
                        var bestelrondeId = await CreateBestelrondeAsync(connection, transaction);
                        if (bestelrondeId <= 0)
                        {
                            throw new Exception("Failed to create Bestelronde");
                        }

                        foreach (var item in orderItems)
                        {
                            await AddProductToBestelrondeAsync(connection, bestelrondeId, item, transaction);
                        }

                        // Update de totale prijs van de bestelling
                        await UpdateBestellingTotalAsync(connection, bestellingId, totalPrice, transaction);

                        // Knalt de transactie door
                        await transaction.CommitAsync();

                        return bestellingId;

                    }
                    catch (Exception ex)
                    {
                        // Voor als die shit fout gaat jetoch
                        await transaction.RollbackAsync();
                        Console.Error.WriteLine($"Error: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public async Task<int> CreateBestelrondeAsync(IDbConnection connection, IDbTransaction transaction)
        {
            var query = "INSERT INTO Bestelronde (OberID, StatusBestelling, Tijd) VALUES (@OberID, 'Pending', NOW()); SELECT LAST_INSERT_ID();";
            var parameters = new { OberID = 1 }; // You can set the actual OberID based on your logic

            var bestelrondeId = await connection.ExecuteScalarAsync<int>(query, parameters, transaction);
            return bestelrondeId;
        }

        public async Task<int> CreateBestellingAsync(IDbConnection connection, int tableId, int bestelrondeId, IDbTransaction transaction)
        {
            var query = "INSERT INTO Bestelling (TafelID, BestelrondeID, StatusBestelling, TijdBestelling, KostenplaatsnummerID, TotaalPrijs) VALUES (@TafelID, @BestelrondeID, 'Pending', NOW(), NULL, 0); SELECT LAST_INSERT_ID();";
            var parameters = new { TafelID = tableId, BestelrondeID = bestelrondeId };

            var bestellingId = await connection.ExecuteScalarAsync<int>(query, parameters, transaction);
            return bestellingId;
        }

        public async Task AddProductToBestelrondeAsync(IDbConnection connection, int bestelrondeId, Models.OrderItem item, IDbTransaction transaction)
        {
            var query = "INSERT INTO Product_per_Bestelronde (ProductID, BestelrondeID, AantalProduct, AantalBetaald, StatusBesteldeProduct) VALUES (@ProductID, @BestelrondeID, @AantalProduct, 0, 'Pending')";
            var parameters = new { ProductID = item.Product?.ProductID, BestelrondeID = bestelrondeId, AantalProduct = item.AantalProduct };

            await connection.ExecuteAsync(query, parameters, transaction);
        }

        public async Task UpdateBestellingTotalAsync(IDbConnection connection, int bestellingId, decimal? totalPrice, IDbTransaction transaction)
        {
            var query = "UPDATE Bestelling SET TotaalPrijs = @TotalPrice WHERE BestellingID = @BestellingID";
            var parameters = new { TotalPrice = totalPrice, BestellingID = bestellingId };

            await connection.ExecuteAsync(query, parameters, transaction);
        }

        public async Task<IEnumerable<Models.Product>> GetAddOnsAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Product";
                return await connection.QueryAsync<Models.Product>(query);
            }
        }

        public async Task<int?> CheckLopendeBestellingVoorTafelID(int tableId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "SELECT BestellingID FROM Bestelling WHERE TafelID = @TafelID AND (StatusBestelling = 'Pending' OR StatusBestelling = 'In Progress') LIMIT 1";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TafelID", tableId);

                    var result = await command.ExecuteScalarAsync();
                    if (result != null && int.TryParse(result.ToString(), out int bestellingId))
                    {
                        return bestellingId;
                    }
                }
            }

            return null; // Geen lopende bestelling gevonden
        }
    }
}