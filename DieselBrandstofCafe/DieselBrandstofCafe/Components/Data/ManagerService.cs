using DieselBrandstofCafe.Components.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;



/*
    The ManagerService class, along with the IManagerService interface, provides a structured way to manage various aspects of a cafe's data, 
    such as orders, employees, products, and categories. By using dependency injection for the configuration, 
    it ensures that the database connection can be easily configured and changed without modifying the service logic.
*/




/*
 * Functions used:
 * 
 * async: Indicates that the method is asynchronous and returns a Task.
 * Task: Represents an asynchronous operation.
 * 
*/


// This namespace groups related classes and interfaces, providing a logical organization and preventing naming conflicts.
namespace DieselBrandstofCafe.Components.Data
{
    // The IManagerService interface defines a contract for the management service. It specifies the methods that any class implementing this interface must provide.
    public interface IManagerService
    {
        // Retrieves the history of orders.
        Task<IEnumerable<Bestelling>> GetOrderHistoryAsync();
        // Retrieves a list of employees.
        Task<IEnumerable<Medewerker>> GetEmployeesAsync();
        // Retrieves a list of products.
        Task<IEnumerable<Product>> GetProductsAsync();
        // Adds a new product.
        Task AddProductAsync(Product product);
        // Updates an existing product.
        Task UpdateProductAsync(Product product);
        // Deletes a product by its ID.
        Task DeleteProductAsync(int productId);
        // Retrieves a list of categories.
        Task<IEnumerable<Categorie>> GetCategoriesAsync();
        // Adds a new category.
        Task AddCategoryAsync(Categorie categorie);
        // Updates an existing category.
        Task UpdateCategoryAsync(Categorie categorie);
        // Deletes a category by its ID.
        Task DeleteCategoryAsync(int categorieId);
    }

    /* 
       The ManagerService class provides the implementation of the IManagerService interface.
       The class is being used to bundle up all the methods we have implemented to export them later on
       The constructor accepts an IConfiguration object to retrieve the connection string for the database.
    */

    public class ManagerService(IConfiguration configuration) : IManagerService
{
    /*
        It initializes a private readonly field _connectionString with the connection string from the configuration.
        The DefaultConnection value is located within the appsettings.json. It's the IP, port, username and password of the SQL db
    */

    private readonly string _connectionString = configuration?.GetConnectionString("DefaultConnection")
        // Throws an ArgumentNullException if the configuration is null, ensuring that the service cannot operate without valid configuration.
        ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");


    /*    
        Method: GetOrderHistoryAsync
        Purpose: Retrieves the history of orders from the database.

        Implementation:

        Creates a new SqlConnection using the _connectionString.
        Defines an SQL query to select all records from the Bestelling table.
        Executes the query asynchronously using Dapper's QueryAsync method.
        Returns the result as an IEnumerable<Bestelling>. IEnumerable is an collection of items with unknown size. The <Bestelling> is the type of the collection or list.
    */
    public async Task<IEnumerable<Bestelling>> GetOrderHistoryAsync()
    {
        using var connection = new MySqlConnection(_connectionString);
        var sql = "SELECT * FROM Bestelling";
        return await connection.QueryAsync<Bestelling>(sql);
    }

    // The method returns a task that, when completed, will provide a list of Employee objects from the DB.
    public async Task<IEnumerable<Medewerker>> GetEmployeesAsync()
    {

        // This creates a new SqlConnection object using the connection string (_connectionString).
        // The using statement ensures that the connection is properly disposed of when the block is exited, even if an exception occurs.
        using (var connection = new MySqlConnection(_connectionString))
        {
            // This defines the SQL query to be executed. The query retrieves all columns from the Medewerker table.
            var sql = "SELECT * FROM Medewerker";

            // await connection.QueryAsync<Employee>(sql): Executes the SQL query asynchronously using Dapper.
            // QueryAsync<Employee>: Maps the result of the query to a collection of Medewerker objects.
            return await connection.QueryAsync<Medewerker>(sql);
        }
    }

