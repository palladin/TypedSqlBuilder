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
        var (sql, context) = SqlQueryCompiler.CompileWithParameters(query);
        
        // Assert
        var expectedSql = "SELECT customers.Id, CONCAT(customers.Name, @p2), (customers.Age + @p3) FROM customers WHERE (customers.Age >= @p0) AND (customers.Name != @p1) ORDER BY customers.Name ASC";
        Assert.Equal(expectedSql, sql);
        Assert.Equal(4, context.Parameters.Count);
        Assert.Equal(21, context.Parameters["@p0"]);
        Assert.Equal("", context.Parameters["@p1"]);
        Assert.Equal(" (Customer)", context.Parameters["@p2"]);
        Assert.Equal(5, context.Parameters["@p3"]);
    }

    [Fact]
    public void TypedSqlBuilder_MultipleTableTypes_WorksCorrectly()
    {
        // Arrange - Test with Product table
        var productQuery = SqlQuery.From<Product>()
            .Where(p => p.ProductName != "Discontinued")
            .Select(p => (p.ProductId, p.ProductName));
        
        // Act
        var (sql, context) = SqlQueryCompiler.CompileWithParameters(productQuery);
        
        // Assert
        Assert.Equal("SELECT products.ProductId, products.ProductName FROM products WHERE products.ProductName != @p0", sql);
        Assert.Single(context.Parameters);
        Assert.Equal("Discontinued", context.Parameters["@p0"]);
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
        var (sql, context) = SqlQueryCompiler.CompileWithParameters(query);
        
        // Assert - The structure should be consistent regardless of parameter values
        Assert.Contains("SELECT customers.Id, customers.Name", sql);
        Assert.Contains("WHERE (customers.Age >= @p0) AND (customers.Age <= @p1)", sql);
        Assert.Equal(2, context.Parameters.Count);
        Assert.Equal(minAge, context.Parameters["@p0"]);
        Assert.Equal(maxAge, context.Parameters["@p1"]);
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
        var (sql, context) = SqlQueryCompiler.CompileWithParameters(query);
        
        // Assert
        Assert.Equal("SELECT customers.Id, ((customers.Id * @p1) + customers.Age), customers.Name FROM customers WHERE customers.Age > @p0", sql);
        Assert.Equal(2, context.Parameters.Count);
        Assert.Equal(18, context.Parameters["@p0"]);
        Assert.Equal(100, context.Parameters["@p1"]);
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
        var (sql, context) = SqlQueryCompiler.CompileWithParameters(query);
        
        // Assert
        Assert.Equal("SELECT customers.Id, customers.Name, (customers.Age + @p2) FROM customers WHERE (customers.Age > @p0) AND (customers.Name != @p1) ORDER BY customers.Age ASC", sql);
        Assert.Equal(3, context.Parameters.Count);
        Assert.Equal(21, context.Parameters["@p0"]);
        Assert.Equal("", context.Parameters["@p1"]);
        Assert.Equal(10, context.Parameters["@p2"]);
    }
}
