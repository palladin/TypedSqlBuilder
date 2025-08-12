using TypedSqlBuilder.Core;
using TypedSqlBuilder.Tests;

// Test the select(where(select(where(from)))) pattern
var query = TestQueries.FromWhereSelectWhereFromNested();

// Generate SQL Server version
var (sqlServerSql, sqlServerParams) = query.ToSqlServerRaw();
Console.WriteLine("SQL Server SQL:");
Console.WriteLine(sqlServerSql);
Console.WriteLine("\nParameters:");
foreach (var param in sqlServerParams)
{
    Console.WriteLine($"  {param.Key} = {param.Value}");
}

// Generate SQLite version
var (sqliteSql, sqliteParams) = query.ToSqliteRaw();
Console.WriteLine("\n\nSQLite SQL:");
Console.WriteLine(sqliteSql);
Console.WriteLine("\nParameters:");
foreach (var param in sqliteParams)
{
    Console.WriteLine($"  {param.Key} = {param.Value}");
}
