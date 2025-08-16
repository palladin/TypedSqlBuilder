using System.Collections.Immutable;
using TypedSqlBuilder.Core;
using TypedSqlBuilder.TestModels;

namespace TypedSqlBuilder.Tests;

/// <summary>
/// SQL Server-specific tests for INSERT, UPDATE, and DELETE statements using TestStatements
/// </summary>
public class SqlServerStatementsTests
{
    [Fact]
    public void InsertBasic_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.InsertBasic();

        // Act
        var (sql, parameters) = statement.ToSqlServerRaw();

        // Assert
        Assert.Equal("INSERT INTO customers (Id, Age, Name) VALUES (@p0, @p1, @p2)", sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(1, parameters["@p0"]);
        Assert.Equal(25, parameters["@p1"]);
        Assert.Equal("John Doe", parameters["@p2"]);
    }

    [Fact]
    public void UpdateBasic_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.UpdateBasic();

        // Act
        var (sql, parameters) = statement.ToSqlServerRaw();

        // Assert
        Assert.Equal("UPDATE customers SET customers.Age = @p0 WHERE customers.Id = @p1", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(26, parameters["@p0"]);
        Assert.Equal(1, parameters["@p1"]);
    }

    [Fact]
    public void DeleteBasic_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.DeleteBasic();

        // Act
        var (sql, parameters) = statement.ToSqlServerRaw();

        // Assert
        Assert.Equal("DELETE FROM customers WHERE customers.Id = @p0", sql);
        Assert.Single(parameters);
        Assert.Equal(1, parameters["@p0"]);
    }

    [Fact]
    public void DeleteAll_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.DeleteAll();

        // Act
        var (sql, parameters) = statement.ToSqlServerRaw();

        // Assert
        Assert.Equal("DELETE FROM customers", sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void UpdateConditional_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.UpdateConditional();

        // Act
        var (sql, parameters) = statement.ToSqlServerRaw();

        // Assert
        Assert.Equal("UPDATE customers SET customers.Age = (customers.Age + @p0) WHERE (customers.Age >= @p1) AND (customers.Name != @p2)", sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(1, parameters["@p0"]);
        Assert.Equal(18, parameters["@p1"]);
        Assert.Equal("Admin", parameters["@p2"]);
    }

    [Fact]
    public void InsertPartial_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.InsertPartial();

        // Act
        var (sql, parameters) = statement.ToSqlServerRaw();

        // Assert
        Assert.Equal("INSERT INTO customers (Age, Name) VALUES (@p0, @p1)", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(30, parameters["@p0"]);
        Assert.Equal("Jane Smith", parameters["@p1"]);
    }

    [Fact]
    public void UpdateMultiple_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.UpdateMultiple();

        // Act
        var (sql, parameters) = statement.ToSqlServerRaw();

        // Assert
        Assert.Equal("UPDATE customers SET customers.Age = @p0, customers.Name = @p1 WHERE customers.Id = @p2", sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(27, parameters["@p0"]);
        Assert.Equal("John Smith", parameters["@p1"]);
        Assert.Equal(1, parameters["@p2"]);
    }

    [Fact]
    public void DeleteConditional_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.DeleteConditional();

        // Act
        var (sql, parameters) = statement.ToSqlServerRaw();

        // Assert
        Assert.Equal("DELETE FROM customers WHERE (customers.Age < @p0) OR (customers.Name = @p1)", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(18, parameters["@p0"]);
        Assert.Equal("Temp", parameters["@p1"]);
    }

    [Fact]
    public void SqlServer_UsesAtSymbolPrefix()
    {
        // Arrange
        var statement = TestStatements.InsertBasic();

        // Act
        var (sql, parameters) = statement.ToSqlServerRaw();

        // Assert
        Assert.Equal("INSERT INTO customers (Id, Age, Name) VALUES (@p0, @p1, @p2)", sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(1, parameters["@p0"]);
        Assert.Equal(25, parameters["@p1"]);
        Assert.Equal("John Doe", parameters["@p2"]);
    }

    [Fact]
    public void UpdateSetNull_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.UpdateSetNull();
        
        // Act
        var (sql, parameters) = statement.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("UPDATE customers SET customers.Name = NULL", sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void UpdateSetNullMixed_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.UpdateSetNullMixed();
        
        // Act
        var (sql, parameters) = statement.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("UPDATE customers SET customers.Name = @p0, customers.Age = NULL", sql);
        Assert.Single(parameters);
        Assert.Equal("John", parameters["@p0"]);
    }

    [Fact]
    public void UpdateSetNullInt_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.UpdateSetNullInt();
        
        // Act
        var (sql, parameters) = statement.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("UPDATE customers SET customers.Age = NULL", sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void UpdateSetNullWhere_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.UpdateSetNullWhere();
        
        // Act
        var (sql, parameters) = statement.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("UPDATE customers SET customers.Name = NULL WHERE customers.Id = @p0", sql);
        Assert.Single(parameters);
        Assert.Equal(1, parameters["@p0"]);
    }

    [Fact]
    public void InsertWithNull_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.InsertWithNull();
        
        // Act
        var (sql, parameters) = statement.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("INSERT INTO customers (Id, Name, Age) VALUES (@p0, NULL, @p1)", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(1, parameters["@p0"]);
        Assert.Equal(25, parameters["@p1"]);
    }

    [Fact]
    public void InsertWithNullInt_GeneratesCorrectSql()
    {
        // Arrange
        var statement = TestStatements.InsertWithNullInt();
        
        // Act
        var (sql, parameters) = statement.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("INSERT INTO customers (Id, Name, Age) VALUES (@p0, @p1, NULL)", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(1, parameters["@p0"]);
        Assert.Equal("John", parameters["@p1"]);
    }
}
