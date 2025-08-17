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
    /// Product data: (ProductId, ProductName)
    /// </summary>
    public static readonly (int ProductId, string ProductName)[] Products = 
    [
        (1, "Laptop"),
        (2, "Mouse"),
        (3, "Discontinued")
    ];

    /// <summary>
    /// Order data: (OrderId, CustomerId, Amount)
    /// </summary>
    public static readonly (int OrderId, int CustomerId, int Amount)[] Orders = 
    [
        (1, 1, 500),
        (2, 1, 150),
        (3, 2, 300),
        (4, 4, 75)
    ];

    /// <summary>
    /// Generate customer tuples as SQL strings
    /// </summary>
    public static string[] CustomerTuples => Customers.Select(c => $"({c.Id}, {c.Age}, '{c.Name}', {(c.IsActive ? 1 : 0)})").ToArray();

    /// <summary>
    /// Generate product tuples as SQL strings
    /// </summary>
    public static string[] ProductTuples => Products.Select(p => $"({p.ProductId}, '{p.ProductName}')").ToArray();

    /// <summary>
    /// Generate order tuples as SQL strings
    /// </summary>
    public static string[] OrderTuples => Orders.Select(o => $"({o.OrderId}, {o.CustomerId}, {o.Amount})").ToArray();
}
