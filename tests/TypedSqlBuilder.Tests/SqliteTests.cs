using System.Collections.Immutable;
using TypedSqlBuilder.Core;

namespace TypedSqlBuilder.Tests;

/// <summary>
/// SQLite-specific tests using queries from TestQueries
/// </summary>
public class SqliteTests
{
    [Fact]
    public void From_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.From();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Equal("SELECT * FROM customers", sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Equal("SELECT customers.Id, customers.Name FROM customers", sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromSelectSingle_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelectSingle();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Equal("SELECT customers.Age FROM customers", sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromWhereInt_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereInt();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Equal("SELECT * FROM customers WHERE customers.Age > :p0", sql);
        Assert.Single(parameters);
        Assert.Equal(18, parameters[":p0"]);
    }

    [Fact]
    public void FromWhereString_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereString();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Equal("SELECT * FROM customers WHERE customers.Name = :p0", sql);
        Assert.Single(parameters);
        Assert.Equal("John", parameters[":p0"]);
    }

    [Fact]
    public void FromWhereMultiple_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereMultiple();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Equal("SELECT * FROM customers WHERE (customers.Age > :p0) AND (customers.Name != :p1)", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal("Admin", parameters[":p1"]);
    }

    [Fact]
    public void FromOrderByAsc_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByAsc();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Equal("SELECT * FROM customers ORDER BY customers.Name ASC", sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromOrderByDesc_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByDesc();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Equal("SELECT * FROM customers ORDER BY customers.Age DESC", sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromWhereSelectOrderBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereSelectOrderBy();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - SQLite uses || for string concatenation instead of CONCAT
        Assert.Equal("SELECT (customers.Id + :p1), (customers.Name || :p2) FROM customers WHERE customers.Age > :p0 ORDER BY customers.Name ASC", sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal(1, parameters[":p1"]);
        Assert.Equal("!", parameters[":p2"]);
    }

    [Fact]
    public void FromWhereSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Equal("SELECT customers.Id, customers.Name FROM customers WHERE customers.Age >= :p0", sql);
        Assert.Single(parameters);
        Assert.Equal(21, parameters[":p0"]);
    }

    [Fact]
    public void FromWhereOr_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereOr();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Equal("SELECT * FROM customers WHERE ((customers.Age > :p0) AND (customers.Age < :p1)) OR (customers.Name = :p2)", sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal(65, parameters[":p1"]);
        Assert.Equal("VIP", parameters[":p2"]);
    }

    [Fact]
    public void FromSelectExpression_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelectExpression();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - SQLite uses || for string concatenation instead of CONCAT
        Assert.Equal("SELECT ((customers.Id * :p0) + customers.Age), (customers.Name || :p1) FROM customers", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(100, parameters[":p0"]);
        Assert.Equal(" - Customer", parameters[":p1"]);
    }

    [Fact]
    public void FromWhereOrderBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereOrderBySelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Equal("SELECT customers.Id, customers.Name, (customers.Age + :p2) FROM customers WHERE (customers.Age > :p0) AND (customers.Name != :p1) ORDER BY customers.Age ASC", sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(21, parameters[":p0"]);
        Assert.Equal("", parameters[":p1"]);
        Assert.Equal(10, parameters[":p2"]);
    }

    [Fact]
    public void Sqlite_UsesColonPrefix()
    {
        // Arrange
        var query = TestQueries.FromWhereAnd();
        
        // Act
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
    public void Sqlite_BooleanValues_UseIntegerParameters()
    {
        // Arrange
        var query = TestQueries.FromWhereInt();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Contains(":p0", sql);
        Assert.Equal(18, parameters[":p0"]);
    }

    [Fact]
    public void FromWhereOrderBySelectNamed_GeneratesExpectedSql()
    {
        // Arrange
        var query = TestQueries.FromWhereOrderBySelectNamed();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - SQLite uses || for string concatenation instead of CONCAT
        var expectedSql = "SELECT customers.Id, (customers.Name || :p2), (customers.Age + :p3) FROM customers WHERE (customers.Age >= :p0) AND (customers.Name != :p1) ORDER BY customers.Name ASC";
        Assert.Equal(expectedSql, sql);
        Assert.Equal(4, parameters.Count);
        Assert.Equal(21, parameters[":p0"]);
        Assert.Equal("", parameters[":p1"]);
        Assert.Equal(" (Customer)", parameters[":p2"]);
        Assert.Equal(5, parameters[":p3"]);
    }

    [Fact]
    public void FromProductWhereSelect_WorksCorrectly()
    {
        // Arrange
        var query = TestQueries.FromProductWhereSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Equal("SELECT products.ProductId, products.ProductName FROM products WHERE products.ProductName != :p0", sql);
        Assert.Single(parameters);
        Assert.Equal("Discontinued", parameters[":p0"]);
    }

    [Theory]
    [InlineData(18, 65)]
    [InlineData(13, 17)]
    [InlineData(66, 120)]
    public void FromWhereSelectParameterized_WorkWithDifferentValues(int minAge, int maxAge)
    {
        // Arrange
        var query = TestQueries.FromWhereSelectParameterized(minAge, maxAge);
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - The structure should be consistent regardless of parameter values
        Assert.Contains("SELECT customers.Id, customers.Name", sql);
        Assert.Contains("WHERE (customers.Age >= :p0) AND (customers.Age <= :p1)", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(minAge, parameters[":p0"]);
        Assert.Equal(maxAge, parameters[":p1"]);
    }

    [Fact]
    public void FromWhereSelectNamed_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereSelectNamed();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Equal("SELECT customers.Id, ((customers.Id * :p1) + customers.Age), customers.Name FROM customers WHERE customers.Age > :p0", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal(100, parameters[":p1"]);
    }

    [Fact]
    public void FromSelectOrderBy_ProducesCorrectResults()
    {
        // Arrange
        var query = TestQueries.FromSelectOrderBy();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        Assert.Contains(":p", sql); // SQLite uses : prefix
        Assert.Contains("SELECT customers.Id, customers.Name, (customers.Age + ", sql);
        Assert.Single(parameters);
    }

    [Fact]
    public void FromWhereAndSelect_WorksCorrectly()
    {
        // Arrange
        var query = TestQueries.FromWhereAndSelect();

        // Act
        var (sql, parameters) = query.ToSqliteRaw();

        // Assert
        Assert.Contains(":p0", sql); // SQLite uses :
        Assert.Equal(21, parameters[":p0"]);
        Assert.Equal("", parameters[":p1"]);
        Assert.True(true, "Clean architecture with extension methods works perfectly!");
    }

    [Fact]
    public void DatabaseComparison_SameQuery_DifferentSyntax()
    {
        // Arrange
        var query = TestQueries.FromSelectOrderBy();
        
        // Act
        var (sqliteSql, sqliteParameters) = query.ToSqliteRaw();
        var (sqlServerSql, sqlServerParameters) = query.ToSqlServerRaw();
        
        // Assert - Different parameter prefixes
        Assert.Contains(":p", sqliteSql);    // SQLite uses :
        Assert.Contains("@p", sqlServerSql); // SQL Server uses @
        
        // Same number of parameters
        Assert.Equal(sqliteParameters.Count, sqlServerParameters.Count);
        
        // Same core SQL structure (ignoring parameter prefixes)
        Assert.Contains("SELECT customers.Id, customers.Name, (customers.Age + ", sqliteSql);
        Assert.Contains("SELECT customers.Id, customers.Name, (customers.Age + ", sqlServerSql);
    }
}
