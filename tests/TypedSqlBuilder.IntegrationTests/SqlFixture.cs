using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Dapper;
using Testcontainers.MsSql;
using Xunit;
using TypedSqlBuilder.Core;
using Testcontainers.PostgreSql;
using Npgsql;
using Microsoft.Data.Sqlite;

namespace TypedSqlBuilder.IntegrationTests;

/// <summary>
/// Fixture for SQL integration tests using Testcontainers
/// This creates one container that is shared across all tests in the class
/// </summary>
public class SqlFixture : IAsyncLifetime
{
    private MsSqlContainer? _sqlServerContainer;
    private PostgreSqlContainer? _postgresContainer;
    private readonly string _sqliteConnectionString = $"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared";

    /// <summary>
    /// Creates a new database connection
    /// </summary>
    public IDbConnection CreateConnection(DatabaseType databaseType) =>
        databaseType switch
        {
            DatabaseType.SqlServer => new SqlConnection(_sqlServerContainer?.GetConnectionString() ?? throw new InvalidOperationException("Container not initialized")),
            DatabaseType.PostgreSQL => new NpgsqlConnection(_postgresContainer?.GetConnectionString() ?? throw new InvalidOperationException("Container not initialized")),
            DatabaseType.SQLite => new SqliteConnection(_sqliteConnectionString),
            _ => throw new NotSupportedException($"Database type {databaseType} is not supported.")
        };

    /// <summary>
    /// Initializes the SQL container and sets up the test database
    /// </summary>
    public async Task InitializeAsync()
    {
        _sqlServerContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest") // Use 2022 latest
            .WithPassword("Test123!Strong") // Strong password required by SQL Server
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("MSSQL_PID", "Express") // Use Express edition for lower resource usage
            .Build();

        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine") // Use PostgreSQL 16 Alpine for fast startup
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .Build();

        await _sqlServerContainer.StartAsync();
        await _postgresContainer.StartAsync();
        await SetupTestDatabase(DatabaseType.SqlServer);
        await SetupTestDatabase(DatabaseType.PostgreSQL);
        await SetupTestDatabase(DatabaseType.SQLite);
    }

    /// <summary>
    /// Disposes the SQL container
    /// </summary>
    public async Task DisposeAsync()
    {
        if (_sqlServerContainer != null)
        {
            await _sqlServerContainer.DisposeAsync();
        }
    }

    /// <summary>
    /// Sets up the test database schema and seeds it with test data
    /// </summary>
    private async Task SetupTestDatabase(DatabaseType databaseType)
    {
        using var connection = CreateConnection(databaseType);
        connection.Open();
        
        await CreateTestTables(connection, databaseType);
        await SeedTestData(connection, databaseType);
    }

    /// <summary>
    /// Creates the test tables in the SQL database
    /// </summary>
    private async Task CreateTestTables(IDbConnection connection, DatabaseType databaseType)
    {
        string createCustomers, createProducts, createOrders;

        switch (databaseType)
        {
            case DatabaseType.SqlServer:
                createCustomers = @"
                    CREATE TABLE customers (
                        Id INT PRIMARY KEY,
                        Age INT,
                        Name NVARCHAR(255),
                        IsActive BIT DEFAULT 1
                    )";

                createProducts = @"
                    CREATE TABLE products (
                        Id INT PRIMARY KEY,
                        ProductName NVARCHAR(255),
                        Price DECIMAL(18,2),
                        CreatedDate DATETIME2,
                        UniqueId UNIQUEIDENTIFIER
                    )";

                createOrders = @"
                    CREATE TABLE orders (
                        Id INT PRIMARY KEY,
                        CustomerId INT,
                        ProductId INT,
                        Amount INT,
                        FOREIGN KEY (CustomerId) REFERENCES customers(Id),
                        FOREIGN KEY (ProductId) REFERENCES products(Id)
                    )";
                break;

            case DatabaseType.PostgreSQL:
                createCustomers = @"
                    CREATE TABLE customers (
                        Id INT PRIMARY KEY,
                        Age INT,
                        Name VARCHAR(255),
                        IsActive BOOLEAN DEFAULT TRUE
                    )";

                createProducts = @"
                    CREATE TABLE products (
                        Id INT PRIMARY KEY,
                        ProductName VARCHAR(255),
                        Price DECIMAL(18,2),
                        CreatedDate TIMESTAMP,
                        UniqueId UUID
                    )";

                createOrders = @"
                    CREATE TABLE orders (
                        Id INT PRIMARY KEY,
                        CustomerId INT,
                        ProductId INT,
                        Amount INT,
                        FOREIGN KEY (CustomerId) REFERENCES customers(Id),
                        FOREIGN KEY (ProductId) REFERENCES products(Id)
                    )";
                break;

            case DatabaseType.SQLite:
                createCustomers = @"
                    CREATE TABLE customers (
                        Id INTEGER PRIMARY KEY,
                        Age INTEGER,
                        Name TEXT,
                        IsActive INTEGER DEFAULT 1
                    )";

                createProducts = @"
                    CREATE TABLE products (
                        Id INTEGER PRIMARY KEY,
                        ProductName TEXT,
                        Price REAL,
                        CreatedDate TEXT,
                        UniqueId TEXT
                    )";

                createOrders = @"
                    CREATE TABLE orders (
                        Id INTEGER PRIMARY KEY,
                        CustomerId INTEGER,
                        ProductId INTEGER,
                        Amount INTEGER,
                        FOREIGN KEY (CustomerId) REFERENCES customers(Id),
                        FOREIGN KEY (ProductId) REFERENCES products(Id)
                    )";
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
        }

        await connection.ExecuteAsync(createCustomers);
        await connection.ExecuteAsync(createProducts);
        await connection.ExecuteAsync(createOrders);
    }

