namespace TypedSqlBuilder.Tests;

/// <summary>
/// xUnit tests for basic SQL query functionality - SELECT statements only
/// </summary>
public class BasicQueryTests
{
    [Fact]
    public void From_BasicTable_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>();
        
        // Act
        var (sql, _) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("SELECT * FROM customers", sql);
    }

    [Fact]
    public void Select_ColumnProjection_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Select(c => (c.Id, c.Name));
        
        // Act
        var (sql, _) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("SELECT customers.Id, customers.Name FROM customers", sql);
    }

    [Fact]
    public void Select_SingleColumn_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Select(c => c.Age);
        
        // Act
        var (sql, _) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("SELECT customers.Age FROM customers", sql);
    }

    [Fact]
    public void Where_IntegerComparison_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age > 18);
        
        // Act
        var (sql, parameters) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("SELECT * FROM customers WHERE customers.Age > @p0", sql);
        Assert.Single(parameters);
        Assert.Contains("@p0", parameters.Keys);
        Assert.Equal(18, parameters["@p0"]);
    }

    [Fact]
    public void Where_StringEquality_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Name == "John");
        
        // Act
        var (sql, parameters) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("SELECT * FROM customers WHERE customers.Name = @p0", sql);
        Assert.Single(parameters);
        Assert.Contains("@p0", parameters.Keys);
        Assert.Equal("John", parameters["@p0"]);
    }

    [Fact]
    public void Where_MultipleConditions_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age > 18 & c.Name != "Admin");
        
        // Act
        var (sql, parameters) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("SELECT * FROM customers WHERE (customers.Age > @p0) AND (customers.Name != @p1)", sql);
        Assert.Equal(2, parameters.Count);
        Assert.Contains("@p0", parameters.Keys);
        Assert.Contains("@p1", parameters.Keys);
        Assert.Equal(18, parameters["@p0"]);
        Assert.Equal("Admin", parameters["@p1"]);
    }

    [Fact]
    public void OrderBy_Ascending_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .OrderBy(c => c.Name);
        
        // Act
        var (sql, _) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("SELECT * FROM customers ORDER BY customers.Name ASC", sql);
    }

    [Fact]
    public void OrderBy_Descending_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .OrderByDescending(c => c.Age);
        
        // Act
        var (sql, _) = query.ToSqlServerRaw();
        
        // Assert
        Assert.Equal("SELECT * FROM customers ORDER BY customers.Age DESC", sql);
    }
}
