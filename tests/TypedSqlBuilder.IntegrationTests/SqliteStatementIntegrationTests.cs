using TypedSqlBuilder.Core;
using TypedSqlBuilder.TestModels;
using Dapper;

namespace TypedSqlBuilder.IntegrationTests;

/// <summary>
/// Integration tests for INSERT, UPDATE, DELETE statements executed against SQLite databases using Dapper
/// </summary>
public class SqliteStatementIntegrationTests : SqliteIntegrationTestBase
{
    [Fact]
    public void InsertStatement_ExecutesCorrectly()
    {
        // Use transaction to ensure test isolation
        WithTransaction(connection =>
        {
            // Arrange
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 100)
                .Value(c => c.Age, 35)
                .Value(c => c.Name, "New Customer");
            var (insertSql, insertParams) = insertStatement.ToSqliteRaw();
            
            // Act - Execute INSERT with Dapper
            var dapperInsertParams = insertParams.ToDapperParameters();
            var insertedRows = connection.Execute(insertSql, dapperInsertParams);

            // Assert
            Assert.Equal(1, insertedRows);

            // Verify the insert worked by querying
            var insertedCustomer = connection.QuerySingle<CustomerDto>(
                "SELECT * FROM customers WHERE Id = 100");
            Assert.Equal("New Customer", insertedCustomer.Name);
            Assert.Equal(35, insertedCustomer.Age);
        });
    }

    [Fact]
    public void DeleteStatement_ExecutesCorrectly()
    {
        // Use transaction to ensure test isolation
        WithTransaction(connection =>
        {
            // Arrange - First insert a customer to delete using inline statement
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 100)
                .Value(c => c.Age, 35)
                .Value(c => c.Name, "New Customer");
            var (insertSql, insertParams) = insertStatement.ToSqliteRaw();
            
            var dapperInsertParams = insertParams.ToDapperParameters();
            connection.Execute(insertSql, dapperInsertParams);

            // Verify customer exists
            var existingCount = connection.QuerySingle<int>("SELECT COUNT(*) FROM customers WHERE Id = 100");
            Assert.Equal(1, existingCount);

            // Act - Execute DELETE statement using inline SQL
            var deleteStatement = TypedSql.Delete<Customer>()
                .Where(c => c.Id == 100);
            var (deleteSql, deleteParams) = deleteStatement.ToSqliteRaw();
            
            var dapperDeleteParams = deleteParams.ToDapperParameters();
            var deletedRows = connection.Execute(deleteSql, dapperDeleteParams);

            // Assert
            Assert.Equal(1, deletedRows);

            // Verify the customer was deleted
            var remainingCount = connection.QuerySingle<int>("SELECT COUNT(*) FROM customers WHERE Id = 100");
            Assert.Equal(0, remainingCount);
        });
    }

        [Fact]
    public void InsertBasic_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        WithTransaction(connection =>
        {
            // Arrange
            var statement = TestStatements.InsertBasic();
            var (sql, parameters) = statement.ToSqliteRaw();
            
            // Act - Execute INSERT against real database
            var dapperParams = parameters.ToDapperParameters();
            var insertedRows = connection.Execute(sql, dapperParams);
            
            // Assert
            Assert.Equal(1, insertedRows);
            
            // Verify the insert worked by querying
            var insertedCustomer = connection.QuerySingle<CustomerDto>("SELECT * FROM customers WHERE Id = 200");
            Assert.Equal("John Doe", insertedCustomer.Name);
            Assert.Equal(25, insertedCustomer.Age);
        });
    }

    [Fact]
    public void InsertPartial_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        WithTransaction(connection =>
        {
            // Arrange
            var statement = TestStatements.InsertPartial();
            var (sql, parameters) = statement.ToSqliteRaw();
            
            // Act - Execute INSERT against real database
            var dapperParams = parameters.ToDapperParameters();
            var insertedRows = connection.Execute(sql, dapperParams);
            
            // Assert
            Assert.Equal(1, insertedRows);
            
            // Verify the insert worked by querying - find the customer with Age=28 and Name="Partial Customer"
            var insertedCustomer = connection.QuerySingle<CustomerDto>("SELECT * FROM customers WHERE Age = 28 AND Name = 'Partial Customer'");
            Assert.Equal("Partial Customer", insertedCustomer.Name);
            Assert.Equal(28, insertedCustomer.Age);
        });
    }

    [Fact]
    public void CombinedOperations_ExecuteCorrectly()
    {
        // Test a sequence of INSERT -> verify -> DELETE -> verify
        
        // 1. INSERT
        var insertStatement = TypedSql.Insert<Customer>()
            .Value(c => c.Id, 100)
            .Value(c => c.Age, 35)
            .Value(c => c.Name, "New Customer");
        var (insertSql, insertParams) = insertStatement.ToSqliteRaw();
        
        var dapperInsertParams = insertParams.ToDapperParameters();
        var insertedRows = _connection.Execute(insertSql, dapperInsertParams);
        Assert.Equal(1, insertedRows);

        // 2. Verify INSERT
        var customerAfterInsert = _connection.QuerySingle<CustomerDto>(
            "SELECT * FROM customers WHERE Id = 100");
        Assert.Equal("New Customer", customerAfterInsert.Name);
        Assert.Equal(35, customerAfterInsert.Age);

        // 3. DELETE
        var deleteStatement = TypedSql.Delete<Customer>()
            .Where(c => c.Id == 100);
        var (deleteSql, deleteParams) = deleteStatement.ToSqliteRaw();
        
        var dapperDeleteParams = deleteParams.ToDapperParameters();
        var deletedRows = _connection.Execute(deleteSql, dapperDeleteParams);
        Assert.Equal(1, deletedRows);

        // 4. Verify DELETE
        var remainingCount = _connection.QuerySingle<int>("SELECT COUNT(*) FROM customers WHERE Id = 100");
        Assert.Equal(0, remainingCount);

        // 5. Verify original data is still intact
        var totalCustomers = _connection.QuerySingle<int>("SELECT COUNT(*) FROM customers");
        Assert.Equal(4, totalCustomers); // Back to original 4 customers
    }

    [Fact]
    public void UpdateBasic_GeneratesSqlCorrectly()
    {
        // Use transaction to ensure test isolation while executing real SQL
        WithTransaction(connection =>
        {
            // Arrange - First insert the customer we'll update using inline statement
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 200)
                .Value(c => c.Age, 25)
                .Value(c => c.Name, "John Doe");
            var (insertSql, insertParams) = insertStatement.ToSqliteRaw();
            var insertDapperParams = insertParams.ToDapperParameters();
            connection.Execute(insertSql, insertDapperParams);
            
            // Get original state to verify change
            var originalCustomer = connection.QuerySingle<CustomerDto>("SELECT * FROM customers WHERE Id = 200");
            
            // Create inline UPDATE statement that updates customer Id=200 age to 26
            var statement = TypedSql.Update<Customer>()
                .Set(c => c.Age, 26)
                .Where(c => c.Id == 200);
            var (sql, parameters) = statement.ToSqliteRaw();
            
            // Act - Execute UPDATE against real database
            var dapperParams = parameters.ToDapperParameters();
            var updatedRows = connection.Execute(sql, dapperParams);

            // Assert
            Assert.Equal(1, updatedRows);
            
            // Verify the update worked - age should now be 26
            var updatedCustomer = connection.QuerySingle<CustomerDto>("SELECT * FROM customers WHERE Id = 200");
            Assert.Equal(26, updatedCustomer.Age);
            
            // Verify other fields weren't changed
            Assert.Equal(originalCustomer.Id, updatedCustomer.Id);
            Assert.Equal(originalCustomer.Name, updatedCustomer.Name);
        });
    }

    [Fact]
    public void UpdateBasic_GeneratesCorrectSql()
    {
        // Just verify SQL generation without database execution
        var statement = TestStatements.UpdateBasic();
        var (sql, parameters) = statement.ToSqliteRaw();

        // Assert SQL structure is correct
        Assert.Equal("UPDATE customers SET Age = :p0 WHERE customers.Id = :p1", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(26, parameters[":p0"]);
        Assert.Equal(200, parameters[":p1"]);
    }

    [Fact]
    public void UpdateNewCustomer_GeneratesSqlCorrectly()
    {
        // Just verify SQL generation and syntax using inline statement
        var statement = TypedSql.Update<Customer>()
            .Set(c => c.Age, 36)
            .Where(c => c.Id == 100);
        var (sql, parameters) = statement.ToSqliteRaw();

        // Assert SQL structure is valid
        Assert.Contains("UPDATE", sql);
        Assert.Contains("SET", sql);
        Assert.Contains("WHERE", sql);
        Assert.NotEmpty(parameters);
    }

    [Fact]
    public void UpdateNewCustomer_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        WithTransaction(connection =>
        {
            // Arrange - First insert the customer we'll update using inline statement
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 100)
                .Value(c => c.Age, 35)
                .Value(c => c.Name, "New Customer");
            var (insertSql, insertParams) = insertStatement.ToSqliteRaw();
            var insertDapperParams = insertParams.ToDapperParameters();
            connection.Execute(insertSql, insertDapperParams);
            
            var statement = TypedSql.Update<Customer>()
                .Set(c => c.Age, 36)
                .Where(c => c.Id == 100);
            var (sql, parameters) = statement.ToSqliteRaw();
            
            // Act - Execute UPDATE against real database
            var dapperParams = parameters.ToDapperParameters();
            var updatedRows = connection.Execute(sql, dapperParams);
            
            // Assert
            Assert.Equal(1, updatedRows);
            
            // Verify the update worked - age should now be 36
            var updatedCustomer = connection.QuerySingle<CustomerDto>("SELECT * FROM customers WHERE Id = 100");
            Assert.Equal(36, updatedCustomer.Age);
            Assert.Equal("New Customer", updatedCustomer.Name); // Name should remain unchanged
        });
    }

    [Fact]
    public void UpdateBasic_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        WithTransaction(connection =>
        {
            // Arrange - First insert the customer we'll update using inline statement
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 200)
                .Value(c => c.Age, 25)
                .Value(c => c.Name, "John Doe");
            var (insertSql, insertParams) = insertStatement.ToSqliteRaw();
            var insertDapperParams = insertParams.ToDapperParameters();
            connection.Execute(insertSql, insertDapperParams);
            
            var statement = TypedSql.Update<Customer>()
                .Set(c => c.Age, 26)
                .Where(c => c.Id == 200);
            var (sql, parameters) = statement.ToSqliteRaw();
            
            // Act - Execute UPDATE against real database
            var dapperParams = parameters.ToDapperParameters();
            var updatedRows = connection.Execute(sql, dapperParams);
            
            // Assert
            Assert.Equal(1, updatedRows);
            
            // Verify the update worked
            var updatedCustomer = connection.QuerySingle<CustomerDto>("SELECT * FROM customers WHERE Id = 200");
            Assert.Equal(26, updatedCustomer.Age);
        });
    }

    [Fact]
    public void UpdateMultiple_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        WithTransaction(connection =>
        {
            // Arrange - First insert the customer we'll update using inline statement
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 200)
                .Value(c => c.Age, 25)
                .Value(c => c.Name, "John Doe");
            var (insertSql, insertParams) = insertStatement.ToSqliteRaw();
            var insertDapperParams = insertParams.ToDapperParameters();
            connection.Execute(insertSql, insertDapperParams);
            
            var statement = TestStatements.UpdateMultiple();
            var (sql, parameters) = statement.ToSqliteRaw();
            
            // Act - Execute UPDATE against real database
            var dapperParams = parameters.ToDapperParameters();
            var updatedRows = connection.Execute(sql, dapperParams);
            
            // Assert
            Assert.Equal(1, updatedRows);
            
            // Verify the update worked
            var updatedCustomer = connection.QuerySingle<CustomerDto>("SELECT * FROM customers WHERE Id = 200");
            Assert.Equal(27, updatedCustomer.Age);
            Assert.Equal("John Smith", updatedCustomer.Name);
        });
    }

    [Fact]
    public void UpdateConditional_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        WithTransaction(connection =>
        {
            // Arrange
            var statement = TestStatements.UpdateConditional();
            var (sql, parameters) = statement.ToSqliteRaw();
            
            // Act - Execute UPDATE against real database
            var dapperParams = parameters.ToDapperParameters();
            var updatedRows = connection.Execute(sql, dapperParams);
            
            // Assert - Should update customers with age >= 18 and not named "Admin"
            Assert.True(updatedRows >= 1);
            
            // Verify at least one customer was updated (John Doe should now be 26)
            var johnDoe = connection.QuerySingle<CustomerDto>("SELECT * FROM customers WHERE Name = 'John Doe'");
            Assert.Equal(26, johnDoe.Age);
        });
    }

    [Fact]
    public void DeleteBasic_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        WithTransaction(connection =>
        {
            // Arrange - First insert the customer we'll delete using inline statement
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 200)
                .Value(c => c.Age, 25)
                .Value(c => c.Name, "John Doe");
            var (insertSql, insertParams) = insertStatement.ToSqliteRaw();
            var insertDapperParams = insertParams.ToDapperParameters();
            connection.Execute(insertSql, insertDapperParams);
            
            // Verify customer exists
            var existingCustomer = connection.QuerySingle<CustomerDto>("SELECT * FROM customers WHERE Id = 200");
            Assert.Equal("John Doe", existingCustomer.Name);
            
            var statement = TestStatements.DeleteBasic();
            var (sql, parameters) = statement.ToSqliteRaw();
            
            // Act - Execute DELETE against real database
            var dapperParams = parameters.ToDapperParameters();
            var deletedRows = connection.Execute(sql, dapperParams);
            
            // Assert - Should delete 1 customer
            Assert.Equal(1, deletedRows);
            
            // Verify the customer was deleted
            var remainingCount = connection.QuerySingle<int>("SELECT COUNT(*) FROM customers WHERE Id = 200");
            Assert.Equal(0, remainingCount);
        });
    }

    [Fact]
    public void DeleteConditional_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        WithTransaction(connection =>
        {
            // Arrange - Delete customers where Age < 18 OR Name = 'Temp'
            var statement = TestStatements.DeleteConditional();
            var (sql, parameters) = statement.ToSqliteRaw();
            
            // Act - Execute DELETE against real database
            var dapperParams = parameters.ToDapperParameters();
            var deletedRows = connection.Execute(sql, dapperParams);
            
            // Assert - Should delete 1 customer (Minor User with age 16)
            Assert.Equal(1, deletedRows);
            
            // Verify the correct customer was deleted
            var remainingCustomers = connection.Query<CustomerDto>("SELECT * FROM customers").ToList();
            Assert.Equal(3, remainingCustomers.Count);
            Assert.DoesNotContain(remainingCustomers, c => c.Name == "Minor User");
            Assert.Contains(remainingCustomers, c => c.Name == "John Doe");
            Assert.Contains(remainingCustomers, c => c.Name == "Jane Smith");
            Assert.Contains(remainingCustomers, c => c.Name == "Senior User");
        });
    }

    [Fact]
    public void DeleteAll_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        WithTransaction(connection =>
        {
            // Arrange - Delete all customers (need to delete orders first due to foreign key)
            // First delete all orders to avoid foreign key constraint
            connection.Execute("DELETE FROM orders");
            
            var statement = TestStatements.DeleteAll();
            var (sql, parameters) = statement.ToSqliteRaw();
            
            // Act - Execute DELETE against real database
            var dapperParams = parameters.ToDapperParameters();
            var deletedRows = connection.Execute(sql, dapperParams);
            
            // Assert - Should delete all 4 customers
            Assert.Equal(4, deletedRows);
            
            // Verify all customers were deleted
            var remainingCustomers = connection.Query<CustomerDto>("SELECT * FROM customers").ToList();
            Assert.Empty(remainingCustomers);
        });
    }

    [Fact]
    public void UpdateSetNull_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        WithTransaction(connection =>
        {
            // Arrange
            var statement = TestStatements.UpdateSetNull();
            var (sql, parameters) = statement.ToSqliteRaw();
            
            // Act - Execute UPDATE against real database
            var dapperParams = parameters.ToDapperParameters();
            var updatedRows = connection.Execute(sql, dapperParams);
            
            // Assert - Should update all customers (4 in test data)
            Assert.Equal(4, updatedRows);
            
            // Verify all customer names are now null
            var customers = connection.Query<CustomerDto>("SELECT * FROM customers");
            Assert.All(customers, c => Assert.Null(c.Name)); // SQLite also stores NULL as null now
        });
    }

    [Fact]
    public void UpdateSetNullInt_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        WithTransaction(connection =>
        {
            // Arrange - Set all customers' ages to NULL
            var statement = TestStatements.UpdateSetNullInt();
            var (sql, parameters) = statement.ToSqliteRaw();
            
            // Act - Execute UPDATE against real database
            var dapperParams = parameters.ToDapperParameters();
            var updatedRows = connection.Execute(sql, dapperParams);
            
            // Assert - Should update all customers
            Assert.Equal(4, updatedRows);
            
            // Verify the update worked - all ages should be NULL
            var customers = connection.Query<CustomerDto>("SELECT * FROM customers").ToList();
            Assert.All(customers, c => Assert.Null(c.Age)); // Age is now int?, so NULL remains null
        });
    }

    [Fact]
    public void UpdateSetNullMixed_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        WithTransaction(connection =>
        {
            // Arrange - Set name to "John" and age to NULL
            var statement = TestStatements.UpdateSetNullMixed();
            var (sql, parameters) = statement.ToSqliteRaw();
            
            // Act - Execute UPDATE against real database
            var dapperParams = parameters.ToDapperParameters();
            var updatedRows = connection.Execute(sql, dapperParams);
            
            // Assert - Should update all customers
            Assert.Equal(4, updatedRows);
            
            // Verify the update worked
            var customers = connection.Query<CustomerDto>("SELECT * FROM customers").ToList();
            Assert.All(customers, c => Assert.Equal("John", c.Name));
            Assert.All(customers, c => Assert.Null(c.Age)); // Age is now int?, so NULL remains null
        });
    }

    [Fact]
    public void UpdateSetNullWhere_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        WithTransaction(connection =>
        {
            // Arrange - First insert the customer we'll update using inline statement
            var insertStatement = TypedSql.Insert<Customer>()
                .Value(c => c.Id, 200)
                .Value(c => c.Age, 25)
                .Value(c => c.Name, "John Doe");
            var (insertSql, insertParams) = insertStatement.ToSqliteRaw();
            var insertDapperParams = insertParams.ToDapperParameters();
            connection.Execute(insertSql, insertDapperParams);
            
            // Set name to NULL for customer with Id = 200
            var statement = TestStatements.UpdateSetNullWhere();
            var (sql, parameters) = statement.ToSqliteRaw();
            
            // Act - Execute UPDATE against real database
            var dapperParams = parameters.ToDapperParameters();
            var updatedRows = connection.Execute(sql, dapperParams);
            
            // Assert - Should update 1 customer
            Assert.Equal(1, updatedRows);
            
            // Verify the update worked
            var updatedCustomer = connection.QuerySingle<CustomerDto>("SELECT * FROM customers WHERE Id = 200");
            Assert.Null(updatedCustomer.Name); // SQLite also stores NULL as null now
        });
    }

    [Fact]
    public void InsertWithNull_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        WithTransaction(connection =>
        {
            // Arrange
            var statement = TestStatements.InsertWithNull();
            var (sql, parameters) = statement.ToSqliteRaw();
            
            // Act - Execute INSERT against real database
            var dapperParams = parameters.ToDapperParameters();
            var insertedRows = connection.Execute(sql, dapperParams);
            
            // Assert
            Assert.Equal(1, insertedRows);
            
            // Verify the insert worked and Name is NULL
            var insertedCustomer = connection.QuerySingle<CustomerDto>("SELECT * FROM customers WHERE Id = 202");
            Assert.Null(insertedCustomer.Name); // SQLite also stores NULL as null now
        });
    }

    [Fact]
    public void InsertWithNullInt_ExecutesAgainstDatabase()
    {
        // Use transaction to ensure test isolation while executing real SQL
        WithTransaction(connection =>
        {
            // Arrange
            var statement = TestStatements.InsertWithNullInt();
            var (sql, parameters) = statement.ToSqliteRaw();
            
            // Act - Execute INSERT against real database
            var dapperParams = parameters.ToDapperParameters();
            var insertedRows = connection.Execute(sql, dapperParams);
            
            // Assert
            Assert.Equal(1, insertedRows);
            
            // Verify the insert worked and Age is NULL (default 0 for int)
            var insertedCustomer = connection.QuerySingle<CustomerDto>("SELECT * FROM customers WHERE Id = 203");
            Assert.Equal("John", insertedCustomer.Name); // Name should be "John"
            Assert.Null(insertedCustomer.Age); // Age should be NULL
        });
    }
}
