using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Dapper;
using Testcontainers.MsSql;
using Xunit;

namespace TypedSqlBuilder.IntegrationTests;

/// <summary>
/// Fixture for SQL Server integration tests using Testcontainers
/// This creates one container that is shared across all tests in the class
/// </summary>
public class SqlServerFixture : IAsyncLifetime
{
    private MsSqlContainer? _container;
    
    /// <summary>
    /// Gets the connection string for the SQL Server container
    /// </summary>
    public string ConnectionString => _container?.GetConnectionString() ?? throw new InvalidOperationException("Container not initialized");

    /// <summary>
    /// Creates a new database connection
    /// </summary>
    public IDbConnection CreateConnection() => new SqlConnection(ConnectionString);

    /// <summary>
    /// Initializes the SQL Server container and sets up the test database
    /// </summary>
    public async Task InitializeAsync()
    {
        _container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest") // Use 2022 latest
            .WithPassword("Test123!Strong") // Strong password required by SQL Server
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("MSSQL_PID", "Express") // Use Express edition for lower resource usage
            .Build();
        
        await _container.StartAsync();
        await SetupTestDatabase();
    }

    /// <summary>
    /// Disposes the SQL Server container
    /// </summary>
    public async Task DisposeAsync()
    {
        if (_container != null)
        {
            await _container.DisposeAsync();
        }
    }

    /// <summary>
    /// Sets up the test database schema and seeds it with test data
    /// </summary>
    private async Task SetupTestDatabase()
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        
        await CreateTestTables(connection);
        await SeedTestData(connection);
    }

    /// <summary>
    /// Creates the test tables in the SQL Server database
    /// </summary>
    private async Task CreateTestTables(IDbConnection connection)
    {
        var createCustomers = @"
            CREATE TABLE customers (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                Age INT,
                Name NVARCHAR(255),
                IsActive BIT DEFAULT 1
            )";

        var createProducts = @"
            CREATE TABLE products (
                ProductId INT IDENTITY(1,1) PRIMARY KEY,
                ProductName NVARCHAR(255),
                Price DECIMAL(18,2),
                CreatedDate DATETIME2,
                UniqueId UNIQUEIDENTIFIER
            )";

        var createOrders = @"
            CREATE TABLE orders (
                OrderId INT IDENTITY(1,1) PRIMARY KEY,
                CustomerId INT,
                Amount INT,
                FOREIGN KEY (CustomerId) REFERENCES customers(Id)
            )";

        await connection.ExecuteAsync(createCustomers);
        await connection.ExecuteAsync(createProducts);
        await connection.ExecuteAsync(createOrders);
    }

    /// <summary>
    /// Seeds the test database with sample data
    /// </summary>
    private async Task SeedTestData(IDbConnection connection)
    {
        // Note: Using SET IDENTITY_INSERT to specify exact IDs for consistent testing
        var customerTuples = TestDataConstants.Customers
            .Select(c => $"({c.Id}, {c.Age}, N'{c.Name}', {(c.IsActive ? 1 : 0)})");

        var productTuples = TestDataConstants.Products
            .Select(p => $"({p.ProductId}, N'{p.ProductName}', " +
                       $"{(p.Price.HasValue ? p.Price.Value.ToString("F2") : "NULL")}, " +
                       $"{(p.CreatedDate.HasValue ? $"'{p.CreatedDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")}, " +
                       $"{(p.UniqueId.HasValue ? $"'{p.UniqueId.Value}'" : "NULL")})");

        var insertCustomers = $@"
            SET IDENTITY_INSERT customers ON;
            INSERT INTO customers (Id, Age, Name, IsActive) VALUES 
            {string.Join(",\n            ", customerTuples)};
            SET IDENTITY_INSERT customers OFF;";

        var insertProducts = $@"
            SET IDENTITY_INSERT products ON;
            INSERT INTO products (ProductId, ProductName, Price, CreatedDate, UniqueId) VALUES
            {string.Join(",\n            ", productTuples)};
            SET IDENTITY_INSERT products OFF;";

        var insertOrders = $@"
            SET IDENTITY_INSERT orders ON;
            INSERT INTO orders (OrderId, CustomerId, Amount) VALUES
            {string.Join(",\n            ", TestDataConstants.OrderTuples)};
            SET IDENTITY_INSERT orders OFF;";

        await connection.ExecuteAsync(insertCustomers);
        await connection.ExecuteAsync(insertProducts);
        await connection.ExecuteAsync(insertOrders);
    }

    /// <summary>
    /// Execute a test within a transaction that gets rolled back
    /// Use this for tests that modify data to ensure isolation
    /// </summary>
    public async Task WithTransactionAsync(Func<IDbConnection, IDbTransaction, Task> testAction)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        try
        {
            await testAction(connection, transaction);
        }
        finally
        {
            transaction.Rollback(); // Always rollback to keep data clean
        }
    }

    /// <summary>
    /// Execute a test within a transaction that gets rolled back (synchronous version)
    /// Use this for tests that modify data to ensure isolation
    /// </summary>
    public void WithTransaction(Action<IDbConnection, IDbTransaction> testAction)
    {
        using var connection = CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            testAction(connection, transaction);
        }
        finally
        {
            transaction.Rollback(); // Always rollback to keep data clean
        }
    }
}