    /// <summary>
    /// Seeds the test database with sample data
    /// </summary>
    private async Task SeedTestData(IDbConnection connection, DatabaseType databaseType)
    {
        string customerTuples, productTuples;

        switch (databaseType)
        {
            case DatabaseType.SqlServer:
                customerTuples = string.Join(",\n            ", TestDataConstants.Customers
                    .Select(c => $"({c.Id}, {c.Age}, N'{c.Name}', {(c.IsActive ? 1 : 0)})"));

                productTuples = string.Join(",\n            ", TestDataConstants.Products
                    .Select(p => $"({p.Id}, N'{p.ProductName}', " +
                               $"{(p.Price.HasValue ? p.Price.Value.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) : "NULL")}, " +
                               $"{(p.CreatedDate.HasValue ? $"'{p.CreatedDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")}, " +
                               $"{(p.UniqueId.HasValue ? $"'{p.UniqueId.Value}'" : "NULL")})"));
                break;

            case DatabaseType.PostgreSQL:
                customerTuples = string.Join(",\n            ", TestDataConstants.Customers
                    .Select(c => $"({c.Id}, {c.Age}, '{c.Name}', {(c.IsActive ? "TRUE" : "FALSE")})"));

                productTuples = string.Join(",\n            ", TestDataConstants.Products
                    .Select(p => $"({p.Id}, '{p.ProductName}', " +
                               $"{(p.Price.HasValue ? p.Price.Value.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) : "NULL")}, " +
                               $"{(p.CreatedDate.HasValue ? $"'{p.CreatedDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")}, " +
                               $"{(p.UniqueId.HasValue ? $"'{p.UniqueId.Value}'" : "NULL")})"));
                break;

            case DatabaseType.SQLite:
                customerTuples = string.Join(",\n            ", TestDataConstants.Customers
                    .Select(c => $"({c.Id}, {c.Age}, '{c.Name}', {(c.IsActive ? 1 : 0)})"));

                productTuples = string.Join(",\n            ", TestDataConstants.Products
                    .Select(p => $"({p.Id}, '{p.ProductName}', " +
                               $"{(p.Price.HasValue ? p.Price.Value.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) : "NULL")}, " +
                               $"{(p.CreatedDate.HasValue ? $"'{p.CreatedDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")}, " +
                               $"{(p.UniqueId.HasValue ? $"'{p.UniqueId.Value}'" : "NULL")})"));
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
        }

        var insertCustomers = $@"            
            INSERT INTO customers (Id, Age, Name, IsActive) VALUES 
            {customerTuples}";

        var insertProducts = $@"            
            INSERT INTO products (Id, ProductName, Price, CreatedDate, UniqueId) VALUES
            {productTuples}";

        var insertOrders = $@"            
            INSERT INTO orders (Id, CustomerId, ProductId, Amount) VALUES
            {string.Join(",\n            ", TestDataConstants.OrderTuples)}";

        await connection.ExecuteAsync(insertCustomers);
        await connection.ExecuteAsync(insertProducts);
        await connection.ExecuteAsync(insertOrders);
    }

    /// <summary>
    /// Execute a test within a transaction that gets rolled back
    /// Use this for tests that modify data to ensure isolation
    /// </summary>
    public async Task WithTransactionAsync(Func<IDbConnection, IDbTransaction, Task> testAction, DatabaseType databaseType)
    {
        using var connection = CreateConnection(databaseType);
        connection.Open();
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
    public void WithTransaction(Action<IDbConnection, IDbTransaction> testAction, DatabaseType databaseType)
    {
        using var connection = CreateConnection(databaseType);
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
