using System.Collections.Immutable;
using TypedSqlBuilder.Core;
using TypedSqlBuilder.TestModels;

namespace TypedSqlBuilder.Tests;

public class QueryTests : IQueryTestContract
{

    private static readonly Dictionary<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)> TestCases = QueryTestCases.TestCases;

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task From_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.From();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(From_GeneratesCorrectSql))];

        // Assert
        Assert.Equal(expectedSql, sql);
        Assert.Empty(expectedParameters);
        return Task.CompletedTask;
    }



    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task AbsColumn_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.AbsColumn();
        var testName = nameof(AbsColumn_GeneratesCorrectSql);

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Assert
        var (expectedSql, expectedParameterNames) = TestCases[(databaseType, testName)];
        Assert.Equal(expectedSql, sql);
        Assert.Empty(expectedParameterNames);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task AbsExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.AbsExpression();
        var testName = nameof(AbsExpression_GeneratesCorrectSql);
        
        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Assert
        var (expectedSql, expectedParameterNames) = TestCases[(databaseType, testName)];
        Assert.Equal(expectedSql, sql);
        Assert.Single(expectedParameterNames);
        Assert.Equal(50, parameters[expectedParameterNames[0]]);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task AbsInWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.AbsInWhere();
        var testName = nameof(AbsInWhere_GeneratesCorrectSql);
        
        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        // Assert
        var (expectedSql, expectedParameterNames) = TestCases[(databaseType, testName)];
        Assert.Equal(expectedSql, sql);
        Assert.Single(expectedParameterNames);
        Assert.Equal(30, parameters[expectedParameterNames[0]]);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task AbsParameter_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.AbsParameter();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var testCase = TestCases[(databaseType, nameof(AbsParameter_GeneratesCorrectSql))];
        Assert.Equal(testCase.Sql, sql);
        
        // Validate parameters
        Assert.Single(parameters);
        var expectedParameterNames = testCase.ParameterNames;
        Assert.Null(parameters[expectedParameterNames[0]]);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task AvgExpensivePrices_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var avgExpr = TestQueries.AvgExpensivePrices();

        // Act
        var (sql, parameters) = avgExpr.ToSqlRaw(databaseType);

        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(AvgExpensivePrices_GeneratesCorrectSql))];

        // Assert
        Assert.Equal(expectedSql, sql);
        Assert.Single(expectedParameters);
        Assert.Equal(100m, parameters[expectedParameters[0]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task AvgPrices_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var avgExpr = TestQueries.AvgPrices();

        // Act
        var (sql, parameters) = avgExpr.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(AvgPrices_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Empty(expectedParameters);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task BoolColumnDirectComparison_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.BoolColumnDirectComparison();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(BoolColumnDirectComparison_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Single(expectedParameters);
        
        var parameterKey = expectedParameters[0];
        Assert.True(parameters.ContainsKey(parameterKey));
        
        return Task.CompletedTask;
    }

    

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task BoolColumnLiteralFalse_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.BoolColumnLiteralFalse();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(BoolColumnLiteralFalse_GeneratesCorrectSql))];

        // Assert
        Assert.Equal(expectedSql, sql);
        
        if (databaseType == DatabaseType.PostgreSQL)
        {
            Assert.Empty(parameters);
        }
        else
        {
            Assert.Single(parameters);
            Assert.Equal(0, parameters[expectedParameters[0]]);
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task BoolColumnLiteralTrue_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.BoolColumnLiteralTrue();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(BoolColumnLiteralTrue_GeneratesCorrectSql))];

        // Assert
        Assert.Equal(expectedSql, sql);
        
        if (databaseType == DatabaseType.PostgreSQL)
        {
            Assert.Empty(parameters);            
        }
        else
        {
            Assert.Single(parameters);
            Assert.Equal(1, parameters[expectedParameters[0]]);
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task CaseBoolExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.CaseBoolExpression();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(CaseBoolExpression_GeneratesCorrectSql))];

        // Assert
        Assert.Equal(expectedSql, sql);
        
        if (databaseType == DatabaseType.PostgreSQL)
        {
            Assert.Single(parameters);
            Assert.Equal(18, parameters[expectedParameters[0]]);
        }
        else
        {
            Assert.Equal(2, parameters.Count);
            Assert.Equal(18, parameters[expectedParameters[0]]);
            Assert.Equal(0, parameters[expectedParameters[1]]);
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task CaseDateTimeExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.CaseDateTimeExpression();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(CaseDateTimeExpression_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Equal(5, parameters.Count);
        Assert.Equal(new DateTime(2020, 1, 1), parameters[expectedParameters[0]]);
        Assert.Equal("Old", parameters[expectedParameters[1]]);
        Assert.Equal(new DateTime(2024, 1, 1), parameters[expectedParameters[2]]);
        Assert.Equal("Recent", parameters[expectedParameters[3]]);
        Assert.Equal("New", parameters[expectedParameters[4]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task CaseDecimalExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.CaseDecimalExpression();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(CaseDecimalExpression_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Equal(5, parameters.Count);
        Assert.Equal(1000m, parameters[expectedParameters[0]]);
        Assert.Equal("Expensive", parameters[expectedParameters[1]]);
        Assert.Equal(100m, parameters[expectedParameters[2]]);
        Assert.Equal("Moderate", parameters[expectedParameters[3]]);
        Assert.Equal("Cheap", parameters[expectedParameters[4]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task CaseGuidExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.CaseGuidExpression();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(CaseGuidExpression_GeneratesCorrectSql))];
        
        Assert.Equal(expectedSql, sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(Guid.Empty, parameters[expectedParameters[0]]);
        Assert.Equal("Empty", parameters[expectedParameters[1]]);
        Assert.Equal("HasId", parameters[expectedParameters[2]]);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task CaseIntExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.CaseIntExpression();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(CaseIntExpression_GeneratesCorrectSql))];
        
        Assert.Equal(expectedSql, sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(65, parameters[expectedParameters[0]]);
        Assert.Equal(1, parameters[expectedParameters[1]]);
        Assert.Equal(0, parameters[expectedParameters[2]]);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task CaseInWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.CaseInWhere();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(CaseInWhere_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Equal(18, parameters[expectedParameters[0]]);
        Assert.Equal("Adult", parameters[expectedParameters[1]]);
        Assert.Equal("Minor", parameters[expectedParameters[2]]);
        Assert.Equal("Adult", parameters[expectedParameters[3]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task CaseStringExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.CaseStringExpression();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(CaseStringExpression_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(18, parameters[expectedParameters[0]]);
        Assert.Equal("Adult", parameters[expectedParameters[1]]);
        Assert.Equal("Minor", parameters[expectedParameters[2]]);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task ComplexJoinWhereGroupByHavingOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var complexJoinExpr = TestQueries.ComplexJoinWhereGroupByHavingOrderBySelect();

        // Act
        var (sql, parameters) = complexJoinExpr.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(ComplexJoinWhereGroupByHavingOrderBySelect_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Equal(4, parameters.Count);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task ComplexLeftJoinWhereGroupByOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var complexLeftJoinExpr = TestQueries.ComplexLeftJoinWhereGroupByOrderBySelect();
        var (sql, parameters) = complexLeftJoinExpr.ToSqlRaw(databaseType);

        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(ComplexLeftJoinWhereGroupByOrderBySelect_GeneratesCorrectSql))];

        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(expectedParameters[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task CountActiveCustomers_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.CountActiveCustomers().ToSqlRaw(databaseType);
        var testCase = QueryTestCases.TestCases[(databaseType, nameof(CountActiveCustomers_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task CountCustomers_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.CountCustomers().ToSqlRaw(databaseType);
        var testCase = QueryTestCases.TestCases[(databaseType, nameof(CountCustomers_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DateTimeAddDays_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.DateTimeAddDays().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(DateTimeAddDays_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        
        if (databaseType == DatabaseType.SqlServer)
        {
            Assert.Single(parameters);
            Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        }
        else // SQLite and PostgreSQL inline the constant
        {
            Assert.Empty(parameters);
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DateTimeAddMonths_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.DateTimeAddMonths().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(DateTimeAddMonths_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        
        if (databaseType == DatabaseType.SqlServer)
        {
            Assert.Single(parameters);
            Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        }
        else // SQLite and PostgreSQL inline the constant
        {
            Assert.Empty(parameters);
        }

        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DateTimeAddYears_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.DateTimeAddYears().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(DateTimeAddYears_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        
        if (databaseType == DatabaseType.SqlServer)
        {
            Assert.Single(parameters);
            Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        }
        else // SQLite and PostgreSQL inline the constant
        {
            Assert.Empty(parameters);
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DateTimeDay_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.DateTimeDay().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(DateTimeDay_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DateTimeDiffDays_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.DateTimeDiffDays().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(DateTimeDiffDays_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DateTimeDiffMonths_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.DateTimeDiffMonths().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(DateTimeDiffMonths_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DateTimeDiffYears_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.DateTimeDiffYears().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(DateTimeDiffYears_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DateTimeFunctionsInSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DateTimeFunctionsInSelect();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(DateTimeFunctionsInSelect_GeneratesCorrectSql))];
        Assert.Equal(expectedSql.Trim(), sql.Trim());
        Assert.Equal(expectedParameters.Length, parameters.Count);
        for (int i = 0; i < expectedParameters.Length; i++)
        {
            Assert.True(parameters.ContainsKey(expectedParameters[i]));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DateTimeFunctionsInWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.DateTimeFunctionsInWhere();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(DateTimeFunctionsInWhere_GeneratesCorrectSql))];
        Assert.Equal(expectedSql.Trim(), sql.Trim());
        Assert.Equal(expectedParameters.Length, parameters.Count);
        for (int i = 0; i < expectedParameters.Length; i++)
        {
            Assert.True(parameters.ContainsKey(expectedParameters[i]));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DateTimeMonth_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.DateTimeMonth().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(DateTimeMonth_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;            
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DateTimeNow_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.DateTimeNow().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(DateTimeNow_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DateTimeYear_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.DateTimeYear().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(DateTimeYear_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DecimalCeiling_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.DecimalCeiling().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(DecimalCeiling_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DecimalFloor_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.DecimalFloor().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(DecimalFloor_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DecimalRound_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.DecimalRound().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(DecimalRound_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromGroupByAvgSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var groupByAvgExpr = TestQueries.FromGroupByAvgSelect();
        var (sql, parameters) = groupByAvgExpr.ToSqlRaw(databaseType);

        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(FromGroupByAvgSelect_GeneratesCorrectSql))];

        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromGroupByDecimalAggregatesSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var groupByDecimalAggregatesExpr = TestQueries.FromGroupByDecimalAggregatesSelect();
        var (sql, parameters) = groupByDecimalAggregatesExpr.ToSqlRaw(databaseType);

        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(FromGroupByDecimalAggregatesSelect_GeneratesCorrectSql))];

        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromGroupByDecimalAvgSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var groupByDecimalAvgExpr = TestQueries.FromGroupByDecimalAvgSelect();
        var (sql, parameters) = groupByDecimalAvgExpr.ToSqlRaw(databaseType);

        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(FromGroupByDecimalAvgSelect_GeneratesCorrectSql))];

        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromGroupByDecimalSumSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var groupByDecimalSumExpr = TestQueries.FromGroupByDecimalSumSelect();
        var (sql, parameters) = groupByDecimalSumExpr.ToSqlRaw(databaseType);

        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(FromGroupByDecimalSumSelect_GeneratesCorrectSql))];

        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromGroupByHavingOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var groupByHavingOrderByExpr = TestQueries.FromGroupByHavingOrderBySelect();
        var (sql, parameters) = groupByHavingOrderByExpr.ToSqlRaw(databaseType);

        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(FromGroupByHavingOrderBySelect_GeneratesCorrectSql))];

        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(expectedParameters[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromGroupByHavingSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromGroupByHavingSelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromGroupByHavingSelect_GeneratesCorrectSql))];

        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;                
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromGroupByMinMaxSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var groupByMinMaxExpr = TestQueries.FromGroupByMinMaxSelect();
        var (sql, parameters) = groupByMinMaxExpr.ToSqlRaw(databaseType);

        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(FromGroupByMinMaxSelect_GeneratesCorrectSql))];

        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromGroupByMultipleOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromGroupByMultipleOrderBySelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromGroupByMultipleOrderBySelect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromGroupByMultipleSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromGroupByMultipleSelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromGroupByMultipleSelect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromGroupByOrderByMultipleSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromGroupByOrderByMultipleSelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromGroupByOrderByMultipleSelect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromGroupByOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromGroupByOrderBySelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromGroupByOrderBySelect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromGroupByOrderByThreeKeysSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromGroupByOrderByThreeKeysSelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromGroupByOrderByThreeKeysSelect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromGroupBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromGroupBySelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromGroupBySelect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromOrderByAsc_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromOrderByAsc().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromOrderByAsc_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromOrderByDescendingThenBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromOrderByDescendingThenBy().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromOrderByDescendingThenBy_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromOrderByDesc_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromOrderByDesc().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromOrderByDesc_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromOrderByMultiple_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromOrderByMultiple().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromOrderByMultiple_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromOrderByThenByDescending_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromOrderByThenByDescending().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromOrderByThenByDescending_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromOrderByThenBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromOrderByThenBySelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromOrderByThenBySelect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromOrderByThenBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromOrderByThenBy().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromOrderByThenBy_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromProductWhereSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromProductWhereSelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromProductWhereSelect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromSelectAvg_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromSelectAvg().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromSelectAvg_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromSelectCreatedDateMinMax_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromSelectCreatedDateMinMax().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromSelectCreatedDateMinMax_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromSelectDecimalArithmetic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromSelectDecimalArithmetic().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromSelectDecimalArithmetic_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames.Length, parameters.Count);
        foreach (var paramName in testCase.ParameterNames)
        {
            Assert.True(parameters.ContainsKey(paramName));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromSelectExpression_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromSelectExpression().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromSelectExpression_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames.Length, parameters.Count);
        foreach (var parameterName in testCase.ParameterNames)
        {
            Assert.True(parameters.ContainsKey(parameterName));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromSelectMax_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromSelectMax().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromSelectMax_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromSelectMin_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromSelectMin().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromSelectMin_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromSelectOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromSelectOrderBy().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromSelectOrderBy_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
                
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromSelectSingle_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromSelectSingle().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromSelectSingle_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames.Length, parameters.Count);
        for (int i = 0; i < testCase.ParameterNames.Length; i++)
        {
            Assert.True(parameters.ContainsKey(testCase.ParameterNames[i]));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromSelectSum_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromSelectSum().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromSelectSum_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromSelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromSelect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromStatic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromStatic().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromStatic_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromSubquery_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromSubquery().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromSubquery_GeneratesCorrectSql))];

        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));

        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereAgeGreaterThanAverageAge_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereAgeGreaterThanAverageAge().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereAgeGreaterThanAverageAge_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereAgeGreaterThanSum_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereAgeGreaterThanSum().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereAgeGreaterThanSum_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereAgeInSubqueryWithClosure_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereAgeInSubqueryWithClosure().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereAgeInSubqueryWithClosure_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames.Length, parameters.Count);
        foreach (var parameterName in testCase.ParameterNames)
        {
            Assert.True(parameters.ContainsKey(parameterName));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereAgeInSubquery_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereAgeInSubquery().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereAgeInSubquery_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereAgeIn_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereAgeIn().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereAgeIn_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames.Length, parameters.Count);
        foreach (var parameterName in testCase.ParameterNames)
        {
            Assert.True(parameters.ContainsKey(parameterName));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereAndSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereAndSelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereAndSelect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[1]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereAnd_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereAnd().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereAnd_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[1]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereCreatedDateComparison_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereCreatedDateComparison().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereCreatedDateComparison_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereCreatedDateIsNotNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereCreatedDateIsNotNull().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereCreatedDateIsNotNull_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereCreatedDateIsNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereCreatedDateIsNull().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereCreatedDateIsNull_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereDecimalComparison_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereDecimalComparison().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereDecimalComparison_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereDecimalIsNotNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereDecimalIsNotNull().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereDecimalIsNotNull_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereDecimalIsNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereDecimalIsNull().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereDecimalIsNull_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereFusionThree_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereFusionThree().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereFusionThree_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(3, parameters.Count);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[1]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[2]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereFusionTwo_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereFusionTwo().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereFusionTwo_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[1]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereFusionWithOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereFusionWithOrderBy().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereFusionWithOrderBy_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[1]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereFusionWithSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereFusionWithSelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereFusionWithSelect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[1]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereGroupBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereGroupBySelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereGroupBySelect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereInt_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereInt().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereInt_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereIsNotNullInt_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereIsNotNullInt().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereIsNotNullInt_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereIsNotNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereIsNotNull().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereIsNotNull_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereIsNullCombined_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereIsNullCombined().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereIsNullCombined_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereIsNullInt_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereIsNullInt().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereIsNullInt_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereIsNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereIsNull().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereIsNull_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereMultiple_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereMultiple().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereMultiple_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames.Length, parameters.Count);
        for (int i = 0; i < testCase.ParameterNames.Length; i++)
        {
            Assert.True(parameters.ContainsKey(testCase.ParameterNames[i]));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereOrderBySelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereOrderBySelect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames.Length, parameters.Count);
        foreach (var paramName in testCase.ParameterNames)
        {
            Assert.True(parameters.ContainsKey(paramName));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereOrderByThenBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereOrderByThenBy().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereOrderByThenBy_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereOrderBy().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereOrderBy_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames.Length, parameters.Count);
        foreach (var paramName in testCase.ParameterNames)
        {
            Assert.True(parameters.ContainsKey(paramName));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereOr_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereOr().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereOr_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(3, parameters.Count);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[1]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[2]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereSelectNamed_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereSelectNamed().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereSelectNamed_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[1]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereSelectOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereSelectOrderBy().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereSelectOrderBy_GeneratesCorrectSql))];

        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(3, parameters.Count);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[1]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[2]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereSelectParameterized_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereSelectParameterized(25, 30).ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereSelectParameterized_GeneratesCorrectSql))];

        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[1]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereSelectWhereFromNested_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereSelectWhereFromNested().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereSelectWhereFromNested_GeneratesCorrectSql))];

        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[1]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereSelectWhereNested_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereSelectWhereNested().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereSelectWhereNested_GeneratesCorrectSql))];

        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames.Length, parameters.Count);
        foreach (var paramName in testCase.ParameterNames)
        {
            Assert.True(parameters.ContainsKey(paramName));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereSelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereSelect_GeneratesCorrectSql))];

        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereString_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.FromWhereString().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereString_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        
        // Validate parameters exist if expected
        foreach (var expectedParam in testCase.ParameterNames)
        {
            Assert.True(parameters.ContainsKey(expectedParam), $"Expected parameter {expectedParam} not found");
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereUniqueIdEquals_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereUniqueIdEquals();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var testCase = TestCases[(databaseType, nameof(FromWhereUniqueIdEquals_GeneratesCorrectSql))];
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereUniqueIdIsNotNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereUniqueIdIsNotNull();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var testCase = TestCases[(databaseType, nameof(FromWhereUniqueIdIsNotNull_GeneratesCorrectSql))];
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereUniqueIdIsNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereUniqueIdIsNull();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var testCase = TestCases[(databaseType, nameof(FromWhereUniqueIdIsNull_GeneratesCorrectSql))];
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereUniqueIdNotEquals_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.FromWhereUniqueIdNotEquals();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var testCase = TestCases[(databaseType, nameof(FromWhereUniqueIdNotEquals_GeneratesCorrectSql))];
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }


    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task InnerJoinBasic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.InnerJoinBasic().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(InnerJoinBasic_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task InnerJoinWithGroupBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.InnerJoinWithGroupBy().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(InnerJoinWithGroupBy_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task InnerJoinWithOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.InnerJoinWithOrderBy().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(InnerJoinWithOrderBy_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task InnerJoinWithSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.InnerJoinWithSelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(InnerJoinWithSelect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task InnerJoinWithWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.InnerJoinWithWhere().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(InnerJoinWithWhere_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[1]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task JoinFusionWithWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.JoinFusionWithWhere();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(JoinFusionWithWhere_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        foreach (var paramName in expectedParameters)
        {
            Assert.True(parameters.ContainsKey(paramName));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task LeftJoinBasic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.LeftJoinBasic().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(LeftJoinBasic_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;        
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task LeftJoinWithAggregates_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.LeftJoinWithAggregates();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(LeftJoinWithAggregates_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task LeftJoinWithOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.LeftJoinWithOrderBy();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(LeftJoinWithOrderBy_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task LeftJoinWithSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.LeftJoinWithSelect();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(LeftJoinWithSelect_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        foreach (var paramName in expectedParameters)
        {
            Assert.True(parameters.ContainsKey(paramName));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task LeftJoinWithWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.LeftJoinWithWhere();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(LeftJoinWithWhere_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        foreach (var paramName in expectedParameters)
        {
            Assert.True(parameters.ContainsKey(paramName));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task LikeBothWildcards_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.LikeBothWildcards().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(LikeBothWildcards_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        foreach (var paramName in testCase.ParameterNames)
        {
            Assert.True(parameters.ContainsKey(paramName));
        }
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task LikeExact_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.LikeExact().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(LikeExact_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        foreach (var paramName in testCase.ParameterNames)
        {
            Assert.True(parameters.ContainsKey(paramName));
        }
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task LikeSingleChar_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.LikeSingleChar().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(LikeSingleChar_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        foreach (var paramName in testCase.ParameterNames)
        {
            Assert.True(parameters.ContainsKey(paramName));
        }
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task LikeWildcard_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.LikeWildcard().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(LikeWildcard_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.Equal("Jo%", parameters[testCase.ParameterNames[0]]);        
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));

        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task MathFunctionsInSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.MathFunctionsInSelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(MathFunctionsInSelect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task MathFunctionsInWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.MathFunctionsInWhere().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(MathFunctionsInWhere_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(3, parameters.Count);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0])); // @p0/:p0
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[1])); // @p1/:p1
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[2])); // @p2/:p2
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task MaxPrice_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var maxExpr = TestQueries.MaxPrice();

        // Act
        var (sql, parameters) = maxExpr.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(MaxPrice_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Empty(expectedParameters);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;        
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task MinPrice_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var minExpr = TestQueries.MinPrice();

        // Act
        var (sql, parameters) = minExpr.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(MinPrice_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Empty(expectedParameters);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;        
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task MixedJoinTypesFusion_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.MixedJoinTypesFusion().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(MixedJoinTypesFusion_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task MultipleInnerJoinsFusion_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.MultipleInnerJoinsFusion();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(MultipleInnerJoinsFusion_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SQLite)]
    public Task ParameterAsBoolParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // NOTE: SQL Server and PostgreSQL excluded due to boolean expression comparison complexity
        // - SQL Server: Doesn't support boolean comparison syntax like "(expression) = @parameter" 
        // - PostgreSQL: Complex boolean expression handling requirements
        // This test validates SQLite's boolean parameter comparison capability only
        
        // Arrange
        var query = TestQueries.ParameterAsBoolParam();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(ParameterAsBoolParam_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        foreach (var paramName in expectedParameters)
        {
            Assert.True(parameters.ContainsKey(paramName));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task ParameterAsDateTimeParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.ParameterAsDateTimeParam();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(ParameterAsDateTimeParam_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        foreach (var paramName in expectedParameters)
        {
            Assert.True(parameters.ContainsKey(paramName));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task ParameterAsDecimalParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.ParameterAsDecimalParam();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(ParameterAsDecimalParam_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        foreach (var paramName in expectedParameters)
        {
            Assert.True(parameters.ContainsKey(paramName));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task ParameterAsGuidParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.ParameterAsGuidParam();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(ParameterAsGuidParam_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        foreach (var paramName in expectedParameters)
        {
            Assert.True(parameters.ContainsKey(paramName));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task ParameterAsIntParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.ParameterAsIntParam();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(ParameterAsIntParam_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        foreach (var paramName in expectedParameters)
        {
            Assert.True(parameters.ContainsKey(paramName));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task ParameterAsStringParam_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var query = TestQueries.ParameterAsStringParam();

        // Act
        var (sql, parameters) = query.ToSqlRaw(databaseType);

        // Assert
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(ParameterAsStringParam_GeneratesCorrectSql))];
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        foreach (var paramName in expectedParameters)
        {
            Assert.True(parameters.ContainsKey(paramName));
        }
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task StringFunctionsInSelect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.StringFunctionsInSelect().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(StringFunctionsInSelect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(2, parameters.Count);        
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task StringFunctionsInWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.StringFunctionsInWhere().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(StringFunctionsInWhere_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[1]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task StringLength_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.StringLength().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(StringLength_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task StringLower_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.StringLower().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(StringLower_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task StringSubstring_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.StringSubstring().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(StringSubstring_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[1]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task StringTrim_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.StringTrim().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(StringTrim_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task StringUpper_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.StringUpper().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(StringUpper_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }


    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task SumAges_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.SumAges().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(SumAges_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task SumExpensivePrices_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.SumExpensivePrices().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(SumExpensivePrices_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Single(parameters);
        Assert.True(parameters.ContainsKey(testCase.ParameterNames[0]));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task SumPrices_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var (sql, parameters) = TestQueries.SumPrices().ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(SumPrices_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Empty(parameters);
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromLimitOffset_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.FromLimitOffset();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromLimitOffset_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromSelectLimitOffset_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.FromSelectLimitOffset();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromSelectLimitOffset_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereLimitOffset_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.FromWhereLimitOffset();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereLimitOffset_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        Assert.Equal(18, parameters[databaseType == DatabaseType.SqlServer ? "@p0" : ":p0"]); // WHERE Age > 18
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereSelectLimitOffset_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.FromWhereSelectLimitOffset();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereSelectLimitOffset_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        Assert.Equal(21, parameters[databaseType == DatabaseType.SqlServer ? "@p0" : ":p0"]); // WHERE Age >= 21
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromOrderByLimitOffset_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.FromOrderByLimitOffset();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromOrderByLimitOffset_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereOrderByLimitOffset_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.FromWhereOrderByLimitOffset();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereOrderByLimitOffset_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        Assert.Equal(18, parameters[databaseType == DatabaseType.SqlServer ? "@p0" : ":p0"]); // WHERE Age > 18
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromWhereOrderBySelectLimitOffset_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.FromWhereOrderBySelectLimitOffset();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromWhereOrderBySelectLimitOffset_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        Assert.Equal("", parameters[databaseType == DatabaseType.SqlServer ? "@p0" : ":p0"]); // WHERE Name != ""
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromLimitOffsetOnly_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.FromLimitOffsetOnly();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromLimitOffsetOnly_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromOffsetOnly_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.FromOffsetOnly();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromOffsetOnly_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        
        return Task.CompletedTask;
    }

    // Special test for LIMIT without ORDER BY - only PostgreSQL and SQLite support this
    [Theory]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task FromLimitOffsetWithoutOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.FromLimitOffsetWithoutOrderBy();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromLimitOffsetWithoutOrderBy_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        
        return Task.CompletedTask;
    }

    // DISTINCT Tests
    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public Task FromSelectDistinct_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.FromSelectDistinct();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromSelectDistinct_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public Task FromSelectDistinctWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.FromSelectDistinctWhere();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromSelectDistinctWhere_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public Task FromSelectDistinctOrderBy_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.FromSelectDistinctOrderBy();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromSelectDistinctOrderBy_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public Task FromSelectDistinctMultipleColumns_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.FromSelectDistinctMultipleColumns();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(FromSelectDistinctMultipleColumns_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public Task Union_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.Union();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(Union_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public Task Intersect_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.Intersect();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(Intersect_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.SQLite)]
    public Task Except_GeneratesCorrectSql(DatabaseType databaseType)
    {
        var query = TestQueries.Except();
        var (sql, parameters) = query.ToSqlRaw(databaseType);
        var testCase = TestCases[(databaseType, nameof(Except_GeneratesCorrectSql))];
        
        Assert.Equal(testCase.Sql, sql);
        Assert.Equal(testCase.ParameterNames, parameters.Select(p => p.Key).OrderBy(x => x));
        
        return Task.CompletedTask;
    }
}