    // The method returns a task that, when completed, will provide a list of Product objects from the DB.
    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        // This creates a new SqlConnection object using the connection string (_connectionString).
        // The using statement ensures that the connection is properly disposed of when the block is exited, even if an exception occurs.
        using (var connection = new MySqlConnection(_connectionString))
        {
            // await connection.QueryAsync<Employee>: Executes the SQL query asynchronously using Dapper.
            // QueryAsync<Employee>: Maps the result of the query to a collection of Product objects.
            return await connection.QueryAsync<Product>("SELECT * FROM Product");
        }
    }


    // AddProductAsync(Product product): Method name and parameter. This method takes a Product object as a parameter.
    // it then returns a task that, when completed, will add a new product into the Product table within the DB.
    public async Task AddProductAsync(Product product)
    {
        // This creates a new SqlConnection object using the connection string (_connectionString).
        // The using statement ensures that the connection is properly disposed of when the block is exited, even if an exception occurs.
        using (var connection = new MySqlConnection(_connectionString))
        {
            /*
             *  Defines the SQL query to insert a new product into the Product table.
             *  Uses parameterized query syntax to prevent SQL injection.
            */
            var sql = @"
            INSERT INTO Product (CategorieID, ProductNaam, Prijs, Voorraad, AddOnID) 
            VALUES (@CategorieID, @ProductNaam, @Prijs, @Voorraad, @AddOnID)";
            
            /*
             *  Executes the SQL query asynchronously using Dapper's ExecuteAsync method.
             *  Passes the product object as a parameter, mapping its properties to the SQL query parameters.
            */
            await connection.ExecuteAsync(sql, product);
        }
    }


    // UpdateProductAsync(Product product): Method name and parameter. This method takes a Product object as a parameter.
    // it then returns a task that, when completed, will update an existing product within the Product table in the DB.
    public async Task UpdateProductAsync(Product product)
    {
        // This creates a new SqlConnection object using the connection string (_connectionString).
        // The using statement ensures that the connection is properly disposed of when the block is exited, even if an exception occurs.
        using (var connection = new MySqlConnection(_connectionString))
        {
            /*
             *  Defines the SQL query to update an existing product within the Product table.
             *  Uses parameterized query syntax to prevent SQL injection.
            */
            var sql = @"
            UPDATE Product 
            SET CategorieID = @CategorieID, ProductNaam = @ProductNaam, Prijs = @Prijs, Voorraad = @Voorraad, AddOnID = @AddOnID 
            WHERE ProductID = @ProductID";
            /*
             *  Executes the SQL query asynchronously using Dapper's ExecuteAsync method.
             *  Passes the product object as a parameter, mapping its properties to the SQL query parameters.
            */
            await connection.ExecuteAsync(sql, product);
        }
    }

    // DeleteProductAsync(int productId): Method name and parameter. This method takes the proudctId integer as a parameter.
    // it then returns a task that, when completed, will delete an existing product from the Product table in the DB.
    public async Task DeleteProductAsync(int productId)
    {
        // This creates a new SqlConnection object using the connection string (_connectionString).
        // The using statement ensures that the connection is properly disposed of when the block is exited, even if an exception occurs.
        using (var connection = new MySqlConnection(_connectionString))
        {
            /*
             *  Defines the SQL query to delete an existing product within the Product table.
             *  Uses parameterized query syntax to prevent SQL injection.
            */
            var sql = "DELETE FROM Product WHERE ProductID = @ProductID";
            /*
             *  Executes the SQL query asynchronously using Dapper's ExecuteAsync method.
             *  Passes the product object as a parameter, mapping its properties to the SQL query parameters.
             *  
             *  Creates an anonymous object with a property ProductID that is set to the value of the productId variable.
             *  This object is used to pass parameters to the SQL query in a safe and structured way, preventing SQL injection.
             *  Dapper uses this object to safely pass the value of productId to the SQL command, replacing @ProductID with the actual value.
             * 
             * Parameterized Query: By using an anonymous object to pass parameters, this approach ensures that the SQL command is parameterized, which helps prevent SQL injection attacks.
             * Asynchronous Execution: The ExecuteAsync method allows the command to be executed asynchronously, which is beneficial for maintaining responsiveness in applications, 
             * especially those with UI components or web servers that handle multiple requests concurrently.
            */

            await connection.ExecuteAsync(sql, new { ProductID = productId });
        }
    }

    
    // --------------------------------------Category-------------------------------------------------// 

    
    // The method returns a task that, when completed, will provide a list of Categorie objects from the DB.
    public async Task<IEnumerable<Categorie>> GetCategoriesAsync()
    {
        // This creates a new SqlConnection object using the connection string (_connectionString).
        // The using statement ensures that the connection is properly disposed of when the block is exited, even if an exception occurs.
        using (var connection = new MySqlConnection(_connectionString))
        {
            // await connection.QueryAsync<Categorie>(sql): Executes the SQL query asynchronously using Dapper.
            // QueryAsync<Categorie>: Maps the result of the query to a collection of Product objects.
            return await connection.QueryAsync<Categorie>("SELECT * FROM Categorie");
        }
    }

