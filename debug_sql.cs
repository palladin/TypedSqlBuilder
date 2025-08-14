using System;
using TypedSqlBuilder.Core;
using TypedSqlBuilder.Tests;

var query = TestQueries.FromWhereAgeInSubquery();
var (sql, parameters) = query.ToSqlServerRaw();

Console.WriteLine("ACTUAL SQL:");
Console.WriteLine($"'{sql}'");
Console.WriteLine();
Console.WriteLine("CHARACTER BY CHARACTER:");
for (int i = 0; i < sql.Length; i++)
{
    char c = sql[i];
    if (c == '\n') Console.Write("\\n");
    else if (c == ' ') Console.Write("·"); // middle dot for spaces
    else Console.Write(c);
}
Console.WriteLine();
