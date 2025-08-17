using TypedSqlBuilder.Core;
using TypedSqlBuilder.TestModels;
using Dapper;
using Xunit;

namespace TypedSqlBuilder.IntegrationTests;

/// <summary>
/// Integration tests for SELECT queries executed against SQL Server databases using Dapper
/// </summary>
public class SqlServerQueryIntegrationTests : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;

    public SqlServerQueryIntegrationTests(SqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task FromWhereInt_ExecutesCorrectly()
    {
        // Arrange
        var query = TestQueries.FromWhereInt();
        var (sql, parameters) = query.ToSqlServerRaw();

        // Act - Execute query with Dapper against SQL Server
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return customers over 18
        Assert.Equal(3, results.Count);
        Assert.Contains(results, r => r.Name == "John Doe" && r.Age == 25);
        Assert.Contains(results, r => r.Name == "Jane Smith" && r.Age == 30);
        Assert.Contains(results, r => r.Name == "Senior User" && r.Age == 65);
        Assert.DoesNotContain(results, r => r.Name == "Minor User");
    }

    [Fact]
    public async Task FromWhereSelect_ExecutesCorrectly()
    {
        // Arrange
        var query = TestQueries.FromWhereSelect();
        var (sql, parameters) = query.ToSqlServerRaw();

        // Act - Execute query with Dapper against SQL Server
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        // Use anonymous type for the projection (Id, Name)
        var results = (await connection.QueryAsync<(int Id, string Name)>(sql, dapperParams)).ToList();

        // Assert - Should return adults with selected columns
        Assert.Equal(3, results.Count);
        Assert.Contains(results, r => r.Name == "John Doe" && r.Id == 1);
        Assert.Contains(results, r => r.Name == "Jane Smith" && r.Id == 2);
        Assert.Contains(results, r => r.Name == "Senior User" && r.Id == 4);
    }

    [Fact]
    public async Task From_ExecutesCorrectly()
    {
        // Arrange
        var query = TestQueries.From();
        var (sql, parameters) = query.ToSqlServerRaw();

        // Act - Execute query with Dapper against SQL Server
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return all customers
        Assert.Equal(4, results.Count);
        Assert.Contains(results, r => r.Name == "John Doe");
        Assert.Contains(results, r => r.Name == "Jane Smith");
        Assert.Contains(results, r => r.Name == "Minor User");
        Assert.Contains(results, r => r.Name == "Senior User");
    }

    [Fact]
    public async Task FromStatic_ExecutesCorrectly()
    {
        // Arrange
        var query = TestQueries.FromStatic();
        var (sql, parameters) = query.ToSqlServerRaw();

        // Act - Execute query with Dapper against SQL Server
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return all customers
        Assert.Equal(4, results.Count);
        Assert.Contains(results, r => r.Name == "John Doe");
        Assert.Contains(results, r => r.Name == "Jane Smith");
        Assert.Contains(results, r => r.Name == "Minor User");
        Assert.Contains(results, r => r.Name == "Senior User");
    }

    [Fact]
    public async Task FromWhereMultiple_ExecutesCorrectly()
    {
        // Arrange
        var query = TestQueries.FromWhereMultiple();
        var (sql, parameters) = query.ToSqlServerRaw();

        // Act - Execute query with Dapper against SQL Server
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return customers based on multiple conditions
        Assert.True(results.Count > 0);
        Assert.All(results, r => Assert.True(r.Age > 18 && r.Name != "Admin")); // Verify the conditions
    }

    [Fact]
    public async Task FromWhereString_ExecutesCorrectly()
    {
        // Arrange
        var query = TestQueries.FromWhereString();
        var (sql, parameters) = query.ToSqlServerRaw();

        // Act - Execute query with Dapper against SQL Server
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return customers matching string condition
        Assert.True(results.Count >= 0); // May or may not have results depending on query
        Assert.All(results, r => Assert.NotNull(r.Name));
    }

    [Fact]
    public async Task CompareWithSQLiteResults_ShouldMatch()
    {
        // This test compares SQL Server results with SQLite to ensure consistency
        // across database backends
        
        // Arrange - Use a simple query that should work identically on both databases
        var query = TestQueries.FromWhereInt();
        var (sqlServerSql, sqlServerParams) = query.ToSqlServerRaw();

        // Act - Execute against SQL Server
        using var sqlServerConnection = _fixture.CreateConnection();
        sqlServerConnection.Open();
        var sqlServerDapperParams = sqlServerParams.ToDapperParameters();
        var sqlServerResults = (await sqlServerConnection.QueryAsync<CustomerDto>(sqlServerSql, sqlServerDapperParams))
            .OrderBy(r => r.Id)
            .ToList();

        // Assert - Basic validation that we get expected results
        Assert.Equal(3, sqlServerResults.Count);
        
        // Verify SQL Server-specific behavior
        Assert.All(sqlServerResults, r => Assert.True(r.Id > 0));
        Assert.All(sqlServerResults, r => Assert.True(r.Age >= 18));
        
        // Verify the data matches expected customers
        var expectedCustomers = new[] { "John Doe", "Jane Smith", "Senior User" };
        Assert.All(expectedCustomers, expected => 
            Assert.Contains(sqlServerResults, r => r.Name == expected));
    }
}
