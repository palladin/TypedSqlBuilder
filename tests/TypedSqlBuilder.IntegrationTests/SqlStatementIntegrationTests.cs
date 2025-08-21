using TypedSqlBuilder.Core;
using TypedSqlBuilder.TestModels;
using Dapper;
using System.Data;
using Xunit;

namespace TypedSqlBuilder.IntegrationTests;

/// <summary>
/// Integration tests for INSERT, UPDATE, DELETE statements executed against SQL databases using Dapper
/// </summary>
public class SqlStatementIntegrationTests : IClassFixture<SqlFixture>, IStatementTestContract
{
    private readonly SqlFixture _fixture;

    public SqlStatementIntegrationTests(SqlFixture fixture)
    {
        _fixture = fixture;
    }

    #region Helper Methods

    /// <summary>
    /// Get the boolean value for the specified database type
    /// </summary>
    private static string GetBooleanValue(DatabaseType databaseType, bool value = true)
    {
        return databaseType == DatabaseType.PostgreSQL 
            ? (value ? "true" : "false") 
            : (value ? "1" : "0");
    }

    /// <summary>
    /// Clean up both orders and customers tables to ensure test isolation
    /// </summary>
    private static string GetCleanupSql()
    {
        return @"
            DELETE FROM orders;
            DELETE FROM customers;";
    }

    /// <summary>
    /// Generate database-specific INSERT SQL for customers with proper boolean handling
    /// </summary>
    private static string GetCustomerInsertSql(DatabaseType databaseType, params (int Id, string Name, int Age)[] customers)
    {
        var boolValue = GetBooleanValue(databaseType);
        var values = string.Join(",\n                        ", 
            customers.Select(c => $"({c.Id}, '{c.Name}', {c.Age}, {boolValue})"));
        
        return $@"{GetCleanupSql()}
                    INSERT INTO customers (Id, Name, Age, IsActive) VALUES 
                        {values};";
    }

    /// <summary>
    /// Handle database-specific product DTO querying (SQLite stores GUID as string)
    /// </summary>
    private async Task<(int ProductId, string ProductName, decimal? Price, DateTime? CreatedDate, string UniqueId)> 
        QueryProductAsync(IDbConnection connection, IDbTransaction transaction, DatabaseType databaseType, string sql)
    {
        if (databaseType == DatabaseType.SQLite)
        {
            var product = await connection.QueryFirstOrDefaultAsync<SqliteProductDto>(sql, transaction: transaction);
            return (product?.ProductId ?? 0, product?.ProductName ?? "", product?.Price, product?.CreatedDate, product?.UniqueId ?? "");
        }
        else
        {
            var product = await connection.QueryFirstOrDefaultAsync<ProductDto>(sql, transaction: transaction);
            return (product?.ProductId ?? 0, product?.ProductName ?? "", product?.Price, product?.CreatedDate, product?.UniqueId?.ToString() ?? "");
        }
    }

    /// <summary>
    /// Execute a test statement and return the affected rows count
    /// </summary>
    private async Task<int> ExecuteStatementAsync(IDbConnection connection, IDbTransaction transaction, 
        ISqlStatement statement, DatabaseType databaseType)
    {
        var (sql, parameters) = statement.ToSqlRaw(databaseType);
        return await connection.ExecuteAsync(sql, parameters, transaction: transaction);
    }

