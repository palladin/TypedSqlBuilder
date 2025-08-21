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
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var actualResults = (await connection.QueryAsync<(int Id, int Age, string Name, bool IsActive)>(sql, dapperParams)).ToHashSet();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => c.Age > 18) // Same logic as SQL query
            .Select(c => (c.Id, c.Age, c.Name, c.IsActive))
            .ToHashSet();

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
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var actualResults = (await connection.QueryAsync<(int Id, int Age)>(sql, dapperParams)).ToHashSet();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Select(c => (c.Id, Math.Abs(c.Age))) // Same logic as SQL query: (Id, ABS(Age))
            .ToHashSet();

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
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var actualResults = (await connection.QueryAsync<(int Id, int AgeExpression)>(sql, dapperParams)).ToHashSet();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Select(c => (c.Id, Math.Abs(c.Age - 50))) // Same logic as SQL query: (Id, ABS(Age - 50))
            .ToHashSet();

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
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var actualResults = (await connection.QueryAsync<(int Id, string Name, int Age)>(sql, dapperParams)).ToHashSet();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => Math.Abs(c.Age) > 30) // Same WHERE logic as SQL query: ABS(Age) > 30
            .Select(c => (c.Id, c.Name, c.Age)) // Same SELECT logic: Id, Name, Age
            .ToHashSet();

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
        connection.Open();
        var dapperParams = updatedParameters.ToDapperParameters();
        var actualResults = (await connection.QueryAsync<(int Id, string Name, int Age)>(sql, dapperParams)).ToHashSet();

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResults = TestDataConstants.Customers
            .Where(c => Math.Abs(c.Age) > Math.Abs(minAgeValue)) // Same WHERE logic as SQL query: ABS(Age) > ABS(20)
            .Select(c => (c.Id, c.Name, c.Age)) // Same SELECT logic: Id, Name, Age
            .ToHashSet();

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
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
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
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
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
        var actualResult = await connection.QueryAsync<(int Id, string Name, int Age, bool IsActive)>(sql, updatedParameters.ToDapperParameters());

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResult = TestDataConstants.Customers
            .Where(c => c.IsActive)            
            .Select(c => (c.Id, c.Name, c.Age, c.IsActive))
            .ToHashSet();

        Assert.Equal(expectedResult, actualResult.ToHashSet());
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
        var actualResult = await connection.QueryAsync<(int Id, string Name, int Age, bool IsActive)>(sql, parameters.ToDapperParameters());

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResult = TestDataConstants.Customers
            .Where(c => !c.IsActive) // WHERE IsActive = false
            .Select(c => (c.Id, c.Name, c.Age, c.IsActive))
            .ToHashSet();

        Assert.Equal(expectedResult, actualResult.ToHashSet());
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
        var actualResult = await connection.QueryAsync<(int Id, string Name, int Age, bool IsActive)>(sql, parameters.ToDapperParameters());

        // Assert - Compare SQL results to LINQ results using TestDataConstants
        var expectedResult = TestDataConstants.Customers
            .Where(c => c.IsActive) // WHERE IsActive = true
            .Select(c => (c.Id, c.Name, c.Age, c.IsActive))
            .ToHashSet();

        Assert.Equal(expectedResult, actualResult.ToHashSet());
    }

    public Task CaseBoolExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task CaseDateTimeExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task CaseDecimalExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task CaseGuidExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task CaseIntExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task CaseInWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task CaseStringExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task ComplexJoinWhereGroupByHavingOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task ComplexLeftJoinWhereGroupByOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task CountActiveCustomersWithDb_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task CountActiveCustomers_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task CountCustomersWithDb_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task CountCustomers_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task DateTimeAddDays_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task DateTimeAddMonths_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task DateTimeAddYears_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task DateTimeDay_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task DateTimeDiffDays_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task DateTimeDiffMonths_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task DateTimeDiffYears_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task DateTimeFunctionsInSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task DateTimeFunctionsInWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task DateTimeMonth_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task DateTimeNow_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task DateTimeYear_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task DecimalCeiling_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task DecimalFloor_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task DecimalRound_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromGroupByAvgSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromGroupByDecimalAggregatesSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromGroupByDecimalAvgSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromGroupByDecimalSumSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromGroupByHavingOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromGroupByHavingSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromGroupByMinMaxSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromGroupByMultipleOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromGroupByMultipleSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromGroupByOrderByMultipleSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromGroupByOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromGroupByOrderByThreeKeysSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromGroupBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromOrderByAsc_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromOrderByDescendingThenBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromOrderByDesc_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromOrderByMultiple_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromOrderByThenByDescending_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromOrderByThenBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromOrderByThenBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromProductWhereSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromSelectAvg_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromSelectCreatedDateMinMax_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromSelectDecimalArithmetic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromSelectExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromSelectMax_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromSelectMin_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromSelectOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromSelectSingle_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromSelectSum_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromStatic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromSubquery_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereAgeGreaterThanAverageAge_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereAgeGreaterThanSum_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereAgeInSubqueryWithClosure_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereAgeInSubquery_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereAgeIn_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereAndSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereAnd_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereCreatedDateComparison_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereCreatedDateIsNotNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereCreatedDateIsNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereDecimalComparison_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereDecimalIsNotNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereDecimalIsNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereFusionThree_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereFusionTwo_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereFusionWithOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereFusionWithSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereGroupBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }    

    public Task FromWhereIsNotNullInt_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereIsNotNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereIsNullCombined_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereIsNullInt_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereIsNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereMultiple_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereOrderByThenBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereOr_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereSelectNamed_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereSelectOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereSelectParameterized_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereSelectWhereFromNested_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereSelectWhereNested_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereString_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereUniqueIdEquals_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereUniqueIdIsNotNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereUniqueIdIsNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task FromWhereUniqueIdNotEquals_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task From_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task InnerJoinBasic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task InnerJoinWithGroupBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task InnerJoinWithOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task InnerJoinWithSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task InnerJoinWithWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task JoinFusionWithWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task LeftJoinBasic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task LeftJoinWithAggregates_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task LeftJoinWithOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task LeftJoinWithSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task LeftJoinWithWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task LikeBothWildcards_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task LikeExact_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task LikeSingleChar_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task LikeWildcard_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task MathFunctionsInSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task MathFunctionsInWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task MaxPrice_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task MinPrice_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task MixedJoinTypesFusion_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task MultipleInnerJoinsFusion_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task ParameterAsBoolParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task ParameterAsDateTimeParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task ParameterAsDecimalParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task ParameterAsGuidParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task ParameterAsIntParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task ParameterAsStringParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task StringFunctionsInSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task StringFunctionsInWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task StringLength_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task StringLower_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task StringSubstring_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task StringTrim_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task StringUpper_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task SumAgesWithDb_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task SumAges_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task SumExpensivePrices_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }

    public Task SumPrices_GeneratesCorrectSql(DatabaseType databaseType)
    {
        throw new NotImplementedException();
    }
}
