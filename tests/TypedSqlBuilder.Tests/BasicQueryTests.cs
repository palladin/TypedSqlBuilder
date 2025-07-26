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
        var sql = SqlQueryCompiler.Compile(query);
        
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
        var sql = SqlQueryCompiler.Compile(query);
        
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
        var sql = SqlQueryCompiler.Compile(query);
        
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
        var sql = SqlQueryCompiler.Compile(query);
        
        // Assert
        Assert.Equal("SELECT * FROM customers WHERE customers.Age > 18", sql);
    }

    [Fact]
    public void Where_StringEquality_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Name == "John");
        
        // Act
        var sql = SqlQueryCompiler.Compile(query);
        
        // Assert
        Assert.Equal("SELECT * FROM customers WHERE customers.Name = 'John'", sql);
    }

    [Fact]
    public void Where_MultipleConditions_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .Where(c => c.Age > 18 & c.Name != "Admin");
        
        // Act
        var sql = SqlQueryCompiler.Compile(query);
        
        // Assert
        Assert.Equal("SELECT * FROM customers WHERE (customers.Age > 18) AND (customers.Name != 'Admin')", sql);
    }

    [Fact]
    public void OrderBy_Ascending_GeneratesCorrectSql()
    {
        // Arrange
        var query = SqlQuery.From<Customer>()
            .OrderBy(c => c.Name);
        
        // Act
        var sql = SqlQueryCompiler.Compile(query);
        
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
        var sql = SqlQueryCompiler.Compile(query);
        
        // Assert
        Assert.Equal("SELECT * FROM customers ORDER BY customers.Age DESC", sql);
    }
}
