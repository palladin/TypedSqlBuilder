using TypedSqlBuilder.Core;
using TypedSqlBuilder.Tests;

var query = TestQueries.FromWhereSelectWhereFromNested();
var (sql, parameters) = query.ToSqliteRaw();
Console.WriteLine($"Generated SQL: {sql}");
foreach (var param in parameters)
{
    Console.WriteLine($"Parameter {param.Key}: {param.Value}");
}
