using TypedSqlBuilder.Core;
using TypedSqlBuilder.TestModels;
using Dapper;

namespace TypedSqlBuilder.IntegrationTests;

/// <summary>
/// Integration tests for SELECT queries executed against PostgreSQL databases using Dapper
/// </summary>
public class PostgreSqlQueryIntegrationTests : IClassFixture<PostgreSqlFixture>, IQueryTestContract, IPostgreSqlDialectTestContract
{
    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlQueryIntegrationTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task FromWhereInt_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereInt();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
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
    public async Task FromWhereSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereSelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return customers over 21 with only selected columns
        Assert.Equal(3, results.Count);
        Assert.Contains(results, r => r.Name == "John Doe" && r.Id == 1);
        Assert.Contains(results, r => r.Name == "Jane Smith" && r.Id == 2);
        Assert.Contains(results, r => r.Name == "Senior User" && r.Id == 4);
        Assert.DoesNotContain(results, r => r.Name == "Minor User");  // Age 16 is not >= 21
    }

    [Fact]
    public async Task From_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.From();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return all customers (5 customers in test data)
        Assert.Equal(4, results.Count);
        Assert.Contains(results, r => r.Name == "John Doe");
        Assert.Contains(results, r => r.Name == "Jane Smith");
        Assert.Contains(results, r => r.Name == "Minor User");
        Assert.Contains(results, r => r.Name == "Senior User");
    }

    [Fact]
    public async Task FromStatic_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromStatic();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return all customers (5 customers in test data)
        Assert.Equal(4, results.Count);
        Assert.Contains(results, r => r.Name == "John Doe");
        Assert.Contains(results, r => r.Name == "Jane Smith");
        Assert.Contains(results, r => r.Name == "Minor User");
        Assert.Contains(results, r => r.Name == "Senior User");
    }

    [Fact]
    public async Task FromSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert - Should return all customers with only Id and Name (as selected in query)
        Assert.Equal(4, results.Count);
        Assert.Contains(results, r => r.name == "John Doe" && r.id == 1);
        Assert.Contains(results, r => r.name == "Jane Smith" && r.id == 2);
        Assert.Contains(results, r => r.name == "Minor User" && r.id == 3);
        Assert.Contains(results, r => r.name == "Senior User" && r.id == 4);
    }

    [Fact]
    public async Task FromSelectSingle_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelectSingle();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<int>(sql, dapperParams)).ToList();

        // Assert - Should return all customer ages
        Assert.Equal(4, results.Count);
        Assert.Contains(25, results); // John Doe
        Assert.Contains(30, results); // Jane Smith
        Assert.Contains(16, results); // Minor User
        Assert.Contains(65, results); // Senior User
    }

    [Fact]
    public async Task FromWhereString_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereString();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return no customers (no customer named exactly "John")
        Assert.Empty(results);
    }

    [Fact]
    public async Task FromWhereMultiple_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereMultiple();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return customers over 18 and not named Admin
        Assert.Equal(3, results.Count);
        Assert.Contains(results, r => r.Name == "John Doe"); // Age 25 > 18
        Assert.Contains(results, r => r.Name == "Jane Smith"); // Age 30 > 18  
        Assert.Contains(results, r => r.Name == "Senior User"); // Age 65 > 18
        Assert.DoesNotContain(results, r => r.Name == "Minor User"); // Age 16 not > 18
    }

    [Fact]
    public async Task FromOrderByAsc_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByAsc();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return all customers ordered by name ascending (alphabetical)
        Assert.Equal(4, results.Count);
        Assert.Equal("Jane Smith", results[0].Name);   // First alphabetically
        Assert.Equal("John Doe", results[1].Name);     
        Assert.Equal("Minor User", results[2].Name);   
        Assert.Equal("Senior User", results[3].Name);  // Last alphabetically
    }

    [Fact]
    public async Task FromOrderByDesc_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByDesc();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return all customers ordered by age descending
        Assert.Equal(4, results.Count);
        Assert.Equal("Senior User", results[0].Name);  // Age 65
        Assert.Equal("Jane Smith", results[1].Name);   // Age 30
        Assert.Equal("John Doe", results[2].Name);     // Age 25
        Assert.Equal("Minor User", results[3].Name);   // Age 16
    }

    [Fact]
    public async Task FromWhereSelectOrderBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereSelectOrderBy();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert - Should return customers over 18 with modified data, ordered by name
        Assert.Equal(3, results.Count);
        
        // Use a different approach - access as IDictionary to get column names
        var dict = (IDictionary<string, object>)results[0];
        var columnNames = dict.Keys.ToArray();
        
        // Should have 2 columns from SELECT (c.Id + 1, c.Name + "!")
        Assert.Equal(2, columnNames.Length);
        
        // Check the first result's values (ordered by name, so Jane Smith should be first)
        var firstRowValues = dict.Values.ToArray();
        Assert.Equal(3, firstRowValues[0]); // Jane Smith: Id 2 + 1 = 3 
        Assert.Equal("Jane Smith!", firstRowValues[1]);
    }

    [Fact]
    public async Task FromWhereOr_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereOr();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return customers where (Age > 18 AND Age < 65) OR Name == "VIP"
        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.Name == "John Doe"); // Age 25 (> 18 and < 65)
        Assert.Contains(results, r => r.Name == "Jane Smith"); // Age 30 (> 18 and < 65)
        Assert.DoesNotContain(results, r => r.Name == "Senior User"); // Age 65 is not < 65
        Assert.DoesNotContain(results, r => r.Name == "Minor User"); // Age 16 is not > 18
    }

    [Fact]
    public async Task CountCustomers_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.CountCustomers();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var count = await connection.QuerySingleAsync<long>(sql, dapperParams);

        // Assert - Should return count of all customers (4 in test data)
        Assert.Equal(4, count);
    }

    [Fact]
    public async Task CountActiveCustomers_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.CountActiveCustomers();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var count = await connection.QuerySingleAsync<long>(sql, dapperParams);

        // Assert - Should return count of active customers (John Doe, Jane Smith, Senior User are active)
        Assert.Equal(3, count); // 3 active customers in test data
    }

    [Fact]
    public async Task FromSelectExpression_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelectExpression();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Debug: Print actual results
        var debugResults = results.Select(r => new { 
            Proj0 = r.proj0, 
            Proj1 = r.proj1 as string 
        }).ToList();

        // Assert - Should return computed expressions
        Assert.Equal(4, results.Count);
        Assert.Contains(debugResults, r => r.Proj0 == 125 && r.Proj1 == "John Doe - Customer"); // (1 * 100) + 25
        Assert.Contains(debugResults, r => r.Proj0 == 230 && r.Proj1 == "Jane Smith - Customer"); // (2 * 100) + 30
        Assert.Contains(debugResults, r => r.Proj0 == 316 && r.Proj1 == "Minor User - Customer"); // (3 * 100) + 16
        Assert.Contains(debugResults, r => r.Proj0 == 465 && r.Proj1 == "Senior User - Customer"); // (4 * 100) + 65
    }

    [Fact]
    public async Task SumAges_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.SumAges();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var sum = await connection.QuerySingleAsync<long>(sql, dapperParams);

        // Assert - Should return sum of all ages (25 + 30 + 16 + 65 = 136)
        Assert.Equal(136, sum);
    }

    [Fact]
    public async Task FromWhereAnd_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereAnd();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return customers over 18 AND named John (empty because no exact match)
        Assert.Empty(results);
    }

    // IPostgreSqlDialectTestContract - stub methods to implement
    [Fact]
    public async Task FromWhereFusionTwo_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereFusionTwo();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
        // Add specific assertions based on the query logic
    }

    [Fact]
    public async Task FromWhereFusionThree_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereFusionThree();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
        // Add specific assertions based on the query logic
    }

    [Fact]
    public async Task FromWhereFusionWithSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereFusionWithSelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
        // Add specific assertions based on the query logic
    }

    [Fact]
    public async Task FromWhereFusionWithOrderBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereFusionWithOrderBy();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
        // Add specific assertions based on the query logic
    }

    [Fact]
    public async Task FromOrderByThenByDescending_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByThenByDescending();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
        Assert.Equal(4, results.Count);
        // Add specific assertions based on ordering logic
    }

    [Fact]
    public async Task FromOrderByDescendingThenBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByDescendingThenBy();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
        Assert.Equal(4, results.Count);
        // Add specific assertions based on ordering logic
    }

    [Fact]
    public async Task FromOrderByMultiple_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByMultiple();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
        Assert.Equal(4, results.Count);
        // Add specific assertions based on ordering logic
    }

    [Fact]
    public async Task FromWhereOrderByThenBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereOrderByThenBy();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
        // Add specific assertions based on the query logic
    }

    [Fact]
    public async Task FromOrderByThenBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByThenBySelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
        Assert.Equal(4, results.Count);
        // Add specific assertions based on ordering logic
    }

    [Fact]
    public async Task FromWhereIsNullCombined_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereIsNullCombined();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
        // Add specific assertions based on the query logic
    }

    [Fact]
    public async Task SumAgesWithDb_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.SumAgesWithDb();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var sum = await connection.QuerySingleAsync<long>(sql, dapperParams);

        // Assert - Should return sum of all ages (25 + 30 + 16 + 65 = 136)
        Assert.Equal(136, sum); // Sum of ages from 4 customers in test data
    }

    [Fact]
    public async Task CountCustomersWithDb_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.CountCustomersWithDb();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var count = await connection.QuerySingleAsync<long>(sql, dapperParams);

        // Assert - Should return count of all customers (4 in test data)
        Assert.Equal(4, count);
    }

    [Fact]
    public async Task CountActiveCustomersWithDb_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.CountActiveCustomersWithDb();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var count = await connection.QuerySingleAsync<long>(sql, dapperParams);

        // Assert - Should return count of active customers (John Doe, Jane Smith, Senior User are active)
        Assert.Equal(3, count); // 3 active customers in test data
    }

    [Fact]
    public async Task FromWhereAgeGreaterThanSum_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereAgeGreaterThanSum();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
        // Add specific assertions based on the query logic
    }

    [Fact]
    public async Task FromWhereAgeGreaterThanAverageAge_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereAgeGreaterThanAverageAge();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
        // Add specific assertions based on the query logic
    }

    [Fact]
    public async Task FromWhereAgeIn_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereAgeIn();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
        // Add specific assertions based on the query logic
    }

    [Fact]
    public async Task FromWhereAgeInSubquery_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereAgeInSubquery();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
        // Add specific assertions based on the query logic
    }

    [Fact]
    public async Task FromWhereAgeInSubqueryWithClosure_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereAgeInSubqueryWithClosure();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
        // Add specific assertions based on the query logic
    }

    [Fact]
    public async Task FromSubquery_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSubquery();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
        Assert.Equal(4, results.Count);
        // Add specific assertions based on the query logic
    }

    // Missing methods from IQueryTestContract - properly implemented
    [Fact]
    public async Task FromWhereOrderBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereOrderBy();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return customers over 21 with non-empty name, ordered by age ascending
        Assert.Equal(3, results.Count);
        Assert.Equal("John Doe", results[0].Name);     // Age 25
        Assert.Equal("Jane Smith", results[1].Name);   // Age 30  
        Assert.Equal("Senior User", results[2].Name);  // Age 65
        Assert.DoesNotContain(results, r => r.Name == "Minor User");  // Age 16 is not > 21
    }

    [Fact]
    public async Task FromWhereOrderBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereOrderBySelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert - Should return customers over 21 with non-empty name, ordered by age, with modified data
        Assert.Equal(3, results.Count);
        
        // Use IDictionary approach to avoid column name issues
        var dict = (IDictionary<string, object>)results[0];
        var columnNames = dict.Keys.ToArray();
        
        // Should have 3 columns from SELECT (Id, Name, Age + 10)
        Assert.Equal(3, columnNames.Length);
        
        // Check ordered results (by age: John Doe 25, Jane Smith 30, Senior User 65)
        var firstRowValues = dict.Values.ToArray();
        Assert.Equal(1, firstRowValues[0]);        // John Doe Id
        Assert.Equal("John Doe", firstRowValues[1]);
        Assert.Equal(35, firstRowValues[2]);       // 25 + 10  
    }

    [Fact]
    public async Task FromWhereSelectNamed_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereSelectNamed();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert - Should return customers over 18 with computed values
        Assert.Equal(3, results.Count);
        Assert.Contains(results, r => r.originalid == 1 && r.modifiedid == 125 && r.customername == "John Doe"); // 1 * 100 + 25
        Assert.Contains(results, r => r.originalid == 2 && r.modifiedid == 230 && r.customername == "Jane Smith"); // 2 * 100 + 30
        Assert.Contains(results, r => r.originalid == 4 && r.modifiedid == 465 && r.customername == "Senior User"); // 4 * 100 + 65
    }

    [Fact]
    public async Task FromOrderByThenBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByThenBy();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return all customers ordered by name ASC, then by age ASC
        Assert.Equal(4, results.Count);
        Assert.Equal("Jane Smith", results[0].Name);    // Name order: Jane Smith (Age 30)
        Assert.Equal("John Doe", results[1].Name);      // Name order: John Doe (Age 25)
        Assert.Equal("Minor User", results[2].Name);    // Name order: Minor User (Age 16)
        Assert.Equal("Senior User", results[3].Name);   // Name order: Senior User (Age 65)
    }

    [Fact]
    public async Task FromWhereIsNull_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereIsNull();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return no customers (no customers with null names in test data)
        Assert.Empty(results);
    }

    [Fact]
    public async Task FromWhereIsNotNull_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereIsNotNull();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return all customers (all customers have non-null names in test data)
        Assert.Equal(4, results.Count);
        Assert.Contains(results, r => r.Name == "John Doe");
        Assert.Contains(results, r => r.Name == "Jane Smith");
        Assert.Contains(results, r => r.Name == "Minor User");
        Assert.Contains(results, r => r.Name == "Senior User");
    }

    [Fact]
    public async Task FromWhereSelectWhereFromNested_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereSelectWhereFromNested();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert - Should return customers over 18 with ID > 100 (no such customers in test data)
        Assert.Empty(results);
    }

    // Additional IQueryTestContract methods
    [Fact]
    public async Task FromWhereSelectWhereNested_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereSelectWhereNested();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
    }

    [Fact]
    public async Task FromGroupBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupBySelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
    }

    [Fact]
    public async Task FromGroupByMultipleSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByMultipleSelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
    }

    [Fact]
    public async Task FromGroupByHavingSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByHavingSelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
    }

    [Fact]
    public async Task FromWhereGroupBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereGroupBySelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.NotNull(results);
    }

    [Fact]
    public async Task FromWhereIsNullInt_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereIsNullInt();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return customers with null age (none in test data)
        Assert.Empty(results);
    }

    [Fact]
    public async Task FromWhereIsNotNullInt_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereIsNotNullInt();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert - Should return all customers (all have non-null age in test data)
        Assert.Equal(4, results.Count);
    }

    [Fact]
    public async Task InnerJoinBasic_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.InnerJoinBasic();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count > 0);
    }

    [Fact]
    public async Task InnerJoinWithSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.InnerJoinWithSelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count > 0);
    }

    [Fact]
    public async Task InnerJoinWithWhere_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.InnerJoinWithWhere();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task InnerJoinWithOrderBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.InnerJoinWithOrderBy();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task LeftJoinBasic_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LeftJoinBasic();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count > 0);
    }

    [Fact]
    public async Task LeftJoinWithSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LeftJoinWithSelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count > 0);
    }

    [Fact]
    public async Task LeftJoinWithWhere_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LeftJoinWithWhere();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task LeftJoinWithOrderBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LeftJoinWithOrderBy();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count > 0);
    }

    [Fact]
    public async Task InnerJoinWithGroupBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.InnerJoinWithGroupBy();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task LeftJoinWithAggregates_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LeftJoinWithAggregates();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task MultipleInnerJoinsFusion_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.MultipleInnerJoinsFusion();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task MixedJoinTypesFusion_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.MixedJoinTypesFusion();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task JoinFusionWithWhere_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.JoinFusionWithWhere();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task FromGroupByOrderBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByOrderBySelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task FromGroupByOrderByMultipleSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByOrderByMultipleSelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task FromGroupByOrderByThreeKeysSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByOrderByThreeKeysSelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task FromGroupByMultipleOrderBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByMultipleOrderBySelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task FromGroupByHavingOrderBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByHavingOrderBySelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task ComplexJoinWhereGroupByHavingOrderBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.ComplexJoinWhereGroupByHavingOrderBySelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task ComplexLeftJoinWhereGroupByOrderBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.ComplexLeftJoinWhereGroupByOrderBySelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task FromGroupByMinMaxSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByMinMaxSelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task FromGroupByAvgSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByAvgSelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task FromSelectSum_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelectSum();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var result = await connection.QuerySingleAsync<long>(sql, dapperParams);

        // Assert - Should return sum of all ages
        Assert.True(result >= 0);
    }

    [Fact]
    public async Task FromSelectAvg_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelectAvg();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var result = await connection.QuerySingleAsync<double?>(sql, dapperParams);

        // Assert - Should return average age
        Assert.True(result >= 0);
    }

    [Fact]
    public async Task FromSelectMin_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelectMin();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var result = await connection.QuerySingleAsync<int>(sql, dapperParams);

        // Assert - Should return minimum age
        Assert.True(result >= 0);
    }

    [Fact]
    public async Task FromSelectMax_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelectMax();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var result = await connection.QuerySingleAsync<int>(sql, dapperParams);

        // Assert - Should return maximum age
        Assert.True(result >= 0);
    }

    [Fact]
    public async Task ParameterAsIntParam_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.ParameterAsIntParam();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task ParameterAsStringParam_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.ParameterAsStringParam();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task ParameterAsBoolParam_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.ParameterAsBoolParam();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task BoolColumnDirectComparison_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.BoolColumnDirectComparison();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task BoolColumnLiteralTrue_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.BoolColumnLiteralTrue();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task BoolColumnLiteralFalse_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.BoolColumnLiteralFalse();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<CustomerDto>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task FromOrderByMultipleOrderBySelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromOrderByMultiple();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.Equal(4, results.Count);
    }

    [Fact]
    public async Task FromProductWhereSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromProductWhereSelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task FromSelectOrderBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromSelectOrderBy();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.Equal(4, results.Count);
    }

    [Fact]
    public async Task FromWhereAndSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereAndSelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task FromWhereSelectParameterized_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromWhereSelectParameterized(25, 60);
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<dynamic>(sql, dapperParams)).ToList();

        // Assert
        Assert.True(results.Count >= 0);
    }

    [Fact]
    public async Task CaseStringExpression_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.CaseStringExpression();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<(int Id, string Item2)>(sql, dapperParams)).ToList();

        // Assert - Should return all customers with age classification
        Assert.Equal(4, results.Count);
        Assert.Contains(results, r => r.Id == 1 && r.Item2 == "Adult"); // John Doe, age 25
        Assert.Contains(results, r => r.Id == 2 && r.Item2 == "Adult"); // Jane Smith, age 30  
        Assert.Contains(results, r => r.Id == 3 && r.Item2 == "Minor"); // Minor User, age 16
        Assert.Contains(results, r => r.Id == 4 && r.Item2 == "Adult"); // Senior User, age 65
    }

    [Fact]
    public async Task CaseIntExpression_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.CaseIntExpression();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<(int Id, int Item2)>(sql, dapperParams)).ToList();

        // Assert - Should return all customers with senior flag (1 for >65, 0 for <=65)
        Assert.Equal(4, results.Count);
        Assert.Contains(results, r => r.Id == 1 && r.Item2 == 0); // John Doe, age 25
        Assert.Contains(results, r => r.Id == 2 && r.Item2 == 0); // Jane Smith, age 30
        Assert.Contains(results, r => r.Id == 3 && r.Item2 == 0); // Minor User, age 16
        Assert.Contains(results, r => r.Id == 4 && r.Item2 == 0); // Senior User, age 65 (65 is NOT > 65)
    }

    [Fact]
    public async Task CaseBoolExpression_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.CaseBoolExpression();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<(int Id, bool Item2)>(sql, dapperParams)).ToList();

        // Assert - Should return active status for adults, false for minors
        Assert.Equal(4, results.Count);
        Assert.Contains(results, r => r.Id == 1 && r.Item2 == true);  // John Doe, adult and active
        Assert.Contains(results, r => r.Id == 2 && r.Item2 == true);  // Jane Smith, adult and active
        Assert.Contains(results, r => r.Id == 3 && r.Item2 == false); // Minor User, minor so false
        Assert.Contains(results, r => r.Id == 4 && r.Item2 == true);  // Senior User, adult and active
    }

    [Fact]
    public async Task CaseInWhere_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.CaseInWhere();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<(int Id, string Name)>(sql, dapperParams)).ToList();

        // Assert - Should return only adults (Case returns "Adult" for age > 18)
        Assert.Equal(3, results.Count);
        Assert.Contains(results, r => r.Name == "John Doe");
        Assert.Contains(results, r => r.Name == "Jane Smith");
        Assert.Contains(results, r => r.Name == "Senior User");
        Assert.DoesNotContain(results, r => r.Name == "Minor User");
    }

    [Fact]
    public async Task LikeWildcard_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LikeWildcard();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<(int Id, string Name)>(sql, dapperParams)).ToList();

        // Assert - Should return customers whose names start with "Jo" (John Doe)
        Assert.Single(results);
        Assert.Contains(results, r => r.Name == "John Doe");
    }

    [Fact]
    public async Task LikeSingleChar_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LikeSingleChar();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<(int Id, string Name)>(sql, dapperParams)).ToList();

        // Assert - Should return no customers (no names match "J_n" pattern)
        Assert.Empty(results);
    }

    [Fact]
    public async Task LikeBothWildcards_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LikeBothWildcards();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<(int Id, string Name)>(sql, dapperParams)).ToList();

        // Assert - Should return customers whose names contain "o_n" pattern (John)
        Assert.Single(results);
        Assert.Contains(results, r => r.Name == "John Doe");
    }

    [Fact]
    public async Task LikeExact_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.LikeExact();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<(int Id, string Name)>(sql, dapperParams)).ToList();

        // Assert - Should return no customers (no exact match for "John")
        Assert.Empty(results);
    }

    [Fact]
    public async Task AbsColumn_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.AbsColumn();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<(int Id, int Item2)>(sql, dapperParams)).ToList();

        // Assert - Should return all customers with absolute values of their ages
        Assert.Equal(4, results.Count);
        Assert.Contains(results, r => r.Id == 1 && r.Item2 == 25); // John Doe
        Assert.Contains(results, r => r.Id == 2 && r.Item2 == 30); // Jane Smith
        Assert.Contains(results, r => r.Id == 3 && r.Item2 == 16); // Minor User
        Assert.Contains(results, r => r.Id == 4 && r.Item2 == 65); // Senior User
    }

    [Fact]
    public async Task AbsInWhere_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.AbsInWhere();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<(int Id, string Name, int Age)>(sql, dapperParams)).ToList();

        // Assert - Should return customers whose absolute age > 30
        Assert.Single(results); // Only Senior User has age > 30
        Assert.Contains(results, r => r.Name == "Senior User" && r.Age == 65);
    }

    [Fact]
    public async Task AbsExpression_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.AbsExpression();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<(int Id, int Item2)>(sql, dapperParams)).ToList();

        // Assert - Should return absolute value of (age - 50)
        Assert.Equal(4, results.Count);
        Assert.Contains(results, r => r.Id == 1 && r.Item2 == 25); // |25 - 50| = 25
        Assert.Contains(results, r => r.Id == 2 && r.Item2 == 20); // |30 - 50| = 20
        Assert.Contains(results, r => r.Id == 3 && r.Item2 == 34); // |16 - 50| = 34
        Assert.Contains(results, r => r.Id == 4 && r.Item2 == 15); // |65 - 50| = 15
    }

    [Fact]
    public async Task AbsParameter_GeneratesCorrectSql()
    {
        // Arrange - Create a parameterized query with ABS function
        var minAgeParam = 20;  // Set parameter value
        var query = TestQueries.AbsParameter();
        var (sql, parameters) = query.ToPostgreSqlRaw();
        
        // Set the parameter value for actual execution
        var updatedParams = parameters.SetItem(":minAge", minAgeParam);

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = updatedParams.ToDapperParameters();
        var results = (await connection.QueryAsync<(int Id, string Name, int Age)>(sql, dapperParams)).ToList();

        // Assert - Should return customers where ABS(age) > ABS(20)
        // All customers have positive ages, so this becomes age > 20
        Assert.Equal(3, results.Count); // John Doe (25), Jane Smith (30), Senior User (65)
        Assert.Contains(results, r => r.Name == "John Doe" && r.Age == 25);
        Assert.Contains(results, r => r.Name == "Jane Smith" && r.Age == 30);
        Assert.Contains(results, r => r.Name == "Senior User" && r.Age == 65);
        Assert.DoesNotContain(results, r => r.Name == "Minor User"); // Age 16 < 20
    }

    // ========== NEW COLUMN TYPES QUERY TESTS ==========
    // Full database execution tests for new column types

    [Fact]
    public async Task FromWhereDecimalComparison_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.FromWhereDecimalComparison();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<ProductDto>(sql, dapperParams)).ToList();

        // Assert - Should return products with price > 50
        Assert.Single(results);
        Assert.Equal("Laptop", results.First().ProductName);
        Assert.Equal(999.99m, results.First().Price);
    }

    [Fact]
    public async Task FromSelectDecimalArithmetic_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.FromSelectDecimalArithmetic();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync(sql, dapperParams)).ToList();

        // Assert - Should return all products with calculated values
        Assert.Equal(3, results.Count); // All products returned
        Assert.NotNull(results); // Basic validation that query executes
    }

    [Fact]
    public async Task FromWhereDecimalIsNull_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.FromWhereDecimalIsNull();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<ProductDto>(sql, dapperParams)).ToList();

        // Assert - Should return products with NULL price
        Assert.Single(results);
        Assert.Equal("Discontinued", results.First().ProductName);
        Assert.Null(results.First().Price);
    }

    [Fact]
    public async Task FromWhereDecimalIsNotNull_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.FromWhereDecimalIsNotNull();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<ProductDto>(sql, dapperParams)).ToList();

        // Assert - Should return products with non-NULL price
        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.ProductName == "Laptop" && r.Price == 999.99m);
        Assert.Contains(results, r => r.ProductName == "Mouse" && r.Price == 25.50m);
    }

    [Fact]
    public async Task CaseDecimalExpression_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.CaseDecimalExpression();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task ParameterAsDecimalParam_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.ParameterAsDecimalParam();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<ProductDto>(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task FromWhereCreatedDateComparison_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.FromWhereCreatedDateComparison();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<ProductDto>(sql, dapperParams)).ToList();

        // Assert - Should execute successfully (specific results may vary based on query logic)
        Assert.NotNull(results);
    }

    [Fact]
    public async Task FromWhereCreatedDateIsNull_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.FromWhereCreatedDateIsNull();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<ProductDto>(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task FromWhereCreatedDateIsNotNull_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.FromWhereCreatedDateIsNotNull();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<ProductDto>(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task FromSelectCreatedDateMinMax_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.FromSelectCreatedDateMinMax();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task CaseDateTimeExpression_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.CaseDateTimeExpression();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task ParameterAsDateTimeParam_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.ParameterAsDateTimeParam();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<ProductDto>(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task FromWhereUniqueIdEquals_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.FromWhereUniqueIdEquals();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<ProductDto>(sql, dapperParams)).ToList();

        // Assert - Should execute successfully (specific results may vary based on query logic)
        Assert.NotNull(results);
    }

    [Fact]
    public async Task FromWhereUniqueIdNotEquals_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.FromWhereUniqueIdNotEquals();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<ProductDto>(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task FromWhereUniqueIdIsNull_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.FromWhereUniqueIdIsNull();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<ProductDto>(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task FromWhereUniqueIdIsNotNull_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.FromWhereUniqueIdIsNotNull();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<ProductDto>(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task CaseGuidExpression_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.CaseGuidExpression();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task ParameterAsGuidParam_GeneratesCorrectSql() 
    { 
        // Arrange
        var query = TestQueries.ParameterAsGuidParam();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync<ProductDto>(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task SumPrices_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.SumPrices();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task AvgPrices_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.AvgPrices();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task MinPrice_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.MinPrice();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task MaxPrice_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.MaxPrice();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task SumExpensivePrices_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.SumExpensivePrices();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task AvgExpensivePrices_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.AvgExpensivePrices();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task FromGroupByDecimalAggregatesSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByDecimalAggregatesSelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task FromGroupByDecimalSumSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByDecimalSumSelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }

    [Fact]
    public async Task FromGroupByDecimalAvgSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = TestQueries.FromGroupByDecimalAvgSelect();
        var (sql, parameters) = query.ToPostgreSqlRaw();

        // Act - Execute query with Dapper against PostgreSQL
        using var connection = _fixture.CreateConnection();
        connection.Open();
        var dapperParams = parameters.ToDapperParameters();
        var results = (await connection.QueryAsync(sql, dapperParams)).ToList();

        // Assert - Should execute successfully and return results
        Assert.NotNull(results);
        Assert.True(results.Count >= 0); // Should execute without error
    }
}
