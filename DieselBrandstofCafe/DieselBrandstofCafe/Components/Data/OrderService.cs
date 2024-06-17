using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DieselBrandstofCafe.Components.Models;
using MySql.Data.MySqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using Microsoft.AspNetCore.Mvc;

public class StripeOptions
{
    public string? option { get; set; }
}

namespace DieselBrandstofCafe.Components.Data
{
    public interface IOrderService
    {
        Task<IActionResult> PlaceOrderAsync(List<Models.InvoiceItem> invoiceItems);
    }

    public class OrderService : IOrderService
    {
        private readonly string _connectionString;

        public OrderService(IConfiguration configuration)
        {
            StripeConfiguration.ApiKey = "sk_test_51PRMMQKAmWIm025ScNGYyIgTeh9czIc4nUO39mg9wqlfmipcG19iTuY1DGjFMJAXOVGEbHgfXzetMbxrEUdtEKQf00oyqPdt9e";

            _connectionString = configuration?.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");
        }

        public async Task<IActionResult> PlaceOrderAsync(List<Models.InvoiceItem> invoiceItems)
        {
            var totalPrice = invoiceItems.Sum(item => item.Product?.ProductPrijs * item.AantalProduct);

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
                        await UpdateBestellingTotalAsync(connection, bestellingId, totalPrice, transaction);

                        // Commit transaction
                        await transaction.CommitAsync();

                    }
                    catch
                    {
                        // Rollback transaction if there is an error
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }


            var domain = "https://localhost:7157";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "eur",
                    UnitAmount = Convert.ToInt32(totalPrice * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "DieselBrandstofCafe"
                    }
                },
                Quantity = 1,
                },
            },
                Mode = "payment",
                SuccessUrl = domain + "/ordersuccess",
                CancelUrl = domain + "/menu",
            };
            var service = new SessionService();
            Session session = service.Create(options);

            Console.WriteLine(session.Url);

            return new RedirectResult(session.Url);

            //return Redirect(session.Url);

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
        
        private async Task AddProductToBestelrondeAsync(IDbConnection connection, int bestelrondeId, Models.InvoiceItem item, IDbTransaction transaction)
        {
            var query = "INSERT INTO Product_per_Bestelronde (ProductID, BestelrondeID, AantalProduct, AantalBetaald, StatusBesteldeProduct) VALUES (@ProductID, @BestelrondeID, @AantalProduct, 0, 'Pending')";
            var parameters = new { ProductID = item.Product?.ProductID, BestelrondeID = bestelrondeId, AantalProduct = item.AantalProduct };

            await connection.ExecuteAsync(query, parameters, transaction);
        }

        private async Task UpdateBestellingTotalAsync(IDbConnection connection, int bestellingId, decimal? totalPrice, IDbTransaction transaction)
        {
            var query = "UPDATE Bestelling SET TotaalPrijs = @TotalPrice WHERE BestellingID = @BestellingID";
            var parameters = new { TotalPrice = totalPrice, BestellingID = bestellingId };

            await connection.ExecuteAsync(query, parameters, transaction);
        }
    }
}