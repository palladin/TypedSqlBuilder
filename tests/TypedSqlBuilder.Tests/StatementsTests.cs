using System.Collections.Immutable;
using TypedSqlBuilder.Core;
using TypedSqlBuilder.TestModels;

namespace TypedSqlBuilder.Tests;

/// <summary>
/// SQL Server-specific tests for INSERT, UPDATE, and DELETE statements using TestStatements
/// </summary>
public class StatementsTests : IStatementTestContract
{
    private static readonly Dictionary<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)> TestCases = StatementsTestCases.TestCases;

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task InsertBasic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.InsertBasic();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(InsertBasic_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(200, parameters[expectedParameters[0]]);
        Assert.Equal(25, parameters[expectedParameters[1]]);
        Assert.Equal("John Doe", parameters[expectedParameters[2]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DeleteAll_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.DeleteAll();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(DeleteAll_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DeleteBasic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.DeleteBasic();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(DeleteBasic_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(200, parameters[expectedParameters[0]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task DeleteConditional_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.DeleteConditional();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(DeleteConditional_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(18, parameters[expectedParameters[0]]);
        Assert.Equal("Temp", parameters[expectedParameters[1]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task InsertWithNewColumnsNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.InsertWithNewColumnsNull();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(InsertWithNewColumnsNull_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count); // ProductId and ProductName are parameterized
        Assert.Equal(201, parameters[expectedParameters[0]]);
        Assert.Equal("Null Test", parameters[expectedParameters[1]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task InsertWithNewColumns_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.InsertWithNewColumns();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(InsertWithNewColumns_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Equal(5, parameters.Count);
        Assert.Equal(200, parameters[expectedParameters[0]]);
        Assert.Equal("Test Product", parameters[expectedParameters[1]]);
        Assert.Equal(99.99m, parameters[expectedParameters[2]]);
        Assert.Equal(new DateTime(2024, 8, 18), parameters[expectedParameters[3]]);
        Assert.Equal(Guid.Parse("12345678-1234-1234-1234-123456789012"), parameters[expectedParameters[4]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task InsertWithNullInt_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.InsertWithNullInt();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(InsertWithNullInt_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(203, parameters[expectedParameters[0]]);
        Assert.Equal("John", parameters[expectedParameters[1]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task InsertWithNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.InsertWithNull();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(InsertWithNull_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(202, parameters[expectedParameters[0]]);
        Assert.Equal(25, parameters[expectedParameters[1]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task UpdateBasic_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.UpdateBasic();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(UpdateBasic_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(26, parameters[expectedParameters[0]]);
        Assert.Equal(200, parameters[expectedParameters[1]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task UpdateConditional_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.UpdateConditional();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(UpdateConditional_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(1, parameters[expectedParameters[0]]);
        Assert.Equal(18, parameters[expectedParameters[1]]);
        Assert.Equal("Admin", parameters[expectedParameters[2]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task UpdateMultiple_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.UpdateMultiple();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(UpdateMultiple_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(27, parameters[expectedParameters[0]]);
        Assert.Equal("John Smith", parameters[expectedParameters[1]]);
        Assert.Equal(200, parameters[expectedParameters[2]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task UpdateSetNewColumnsNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.UpdateSetNewColumnsNull();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(UpdateSetNewColumnsNull_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(101, parameters[expectedParameters[0]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task UpdateSetNullInt_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.UpdateSetNullInt();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(UpdateSetNullInt_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task UpdateSetNullMixed_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.UpdateSetNullMixed();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(UpdateSetNullMixed_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal("John", parameters[expectedParameters[0]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task UpdateSetNullWhere_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.UpdateSetNullWhere();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(UpdateSetNullWhere_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Single(parameters);
        Assert.Equal(200, parameters[expectedParameters[0]]);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task UpdateSetNull_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.UpdateSetNull();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(UpdateSetNull_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Empty(parameters);
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.SQLite)]
    [InlineData(DatabaseType.PostgreSQL)]
    public Task UpdateWithNewColumns_GeneratesCorrectSql(DatabaseType databaseType)
    {
        // Arrange
        var statement = TestStatements.UpdateWithNewColumns();
        var (expectedSql, expectedParameters) = TestCases[(databaseType, nameof(UpdateWithNewColumns_GeneratesCorrectSql))];
        
        // Act
        var (sql, parameters) = statement.ToSqlRaw(databaseType);

        // Assert        
        Assert.Equal(expectedSql, sql);
        Assert.Equal(4, parameters.Count);
        Assert.Equal(119.99m, parameters[expectedParameters[0]]);
        Assert.Equal(new DateTime(2024, 12, 25), parameters[expectedParameters[1]]);
        Assert.Equal(Guid.Parse("87654321-4321-4321-4321-210987654321"), parameters[expectedParameters[2]]);
        Assert.Equal(100, parameters[expectedParameters[3]]);
        return Task.CompletedTask;
    }
}
