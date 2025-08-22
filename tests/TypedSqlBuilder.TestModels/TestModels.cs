using System.ComponentModel.DataAnnotations.Schema;
using TypedSqlBuilder.Core;

namespace TypedSqlBuilder.TestModels;

/// <summary>
/// Sample table definition for testing
/// </summary>
public class Customer() : SqlTable("customers")
{
    [Column("Id")]
    public SqlIntColumn Id { get; set; } = default!;
    [Column("Age")]
    public SqlIntColumn Age { get; set; } = default!;
    [Column("Name")]
    public SqlStringColumn Name { get; set; } = default!;
    [Column("IsActive")]
    public SqlBoolColumn IsActive { get; set; } = default!;
}

/// <summary>
/// Sample table with different column types for comprehensive testing
/// </summary>
public class Product() : SqlTable("products") 
{
    [Column("Id")]
    public SqlIntColumn Id { get; set; } = default!;
    [Column("ProductName")]
    public SqlStringColumn ProductName { get; set; } = default!;
    [Column("Price")]
    public SqlDecimalColumn Price { get; set; } = default!;
    [Column("CreatedDate")]
    public SqlDateTimeColumn CreatedDate { get; set; } = default!;
    [Column("UniqueId")]
    public SqlGuidColumn UniqueId { get; set; } = default!;
}

/// <summary>
/// Sample order table for testing joins with customers
/// </summary>
public class Order() : SqlTable("orders")
{
    [Column("Id")]
    public SqlIntColumn Id { get; set; } = default!;
    [Column("CustomerId")]
    public SqlIntColumn CustomerId { get; set; } = default!;
    [Column("Amount")]
    public SqlIntColumn Amount { get; set; } = default!;
}

/// <summary>
/// Database instance containing static readonly table instances for testing
/// </summary>
public static class Db
{
    public static Customer Customers { get; } = new();
    public static Product Products { get; } = new();
    public static Order Orders { get; } = new();
}
