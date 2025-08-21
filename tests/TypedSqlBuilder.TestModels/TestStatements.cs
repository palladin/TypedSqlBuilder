using TypedSqlBuilder.Core;
using TypedSqlBuilder.TestModels;

namespace TypedSqlBuilder.TestModels;

/// <summary>
/// Test queries for INSERT, UPDATE, and DELETE operations.
/// These demonstrate the intended fluent API for data modification.
/// </summary>
public static class TestStatements
{
    // ========== INSERT EXAMPLES ==========
    
    /// <summary>
    /// Basic INSERT with explicit values using Value method
    /// INSERT INTO customers (Id, Age, Name) VALUES (200, 25, 'John Doe')
    /// </summary>
    public static ISqlStatement InsertBasic() 
        => TypedSql.Insert<Customer>()
            .Value(c => c.Id, 200) // Use ID 200 to avoid conflicts
            .Value(c => c.Age, 25)
            .Value(c => c.Name, "John Doe");

    // ========== UPDATE EXAMPLES ==========
    
    /// <summary>
    /// Basic UPDATE with SET clause for one column
    /// UPDATE customers SET Age = 26 WHERE Id = 200
    /// </summary>
    public static ISqlStatement UpdateBasic() 
        => TypedSql.Update<Customer>()
            .Set(c => c.Age, 26)
            .Where(c => c.Id == 200);

    /// <summary>
    /// UPDATE with multiple SET clauses  
    /// UPDATE customers SET Age = 27, Name = 'John Smith' WHERE Id = 200
    /// </summary>
    public static ISqlStatement UpdateMultiple() 
        => TypedSql.Update<Customer>()
            .Set(c => c.Age, 27)
            .Set(c => c.Name, "John Smith")
            .Where(c => c.Id == 200);

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
    /// DELETE FROM customers WHERE Id = 200
    /// </summary>
    public static ISqlStatement DeleteBasic() 
        => TypedSql.Delete<Customer>()
            .Where(c => c.Id == 200);

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
    // Update with SET NULL
    public static ISqlStatement UpdateSetNull() 
        => TypedSql.Update<Customer>()
            .Set(c => c.Name, SqlNull.Value);

    public static ISqlStatement UpdateSetNullInt() 
        => TypedSql.Update<Customer>()
            .Set(c => c.Age, SqlNull.Value);

    public static ISqlStatement UpdateSetNullMixed() 
        => TypedSql.Update<Customer>()
            .Set(c => c.Name, "John")
            .Set(c => c.Age, SqlNull.Value);

    public static ISqlStatement UpdateSetNullWhere() 
        => TypedSql.Update<Customer>()
            .Set(c => c.Name, SqlNull.Value)
            .Where(c => c.Id == 200);

    // Insert with NULL values
    public static ISqlStatement InsertWithNull() 
        => TypedSql.Insert<Customer>()
            .Value(c => c.Id, 202)
            .Value(c => c.Name, SqlNull.Value)
            .Value(c => c.Age, 25);

    public static ISqlStatement InsertWithNullInt() 
        => TypedSql.Insert<Customer>()
            .Value(c => c.Id, 203)
            .Value(c => c.Name, "John")
            .Value(c => c.Age, SqlNull.Value);

    // ========== NEW COLUMN TYPES STATEMENT TESTS ==========
    
    /// <summary>
    /// INSERT with new column types (decimal, datetime, guid)
    /// INSERT INTO products (ProductId, ProductName, Price, CreatedDate, UniqueId) VALUES (200, 'Test Product', 99.99, '2024-08-18T00:00:00', '12345678-1234-1234-1234-123456789012')
    /// </summary>
    public static ISqlStatement InsertWithNewColumns() 
        => TypedSql.Insert<Product>()
            .Value(p => p.ProductId, 200)
            .Value(p => p.ProductName, "Test Product")
            .Value(p => p.Price, 99.99m)
            .Value(p => p.CreatedDate, new DateTime(2024, 8, 18))
            .Value(p => p.UniqueId, Guid.Parse("12345678-1234-1234-1234-123456789012"));

    /// <summary>
    /// UPDATE with new column types  
    /// UPDATE products SET Price = 119.99, CreatedDate = '2024-12-25T00:00:00', UniqueId = '87654321-4321-4321-4321-210987654321' WHERE ProductId = 100
    /// </summary>
    public static ISqlStatement UpdateWithNewColumns() 
        => TypedSql.Update<Product>()
            .Set(p => p.Price, 119.99m)
            .Set(p => p.CreatedDate, new DateTime(2024, 12, 25))
            .Set(p => p.UniqueId, Guid.Parse("87654321-4321-4321-4321-210987654321"))
            .Where(p => p.ProductId == 100);

    /// <summary>
    /// INSERT with new column types set to NULL
    /// INSERT INTO products (ProductId, ProductName, Price, CreatedDate, UniqueId) VALUES (201, 'Null Test', NULL, NULL, NULL)
    /// </summary>
    public static ISqlStatement InsertWithNewColumnsNull() 
        => TypedSql.Insert<Product>()
            .Value(p => p.ProductId, 201)
            .Value(p => p.ProductName, "Null Test")
            .Value(p => p.Price, SqlNull.Value)
            .Value(p => p.CreatedDate, SqlNull.Value)
            .Value(p => p.UniqueId, SqlNull.Value);

    /// <summary>
    /// UPDATE setting new column types to NULL
    /// UPDATE products SET Price = NULL, CreatedDate = NULL, UniqueId = NULL WHERE ProductId = 101
    /// </summary>
    public static ISqlStatement UpdateSetNewColumnsNull() 
        => TypedSql.Update<Product>()
            .Set(p => p.Price, SqlNull.Value)
            .Set(p => p.CreatedDate, SqlNull.Value)
            .Set(p => p.UniqueId, SqlNull.Value)
            .Where(p => p.ProductId == 101);
}
