namespace TypedSqlBuilder.Tests;

/// <summary>
/// Integration tests for complete SQL query generation
/// </summary>
public class IntegrationTests
{
    [Fact]
    public void TypedSqlBuilder_CompleteWorkflow_GeneratesExpectedSql()
    {
        // Arrange - Create a complex query using the fluent API
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age >= 21 & c.Name != "")
            .OrderBy(c => c.Name)
            .Select(c => (
                CustomerId: c.Id,
                CustomerInfo: c.Name + " (Customer)",
                AdjustedAge: c.Age + 5
            ));
        
        // Act
        var (sql, parameters) = query.ToSqlServerRaw();
        
        // Assert
        var expectedSql = "SELECT customers.Id, CONCAT(customers.Name, @p2), (customers.Age + @p3) FROM customers WHERE (customers.Age >= @p0) AND (customers.Name != @p1) ORDER BY customers.Name ASC";
        Assert.Equal(expectedSql, sql);
        Assert.Equal(4, parameters.Count);
        Assert.Equal(21, parameters["@p0"]);
        Assert.Equal("", parameters["@p1"]);
        Assert.Equal(" (Customer)", parameters["@p2"]);
        Assert.Equal(5, parameters["@p3"]);
    }

    [Fact]
    public void TypedSqlBuilder_MultipleTableTypes_WorksCorrectly()
    {
        // Arrange - Test with Product table
        var productQuery = SqlQuery.From<Product>()
            .Where(p => p.ProductName != "Discontinued")
            .Select(p => (p.ProductId, p.ProductName));
        
        // Act
        var (sql, parameters) = productQuery.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("SELECT products.ProductId, products.ProductName FROM products WHERE products.ProductName != @p0", sql);
        Assert.Single(parameters);
        Assert.Equal("Discontinued", parameters["@p0"]);
    }

    [Theory]
    [InlineData(18, 65)]
    [InlineData(13, 17)]
    [InlineData(66, 120)]
    public void TypedSqlBuilder_ParameterizedQueries_WorkWithDifferentValues(int minAge, int maxAge)
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age >= minAge & c.Age <= maxAge)
            .Select(c => (c.Id, c.Name));
        
        // Act
        var (sql, parameters) = query.ToSqlServerRaw();
        
        // Assert - The structure should be consistent regardless of parameter values
        Assert.Contains("SELECT customers.Id, customers.Name", sql);
        Assert.Contains("WHERE (customers.Age >= @p0) AND (customers.Age <= @p1)", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(minAge, parameters["@p0"]);
        Assert.Equal(maxAge, parameters["@p1"]);
    }

    [Fact]
    public void TypedSqlBuilder_ComplexArithmeticInSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age > 18)
            .Select(c => (
                OriginalId: c.Id,
                ModifiedId: c.Id * 100 + c.Age,
                CustomerName: c.Name
            ));
        
        // Act
        var (sql, parameters) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("SELECT customers.Id, ((customers.Id * @p1) + customers.Age), customers.Name FROM customers WHERE customers.Age > @p0", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(18, parameters["@p0"]);
        Assert.Equal(100, parameters["@p1"]);
    }

    [Fact]
    public void TypedSqlBuilder_AllClausesCombined_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age > 21 & c.Name != "")
            .OrderBy(c => c.Age)
            .Select(c => (c.Id, c.Name, c.Age + 10));
        
        // Act
        var (sql, parameters) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("SELECT customers.Id, customers.Name, (customers.Age + @p2) FROM customers WHERE (customers.Age > @p0) AND (customers.Name != @p1) ORDER BY customers.Age ASC", sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(21, parameters["@p0"]);
        Assert.Equal("", parameters["@p1"]);
        Assert.Equal(10, parameters["@p2"]);
    }
}
