using TypedSqlBuilder.Core;
using TypedSqlBuilder.TestModels;

// Quick test to see the actual SQL being generated
var customers = Db.Customers.From();

Console.WriteLine("=== SQL Server LIMIT/OFFSET Generation Test ===");

// Test 1: Only limit (null offset)
var limitOnlyQuery = customers.Select(c => c, limitOffset: (10L, null));
Console.WriteLine("LIMIT ONLY (10L, null):");
Console.WriteLine(limitOnlyQuery.ToSql(DatabaseType.SqlServer));
Console.WriteLine();

// Test 2: Limit with offset
var limitWithOffsetQuery = customers.Select(c => c, limitOffset: (5L, 20L));
Console.WriteLine("LIMIT WITH OFFSET (5L, 20L):");
Console.WriteLine(limitWithOffsetQuery.ToSql(DatabaseType.SqlServer));
Console.WriteLine();

// Test 3: With ORDER BY (this should work in SQL Server)
var orderByLimitQuery = customers
    .OrderBy(c => (c.Name, Sort.Asc))
    .Select(c => c, limitOffset: (10L, 5L));
Console.WriteLine("ORDER BY + LIMIT/OFFSET (10L, 5L):");
Console.WriteLine(orderByLimitQuery.ToSql(DatabaseType.SqlServer));
Console.WriteLine();
