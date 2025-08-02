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
        var (sql, parameters) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("SELECT (customers.Id + @p1), CONCAT(customers.Name, @p2) FROM customers WHERE customers.Age > @p0 ORDER BY customers.Name ASC", sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(18, parameters["@p0"]);
        Assert.Equal(1, parameters["@p1"]);
        Assert.Equal("!", parameters["@p2"]);
    }

    [Fact]
    public void Query_WhereAndSelect_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age >= 21)
            .Select(c => (c.Id, c.Name));
        
        // Act
        var (sql, parameters) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("SELECT customers.Id, customers.Name FROM customers WHERE customers.Age >= @p0", sql);
        Assert.Single(parameters);
        Assert.Equal(21, parameters["@p0"]);
    }

    [Fact]
    public void Query_MultipleWhereConditionsWithComplexLogic_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => (c.Age > 18 & c.Age < 65) || c.Name == "VIP");
        
        // Act
        var (sql, parameters) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("SELECT * FROM customers WHERE ((customers.Age > @p0) AND (customers.Age < @p1)) OR (customers.Name = @p2)", sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(18, parameters["@p0"]);
        Assert.Equal(65, parameters["@p1"]);
        Assert.Equal("VIP", parameters["@p2"]);
    }

    [Fact]
    public void Query_SelectWithArithmeticAndStringOps_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Select(c => (c.Id * 100 + c.Age, c.Name + " - Customer"));
        
        // Act
        var (sql, parameters) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("SELECT ((customers.Id * @p0) + customers.Age), CONCAT(customers.Name, @p1) FROM customers", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(100, parameters["@p0"]);
        Assert.Equal(" - Customer", parameters["@p1"]);
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
        var (sql, parameters) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("SELECT customers.Id, customers.Name, (customers.Age + @p2) FROM customers WHERE (customers.Age > @p0) AND (customers.Name != @p1) ORDER BY customers.Age ASC", sql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(21, parameters["@p0"]);
        Assert.Equal("", parameters["@p1"]);
        Assert.Equal(10, parameters["@p2"]);
    }
}
