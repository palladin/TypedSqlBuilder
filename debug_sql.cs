using TypedSqlBuilder.Core;
using TypedSqlBuilder.Tests;

// Let's see the actual SQL output for the failing tests
var query1 = TestQueries.InnerJoinWithWhere();
var (sql1, parameters1) = query1.ToSqlServerRaw();
Console.WriteLine("InnerJoinWithWhere SQL:");
Console.WriteLine(sql1);
Console.WriteLine($"Parameters: {string.Join(", ", parameters1.Select(p => $"{p.Key}={p.Value}"))}");
Console.WriteLine();

var query2 = TestQueries.InnerJoinWithGroupBy();
var (sql2, parameters2) = query2.ToSqlServerRaw();
Console.WriteLine("InnerJoinWithGroupBy SQL:");
Console.WriteLine(sql2);
Console.WriteLine($"Parameters: {string.Join(", ", parameters2.Select(p => $"{p.Key}={p.Value}"))}");
Console.WriteLine();

var query3 = TestQueries.LeftJoinWithAggregates();
var (sql3, parameters3) = query3.ToSqlServerRaw();
Console.WriteLine("LeftJoinWithAggregates SQL:");
Console.WriteLine(sql3);
Console.WriteLine($"Parameters: {string.Join(", ", parameters3.Select(p => $"{p.Key}={p.Value}"))}");
