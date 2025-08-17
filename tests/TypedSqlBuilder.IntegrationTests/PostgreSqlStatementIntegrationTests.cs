using TypedSqlBuilder.Core;
using TypedSqlBuilder.TestModels;
using Dapper;
using Xunit;

namespace TypedSqlBuilder.IntegrationTests;

/// <summary>
/// Integration tests for INSERT, UPDATE, DELETE statements executed against SQL Server databases using Dapper
/// </summary>
public class PostgreSqlStatementIntegrationTests : IClassFixture<PostgreSqlFixture>, IStatementTestContract, IPostgreSqlDialectTestContract
{
    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlStatementIntegrationTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Helper method to execute a statement with proper sequence handling for PostgreSQL
    /// </summary>
    private async Task<int> ExecuteStatementAsync(System.Data.IDbConnection connection, System.Data.IDbTransaction transaction, ISqlStatement statement, bool hasExplicitId = true)
    {
        var (sql, parameters) = statement.ToPostgreSqlRaw();
        var dapperParams = parameters.ToDapperParameters();

        // PostgreSQL doesn't need IDENTITY_INSERT like SQL Server
        // The SERIAL columns will work correctly, and explicit ID inserts work without special commands
        return await connection.ExecuteAsync(sql, dapperParams, transaction);
    }

    [Fact]
    public async Task InsertBasic_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange
            var statement = TestStatements.InsertBasic();
            
            // Act - Execute INSERT against real database
            var insertedRows = await ExecuteStatementAsync(connection, transaction, statement, hasExplicitId: true);
            
            // Assert
            Assert.Equal(1, insertedRows);
            
