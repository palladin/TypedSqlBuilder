using System;
using System.Linq;
using TypedSqlBuilder.Core;
using TypedSqlBuilder.Tests;

var query = TestQueries.FromGroupBySelect();
var (sql, parameters) = query.ToSqlServerRaw();

Console.WriteLine($"SQL: {sql}");
Console.WriteLine($"Parameters: {string.Join(", ", parameters.Select(p => $"{p.Key}={p.Value}"))}");
