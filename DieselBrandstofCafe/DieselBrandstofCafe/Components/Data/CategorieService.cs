using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using DieselBrandstofCafe.Components.Models;

namespace DieselBrandstofCafe.Components.Data
{
    public interface ICategorieService
    {
        Task<IEnumerable<Categorie>> GetCategoryAsync();
        Task<IEnumerable<Categorie>> GetParentCategoryAsync();
        Task<IEnumerable<Categorie>> GetChildCategoryAsync(int parentId);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetProductsAsync();

    }

    public class CategorieService : ICategorieService
    {
        private readonly string? _connectionString;

        public CategorieService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Categorie>> GetCategoryAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Categorie";
                return await connection.QueryAsync<Categorie>(query);
            }
        }

        public async Task<IEnumerable<Categorie>> GetParentCategoryAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Categorie WHERE ParentID IS NULL";
                return await connection.QueryAsync<Categorie>(query);
            }
        }

        public async Task<IEnumerable<Categorie>> GetChildCategoryAsync(int parentId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Categorie WHERE ParentID = @ParentId";
                return await connection.QueryAsync<Categorie>(query, new { ParentId = parentId });
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Product WHERE CategorieID = @CategoryId";
                return await connection.QueryAsync<Product>(query, new { CategoryId = categoryId });
            }
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Product";
                return await connection.QueryAsync<Product>(query);
            }
        }
    }
}
