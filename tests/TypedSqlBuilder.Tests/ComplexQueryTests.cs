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
        var sql = SqlQueryCompiler.Compile(query);
        
        // Assert
        Assert.Equal("SELECT (customers.Id + 1), CONCAT(customers.Name, '!') FROM customers WHERE customers.Age > 18 ORDER BY customers.Name ASC", sql);
    }

    [Fact]
    public void Query_WhereAndSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age >= 21)
            .Select(c => (c.Id, c.Name));
        
        // Act
        var sql = SqlQueryCompiler.Compile(query);
        
        // Assert
        Assert.Equal("SELECT customers.Id, customers.Name FROM customers WHERE customers.Age >= 21", sql);
    }

    [Fact]
    public void Query_MultipleWhereConditionsWithComplexLogic_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => (c.Age > 18 & c.Age < 65) || c.Name == "VIP");
        
        // Act
        var sql = SqlQueryCompiler.Compile(query);
        
        // Assert
        Assert.Equal("SELECT * FROM customers WHERE ((customers.Age > 18) AND (customers.Age < 65)) OR (customers.Name = 'VIP')", sql);
    }

    [Fact]
    public void Query_SelectWithArithmeticAndStringOps_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Select(c => (c.Id * 100 + c.Age, c.Name + " - Customer"));
        
        // Act
        var sql = SqlQueryCompiler.Compile(query);
        
        // Assert
        Assert.Equal("SELECT ((customers.Id * 100) + customers.Age), CONCAT(customers.Name, ' - Customer') FROM customers", sql);
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
        var sql = SqlQueryCompiler.Compile(query);
        
        // Assert
        Assert.Equal("SELECT customers.Id, customers.Name, (customers.Age + 10) FROM customers WHERE (customers.Age > 21) AND (customers.Name != '') ORDER BY customers.Age ASC", sql);
    }
}
