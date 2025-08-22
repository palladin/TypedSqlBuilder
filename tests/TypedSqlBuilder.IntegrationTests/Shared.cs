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
    public int Id { get; set; }
    public string? ProductName { get; set; }
    public decimal? Price { get; set; }
    public DateTime? CreatedDate { get; set; }
    public Guid? UniqueId { get; set; }
}

public class SqliteProductDto
{
    public int Id { get; set; }
    public string? ProductName { get; set; }
    public decimal? Price { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? UniqueId { get; set; }  // SQLite stores GUID as TEXT
}

public class OrderDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int Amount { get; set; }
}

