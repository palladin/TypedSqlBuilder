using System.Collections.Immutable;
using TypedSqlBuilder.Core;
using TypedSqlBuilder.TestModels;

namespace TypedSqlBuilder.Tests;

/// <summary>
/// SQLite-specific tests for INSERT, UPDATE, and DELETE statements using TestStatements
/// </summary>
public class SqliteStatementsTests : IStatementTestContract, ISqliteDialectTestContract
{
    [Fact]
    public Task InsertBasic_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.InsertBasic();

        // Act
        var (sql, parameters) = statement.ToSqliteRaw();

        // Assert
        Assert.Equal("INSERT INTO customers (Id, Age, Name) VALUES (:p0, :p1, :p2)", sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(200, parameters[":p0"]);
        Assert.Equal(25, parameters[":p1"]);
        Assert.Equal("John Doe", parameters[":p2"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task UpdateBasic_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.UpdateBasic();

        // Act
        var (sql, parameters) = statement.ToSqliteRaw();

        // Assert
        Assert.Equal("UPDATE customers SET Age = :p0 WHERE customers.Id = :p1", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(26, parameters[":p0"]);
        Assert.Equal(200, parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DeleteBasic_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.DeleteBasic();

        // Act
        var (sql, parameters) = statement.ToSqliteRaw();

        // Assert
        Assert.Equal("DELETE FROM customers WHERE customers.Id = :p0", sql);
        Assert.Single(parameters);
        Assert.Equal(200, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DeleteAll_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.DeleteAll();

        // Act
        var (sql, parameters) = statement.ToSqliteRaw();

        // Assert
        Assert.Equal("DELETE FROM customers", sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task UpdateConditional_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.UpdateConditional();

        // Act
        var (sql, parameters) = statement.ToSqliteRaw();

        // Assert
        Assert.Equal("UPDATE customers SET Age = (customers.Age + :p0) WHERE (customers.Age >= :p1) AND (customers.Name != :p2)", sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(1, parameters[":p0"]);
        Assert.Equal(18, parameters[":p1"]);
        Assert.Equal("Admin", parameters[":p2"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task InsertPartial_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.InsertPartial();

        // Act
        var (sql, parameters) = statement.ToSqliteRaw();

        // Assert
        Assert.Equal("INSERT INTO customers (Age, Name) VALUES (:p0, :p1)", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(28, parameters[":p0"]);
        Assert.Equal("Partial Customer", parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task UpdateMultiple_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.UpdateMultiple();

        // Act
        var (sql, parameters) = statement.ToSqliteRaw();

        // Assert
        Assert.Equal("UPDATE customers SET Age = :p0, Name = :p1 WHERE customers.Id = :p2", sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(27, parameters[":p0"]);
        Assert.Equal("John Smith", parameters[":p1"]);
        Assert.Equal(200, parameters[":p2"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DeleteConditional_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.DeleteConditional();

        // Act
        var (sql, parameters) = statement.ToSqliteRaw();

        // Assert
        Assert.Equal("DELETE FROM customers WHERE (customers.Age < :p0) OR (customers.Name = :p1)", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal("Temp", parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task Sqlite_UsesColonPrefix()
    {
        // Arrange - using inline statement to verify colon prefix behavior
        var statement = TypedSql.Insert<Customer>()
            .Value(c => c.Id, 200)
            .Value(c => c.Age, 25)
            .Value(c => c.Name, "John Doe");

        // Act
        var (sql, parameters) = statement.ToSqliteRaw();

        // Assert
        Assert.Equal("INSERT INTO customers (Id, Age, Name) VALUES (:p0, :p1, :p2)", sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(200, parameters[":p0"]);
        Assert.Equal(25, parameters[":p1"]);
        Assert.Equal("John Doe", parameters[":p2"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task UpdateSetNull_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.UpdateSetNull();
        
        // Act
        var (sql, parameters) = statement.ToSqliteRaw();
        
        // Assert
        Assert.Equal("UPDATE customers SET Name = NULL", sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task UpdateSetNullInt_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.UpdateSetNullInt();
        
        // Act
        var (sql, parameters) = statement.ToSqliteRaw();
        
        // Assert
        Assert.Equal("UPDATE customers SET Age = NULL", sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task UpdateSetNullMixed_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.UpdateSetNullMixed();
        
        // Act
        var (sql, parameters) = statement.ToSqliteRaw();
        
        // Assert
        Assert.Equal("UPDATE customers SET Name = :p0, Age = NULL", sql);
        Assert.Single(parameters);
        Assert.Equal("John", parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task UpdateSetNullWhere_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.UpdateSetNullWhere();
        
        // Act
        var (sql, parameters) = statement.ToSqliteRaw();
        
        // Assert
        Assert.Equal("UPDATE customers SET Name = NULL WHERE customers.Id = :p0", sql);
        Assert.Single(parameters);
        Assert.Equal(200, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task InsertWithNull_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.InsertWithNull();
        
        // Act
        var (sql, parameters) = statement.ToSqliteRaw();
        
        // Assert
        Assert.Equal("INSERT INTO customers (Id, Name, Age) VALUES (:p0, NULL, :p1)", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(202, parameters[":p0"]);
        Assert.Equal(25, parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task InsertWithNullInt_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.InsertWithNullInt();
        
        // Act
        var (sql, parameters) = statement.ToSqliteRaw();
        
        // Assert
        Assert.Equal("INSERT INTO customers (Id, Name, Age) VALUES (:p0, :p1, NULL)", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(203, parameters[":p0"]);
        Assert.Equal("John", parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public void InsertNewCustomer_GeneratesSqlCorrectly()
    {
        // Arrange
        var statement = TypedSql.Insert<Customer>()
            .Value(c => c.Id, 100)
            .Value(c => c.Age, 35)
            .Value(c => c.Name, "New Customer");
        
        // Act
        var (sql, parameters) = statement.ToSqliteRaw();
        
        // Assert
        Assert.Equal("INSERT INTO customers (Id, Age, Name) VALUES (:p0, :p1, :p2)", sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(100, parameters[":p0"]);
        Assert.Equal(35, parameters[":p1"]);
        Assert.Equal("New Customer", parameters[":p2"]);
    }
}
