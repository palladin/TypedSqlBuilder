using System;
using TypedSqlBuilder.Core;
using TypedSqlBuilder.Tests;

class Program
{
    static void Main()
    {
        var query = TestQueries.InnerJoinWithGroupBy();
        var (sql, parameters) = query.ToSqlServerRaw();
        
        Console.WriteLine("Generated SQL:");
        Console.WriteLine(sql);
        Console.WriteLine();
        Console.WriteLine("Parameters:");
        Console.WriteLine(parameters?.ToString() ?? "None");
    }
}
