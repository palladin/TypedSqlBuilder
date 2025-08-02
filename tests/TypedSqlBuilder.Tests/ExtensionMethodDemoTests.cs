using TypedSqlBuilder.Core;
using TypedSqlBuilder.Tests;

namespace TypedSqlBuilder.Tests;

/// <summary>
/// Demonstration tests for the new extension methods
/// </summary>
public class ExtensionMethodDemoTests
{
    [Fact]
    public void ExtensionMethods_SqlServerAndSqlite_ProduceDifferentSyntax()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age >= 21 && c.Name != "")
            .OrderBy(c => c.Name)
            .Select(c => (c.Id, c.Name, c.Age + 5));

        // Act - Use new extension methods
        var (sqlServerSql, sqlServerParameters) = query.ToSqlServerRaw();
        var (sqliteSql, sqliteParameters) = query.ToSqliteRaw();

        // Assert - Different parameter prefixes
        Assert.Contains("@p", sqlServerSql); // SQL Server uses @ prefix
        Assert.Contains(":p", sqliteSql);    // SQLite uses : prefix
        
        // Same number of parameters
        Assert.Equal(sqlServerParameters.Count, sqliteParameters.Count);
        
        // Same core SQL structure (ignoring parameter prefixes)
        Assert.Contains("SELECT customers.Id, customers.Name, (customers.Age + ", sqlServerSql);
        Assert.Contains("SELECT customers.Id, customers.Name, (customers.Age + ", sqliteSql);
    }

    [Fact]
    public void ExtensionMethods_SimpleQueries_WorkWithoutParameters()
    {
        // Arrange
        var query = SqlQuery.From<Customer>().Select(c => (c.Id, c.Name));

        // Act - Use new extension methods for simple queries
        var (sqlServerSql, _) = query.ToSqlServerRaw();
        var (sqliteSql, _) = query.ToSqliteRaw();

        // Assert
        var expectedSql = "SELECT customers.Id, customers.Name FROM customers";
        Assert.Equal(expectedSql, sqlServerSql);
        Assert.Equal(expectedSql, sqliteSql); // Simple queries without parameters are the same
    }

    [Fact]
    public void ExtensionMethods_WorkCorrectlyForBothDialects()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age >= 21)
            .Select(c => (c.Id, c.Name));

        // Act - Use extension methods
        var (sqlServerSql, sqlServerParameters) = query.ToSqlServerRaw();
        var (sqliteSql, sqliteParameters) = query.ToSqliteRaw();

        // Assert
        Assert.Contains("@p0", sqlServerSql); // SQL Server uses @ prefix
        Assert.Contains(":p0", sqliteSql);    // SQLite uses : prefix
        Assert.Equal(21, sqlServerParameters["@p0"]);
        Assert.Equal(21, sqliteParameters[":p0"]);
    }
}
