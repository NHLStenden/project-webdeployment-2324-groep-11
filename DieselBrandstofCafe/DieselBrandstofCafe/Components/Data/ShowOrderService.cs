//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;
//using DieselBrandstofCafe.Components.Models;
//using MySql.Data.MySqlClient;
//using Dapper;
//using System.Collections;
//using Microsoft.AspNetCore.Mvc;

//namespace DieselBrandstofCafe.Components.Data
//{
//    public interface IShowOrderService
//    {

//    }

//    public class ShowOrderService : IShowOrderService
//    {
//        private readonly string _connectionString;

//        public OrderService(IConfiguration configuration)
//        {
//            _connectionString = configuration?.GetConnectionString("DefaultConnection")
//                ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");
//        }

        

//        public async Task<int> GiveTotaalPrijs(IDbConnection connection, IDbTransaction transaction)
//        {
//            var query = "SELECT TotaalPrijs, BestellingID FROM Bestelling INNER JOIN OrderWithProducts ON Bestelling.TotaalPrijs = OrderWithProducts.TotaalPrijs();";
//            var parameters = new { OberID = 1 }; // You can set the actual OberID based on your logic

//            var bestelrondeId = await connection.ExecuteScalarAsync<int>(query, parameters, transaction);
//            return bestelrondeId;
        
//    }
//}