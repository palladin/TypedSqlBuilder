using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Dapper;
using Testcontainers.MsSql;
using Testcontainers.Xunit;
using Xunit.Abstractions;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

namespace TypedSqlBuilder.IntegrationTests;

/// <summary>
/// Base class for SQL Server integration tests using Testcontainers
/// Provides automatic SQL Server container management and database setup
/// </summary>
public abstract class SqlServerIntegrationTestBase : DbContainerTest<MsSqlBuilder, MsSqlContainer>
{
    protected SqlServerIntegrationTestBase(ITestOutputHelper testOutputHelper) 
        : base(testOutputHelper) { }

    /// <summary>
    /// Specifies the SQL Server DbProviderFactory for creating connections
    /// </summary>
    public override DbProviderFactory DbProviderFactory => SqlClientFactory.Instance;

    /// <summary>
    /// Configures the SQL Server container for testing
    /// </summary>
    protected override MsSqlBuilder Configure(MsSqlBuilder builder)
    {
        return builder
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest") // Use 2022 latest
            .WithPassword("Test123!Strong") // Strong password required by SQL Server
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("MSSQL_PID", "Express"); // Use Express edition for lower resource usage
    }

    /// <summary>
    /// Initializes the test database with schema and test data
    /// Called automatically when container starts
    /// </summary>
    protected override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await SetupTestDatabase();
    }

    /// <summary>
    /// Sets up the test database schema and seeds it with test data
    /// </summary>
    private async Task SetupTestDatabase()
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();

        // Create test tables with SQL Server syntax
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
                Name NVARCHAR(255)
            )";

        var createProducts = @"
            CREATE TABLE products (
                ProductId INT IDENTITY(1,1) PRIMARY KEY,
                ProductName NVARCHAR(255)
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
        var insertCustomers = @"
            SET IDENTITY_INSERT customers ON;
            INSERT INTO customers (Id, Age, Name) VALUES 
            (1, 25, N'John Doe'),
            (2, 30, N'Jane Smith'),
            (3, 16, N'Minor User'),
            (4, 65, N'Senior User');
            SET IDENTITY_INSERT customers OFF;";

        var insertProducts = @"
            SET IDENTITY_INSERT products ON;
            INSERT INTO products (ProductId, ProductName) VALUES
            (1, N'Laptop'),
            (2, N'Mouse'),
            (3, N'Discontinued');
            SET IDENTITY_INSERT products OFF;";

        var insertOrders = @"
            SET IDENTITY_INSERT orders ON;
            INSERT INTO orders (OrderId, CustomerId, Amount) VALUES
            (1, 1, 500),
            (2, 1, 150),
            (3, 2, 300),
            (4, 4, 75);
            SET IDENTITY_INSERT orders OFF;";

        await connection.ExecuteAsync(insertCustomers);
        await connection.ExecuteAsync(insertProducts);
        await connection.ExecuteAsync(insertOrders);
    }

    /// <summary>
    /// Execute a test within a transaction that gets rolled back
    /// Use this for tests that modify data to ensure isolation
    /// </summary>
    protected async Task WithTransactionAsync(Func<IDbConnection, IDbTransaction, Task> testAction)
    {
        using var connection = CreateConnection();
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
    protected void WithTransaction(Action<IDbConnection, IDbTransaction> testAction)
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
