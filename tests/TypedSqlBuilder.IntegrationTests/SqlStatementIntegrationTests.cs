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
    /// Get database-specific SELECT customer by ID SQL
    /// </summary>
    private static string GetSelectCustomerByIdSql(DatabaseType databaseType, int id)
    {
        return databaseType switch
        {
            DatabaseType.SqlServer => $"SELECT * FROM [customers] WHERE [Id] = {id}",
            DatabaseType.PostgreSQL or DatabaseType.SQLite => $"SELECT * FROM \"customers\" WHERE \"Id\" = {id}",
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
        };
    }

    /// <summary>
    /// Get database-specific SELECT all customers SQL
    /// </summary>
    private static string GetSelectAllCustomersSql(DatabaseType databaseType)
    {
        return databaseType switch
        {
            DatabaseType.SqlServer => "SELECT * FROM [customers] ORDER BY [Id]",
            DatabaseType.PostgreSQL or DatabaseType.SQLite => "SELECT * FROM \"customers\" ORDER BY \"Id\"",
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
        };
    }

    /// <summary>
    /// Get database-specific COUNT customers SQL
    /// </summary>
    private static string GetCountCustomersSql(DatabaseType databaseType)
    {
        return databaseType switch
        {
            DatabaseType.SqlServer => "SELECT COUNT(*) FROM [customers]",
            DatabaseType.PostgreSQL or DatabaseType.SQLite => "SELECT COUNT(*) FROM \"customers\"",
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
        };
    }

    /// <summary>
    /// Get database-specific COUNT customers by ID SQL
    /// </summary>
    private static string GetCountCustomersByIdSql(DatabaseType databaseType, int id)
    {
        return databaseType switch
        {
            DatabaseType.SqlServer => $"SELECT COUNT(*) FROM [customers] WHERE [Id] = {id}",
            DatabaseType.PostgreSQL or DatabaseType.SQLite => $"SELECT COUNT(*) FROM \"customers\" WHERE \"Id\" = {id}",
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
        };
    }

    /// <summary>
    /// Get database-specific DELETE FROM orders SQL
    /// </summary>
    private static string GetDeleteOrdersSql(DatabaseType databaseType)
    {
        return databaseType switch
        {
            DatabaseType.SqlServer => "DELETE FROM [orders]",
            DatabaseType.PostgreSQL or DatabaseType.SQLite => "DELETE FROM \"orders\"",
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
        };
    }

    /// <summary>
    /// Get database-specific INSERT customer SQL
    /// </summary>
    private static string GetInsertCustomerSql(DatabaseType databaseType, int id, string name, int age, bool isActive = true)
    {
        var boolValue = GetBooleanValue(databaseType, isActive);
        return databaseType switch
        {
            DatabaseType.SqlServer => $"INSERT INTO [customers] ([Id], [Name], [Age], [IsActive]) VALUES ({id}, '{name}', {age}, {boolValue})",
            DatabaseType.PostgreSQL or DatabaseType.SQLite => $"INSERT INTO \"customers\" (\"Id\", \"Name\", \"Age\", \"IsActive\") VALUES ({id}, '{name}', {age}, {boolValue})",
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
        };
    }

    /// <summary>
    /// Get database-specific SELECT products SQL
    /// </summary>
    private static string GetSelectProductsSql(DatabaseType databaseType, string whereClause = "")
    {
        var where = string.IsNullOrEmpty(whereClause) ? "" : $" WHERE {whereClause}";
        return databaseType switch
        {
            DatabaseType.SqlServer => $"SELECT * FROM [products]{where}",
            DatabaseType.PostgreSQL or DatabaseType.SQLite => $"SELECT * FROM \"products\"{where}",
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
        };
    }

    /// <summary>
    /// Get database-specific DELETE conditional SQL for the conditional delete test
    /// </summary>
    private static string GetDeleteConditionalSetupSql(DatabaseType databaseType)
    {
        var boolValue = GetBooleanValue(databaseType);
        return databaseType switch
        {
            DatabaseType.SqlServer => $@"
                DELETE FROM [customers] WHERE [Age] < 18 OR [Name] = 'Temp';
                INSERT INTO [customers] ([Id], [Name], [Age], [IsActive]) VALUES 
                    (500, 'Minor Customer', 17, {boolValue}),
                    (501, 'Temp', 25, {boolValue}),
                    (502, 'Adult Customer', 30, {boolValue}),
                    (503, 'Another Minor', 16, {boolValue});",
            DatabaseType.PostgreSQL or DatabaseType.SQLite => $@"
                DELETE FROM ""customers"" WHERE ""Age"" < 18 OR ""Name"" = 'Temp';
                INSERT INTO ""customers"" (""Id"", ""Name"", ""Age"", ""IsActive"") VALUES 
                    (500, 'Minor Customer', 17, {boolValue}),
                    (501, 'Temp', 25, {boolValue}),
                    (502, 'Adult Customer', 30, {boolValue}),
                    (503, 'Another Minor', 16, {boolValue});",
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
        };
    }

    /// <summary>
    /// Get database-specific SELECT customers in range SQL
    /// </summary>
    private static string GetSelectCustomersInRangeSql(DatabaseType databaseType, int minId, int maxId)
    {
        return databaseType switch
        {
            DatabaseType.SqlServer => $"SELECT * FROM [customers] WHERE [Id] >= {minId} AND [Id] <= {maxId}",
            DatabaseType.PostgreSQL or DatabaseType.SQLite => $"SELECT * FROM \"customers\" WHERE \"Id\" >= {minId} AND \"Id\" <= {maxId}",
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
        };
    }

    /// <summary>
    /// Get database-specific SELECT product by name SQL
    /// </summary>
    private static string GetSelectProductByNameSql(DatabaseType databaseType, string productName)
    {
        return databaseType switch
        {
            DatabaseType.SqlServer => $"SELECT * FROM [products] WHERE [ProductName] = '{productName}'",
            DatabaseType.PostgreSQL or DatabaseType.SQLite => $"SELECT * FROM \"products\" WHERE \"ProductName\" = '{productName}'",
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
        };
    }

    /// <summary>
    /// Get database-specific SELECT product by ID SQL
    /// </summary>
    private static string GetSelectProductByIdSql(DatabaseType databaseType, int id)
    {
        return databaseType switch
        {
            DatabaseType.SqlServer => $"SELECT * FROM [products] WHERE [Id] = {id}",
            DatabaseType.PostgreSQL or DatabaseType.SQLite => $"SELECT * FROM \"products\" WHERE \"Id\" = {id}",
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
        };
    }

    /// <summary>
    /// Get database-specific INSERT product SQL
    /// </summary>
    private static string GetInsertProductSql(DatabaseType databaseType, int id, string productName, 
        decimal? price = null, DateTime? createdDate = null, string? uniqueId = null)
    {
        var priceValue = price?.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) ?? "NULL";
        var dateValue = createdDate?.ToString("yyyy-MM-dd") ?? "NULL";
        var guidValue = uniqueId != null ? $"'{uniqueId}'" : "NULL";
        
        if (createdDate.HasValue && dateValue != "NULL")
        {
            dateValue = $"'{dateValue}'";
        }

        return databaseType switch
        {
            DatabaseType.SqlServer => $"INSERT INTO [products] ([Id], [ProductName], [Price], [CreatedDate], [UniqueId]) VALUES ({id}, '{productName}', {priceValue}, {dateValue}, {guidValue})",
            DatabaseType.PostgreSQL or DatabaseType.SQLite => $"INSERT INTO \"products\" (\"Id\", \"ProductName\", \"Price\", \"CreatedDate\", \"UniqueId\") VALUES ({id}, '{productName}', {priceValue}, {dateValue}, {guidValue})",
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
        };
    }

    /// <summary>
    /// Clean up both orders and customers tables to ensure test isolation
    /// </summary>
    private static string GetCleanupSql(DatabaseType databaseType)
    {
        return databaseType switch
        {
            DatabaseType.SqlServer => @"
                DELETE FROM [orders];
                DELETE FROM [customers];",
            DatabaseType.PostgreSQL or DatabaseType.SQLite => @"
                DELETE FROM ""orders"";
                DELETE FROM ""customers"";",
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
        };
    }

    /// <summary>
    /// Generate database-specific INSERT SQL for customers with proper boolean handling
    /// </summary>
    private static string GetCustomerInsertSql(DatabaseType databaseType, params (int Id, string Name, int Age)[] customers)
    {
        var boolValue = GetBooleanValue(databaseType);
        var values = string.Join(",\n                        ", 
            customers.Select(c => $"({c.Id}, '{c.Name}', {c.Age}, {boolValue})"));
        
        var insertSql = databaseType switch
        {
            DatabaseType.SqlServer => $@"INSERT INTO [customers] ([Id], [Name], [Age], [IsActive]) VALUES 
                        {values};",
            DatabaseType.PostgreSQL or DatabaseType.SQLite => $@"INSERT INTO ""customers"" (""Id"", ""Name"", ""Age"", ""IsActive"") VALUES 
                        {values};",
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
        };

        return $@"{GetCleanupSql(databaseType)}
                    {insertSql}";
    }

    /// <summary>
    /// Handle database-specific product DTO querying (SQLite stores GUID as string)
    /// </summary>
    private async Task<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, string UniqueId)> 
        QueryProductAsync(IDbConnection connection, IDbTransaction transaction, DatabaseType databaseType, string sql)
    {
        if (databaseType == DatabaseType.SQLite)
        {
            var product = await connection.QueryFirstOrDefaultAsync<SqliteProductDto>(sql, transaction: transaction);
            return (product?.Id ?? 0, product?.ProductName ?? "", product?.Price, product?.CreatedDate, product?.UniqueId ?? "");
        }
        else
        {
            var product = await connection.QueryFirstOrDefaultAsync<ProductDto>(sql, transaction: transaction);
            return (product?.Id ?? 0, product?.ProductName ?? "", product?.Price, product?.CreatedDate, product?.UniqueId?.ToString() ?? "");
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
                GetSelectCustomerByIdSql(databaseType, 200), transaction: transaction);
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
                GetCountCustomersSql(databaseType), transaction: transaction);
            Assert.True(initialCustomerCount > 0, "Should have customers before deletion");

            // First delete orders to avoid foreign key constraint violations
            await connection.ExecuteAsync(GetDeleteOrdersSql(databaseType), transaction: transaction);

            // Act - Now delete all customers
            var deletedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.DeleteAll(), databaseType);

            // Assert
            Assert.True(deletedRows > 0, "Should have deleted some rows");
            Assert.Equal(initialCustomerCount, deletedRows);

            // Verify all customers were deleted
            var finalCustomerCount = await connection.QuerySingleAsync<int>(
                GetCountCustomersSql(databaseType), transaction: transaction);
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
                GetCountCustomersByIdSql(databaseType, 200), transaction: transaction) ?? 0;
            
            // Insert a test customer with Id = 200 first - use proper boolean values for each database
            var insertSql = GetInsertCustomerSql(databaseType, 200, "Test Customer", 30);
            await connection.ExecuteAsync(insertSql, transaction: transaction);

            // Verify the customer was inserted
            var customerExists = await connection.QuerySingleAsync<int>(
                GetCountCustomersByIdSql(databaseType, 200), transaction: transaction);
            Assert.Equal(1, customerExists);

            // Act
            var deletedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.DeleteBasic(), databaseType);

            // Assert
            Assert.Equal(1, deletedRows);

            // Verify the customer was deleted
            var finalCustomerCount = await connection.QuerySingleAsync<int>(
                GetCountCustomersByIdSql(databaseType, 200), transaction: transaction);
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
            var setupSql = GetDeleteConditionalSetupSql(databaseType);
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Execute the delete statement
            var rowsAffected = await ExecuteStatementAsync(connection, transaction, TestStatements.DeleteConditional(), databaseType);

            // Should delete customers with Age < 18 OR Name = "Temp"
            // From our test data: customer 500 (age 17), 501 (name "Temp"), and 503 (age 16)
            Assert.Equal(3, rowsAffected);

            // Verify remaining customers
            var verifySql = GetSelectCustomersInRangeSql(databaseType, 500, 510);
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
                GetSelectProductByNameSql(databaseType, "Null Test"), transaction: transaction);
            Assert.Equal("Null Test", insertedProduct.ProductName);
            Assert.Equal(201, insertedProduct.Id);
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
            var insertedProduct = await QueryProductAsync(connection, transaction, databaseType, 
                GetSelectProductByIdSql(databaseType, 200));
            
            Assert.Equal(200, insertedProduct.Id);
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
                GetSelectCustomerByIdSql(databaseType, 203), transaction: transaction);

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
                GetSelectCustomerByIdSql(databaseType, 202), transaction: transaction);

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
            var setupSql = GetInsertCustomerSql(databaseType, 200, "Test User", 25);
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateBasic(), databaseType);

            // Assert - should update 1 row
            Assert.Equal(1, updatedRows);

            // Verify the update worked
            var updatedCustomer = await connection.QueryFirstOrDefaultAsync<CustomerDto>(
                GetSelectCustomerByIdSql(databaseType, 200), transaction: transaction);

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
            var customers = (await connection.QueryAsync<CustomerDto>(GetSelectAllCustomersSql(databaseType), transaction: transaction)).ToList();

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
            var setupSql = GetInsertCustomerSql(databaseType, 200, "Original Name", 25);
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateMultiple(), databaseType);

            // Assert - should update 1 row
            Assert.Equal(1, updatedRows);

            // Verify the update worked
            var updatedCustomer = await connection.QueryFirstOrDefaultAsync<CustomerDto>(
                GetSelectCustomerByIdSql(databaseType, 200), transaction: transaction);

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
            var setupSql = GetInsertProductSql(databaseType, 101, "Test Product", 99.99m, new DateTime(2024, 1, 1), "11111111-2222-3333-4444-555555555555");
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateSetNewColumnsNull(), databaseType);

            // Assert - should update 1 row
            Assert.Equal(1, updatedRows);

            // Verify the update worked (handle database-specific GUID types)
            if (databaseType == DatabaseType.SQLite)
            {
                var updatedProduct = await connection.QueryFirstOrDefaultAsync<SqliteProductDto>(GetSelectProductByIdSql(databaseType, 101), transaction: transaction);
                Assert.NotNull(updatedProduct);
                Assert.Equal(101, updatedProduct.Id);
                Assert.Equal("Test Product", updatedProduct.ProductName); // Should remain unchanged
                Assert.Null(updatedProduct.Price); // Should be set to NULL
                Assert.Null(updatedProduct.CreatedDate); // Should be set to NULL
                Assert.Null(updatedProduct.UniqueId); // Should be set to NULL
            }
            else
            {
                var updatedProduct = await connection.QueryFirstOrDefaultAsync<ProductDto>(GetSelectProductByIdSql(databaseType, 101), transaction: transaction);
                Assert.NotNull(updatedProduct);
                Assert.Equal(101, updatedProduct.Id);
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
            var cleanupSql = GetCleanupSql(databaseType);
            var insertSql = databaseType switch
            {
                DatabaseType.SqlServer => $@"INSERT INTO [customers] ([Id], [Name], [Age], [IsActive]) VALUES 
                    (700, 'Customer 1', 25, {GetBooleanValue(databaseType)}),
                    (701, 'Customer 2', 30, {GetBooleanValue(databaseType)}),
                    (702, 'Customer 3', 35, {GetBooleanValue(databaseType)});",
                DatabaseType.PostgreSQL or DatabaseType.SQLite => $@"INSERT INTO ""customers"" (""Id"", ""Name"", ""Age"", ""IsActive"") VALUES 
                    (700, 'Customer 1', 25, {GetBooleanValue(databaseType)}),
                    (701, 'Customer 2', 30, {GetBooleanValue(databaseType)}),
                    (702, 'Customer 3', 35, {GetBooleanValue(databaseType)});",
                _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
            };
            var setupSql = $@"{cleanupSql}
                {insertSql}";
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateSetNullInt(), databaseType);

            // Assert - should update all 3 customers
            Assert.Equal(3, updatedRows);

            // Verify all customers have Age set to NULL
            var customers = (await connection.QueryAsync<CustomerDto>(GetSelectAllCustomersSql(databaseType), transaction: transaction)).ToList();

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
            var cleanupSql = GetCleanupSql(databaseType);
            var insertSql = databaseType switch
            {
                DatabaseType.SqlServer => $@"INSERT INTO [customers] ([Id], [Name], [Age], [IsActive]) VALUES 
                    (800, 'Original Name 1', 25, {GetBooleanValue(databaseType)}),
                    (801, 'Original Name 2', 30, {GetBooleanValue(databaseType)}),
                    (802, 'Original Name 3', 35, {GetBooleanValue(databaseType)});",
                DatabaseType.PostgreSQL or DatabaseType.SQLite => $@"INSERT INTO ""customers"" (""Id"", ""Name"", ""Age"", ""IsActive"") VALUES 
                    (800, 'Original Name 1', 25, {GetBooleanValue(databaseType)}),
                    (801, 'Original Name 2', 30, {GetBooleanValue(databaseType)}),
                    (802, 'Original Name 3', 35, {GetBooleanValue(databaseType)});",
                _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
            };
            var setupSql = $@"{cleanupSql}
                {insertSql}";
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateSetNullMixed(), databaseType);

            // Assert - should update all 3 customers
            Assert.Equal(3, updatedRows);

            // Verify all customers have Name="John" and Age=NULL
            var customers = (await connection.QueryAsync<CustomerDto>(GetSelectAllCustomersSql(databaseType), transaction: transaction)).ToList();

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
            await connection.ExecuteAsync(GetInsertCustomerSql(databaseType, 200, "Original Name", 25, true), transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateSetNullWhere(), databaseType);

            // Assert - should update 1 row
            Assert.Equal(1, updatedRows);

            // Verify the update worked
            var updatedCustomer = await connection.QueryFirstOrDefaultAsync<CustomerDto>(GetSelectCustomerByIdSql(databaseType, 200), transaction: transaction);

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
            var cleanupSql = GetCleanupSql(databaseType);
            var insertSql = databaseType switch
            {
                DatabaseType.SqlServer => $@"INSERT INTO [customers] ([Id], [Name], [Age], [IsActive]) VALUES 
                    (900, 'Customer A', 25, {GetBooleanValue(databaseType)}),
                    (901, 'Customer B', 30, {GetBooleanValue(databaseType)}),
                    (902, 'Customer C', 35, {GetBooleanValue(databaseType)});",
                DatabaseType.PostgreSQL or DatabaseType.SQLite => $@"INSERT INTO ""customers"" (""Id"", ""Name"", ""Age"", ""IsActive"") VALUES 
                    (900, 'Customer A', 25, {GetBooleanValue(databaseType)}),
                    (901, 'Customer B', 30, {GetBooleanValue(databaseType)}),
                    (902, 'Customer C', 35, {GetBooleanValue(databaseType)});",
                _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
            };
            var setupSql = $@"{cleanupSql}
                {insertSql}";
            await connection.ExecuteAsync(setupSql, transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateSetNull(), databaseType);

            // Assert - should update all 3 customers
            Assert.Equal(3, updatedRows);

            // Verify all customers have Name set to NULL
            var customers = (await connection.QueryAsync<CustomerDto>(GetSelectAllCustomersSql(databaseType), transaction: transaction)).ToList();

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
            await connection.ExecuteAsync(GetInsertProductSql(databaseType, 100, "Test Product", 50.00m, DateTime.Parse("2024-01-01"), "00000000-1111-2222-3333-444444444444"), transaction: transaction);

            // Act
            var updatedRows = await ExecuteStatementAsync(connection, transaction, TestStatements.UpdateWithNewColumns(), databaseType);

            // Assert - should update 1 row
            Assert.Equal(1, updatedRows);

            // Verify the update worked (handle database-specific GUID types)
            var updatedProduct = await QueryProductAsync(connection, transaction, databaseType, GetSelectProductByIdSql(databaseType, 100));

            Assert.Equal(100, updatedProduct.Id);
            Assert.Equal("Test Product", updatedProduct.ProductName); // Should remain unchanged
            Assert.Equal(119.99m, updatedProduct.Price); // Should be updated
            Assert.Equal(new DateTime(2024, 12, 25), updatedProduct.CreatedDate); // Should be updated
            Assert.Equal("87654321-4321-4321-4321-210987654321", updatedProduct.UniqueId); // Should be updated
        }, databaseType);
    }
}
