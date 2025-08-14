using System;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        var files = new[] {
            "/Users/nick.palladinos/Projects/TypedSqlBuilder/tests/TypedSqlBuilder.Tests/SqlServerQueryTests.cs",
            "/Users/nick.palladinos/Projects/TypedSqlBuilder/tests/TypedSqlBuilder.Tests/SqliteQueryTests.cs"
        };

        foreach (var file in files)
        {
            Console.WriteLine($"Processing {file}...");
            ProcessFile(file);
        }
    }

    static void ProcessFile(string filePath)
    {
        var content = File.ReadAllText(filePath);
        
        // Pattern to match Assert.Equal with SQL strings
        var pattern = @"Assert\.Equal\(""(SELECT[^""]+)"", sql\);";
        
        content = Regex.Replace(content, pattern, match =>
        {
            var sqlString = match.Groups[1].Value;
            var formattedSql = ConvertToFormattedSql(sqlString);
            return $"""
                var expectedSql = $$$"""
                {formattedSql}
                """;
                Assert.Equal(expectedSql, sql);
                """;
        });
        
        File.WriteAllText(filePath, content);
    }

    static string ConvertToFormattedSql(string originalSql)
    {
        // Basic conversion patterns
        originalSql = originalSql.Replace("SELECT ", "SELECT \n    ");
        originalSql = originalSql.Replace(" FROM ", "\nFROM ");
        originalSql = originalSql.Replace(" WHERE ", "\nWHERE \n    ");
        originalSql = originalSql.Replace(" ORDER BY ", "\nORDER BY ");
        originalSql = originalSql.Replace(" GROUP BY ", "\nGROBY ");
        originalSql = originalSql.Replace(" HAVING ", "\nHAVING ");
        originalSql = originalSql.Replace(" INNER JOIN ", "\nINNER JOIN ");
        originalSql = originalSql.Replace(" LEFT JOIN ", "\nLEFT JOIN ");
        
        return originalSql;
    }
}
