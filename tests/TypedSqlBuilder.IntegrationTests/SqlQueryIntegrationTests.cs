using TypedSqlBuilder.Core;
using TypedSqlBuilder.TestModels;
using Dapper;
using Xunit;

namespace TypedSqlBuilder.IntegrationTests;

/// <summary>
/// Integration tests for SELECT queries executed against SQL databases using Dapper
/// </summary>
public class SqlQueryIntegrationTests : IClassFixture<SqlFixture>, IQueryTestContract
{
    private readonly SqlFixture _fixture;

    public SqlQueryIntegrationTests(SqlFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Helper method to format parameter names correctly based on database type
    /// </summary>
    private static string FormatParam(string paramName, DatabaseType databaseType)
    {
        return databaseType switch
        {
            DatabaseType.SqlServer => $"@{paramName}",
            DatabaseType.PostgreSQL => $":{paramName}", 
            DatabaseType.SQLite => $":{paramName}",
            _ => throw new NotSupportedException($"Database type {databaseType} is not supported.")
        };
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromWhereInt_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereInt(); // WHERE c.Age > 18
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 18) // Same logic as SQL query
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task AbsColumn_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.AbsColumn(); // SELECT Id, ABS(Age)
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(int Id, int Age)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Select(c => (c.Id, Math.Abs(c.Age))) // Same logic as SQL query: (Id, ABS(Age))
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task AbsExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.AbsExpression(); // SELECT Id, ABS(Age - 50)
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(int Id, int AgeExpression)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Select(c => (c.Id, Math.Abs(c.Age - 50))) // Same logic as SQL query: (Id, ABS(Age - 50))
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task AbsInWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.AbsInWhere(); // WHERE ABS(Age) > 30, SELECT Id, Name, Age
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(int Id, string Name, int Age)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => Math.Abs(c.Age) > 30) // Same WHERE logic as SQL query: ABS(Age) > 30
            .Select(c => (c.Id, c.Name, c.Age)) // Same SELECT logic: Id, Name, Age
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task AbsParameter_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.AbsParameter(); // WHERE ABS(Age) > ABS(@minAge)
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Set parameter value using helper method
        const int minAgeValue = 20;
        var updatedParameters = parameters.SetItem(FormatParam("minAge", databaseType), minAgeValue);
        
        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = updatedParameters;
        var actualResults = (await connection.QueryAsync<(int Id, string Name, int Age)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => Math.Abs(c.Age) > Math.Abs(minAgeValue)) // Same WHERE logic as SQL query: ABS(Age) > ABS(20)
            .Select(c => (c.Id, c.Name, c.Age)) // Same SELECT logic: Id, Name, Age
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task AvgExpensivePrices_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.AvgExpensivePrices();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResult = await connection.QuerySingleAsync<decimal>(sql, dapperParams);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResult = TestDataConstants.Products
            .Where(p => p.Price > 100m)
            .Average(p => p.Price!.Value);

        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task AvgPrices_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.AvgPrices();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResult = await connection.QuerySingleAsync<decimal>(sql, dapperParams);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResult = TestDataConstants.Products
            .Where(p => p.Price.HasValue)  // Filter out null prices first
            .Average(p => p.Price!.Value);

        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task BoolColumnDirectComparison_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.BoolColumnDirectComparison();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Set parameter value using helper method
        var updatedParameters = parameters.SetItem(FormatParam("isActive", databaseType), true);
        
        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResult = await connection.QueryAsync<(int Id, string Name, int Age, bool IsActive)>(sql, updatedParameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResult = TestDataConstants.Customers
            .Where(c => c.IsActive)            
            .Select(c => (c.Id, c.Name, c.Age, c.IsActive))
            .ToList();

        Assert.Equal(expectedResult, actualResult.ToList());
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task BoolColumnLiteralFalse_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.BoolColumnLiteralFalse();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResult = await connection.QueryAsync<(int Id, string Name, int Age, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResult = TestDataConstants.Customers
            .Where(c => !c.IsActive) // WHERE IsActive = false
            .Select(c => (c.Id, c.Name, c.Age, c.IsActive))
            .ToList();

        Assert.Equal(expectedResult, actualResult.ToList());
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task BoolColumnLiteralTrue_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.BoolColumnLiteralTrue();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResult = await connection.QueryAsync<(int Id, string Name, int Age, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResult = TestDataConstants.Customers
            .Where(c => c.IsActive) // WHERE IsActive = true
            .Select(c => (c.Id, c.Name, c.Age, c.IsActive))
            .ToList();

        Assert.Equal(expectedResult, actualResult.ToList());
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task CaseBoolExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.CaseBoolExpression(); // SELECT Id, CASE WHEN Age > 18 THEN IsActive ELSE false END
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var result = await connection.QueryAsync<(int Id, bool CaseResult)>(sql, parameters);
        var actualResults = result.ToList();
        
        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Select(c => (c.Id, CaseResult: c.Age > 18 ? c.IsActive : false)) // Same CASE logic: Age > 18 ? IsActive : false
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task CaseDateTimeExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.CaseDateTimeExpression(); // Nested CASE with DateTime comparisons
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var result = await connection.QueryAsync<(string ProductName, string Age)>(sql, parameters);
        var actualResults = result.ToList();
        
        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => (p.ProductName, Age: p.CreatedDate < new DateTime(2020, 1, 1) ? "Old" : 
                                                p.CreatedDate < new DateTime(2024, 1, 1) ? "Recent" : "New"))
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task CaseDecimalExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.CaseDecimalExpression(); // Nested CASE with decimal comparisons
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var result = await connection.QueryAsync<(string ProductName, string ExpensiveFlag)>(sql, parameters);
        var actualResults = result.ToList();
        
        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => (p.ProductName, ExpensiveFlag: p.Price > 1000m ? "Expensive" : 
                                                         p.Price > 100m ? "Moderate" : "Cheap"))
            .ToList();
        
        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task CaseGuidExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.CaseGuidExpression();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var result = await connection.QueryAsync<(string ProductName, string Status)>(sql, parameters);
        
        // Assert
        var expected = TestDataConstants.Products.Select(p => (
            ProductName: p.ProductName,
            Status: p.UniqueId == Guid.Empty ? "Empty" : "HasId"
        ));
        
        Assert.Equal(expected.ToList(), result.ToList());
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task CaseIntExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.CaseIntExpression();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var results = await connection.QueryAsync<(int Id, int CaseResult)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Select(c => (Id: c.Id, CaseResult: c.Age > 65 ? 1 : 0))
            .OrderBy(x => x.Id);

        var actualResults = results.OrderBy(x => x.Id);
        
        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task CaseInWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.CaseInWhere();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var results = await connection.QueryAsync<(int Id, string Name)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => (c.Age > 18 ? "Adult" : "Minor") == "Adult")
            .Select(c => (Id: c.Id, Name: c.Name))
            .OrderBy(x => x.Id);

        var actualResults = results.OrderBy(x => x.Id);
        
        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task CaseStringExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.CaseStringExpression();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var results = await connection.QueryAsync<(int Id, string CaseResult)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Select(c => (Id: c.Id, CaseResult: c.Age > 18 ? "Adult" : "Minor"))
            .OrderBy(x => x.Id);

        var actualResults = results.OrderBy(x => x.Id);
        
        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task ComplexJoinWhereGroupByHavingOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.ComplexJoinWhereGroupByHavingOrderBySelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        using var connection = _fixture.CreateConnection(databaseType);
        var results = await connection.QueryAsync<(int CustomerId, string CustomerName, int TotalOrders, decimal TotalSpent, decimal AvgOrderValue)>(
            sql,
            parameters
        );

        var expected = TestDataConstants.Customers
            .Join(TestDataConstants.Orders, c => c.Id, o => o.CustomerId, (c, o) => new { Customer = c, Order = o })
            .Where(x => x.Customer.Age >= 18 && x.Order.Amount > 50)
            .GroupBy(x => new { x.Customer.Id, x.Customer.Name })
            .Where(g => g.Count() > 2 && g.Sum(x => x.Order.Amount) > 500)
            .OrderByDescending(g => g.Sum(x => x.Order.Amount))
            .ThenBy(g => g.Count())
            .Select(g => new
            {
                CustomerId = g.Key.Id,
                CustomerName = g.Key.Name,
                TotalOrders = g.Count(),
                TotalSpent = g.Sum(x => x.Order.Amount),
                AvgOrderValue = g.Sum(x => x.Order.Amount) / g.Count()
            })
            .ToList();

        Assert.Equal(expected.Count, results.Count());
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task ComplexLeftJoinWhereGroupByOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.ComplexLeftJoinWhereGroupByOrderBySelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        using var connection = _fixture.CreateConnection(databaseType);
        var results = await connection.QueryAsync<(int CustomerId, string CustomerName, int OrderCount, decimal? TotalSpent)>(
            sql,
            parameters
        );

        var expected = TestDataConstants.Customers
            .GroupJoin(TestDataConstants.Orders, c => c.Id, o => o.CustomerId, (c, orders) => new { Customer = c, Orders = orders })
            .Where(x => x.Customer.Age >= 21)
            .GroupBy(x => new { x.Customer.Id, x.Customer.Name })
            .OrderByDescending(g => g.SelectMany(x => x.Orders).Sum(o => o.Amount))
            .ThenBy(g => g.Key.Name)
            .Select(g => new
            {
                CustomerId = g.Key.Id,
                CustomerName = g.Key.Name,
                OrderCount = g.SelectMany(x => x.Orders).Count(),
                TotalSpent = g.SelectMany(x => x.Orders).Any() ? g.SelectMany(x => x.Orders).Sum(o => o.Amount) : (decimal?)null
            })
            .ToList();

