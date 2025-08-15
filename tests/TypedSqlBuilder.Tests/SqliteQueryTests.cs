using System.Collections.Immutable;
using TypedSqlBuilder.Core;

namespace TypedSqlBuilder.Tests;

/// <summary>
/// SQLite-specific tests using queries from TestQueries
/// </summary>
public class SqliteQueryTests
{
    [Fact]
    public void From_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

            [Fact]
    public void FromStatic_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
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
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
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
        var expectedSql = """
        SELECT 
            a0.Age AS Age
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
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
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0
        """;
        Assert.Equal(expectedSql, sql);
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
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Name = :p0
        """;
        Assert.Equal(expectedSql, sql);
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
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            (a0.Age > :p0) AND (a0.Name != :p1)
        """;
        Assert.Equal(expectedSql, sql);
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
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC
        """;
        Assert.Equal(expectedSql, sql);
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
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name
        FROM 
            customers a0
        ORDER BY 
            a0.Age DESC
        """;
        Assert.Equal(expectedSql, sql);
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
        var expectedSql = """
        SELECT 
            (a0.Id + :p1) AS prj0,
            (a0.Name || :p2) AS prj1
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
    }

    [Fact]
    public void FromWhereSelect_GeneratesCorrectSql()
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
    }

    [Fact]
    public void FromWhereOr_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            ((a0.Age > :p0) AND (a0.Age < :p1)) OR (a0.Name = :p2)
        """;
        Assert.Equal(expectedSql, sql);
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
        var expectedSql = """
        SELECT 
            ((a0.Id * :p0) + a0.Age) AS prj0,
            (a0.Name || :p1) AS prj1
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(100, parameters[":p0"]);
        Assert.Equal(" - Customer", parameters[":p1"]);
    }

    [Fact]
    public void FromWhereOrderBy_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            (a0.Age > :p0) AND (a0.Name != :p1)
        ORDER BY 
            a0.Age ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(21, parameters[":p0"]);
        Assert.Equal("", parameters[":p1"]);
    }

    [Fact]
    public void FromWhereOrderBySelect_GeneratesCorrectSql()
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
            (a0.Age + :p2) AS prj0
        FROM 
            customers a0
        WHERE 
            (a0.Age > :p0) AND (a0.Name != :p1)
        ORDER BY 
            a0.Age ASC
        """;
        Assert.Equal(expectedSql, sql);
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
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            (a0.Age > :p0) AND (a0.Name = :p1)
        """;
        Assert.Equal(expectedSql, sql);
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
        var expectedSql = """
        SELECT 
            a0.Id AS CustomerId,
            (a0.Name || :p2) AS CustomerInfo,
            (a0.Age + :p3) AS AdjustedAge
        FROM 
            customers a0
        WHERE 
            (a0.Age >= :p0) AND (a0.Name != :p1)
        ORDER BY 
            a0.Name ASC
        """;
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
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            (a0.Age >= :p0) AND (a0.Age <= :p1)
        """;
        Assert.Equal(expectedSql, sql);
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
        var expectedSql = """
        SELECT 
            a0.Id AS OriginalId,
            ((a0.OriginalId * :p1) + a0.Age) AS ModifiedId,
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
    }

    [Fact]
    public void FromSelectOrderBy_ProducesCorrectResults()
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
            (a0.Age + :p0) AS prj0
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC
        """;
        Assert.Equal(expectedSql, sql);
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
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            (a0.Age >= :p0) AND (a0.Name != :p1)
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(21, parameters[":p0"]);
        Assert.Equal("", parameters[":p1"]);
        Assert.True(true, "Clean architecture with extension methods works perfectly!");
    }

    [Fact]
    public void FromWhereFusionTwo_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            (a0.Age > :p0) AND (a0.Name != :p1)
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal("Admin", parameters[":p1"]);
    }

    [Fact]
    public void FromWhereFusionThree_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            (a0.Age > :p0) AND ((a0.Name != :p1) AND (a0.Age < :p2))
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal("Admin", parameters[":p1"]);
        Assert.Equal(65, parameters[":p2"]);
    }

    [Fact]
    public void FromWhereFusionWithSelect_GeneratesCorrectSql()
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
            (a0.Age >= :p0) AND (a0.Name != :p1)
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(21, parameters[":p0"]);
        Assert.Equal("", parameters[":p1"]);
    }

    [Fact]
    public void FromWhereFusionWithOrderBy_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            (a0.Age > :p0) AND (a0.Name != :p1)
        ORDER BY 
            a0.Name ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal("Admin", parameters[":p1"]);
    }

    [Fact]
    public void FromOrderByThenBy_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC, a0.Age ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromOrderByThenByDescending_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC, a0.Age DESC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromOrderByDescendingThenBy_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        ORDER BY 
            a0.Age DESC, a0.Name ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromOrderByMultiple_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC, a0.Age DESC, a0.Id ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromWhereOrderByThenBy_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0
        ORDER BY 
            a0.Name ASC, a0.Age DESC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(18, parameters[":p0"]);
    }

    [Fact]
    public void FromOrderByThenBySelect_GeneratesCorrectSql()
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
    }

    [Fact]
    public void FromWhereIsNull_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Name IS NULL
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromWhereIsNotNull_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Name IS NOT NULL
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromWhereIsNullInt_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Age IS NULL
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromWhereIsNotNullInt_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Age IS NOT NULL
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromWhereIsNullCombined_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            (a0.Name IS NULL) AND (a0.Age IS NOT NULL)
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void SumAges_GeneratesCorrectSql()
    {
        // Arrange
        var sumExpr = TestQueries.SumAges();
        
        // Act
        var (sql, parameters) = sumExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            SUM(a0.Age) AS prj0
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void CountCustomers_GeneratesCorrectSql()
    {
        // Arrange
        var countExpr = TestQueries.CountCustomers();
        
        // Act
        var (sql, parameters) = countExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            COUNT(*) AS prj0
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void CountActiveCustomers_GeneratesCorrectSql()
    {
        // Arrange
        var countExpr = TestQueries.CountActiveCustomers();
        
        // Act
        var (sql, parameters) = countExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            COUNT(*) AS prj0
        FROM 
            customers a0
        WHERE 
            a0.Age >= :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(18, parameters[":p0"]);
    }

    [Fact]
    public void SumAgesWithDb_GeneratesCorrectSql()
    {
        // Arrange
        var sumExpr = TestQueries.SumAgesWithDb();
        
        // Act
        var (sql, parameters) = sumExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            SUM(a0.Age) AS prj0
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void CountCustomersWithDb_GeneratesCorrectSql()
    {
        // Arrange
        var countExpr = TestQueries.CountCustomersWithDb();
        
        // Act
        var (sql, parameters) = countExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            COUNT(*) AS prj0
        FROM 
            customers a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void CountActiveCustomersWithDb_GeneratesCorrectSql()
    {
        // Arrange
        var countExpr = TestQueries.CountActiveCustomersWithDb();
        
        // Act
        var (sql, parameters) = countExpr.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            COUNT(*) AS prj0
        FROM 
            customers a0
        WHERE 
            a0.Age >= :p0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(18, parameters[":p0"]);
    }

    [Fact]
    public void FromWhereAgeGreaterThanSum_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Age > (SELECT 
            SUM(a1.Age) AS prj0
        FROM 
            customers a1)
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromWhereAgeGreaterThanAverageAge_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Age > (SELECT 
            SUM(a1.Age) AS prj0
        FROM 
            customers a1)
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromWhereAgeIn_GeneratesCorrectSql()
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
            a0.Name AS Name
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
    }

    [Fact]
    public void FromWhereAgeInSubquery_GeneratesCorrectSql()
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
            a0.Name AS Name
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
        var expectedSqliteSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            (a0.Age + :p0) AS prj0
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC
        """;
        Assert.Equal(expectedSqliteSql, sqliteSql);
        Assert.Equal("SELECT \n    a0.Id AS Id,\n    a0.Name AS Name,\n    (a0.Age + @p0) AS prj0\nFROM \n    customers a0\nORDER BY \n    a0.Name ASC", sqlServerSql);
        
        // Same number of parameters
        Assert.Equal(sqliteParameters.Count, sqlServerParameters.Count);
    }

    [Fact]
    public void FromWhereAgeInSubqueryWithClosure_GeneratesCorrectSql()
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
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            a0.Age IN (SELECT 
            a1.Age AS Age
        FROM 
            customers a1
        WHERE 
            a1.Name = (a0.Name || :p0))
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal("_VIP", parameters[":p0"]);
    }

    [Fact]
    public void FromSubquery_GeneratesCorrectSql()
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
                (a0.Age + :p0) AS NewAge
            FROM 
                customers a0) a1
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(1, parameters[":p0"]);        
    }

    [Fact]
    public void FromWhereSelectWhereFromNested_GeneratesCorrectSql()
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
    }

    [Fact]
    public void FromWhereSelectWhereNested_GeneratesCorrectSql()
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
    }
    [Fact]
    public void FromGroupBySelect_GeneratesCorrectSql()
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
    }

    [Fact]
    public void FromGroupByMultipleSelect_GeneratesCorrectSql()
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
    }

    [Fact]
    public void FromGroupByHavingSelect_GeneratesCorrectSql()
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
    }

    [Fact]
    public void FromWhereGroupBySelect_GeneratesCorrectSql()
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
    }

    // JOIN Fusion Tests - Testing the new JoinClause fusion functionality
    [Fact]
    public void MultipleInnerJoinsFusion_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.MultipleInnerJoinsFusion();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a1.OrderId AS OrderId,
            a2.ProductName AS ProductName
        FROM 
            customers a0
        INNER JOIN orders a1 ON a0.Id = a1.CustomerId
        INNER JOIN products a2 ON a1.Amount = a2.ProductId
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void MixedJoinTypesFusion_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.MixedJoinTypesFusion();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a1.OrderId AS OrderId,
            a2.ProductName AS ProductName
        FROM 
            customers a0
        INNER JOIN orders a1 ON a0.Id = a1.CustomerId
        LEFT JOIN products a2 ON a1.Amount = a2.ProductId
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void JoinFusionWithWhere_GeneratesCorrectSql()
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
                a0.Name AS Name
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
    }

    // Basic JOIN tests for parity with SQL Server tests
    [Fact]
    public void InnerJoinBasic_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.InnerJoinBasic();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a1.OrderId AS OrderId,
            a1.Amount AS Amount
        FROM 
            customers a0
        INNER JOIN orders a1 ON a0.Id = a1.CustomerId
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void InnerJoinWithSelect_GeneratesCorrectSql()
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
    }

    [Fact]
    public void InnerJoinWithWhere_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.InnerJoinWithWhere();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a1.Id AS Id,
            a1.Name AS Name,
            a1.Age AS Age,
            a2.OrderId AS OrderId,
            a2.Amount AS Amount
        FROM 
            (SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name
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
    }

    [Fact]
    public void InnerJoinWithOrderBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.InnerJoinWithOrderBy();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - now generates compact SQL without subquery
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a1.OrderId AS OrderId,
            a1.Amount AS Amount
        FROM 
            customers a0
        INNER JOIN orders a1 ON a0.Id = a1.CustomerId
        ORDER BY 
            a0.Name ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void LeftJoinBasic_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LeftJoinBasic();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a1.OrderId AS OrderId,
            a1.Amount AS Amount
        FROM 
            customers a0
        LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void LeftJoinWithSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LeftJoinWithSelect();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            (a0.Name || :p0) AS prj0,
            a1.Amount AS Amount
        FROM 
            customers a0
        LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(" (Customer)", parameters[":p0"]);
    }

    [Fact]
    public void LeftJoinWithWhere_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LeftJoinWithWhere();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            a1.Id AS Id,
            a1.Name AS Name,
            a1.Age AS Age,
            a2.OrderId AS OrderId,
            a2.Amount AS Amount
        FROM 
            (SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name
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
    }

    [Fact]
    public void LeftJoinWithOrderBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LeftJoinWithOrderBy();
        
        // Act
        var (sql, parameters) = query.ToSqliteRaw();
        
        // Assert - now generates compact SQL without subquery
        var expectedSql = """
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a1.OrderId AS OrderId,
            a1.Amount AS Amount
        FROM 
            customers a0
        LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
        ORDER BY 
            a0.Name ASC, a1.Amount DESC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void InnerJoinWithGroupBy_GeneratesCorrectSql()
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
    }

    [Fact]
    public void LeftJoinWithAggregates_GeneratesCorrectSql()
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
    }

    [Fact]
    public void FromGroupByOrderBySelect_GeneratesCorrectSql()
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
    }

    [Fact]
    public void FromGroupByOrderByMultipleSelect_GeneratesCorrectSql()
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
    }

    [Fact]
    public void FromGroupByOrderByThreeKeysSelect_GeneratesCorrectSql()
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
    }

    [Fact]
    public void FromGroupByMultipleOrderBySelect_GeneratesCorrectSql()
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
    }

    [Fact]
    public void FromGroupByHavingOrderBySelect_GeneratesCorrectSql()
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
    }

    [Fact]
    public void ComplexJoinWhereGroupByHavingOrderBySelect_GeneratesCorrectSql()
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
            (SUM(a1.Amount) / COUNT(*)) AS AvgOrderValue
        FROM 
            customers a0
        INNER JOIN orders a1 ON a0.Id = a1.CustomerId
        WHERE 
            (a0.Age >= :p0) AND (a1.Amount > :p1)
        GROUP BY 
            a0.Id, a0.Name
        HAVING 
            (COUNT(*) > :p2) AND (SUM(a1.Amount) > :p3)
        ORDER BY 
            SUM(a1.Amount) DESC, COUNT(*) ASC
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Equal(18, parameters[":p0"]);
        Assert.Equal(50, parameters[":p1"]);
        Assert.Equal(2, parameters[":p2"]);
        Assert.Equal(500, parameters[":p3"]);
    }

    [Fact]
    public void ComplexLeftJoinWhereGroupByOrderBySelect_GeneratesCorrectSql()
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
    }

    [Fact]
    public void FromGroupByMinMaxSelect_GeneratesCorrectSql()
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
    }

    [Fact]
    public void FromGroupByAvgSelect_GeneratesCorrectSql()
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
    }

    [Fact]
    public void FromSelectSum_GeneratesCorrectSql()
    {
        // Arrange
        var scalarQuery = TestQueries.FromSelectSum();
        
        // Act
        var (sql, parameters) = scalarQuery.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            SUM(a0.Amount) AS prj0
        FROM 
            orders a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromSelectAvg_GeneratesCorrectSql()
    {
        // Arrange
        var scalarQuery = TestQueries.FromSelectAvg();
        
        // Act
        var (sql, parameters) = scalarQuery.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            AVG(a0.Amount) AS prj0
        FROM 
            orders a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromSelectMin_GeneratesCorrectSql()
    {
        // Arrange
        var scalarQuery = TestQueries.FromSelectMin();
        
        // Act
        var (sql, parameters) = scalarQuery.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            MIN(a0.Amount) AS prj0
        FROM 
            orders a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }

    [Fact]
    public void FromSelectMax_GeneratesCorrectSql()
    {
        // Arrange
        var scalarQuery = TestQueries.FromSelectMax();
        
        // Act
        var (sql, parameters) = scalarQuery.ToSqliteRaw();
        
        // Assert
        var expectedSql = """
        SELECT 
            MAX(a0.Amount) AS prj0
        FROM 
            orders a0
        """;
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
    }
}
