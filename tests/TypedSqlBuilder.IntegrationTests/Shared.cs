using Dapper;

namespace TypedSqlBuilder.IntegrationTests;

/// <summary>
/// Shared POCO classes for Dapper mapping used by both SQLite and SQL Server integration tests
/// </summary>
public class CustomerDto
{
    public int Id { get; set; }
    public int? Age { get; set; }
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
}

public class ProductDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
}

public class OrderDto
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public int Amount { get; set; }
}

/// <summary>
/// Shared extension methods for integration test helpers
/// </summary>
public static class IntegrationTestExtensions
{
    /// <summary>
    /// Converts SQL builder parameters to Dapper parameters
    /// </summary>
    public static DynamicParameters ToDapperParameters(this IReadOnlyDictionary<string, object> parameters)
    {
        var dapperParams = new DynamicParameters();
        foreach (var param in parameters)
        {
            dapperParams.Add(param.Key, param.Value);
        }
        return dapperParams;
    }
}