    #endregion

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task InsertBasic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Act
            var insertedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.InsertBasic(), databaseType);

            // Assert
            Assert.Equal(1, insertedRows);

            // Verify the insert worked
            var insertedCustomer = await connection.QuerySingleAsync<CustomerDto>(
                "SELECT * FROM customers WHERE Id = 200", transaction: transaction);
            Assert.Equal("John Doe", insertedCustomer.Name);
            Assert.Equal(25, insertedCustomer.Age);
        }, databaseType);
    }


    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DeleteAll_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Verify customers exist before deletion
            var initialCustomerCount = await connection.QuerySingleAsync<int>(
                "SELECT COUNT(*) FROM customers", transaction: transaction);
            Assert.True(initialCustomerCount > 0, "Should have customers before deletion");

            // First delete orders to avoid foreign key constraint violations
            await connection.ExecuteAsync("DELETE FROM orders", transaction: transaction);

            // Act - Now delete all customers
            var deletedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.DeleteAll(), databaseType);

            // Assert
            Assert.True(deletedRows > 0, "Should have deleted some rows");
            Assert.Equal(initialCustomerCount, deletedRows);

            // Verify all customers were deleted
            var finalCustomerCount = await connection.QuerySingleAsync<int>(
                "SELECT COUNT(*) FROM customers", transaction: transaction);
            Assert.Equal(0, finalCustomerCount);
        }, databaseType);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DeleteBasic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Verify customer with Id = 200 doesn't exist initially (from our previous test)
            var initialCustomerCount = await connection.QuerySingleOrDefaultAsync<int?>(
                "SELECT COUNT(*) FROM customers WHERE Id = 200", transaction: transaction) ?? 0;
            
            // Insert a test customer with Id = 200 first - use proper boolean values for each database
            var insertSql = $"INSERT INTO customers (Id, Age, Name, IsActive) VALUES (200, 30, 'Test Customer', {GetBooleanValue(databaseType)})";
            await connection.ExecuteAsync(insertSql, transaction: transaction);

            // Verify the customer was inserted
            var customerExists = await connection.QuerySingleAsync<int>(
                "SELECT COUNT(*) FROM customers WHERE Id = 200", transaction: transaction);
            Assert.Equal(1, customerExists);

            // Act
            var deletedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.DeleteBasic(), databaseType);

            // Assert
            Assert.Equal(1, deletedRows);

            // Verify the customer was deleted
            var finalCustomerCount = await connection.QuerySingleAsync<int>(
                "SELECT COUNT(*) FROM customers WHERE Id = 200", transaction: transaction);
            Assert.Equal(0, finalCustomerCount);
        }, databaseType);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DeleteConditional_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            
            // Clean up any test customers in our range first and insert test data
            var setupSql = $@"
                DELETE FROM customers WHERE Age < 18 OR Name = 'Temp';
                INSERT INTO customers (Id, Name, Age, IsActive) VALUES 
                    (500, 'Minor Customer', 17, {GetBooleanValue(databaseType)}),
                    (501, 'Temp', 25, {GetBooleanValue(databaseType)}),
                    (502, 'Adult Customer', 30, {GetBooleanValue(databaseType)}),
                    (503, 'Another Minor', 16, {GetBooleanValue(databaseType)});";
            
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Execute the delete statement
            var rowsAffected = await ExecuteStatementAsync(connection, transaction, TestStatements.DeleteConditional(), databaseType);

            // Should delete customers with Age < 18 OR Name = "Temp"
            // From our test data: customer 500 (age 17), 501 (name "Temp"), and 503 (age 16)
            Assert.Equal(3, rowsAffected);

            // Verify remaining customers
            var verifySql = "SELECT * FROM customers WHERE Id >= 500 AND Id <= 510";
            var remainingCustomers = await connection.QueryAsync<CustomerDto>(verifySql, transaction: transaction);

            var customerList = remainingCustomers.ToList();
            Assert.Single(customerList);
            Assert.Equal(502, customerList[0].Id);
            Assert.Equal("Adult Customer", customerList[0].Name);
            Assert.Equal(30, customerList[0].Age);
        }, databaseType);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task InsertWithNewColumnsNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Act
            var insertedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.InsertWithNewColumnsNull(), databaseType);

            // Assert
            Assert.Equal(1, insertedRows);

            // Verify the insert worked - should have new columns set to NULL
            var insertedProduct = await connection.QuerySingleAsync<ProductDto>(
                "SELECT * FROM products WHERE ProductName = 'Null Test'", transaction: transaction);
            Assert.Equal("Null Test", insertedProduct.ProductName);
            Assert.Equal(201, insertedProduct.ProductId);
            Assert.Null(insertedProduct.Price); // Should be null
            Assert.Null(insertedProduct.CreatedDate); // Should be null
            Assert.Null(insertedProduct.UniqueId); // Should be null
        }, databaseType);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task InsertWithNewColumns_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Act
            var insertedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.InsertWithNewColumns(), databaseType);

            // Assert - should insert 1 row
            Assert.Equal(1, insertedRows);

            // Verify the inserted product (handle database-specific GUID types)
            var insertedProduct = await QueryProductAsync(connection, transaction, databaseType, "SELECT * FROM products WHERE ProductId = 200");
            
            Assert.Equal(200, insertedProduct.ProductId);
            Assert.Equal("Test Product", insertedProduct.ProductName);
            Assert.Equal(99.99m, insertedProduct.Price);
            Assert.Equal(new DateTime(2024, 8, 18), insertedProduct.CreatedDate);
            Assert.Equal("12345678-1234-1234-1234-123456789012", insertedProduct.UniqueId);
        }, databaseType);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task InsertWithNullInt_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Act
            var insertedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.InsertWithNullInt(), databaseType);

            // Assert - should insert 1 row
            Assert.Equal(1, insertedRows);

            // Verify the inserted customer
            var insertedCustomer = await connection.QueryFirstOrDefaultAsync<CustomerDto>(
                "SELECT * FROM customers WHERE Id = 203", transaction: transaction);

            Assert.NotNull(insertedCustomer);
            Assert.Equal(203, insertedCustomer.Id);
            Assert.Equal("John", insertedCustomer.Name);
            Assert.Null(insertedCustomer.Age); // Age should be null
        }, databaseType);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task InsertWithNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Act
            var insertedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.InsertWithNull(), databaseType);

            // Assert - should insert 1 row
            Assert.Equal(1, insertedRows);

            // Verify the inserted customer
            var insertedCustomer = await connection.QueryFirstOrDefaultAsync<CustomerDto>(
                "SELECT * FROM customers WHERE Id = 202", transaction: transaction);

            Assert.NotNull(insertedCustomer);
            Assert.Equal(202, insertedCustomer.Id);
            Assert.Null(insertedCustomer.Name); // Name should be null
            Assert.Equal(25, insertedCustomer.Age);
        }, databaseType);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task UpdateBasic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - setup test data first
            var setupSql = $"INSERT INTO customers (Id, Name, Age, IsActive) VALUES (200, 'Test User', 25, {GetBooleanValue(databaseType)})";
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateBasic(), databaseType);

            // Assert - should update 1 row
            Assert.Equal(1, updatedRows);

            // Verify the update worked
            var updatedCustomer = await connection.QueryFirstOrDefaultAsync<CustomerDto>(
                "SELECT * FROM customers WHERE Id = 200", transaction: transaction);

            Assert.NotNull(updatedCustomer);
            Assert.Equal(200, updatedCustomer.Id);
            Assert.Equal("Test User", updatedCustomer.Name); // Name should remain unchanged
            Assert.Equal(26, updatedCustomer.Age); // Age should be updated to 26
        }, databaseType);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task UpdateConditional_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - setup controlled test data
            var setupSql = GetCustomerInsertSql(databaseType, 
                (600, "Adult", 25),
                (601, "Minor", 16), 
                (602, "Admin", 30), 
                (603, "Young Adult", 18));
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateConditional(), databaseType);

            // Assert - should update only records where Age >= 18 AND Name != 'Admin'
            // From test data: customers 600 (Adult, 25) and 603 (Young Adult, 18) should be updated
            Assert.Equal(2, updatedRows);

            // Verify the updates worked correctly
            var customers = (await connection.QueryAsync<CustomerDto>("SELECT * FROM customers ORDER BY Id", transaction: transaction)).ToList();

            Assert.Equal(4, customers.Count);
            
            // Customer 600: Adult, should be updated from 25 to 26
            Assert.Equal(600, customers[0].Id);
            Assert.Equal("Adult", customers[0].Name);
            Assert.Equal(26, customers[0].Age);
            
            // Customer 601: Minor (age 16), should NOT be updated
            Assert.Equal(601, customers[1].Id);
            Assert.Equal("Minor", customers[1].Name);
            Assert.Equal(16, customers[1].Age); // Should remain 16
            
            // Customer 602: Admin (excluded by name), should NOT be updated
            Assert.Equal(602, customers[2].Id);
            Assert.Equal("Admin", customers[2].Name);
            Assert.Equal(30, customers[2].Age); // Should remain 30
            
            // Customer 603: Young Adult (exactly 18), should be updated from 18 to 19
            Assert.Equal(603, customers[3].Id);
            Assert.Equal("Young Adult", customers[3].Name);
            Assert.Equal(19, customers[3].Age);
        }, databaseType);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task UpdateMultiple_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - setup test data first
            var setupSql = $"INSERT INTO customers (Id, Name, Age, IsActive) VALUES (200, 'Original Name', 25, {GetBooleanValue(databaseType)})";
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateMultiple(), databaseType);

            // Assert - should update 1 row
            Assert.Equal(1, updatedRows);

            // Verify the update worked
            var updatedCustomer = await connection.QueryFirstOrDefaultAsync<CustomerDto>(
                "SELECT * FROM customers WHERE Id = 200", transaction: transaction);

            Assert.NotNull(updatedCustomer);
            Assert.Equal(200, updatedCustomer.Id);
            Assert.Equal("John Smith", updatedCustomer.Name); // Name should be updated to "John Smith"
            Assert.Equal(27, updatedCustomer.Age); // Age should be updated to 27
        }, databaseType);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task UpdateSetNewColumnsNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - setup test data first with some values in the new columns
            var setupSql = "INSERT INTO products (ProductId, ProductName, Price, CreatedDate, UniqueId) VALUES (101, 'Test Product', 99.99, '2024-01-01', '11111111-2222-3333-4444-555555555555')";
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateSetNewColumnsNull(), databaseType);

            // Assert - should update 1 row
            Assert.Equal(1, updatedRows);

            // Verify the update worked (handle database-specific GUID types)
            if (databaseType == DatabaseType.SQLite)
            {
                var updatedProduct = await connection.QueryFirstOrDefaultAsync<SqliteProductDto>("SELECT * FROM products WHERE ProductId = 101", transaction: transaction);
                Assert.NotNull(updatedProduct);
                Assert.Equal(101, updatedProduct.ProductId);
                Assert.Equal("Test Product", updatedProduct.ProductName); // Should remain unchanged
                Assert.Null(updatedProduct.Price); // Should be set to NULL
                Assert.Null(updatedProduct.CreatedDate); // Should be set to NULL
                Assert.Null(updatedProduct.UniqueId); // Should be set to NULL
            }
            else
            {
                var updatedProduct = await connection.QueryFirstOrDefaultAsync<ProductDto>("SELECT * FROM products WHERE ProductId = 101", transaction: transaction);
                Assert.NotNull(updatedProduct);
                Assert.Equal(101, updatedProduct.ProductId);
                Assert.Equal("Test Product", updatedProduct.ProductName); // Should remain unchanged
                Assert.Null(updatedProduct.Price); // Should be set to NULL
                Assert.Null(updatedProduct.CreatedDate); // Should be set to NULL
                Assert.Null(updatedProduct.UniqueId); // Should be set to NULL
            }
        }, databaseType);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task UpdateSetNullInt_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - setup controlled test data first
            var cleanupSql = GetCleanupSql();
            var setupSql = $@"{cleanupSql}
                INSERT INTO customers (Id, Name, Age, IsActive) VALUES 
                    (700, 'Customer 1', 25, {GetBooleanValue(databaseType)}),
                    (701, 'Customer 2', 30, {GetBooleanValue(databaseType)}),
                    (702, 'Customer 3', 35, {GetBooleanValue(databaseType)});";
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateSetNullInt(), databaseType);

            // Assert - should update all 3 customers
            Assert.Equal(3, updatedRows);

            // Verify all customers have Age set to NULL
            var customers = (await connection.QueryAsync<CustomerDto>("SELECT * FROM customers ORDER BY Id", transaction: transaction)).ToList();

            Assert.Equal(3, customers.Count);
            
            Assert.Equal(700, customers[0].Id);
            Assert.Equal("Customer 1", customers[0].Name);
            Assert.Null(customers[0].Age); // Should be set to NULL
            
            Assert.Equal(701, customers[1].Id);
            Assert.Equal("Customer 2", customers[1].Name);
            Assert.Null(customers[1].Age); // Should be set to NULL
            
            Assert.Equal(702, customers[2].Id);
            Assert.Equal("Customer 3", customers[2].Name);
            Assert.Null(customers[2].Age); // Should be set to NULL
        }, databaseType);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task UpdateSetNullMixed_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - setup controlled test data first
            var cleanupSql = GetCleanupSql();
            var setupSql = $@"{cleanupSql}
                INSERT INTO customers (Id, Name, Age, IsActive) VALUES 
                    (800, 'Original Name 1', 25, {GetBooleanValue(databaseType)}),
                    (801, 'Original Name 2', 30, {GetBooleanValue(databaseType)}),
                    (802, 'Original Name 3', 35, {GetBooleanValue(databaseType)});";
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateSetNullMixed(), databaseType);

            // Assert - should update all 3 customers
            Assert.Equal(3, updatedRows);

            // Verify all customers have Name="John" and Age=NULL
            var customers = (await connection.QueryAsync<CustomerDto>("SELECT * FROM customers ORDER BY Id", transaction: transaction)).ToList();

            Assert.Equal(3, customers.Count);
            
            Assert.Equal(800, customers[0].Id);
            Assert.Equal("John", customers[0].Name); // Should be updated to "John"
            Assert.Null(customers[0].Age); // Should be set to NULL
            
            Assert.Equal(801, customers[1].Id);
            Assert.Equal("John", customers[1].Name); // Should be updated to "John"
            Assert.Null(customers[1].Age); // Should be set to NULL
            
            Assert.Equal(802, customers[2].Id);
            Assert.Equal("John", customers[2].Name); // Should be updated to "John"
            Assert.Null(customers[2].Age); // Should be set to NULL
        }, databaseType);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task UpdateSetNullWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - setup test data first
            var setupSql = $"INSERT INTO customers (Id, Name, Age, IsActive) VALUES (200, 'Original Name', 25, {GetBooleanValue(databaseType)})";
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateSetNullWhere(), databaseType);

            // Assert - should update 1 row
            Assert.Equal(1, updatedRows);

            // Verify the update worked
            var verifySql = "SELECT * FROM customers WHERE Id = 200";
            var updatedCustomer = await connection.QueryFirstOrDefaultAsync<CustomerDto>(verifySql, transaction: transaction);

            Assert.NotNull(updatedCustomer);
            Assert.Equal(200, updatedCustomer.Id);
            Assert.Null(updatedCustomer.Name); // Name should be set to NULL
            Assert.Equal(25, updatedCustomer.Age); // Age should remain unchanged
        }, databaseType);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task UpdateSetNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - setup controlled test data first
            var cleanupSql = GetCleanupSql();
            var setupSql = $@"{cleanupSql}
                INSERT INTO customers (Id, Name, Age, IsActive) VALUES 
                    (900, 'Customer A', 25, {GetBooleanValue(databaseType)}),
                    (901, 'Customer B', 30, {GetBooleanValue(databaseType)}),
                    (902, 'Customer C', 35, {GetBooleanValue(databaseType)});";
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateSetNull(), databaseType);

            // Assert - should update all 3 customers
            Assert.Equal(3, updatedRows);

            // Verify all customers have Name set to NULL
            var customers = (await connection.QueryAsync<CustomerDto>("SELECT * FROM customers ORDER BY Id", transaction: transaction)).ToList();

            Assert.Equal(3, customers.Count);
            
            Assert.Equal(900, customers[0].Id);
            Assert.Null(customers[0].Name); // Should be set to NULL
            Assert.Equal(25, customers[0].Age); // Should remain unchanged
            
            Assert.Equal(901, customers[1].Id);
            Assert.Null(customers[1].Name); // Should be set to NULL
            Assert.Equal(30, customers[1].Age); // Should remain unchanged
            
            Assert.Equal(902, customers[2].Id);
            Assert.Null(customers[2].Name); // Should be set to NULL
            Assert.Equal(35, customers[2].Age); // Should remain unchanged
        }, databaseType);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task UpdateWithNewColumns_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - setup test data first with some initial values in the new columns
            var setupSql = "INSERT INTO products (ProductId, ProductName, Price, CreatedDate, UniqueId) VALUES (100, 'Test Product', 50.00, '2024-01-01', '00000000-1111-2222-3333-444444444444')";
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateWithNewColumns(), databaseType);

            // Assert - should update 1 row
            Assert.Equal(1, updatedRows);

            // Verify the update worked (handle database-specific GUID types)
            var updatedProduct = await QueryProductAsync(connection, transaction, databaseType, "SELECT * FROM products WHERE ProductId = 100");

            Assert.Equal(100, updatedProduct.ProductId);
            Assert.Equal("Test Product", updatedProduct.ProductName); // Should remain unchanged
            Assert.Equal(119.99m, updatedProduct.Price); // Should be updated
            Assert.Equal(new DateTime(2024, 12, 25), updatedProduct.CreatedDate); // Should be updated
            Assert.Equal("87654321-4321-4321-4321-210987654321", updatedProduct.UniqueId); // Should be updated
        }, databaseType);
    }
}
