using TypedSqlBuilder.Core;

namespace TypedSqlBuilder.Tests;

/// <summary>
/// Test queries for INSERT, UPDATE, and DELETE operations.
/// These demonstrate the intended fluent API for data modification.
/// </summary>
public static class TestStatements
{
    // ========== INSERT EXAMPLES ==========
    
    /// <summary>
    /// Basic INSERT with explicit values using Value method
    /// INSERT INTO customers (Id, Age, Name) VALUES (1, 25, 'John Doe')
    /// </summary>
    public static ISqlStatement InsertBasic() 
        => TypedSql.Insert<Customer>()
            .Value(c => c.Id, 1)
            .Value(c => c.Age, 25)
            .Value(c => c.Name, "John Doe");

    /// <summary>
    /// INSERT with only some columns specified
    /// INSERT INTO customers (Age, Name) VALUES (30, 'Jane Smith')
    /// </summary>
    public static ISqlStatement InsertPartial() 
        => TypedSql.Insert<Customer>()
            .Value(c => c.Age, 30)
            .Value(c => c.Name, "Jane Smith");

    // ========== UPDATE EXAMPLES ==========
    
    /// <summary>
    /// Basic UPDATE with SET clause for one column
    /// UPDATE customers SET Age = 26 WHERE Id = 1
    /// </summary>
    public static ISqlStatement UpdateBasic() 
        => TypedSql.Update<Customer>()
            .Set(c => c.Age, 26)
            .Where(c => c.Id == 1);

    /// <summary>
    /// UPDATE with multiple SET clauses  
    /// UPDATE customers SET Age = 27, Name = 'John Smith' WHERE Id = 1
    /// </summary>
    public static ISqlStatement UpdateMultiple() 
        => TypedSql.Update<Customer>()
            .Set(c => c.Age, 27)
            .Set(c => c.Name, "John Smith")
            .Where(c => c.Id == 1);

    /// <summary>
    /// UPDATE with complex WHERE condition and expression-based SET
    /// UPDATE customers SET Age = Age + 1 WHERE Age >= 18 AND Name != 'Admin'
    /// </summary>
    public static ISqlStatement UpdateConditional() 
        => TypedSql.Update<Customer>()
            .Set(c => c.Age, c => c.Age + 1)  // Using two lambdas for expression increment
            .Where(c => c.Age >= 18 && c.Name != "Admin");

    // ========== DELETE EXAMPLES ==========
    
    /// <summary>
    /// Basic DELETE with WHERE clause
    /// DELETE FROM customers WHERE Id = 1
    /// </summary>
    public static ISqlStatement DeleteBasic() 
        => TypedSql.Delete<Customer>()
            .Where(c => c.Id == 1);

    /// <summary>
    /// DELETE with complex WHERE condition
    /// DELETE FROM customers WHERE Age < 18 OR Name = 'Temp'
    /// </summary>
    public static ISqlStatement DeleteConditional() 
        => TypedSql.Delete<Customer>()
            .Where(c => c.Age < 18 || c.Name == "Temp");

    /// <summary>
    /// DELETE all records (no WHERE clause)
    /// DELETE FROM customers
    /// Note: This might be dangerous in production!
    /// </summary>
    public static ISqlStatement DeleteAll() 
        => TypedSql.Delete<Customer>();
}
