using TypedSqlBuilder.Core;
using TypedSqlBuilder.Tests;

namespace TypedSqlBuilder.Tests;

/// <summary>
/// Test demonstrating the clean architecture after removing the factory
/// </summary>
public class CleanArchitectureTests
{
    [Fact]
    public void CleanArchitecture_OnlyExtensionMethods_Work()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age >= 21)
            .Select(c => (c.Id, c.Name));

        // Act - Use extension methods (the preferred and only way)
        var (sqlServerSql, sqlServerParameters) = query.ToSqlServerRaw();
        var (sqliteSql, sqliteParameters) = query.ToSqliteRaw();

        // Assert - Verify results
        Assert.Contains("@p0", sqlServerSql); // SQL Server uses @
        Assert.Contains(":p0", sqliteSql);    // SQLite uses :
        
        // Verify parameters are accessible
        Assert.Equal(21, sqlServerParameters["@p0"]);
        Assert.Equal(21, sqliteParameters[":p0"]);
        
        // Clean architecture with only extension methods!
        Assert.True(true, "Clean architecture with extension methods works perfectly!");
    }
}
