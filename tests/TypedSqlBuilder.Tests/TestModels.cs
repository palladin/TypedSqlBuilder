namespace TypedSqlBuilder.Tests;

/// <summary>
/// Sample table definition for testing
/// </summary>
public class Customer() : SqlTable<SqlIntColumn, SqlIntColumn, SqlStringColumn>("customers", 
    new SqlIntColumn("Id"), 
    new SqlIntColumn("Age"), 
    new SqlStringColumn("Name"))
{
    public SqlIntColumn Id => Column1;
    public SqlIntColumn Age => Column2;
    public SqlStringColumn Name => Column3;
}

/// <summary>
/// Sample table with different column types for comprehensive testing
/// </summary>
public class Product() : SqlTable<SqlIntColumn, SqlStringColumn>("products", 
    new SqlIntColumn("ProductId"), 
    new SqlStringColumn("ProductName"))
{
    public SqlIntColumn ProductId => Column1;
    public SqlStringColumn ProductName => Column2;
}
