namespace TypedSqlBuilder.Tests;

/// <summary>
/// Sample table definition for testing
/// </summary>
public class Customer() : SqlTable<SqlIntColumn, SqlIntColumn, SqlStringColumn>("customers")
{
    public SqlIntColumn Id => Column1("Id");
    public SqlIntColumn Age => Column2("Age");
    public SqlStringColumn Name => Column3("Name");
}

/// <summary>
/// Sample table with different column types for comprehensive testing
/// </summary>
public class Product() : SqlTable<SqlIntColumn, SqlStringColumn>("products")
{
    public SqlIntColumn ProductId => Column1("ProductId");
    public SqlStringColumn ProductName => Column2("ProductName");
}