            // Verify the insert worked
            var insertedCustomer = await connection.QuerySingleAsync<CustomerDto>(
                "SELECT * FROM customers WHERE id = 200", transaction: transaction);
            Assert.Equal("John Doe", insertedCustomer.Name);
            Assert.Equal(25, insertedCustomer.Age);
        });
    }

    [Fact]
    public async Task InsertPartial_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange
            var statement = TestStatements.InsertPartial();
            
            // Act - Execute INSERT against real database (no explicit ID, so auto-generated)
            var insertedRows = await ExecuteStatementAsync(connection, transaction, statement, hasExplicitId: false);
            
            // Assert
            Assert.Equal(1, insertedRows);
            
            // Verify the insert worked (ID will be auto-generated)
            var insertedCustomer = await connection.QuerySingleAsync<CustomerDto>(
                "SELECT * FROM customers WHERE name = 'Partial Customer' ORDER BY id DESC LIMIT 1", 
                transaction: transaction);
            Assert.Equal("Partial Customer", insertedCustomer.Name);
            Assert.Equal(28, insertedCustomer.Age);
        });
    }

    [Fact]
    public async Task UpdateNewCustomer_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - First insert the customer we'll update using inline statement
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 100)
                .Value(c => c.Age, 35)
                .Value(c => c.Name, "New Customer");
            await ExecuteStatementAsync(connection, transaction, insertStatement, hasExplicitId: true);
            
            var statement = TypedSql.Update<Customer>()
                .Set(c => c.Age, 36)
                .Where(c => c.Id == 100);
            
            // Act - Execute UPDATE against real database
            var updatedRows = await ExecuteStatementAsync(connection, transaction, statement, hasExplicitId: false);
            
            // Assert
            Assert.Equal(1, updatedRows);
            
            // Verify the update worked - age should now be 36
            var updatedCustomer = await connection.QuerySingleAsync<CustomerDto>(
                "SELECT * FROM customers WHERE id = 100", transaction: transaction);
            Assert.Equal(36, updatedCustomer.Age);
            Assert.Equal("New Customer", updatedCustomer.Name); // Name should remain unchanged
        });
    }

    [Fact]
    public async Task UpdateBasic_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - First insert a customer to update (use ID from TestStatements.UpdateBasic)
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 200)
                .Value(c => c.Age, 25)
                .Value(c => c.Name, "John Doe");
            await ExecuteStatementAsync(connection, transaction, insertStatement, hasExplicitId: true);
            
            var statement = TestStatements.UpdateBasic();
            
            // Act - Execute UPDATE against real database
            var updatedRows = await ExecuteStatementAsync(connection, transaction, statement, hasExplicitId: false);
            
            // Assert
            Assert.Equal(1, updatedRows);
            
            // Verify the update worked (Age should be 26)
            var updatedCustomer = await connection.QuerySingleAsync<CustomerDto>(
                "SELECT * FROM customers WHERE id = 200", transaction: transaction);
            Assert.Equal("John Doe", updatedCustomer.Name); // Name unchanged
            Assert.Equal(26, updatedCustomer.Age); // Age updated from 25 to 26
        });
    }

    [Fact]
    public async Task UpdateMultiple_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - First insert a customer to update (use ID from TestStatements.UpdateMultiple)
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 200)
                .Value(c => c.Age, 25)
                .Value(c => c.Name, "John Doe");
            await ExecuteStatementAsync(connection, transaction, insertStatement, hasExplicitId: true);
            
            var statement = TestStatements.UpdateMultiple();
            
            // Act - Execute UPDATE against real database
            var updatedRows = await ExecuteStatementAsync(connection, transaction, statement, hasExplicitId: false);
            
            // Assert
            Assert.Equal(1, updatedRows);
            
            // Verify the update worked
            var updatedCustomer = await connection.QuerySingleAsync<CustomerDto>(
                "SELECT * FROM customers WHERE id = 200", transaction: transaction);
            Assert.Equal("John Smith", updatedCustomer.Name); // Name updated from "John Doe" to "John Smith"
            Assert.Equal(27, updatedCustomer.Age); // Age updated from 25 to 27
        });
    }

    [Fact]
    public async Task UpdateConditional_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - First insert a customer to update
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 203)
                .Value(c => c.Age, 25)
                .Value(c => c.Name, "Update Conditional Test");
            await ExecuteStatementAsync(connection, transaction, insertStatement, hasExplicitId: true);
            
            var statement = TestStatements.UpdateConditional();
            
            // Act - Execute UPDATE against real database
            var updatedRows = await ExecuteStatementAsync(connection, transaction, statement, hasExplicitId: false);
            
            // Assert - Should update multiple rows (existing test data + our inserted customer)
            Assert.True(updatedRows > 0); // At least our customer should be updated
            
            // Verify our inserted customer was updated (25 + 1 = 26)
            var updatedCustomer = await connection.QuerySingleAsync<CustomerDto>(
                "SELECT * FROM customers WHERE id = 203", transaction: transaction);
            Assert.Equal("Update Conditional Test", updatedCustomer.Name);
            Assert.Equal(26, updatedCustomer.Age); // 25 + 1
        });
    }

    [Fact]
    public async Task DeleteBasic_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - First insert a customer to delete (use ID from TestStatements.DeleteBasic)
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 200)
                .Value(c => c.Age, 25)
                .Value(c => c.Name, "John Doe");
            await ExecuteStatementAsync(connection, transaction, insertStatement, hasExplicitId: true);
            
            var statement = TestStatements.DeleteBasic();
            
            // Act - Execute DELETE against real database
            var deletedRows = await ExecuteStatementAsync(connection, transaction, statement, hasExplicitId: false);
            
            // Assert
            Assert.Equal(1, deletedRows);
            
            // Verify the delete worked
            var remainingCustomers = await connection.QueryAsync<CustomerDto>(
                "SELECT * FROM customers WHERE id = 200", transaction: transaction);
            Assert.Empty(remainingCustomers);
        });
    }

    [Fact]
    public async Task DeleteConditional_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - Insert a customer that will match the delete condition (Age < 18 OR Name = 'Temp')
            await ExecuteStatementAsync(connection, transaction, 
                TypedSql.Insert<Customer>()
                    .Value(c => c.Id, 300)
                    .Value(c => c.Age, 15) // < 18
                    .Value(c => c.Name, "Young Customer"), 
                hasExplicitId: true);
            
            var statement = TestStatements.DeleteConditional();
            
            // Act - Execute DELETE against real database
            var deletedRows = await ExecuteStatementAsync(connection, transaction, statement, hasExplicitId: false);
            
            // Assert - Should delete the customer we just added (Age < 18) plus Minor User from test data (Age = 16)
            Assert.True(deletedRows >= 1);
            
            // Verify our inserted customer was deleted
            var remainingCustomers = await connection.QueryAsync<CustomerDto>(
                "SELECT * FROM customers WHERE id = 300", transaction: transaction);
            Assert.Empty(remainingCustomers);
        });
    }

    [Fact]
    public async Task DeleteAll_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - Delete all customers (need to delete orders first due to foreign key)
            // First delete all orders to avoid foreign key constraint
            await connection.ExecuteAsync("DELETE FROM orders", transaction: transaction);
            
            var statement = TestStatements.DeleteAll();
            
            // Act - Execute DELETE ALL against real database
            var deletedRows = await ExecuteStatementAsync(connection, transaction, statement, hasExplicitId: false);
            
            // Assert - Should delete all customers (however many exist)
            Assert.True(deletedRows > 0); // At least some customers should be deleted
            
            // Verify all customers were deleted
            var remainingCustomers = await connection.QueryAsync<CustomerDto>(
                "SELECT * FROM customers", transaction: transaction);
            Assert.Empty(remainingCustomers);
        });
    }

    [Fact]
    public async Task UpdateSetNull_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - This test updates ALL customers (no WHERE clause)
            var statement = TestStatements.UpdateSetNull();
            
            // Act - Execute UPDATE against real database
            var updatedRows = await ExecuteStatementAsync(connection, transaction, statement, hasExplicitId: false);
            
            // Assert - Should update all customers in test data (however many exist)
            Assert.True(updatedRows > 0); // At least some customers should be updated
            
            // Verify all customers had their names set to NULL
            var customers = await connection.QueryAsync<CustomerDto>("SELECT * FROM customers", transaction: transaction);
            Assert.All(customers, c => Assert.Null(c.Name)); // All names should be null
        });
    }

    [Fact]
    public async Task UpdateSetNullInt_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - This test updates ALL customers (no WHERE clause) 
            var statement = TestStatements.UpdateSetNullInt();
            
            // Act - Execute UPDATE against real database
            var updatedRows = await ExecuteStatementAsync(connection, transaction, statement, hasExplicitId: false);
            
            // Assert - Should update all customers
            Assert.True(updatedRows > 0); // At least some customers should be updated
            
            // Verify all customers had their ages set to NULL
            var customers = await connection.QueryAsync<CustomerDto>("SELECT * FROM customers", transaction: transaction);
            Assert.All(customers, c => Assert.Null(c.Age)); // All ages should be null
        });
    }

    [Fact]
    public async Task UpdateSetNullMixed_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - This test updates ALL customers (no WHERE clause)
            var statement = TestStatements.UpdateSetNullMixed();
            
            // Act - Execute UPDATE against real database
            var updatedRows = await ExecuteStatementAsync(connection, transaction, statement, hasExplicitId: false);
            
            // Assert - Should update all customers
            Assert.True(updatedRows > 0); // At least some customers should be updated
            
            // Verify the update worked
            var customers = await connection.QueryAsync<CustomerDto>("SELECT * FROM customers", transaction: transaction);
            Assert.All(customers, c => Assert.Equal("John", c.Name)); // All names should be "John"
            Assert.All(customers, c => Assert.Null(c.Age)); // All ages should be null
        });
    }

    [Fact]
    public async Task UpdateSetNullWhere_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - Insert a customer that matches the WHERE condition (Id = 200)
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 200)
                .Value(c => c.Age, 25)
                .Value(c => c.Name, "John Doe");
            await ExecuteStatementAsync(connection, transaction, insertStatement, hasExplicitId: true);
            
            // Insert another customer that doesn't match (Id = 206)
            await ExecuteStatementAsync(connection, transaction,
                TypedSql.Insert<Customer>()
                    .Value(c => c.Id, 206)
                    .Value(c => c.Age, 40)
                    .Value(c => c.Name, "Will Not Change"),
                hasExplicitId: true);
            
            var statement = TestStatements.UpdateSetNullWhere();
            
            // Act - Execute UPDATE against real database
            var updatedRows = await ExecuteStatementAsync(connection, transaction, statement, hasExplicitId: false);
            
            // Assert - Should only update the customer with Id = 200
            Assert.Equal(1, updatedRows);
            
            // Verify the correct customer was updated
            var updatedCustomer = await connection.QuerySingleAsync<CustomerDto>(
                "SELECT * FROM customers WHERE id = 200", transaction: transaction);
            Assert.Null(updatedCustomer.Name); // Name should be set to null
            Assert.Equal(25, updatedCustomer.Age); // Age should remain unchanged
            
            // Verify the other customer was not affected
            var unchangedCustomer = await connection.QuerySingleAsync<CustomerDto>(
                "SELECT * FROM customers WHERE id = 206", transaction: transaction);
            Assert.Equal("Will Not Change", unchangedCustomer.Name); // Should remain unchanged
            Assert.Equal(40, unchangedCustomer.Age);
        });
    }

    [Fact]
    public async Task InsertWithNull_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange
            var statement = TestStatements.InsertWithNull();
            
            // Act - Execute INSERT against real database
            var insertedRows = await ExecuteStatementAsync(connection, transaction, statement, hasExplicitId: true);
            
            // Assert
            Assert.Equal(1, insertedRows);
            
            // Verify the insert worked and Name is NULL (SQL Server stores NULL as NULL)
            var insertedCustomer = await connection.QuerySingleAsync<CustomerDto>(
                "SELECT * FROM customers WHERE id = 202", transaction: transaction);
            Assert.Null(insertedCustomer.Name); // SQL Server stores NULL as NULL
            Assert.Equal(25, insertedCustomer.Age);
        });
    }

    [Fact]
    public async Task InsertWithNullInt_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange
            var statement = TestStatements.InsertWithNullInt();
            
            // Act - Execute INSERT against real database
            var insertedRows = await ExecuteStatementAsync(connection, transaction, statement, hasExplicitId: true);
            
            // Assert
            Assert.Equal(1, insertedRows);
            
            // Verify the insert worked and Age is NULL (the 203 reference!)
            var insertedCustomer = await connection.QuerySingleAsync<CustomerDto>(
                "SELECT * FROM customers WHERE id = 203", transaction: transaction);
            Assert.Equal("John", insertedCustomer.Name); // Name should be "John"
            Assert.Null(insertedCustomer.Age); // Age should be NULL
        });
    }

    // Interface implementation methods - these delegate to the actual integration test methods for consistency
    [Fact]
    public async Task InsertBasic_GeneratesCorrectSql()
    {
        await InsertBasic_ExecutesAgainstDatabase();
    }

    [Fact]
    public async Task UpdateBasic_GeneratesCorrectSql()
    {
        await UpdateBasic_ExecutesAgainstDatabase();
    }

    [Fact]
    public async Task DeleteBasic_GeneratesCorrectSql()
    {
        await DeleteBasic_ExecutesAgainstDatabase();
    }

    [Fact]
    public async Task DeleteAll_GeneratesCorrectSql()
    {
        await DeleteAll_ExecutesAgainstDatabase();
    }

    [Fact]
    public async Task UpdateConditional_GeneratesCorrectSql()
    {
        await UpdateConditional_ExecutesAgainstDatabase();
    }

    [Fact]
    public async Task InsertPartial_GeneratesCorrectSql()
    {
        await InsertPartial_ExecutesAgainstDatabase();
    }

    [Fact]
    public async Task UpdateMultiple_GeneratesCorrectSql()
    {
        await UpdateMultiple_ExecutesAgainstDatabase();
    }

    [Fact]
    public async Task DeleteConditional_GeneratesCorrectSql()
    {
        await DeleteConditional_ExecutesAgainstDatabase();
    }

    [Fact]
    public async Task UpdateSetNull_GeneratesCorrectSql()
    {
        await UpdateSetNull_ExecutesAgainstDatabase();
    }

    [Fact]
    public async Task UpdateSetNullMixed_GeneratesCorrectSql()
    {
        await UpdateSetNullMixed_ExecutesAgainstDatabase();
    }

    [Fact]
    public async Task UpdateSetNullInt_GeneratesCorrectSql()
    {
        await UpdateSetNullInt_ExecutesAgainstDatabase();
    }

    [Fact]
    public async Task UpdateSetNullWhere_GeneratesCorrectSql()
    {
        await UpdateSetNullWhere_ExecutesAgainstDatabase();
    }

    [Fact]
    public async Task InsertWithNull_GeneratesCorrectSql()
    {
        await InsertWithNull_ExecutesAgainstDatabase();
    }

    [Fact]
    public async Task InsertWithNullInt_GeneratesCorrectSql()
    {
        await InsertWithNullInt_ExecutesAgainstDatabase();
    }

    [Fact]
    public async Task PostgreSql_UsesColonParameterPrefix()
    {
        // Integration test for PostgreSQL : parameter prefix
        await _fixture.WithTransactionAsync(async (connection, transaction) =>
        {
            // Arrange - using inline statement to test : symbol prefix
            var statement = TypedSql.Update<Customer>()
                .Set(c => c.Name, "Inline Test");

            // Act - Execute against real database  
            var (sql, parameters) = statement.ToPostgreSqlRaw();
            var dapperParams = parameters.ToDapperParameters();
            
            // First create test data
            await ExecuteStatementAsync(connection, transaction,
                TypedSql.Insert<Customer>()
                    .Value(c => c.Id, 999)
                    .Value(c => c.Age, 30)
                    .Value(c => c.Name, "Original Name"),
                hasExplicitId: true);

            // Add WHERE clause for specific update
            var updateStatement = TypedSql.Update<Customer>()
                .Set(c => c.Name, "Inline Test")
                .Where(c => c.Id == 999);
                
            var updatedRows = await ExecuteStatementAsync(connection, transaction, updateStatement, hasExplicitId: false);
            
            // Assert - should use : prefix in generated SQL and execute successfully
            var (finalSql, finalParams) = updateStatement.ToPostgreSqlRaw();
            Assert.Contains(":p", finalSql);
            Assert.Equal(1, updatedRows);
            
            // Verify update worked
            var updatedCustomer = await connection.QuerySingleAsync<CustomerDto>(
                "SELECT * FROM customers WHERE id = 999", transaction: transaction);
            Assert.Equal("Inline Test", updatedCustomer.Name);
        });
    }
}
