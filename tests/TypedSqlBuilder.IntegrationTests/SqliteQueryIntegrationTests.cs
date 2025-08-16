using TypedSqlBuilder.Core;
using TypedSqlBuilder.TestModels;
using Dapper;

namespace TypedSqlBuilder.IntegrationTests;

/// <summary>
/// Integration tests for SELECT queries executed against SQLite databases using Dapper
/// </summary>
public class SqliteQueryIntegrationTests : SqliteIntegrationTestBase
{
    [Fact]
    public void FromWhereInt_ExecutesCorrectly()
    {
        // Arrange
        var query = TestQueries.FromWhereInt();
        var (sql, parameters) = query.ToSqliteRaw();

        // Act - Execute query with Dapper
        var dapperParams = parameters.ToDapperParameters();
        var results = _connection.Query<CustomerDto>(sql, dapperParams).ToList();

        // Assert - Should return customers over 18
        Assert.Equal(3, results.Count);
        Assert.Contains(results, r => r.Name == "John Doe" && r.Age == 25);
        Assert.Contains(results, r => r.Name == "Jane Smith" && r.Age == 30);
        Assert.Contains(results, r => r.Name == "Senior User" && r.Age == 65);
        Assert.DoesNotContain(results, r => r.Name == "Minor User");
    }

    [Fact]
    public void FromWhereSelect_ExecutesCorrectly()
    {
        // Arrange
        var query = TestQueries.FromWhereSelect();
        var (sql, parameters) = query.ToSqliteRaw();

        // Act - Execute query with Dapper
        var dapperParams = parameters.ToDapperParameters();
        // Use anonymous type for the projection (Id, Name)
        var results = _connection.Query<(int Id, string Name)>(sql, dapperParams).ToList();

        // Assert - Should return adults with selected columns
        Assert.Equal(3, results.Count);
        Assert.Contains(results, r => r.Name == "John Doe" && r.Id == 1);
        Assert.Contains(results, r => r.Name == "Jane Smith" && r.Id == 2);
        Assert.Contains(results, r => r.Name == "Senior User" && r.Id == 4);
    }

    [Fact]
    public void InnerJoinBasic_ExecutesCorrectly()
    {
        // Arrange
        var query = TestQueries.InnerJoinBasic();
        var (sql, parameters) = query.ToSqliteRaw();

        // Act - Execute query with Dapper
        var dapperParams = parameters.ToDapperParameters();
        // Use anonymous type for the projection (Id, Name, OrderId, Amount)
        var results = _connection.Query<(int Id, string Name, int OrderId, int Amount)>(sql, dapperParams).ToList();

        // Assert - Should return customers with their orders
        Assert.Equal(4, results.Count);
        
        // John Doe should have 2 orders
        var johnOrders = results.Where(r => r.Name == "John Doe").ToList();
        Assert.Equal(2, johnOrders.Count);
        Assert.Contains(johnOrders, o => o.OrderId == 1 && o.Amount == 500);
        Assert.Contains(johnOrders, o => o.OrderId == 2 && o.Amount == 150);
        
        // Jane Smith should have 1 order
        var janeOrders = results.Where(r => r.Name == "Jane Smith").ToList();
        Assert.Single(janeOrders);
        Assert.Equal(300, janeOrders[0].Amount);
    }

    [Fact]
    public void FromWhereString_ExecutesCorrectly()
    {
        // Arrange
        var query = TestQueries.FromWhereString();
        var (sql, parameters) = query.ToSqliteRaw();

        // Act - Execute query with Dapper
        var dapperParams = parameters.ToDapperParameters();
        var results = _connection.Query<CustomerDto>(sql, dapperParams).ToList();

        // Assert - Should return customers named "John"
        Assert.Empty(results); // No customer named exactly "John" in test data
    }

    [Fact]
    public void FromOrderByAsc_ExecutesCorrectly()
    {
        // Arrange
        var query = TestQueries.FromOrderByAsc();
        var (sql, parameters) = query.ToSqliteRaw();

        // Act - Execute query with Dapper
        var dapperParams = parameters.ToDapperParameters();
        var results = _connection.Query<CustomerDto>(sql, dapperParams).ToList();

        // Assert - Should return all customers ordered by name ascending
        Assert.Equal(4, results.Count);
        Assert.Equal("Jane Smith", results[0].Name);
        Assert.Equal("John Doe", results[1].Name);
        Assert.Equal("Minor User", results[2].Name);
        Assert.Equal("Senior User", results[3].Name);
    }

    [Fact]
    public void FromProductWhereSelect_ExecutesCorrectly()
    {
        // Arrange
        var query = TestQueries.FromProductWhereSelect();
        var (sql, parameters) = query.ToSqliteRaw();

        // Act - Execute query with Dapper
        var dapperParams = parameters.ToDapperParameters();
        var results = _connection.Query<(int ProductId, string ProductName)>(sql, dapperParams).ToList();

        // Assert - Should return products that are not discontinued
        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.ProductName == "Laptop");
        Assert.Contains(results, r => r.ProductName == "Mouse");
        Assert.DoesNotContain(results, r => r.ProductName == "Discontinued");
    }
}
