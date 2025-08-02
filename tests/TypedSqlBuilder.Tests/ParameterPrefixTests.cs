using TypedSqlBuilder.Core;
using TypedSqlBuilder.Tests;

namespace TypedSqlBuilder.Tests;

/// <summary>
/// Tests to verify database-specific parameter prefixes
/// </summary>
public class ParameterPrefixTests
{
    [Fact]
    public void SqlServer_UsesAtSymbolPrefix()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age > 18 && c.Name == "John");
        
        // Act - Using SQL Server extension method
        var (sql, parameters) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Contains("@p0", sql);
        Assert.Contains("@p1", sql);
        Assert.Equal("SELECT * FROM customers WHERE (customers.Age > @p0) AND (customers.Name = @p1)", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(18, parameters["@p0"]);
        Assert.Equal("John", parameters["@p1"]);
    }

    [Fact]
    public void Sqlite_UsesColonPrefix()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age > 18 && c.Name == "John");
        
        // Act - Using SQLite extension method
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Contains(":p0", sql);
        Assert.Contains(":p1", sql);
        Assert.Equal("SELECT * FROM customers WHERE (customers.Age > :p0) AND (customers.Name = :p1)", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal("John", parameters[":p1"]);
    }

    [Fact]
    public void SqlServer_BooleanValues_UseIntegerParameters()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age > 18);
        
        // Act - Using SQL Server extension method  
        var (sql, parameters) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Contains("@p0", sql);
        Assert.Equal(18, parameters["@p0"]);
    }

    [Fact]
    public void Sqlite_BooleanValues_UseIntegerParameters()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age > 18);
        
        // Act - Using SQLite extension method
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Contains(":p0", sql);
        Assert.Equal(18, parameters[":p0"]);
    }
}
