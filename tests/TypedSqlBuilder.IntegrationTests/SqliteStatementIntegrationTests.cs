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
        // Arrange
        var insertStatement = TestStatements.InsertNewCustomer();
        var (insertSql, insertParams) = insertStatement.ToSqliteRaw();
        
        // Act - Execute INSERT with Dapper
        var dapperInsertParams = insertParams.ToDapperParameters();
        var insertedRows = _connection.Execute(insertSql, dapperInsertParams);

        // Assert
        Assert.Equal(1, insertedRows);

        // Verify the insert worked by querying
        var insertedCustomer = _connection.QuerySingle<CustomerDto>(
            "SELECT * FROM customers WHERE Id = 100");
        Assert.Equal("New Customer", insertedCustomer.Name);
        Assert.Equal(35, insertedCustomer.Age);
    }

    [Fact]
    public void DeleteStatement_ExecutesCorrectly()
    {
        // Arrange - First insert a customer to delete
        var insertStatement = TestStatements.InsertNewCustomer();
        var (insertSql, insertParams) = insertStatement.ToSqliteRaw();
        
        var dapperInsertParams = insertParams.ToDapperParameters();
        _connection.Execute(insertSql, dapperInsertParams);

        // Verify customer exists
        var existingCount = _connection.QuerySingle<int>("SELECT COUNT(*) FROM customers WHERE Id = 100");
        Assert.Equal(1, existingCount);

        // Act - Execute DELETE statement
        var deleteStatement = TestStatements.DeleteNewCustomer();
        var (deleteSql, deleteParams) = deleteStatement.ToSqliteRaw();
        
        var dapperDeleteParams = deleteParams.ToDapperParameters();
        var deletedRows = _connection.Execute(deleteSql, dapperDeleteParams);

        // Assert
        Assert.Equal(1, deletedRows);

        // Verify the delete worked
        var remainingCount = _connection.QuerySingle<int>("SELECT COUNT(*) FROM customers WHERE Id = 100");
        Assert.Equal(0, remainingCount);
    }

    [Fact]
    public void InsertBasic_ExecutesCorrectly()
    {
        // Arrange
        var insertStatement = TestStatements.InsertBasic();
        var (sql, parameters) = insertStatement.ToSqliteRaw();

        // This will conflict with existing ID 1, so we expect it to fail
        var dapperParams = parameters.ToDapperParameters();

        // Act & Assert - Should throw because of unique constraint violation
        Assert.Throws<Microsoft.Data.Sqlite.SqliteException>(() => 
            _connection.Execute(sql, dapperParams));
    }

    [Fact]
    public void InsertPartial_ExecutesCorrectly()
    {
        // Arrange - Use a custom insert that doesn't specify ID (auto-increment)
        var insertStatement = TestStatements.InsertPartial();
        var (sql, parameters) = insertStatement.ToSqliteRaw();

        // Act - Execute INSERT with Dapper
        var dapperParams = parameters.ToDapperParameters();
        var insertedRows = _connection.Execute(sql, dapperParams);

        // Assert
        Assert.Equal(1, insertedRows);

        // Verify the insert worked - should be a new customer with auto-generated ID
        var insertedCustomers = _connection.Query<CustomerDto>(
            "SELECT * FROM customers WHERE Name = 'Jane Smith' AND Age = 30").ToList();
        Assert.Equal(2, insertedCustomers.Count); // Original + new one
    }

    [Fact]
    public void CombinedOperations_ExecuteCorrectly()
    {
        // Test a sequence of INSERT -> verify -> DELETE -> verify
        
        // 1. INSERT
        var insertStatement = TestStatements.InsertNewCustomer();
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
        var deleteStatement = TestStatements.DeleteNewCustomer();
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
}
