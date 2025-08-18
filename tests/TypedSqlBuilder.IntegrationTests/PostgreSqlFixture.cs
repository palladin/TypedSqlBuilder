using System.Data;
using System.Data.Common;
using Npgsql;
using Dapper;
using Testcontainers.PostgreSql;
using Xunit;

namespace TypedSqlBuilder.IntegrationTests;

/// <summary>
/// Fixture for PostgreSQL integration tests using Testcontainers
/// This creates one container that is shared across all tests in the class
/// </summary>
public class PostgreSqlFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;
    
    /// <summary>
    /// Gets the connection string for the PostgreSQL container
    /// </summary>
    public string ConnectionString => _container?.GetConnectionString() ?? throw new InvalidOperationException("Container not initialized");

    /// <summary>
    /// Creates a new database connection
    /// </summary>
    public IDbConnection CreateConnection() => new NpgsqlConnection(ConnectionString);

    /// <summary>
    /// Initializes the PostgreSQL container and sets up the test database
    /// </summary>
    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine") // Use PostgreSQL 16 Alpine for fast startup
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .Build();
        
        await _container.StartAsync();
        await SetupTestDatabase();
    }

    /// <summary>
    /// Disposes the PostgreSQL container
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
        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        
        await CreateTestTables(connection);
        await SeedTestData(connection);
    }

    /// <summary>
    /// Creates the test tables in the PostgreSQL database
    /// </summary>
    private async Task CreateTestTables(IDbConnection connection)
    {
        var createCustomers = @"
            CREATE TABLE customers (
                id SERIAL PRIMARY KEY,
                age INT,
                name VARCHAR(255),
                isactive BOOLEAN DEFAULT true
            )";

        var createProducts = @"
            CREATE TABLE products (
                productid SERIAL PRIMARY KEY,
                productname VARCHAR(255),
                price DECIMAL(18,2),
                createddate TIMESTAMP,
                uniqueid UUID
            )";

        var createOrders = @"
            CREATE TABLE orders (
                id SERIAL PRIMARY KEY,
                customerid INT,
                amount INT,
                FOREIGN KEY (customerid) REFERENCES customers(id) ON DELETE CASCADE
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
        // Note: Using explicit IDs for consistent testing
        var customerTuples = TestDataConstants.Customers
            .Select(c => $"({c.Id}, {c.Age}, '{c.Name}', {c.IsActive.ToString().ToLower()})");
        
        var insertCustomers = $@"
            INSERT INTO customers (id, age, name, isactive) VALUES 
            {string.Join(",\n            ", customerTuples)};
            SELECT setval(pg_get_serial_sequence('customers', 'id'), {TestDataConstants.Customers.Length});";

        var insertProducts = $@"
            INSERT INTO products (productid, productname, price, createddate, uniqueid) VALUES
            {string.Join(",\n            ", TestDataConstants.ProductTuples)};
            SELECT setval(pg_get_serial_sequence('products', 'productid'), {TestDataConstants.Products.Length});";

        var insertOrders = $@"
            INSERT INTO orders (id, customerid, amount) VALUES
            {string.Join(",\n            ", TestDataConstants.OrderTuples)};
            SELECT setval(pg_get_serial_sequence('orders', 'id'), {TestDataConstants.Orders.Length});";

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
        using var connection = new NpgsqlConnection(ConnectionString);
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
