using TypedSqlBuilder.Core;

namespace TypedSqlBuilder.TestModels;

/// <summary>
/// Sample table definition for testing
/// </summary>
public class Customer() : SqlTable<SqlIntColumn, SqlIntColumn, SqlStringColumn, SqlBoolColumn>("customers")
{
    public SqlIntColumn Id => Column1("Id");
    public SqlIntColumn Age => Column2("Age");
    public SqlStringColumn Name => Column3("Name");
    public SqlBoolColumn IsActive => Column4("IsActive");
}

/// <summary>
/// Sample table with different column types for comprehensive testing
/// </summary>
public class Product() : SqlTable<SqlIntColumn, SqlStringColumn>("products") 
{
    public SqlIntColumn ProductId => Column1("ProductId");
    public SqlStringColumn ProductName => Column2("ProductName");
}

/// <summary>
/// Sample order table for testing joins with customers
/// </summary>
public class Order() : SqlTable<SqlIntColumn, SqlIntColumn, SqlIntColumn>("orders")
{
    public SqlIntColumn OrderId => Column1("OrderId");
    public SqlIntColumn CustomerId => Column2("CustomerId");
    public SqlIntColumn Amount => Column3("Amount");
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
