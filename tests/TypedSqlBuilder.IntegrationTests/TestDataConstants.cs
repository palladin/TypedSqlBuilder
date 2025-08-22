using System.Linq;

namespace TypedSqlBuilder.IntegrationTests;

/// <summary>
/// Shared test data tuples for database integration tests
/// </summary>
public static class TestDataConstants
{
    /// <summary>
    /// Customer data: (Id, Age, Name, IsActive)
    /// </summary>
    public static readonly (int Id, int Age, string Name, bool IsActive)[] Customers = 
    [
        (1, 25, "John Doe", true),
        (2, 30, "Jane Smith", true),
        (3, 16, "Minor User", false),
        (4, 65, "Senior User", true)
    ];

    /// <summary>
    /// Product data: (Id, ProductName, Price, CreatedDate, UniqueId)
    /// </summary>
    public static readonly (int Id, string ProductName, decimal? Price, DateTime? CreatedDate, Guid? UniqueId)[] Products = 
    [
        (1, "Laptop", 999.99m, new DateTime(2023, 1, 15), new Guid("11111111-1111-1111-1111-111111111111")),
        (2, "Mouse", 25.50m, new DateTime(2023, 6, 10), new Guid("22222222-2222-2222-2222-222222222222")),
        (3, "Discontinued", null, null, null)
    ];

    /// <summary>
    /// Order data: (Id, CustomerId, ProductId, Amount)
    /// </summary>
    public static readonly (int Id, int CustomerId, int ProductId, int Amount)[] Orders = 
    [
        (1, 1, 1, 500),  // John Doe orders Laptop
        (2, 1, 2, 150),  // John Doe orders Mouse
        (3, 2, 1, 300),  // Jane Smith orders Laptop
        (4, 4, 2, 75)    // Senior User orders Mouse
    ];

    /// <summary>
    /// Generate customer tuples as SQL strings
    /// </summary>
    public static string[] CustomerTuples => Customers.Select(c => $"({c.Id}, {c.Age}, '{c.Name}', {(c.IsActive ? 1 : 0)})").ToArray();

    /// <summary>
    /// Generate product tuples as SQL strings
    /// </summary>
    public static string[] ProductTuples => Products.Select(p => 
        $"({p.Id}, '{p.ProductName}', " +
        $"{(p.Price.HasValue ? p.Price.Value.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) : "NULL")}, " +
        $"{(p.CreatedDate.HasValue ? $"'{p.CreatedDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")}, " +
        $"{(p.UniqueId.HasValue ? $"'{p.UniqueId.Value}'" : "NULL")})"
    ).ToArray();

    /// <summary>
    /// Generate product tuples for SQLite (different date format)
    /// </summary>
    public static string[] SqliteProductTuples => Products.Select(p => 
        $"({p.Id}, '{p.ProductName}', " +
        $"{(p.Price.HasValue ? p.Price.Value.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) : "NULL")}, " +
        $"{(p.CreatedDate.HasValue ? $"'{p.CreatedDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")}, " +
        $"{(p.UniqueId.HasValue ? $"'{p.UniqueId.Value}'" : "NULL")})"
    ).ToArray();

    /// <summary>
    /// Generate order tuples as SQL strings
    /// </summary>
    public static string[] OrderTuples => Orders.Select(o => $"({o.Id}, {o.CustomerId}, {o.ProductId}, {o.Amount})").ToArray();
}
