using TypedSqlBuilder.Core;
using TypedSqlBuilder.Tests;

// Let's see the actual SQL output for the failing tests
var query4 = TestQueries.InnerJoinWithOrderBy();
var (sql4, parameters4) = query4.ToSqlServerRaw();
Console.WriteLine("InnerJoinWithOrderBy SQL:");
Console.WriteLine(sql4);
Console.WriteLine();

var query5 = TestQueries.LeftJoinWithWhere();
var (sql5, parameters5) = query5.ToSqlServerRaw();
Console.WriteLine("LeftJoinWithWhere SQL:");
Console.WriteLine(sql5);
Console.WriteLine($"Parameters: {string.Join(", ", parameters5.Select(p => $"{p.Key}={p.Value}"))}");
Console.WriteLine();

var query6 = TestQueries.LeftJoinWithOrderBy();
var (sql6, parameters6) = query6.ToSqlServerRaw();
Console.WriteLine("LeftJoinWithOrderBy SQL:");
Console.WriteLine(sql6);
Console.WriteLine();
