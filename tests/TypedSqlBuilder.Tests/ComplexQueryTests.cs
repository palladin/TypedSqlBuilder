namespace TypedSqlBuilder.Tests;

/// <summary>
/// xUnit tests for complex SQL query combinations
/// </summary>
public class ComplexQueryTests
{
    [Fact]
    public void ComplexQuery_SelectWhereOrderBy_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age > 18)
            .OrderBy(c => c.Name)
            .Select(c => (c.Id + 1, c.Name + "!"));
        
        // Act
        var (sql, context) = SqlQueryCompiler.CompileWithParameters(query);
        
        // Assert
        Assert.Equal("SELECT (customers.Id + @p1), CONCAT(customers.Name, @p2) FROM customers WHERE customers.Age > @p0 ORDER BY customers.Name ASC", sql);
        Assert.Equal(3, context.Parameters.Count);
        Assert.Equal(18, context.Parameters["@p0"]);
        Assert.Equal(1, context.Parameters["@p1"]);
        Assert.Equal("!", context.Parameters["@p2"]);
    }

    [Fact]
    public void Query_WhereAndSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age >= 21)
            .Select(c => (c.Id, c.Name));
        
        // Act
        var (sql, context) = SqlQueryCompiler.CompileWithParameters(query);
        
        // Assert
        Assert.Equal("SELECT customers.Id, customers.Name FROM customers WHERE customers.Age >= @p0", sql);
        Assert.Single(context.Parameters);
        Assert.Equal(21, context.Parameters["@p0"]);
    }

    [Fact]
    public void Query_MultipleWhereConditionsWithComplexLogic_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => (c.Age > 18 & c.Age < 65) || c.Name == "VIP");
        
        // Act
        var (sql, context) = SqlQueryCompiler.CompileWithParameters(query);
        
        // Assert
        Assert.Equal("SELECT * FROM customers WHERE ((customers.Age > @p0) AND (customers.Age < @p1)) OR (customers.Name = @p2)", sql);
        Assert.Equal(3, context.Parameters.Count);
        Assert.Equal(18, context.Parameters["@p0"]);
        Assert.Equal(65, context.Parameters["@p1"]);
        Assert.Equal("VIP", context.Parameters["@p2"]);
    }

    [Fact]
    public void Query_SelectWithArithmeticAndStringOps_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Select(c => (c.Id * 100 + c.Age, c.Name + " - Customer"));
        
        // Act
        var (sql, context) = SqlQueryCompiler.CompileWithParameters(query);
        
        // Assert
        Assert.Equal("SELECT ((customers.Id * @p0) + customers.Age), CONCAT(customers.Name, @p1) FROM customers", sql);
        Assert.Equal(2, context.Parameters.Count);
        Assert.Equal(100, context.Parameters["@p0"]);
        Assert.Equal(" - Customer", context.Parameters["@p1"]);
    }

    [Fact]
    public void Query_AllClausesCombined_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age > 21 && c.Name != "")
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
