using System.Collections.Immutable;
using TypedSqlBuilder.Core;
using TypedSqlBuilder.TestModels;

namespace TypedSqlBuilder.Tests;

/// <summary>
/// SQLite-specific tests using queries from TestQueries
/// Implements IQueryTestContract and ISqliteDialectTestContract to ensure consistent test coverage
/// </summary>
public class SqliteQueryTests : IQueryTestContract, ISqliteDialectTestContract
{
    [Fact]
    public Task From_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.From();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

            [Fact]
    public Task FromStatic_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromStatic();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromSelectSingle_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelectSingle();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Age AS Age
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereInt_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereInt();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(18, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereString_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereString();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Name = :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal("John", parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereMultiple_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereMultiple();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0 AND a0.Name != :p1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal("Admin", parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromOrderByAsc_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByAsc();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromOrderByDesc_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByDesc();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        ORDER BY 
            a0.Age DESC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereSelectOrderBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereSelectOrderBy();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - SQLite uses || for string concatenation instead of CONCAT
        var expectedSql = """
        SELECT 
            a0.Id + :p1 AS Proj0,
            a0.Name || :p2 AS Proj1
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0
        ORDER BY 
            a0.Name ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal(1, parameters[":p1"]);
        Assert.Equal("!", parameters[":p2"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Age >= :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(21, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereOr_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereOr();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0 AND a0.Age < :p1 OR a0.Name = :p2
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal(65, parameters[":p1"]);
        Assert.Equal("VIP", parameters[":p2"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromSelectExpression_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelectExpression();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - SQLite uses || for string concatenation instead of CONCAT
        var expectedSql = """
        SELECT 
            a0.Id * :p0 + a0.Age AS Proj0,
            a0.Name || :p1 AS Proj1
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(100, parameters[":p0"]);
        Assert.Equal(" - Customer", parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereOrderBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereOrderBy();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0 AND a0.Name != :p1
        ORDER BY 
            a0.Age ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(21, parameters[":p0"]);
        Assert.Equal("", parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereOrderBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereOrderBySelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a0.Age + :p2 AS Proj0
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0 AND a0.Name != :p1
        ORDER BY 
            a0.Age ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(21, parameters[":p0"]);
        Assert.Equal("", parameters[":p1"]);
        Assert.Equal(10, parameters[":p2"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task Sqlite_UsesColonPrefix()
    {
        // Arrange
        var query = TestQueries.FromWhereAnd();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0 AND a0.Name = :p1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal("John", parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereOrderBySelectNamed_GeneratesExpectedSql()
    {
        // Arrange
        var query = TestQueries.FromWhereOrderBySelectNamed();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - SQLite uses || for string concatenation instead of CONCAT
        var expectedSql = """
        SELECT 
            a0.Id AS CustomerId,
            a0.Name || :p2 AS CustomerInfo,
            a0.Age + :p3 AS AdjustedAge
        FROM 
            customers a0
        WHERE 
            a0.Age >= :p0 AND a0.Name != :p1
        ORDER BY 
            a0.Name ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(4, parameters.Count);
        Assert.Equal(21, parameters[":p0"]);
        Assert.Equal("", parameters[":p1"]);
        Assert.Equal(" (Customer)", parameters[":p2"]);
        Assert.Equal(5, parameters[":p3"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromProductWhereSelect_WorksCorrectly()
    {
        // Arrange
        var query = TestQueries.FromProductWhereSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.ProductName AS ProductName
        FROM 
            products a0
        WHERE 
            a0.ProductName != :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal("Discontinued", parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(18, 65)]
    [InlineData(13, 17)]
    [InlineData(66, 120)]
    public Task FromWhereSelectParameterized_WorkWithDifferentValues(int minAge, int maxAge)
    {
        // Arrange
        var query = TestQueries.FromWhereSelectParameterized(minAge, maxAge);
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - The structure should be consistent regardless of parameter values
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Age >= :p0 AND a0.Age <= :p1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(minAge, parameters[":p0"]);
        Assert.Equal(maxAge, parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereSelectNamed_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereSelectNamed();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS OriginalId,
            a0.Id * :p1 + a0.Age AS ModifiedId,
            a0.Name AS CustomerName
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal(100, parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromSelectOrderBy_ProducesCorrectResults()
    {
        // Arrange
        var query = TestQueries.FromSelectOrderBy();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a0.Age + :p0 AS Proj0
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereAndSelect_WorksCorrectly()
    {
        // Arrange
        var query = TestQueries.FromWhereAndSelect();

        // Act
        var (sql, parameters) = query.ToSqliteRaw();

        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Age >= :p0 AND a0.Name != :p1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(21, parameters[":p0"]);
        Assert.Equal("", parameters[":p1"]);
        Assert.True(true, "Clean architecture with extension methods works perfectly!");
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereFusionTwo_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereFusionTwo();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0 AND a0.Name != :p1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal("Admin", parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereFusionThree_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereFusionThree();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0 AND a0.Name != :p1 AND a0.Age < :p2
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal("Admin", parameters[":p1"]);
        Assert.Equal(65, parameters[":p2"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereFusionWithSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereFusionWithSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Age >= :p0 AND a0.Name != :p1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(21, parameters[":p0"]);
        Assert.Equal("", parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereFusionWithOrderBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereFusionWithOrderBy();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0 AND a0.Name != :p1
        ORDER BY 
            a0.Name ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal("Admin", parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromOrderByThenBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByThenBy();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC, a0.Age ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromOrderByThenByDescending_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByThenByDescending();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC, a0.Age DESC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromOrderByDescendingThenBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByDescendingThenBy();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        ORDER BY 
            a0.Age DESC, a0.Name ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromOrderByMultiple_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByMultiple();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC, a0.Age DESC, a0.Id ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereOrderByThenBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereOrderByThenBy();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0
        ORDER BY 
            a0.Name ASC, a0.Age DESC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(18, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromOrderByThenBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByThenBySelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC, a0.Age ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereIsNull_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereIsNull();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Name IS NULL
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereIsNotNull_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereIsNotNull();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Name IS NOT NULL
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereIsNullInt_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereIsNullInt();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age IS NULL
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereIsNotNullInt_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereIsNotNullInt();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age IS NOT NULL
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereIsNullCombined_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereIsNullCombined();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Name IS NULL AND a0.Age IS NOT NULL
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task SumAges_GeneratesCorrectSql()
    {
        // Arrange
        var sumExpr = TestQueries.SumAges();
        
        // Act
        var (sql, parameters) = sumExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            SUM(a0.Age) AS Proj0
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task CountCustomers_GeneratesCorrectSql()
    {
        // Arrange
        var countExpr = TestQueries.CountCustomers();
        
        // Act
        var (sql, parameters) = countExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            COUNT(*) AS Proj0
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task CountActiveCustomers_GeneratesCorrectSql()
    {
        // Arrange
        var countExpr = TestQueries.CountActiveCustomers();
        
        // Act
        var (sql, parameters) = countExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            COUNT(*) AS Proj0
        FROM 
            customers a0
        WHERE 
            a0.Age >= :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(18, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task SumAgesWithDb_GeneratesCorrectSql()
    {
        // Arrange
        var sumExpr = TestQueries.SumAgesWithDb();
        
        // Act
        var (sql, parameters) = sumExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            SUM(a0.Age) AS Proj0
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task CountCustomersWithDb_GeneratesCorrectSql()
    {
        // Arrange
        var countExpr = TestQueries.CountCustomersWithDb();
        
        // Act
        var (sql, parameters) = countExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            COUNT(*) AS Proj0
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task CountActiveCustomersWithDb_GeneratesCorrectSql()
    {
        // Arrange
        var countExpr = TestQueries.CountActiveCustomersWithDb();
        
        // Act
        var (sql, parameters) = countExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            COUNT(*) AS Proj0
        FROM 
            customers a0
        WHERE 
            a0.Age >= :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(18, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereAgeGreaterThanSum_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereAgeGreaterThanSum();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > (SELECT 
            SUM(a1.Age) AS Proj0
        FROM 
            customers a1)
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereAgeGreaterThanAverageAge_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereAgeGreaterThanAverageAge();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - Scalar query used as expression should have parentheses
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > (SELECT 
            SUM(a1.Age) AS Proj0
        FROM 
            customers a1)
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereAgeIn_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereAgeIn();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age IN (:p0, :p1, :p2, :p3)
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(4, parameters.Count);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal(21, parameters[":p1"]);
        Assert.Equal(25, parameters[":p2"]);
        Assert.Equal(30, parameters[":p3"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereAgeInSubquery_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereAgeInSubquery();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age IN (SELECT 
            a1.Age AS Age
        FROM 
            customers a1
        WHERE 
            a1.Name = :p0)
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal("VIP", parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereAgeInSubqueryWithClosure_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereAgeInSubqueryWithClosure();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - Closure semantics should capture outer scope variables
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age IN (SELECT 
            a1.Age AS Age
        FROM 
            customers a1
        WHERE 
            a1.Name = a0.Name || :p0)
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal("_VIP", parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromSubquery_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSubquery();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - Subquery in FROM clause should be wrapped in parentheses with alias and projection columns should use generated aliases
        var expectedSql = """
        SELECT 
            a1.Id AS Id,
            a1.NewAge AS NewAge
        FROM 
            (SELECT 
                a0.Id AS Id,
                a0.Age + :p0 AS NewAge
            FROM 
                customers a0) a1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(1, parameters[":p0"]);        
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereSelectWhereFromNested_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereSelectWhereFromNested();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - Nested subquery: select(where(select(where(from))))
        var expectedSql = """
        SELECT 
            a1.Id AS Id,
            a1.Name AS Name
        FROM 
            (SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0) a1
        WHERE 
            a1.Id > :p1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal(100, parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereSelectWhereNested_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereSelectWhereNested();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - Linear chain: from().where().select().where().select()
        var expectedSql = """
        SELECT 
            a1.Id AS Id,
            a1.Name AS Name
        FROM 
            (SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0) a1
        WHERE 
            a1.Id > :p1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal(100, parameters[":p1"]);
        return Task.CompletedTask;
    }
    [Fact]
    public Task FromGroupBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupBySelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Age AS Age,
            COUNT(*) AS Count
        FROM 
            customers a0
        GROUP BY 
            a0.Age
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromGroupByMultipleSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByMultipleSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Age AS Age,
            a0.Name AS Name,
            COUNT(*) AS Count
        FROM 
            customers a0
        GROUP BY 
            a0.Age, a0.Name
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromGroupByHavingSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByHavingSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Age AS Age,
            COUNT(*) AS Count
        FROM 
            customers a0
        GROUP BY 
            a0.Age
        HAVING 
            COUNT(*) > :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(1, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereGroupBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereGroupBySelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Age AS Age,
            COUNT(*) AS Count
        FROM 
            customers a0
        WHERE 
            a0.Age >= :p0
        GROUP BY 
            a0.Age
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(18, parameters[":p0"]);
        return Task.CompletedTask;
    }

    // JOIN Fusion Tests - Testing the new JoinClause fusion functionality
    [Fact]
    public Task MultipleInnerJoinsFusion_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.MultipleInnerJoinsFusion();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS CustomerId,
            a0.Name AS Name,
            a1.Id AS OrderId,
            a2.ProductName AS ProductName
        FROM 
            customers a0
        INNER JOIN orders a1 ON a0.Id = a1.CustomerId
        INNER JOIN products a2 ON a1.Amount = a2.ProductId
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task MixedJoinTypesFusion_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.MixedJoinTypesFusion();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS CustomerId,
            a0.Name AS Name,
            a1.Id AS OrderId,
            a2.ProductName AS ProductName
        FROM 
            customers a0
        INNER JOIN orders a1 ON a0.Id = a1.CustomerId
        LEFT JOIN products a2 ON a1.Amount = a2.ProductId
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task JoinFusionWithWhere_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.JoinFusionWithWhere();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert        
        var expectedSql = """
        SELECT 
            a1.Id AS Id,
            a1.Name AS Name,
            a2.Amount AS Amount,
            a3.ProductName AS ProductName
        FROM 
            (SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age >= :p0) a1
        INNER JOIN orders a2 ON a1.Id = a2.CustomerId
        INNER JOIN products a3 ON a2.Amount = a3.ProductId
        WHERE 
            a2.Amount > :p1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal(100, parameters[":p1"]);
        return Task.CompletedTask;
    }

    // Basic JOIN tests for parity with SQL Server tests
    [Fact]
    public Task InnerJoinBasic_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.InnerJoinBasic();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS CustomerId,
            a0.Name AS Name,
            a1.Id AS OrderId,
            a1.Amount AS Amount
        FROM 
            customers a0
        INNER JOIN orders a1 ON a0.Id = a1.CustomerId
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task InnerJoinWithSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.InnerJoinWithSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Name AS Name,
            a1.Amount AS Amount
        FROM 
            customers a0
        INNER JOIN orders a1 ON a0.Id = a1.CustomerId
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task InnerJoinWithWhere_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.InnerJoinWithWhere();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a1.Id AS CustomerId,
            a1.Name AS Name,
            a1.Age AS Age,
            a2.Id AS OrderId,
            a2.Amount AS Amount
        FROM 
            (SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age >= :p0) a1
        INNER JOIN orders a2 ON a1.Id = a2.CustomerId
        WHERE 
            a2.Amount > :p1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal(100, parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task InnerJoinWithOrderBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.InnerJoinWithOrderBy();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - now generates compact SQL without subquery
        var expectedSql = """
        SELECT 
            a0.Id AS CustomerId,
            a0.Name AS Name,
            a1.Id AS OrderId,
            a1.Amount AS Amount
        FROM 
            customers a0
        INNER JOIN orders a1 ON a0.Id = a1.CustomerId
        ORDER BY 
            a0.Name ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task LeftJoinBasic_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LeftJoinBasic();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS CustomerId,
            a0.Name AS Name,
            a1.Id AS OrderId,
            a1.Amount AS Amount
        FROM 
            customers a0
        LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task LeftJoinWithSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LeftJoinWithSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Name || :p0 AS Proj0,
            a1.Amount AS Amount
        FROM 
            customers a0
        LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(" (Customer)", parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task LeftJoinWithWhere_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LeftJoinWithWhere();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a1.Id AS CustomerId,
            a1.Name AS Name,
            a1.Age AS Age,
            a2.Id AS OrderId,
            a2.Amount AS Amount
        FROM 
            (SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age >= :p0) a1
        LEFT JOIN orders a2 ON a1.Id = a2.CustomerId
        WHERE 
            a1.Age < :p1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(21, parameters[":p0"]);
        Assert.Equal(65, parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task LeftJoinWithOrderBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LeftJoinWithOrderBy();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - now generates compact SQL without subquery
        var expectedSql = """
        SELECT 
            a0.Id AS CustomerId,
            a0.Name AS Name,
            a1.Id AS OrderId,
            a1.Amount AS Amount
        FROM 
            customers a0
        LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
        ORDER BY 
            a0.Name ASC, a1.Amount DESC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task InnerJoinWithGroupBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.InnerJoinWithGroupBy();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - now generates compact SQL without subquery
        var expectedSql = """
        SELECT 
            a0.Id AS CustomerId,
            a0.Name AS CustomerName,
            SUM(a1.Amount) AS TotalAmount
        FROM 
            customers a0
        INNER JOIN orders a1 ON a0.Id = a1.CustomerId
        GROUP BY 
            a0.Id, a0.Name
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task LeftJoinWithAggregates_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LeftJoinWithAggregates();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - now generates compact SQL without subquery
        var expectedSql = """
        SELECT 
            a0.Id AS CustomerId,
            a0.Name AS CustomerName,
            a0.Age AS CustomerAge,
            COUNT(*) AS OrderCount,
            SUM(a1.Amount) AS TotalSpent
        FROM 
            customers a0
        LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
        GROUP BY 
            a0.Id
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromGroupByOrderBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByOrderBySelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.CustomerId AS CustomerId,
            SUM(a0.Amount) AS TotalAmount
        FROM 
            orders a0
        GROUP BY 
            a0.CustomerId
        ORDER BY 
            SUM(a0.Amount) DESC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromGroupByOrderByMultipleSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByOrderByMultipleSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.CustomerId AS CustomerId,
            SUM(a0.Amount) AS TotalAmount,
            COUNT(*) AS OrderCount
        FROM 
            orders a0
        GROUP BY 
            a0.CustomerId
        ORDER BY 
            SUM(a0.Amount) DESC, COUNT(*) ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromGroupByOrderByThreeKeysSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByOrderByThreeKeysSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.CustomerId AS CustomerId,
            SUM(a0.Amount) AS TotalAmount,
            COUNT(*) AS OrderCount
        FROM 
            orders a0
        GROUP BY 
            a0.CustomerId
        ORDER BY 
            SUM(a0.Amount) DESC, COUNT(*) ASC, a0.CustomerId ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromGroupByMultipleOrderBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByMultipleOrderBySelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Age AS Age,
            a0.Name AS Name,
            COUNT(*) AS Count
        FROM 
            customers a0
        GROUP BY 
            a0.Age, a0.Name
        ORDER BY 
            COUNT(*) DESC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromGroupByHavingOrderBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByHavingOrderBySelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.CustomerId AS CustomerId,
            SUM(a0.Amount) AS TotalAmount
        FROM 
            orders a0
        GROUP BY 
            a0.CustomerId
        HAVING 
            COUNT(*) > :p0
        ORDER BY 
            SUM(a0.Amount) DESC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(1, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task ComplexJoinWhereGroupByHavingOrderBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.ComplexJoinWhereGroupByHavingOrderBySelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - This complex query combines JOIN, WHERE, GROUP BY, HAVING, ORDER BY
        var expectedSql = """
        SELECT 
            a0.Id AS CustomerId,
            a0.Name AS CustomerName,
            COUNT(*) AS TotalOrders,
            SUM(a1.Amount) AS TotalSpent,
            SUM(a1.Amount) / COUNT(*) AS AvgOrderValue
        FROM 
            customers a0
        INNER JOIN orders a1 ON a0.Id = a1.CustomerId
        WHERE 
            a0.Age >= :p0 AND a1.Amount > :p1
        GROUP BY 
            a0.Id, a0.Name
        HAVING 
            COUNT(*) > :p2 AND SUM(a1.Amount) > :p3
        ORDER BY 
            SUM(a1.Amount) DESC, COUNT(*) ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal(50, parameters[":p1"]);
        Assert.Equal(2, parameters[":p2"]);
        Assert.Equal(500, parameters[":p3"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task ComplexLeftJoinWhereGroupByOrderBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.ComplexLeftJoinWhereGroupByOrderBySelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - This complex query combines LEFT JOIN, WHERE, GROUP BY, ORDER BY
        var expectedSql = """
        SELECT 
            a0.Id AS CustomerId,
            a0.Name AS CustomerName,
            COUNT(*) AS OrderCount,
            SUM(a1.Amount) AS TotalSpent
        FROM 
            customers a0
        LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
        WHERE 
            a0.Age >= :p0
        GROUP BY 
            a0.Id, a0.Name
        ORDER BY 
            SUM(a1.Amount) DESC, a0.Name ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(21, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromGroupByMinMaxSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByMinMaxSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.CustomerId AS CustomerId,
            MIN(a0.Amount) AS MinAmount,
            MAX(a0.Amount) AS MaxAmount,
            COUNT(*) AS OrderCount
        FROM 
            orders a0
        GROUP BY 
            a0.CustomerId
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromGroupByAvgSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByAvgSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.CustomerId AS CustomerId,
            AVG(a0.Amount) AS AvgAmount,
            COUNT(*) AS OrderCount
        FROM 
            orders a0
        GROUP BY 
            a0.CustomerId
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromSelectSum_GeneratesCorrectSql()
    {
        // Arrange
        var scalarQuery = TestQueries.FromSelectSum();
        
        // Act
        var (sql, parameters) = scalarQuery.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            SUM(a0.Amount) AS Proj0
        FROM 
            orders a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromSelectAvg_GeneratesCorrectSql()
    {
        // Arrange
        var scalarQuery = TestQueries.FromSelectAvg();
        
        // Act
        var (sql, parameters) = scalarQuery.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            AVG(a0.Amount) AS Proj0
        FROM 
            orders a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromSelectMin_GeneratesCorrectSql()
    {
        // Arrange
        var scalarQuery = TestQueries.FromSelectMin();
        
        // Act
        var (sql, parameters) = scalarQuery.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            MIN(a0.Amount) AS Proj0
        FROM 
            orders a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromSelectMax_GeneratesCorrectSql()
    {
        // Arrange
        var scalarQuery = TestQueries.FromSelectMax();
        
        // Act
        var (sql, parameters) = scalarQuery.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            MAX(a0.Amount) AS Proj0
        FROM 
            orders a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task ParameterAsIntParam_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.ParameterAsIntParam();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Age > :minAge
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(":minAge"));
        return Task.CompletedTask;
    }

    [Fact]
    public Task ParameterAsStringParam_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.ParameterAsStringParam();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age
        FROM 
            customers a0
        WHERE 
            a0.Name = :customerName
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(":customerName"));
        return Task.CompletedTask;
    }

    [Fact]
    public Task ParameterAsBoolParam_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.ParameterAsBoolParam();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a0.Age AS Age
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0 = :isAdult
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.True(parameters.ContainsKey(":isAdult"));
        Assert.Equal(18, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task BoolColumnDirectComparison_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.BoolColumnDirectComparison();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a0.Age AS Age,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.IsActive = :isActive
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(":isActive"));
        return Task.CompletedTask;
    }

    [Fact]
    public Task BoolColumnLiteralTrue_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.BoolColumnLiteralTrue();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.IsActive = :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(1, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task BoolColumnLiteralFalse_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.BoolColumnLiteralFalse();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.IsActive = :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(0, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereAnd_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereAnd();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0 AND a0.Name = :p1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal("John", parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromOrderByMultipleOrderBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByMultiple();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC, a0.Age DESC, a0.Id ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]  
    public Task FromProductWhereSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromProductWhereSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.ProductName AS ProductName
        FROM 
            products a0
        WHERE 
            a0.ProductName != :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal("Discontinued", parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromSelectOrderBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelectOrderBy();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a0.Age + :p0 AS Proj0
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereAndSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereAndSelect();

        // Act
        var (sql, parameters) = query.ToSqliteRaw();

        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Age >= :p0 AND a0.Name != :p1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(21, parameters[":p0"]);
        Assert.Equal("", parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereSelectParameterized_GeneratesCorrectSql()
    {
        // Arrange  
        var query = TestQueries.FromWhereSelectParameterized(21, 65);
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Age >= :p0 AND a0.Age <= :p1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(21, parameters[":p0"]);
        Assert.Equal(65, parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task CaseStringExpression_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.CaseStringExpression();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            CASE WHEN a0.Age > :p0 THEN :p1 ELSE :p2 END AS Proj0
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal("Adult", parameters[":p1"]);
        Assert.Equal("Minor", parameters[":p2"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task CaseIntExpression_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.CaseIntExpression();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            CASE WHEN a0.Age > :p0 THEN :p1 ELSE :p2 END AS Proj0
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(65, parameters[":p0"]);
        Assert.Equal(1, parameters[":p1"]);
        Assert.Equal(0, parameters[":p2"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task CaseBoolExpression_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.CaseBoolExpression();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            CASE WHEN a0.Age > :p0 THEN a0.IsActive ELSE :p1 END AS Proj0
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal(0, parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task CaseInWhere_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.CaseInWhere();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            CASE WHEN a0.Age > :p0 THEN :p1 ELSE :p2 END = :p3
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal("Adult", parameters[":p1"]);
        Assert.Equal("Minor", parameters[":p2"]);
        Assert.Equal("Adult", parameters[":p3"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task LikeWildcard_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LikeWildcard();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Name LIKE :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal("Jo%", parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task LikeSingleChar_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LikeSingleChar();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Name LIKE :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal("J_n", parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task LikeBothWildcards_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LikeBothWildcards();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Name LIKE :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal("%o_n%", parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task LikeExact_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LikeExact();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Name LIKE :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal("John", parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task AbsColumn_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.AbsColumn();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            ABS(a0.Age) AS Proj0
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task AbsInWhere_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.AbsInWhere();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a0.Age AS Age
        FROM 
            customers a0
        WHERE 
            ABS(a0.Age) > :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(30, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task AbsExpression_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.AbsExpression();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            ABS(a0.Age - :p0) AS Proj0
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(50, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task AbsParameter_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.AbsParameter();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a0.Age AS Age
        FROM 
            customers a0
        WHERE 
            ABS(a0.Age) > ABS(:minAge)
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Null(parameters[":minAge"]);
        return Task.CompletedTask;
    }

    // ========== NEW COLUMN TYPES TESTS - DECIMAL ==========
    
    [Fact]
    public Task FromWhereDecimalComparison_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereDecimalComparison();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.ProductName AS ProductName,
            a0.Price AS Price,
            a0.CreatedDate AS CreatedDate,
            a0.UniqueId AS UniqueId
        FROM 
            products a0
        WHERE 
            a0.Price > :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(100.50m, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromSelectDecimalArithmetic_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelectDecimalArithmetic();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductName AS ProductName,
            a0.Price * :p0 AS Proj0,
            a0.Price + :p1 AS Proj1,
            a0.Price - :p2 AS Proj2
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(1.1m, parameters[":p0"]);
        Assert.Equal(10.0m, parameters[":p1"]);
        Assert.Equal(5.0m, parameters[":p2"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereDecimalIsNull_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereDecimalIsNull();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.ProductName AS ProductName,
            a0.Price AS Price,
            a0.CreatedDate AS CreatedDate,
            a0.UniqueId AS UniqueId
        FROM 
            products a0
        WHERE 
            a0.Price IS NULL
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereDecimalIsNotNull_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereDecimalIsNotNull();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.ProductName AS ProductName,
            a0.Price AS Price,
            a0.CreatedDate AS CreatedDate,
            a0.UniqueId AS UniqueId
        FROM 
            products a0
        WHERE 
            a0.Price IS NOT NULL
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task CaseDecimalExpression_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.CaseDecimalExpression();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductName AS ProductName,
            CASE WHEN a0.Price > :p0 THEN :p1 ELSE CASE WHEN a0.Price > :p2 THEN :p3 ELSE :p4 END END AS ExpensiveFlag
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(5, parameters.Count);
        Assert.Equal(1000m, parameters[":p0"]);
        Assert.Equal("Expensive", parameters[":p1"]);
        Assert.Equal(100m, parameters[":p2"]);
        Assert.Equal("Moderate", parameters[":p3"]);
        Assert.Equal("Cheap", parameters[":p4"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task ParameterAsDecimalParam_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.ParameterAsDecimalParam();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.ProductName AS ProductName,
            a0.Price AS Price,
            a0.CreatedDate AS CreatedDate,
            a0.UniqueId AS UniqueId
        FROM 
            products a0
        WHERE 
            a0.Price > :minPrice
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(":minPrice"));
        return Task.CompletedTask;
    }

    // ========== NEW COLUMN TYPES TESTS - DATETIME ==========

    [Fact]
    public Task FromWhereCreatedDateComparison_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereCreatedDateComparison();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.ProductName AS ProductName,
            a0.Price AS Price,
            a0.CreatedDate AS CreatedDate,
            a0.UniqueId AS UniqueId
        FROM 
            products a0
        WHERE 
            a0.CreatedDate > :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(new DateTime(2024, 1, 1), parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereCreatedDateIsNull_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereCreatedDateIsNull();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.ProductName AS ProductName,
            a0.Price AS Price,
            a0.CreatedDate AS CreatedDate,
            a0.UniqueId AS UniqueId
        FROM 
            products a0
        WHERE 
            a0.CreatedDate IS NULL
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereCreatedDateIsNotNull_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereCreatedDateIsNotNull();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.ProductName AS ProductName,
            a0.Price AS Price,
            a0.CreatedDate AS CreatedDate,
            a0.UniqueId AS UniqueId
        FROM 
            products a0
        WHERE 
            a0.CreatedDate IS NOT NULL
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromSelectCreatedDateMinMax_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelectCreatedDateMinMax();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductName AS ProductName,
            a0.CreatedDate AS EarliestDate,
            a0.CreatedDate AS LatestDate
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task CaseDateTimeExpression_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.CaseDateTimeExpression();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductName AS ProductName,
            CASE WHEN a0.CreatedDate < :p0 THEN :p1 ELSE CASE WHEN a0.CreatedDate < :p2 THEN :p3 ELSE :p4 END END AS Age
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(5, parameters.Count);
        Assert.Equal(new DateTime(2020, 1, 1), parameters[":p0"]);
        Assert.Equal("Old", parameters[":p1"]);
        Assert.Equal(new DateTime(2024, 1, 1), parameters[":p2"]);
        Assert.Equal("Recent", parameters[":p3"]);
        Assert.Equal("New", parameters[":p4"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task ParameterAsDateTimeParam_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.ParameterAsDateTimeParam();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.ProductName AS ProductName,
            a0.Price AS Price,
            a0.CreatedDate AS CreatedDate,
            a0.UniqueId AS UniqueId
        FROM 
            products a0
        WHERE 
            a0.CreatedDate > :startDate
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(":startDate"));
        return Task.CompletedTask;
    }

    // ========== NEW COLUMN TYPES TESTS - GUID ==========

    [Fact]
    public Task FromWhereUniqueIdEquals_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereUniqueIdEquals();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.ProductName AS ProductName,
            a0.Price AS Price,
            a0.CreatedDate AS CreatedDate,
            a0.UniqueId AS UniqueId
        FROM 
            products a0
        WHERE 
            a0.UniqueId = :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(Guid.Parse("12345678-1234-1234-1234-123456789012"), parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereUniqueIdNotEquals_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereUniqueIdNotEquals();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.ProductName AS ProductName,
            a0.Price AS Price,
            a0.CreatedDate AS CreatedDate,
            a0.UniqueId AS UniqueId
        FROM 
            products a0
        WHERE 
            a0.UniqueId != :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(Guid.Empty, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereUniqueIdIsNull_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereUniqueIdIsNull();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.ProductName AS ProductName,
            a0.Price AS Price,
            a0.CreatedDate AS CreatedDate,
            a0.UniqueId AS UniqueId
        FROM 
            products a0
        WHERE 
            a0.UniqueId IS NULL
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromWhereUniqueIdIsNotNull_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereUniqueIdIsNotNull();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.ProductName AS ProductName,
            a0.Price AS Price,
            a0.CreatedDate AS CreatedDate,
            a0.UniqueId AS UniqueId
        FROM 
            products a0
        WHERE 
            a0.UniqueId IS NOT NULL
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task CaseGuidExpression_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.CaseGuidExpression();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductName AS ProductName,
            CASE WHEN a0.UniqueId = :p0 THEN :p1 ELSE :p2 END AS Status
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(Guid.Empty, parameters[":p0"]);
        Assert.Equal("Empty", parameters[":p1"]);
        Assert.Equal("HasId", parameters[":p2"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task ParameterAsGuidParam_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.ParameterAsGuidParam();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.ProductName AS ProductName,
            a0.Price AS Price,
            a0.CreatedDate AS CreatedDate,
            a0.UniqueId AS UniqueId
        FROM 
            products a0
        WHERE 
            a0.UniqueId = :targetId
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(":targetId"));
        return Task.CompletedTask;
    }

    // Decimal Aggregate Tests
    [Fact]
    public Task SumPrices_GeneratesCorrectSql()
    {
        // Arrange
        var sumExpr = TestQueries.SumPrices();
        
        // Act
        var (sql, parameters) = sumExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            SUM(a0.Price) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task AvgPrices_GeneratesCorrectSql()
    {
        // Arrange
        var avgExpr = TestQueries.AvgPrices();
        
        // Act
        var (sql, parameters) = avgExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            AVG(a0.Price) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task MinPrice_GeneratesCorrectSql()
    {
        // Arrange
        var minExpr = TestQueries.MinPrice();
        
        // Act
        var (sql, parameters) = minExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            MIN(a0.Price) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task MaxPrice_GeneratesCorrectSql()
    {
        // Arrange
        var maxExpr = TestQueries.MaxPrice();
        
        // Act
        var (sql, parameters) = maxExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            MAX(a0.Price) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromGroupByDecimalAggregatesSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByDecimalAggregatesSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductName AS ProductName,
            SUM(a0.Price) AS TotalPrice,
            AVG(a0.Price) AS AvgPrice,
            MIN(a0.Price) AS MinPrice,
            MAX(a0.Price) AS MaxPrice,
            COUNT(*) AS ProductCount
        FROM 
            products a0
        GROUP BY 
            a0.ProductName
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task SumExpensivePrices_GeneratesCorrectSql()
    {
        // Arrange
        var sumExpr = TestQueries.SumExpensivePrices();
        
        // Act
        var (sql, parameters) = sumExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            SUM(a0.Price) AS Proj0
        FROM 
            products a0
        WHERE 
            a0.Price > :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(100m, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task AvgExpensivePrices_GeneratesCorrectSql()
    {
        // Arrange
        var avgExpr = TestQueries.AvgExpensivePrices();
        
        // Act
        var (sql, parameters) = avgExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            AVG(a0.Price) AS Proj0
        FROM 
            products a0
        WHERE 
            a0.Price > :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(100m, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromGroupByDecimalSumSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByDecimalSumSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductName AS ProductName,
            SUM(a0.Price) AS TotalPrice
        FROM 
            products a0
        GROUP BY 
            a0.ProductName
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FromGroupByDecimalAvgSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByDecimalAvgSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductName AS ProductName,
            AVG(a0.Price) AS AvgPrice
        FROM 
            products a0
        GROUP BY 
            a0.ProductName
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task StringSubstring_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.StringSubstring();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            SUBSTR(a0.ProductName, :p0, :p1) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(1, parameters[":p0"]);
        Assert.Equal(5, parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task StringUpper_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.StringUpper();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            UPPER(a0.ProductName) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task StringLower_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.StringLower();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            LOWER(a0.ProductName) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task StringTrim_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.StringTrim();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            TRIM(a0.ProductName) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task StringLength_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.StringLength();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            LENGTH(a0.ProductName) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task StringFunctionsInWhere_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.StringFunctionsInWhere();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            UPPER(a0.Name) = :p0 AND LENGTH(a0.Name) > :p1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal("JOHN", parameters[":p0"]);
        Assert.Equal(3, parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task StringFunctionsInSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.StringFunctionsInSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            UPPER(a0.Name) AS UpperName,
            LOWER(a0.Name) AS LowerName,
            TRIM(a0.Name) AS TrimmedName,
            LENGTH(a0.Name) AS NameLength,
            SUBSTR(a0.Name, :p0, :p1) AS FirstThree
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(1, parameters[":p0"]);
        Assert.Equal(3, parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DateTimeNow_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.DateTimeNow();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            datetime('now') AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DateTimeYear_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.DateTimeYear();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            CAST(strftime('%Y', a0.CreatedDate) AS INTEGER) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DateTimeMonth_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.DateTimeMonth();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            CAST(strftime('%m', a0.CreatedDate) AS INTEGER) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DateTimeDay_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.DateTimeDay();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            CAST(strftime('%d', a0.CreatedDate) AS INTEGER) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DateTimeAddDays_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.DateTimeAddDays();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            datetime(a0.CreatedDate, '+30 day') AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DateTimeAddMonths_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.DateTimeAddMonths();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            datetime(a0.CreatedDate, '+6 month') AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DateTimeAddYears_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.DateTimeAddYears();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            datetime(a0.CreatedDate, '+1 year') AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DateTimeDiffDays_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.DateTimeDiffDays();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            CAST((julianday(:p0) - julianday(a0.CreatedDate)) AS INTEGER) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DateTimeDiffMonths_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.DateTimeDiffMonths();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            CAST(((CAST(strftime('%Y', :p0) AS INTEGER) - CAST(strftime('%Y', a0.CreatedDate) AS INTEGER)) * 12 + (CAST(strftime('%m', :p0) AS INTEGER) - CAST(strftime('%m', a0.CreatedDate) AS INTEGER))) AS INTEGER) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DateTimeDiffYears_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.DateTimeDiffYears();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            CAST((CAST(strftime('%Y', :p0) AS INTEGER) - CAST(strftime('%Y', a0.CreatedDate) AS INTEGER)) AS INTEGER) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DateTimeFunctionsInWhere_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.DateTimeFunctionsInWhere();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.CreatedDate AS CreatedDate
        FROM 
            products a0
        WHERE 
            CAST(strftime('%Y', a0.CreatedDate) AS INTEGER) = :p0 AND CAST(strftime('%m', a0.CreatedDate) AS INTEGER) > :p1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(2024, parameters[":p0"]);
        Assert.Equal(6, parameters[":p1"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DateTimeFunctionsInSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.DateTimeFunctionsInSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            CAST(strftime('%Y', a0.CreatedDate) AS INTEGER) AS CreatedYear,
            CAST(strftime('%m', a0.CreatedDate) AS INTEGER) AS CreatedMonth,
            CAST(strftime('%d', a0.CreatedDate) AS INTEGER) AS CreatedDay,
            datetime(a0.CreatedDate, '+7 day') AS NextWeek,
            datetime(a0.CreatedDate, '+1 month') AS NextMonth,
            CAST((julianday(datetime('now')) - julianday(a0.CreatedDate)) AS INTEGER) AS DaysAgo
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters); // No parameters expected for SQLite as literals are embedded
        return Task.CompletedTask;
    }

    [Fact]
    public Task DecimalRound_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.DecimalRound();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            ROUND(a0.Price, :p0) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(2, parameters[":p0"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DecimalCeiling_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.DecimalCeiling();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            CAST((CASE WHEN a0.Price = CAST(a0.Price AS INTEGER) THEN a0.Price ELSE CAST(a0.Price AS INTEGER) + 1 END) AS REAL) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task DecimalFloor_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.DecimalFloor();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            CAST(CAST(a0.Price AS INTEGER) AS REAL) AS Proj0
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Fact]
    public Task MathFunctionsInWhere_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.MathFunctionsInWhere();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.Price AS Price
        FROM 
            products a0
        WHERE 
            ROUND(a0.Price, :p0) > :p1 AND CAST((CASE WHEN a0.Price = CAST(a0.Price AS INTEGER) THEN a0.Price ELSE CAST(a0.Price AS INTEGER) + 1 END) AS REAL) < :p2
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(0, parameters[":p0"]);
        Assert.Equal(100m, parameters[":p1"]); // Decimal literal
        Assert.Equal(1000, parameters[":p2"]);
        return Task.CompletedTask;
    }

    [Fact]
    public Task MathFunctionsInSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.MathFunctionsInSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.ProductId AS ProductId,
            a0.Price AS OriginalPrice,
            ROUND(a0.Price, :p0) AS RoundedPrice,
            CAST((CASE WHEN a0.Price = CAST(a0.Price AS INTEGER) THEN a0.Price ELSE CAST(a0.Price AS INTEGER) + 1 END) AS REAL) AS CeilingPrice,
            CAST(CAST(a0.Price AS INTEGER) AS REAL) AS FloorPrice
        FROM 
            products a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(2, parameters[":p0"]);
        return Task.CompletedTask;
    }
}
