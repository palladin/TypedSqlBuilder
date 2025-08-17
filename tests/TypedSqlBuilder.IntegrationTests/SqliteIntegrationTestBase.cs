using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;

namespace TypedSqlBuilder.IntegrationTests;

/// <summary>
/// Base class for SQLite integration tests with isolated database per test
/// </summary>
public abstract class SqliteIntegrationTestBase : IDisposable
{
    protected SqliteConnection _connection = null!; // Will be initialized in ResetDatabase()
    
    protected SqliteIntegrationTestBase()
    {
        // Each test gets a fresh database connection
        ResetDatabase();
    }
    
    /// <summary>
    /// Reset the database to a clean state - called before each test
    /// </summary>
    protected void ResetDatabase()
    {
        _connection?.Dispose();
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        CreateTestTables();
        SeedTestData();
    }

    /// <summary>
    /// Execute a test within a transaction that gets rolled back
    /// Use this for tests that modify data to ensure isolation
    /// </summary>
    protected void WithTransaction(Action<SqliteConnection> testAction)
    {
        using var transaction = _connection.BeginTransaction();
        try
        {
            testAction(_connection);
        }
        finally
        {
            transaction.Rollback(); // Always rollback to keep data clean
        }
    }

    private void CreateTestTables()
    {
        var createCustomers = @"
            CREATE TABLE customers (
                Id INTEGER PRIMARY KEY,
                Age INTEGER,
                Name TEXT,
                IsActive INTEGER DEFAULT 1
            )";
        
        var createProducts = @"
            CREATE TABLE products (
                ProductId INTEGER PRIMARY KEY,
                ProductName TEXT
            )";
        
        var createOrders = @"
            CREATE TABLE orders (
                OrderId INTEGER PRIMARY KEY,
                CustomerId INTEGER,
                Amount INTEGER,
                FOREIGN KEY (CustomerId) REFERENCES customers(Id)
            )";

        _connection.Execute(createCustomers);
        _connection.Execute(createProducts);
        _connection.Execute(createOrders);
    }

    private void SeedTestData()
    {
        var insertCustomers = @"
            INSERT INTO customers (Id, Age, Name, IsActive) VALUES 
            (1, 25, 'John Doe', 1),
            (2, 30, 'Jane Smith', 1),
            (3, 16, 'Minor User', 0),
            (4, 65, 'Senior User', 1)";

        var insertProducts = @"
            INSERT INTO products (ProductId, ProductName) VALUES
            (1, 'Laptop'),
            (2, 'Mouse'),
            (3, 'Discontinued')";

        var insertOrders = @"
            INSERT INTO orders (OrderId, CustomerId, Amount) VALUES
            (1, 1, 500),
            (2, 1, 150),
            (3, 2, 300),
            (4, 4, 75)";

        _connection.Execute(insertCustomers);
        _connection.Execute(insertProducts);
        _connection.Execute(insertOrders);
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
