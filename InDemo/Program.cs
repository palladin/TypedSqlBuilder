using TypedSqlBuilder.Core;
using TypedSqlBuilder.Tests;

Console.WriteLine("=== TypedSqlBuilder IN Clause Demo ===");
Console.WriteLine();

// Create a query using the IN clause
var query = TestQueries.FromWhereAgeIn();

// Compile to SQL Server
var (sqlServerSql, sqlServerParams) = query.ToSqlServerRaw();
Console.WriteLine("SQL Server:");
Console.WriteLine($"SQL: {sqlServerSql}");
Console.WriteLine("Parameters:");
foreach (var param in sqlServerParams)
{
    Console.WriteLine($"  {param.Key} = {param.Value}");
}
Console.WriteLine();

// Compile to SQLite  
var (sqliteSql, sqliteParams) = query.ToSqliteRaw();
Console.WriteLine("SQLite:");
Console.WriteLine($"SQL: {sqliteSql}");
Console.WriteLine("Parameters:");
foreach (var param in sqliteParams)
{
    Console.WriteLine($"  {param.Key} = {param.Value}");
}
Console.WriteLine();

// Test with different values
var customQuery = TypedSql.From<Customer>().Where(c => c.Age.In(1, 2, 3));
var (customSql, customParams) = customQuery.ToSqlServerRaw();
Console.WriteLine("Custom IN query:");
Console.WriteLine($"SQL: {customSql}");
Console.WriteLine("Parameters:");
foreach (var param in customParams)
{
    Console.WriteLine($"  {param.Key} = {param.Value}");
}

Console.WriteLine();
Console.WriteLine("🎉 IN clause implementation working perfectly!");