    // AddCategoryAsync(Categorie category): Method name and parameter. This method takes a Categorie object as a parameter.
    // it then returns a task that, when completed, will add a new category into the Categorie table within the DB.
    public async Task AddCategoryAsync(Categorie categorie)
    {
        // This creates a new SqlConnection object using the connection string (_connectionString).
        // The using statement ensures that the connection is properly disposed of when the block is exited, even if an exception occurs.
        using (var connection = new MySqlConnection(_connectionString))
        {
            /*
             *  Defines the SQL query to insert a new category to the Category table.
             *  Uses parameterized query syntax to prevent SQL injection.
            */
            var sql = @"
            INSERT INTO Categorie (NaamCategorie, ParentID) 
            VALUES (@NaamCategorie, @ParentID)";
            await connection.ExecuteAsync(sql, categorie);
        }
    }
    // UpdateCategoryAsync(Categorie category): Method name and parameter. This method takes a Category object as a parameter.
    // it then returns a task that, when completed, will update an existing category within the Category table in the DB.
    public async Task UpdateCategoryAsync(Categorie categorie)
    {
        // This creates a new SqlConnection object using the connection string (_connectionString).
        // The using statement ensures that the connection is properly disposed of when the block is exited, even if an exception occurs.
        using (var connection = new MySqlConnection(_connectionString))
        {
            /*
             *  Defines the SQL query to update an existing category within the Category table.
             *  Uses parameterized query syntax to prevent SQL injection.
            */
            var sql = @"
            UPDATE Categorie 
            SET NaamCategorie = @NaamCategorie, ParentID = @ParentID 
            WHERE CategorieID = @CategorieID";
            await connection.ExecuteAsync(sql, categorie);
        }
    }

    // DeleteCategoryAsync(int categoryId): Method name and parameter. This method takes the categoryId integer as a parameter.
    // it then returns a task that, when completed, will delete an existing category from the Category table in the DB.
    public async Task DeleteCategoryAsync(int categorieId)
    {
        // This creates a new SqlConnection object using the connection string (_connectionString).
        // The using statement ensures that the connection is properly disposed of when the block is exited, even if an exception occurs.
        using (var connection = new MySqlConnection(_connectionString))
        {
            /*
             *  Defines the SQL query to delete an existing category within the Category table.
             *  Uses parameterized query syntax to prevent SQL injection.
            */
            var sql = "DELETE FROM Categorie WHERE CategorieID = @CategorieID";

            /*
             *  Executes the SQL query asynchronously using Dapper's ExecuteAsync method.
             *  Passes the product object as a parameter, mapping its properties to the SQL query parameters.
             *  
             *  Creates an anonymous object with a property CategoryID that is set to the value of the categoryId variable.
             *  This object is used to pass parameters to the SQL query in a safe and structured way, preventing SQL injection.
             *  Dapper uses this object to safely pass the value of categoryId to the SQL command, replacing @CategoryID with the actual value.
             * 
             * Parameterized Query: By using an anonymous object to pass parameters, this approach ensures that the SQL command is parameterized, which helps prevent SQL injection attacks.
             * Asynchronous Execution: The ExecuteAsync method allows the command to be executed asynchronously, which is beneficial for maintaining responsiveness in applications, 
             * especially those with UI components or web servers that handle multiple requests concurrently.
            */

            await connection.ExecuteAsync(sql, new { CategorieID = categorieId });
        }
    }
}
}