        Assert.Equal(expected.Count, results.Count());
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task CountActiveCustomers_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.CountActiveCustomers();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var result = await connection.QuerySingleAsync<int>(sql, parameters);

        // Assert
        var expected = TestDataConstants.Customers.Count(x => x.IsActive);
        Assert.Equal(expected, result);
    }    

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task CountCustomers_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.CountCustomers();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var result = await connection.QuerySingleAsync<int>(sql, parameters);

        // Assert
        var expected = TestDataConstants.Customers.Count();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task DateTimeAddDays_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DateTimeAddDays();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<DateTime?>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => p.CreatedDate?.AddDays(30)) // Same logic as SQL query: AddDays(30)
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DateTimeAddMonths_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DateTimeAddMonths();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<DateTime?>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => p.CreatedDate?.AddMonths(6)) // Same logic as SQL query: AddMonths(6)
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DateTimeAddYears_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DateTimeAddYears();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<DateTime?>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => p.CreatedDate?.AddYears(1)) // Same logic as SQL query: AddYears(1)
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DateTimeDay_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DateTimeDay();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<int?>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => p.CreatedDate?.Day) // Same logic as SQL query: Day property
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DateTimeDiffDays_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DateTimeDiffDays();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<int?>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var now = DateTime.Now;
        var expectedResults = TestDataConstants.Products
            .Select(p => p.CreatedDate.HasValue ? (int?)(now - p.CreatedDate.Value).TotalDays : null)
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DateTimeDiffMonths_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DateTimeDiffMonths();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<int?>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var now = DateTime.Now;
        var expectedResults = TestDataConstants.Products
            .Select(p => p.CreatedDate.HasValue ? (int?)((now.Year - p.CreatedDate.Value.Year) * 12 + (now.Month - p.CreatedDate.Value.Month)) : null)
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DateTimeDiffYears_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DateTimeDiffYears();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<int?>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var now = DateTime.Now;
        var expectedResults = TestDataConstants.Products
            .Select(p => p.CreatedDate.HasValue ? (int?)(now.Year - p.CreatedDate.Value.Year) : null)
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DateTimeFunctionsInSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DateTimeFunctionsInSelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int? CreatedYear, int? CreatedMonth, int? CreatedDay, DateTime? NextWeek, DateTime? NextMonth, int? DaysAgo)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var now = DateTime.Now;
        var expectedResults = TestDataConstants.Products
            .Select(p => (
                Id: p.Id,
                CreatedYear: p.CreatedDate?.Year,
                CreatedMonth: p.CreatedDate?.Month,
                CreatedDay: p.CreatedDate?.Day,
                NextWeek: p.CreatedDate?.AddDays(7),
                NextMonth: p.CreatedDate?.AddMonths(1),
                DaysAgo: p.CreatedDate.HasValue ? (int?)(now - p.CreatedDate.Value).TotalDays : null
            ))
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DateTimeFunctionsInWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DateTimeFunctionsInWhere();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<int>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var currentYear = DateTime.Now.Year;
        var expectedResults = TestDataConstants.Products
            .Where(p => p.CreatedDate.HasValue && p.CreatedDate.Value.Year == currentYear && p.CreatedDate.Value.Month >= 6)
            .Select(p => p.Id)
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DateTimeMonth_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DateTimeMonth();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<int?>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => p.CreatedDate?.Month) // Same logic as SQL query: Month property
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DateTimeNow_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DateTimeNow();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<DateTime?>(sql, parameters);

        // Assert
        var result = actualResults.FirstOrDefault();
        Assert.True(result.HasValue);
        
        // Check that the result is close to now (within 10 seconds)
        var timeDiff = Math.Abs((DateTime.UtcNow - result.Value).TotalSeconds);
        Assert.True(timeDiff < 10, $"DateTime.Now result {result} is not close to current time. Difference: {timeDiff} seconds");
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DateTimeYear_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DateTimeYear();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<int?>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => p.CreatedDate?.Year) // Same logic as SQL query: Year property
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DecimalCeiling_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DecimalCeiling();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<decimal?>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => p.Price.HasValue ? Math.Ceiling(p.Price.Value) : (decimal?)null)
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DecimalFloor_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DecimalFloor();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<decimal?>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => p.Price.HasValue ? Math.Floor(p.Price.Value) : (decimal?)null)
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task DecimalRound_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DecimalRound();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<decimal?>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => p.Price.HasValue ? Math.Round(p.Price.Value, 2) : (decimal?)null)
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromGroupByAvgSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromGroupByAvgSelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int CustomerId, decimal AvgAmount, int OrderCount)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Orders
            .GroupBy(o => o.CustomerId)
            .Select(g => (CustomerId: g.Key, AvgAmount: g.Average(o => (decimal)o.Amount), OrderCount: g.Count()))
            .OrderBy(x => x.CustomerId)
            .ToList();

        var actualResultsSet = actualResults.OrderBy(x => x.CustomerId).ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromGroupByDecimalAggregatesSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromGroupByDecimalAggregatesSelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        
        IEnumerable<(string ProductName, decimal? SumPrice, decimal? AvgPrice, decimal? MinPrice, decimal? MaxPrice)> actualResults;
        if (databaseType == DatabaseType.SQLite)
        {
            // SQLite returns double for aggregates, so we need to convert
            var rawResults = await connection.QueryAsync<(string ProductName, double? SumPrice, double? AvgPrice, double? MinPrice, double? MaxPrice)>(sql, parameters);
            actualResults = rawResults.Select(r => (
                r.ProductName, 
                SumPrice: r.SumPrice.HasValue ? (decimal?)r.SumPrice.Value : null,
                AvgPrice: r.AvgPrice.HasValue ? (decimal?)r.AvgPrice.Value : null,
                MinPrice: r.MinPrice.HasValue ? (decimal?)r.MinPrice.Value : null,
                MaxPrice: r.MaxPrice.HasValue ? (decimal?)r.MaxPrice.Value : null
            ));
        }
        else
        {
            actualResults = await connection.QueryAsync<(string ProductName, decimal? SumPrice, decimal? AvgPrice, decimal? MinPrice, decimal? MaxPrice)>(sql, parameters);
        }

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .GroupBy(p => p.ProductName)
            .Select(g => (
                ProductName: g.Key,
                SumPrice: g.Any(p => p.Price.HasValue) ? g.Where(p => p.Price.HasValue).Sum(p => p.Price!.Value) : (decimal?)null,
                AvgPrice: g.Any(p => p.Price.HasValue) ? g.Where(p => p.Price.HasValue).Average(p => p.Price!.Value) : (decimal?)null,
                MinPrice: g.Any(p => p.Price.HasValue) ? g.Where(p => p.Price.HasValue).Min(p => p.Price!.Value) : (decimal?)null,
                MaxPrice: g.Any(p => p.Price.HasValue) ? g.Where(p => p.Price.HasValue).Max(p => p.Price!.Value) : (decimal?)null
            ))
            .OrderBy(x => x.ProductName)
            .ToList();

        var actualResultsSet = actualResults.OrderBy(x => x.ProductName).ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromGroupByDecimalAvgSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromGroupByDecimalAvgSelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        
        IEnumerable<(string ProductName, decimal? AvgPrice)> actualResults;
        if (databaseType == DatabaseType.SQLite)
        {
            // SQLite returns double for AVG, so we need to convert
            var rawResults = await connection.QueryAsync<(string ProductName, double? AvgPrice)>(sql, parameters);
            actualResults = rawResults.Select(r => (r.ProductName, AvgPrice: r.AvgPrice.HasValue ? (decimal?)r.AvgPrice.Value : null));
        }
        else
        {
            actualResults = await connection.QueryAsync<(string ProductName, decimal? AvgPrice)>(sql, parameters);
        }

        // Assert - Compare SQL results to LINQ results using TestDataConstants  
        var expectedResults = TestDataConstants.Products
            .GroupBy(p => p.ProductName)
            .Select(g => (
                ProductName: g.Key, 
                AvgPrice: g.Any(p => p.Price.HasValue) ? g.Where(p => p.Price.HasValue).Average(p => p.Price!.Value) : (decimal?)null
            ))
            .OrderBy(x => x.ProductName)
            .ToList();

        var actualResultsSet = actualResults.OrderBy(x => x.ProductName).ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromGroupByDecimalSumSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromGroupByDecimalSumSelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        
        IEnumerable<(string ProductName, decimal? SumPrice)> actualResults;
        if (databaseType == DatabaseType.SQLite)
        {
            // SQLite returns double for SUM, so we need to convert
            var rawResults = await connection.QueryAsync<(string ProductName, double? SumPrice)>(sql, parameters);
            actualResults = rawResults.Select(r => (r.ProductName, SumPrice: r.SumPrice.HasValue ? (decimal?)r.SumPrice.Value : null));
        }
        else
        {
            actualResults = await connection.QueryAsync<(string ProductName, decimal? SumPrice)>(sql, parameters);
        }

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .GroupBy(p => p.ProductName)
            .Select(g => (
                ProductName: g.Key, 
                SumPrice: g.Any(p => p.Price.HasValue) ? g.Where(p => p.Price.HasValue).Sum(p => p.Price!.Value) : (decimal?)null
            ))
            .OrderBy(x => x.ProductName)
            .ToList();

        var actualResultsSet = actualResults.OrderBy(x => x.ProductName).ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromGroupByHavingOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromGroupByHavingOrderBySelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int CustomerId, int TotalAmount)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Orders
            .GroupBy(o => o.CustomerId)
            .Where(g => g.Count() > 1)
            .OrderByDescending(g => g.Sum(o => o.Amount))
            .Select(g => (CustomerId: g.Key, TotalAmount: g.Sum(o => o.Amount)))
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromGroupByHavingSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromGroupByHavingSelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Age, int Count)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .GroupBy(c => c.Age)
            .Where(g => g.Count() > 1)
            .Select(g => (Age: g.Key, Count: g.Count()))
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromGroupByMinMaxSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromGroupByMinMaxSelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int CustomerId, int MinAmount, int MaxAmount, int OrderCount)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Orders
            .GroupBy(o => o.CustomerId)
            .Select(g => (CustomerId: g.Key, MinAmount: g.Min(o => o.Amount), MaxAmount: g.Max(o => o.Amount), OrderCount: g.Count()))
            .OrderBy(x => x.CustomerId)
            .ToList();

        var actualResultsSet = actualResults.OrderBy(x => x.CustomerId).ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromGroupByMultipleOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromGroupByMultipleOrderBySelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Age, string Name, int Count)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .GroupBy(c => new { c.Age, c.Name })
            .Select(g => (
                Age: g.Key.Age,
                Name: g.Key.Name,
                Count: g.Count()
            ))
            .OrderBy(r => r.Count)
            .ThenBy(r => r.Age)
            .ThenBy(r => r.Name)
            .ToList();

        var actualResultsSet = actualResults.OrderBy(r => r.Count).ThenBy(r => r.Age).ThenBy(r => r.Name).ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromGroupByMultipleSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromGroupByMultipleSelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Age, string Name, int Count)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .GroupBy(c => new { c.Age, c.Name })
            .Select(g => (
                Age: g.Key.Age,
                Name: g.Key.Name,
                Count: g.Count()
            ))
            .OrderBy(x => x.Age)
            .ThenBy(x => x.Name)
            .ToList();

        var actualResultsSet = actualResults.OrderBy(x => x.Age).ThenBy(x => x.Name).ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromGroupByOrderByMultipleSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromGroupByOrderByMultipleSelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int CustomerId, int TotalAmount, int OrderCount)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Orders
            .GroupBy(o => o.CustomerId)
            .Select(g => (
                CustomerId: g.Key,
                TotalAmount: g.Sum(o => o.Amount),
                OrderCount: g.Count()
            ))
            .OrderByDescending(r => r.TotalAmount)
            .ThenBy(r => r.OrderCount)
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromGroupByOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromGroupByOrderBySelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int CustomerId, int TotalAmount)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Orders
            .GroupBy(o => o.CustomerId)
            .Select(g => (
                CustomerId: g.Key,
                TotalAmount: g.Sum(o => o.Amount)
            ))
            .OrderByDescending(r => r.TotalAmount)
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromGroupByOrderByThreeKeysSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromGroupByOrderByThreeKeysSelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int CustomerId, int TotalAmount, int OrderCount)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Orders
            .GroupBy(o => o.CustomerId)
            .Select(g => (
                CustomerId: g.Key,
                TotalAmount: g.Sum(o => o.Amount),
                OrderCount: g.Count()
            ))
            .OrderByDescending(r => r.TotalAmount)
            .ThenBy(r => r.OrderCount)
            .ThenBy(r => r.CustomerId)
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromGroupBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromGroupBySelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Age, int Count)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        // Note: GROUP BY without ORDER BY can return results in different orders across databases
        // so we sort both expected and actual results for consistent comparison
        var expectedResults = TestDataConstants.Customers
            .GroupBy(c => c.Age)
            .Select(g => (
                Age: g.Key,
                Count: g.Count()
            ))
            .OrderBy(x => x.Age) // Sort for consistent comparison
            .ToList();

        var actualResultsSet = actualResults.OrderBy(x => x.Age).ToList(); // Sort for consistent comparison
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromOrderByAsc_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromOrderByAsc();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .OrderBy(c => c.Name)
            .Select(c => (Id: c.Id, Age: c.Age, Name: c.Name, IsActive: c.IsActive))
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromOrderByDescendingThenBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromOrderByDescendingThenBy();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .OrderByDescending(c => c.Age)
            .ThenBy(c => c.Name)
            .Select(c => (Id: c.Id, Age: c.Age, Name: c.Name, IsActive: c.IsActive))
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromOrderByDesc_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromOrderByDesc();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .OrderByDescending(c => c.Age)
            .Select(c => (Id: c.Id, Age: c.Age, Name: c.Name, IsActive: c.IsActive))
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromOrderByMultiple_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromOrderByMultiple();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .OrderBy(c => c.Name)
            .ThenByDescending(c => c.Age)
            .ThenBy(c => c.Id)
            .Select(c => (Id: c.Id, Age: c.Age, Name: c.Name, IsActive: c.IsActive))
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromOrderByThenByDescending_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromOrderByThenByDescending();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .OrderBy(c => c.Name)
            .ThenByDescending(c => c.Age)
            .Select(c => (Id: c.Id, Age: c.Age, Name: c.Name, IsActive: c.IsActive))
            .ToList();

        var actualResultsSet = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsSet);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromOrderByThenBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromOrderByThenBySelect(); // ORDER BY Name ASC, Age ASC, SELECT Id, Name
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(int Id, string Name)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers.OrderBy(c => c.Name).ThenBy(c => c.Age).Select(c => (c.Id, c.Name)).ToList();
        
        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromOrderByThenBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromOrderByThenBy(); // ORDER BY Name ASC, Age ASC
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers.OrderBy(c => c.Name).ThenBy(c => c.Age).Select(c => (Id: c.Id, Age: c.Age, Name: c.Name, IsActive: c.IsActive)).ToList();
        
        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromProductWhereSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromProductWhereSelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, string ProductName)>(sql, parameters);
        var actualResultsList = actualResults.ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Where(p => p.ProductName != "Discontinued")
            .Select(p => (Id: p.Id, ProductName: p.ProductName))
            .ToList();
        
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromSelectAvg_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromSelectAvg();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResult = await connection.QuerySingleAsync<int?>(sql, parameters);

        // Assert - Compare SQL result to LINQ result using TestDataConstants
        var expectedResult = (int)TestDataConstants.Orders.Average(o => (decimal)o.Amount);

        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromSelectCreatedDateMinMax_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromSelectCreatedDateMinMax();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(string ProductName, DateTime? EarliestDate, DateTime? LatestDate)>(sql, parameters);
        var actualResultsList = actualResults.ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => (ProductName: p.ProductName, EarliestDate: p.CreatedDate, LatestDate: p.CreatedDate))
            .ToList();
        
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromSelectDecimalArithmetic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromSelectDecimalArithmetic();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(string ProductName, decimal? Price110, decimal? PricePlus10, decimal? PriceMinus5)>(sql, parameters);
        var actualResultsList = actualResults.ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => (ProductName: p.ProductName, 
                         Price110: p.Price * 1.1m, 
                         PricePlus10: p.Price + 10.0m, 
                         PriceMinus5: p.Price - 5.0m))
            .ToList();
        
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromSelectExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromSelectExpression();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int, string)>(sql, parameters);
        var actualResultsList = actualResults.ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Select(c => (c.Id * 100 + c.Age, c.Name + " - Customer"))
            .ToList();
        
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromSelectMax_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromSelectMax();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResult = await connection.QuerySingleAsync<int>(sql, parameters);

        // Assert - Compare SQL result to LINQ result using TestDataConstants
        var expectedResult = TestDataConstants.Orders.Max(o => o.Amount);
        
        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromSelectMin_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromSelectMin();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResult = await connection.QuerySingleAsync<int>(sql, parameters);

        // Assert - Compare SQL result to LINQ result using TestDataConstants
        var expectedResult = TestDataConstants.Orders.Min(o => o.Amount);
        
        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromSelectOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromSelectOrderBy();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, string Name, int AgeAddition)>(sql, parameters);
        var actualResultsList = actualResults.ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .OrderBy(c => c.Name)
            .Select(c => (Id: c.Id, Name: c.Name, AgeAddition: c.Age + 5))
            .ToList();
        
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromSelectSingle_GeneratesCorrectSql(DatabaseType databaseType)
    {
        using var connection = _fixture.CreateConnection(databaseType);
        var (sql, parameters) = TestQueries.FromSelectSingle().ToSqlRaw(databaseType);

        var results = await connection.QueryAsync<int>(sql, parameters);
        var resultsList = results.ToList();

        var expectedResults = TestDataConstants.Customers.Select(c => c.Age).ToList();
        
        Assert.Equal(expectedResults, resultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromSelectSum_GeneratesCorrectSql(DatabaseType databaseType)
    {
        using var connection = _fixture.CreateConnection(databaseType);
        var (sql, parameters) = TestQueries.FromSelectSum().ToSqlRaw(databaseType);

        var result = await connection.QuerySingleAsync<decimal>(sql, parameters);

        var expectedResult = TestDataConstants.Orders.Sum(o => o.Amount);
        
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        using var connection = _fixture.CreateConnection(databaseType);
        var (sql, parameters) = TestQueries.FromSelect().ToSqlRaw(databaseType);

        var results = await connection.QueryAsync<(int Id, string Name)>(sql, parameters);
        var resultsList = results.ToList();

        var expectedResults = TestDataConstants.Customers.Select(c => (Id: c.Id, Name: c.Name)).ToList();
        
        Assert.Equal(expectedResults, resultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromStatic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        using var connection = _fixture.CreateConnection(databaseType);
        var (sql, parameters) = TestQueries.FromStatic().ToSqlRaw(databaseType);

        // Act
        var results = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);
        var actualResultsList = results.ToList();

        // Assert
        var expectedResults = TestDataConstants.Customers.ToList();
        Assert.Equal(expectedResults.Count, actualResultsList.Count);
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromSubquery_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        using var connection = _fixture.CreateConnection(databaseType);
        var (sql, parameters) = TestQueries.FromSubquery().ToSqlRaw(databaseType);

        // Act
        var results = await connection.QueryAsync<(int Id, int NewAge)>(sql, parameters);
        var actualResultsList = results.ToList();

        // Assert
        // The subquery adds 1 to each customer's age, so we expect customers with incremented ages
        var expectedResults = TestDataConstants.Customers.Select(c => (Id: c.Id, NewAge: c.Age + 1)).ToList();
        Assert.Equal(expectedResults.Count, actualResultsList.Count);
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromWhereAgeGreaterThanAverageAge_GeneratesCorrectSql(DatabaseType databaseType)
    {
        using var connection = _fixture.CreateConnection(databaseType);
        var (sql, parameters) = TestQueries.FromWhereAgeGreaterThanAverageAge().ToSqlRaw(databaseType);
        
        var results = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);
        var actualResultsList = results.ToList();
        
        // This compares ages to sum of all ages - unlikely to have any matches in normal test data
        var totalAgeSum = TestDataConstants.Customers.Sum(c => c.Age);
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > totalAgeSum)
            .Select(c => (Id: c.Id, Age: c.Age, Name: c.Name, IsActive: c.IsActive))
            .ToList();
        
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromWhereAgeGreaterThanSum_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        using var connection = _fixture.CreateConnection(databaseType);
        var query = TestQueries.FromWhereAgeGreaterThanSum();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Act
        var actualResultsList = (await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(
            sql, parameters)).ToList();
            
        // Expected: customers with age greater than sum of all ages (sum is way higher than individual ages, so should be empty)
        var totalAgeSum = TestDataConstants.Customers.Sum(c => c.Age);
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > totalAgeSum)
            .Select(c => (Id: c.Id, Age: c.Age, Name: c.Name, IsActive: c.IsActive))
            .ToList();

        // Assert
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromWhereAgeInSubqueryWithClosure_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        using var connection = _fixture.CreateConnection(databaseType);
        var query = TestQueries.FromWhereAgeInSubqueryWithClosure();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Act
        var actualResultsList = (await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(
            sql, parameters)).ToList();
            
        // Expected: customers with age IN (ages of customers whose name equals this customer's name + "_VIP")
        // This is a complex closure query - likely returns empty results as no customer names have "_VIP" variants
        var expectedResults = TestDataConstants.Customers
            .Where(c => TestDataConstants.Customers
                .Where(x => x.Name == c.Name + "_VIP")
                .Select(x => x.Age)
                .Contains(c.Age))
            .Select(c => (Id: c.Id, Age: c.Age, Name: c.Name, IsActive: c.IsActive))
            .ToList();

        // Assert
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromWhereAgeInSubquery_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        using var connection = _fixture.CreateConnection(databaseType);
        var query = TestQueries.FromWhereAgeInSubquery();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Act
        var actualResultsList = (await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(
            sql, parameters)).ToList();
            
        // Expected: customers with age IN (ages of customers with name = "VIP")
        var expectedResults = TestDataConstants.Customers
            .Where(c => TestDataConstants.Customers
                .Where(x => x.Name == "VIP")
                .Select(x => x.Age)
                .Contains(c.Age))
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .OrderBy(x => x.Id)
            .ToList();
            
        actualResultsList = actualResultsList.OrderBy(x => x.Id).ToList();
        
        // Assert
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromWhereAgeIn_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        using var connection = _fixture.CreateConnection(databaseType);
        var query = TestQueries.FromWhereAgeIn();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Act
        var actualResultsList = (await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(
            sql, parameters)).ToList();
            
        // Expected: customers with age IN (18, 21, 25, 30)
        var expectedResults = TestDataConstants.Customers
            .Where(c => new[] { 18, 21, 25, 30 }.Contains(c.Age))
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .OrderBy(x => x.Id)
            .ToList();
            
        actualResultsList = actualResultsList.OrderBy(x => x.Id).ToList();
        
        // Assert
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromWhereAndSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        using var connection = _fixture.CreateConnection(databaseType);
        var query = TestQueries.FromWhereAndSelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Act
        var actualResultsList = (await connection.QueryAsync<(int Id, string Name)>(
            sql, parameters)).ToList();
            
        // Expected: customers with Age >= 21 AND Name != "", selecting Id and Name
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age >= 21 && c.Name != "")
            .Select(c => (c.Id, c.Name))
            .OrderBy(x => x.Id)
            .ToList();
            
        actualResultsList = actualResultsList.OrderBy(x => x.Id).ToList();
        
        // Assert
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromWhereAnd_GeneratesCorrectSql(DatabaseType databaseType)
    {
        using var connection = _fixture.CreateConnection(databaseType);
        var query = TestQueries.FromWhereAnd();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        var actualResultsList = (await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters)).ToList();
        
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 18 && c.Name == "John")
            .ToList();
            
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromWhereCreatedDateComparison_GeneratesCorrectSql(DatabaseType databaseType)
    {
        using var connection = _fixture.CreateConnection(databaseType);
        var query = TestQueries.FromWhereCreatedDateComparison();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        var actualResultsList = (await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, Guid? UniqueId)>(sql, parameters)).ToList();
        
        var expectedResults = TestDataConstants.Products
            .Where(p => p.CreatedDate > new DateTime(2024, 1, 1))
            .ToList();
            
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromWhereCreatedDateIsNotNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        using var connection = _fixture.CreateConnection(databaseType);
        var query = TestQueries.FromWhereCreatedDateIsNotNull();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        if (databaseType == DatabaseType.SQLite)
        {
            // SQLite stores GUIDs as TEXT, so we need to use string type
            var actualResultsList = (await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, string? UniqueId)>(sql, parameters)).ToList();
            
            var expectedResults = TestDataConstants.Products
                .Where(p => p.CreatedDate != null)
                .Select(p => (p.Id, p.ProductName, p.Price, p.CreatedDate, p.UniqueId?.ToString()))
                .ToList();
                
            Assert.Equal(expectedResults, actualResultsList);
        }
        else
        {
            var actualResultsList = (await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, Guid? UniqueId)>(sql, parameters)).ToList();
            
            var expectedResults = TestDataConstants.Products
                .Where(p => p.CreatedDate != null)
                .ToList();
                
            Assert.Equal(expectedResults, actualResultsList);
        }
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromWhereCreatedDateIsNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        using var connection = _fixture.CreateConnection(databaseType);
        
        var query = TestQueries.FromWhereCreatedDateIsNull();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Verify we get products where CreatedDate is null
        var expected = TestDataConstants.Products.Where(p => p.CreatedDate == null).ToArray();
        
        if (databaseType == DatabaseType.SQLite)
        {
            var result = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, string? CreatedDate, string? UniqueId)>(sql, parameters);
            var actualResultsList = result.Select(r => (r.Id, r.ProductName, r.Price, 
                string.IsNullOrEmpty(r.CreatedDate) ? (DateTime?)null : DateTime.Parse(r.CreatedDate), 
                string.IsNullOrEmpty(r.UniqueId) ? (Guid?)null : Guid.Parse(r.UniqueId))).ToList();
            
            var expectedResults = expected.ToList();
            Assert.Equal(expectedResults, actualResultsList);
        }
        else
        {
            var result = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, Guid? UniqueId)>(sql, parameters);
            var actualResultsList = result.ToList();
            
            var expectedResults = expected.ToList();
            Assert.Equal(expectedResults, actualResultsList);
        }
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromWhereDecimalComparison_GeneratesCorrectSql(DatabaseType databaseType)
    {
        using var connection = _fixture.CreateConnection(databaseType);
        
        var query = TestQueries.FromWhereDecimalComparison();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Verify we get products where Price > 100.50m
        var expected = TestDataConstants.Products.Where(p => p.Price > 100.50m).ToArray();
        
        if (databaseType == DatabaseType.SQLite)
        {
            var result = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, string? CreatedDate, string? UniqueId)>(sql, parameters);
            var actualResultsList = result.Select(r => (r.Id, r.ProductName, r.Price, 
                string.IsNullOrEmpty(r.CreatedDate) ? (DateTime?)null : DateTime.Parse(r.CreatedDate), 
                string.IsNullOrEmpty(r.UniqueId) ? (Guid?)null : Guid.Parse(r.UniqueId))).ToList();
            
            var expectedResults = expected.ToList();
            Assert.Equal(expectedResults, actualResultsList);
        }
        else
        {
            var result = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, Guid? UniqueId)>(sql, parameters);
            var actualResultsList = result.ToList();
            
            var expectedResults = expected.ToList();
            Assert.Equal(expectedResults, actualResultsList);
        }
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromWhereDecimalIsNotNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        using var connection = _fixture.CreateConnection(databaseType);
        
        var query = TestQueries.FromWhereDecimalIsNotNull();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Verify we get products where Price is not null
        var expected = TestDataConstants.Products.Where(p => p.Price != null).ToArray();
        
        if (databaseType == DatabaseType.SQLite)
        {
            var result = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, string? CreatedDate, string? UniqueId)>(sql, parameters);
            var actualResultsList = result.Select(r => (r.Id, r.ProductName, r.Price, 
                string.IsNullOrEmpty(r.CreatedDate) ? (DateTime?)null : DateTime.Parse(r.CreatedDate), 
                string.IsNullOrEmpty(r.UniqueId) ? (Guid?)null : Guid.Parse(r.UniqueId))).ToList();
            
            var expectedResults = expected.ToList();
            Assert.Equal(expectedResults, actualResultsList);
        }
        else
        {
            var result = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, Guid? UniqueId)>(sql, parameters);
            var actualResultsList = result.ToList();
            
            var expectedResults = expected.ToList();
            Assert.Equal(expectedResults, actualResultsList);
        }
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromWhereDecimalIsNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        using var connection = _fixture.CreateConnection(databaseType);
        
        var query = TestQueries.FromWhereDecimalIsNull();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Verify we get products where Price is null
        var expected = TestDataConstants.Products.Where(p => p.Price == null).ToArray();
        
        if (databaseType == DatabaseType.SQLite)
        {
            var result = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, string? CreatedDate, string? UniqueId)>(sql, parameters);
            var actualResultsList = result.Select(r => (r.Id, r.ProductName, r.Price, 
                string.IsNullOrEmpty(r.CreatedDate) ? (DateTime?)null : DateTime.Parse(r.CreatedDate), 
                string.IsNullOrEmpty(r.UniqueId) ? (Guid?)null : Guid.Parse(r.UniqueId))).ToList();
            
            var expectedResults = expected.ToList();
            Assert.Equal(expectedResults, actualResultsList);
        }
        else
        {
            var result = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, Guid? UniqueId)>(sql, parameters);
            var actualResultsList = result.ToList();
            
            var expectedResults = expected.ToList();
            Assert.Equal(expectedResults, actualResultsList);
        }
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereFusionThree_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereFusionThree();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 18)
            .Where(c => c.Name != "Admin")
            .Where(c => c.Age < 65)
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereFusionTwo_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereFusionTwo();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 18)
            .Where(c => c.Name != "Admin")
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereFusionWithOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereFusionWithOrderBy();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 18)
            .Where(c => c.Name != "Admin")
            .OrderBy(c => c.Name)
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereFusionWithSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereFusionWithSelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, string Name)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age >= 21)
            .Where(c => c.Name != "")
            .Select(c => (c.Id, c.Name))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereGroupBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereGroupBySelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Age, int Count)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age >= 18)
            .GroupBy(c => c.Age)
            .Select(g => (Age: g.Key, Count: g.Count()))
            .OrderBy(x => x.Age)
            .ToList();

        var actualResultsList = actualResults.OrderBy(x => x.Age).ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }    

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereIsNotNullInt_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereIsNotNullInt();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            // Since Age is non-nullable in test data, "Age != NULL" would return all rows
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereIsNotNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereIsNotNull();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            // Since Name is non-nullable in test data, "Name != NULL" would return all rows
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereIsNullCombined_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereIsNullCombined();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        // This query looks for rows where Name IS NULL AND Age IS NOT NULL
        // Since all test data has non-null values, this returns empty result
        var expectedResults = TestDataConstants.Customers
            .Where(c => false) // No rows match this condition in our test data
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereIsNullInt_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereIsNullInt();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        // This query looks for rows where Age IS NULL
        // Since all test data has non-null Age values, this returns empty result
        var expectedResults = TestDataConstants.Customers
            .Where(c => false) // No rows match this condition in our test data
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereIsNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereIsNull();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        // This query looks for rows where Name IS NULL
        // Since all test data has non-null Name values, this returns empty result
        var expectedResults = TestDataConstants.Customers
            .Where(c => false) // No rows match this condition in our test data
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereMultiple_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereMultiple();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 18 && c.Name != "Admin")
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereOrderBySelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, string Name, int Age)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 21 && c.Name != "")
            .OrderBy(c => c.Age)
            .Select(c => (c.Id, c.Name, Age: c.Age + 10))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereOrderByThenBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereOrderByThenBy();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 18)
            .OrderBy(c => c.Name)
            .ThenByDescending(c => c.Age)
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereOrderBy();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 21 && c.Name != "")
            .OrderBy(c => c.Age)
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromWhereOr_GeneratesCorrectSql(DatabaseType databaseType)
    {
        using var connection = _fixture.CreateConnection(databaseType);
        
        var query = TestQueries.FromWhereOr(); // WHERE (c.Age > 18 AND c.Age < 65) OR c.Name == "VIP"
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);
        var actualResultsList = actualResults.ToList();
        
        var expectedResults = TestDataConstants.Customers
            .Where(c => (c.Age > 18 && c.Age < 65) || c.Name == "VIP")
            .ToList();
        
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromWhereSelectNamed_GeneratesCorrectSql(DatabaseType databaseType)
    {
        using var connection = _fixture.CreateConnection(databaseType);
        
        var query = TestQueries.FromWhereSelectNamed(); // WHERE c.Age > 18, SELECT with named fields
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        var actualResults = await connection.QueryAsync<(int OriginalId, int ModifiedId, string CustomerName)>(sql, parameters);
        var actualResultsList = actualResults.ToList();
        
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 18)
            .Select(c => (OriginalId: c.Id, ModifiedId: c.Id * 100 + c.Age, CustomerName: c.Name))
            .ToList();
        
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereSelectOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereSelectOrderBy();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, string Name)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 18)
            .OrderBy(c => c.Name)
            .Select(c => (c.Id + 1, c.Name + "!"))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereSelectParameterized_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        const int minAge = 20;
        const int maxAge = 30;
        var query = TestQueries.FromWhereSelectParameterized(minAge, maxAge);
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, string Name)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age >= minAge && c.Age <= maxAge)
            .Select(c => (c.Id, c.Name))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereSelectWhereFromNested_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereSelectWhereFromNested();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, string Name)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 18)
            .Select(c => (c.Id, c.Name))
            .Where(x => x.Id > 100)
            .Select(x => (x.Id, x.Name))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereSelectWhereNested_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereSelectWhereNested();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, string Name)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 18)
            .Select(c => (c.Id, c.Name))
            .Where(x => x.Id > 100)
            .Select(x => (x.Id, x.Name))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereSelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, string Name)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age >= 21)
            .Select(c => (c.Id, c.Name))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereString_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereString();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Name == "John")
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereUniqueIdEquals_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereUniqueIdEquals();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, Guid? UniqueId)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Where(p => p.UniqueId == Guid.Parse("12345678-1234-1234-1234-123456789012"))
            .Select(p => (p.Id, p.ProductName, p.Price, p.CreatedDate, p.UniqueId))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereUniqueIdIsNotNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereUniqueIdIsNotNull();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        
        // SQLite stores GUIDs as strings, so we need different handling
        if (databaseType == DatabaseType.SQLite)
        {
            var actualResults = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, string? UniqueId)>(sql, parameters);
            
            // Assert - Compare SQL results to LINQ results using TestDataConstants
            var expectedResults = TestDataConstants.Products
                .Where(p => p.UniqueId != null)
                .Select(p => (p.Id, p.ProductName, p.Price, p.CreatedDate, p.UniqueId.ToString()))
                .ToList();

            var actualResultsList = actualResults.Select(r => (r.Id, r.ProductName, r.Price, r.CreatedDate, r.UniqueId)).ToList();
            Assert.Equal(expectedResults, actualResultsList);
        }
        else
        {
            var actualResults = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, Guid? UniqueId)>(sql, parameters);
            
            // Assert - Compare SQL results to LINQ results using TestDataConstants
            var expectedResults = TestDataConstants.Products
                .Where(p => p.UniqueId != null)
                .Select(p => (p.Id, p.ProductName, p.Price, p.CreatedDate, p.UniqueId))
                .ToList();

            var actualResultsList = actualResults.ToList();
            Assert.Equal(expectedResults, actualResultsList);
        }
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereUniqueIdIsNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereUniqueIdIsNull();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        
        // SQLite stores GUIDs as strings, so we need different handling
        if (databaseType == DatabaseType.SQLite)
        {
            var actualResults = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, string? UniqueId)>(sql, parameters);
            
            // Assert - Compare SQL results to LINQ results using TestDataConstants
            var expectedResults = TestDataConstants.Products
                .Where(p => p.UniqueId == null)
                .Select(p => (p.Id, p.ProductName, p.Price, p.CreatedDate, (string?)null))
                .ToList();

            var actualResultsList = actualResults.Select(r => (r.Id, r.ProductName, r.Price, r.CreatedDate, r.UniqueId)).ToList();
            Assert.Equal(expectedResults, actualResultsList);
        }
        else
        {
            var actualResults = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, Guid? UniqueId)>(sql, parameters);
            
            // Assert - Compare SQL results to LINQ results using TestDataConstants
            var expectedResults = TestDataConstants.Products
                .Where(p => p.UniqueId == null)
                .Select(p => (p.Id, p.ProductName, p.Price, p.CreatedDate, p.UniqueId))
                .ToList();

            var actualResultsList = actualResults.ToList();
            Assert.Equal(expectedResults, actualResultsList);
        }
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereUniqueIdNotEquals_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereUniqueIdNotEquals();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        
        // SQLite stores GUIDs as strings, so we need different handling
        if (databaseType == DatabaseType.SQLite)
        {
            var actualResults = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, string? UniqueId)>(sql, parameters);
            
            // Assert - Compare SQL results to LINQ results using TestDataConstants
            // UniqueId != Guid.Empty should return records with non-empty GUIDs (excluding nulls and empty GUIDs)
            var expectedResults = TestDataConstants.Products
                .Where(p => p.UniqueId != null && p.UniqueId != Guid.Empty)
                .Select(p => (p.Id, p.ProductName, p.Price, p.CreatedDate, p.UniqueId.ToString()))
                .ToList();

            var actualResultsList = actualResults.Select(r => (r.Id, r.ProductName, r.Price, r.CreatedDate, r.UniqueId)).ToList();
            Assert.Equal(expectedResults, actualResultsList);
        }
        else
        {
            var actualResults = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, Guid? UniqueId)>(sql, parameters);
            
            // Assert - Compare SQL results to LINQ results using TestDataConstants
            // UniqueId != Guid.Empty should return records with non-empty GUIDs (excluding nulls and empty GUIDs)
            var expectedResults = TestDataConstants.Products
                .Where(p => p.UniqueId != null && p.UniqueId != Guid.Empty)
                .Select(p => (p.Id, p.ProductName, p.Price, p.CreatedDate, p.UniqueId))
                .ToList();

            var actualResultsList = actualResults.ToList();
            Assert.Equal(expectedResults, actualResultsList);
        }
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task From_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        using var connection = _fixture.CreateConnection(databaseType);
        var query = TestQueries.From();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        var actualResults = await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task InnerJoinBasic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        using var connection = _fixture.CreateConnection(databaseType);
        var query = TestQueries.InnerJoinBasic();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        var actualResults = await connection.QueryAsync<(int Id, string Name, int OrderId, int Amount)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Join(TestDataConstants.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (Id: c.Id, Name: c.Name, OrderId: o.Id, Amount: o.Amount))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task InnerJoinWithGroupBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        using var connection = _fixture.CreateConnection(databaseType);
        var query = TestQueries.InnerJoinWithGroupBy();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        var actualResults = await connection.QueryAsync<(int CustomerId, string CustomerName, int TotalAmount)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Join(TestDataConstants.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => new { CustomerId = c.Id, CustomerName = c.Name, Amount = o.Amount })
            .GroupBy(result => new { result.CustomerId, result.CustomerName })
            .Select(g => (CustomerId: g.Key.CustomerId, CustomerName: g.Key.CustomerName, TotalAmount: g.Sum(x => x.Amount)))
            .OrderBy(x => x.CustomerId)
            .ToList();

        var actualResultsList = actualResults.OrderBy(x => x.CustomerId).ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task InnerJoinWithOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        using var connection = _fixture.CreateConnection(databaseType);
        var query = TestQueries.InnerJoinWithOrderBy();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        var actualResults = await connection.QueryAsync<(int Id, string Name, int OrderId, int Amount)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Join(TestDataConstants.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (Id: c.Id, Name: c.Name, OrderId: o.Id, Amount: o.Amount))
            .OrderBy(result => result.Name)
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task InnerJoinWithSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        using var connection = _fixture.CreateConnection(databaseType);
        var query = TestQueries.InnerJoinWithSelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        var actualResults = await connection.QueryAsync<(string CustomerName, int OrderAmount)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Join(TestDataConstants.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (CustomerName: c.Name, OrderAmount: o.Amount))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task InnerJoinWithWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.InnerJoinWithWhere(); // WHERE c.Age >= 18 AND o.Amount > 100
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(int Id, string Name, int Age, int OrderId, int Amount)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age >= 18)
            .Join(TestDataConstants.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (c.Id, c.Name, c.Age, o.Id, o.Amount))
            .Where(result => result.Amount > 100)
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task JoinFusionWithWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.JoinFusionWithWhere(); // Complex multi-join with WHERE
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(int CustomerId, string CustomerName, int Amount, string ProductName)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        // This is a complex join: Customers -> Orders -> Products with WHERE c.Age >= 18 AND Amount > 100
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age >= 18)
            .Join(TestDataConstants.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => new { Customer = c, Order = o })
            .Join(TestDataConstants.Products,
                joined => joined.Order.ProductId, // Using ProductId as proper foreign key
                p => p.Id,
                (joined, p) => (CustomerId: joined.Customer.Id, CustomerName: joined.Customer.Name, Amount: joined.Order.Amount, ProductName: p.ProductName))
            .Where(result => result.Amount > 100) // Additional WHERE clause to match the query
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task LeftJoinBasic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.LeftJoinBasic(); // LEFT JOIN Customers and Orders
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(int Id, string Name, int? OrderId, int? Amount)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .GroupJoin(TestDataConstants.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, orders) => new { Customer = c, Orders = orders })
            .SelectMany(
                x => x.Orders.DefaultIfEmpty(),
                (x, order) => (x.Customer.Id, x.Customer.Name, 
                    OrderId: order.Id == 0 ? (int?)null : order.Id, 
                    Amount: order.Amount == 0 ? (int?)null : order.Amount))
            .OrderBy(result => result.Id)
            .ThenBy(result => result.OrderId)
            .ToList();

        var actualResultsList = actualResults.OrderBy(result => result.Id).ThenBy(result => result.OrderId).ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task LeftJoinWithAggregates_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.LeftJoinWithAggregates(); // LEFT JOIN with GROUP BY and aggregations
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(int CustomerId, int OrderCount, int TotalSpent)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        // Note: The SQL query groups by result.Id only
        // LEFT JOIN + GROUP BY counts at least 1 row per customer (even if no orders)
        var expectedResults = TestDataConstants.Customers
            .GroupJoin(TestDataConstants.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, orders) => new { Customer = c, Orders = orders.DefaultIfEmpty() })
            .Select(x => (
                CustomerId: x.Customer.Id,                
                OrderCount: x.Orders.Count(), // COUNT() in GROUP BY always returns at least 1 for LEFT JOIN
                TotalSpent: x.Orders.Where(o => o.Id != 0).Sum(o => o.Amount))) // Only sum real orders
            .OrderBy(x => x.CustomerId)
            .ToList();

        var actualResultsList = actualResults.OrderBy(x => x.CustomerId).ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task LeftJoinWithOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.LeftJoinWithOrderBy(); // LEFT JOIN with ORDER BY Name ASC, Amount DESC
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(int Id, string Name, int? OrderId, int? Amount)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .GroupJoin(TestDataConstants.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, orders) => new { Customer = c, Orders = orders })
            .SelectMany(
                x => x.Orders.DefaultIfEmpty(),
                (x, order) => (x.Customer.Id, x.Customer.Name, 
                    OrderId: order.Id == 0 ? (int?)null : order.Id, 
                    Amount: order.Amount == 0 ? (int?)null : order.Amount))
            .OrderBy(result => result.Name)
            .ThenByDescending(result => result.Amount)
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task LeftJoinWithSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.LeftJoinWithSelect(); // LEFT JOIN with custom SELECT (string concatenation)
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(string CustomerInfo, int? OrderAmount)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .GroupJoin(TestDataConstants.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, orders) => new { Customer = c, Orders = orders })
            .SelectMany(
                x => x.Orders.DefaultIfEmpty(),
                (x, order) => (
                    CustomerInfo: x.Customer.Name + " (Customer)", 
                    OrderAmount: order.Amount == 0 ? (int?)null : order.Amount))
            .OrderBy(x => x.CustomerInfo)
            .ThenBy(x => x.OrderAmount)
            .ToList();

        var actualResultsList = actualResults.OrderBy(x => x.CustomerInfo).ThenBy(x => x.OrderAmount).ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task LeftJoinWithWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.LeftJoinWithWhere(); // LEFT JOIN with WHERE clause filtering
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(int Id, string Name, int Age, int? OrderId, int? OrderAmount)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        // WHERE c.Age >= 21 AND result.Age < 65 with LEFT JOIN
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age >= 21 && c.Age < 65)
            .GroupJoin(TestDataConstants.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, orders) => new { Customer = c, Orders = orders })
            .SelectMany(
                x => x.Orders.DefaultIfEmpty(),
                (x, order) => (x.Customer.Id, x.Customer.Name, x.Customer.Age,
                    OrderId: order.Id == 0 ? (int?)null : order.Id, 
                    OrderAmount: order.Amount == 0 ? (int?)null : order.Amount))
            .OrderBy(result => result.Id)
            .ThenBy(result => result.OrderId)
            .ToList();

        var actualResultsList = actualResults.OrderBy(result => result.Id).ThenBy(result => result.OrderId).ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task LikeBothWildcards_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.LikeBothWildcards(); // LIKE pattern with both wildcards: %o_n%
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(int Id, string Name)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        // LIKE '%o_n%' should match names containing 'o' followed by any single character then 'n'
        var expectedResults = TestDataConstants.Customers
            .Where(c => System.Text.RegularExpressions.Regex.IsMatch(c.Name, ".*o.n.*"))
            .Select(c => (c.Id, c.Name))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task LikeExact_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.LikeExact(); // LIKE pattern with exact match (no wildcards)
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(int Id, string Name)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        // LIKE 'John' should match names exactly equal to 'John'
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Name == "John")
            .Select(c => (c.Id, c.Name))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task LikeSingleChar_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.LikeSingleChar(); // LIKE pattern with single character wildcard (_)
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(int Id, string Name)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        // LIKE 'J_n' should match names like 'Jin', 'Jon' where _ matches exactly one character
        var expectedResults = TestDataConstants.Customers
            .Where(c => System.Text.RegularExpressions.Regex.IsMatch(c.Name, "^J.n$"))
            .Select(c => (c.Id, c.Name))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task LikeWildcard_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.LikeWildcard(); // LIKE pattern with single wildcard: J%
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
        var actualResults = (await connection.QueryAsync<(int Id, string Name)>(sql, dapperParams)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        // LIKE 'Jo%' should match names starting with 'Jo'
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Name.StartsWith("Jo"))
            .Select(c => (c.Id, c.Name))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task MathFunctionsInSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.MathFunctionsInSelect(); // SELECT with math functions (Round, Ceiling, Floor)
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        var dapperParams = parameters;
            
        var actualResults = (await connection.QueryAsync<(int Id, decimal? OriginalPrice, decimal? RoundedPrice, decimal? CeilingPrice, decimal? FloorPrice)>(sql, dapperParams)).ToList();
        
        var expectedResults = TestDataConstants.Products
            .Select(p => (
                Id: p.Id, 
                OriginalPrice: p.Price,
                RoundedPrice: p.Price != null ? Math.Round(p.Price.Value, 2) : (decimal?)null,
                CeilingPrice: p.Price != null ? Math.Ceiling(p.Price.Value) : (decimal?)null,
                FloorPrice: p.Price != null ? Math.Floor(p.Price.Value) : (decimal?)null))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task MathFunctionsInWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.MathFunctionsInWhere(); // WHERE Price.Round(0) > 100 AND Price.Ceiling() < 1000
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, decimal? Price)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Where(p => p.Price.HasValue && 
                       Math.Round(p.Price.Value, 0) > 100 && 
                       Math.Ceiling(p.Price.Value) < 1000)
            .Select(p => (p.Id, p.Price))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task MaxPrice_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.MaxPrice(); // SELECT MAX(Price) FROM products
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResult = await connection.QuerySingleAsync<decimal?>(sql, parameters);

        // Assert - Compare SQL result to LINQ result using TestDataConstants
        var expectedResult = TestDataConstants.Products
            .Select(p => p.Price)
            .Max(); // Should be 999.99m (Laptop)

        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task MinPrice_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.MinPrice(); // SELECT MIN(Price) FROM products
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResult = await connection.QuerySingleAsync<decimal?>(sql, parameters);

        // Assert - Compare SQL result to LINQ result using TestDataConstants
        var expectedResult = TestDataConstants.Products
            .Select(p => p.Price)
            .Min(); // Should be 25.50m (Mouse)

        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task MixedJoinTypesFusion_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.MixedJoinTypesFusion(); // Complex query mixing INNER and LEFT joins
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int CustomerId, string CustomerName, int? OrderId, string ProductName)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        // This represents mixed joins: Customers -> Orders (LEFT) -> Products (LEFT JOIN when no product match)
        // The SQL returns customers with orders but null product info when no Product matches the Order.ProductId
        var expectedResults = TestDataConstants.Customers
            .GroupJoin(TestDataConstants.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, orders) => new { Customer = c, Orders = orders })
            .SelectMany(
                x => x.Orders.DefaultIfEmpty(),
                (x, order) => new { x.Customer, Order = order })
            .Where(joined => joined.Order.Id != 0) // Only include customers with orders
            .GroupJoin(TestDataConstants.Products,
                joined => joined.Order.ProductId, // Using ProductId as proper foreign key
                p => p.Id,
                (joined, products) => new { joined.Customer, joined.Order, Products = products })
            .SelectMany(
                x => x.Products.DefaultIfEmpty(),
                (x, product) => (CustomerId: x.Customer.Id, 
                    CustomerName: x.Customer.Name, 
                    OrderId: (int?)x.Order.Id, 
                    ProductName: product.ProductName))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task MultipleInnerJoinsFusion_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.MultipleInnerJoinsFusion(); // Complex multi-table inner joins
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int CustomerId, string CustomerName, int OrderId, string ProductName)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        // This is a complex join: Customers -> Orders -> Products (using ProductId as foreign key)
        var expectedResults = TestDataConstants.Customers
            .Join(TestDataConstants.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => new { Customer = c, Order = o })
            .Join(TestDataConstants.Products,
                joined => joined.Order.ProductId, // Using ProductId as proper foreign key
                p => p.Id,
                (joined, p) => (CustomerId: joined.Customer.Id, CustomerName: joined.Customer.Name, OrderId: joined.Order.Id, ProductName: p.ProductName))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SQLite)]
    public async Task ParameterAsBoolParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // NOTE: SQL Server and PostgreSQL excluded due to boolean expression comparison complexity
        // - SQL Server: Doesn't support boolean comparison syntax like "(expression) = @parameter"
        // - PostgreSQL: Complex boolean expression handling that would require special SQL generation
        // SQLite handles this boolean parameter comparison correctly without special cases
        
        // Arrange
        var query = TestQueries.ParameterAsBoolParam(); // Complex boolean parameter query
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Set parameter value - test both true and false cases
        const bool isAdultValue = true; // Test case: we want adults (Age > 18)
        var updatedParameters = parameters.SetItem(FormatParam("isAdult", databaseType), isAdultValue);
        
        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, string Name, int Age)>(sql, updatedParameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        // Logic: (c.Age > 18) == isAdult should return customers where being an adult matches the parameter
        var expectedResults = TestDataConstants.Customers
            .Where(c => (c.Age > 18) == isAdultValue)
            .Select(c => (c.Id, c.Name, c.Age))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task ParameterAsDateTimeParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        using var connection = _fixture.CreateConnection(databaseType);
        var query = TestQueries.ParameterAsDateTimeParam();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Set parameter value - choose a date that will return only Product 2 (2023-06-10)
        var startDate = new DateTime(2023, 3, 1);
        var updatedParameters = parameters.ToDictionary(kvp => kvp.Key, kvp => (object?)startDate);

        // Act
        if (databaseType == DatabaseType.SQLite)
        {
            var actualResults = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, string CreatedDate, string UniqueId)>(sql, updatedParameters);
            
            // Convert to expected format for comparison
            var actualResultsList = actualResults.Select(r => (r.Id, r.ProductName, r.Price, 
                string.IsNullOrEmpty(r.CreatedDate) ? (DateTime?)null : DateTime.Parse(r.CreatedDate), 
                string.IsNullOrEmpty(r.UniqueId) ? (Guid?)null : Guid.Parse(r.UniqueId))).ToList();
            
            var expectedResults = TestDataConstants.Products
                .Where(p => p.CreatedDate.HasValue && p.CreatedDate.Value > startDate)
                .ToList();

            // Assert
            Assert.Equal(expectedResults, actualResultsList);
        }
        else
        {
            var actualResults = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, Guid? UniqueId)>(sql, updatedParameters);
            var actualResultsList = actualResults.ToList();
            
            var expectedResults = TestDataConstants.Products
                .Where(p => p.CreatedDate.HasValue && p.CreatedDate.Value > startDate)
                .ToList();

            // Assert
            Assert.Equal(expectedResults, actualResultsList);
        }
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task ParameterAsDecimalParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.ParameterAsDecimalParam(); // WHERE Price > @minPrice/@minPrice/:minPrice
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Set parameter value using helper method
        const decimal minPriceValue = 50.0m;
        var updatedParameters = parameters.SetItem(FormatParam("minPrice", databaseType), minPriceValue);
        
        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        // Use simpler result mapping to avoid SQLite GUID casting issues
        if (databaseType == DatabaseType.SQLite)
        {
            var actualResults = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, string CreatedDate, string UniqueId)>(sql, updatedParameters);
            
            // Convert to expected format for comparison
            var actualResultsList = actualResults.Select(r => (r.Id, r.ProductName, r.Price, 
                string.IsNullOrEmpty(r.CreatedDate) ? (DateTime?)null : DateTime.Parse(r.CreatedDate), 
                string.IsNullOrEmpty(r.UniqueId) ? (Guid?)null : Guid.Parse(r.UniqueId))).ToList();
            
            var expectedResults = TestDataConstants.Products
                .Where(p => p.Price > minPriceValue) // Same WHERE logic as SQL query: Price > 50.0
                .Select(p => (p.Id, p.ProductName, p.Price, p.CreatedDate, p.UniqueId)) // Full product data
                .ToList();

            Assert.Equal(expectedResults, actualResultsList);
        }
        else
        {
            var actualResults = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, Guid? UniqueId)>(sql, updatedParameters);

            var expectedResults = TestDataConstants.Products
                .Where(p => p.Price > minPriceValue) // Same WHERE logic as SQL query: Price > 50.0
                .Select(p => (p.Id, p.ProductName, p.Price, p.CreatedDate, p.UniqueId)) // Full product data
                .ToList();

            var actualResultsList = actualResults.ToList();
            Assert.Equal(expectedResults, actualResultsList);
        }
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task ParameterAsGuidParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.ParameterAsGuidParam(); // WHERE UniqueId = @targetId/@targetId/:targetId
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Set parameter value using helper method - use a GUID from test data
        var targetIdValue = new Guid("123e4567-e89b-12d3-a456-426614174001"); // Use from TestDataConstants if available
        var updatedParameters = parameters.SetItem(FormatParam("targetId", databaseType), targetIdValue);
        
        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        
        // SQLite stores GUIDs as strings, so we need different handling
        if (databaseType == DatabaseType.SQLite)
        {
            var actualResults = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, string CreatedDate, string UniqueId)>(sql, updatedParameters);
            
            // Convert to expected format for comparison
            var actualResultsList = actualResults.Select(r => (r.Id, r.ProductName, r.Price, 
                string.IsNullOrEmpty(r.CreatedDate) ? (DateTime?)null : DateTime.Parse(r.CreatedDate), 
                string.IsNullOrEmpty(r.UniqueId) ? (Guid?)null : Guid.Parse(r.UniqueId))).ToList();
            
            var expectedResults = TestDataConstants.Products
                .Where(p => p.UniqueId == targetIdValue) // Same WHERE logic as SQL query: UniqueId = targetIdValue
                .Select(p => (p.Id, p.ProductName, p.Price, p.CreatedDate, p.UniqueId)) // Full product data
                .ToList();

            Assert.Equal(expectedResults, actualResultsList);
        }
        else
        {
            var actualResults = await connection.QueryAsync<(int Id, string ProductName, decimal? Price, DateTime? CreatedDate, Guid? UniqueId)>(sql, updatedParameters);

            var expectedResults = TestDataConstants.Products
                .Where(p => p.UniqueId == targetIdValue) // Same WHERE logic as SQL query: UniqueId = targetIdValue
                .Select(p => (p.Id, p.ProductName, p.Price, p.CreatedDate, p.UniqueId)) // Full product data
                .ToList();

            var actualResultsList = actualResults.ToList();
            Assert.Equal(expectedResults, actualResultsList);
        }
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task ParameterAsIntParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.ParameterAsIntParam(); // WHERE Age > @minAge/@minAge/:minAge
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Set parameter value using helper method
        const int minAgeValue = 25;
        var updatedParameters = parameters.SetItem(FormatParam("minAge", databaseType), minAgeValue);
        
        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, string Name)>(sql, updatedParameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > minAgeValue) // Same WHERE logic as SQL query: Age > 25
            .Select(c => (c.Id, c.Name)) // Same SELECT logic: Id, Name
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task ParameterAsStringParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.ParameterAsStringParam(); // WHERE Name = @customerName/@customerName/:customerName
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Set parameter value using helper method
        const string customerNameValue = "Alice Johnson";
        var updatedParameters = parameters.SetItem(FormatParam("customerName", databaseType), customerNameValue);
        
        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, int Age)>(sql, updatedParameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Name == customerNameValue) // Same WHERE logic as SQL query: Name = 'Alice Johnson'
            .Select(c => (c.Id, c.Age)) // Same SELECT logic: Id, Age
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task StringFunctionsInSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.StringFunctionsInSelect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, string UpperName, string LowerName, string TrimmedName, int NameLength, string FirstThree)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Select(c => (
                c.Id, 
                UpperName: c.Name.ToUpper(),
                LowerName: c.Name.ToLower(),
                TrimmedName: c.Name.Trim(),
                NameLength: c.Name.Length,
                FirstThree: c.Name.Substring(0, Math.Min(3, c.Name.Length))
            ))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task StringFunctionsInWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.StringFunctionsInWhere();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<(int Id, string Name)>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Name.ToUpper() == "JOHN" && c.Name.Length > 3)
            .Select(c => (c.Id, c.Name))
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task StringLength_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.StringLength(); // SELECT ProductName.Length() FROM Products
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<int>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => p.ProductName.Length) // Same SELECT logic: ProductName.Length
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task StringLower_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.StringLower(); // SELECT ProductName.Lower() FROM Products
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<string>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => p.ProductName.ToLower()) // Same SELECT logic: ProductName.ToLower()
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task StringSubstring_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.StringSubstring(); // SELECT ProductName.Substring(1, 5) FROM Products
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<string>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => p.ProductName.Substring(0, Math.Min(5, p.ProductName.Length))) // C# uses 0-based indexing, SQL uses 1-based
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task StringTrim_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.StringTrim(); // SELECT ProductName.Trim() FROM Products
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<string>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => p.ProductName.Trim()) // Same SELECT logic: ProductName.Trim()
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task StringUpper_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.StringUpper(); // SELECT ProductName.Upper() FROM Products
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act 
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = await connection.QueryAsync<string>(sql, parameters);

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Products
            .Select(p => p.ProductName.ToUpper()) // Same SELECT logic: ProductName.ToUpper()
            .ToList();

        var actualResultsList = actualResults.ToList();
        Assert.Equal(expectedResults, actualResultsList);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task SumAges_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.SumAges(); // SELECT SUM(Age) FROM customers
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResult = await connection.QuerySingleAsync<int>(sql, parameters);

        // Assert - Compare SQL result to LINQ result using TestDataConstants
        var expectedResult = TestDataConstants.Customers.Sum(c => c.Age);

        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task SumExpensivePrices_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.SumExpensivePrices(); // SELECT SUM(Price) FROM products WHERE Price > 100
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResult = await connection.QuerySingleAsync<decimal?>(sql, parameters);

        // Assert - Compare SQL result to LINQ result using TestDataConstants
        var expectedResult = TestDataConstants.Products
            .Where(p => p.Price > 100m) // Only Laptop (999.99m) qualifies
            .Sum(p => p.Price!.Value); // Should be 999.99

        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task SumPrices_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.SumPrices(); // SELECT SUM(Price) FROM products
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResult = await connection.QuerySingleAsync<decimal?>(sql, parameters);

        // Assert - Compare SQL result to LINQ result using TestDataConstants
        var expectedResult = TestDataConstants.Products
            .Where(p => p.Price.HasValue) // Filter out null prices
            .Sum(p => p.Price!.Value); // Should be 999.99 + 25.50 = 1025.49

        Assert.Equal(expectedResult, actualResult);
    }

    // LimitOffset Integration Tests
    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromLimitOffset_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromLimitOffset(); // LIMIT 5 OFFSET 10
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .OrderBy(c => c.Id)
            .Skip(10) // offset 10
            .Take(5)  // limit 5
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromSelectLimitOffset_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromSelectLimitOffset(); // SELECT Id, Name LIMIT 3 OFFSET 5
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<(int Id, string Name)>(sql, parameters)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .OrderBy(c => c.Id)
            .Skip(5) // offset 5
            .Take(3) // limit 3
            .Select(c => (c.Id, c.Name))
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereLimitOffset_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereLimitOffset(); // WHERE Age > 18 LIMIT 10 OFFSET 0
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 18)
            .OrderBy(c => c.Id)
            .Skip(0)  // offset 0
            .Take(10) // limit 10
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereSelectLimitOffset_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereSelectLimitOffset(); // WHERE Age >= 21 SELECT Id, Name, Age LIMIT 5 OFFSET 15
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<(int Id, string Name, int Age)>(sql, parameters)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age >= 21)
            .OrderBy(c => c.Id)
            .Skip(15) // offset 15
            .Take(5)  // limit 5
            .Select(c => (c.Id, c.Name, c.Age))
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromOrderByLimitOffset_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromOrderByLimitOffset(); // ORDER BY Name ASC LIMIT 10 OFFSET 5
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .OrderBy(c => c.Name)
            .Skip(5)  // offset 5
            .Take(10) // limit 10
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereOrderByLimitOffset_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereOrderByLimitOffset(); // WHERE Age > 18 ORDER BY Age DESC LIMIT 20 OFFSET 10
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 18)
            .OrderByDescending(c => c.Age)
            .Skip(10) // offset 10
            .Take(20) // limit 20
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromWhereOrderBySelectLimitOffset_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereOrderBySelectLimitOffset(); // WHERE Name != "" ORDER BY Name ASC, Age DESC SELECT Id, Name, Age LIMIT 5 OFFSET 0
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<(int Id, string Name, int Age)>(sql, parameters)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Name != "")
            .OrderBy(c => c.Name).ThenByDescending(c => c.Age)
            .Skip(0) // offset 0
            .Take(5) // limit 5
            .Select(c => (c.Id, c.Name, c.Age))
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromLimitOffsetOnly_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromLimitOffsetOnly(); // LIMIT 10 (no offset - null offset)
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .OrderBy(c => c.Id)
            .Take(10) // limit 10 only, no skip (offset is null)
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromOffsetOnly_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromOffsetOnly(); // LIMIT long.MaxValue OFFSET 5 (effectively just offset 5, take all remaining)
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .OrderBy(c => c.Id)
            .Skip(5) // offset 5
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    // Special integration test for LIMIT without ORDER BY - only PostgreSQL and SQLite support this
    [Theory]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public async Task FromLimitOffsetWithoutOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromLimitOffsetWithoutOrderBy();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, parameters)).ToList();

        // Assert - Should return limited results (up to 10 customers)
        var expectedResults = TestDataConstants.Customers
            .Take(10) // Same logic as SQL query: LIMIT 10
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToList();

        Assert.True(actualResults.Count <= 10, "Should return at most 10 results due to LIMIT 10");
        Assert.True(actualResults.Count > 0, "Should return some results from test data");
        
        // Note: We can't guarantee exact ordering without ORDER BY, so we just verify count constraints
    }

    // DISTINCT Integration Tests
    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromSelectDistinct_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromSelectDistinct();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<string>(sql, parameters)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Select(c => c.Name)
            .Distinct()
            .ToList();

        Assert.Equal(expectedResults.Count, actualResults.Count);
        Assert.True(expectedResults.All(name => actualResults.Contains(name)));
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromSelectDistinctWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromSelectDistinctWhere();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<string>(sql, parameters)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 18)
            .Select(c => c.Name)
            .Distinct()
            .ToList();

        Assert.Equal(expectedResults.Count, actualResults.Count);
        Assert.True(expectedResults.All(name => actualResults.Contains(name)));
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromSelectDistinctOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromSelectDistinctOrderBy();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<string>(sql, parameters)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Select(c => c.Name)
            .Distinct()
            .OrderBy(name => name)
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task FromSelectDistinctMultipleColumns_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromSelectDistinctMultipleColumns();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<(string Name, int Age)>(sql, parameters)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Select(c => (c.Name, c.Age))
            .Distinct()
            .OrderBy(x => x.Name)
            .ToList();

        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task Union_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.Union();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<(int Id, string Name)>(sql, parameters)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var query1Results = TestDataConstants.Customers
            .Where(c => c.Age > 30)
            .Select(c => (c.Id, c.Name));
            
        var query2Results = TestDataConstants.Customers
            .Where(c => c.Name == "Alice")
            .Select(c => (c.Id, c.Name));
            
        var expectedResults = query1Results
            .Union(query2Results)
            .OrderBy(x => x.Id)
            .ToList();

        actualResults = actualResults.OrderBy(x => x.Id).ToList();
        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task Intersect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.Intersect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<(int Id, string Name)>(sql, parameters)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var query1Results = TestDataConstants.Customers
            .Where(c => c.Age > 25)
            .Select(c => (c.Id, c.Name));
            
        var query2Results = TestDataConstants.Customers
            .Where(c => c.Name == "John")
            .Select(c => (c.Id, c.Name));
            
        var expectedResults = query1Results
            .Intersect(query2Results)
            .OrderBy(x => x.Id)
            .ToList();

        actualResults = actualResults.OrderBy(x => x.Id).ToList();
        Assert.Equal(expectedResults, actualResults);
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public async Task Except_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.Except();
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Act
        using var connection = _fixture.CreateConnection(databaseType);
        var actualResults = (await connection.QueryAsync<(int Id, string Name)>(sql, parameters)).ToList();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var allCustomers = TestDataConstants.Customers
            .Select(c => (c.Id, c.Name));
            
        var underage = TestDataConstants.Customers
            .Where(c => c.Age < 18)
            .Select(c => (c.Id, c.Name));
            
        var expectedResults = allCustomers
            .Except(underage)
            .OrderBy(x => x.Id)
            .ToList();

        actualResults = actualResults.OrderBy(x => x.Id).ToList();
        Assert.Equal(expectedResults, actualResults);
    }
}
