using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DieselBrandstofCafe.Components.Models;
using MySql.Data.MySqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace DieselBrandstofCafe.Components.Data
{
    public interface IOrderService
    {
        Task<int> PlaceOrderAsync(List<InvoiceItem> invoiceItems);
    }

    public class OrderService : IOrderService
    {
        private readonly string _connectionString;

        public OrderService(IConfiguration configuration)
        {
            _connectionString = configuration?.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");
        }

        public async Task<int> PlaceOrderAsync(List<InvoiceItem> invoiceItems)
        {
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
                        var bestellingId = await CreateBestellingAsync(connection, bestelrondeId, transaction);
                        if (bestellingId <= 0)
                        {
                            throw new Exception("Failed to create Bestelling");
                        }

                        // Add products to the Product_per_Bestelronde table
                        foreach (var item in invoiceItems)
                        {
                            await AddProductToBestelrondeAsync(connection, bestelrondeId, item, transaction);
                        }

                        // Update totals
                        await UpdateBestellingTotalAsync(connection, bestellingId, invoiceItems, transaction);

                        // Commit transaction
                        await transaction.CommitAsync();

                        return bestellingId;
                    }
                    catch
                    {
                        // Rollback transaction if there is an error
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        private async Task<int> CreateBestelrondeAsync(IDbConnection connection, IDbTransaction transaction)
        {
            var query = "INSERT INTO Bestelronde (OberID, StatusBestelling, Tijd) VALUES (@OberID, 'Pending', NOW()); SELECT LAST_INSERT_ID();";
            var parameters = new { OberID = 1 }; // You can set the actual OberID based on your logic

            var bestelrondeId = await connection.ExecuteScalarAsync<int>(query, parameters, transaction);
            return bestelrondeId;
        }

        private async Task<int> CreateBestellingAsync(IDbConnection connection, int bestelrondeId, IDbTransaction transaction)
        {
            var query = "INSERT INTO Bestelling (TafelID, BestelrondeID, StatusBestelling, TijdBestelling, KostenplaatsnummerID, TotaalPrijs) VALUES (@TafelID, @BestelrondeID, 'Pending', NOW(), NULL, 0); SELECT LAST_INSERT_ID();";
            var parameters = new { TafelID = 1, BestelrondeID = bestelrondeId }; // You can set the actual TafelID based on your logic

            var bestellingId = await connection.ExecuteScalarAsync<int>(query, parameters, transaction);
            return bestellingId;
        }

        private async Task AddProductToBestelrondeAsync(IDbConnection connection, int bestelrondeId, InvoiceItem item, IDbTransaction transaction)
        {
            var query = "INSERT INTO Product_per_Bestelronde (ProductID, BestelrondeID, AantalProduct, AantalBetaald, StatusBesteldeProduct) VALUES (@ProductID, @BestelrondeID, @AantalProduct, 0, 'Pending')";
            var parameters = new { ProductID = item.Product?.ProductID, BestelrondeID = bestelrondeId, AantalProduct = item.AantalProduct };

            await connection.ExecuteAsync(query, parameters, transaction);
        }

        private async Task UpdateBestellingTotalAsync(IDbConnection connection, int bestellingId, List<InvoiceItem> invoiceItems, IDbTransaction transaction)
        {
            var totalPrice = invoiceItems.Sum(item => item.Product?.ProductPrijs * item.AantalProduct);
            var query = "UPDATE Bestelling SET TotaalPrijs = @TotalPrice WHERE BestellingID = @BestellingID";
            var parameters = new { TotalPrice = totalPrice, BestellingID = bestellingId };

            await connection.ExecuteAsync(query, parameters, transaction);
        }
    }
}