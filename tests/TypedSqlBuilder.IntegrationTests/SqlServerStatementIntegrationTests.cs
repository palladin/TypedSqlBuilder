using TypedSqlBuilder.Core;
using TypedSqlBuilder.TestModels;
using Dapper;
using Xunit;

namespace TypedSqlBuilder.IntegrationTests;

/// <summary>
/// Integration tests for INSERT, UPDATE, DELETE statements executed against SQL Server databases using Dapper
/// </summary>
public class SqlServerStatementIntegrationTests : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;

    public SqlServerStatementIntegrationTests(SqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task InsertStatement_ExecutesCorrectly()
    {
        // Use transaction to ensure test isolation
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - Enable IDENTITY_INSERT for explicit ID insertion
            await connection.ExecuteAsync("SET IDENTITY_INSERT customers ON", transaction: transaction);
            
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 100)
                .Value(c => c.Age, 35)
                .Value(c => c.Name, "New Customer");
            var (insertSql, insertParams) = insertStatement.ToSqlServerRaw();
            
            // Act - Execute INSERT with Dapper
            var dapperInsertParams = insertParams.ToDapperParameters();
            var insertedRows = await connection.ExecuteAsync(insertSql, dapperInsertParams, transaction);
            
            // Turn off IDENTITY_INSERT
            await connection.ExecuteAsync("SET IDENTITY_INSERT customers OFF", transaction: transaction);

            // Assert
            Assert.Equal(1, insertedRows);

            // Verify the insert worked by querying
            var insertedCustomer = await connection.QuerySingleAsync<CustomerDto>(
                "SELECT * FROM customers WHERE Id = 100", 
                transaction: transaction);
            Assert.Equal("New Customer", insertedCustomer.Name);
            Assert.Equal(35, insertedCustomer.Age);
        });
    }

    [Fact]
    public async Task DeleteStatement_ExecutesCorrectly()
    {
        // Use transaction to ensure test isolation
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - Enable IDENTITY_INSERT for explicit ID insertion
            await connection.ExecuteAsync("SET IDENTITY_INSERT customers ON", transaction: transaction);
            
            // First insert a customer to delete using inline statement
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 100)
                .Value(c => c.Age, 35)
                .Value(c => c.Name, "New Customer");
            
            var (insertSql, insertParams) = insertStatement.ToSqlServerRaw();
            var dapperInsertParams = insertParams.ToDapperParameters();
            await connection.ExecuteAsync(insertSql, dapperInsertParams, transaction);
            
            // Turn off IDENTITY_INSERT
            await connection.ExecuteAsync("SET IDENTITY_INSERT customers OFF", transaction: transaction);

            // Create delete statement
            var deleteStatement = TypedSql.Delete<Customer>()
                .Where(c => c.Id == 100);
            var (deleteSql, deleteParams) = deleteStatement.ToSqlServerRaw();

            // Act - Execute DELETE with Dapper
            var dapperDeleteParams = deleteParams.ToDapperParameters();
            var deletedRows = await connection.ExecuteAsync(deleteSql, dapperDeleteParams, transaction);

            // Assert
            Assert.Equal(1, deletedRows);

            // Verify the delete worked by querying
            var remainingCustomers = await connection.QueryAsync<CustomerDto>(
                "SELECT * FROM customers WHERE Id = 100", 
                transaction: transaction);
            Assert.Empty(remainingCustomers);
        });
    }

    [Fact]
    public async Task UpdateStatement_ExecutesCorrectly()
    {
        // Use transaction to ensure test isolation
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - Enable IDENTITY_INSERT for explicit ID insertion
            await connection.ExecuteAsync("SET IDENTITY_INSERT customers ON", transaction: transaction);
            
            // First insert a customer to update
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 100)
                .Value(c => c.Age, 25)
                .Value(c => c.Name, "Original Name");
            
            var (insertSql, insertParams) = insertStatement.ToSqlServerRaw();
            var dapperInsertParams = insertParams.ToDapperParameters();
            await connection.ExecuteAsync(insertSql, dapperInsertParams, transaction);
            
            // Turn off IDENTITY_INSERT
            await connection.ExecuteAsync("SET IDENTITY_INSERT customers OFF", transaction: transaction);

            // Create update statement
            var updateStatement = TypedSql.Update<Customer>()
                .Set(c => c.Name, "Updated Name")
                .Set(c => c.Age, 30)
                .Where(c => c.Id == 100);
            var (updateSql, updateParams) = updateStatement.ToSqlServerRaw();

            // Act - Execute UPDATE with Dapper
            var dapperUpdateParams = updateParams.ToDapperParameters();
            var updatedRows = await connection.ExecuteAsync(updateSql, dapperUpdateParams, transaction);

            // Assert
            Assert.Equal(1, updatedRows);

            // Verify the update worked by querying
            var updatedCustomer = await connection.QuerySingleAsync<CustomerDto>(
                "SELECT * FROM customers WHERE Id = 100", 
                transaction: transaction);
            Assert.Equal("Updated Name", updatedCustomer.Name);
            Assert.Equal(30, updatedCustomer.Age);
        });
    }

    [Fact]
    public async Task InsertMultipleValues_ExecutesCorrectly()
    {
        // Use transaction to ensure test isolation
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - Enable IDENTITY_INSERT for explicit ID insertion
            await connection.ExecuteAsync("SET IDENTITY_INSERT customers ON", transaction: transaction);
            
            // Create insert statement for Customer 1
            var insertStatement1 = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 200)
                .Value(c => c.Age, 25)
                .Value(c => c.Name, "Customer 1");
            
            var (insertSql1, insertParams1) = insertStatement1.ToSqlServerRaw();
            
            // Create insert statement for Customer 2
            var insertStatement2 = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 201)
                .Value(c => c.Age, 30)
                .Value(c => c.Name, "Customer 2");
            
            var (insertSql2, insertParams2) = insertStatement2.ToSqlServerRaw();

            // Act - Execute multiple INSERTs with Dapper
            var dapperInsertParams1 = insertParams1.ToDapperParameters();
            var dapperInsertParams2 = insertParams2.ToDapperParameters();
            var insertedRows1 = await connection.ExecuteAsync(insertSql1, dapperInsertParams1, transaction);
            var insertedRows2 = await connection.ExecuteAsync(insertSql2, dapperInsertParams2, transaction);
            
            // Turn off IDENTITY_INSERT
            await connection.ExecuteAsync("SET IDENTITY_INSERT customers OFF", transaction: transaction);

            // Assert
            Assert.Equal(1, insertedRows1);
            Assert.Equal(1, insertedRows2);

            // Verify the inserts worked by querying
            var insertedCustomers = await connection.QueryAsync<CustomerDto>(
                "SELECT * FROM customers WHERE Id IN (200, 201) ORDER BY Id", 
                transaction: transaction);
            var customerList = insertedCustomers.ToList();
            
            Assert.Equal(2, customerList.Count);
            Assert.Equal("Customer 1", customerList[0].Name);
            Assert.Equal(25, customerList[0].Age);
            Assert.Equal("Customer 2", customerList[1].Name);
            Assert.Equal(30, customerList[1].Age);
        });
    }

    [Fact]
    public async Task DeleteAllStatement_ExecutesCorrectly()
    {
        // Use transaction to ensure test isolation
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - Count existing customers
            var initialCount = await connection.QuerySingleAsync<int>(
                "SELECT COUNT(*) FROM customers", 
                transaction: transaction);
            Assert.True(initialCount > 0); // Should have test data
            
            // Delete all orders first to avoid foreign key constraint issues
            await connection.ExecuteAsync("DELETE FROM orders", transaction: transaction);

            // Create delete all statement
            var deleteStatement = TypedSql.Delete<Customer>();
            var (deleteSql, deleteParams) = deleteStatement.ToSqlServerRaw();

            // Act - Execute DELETE ALL with Dapper
            var dapperDeleteParams = deleteParams.ToDapperParameters();
            var deletedRows = await connection.ExecuteAsync(deleteSql, dapperDeleteParams, transaction);

            // Assert
            Assert.Equal(initialCount, deletedRows);

            // Verify all customers were deleted
            var remainingCount = await connection.QuerySingleAsync<int>(
                "SELECT COUNT(*) FROM customers", 
                transaction: transaction);
            Assert.Equal(0, remainingCount);
        });
    }
}
