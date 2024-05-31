using DieselBrandstofCafe.Components.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace DieselBrandstofCafe.Components.Data
{
    public interface IManagerService
    {
        Task<IEnumerable<Bestelling>> GetOrderHistoryAsync();
        Task<IEnumerable<Medewerker>> GetEmployeesAsync();
        Task<IEnumerable<Product>> GetProductsAsync();
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int productId);
        Task<IEnumerable<Categorie>> GetCategoriesAsync();
        Task AddCategoryAsync(Categorie category);
        Task UpdateCategoryAsync(Categorie category);
        Task DeleteCategoryAsync(int categoryId);
    }

    public class ManagerService(IConfiguration configuration) : IManagerService
    {
        private readonly string _connectionString = configuration?.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");

        public async Task<IEnumerable<Bestelling>> GetOrderHistoryAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "SELECT * FROM Bestelling";
            return await connection.QueryAsync<Bestelling>(sql);
        }

        public async Task<IEnumerable<Medewerker>> GetEmployeesAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Medewerker";
                return await connection.QueryAsync<Medewerker>(sql);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Product>("SELECT * FROM Product");
            }
        }

        public async Task AddProductAsync(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"
                INSERT INTO Product (CategoryID, ProductNaam, Prijs, Voorraad, AddOnID) 
                VALUES (@CategoryID, @ProductNaam, @Prijs, @Voorraad, @AddOnID)";
                await connection.ExecuteAsync(sql, product);
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"
                UPDATE Product 
                SET CategoryID = @CategoryID, ProductNaam = @ProductNaam, Prijs = @Prijs, Voorraad = @Voorraad, AddOnID = @AddOnID 
                WHERE ProductID = @ProductID";
                await connection.ExecuteAsync(sql, product);
            }
        }

        public async Task DeleteProductAsync(int productId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "DELETE FROM Product WHERE ProductID = @ProductID";
                await connection.ExecuteAsync(sql, new { ProductID = productId });
            }
        }

        public async Task<IEnumerable<Categorie>> GetCategoriesAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Categorie>("SELECT * FROM Categorie");
            }
        }

        public async Task AddCategoryAsync(Categorie category)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"
                INSERT INTO Categorie (NaamCategorie, ParentID) 
                VALUES (@NaamCategorie, @ParentID)";
                await connection.ExecuteAsync(sql, category);
            }
        }

        public async Task UpdateCategoryAsync(Categorie category)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"
                UPDATE Categorie 
                SET NaamCategorie = @NaamCategorie, ParentID = @ParentID 
                WHERE CategoryID = @CategoryID";
                await connection.ExecuteAsync(sql, category);
            }
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "DELETE FROM Categorie WHERE CategoryID = @CategoryID";
                await connection.ExecuteAsync(sql, new { CategoryID = categoryId });
            }
        }
    }
}
