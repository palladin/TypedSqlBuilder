using TypedSqlBuilder.Core;
using TypedSqlBuilder.Tests;

// Demo showcasing the improved compiler that handles SqlNull.Value comparisons
Console.WriteLine("=== Smart Compiler NULL Handling Demo ===\n");

Console.WriteLine("1. Extension methods now use == SqlNull.Value internally:");

// These extension methods now use expr == SqlNull.Value internally
var isNullQuery = TypedSql.From<Customer>()
    .Where(c => c.Name.IsNull());
var (sql1, _) = isNullQuery.ToSqliteRaw();
Console.WriteLine($"   .IsNull(): {sql1}");

var isNotNullQuery = TypedSql.From<Customer>()
    .Where(c => c.Name.IsNotNull());
var (sql2, _) = isNotNullQuery.ToSqliteRaw();
Console.WriteLine($"   .IsNotNull(): {sql2}\n");

Console.WriteLine("2. Direct comparison also works (same result):");

// Direct comparison - compiler detects SqlNull.Value and generates IS NULL
var directIsNull = TypedSql.From<Customer>()
    .Where(c => c.Name == SqlNull.Value);
var (sql3, _) = directIsNull.ToSqliteRaw();
Console.WriteLine($"   == SqlNull.Value: {sql3}");

var directIsNotNull = TypedSql.From<Customer>()
    .Where(c => c.Age != SqlNull.Value);
var (sql4, _) = directIsNotNull.ToSqliteRaw();
Console.WriteLine($"   != SqlNull.Value: {sql4}\n");

Console.WriteLine("3. Assignment still works with SqlNull.Value:");

var updateWithNull = TypedSql.Update<Customer>()
    .Set(c => c.Name, SqlNull.Value)
    .Set(c => c.Age, SqlNull.Value)
    .Where(c => c.Id == 1);
var (sql5, _) = updateWithNull.ToSqliteRaw();
Console.WriteLine($"   UPDATE: {sql5}\n");

Console.WriteLine("4. Mixed comparisons work correctly:");

var complexQuery = TypedSql.From<Customer>()
    .Where(c => (c.Name == SqlNull.Value) || (c.Age != SqlNull.Value && c.Id > 0));
var (sql6, _) = complexQuery.ToSqliteRaw();
Console.WriteLine($"   Complex: {sql6}\n");

Console.WriteLine("=== Compiler intelligently handles all NULL patterns! ===");
