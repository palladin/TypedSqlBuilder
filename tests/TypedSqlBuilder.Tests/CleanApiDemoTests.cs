using TypedSqlBuilder.Core;
using TypedSqlBuilder.Tests;

namespace TypedSqlBuilder.Tests;

/// <summary>
/// Test demonstrating the new clean API design
/// </summary>
public class CleanApiDemoTests
{
    [Fact]
    public void NewCleanApi_OnlyRawMethods_WorkPerfectly()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age >= 21 && c.Name != "")
            .Select(c => (c.Id, c.Name));

        // Act - New clean API with consistent return types
        var (sqlServerSql, sqlServerParams) = query.ToSqlServerRaw();
        var (sqliteSql, sqliteParams) = query.ToSqliteRaw();

        // Assert - Both methods return the same clean structure
        Assert.IsType<string>(sqlServerSql);
        Assert.IsType<string>(sqliteSql);
        Assert.NotNull(sqlServerParams);
        Assert.NotNull(sqliteParams);
        
        // Different database syntax
        Assert.Contains("@p", sqlServerSql); // SQL Server uses @
        Assert.Contains(":p", sqliteSql);    // SQLite uses :
        
        // Same parameter count
        Assert.Equal(sqlServerParams.Count, sqliteParams.Count);
        
        // Parameters are easily accessible
        Assert.Equal(21, sqlServerParams["@p0"]);
        Assert.Equal(21, sqliteParams[":p0"]);
        Assert.Equal("", sqlServerParams["@p1"]);
        Assert.Equal("", sqliteParams[":p1"]);
    }
}
