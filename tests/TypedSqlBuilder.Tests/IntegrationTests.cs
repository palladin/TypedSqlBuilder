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
        var sql = SqlQueryCompiler.Compile(query);
        
        // Assert
        var expectedSql = "SELECT customers.Id, CONCAT(customers.Name, ' (Customer)'), (customers.Age + 5) FROM customers WHERE (customers.Age >= 21) AND (customers.Name != '') ORDER BY customers.Name ASC";
        Assert.Equal(expectedSql, sql);
    }

    [Fact]
    public void TypedSqlBuilder_MultipleTableTypes_WorksCorrectly()
    {
        // Arrange - Test with Product table
        var productQuery = SqlQuery.From<Product>()
            .Where(p => p.ProductName != "Discontinued")
            .Select(p => (p.ProductId, p.ProductName));
        
        // Act
        var sql = SqlQueryCompiler.Compile(productQuery);
        
        // Assert
        Assert.Equal("SELECT products.ProductId, products.ProductName FROM products WHERE products.ProductName != 'Discontinued'", sql);
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
        var sql = SqlQueryCompiler.Compile(query);
        
        // Assert - The structure should be consistent regardless of parameter values
        Assert.Contains("SELECT customers.Id, customers.Name", sql);
        Assert.Contains($"WHERE (customers.Age >= {minAge}) AND (customers.Age <= {maxAge})", sql);
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
        var sql = SqlQueryCompiler.Compile(query);
        
        // Assert
        Assert.Equal("SELECT customers.Id, ((customers.Id * 100) + customers.Age), customers.Name FROM customers WHERE customers.Age > 18", sql);
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
        var sql = SqlQueryCompiler.Compile(query);
        
        // Assert
        Assert.Equal("SELECT customers.Id, customers.Name, (customers.Age + 10) FROM customers WHERE (customers.Age > 21) AND (customers.Name != '') ORDER BY customers.Age ASC", sql);
    }
}
