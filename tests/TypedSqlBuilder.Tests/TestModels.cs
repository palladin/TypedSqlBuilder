namespace TypedSqlBuilder.Tests;

/// <summary>
/// Sample table definition for testing
/// </summary>
public record Customer() : SqlTable<SqlIntColumn, SqlIntColumn, SqlStringColumn>("customers", new("customers", "Id"), new("customers", "Age"), new("customers", "Name"))
{ 
    public SqlIntColumn Id => Column1;
    public SqlIntColumn Age => Column2;
    public SqlStringColumn Name => Column3;
}

/// <summary>
/// Sample table with different column types for comprehensive testing
/// </summary>
public record Product() : SqlTable<SqlIntColumn, SqlStringColumn>("products", new("products", "ProductId"), new("products", "ProductName"))
{
    public SqlIntColumn ProductId => Column1;
    public SqlStringColumn ProductName => Column2;
}
