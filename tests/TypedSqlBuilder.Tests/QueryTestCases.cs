using TypedSqlBuilder.Core;

namespace TypedSqlBuilder.Tests;

/// <summary>
/// Contains all test case data for QueryTests, organized as KeyValuePair arrays
/// that are combined into a unified TestCases dictionary for cross-database testing.
/// This file centralizes all test data to keep QueryTests.cs focused on test methods.
/// </summary>
public static class QueryTestCases
{
    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] From_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "From_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        """, [])),
        new((DatabaseType.SQLite, "From_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        """, [])),
        new((DatabaseType.PostgreSQL, "From_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] AbsColumn_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "AbsColumn_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                ABS(a0.Age) AS Proj0
            FROM 
                customers a0
            """, [])),
            new((DatabaseType.SQLite, "AbsColumn_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                ABS(a0.Age) AS Proj0
            FROM 
                customers a0
            """, [])),
            new((DatabaseType.PostgreSQL, "AbsColumn_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                ABS(a0.Age) AS Proj0
            FROM 
                customers a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] AbsExpression_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "AbsExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                ABS(a0.Age - @p0) AS Proj0
            FROM 
                customers a0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "AbsExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                ABS(a0.Age - :p0) AS Proj0
            FROM 
                customers a0
            """, [":p0"])),
            new((DatabaseType.PostgreSQL, "AbsExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                ABS(a0.Age - :p0) AS Proj0
            FROM 
                customers a0
            """, [":p0"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] AbsInWhere_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "AbsInWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age AS Age
            FROM 
                customers a0
            WHERE 
                ABS(a0.Age) > @p0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "AbsInWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age AS Age
            FROM 
                customers a0
            WHERE 
                ABS(a0.Age) > :p0
            """, [":p0"])),
            new((DatabaseType.PostgreSQL, "AbsInWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age AS Age
            FROM 
                customers a0
            WHERE 
                ABS(a0.Age) > :p0
            """, [":p0"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] AbsParameter_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "AbsParameter_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age AS Age
            FROM 
                customers a0
            WHERE 
                ABS(a0.Age) > ABS(@minAge)
            """, ["@minAge"])),
            new((DatabaseType.SQLite, "AbsParameter_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age AS Age
            FROM 
                customers a0
            WHERE 
                ABS(a0.Age) > ABS(:minAge)
            """, [":minAge"])),
            new((DatabaseType.PostgreSQL, "AbsParameter_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age AS Age
            FROM 
                customers a0
            WHERE 
                ABS(a0.Age) > ABS(:minAge)
            """, [":minAge"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] BoolColumnDirectComparison_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "BoolColumnDirectComparison_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age AS Age,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.IsActive = @isActive
            """, ["@isActive"])),
            new((DatabaseType.SQLite, "BoolColumnDirectComparison_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age AS Age,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.IsActive = :isActive
            """, [":isActive"])),
            new((DatabaseType.PostgreSQL, "BoolColumnDirectComparison_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age AS Age,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.IsActive = :isActive
            """, [":isActive"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] AvgExpensivePrices_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "AvgExpensivePrices_GeneratesCorrectSql"), ("""
            SELECT 
                AVG(a0.Price) AS Proj0
            FROM 
                products a0
            WHERE 
                a0.Price > @p0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "AvgExpensivePrices_GeneratesCorrectSql"), ("""
            SELECT 
                AVG(a0.Price) AS Proj0
            FROM 
                products a0
            WHERE 
                a0.Price > :p0
            """, [":p0"])),
            new((DatabaseType.PostgreSQL, "AvgExpensivePrices_GeneratesCorrectSql"), ("""
            SELECT 
                AVG(a0.Price) AS Proj0
            FROM 
                products a0
            WHERE 
                a0.Price > :p0
            """, [":p0"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] AvgPrices_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "AvgPrices_GeneratesCorrectSql"), ("""
            SELECT 
                AVG(a0.Price) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.SQLite, "AvgPrices_GeneratesCorrectSql"), ("""
            SELECT 
                AVG(a0.Price) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.PostgreSQL, "AvgPrices_GeneratesCorrectSql"), ("""
            SELECT 
                AVG(a0.Price) AS Proj0
            FROM 
                products a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] MinPrice_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "MinPrice_GeneratesCorrectSql"), ("""
            SELECT 
                MIN(a0.Price) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.SQLite, "MinPrice_GeneratesCorrectSql"), ("""
            SELECT 
                MIN(a0.Price) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.PostgreSQL, "MinPrice_GeneratesCorrectSql"), ("""
            SELECT 
                MIN(a0.Price) AS Proj0
            FROM 
                products a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] MaxPrice_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "MaxPrice_GeneratesCorrectSql"), ("""
            SELECT 
                MAX(a0.Price) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.SQLite, "MaxPrice_GeneratesCorrectSql"), ("""
            SELECT 
                MAX(a0.Price) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.PostgreSQL, "MaxPrice_GeneratesCorrectSql"), ("""
            SELECT 
                MAX(a0.Price) AS Proj0
            FROM 
                products a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] CountActiveCustomers_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "CountActiveCustomers_GeneratesCorrectSql"), ("""
            SELECT 
                COUNT(*) AS Proj0
            FROM 
                customers a0
            WHERE 
                a0.Age >= @p0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "CountActiveCustomers_GeneratesCorrectSql"), ("""
            SELECT 
                COUNT(*) AS Proj0
            FROM 
                customers a0
            WHERE 
                a0.Age >= :p0
            """, [":p0"])),
            new((DatabaseType.PostgreSQL, "CountActiveCustomers_GeneratesCorrectSql"), ("""
            SELECT 
                COUNT(*) AS Proj0
            FROM 
                customers a0
            WHERE 
                a0.Age >= :p0
            """, [":p0"]))
        ];

    
    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] CountCustomers_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "CountCustomers_GeneratesCorrectSql"), ("""
            SELECT 
                COUNT(*) AS Proj0
            FROM 
                customers a0
            """, [])),
            new((DatabaseType.SQLite, "CountCustomers_GeneratesCorrectSql"), ("""
            SELECT 
                COUNT(*) AS Proj0
            FROM 
                customers a0
            """, [])),
            new((DatabaseType.PostgreSQL, "CountCustomers_GeneratesCorrectSql"), ("""
            SELECT 
                COUNT(*) AS Proj0
            FROM 
                customers a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DateTimeAddDays_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "DateTimeAddDays_GeneratesCorrectSql"), ("""
        SELECT 
            DATEADD(day, @p0, a0.CreatedDate) AS Proj0
        FROM 
            products a0
        """, ["@p0"])),
            new((DatabaseType.SQLite, "DateTimeAddDays_GeneratesCorrectSql"), ("""
        SELECT 
            datetime(a0.CreatedDate, '+30 day') AS Proj0
        FROM 
            products a0
        """, [])),
            new((DatabaseType.PostgreSQL, "DateTimeAddDays_GeneratesCorrectSql"), ("""
        SELECT 
            (a0.CreatedDate + INTERVAL '30 day') AS Proj0
        FROM 
            products a0
        """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DateTimeAddMonths_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "DateTimeAddMonths_GeneratesCorrectSql"), ("""
            SELECT 
                DATEADD(month, @p0, a0.CreatedDate) AS Proj0
            FROM 
                products a0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "DateTimeAddMonths_GeneratesCorrectSql"), ("""
            SELECT 
                datetime(a0.CreatedDate, '+6 month') AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.PostgreSQL, "DateTimeAddMonths_GeneratesCorrectSql"), ("""
            SELECT 
                (a0.CreatedDate + INTERVAL '6 month') AS Proj0
            FROM 
                products a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DateTimeAddYears_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "DateTimeAddYears_GeneratesCorrectSql"), ("""
            SELECT 
                DATEADD(year, @p0, a0.CreatedDate) AS Proj0
            FROM 
                products a0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "DateTimeAddYears_GeneratesCorrectSql"), ("""
            SELECT 
                datetime(a0.CreatedDate, '+1 year') AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.PostgreSQL, "DateTimeAddYears_GeneratesCorrectSql"), ("""
            SELECT 
                (a0.CreatedDate + INTERVAL '1 year') AS Proj0
            FROM 
                products a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DateTimeDay_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "DateTimeDay_GeneratesCorrectSql"), ("""
            SELECT 
                DAY(a0.CreatedDate) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.SQLite, "DateTimeDay_GeneratesCorrectSql"), ("""
            SELECT 
                CAST(strftime('%d', a0.CreatedDate) AS INTEGER) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.PostgreSQL, "DateTimeDay_GeneratesCorrectSql"), ("""
            SELECT 
                EXTRACT(DAY FROM a0.CreatedDate) AS Proj0
            FROM 
                products a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DateTimeDiffDays_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "DateTimeDiffDays_GeneratesCorrectSql"), ("""
            SELECT 
                DATEDIFF(day, a0.CreatedDate, @p0) AS Proj0
            FROM 
                products a0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "DateTimeDiffDays_GeneratesCorrectSql"), ("""
            SELECT 
                CAST((julianday(:p0) - julianday(a0.CreatedDate)) AS INTEGER) AS Proj0
            FROM 
                products a0
            """, [":p0"])),
            new((DatabaseType.PostgreSQL, "DateTimeDiffDays_GeneratesCorrectSql"), ("""
            SELECT 
                EXTRACT(DAY FROM (:p0 - a0.CreatedDate)) AS Proj0
            FROM 
                products a0
            """, [":p0"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DateTimeDiffMonths_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "DateTimeDiffMonths_GeneratesCorrectSql"), ("""
            SELECT 
                DATEDIFF(month, a0.CreatedDate, @p0) AS Proj0
            FROM 
                products a0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "DateTimeDiffMonths_GeneratesCorrectSql"), ("""
            SELECT 
                CAST(((CAST(strftime('%Y', :p0) AS INTEGER) - CAST(strftime('%Y', a0.CreatedDate) AS INTEGER)) * 12 + (CAST(strftime('%m', :p0) AS INTEGER) - CAST(strftime('%m', a0.CreatedDate) AS INTEGER))) AS INTEGER) AS Proj0
            FROM 
                products a0
            """, [":p0"])),
            new((DatabaseType.PostgreSQL, "DateTimeDiffMonths_GeneratesCorrectSql"), ("""
            SELECT 
                (EXTRACT(YEAR FROM :p0) - EXTRACT(YEAR FROM a0.CreatedDate)) * 12 + (EXTRACT(MONTH FROM :p0) - EXTRACT(MONTH FROM a0.CreatedDate)) AS Proj0
            FROM 
                products a0
            """, [":p0"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DateTimeDiffYears_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "DateTimeDiffYears_GeneratesCorrectSql"), ("""
            SELECT 
                DATEDIFF(year, a0.CreatedDate, @p0) AS Proj0
            FROM 
                products a0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "DateTimeDiffYears_GeneratesCorrectSql"), ("""
            SELECT 
                CAST((CAST(strftime('%Y', :p0) AS INTEGER) - CAST(strftime('%Y', a0.CreatedDate) AS INTEGER)) AS INTEGER) AS Proj0
            FROM 
                products a0
            """, [":p0"])),
            new((DatabaseType.PostgreSQL, "DateTimeDiffYears_GeneratesCorrectSql"), ("""
            SELECT 
                (EXTRACT(YEAR FROM :p0) - EXTRACT(YEAR FROM a0.CreatedDate)) AS Proj0
            FROM 
                products a0
            """, [":p0"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DateTimeMonth_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "DateTimeMonth_GeneratesCorrectSql"), ("""
            SELECT 
                MONTH(a0.CreatedDate) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.SQLite, "DateTimeMonth_GeneratesCorrectSql"), ("""
            SELECT 
                CAST(strftime('%m', a0.CreatedDate) AS INTEGER) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.PostgreSQL, "DateTimeMonth_GeneratesCorrectSql"), ("""
            SELECT 
                EXTRACT(MONTH FROM a0.CreatedDate) AS Proj0
            FROM 
                products a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DateTimeNow_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "DateTimeNow_GeneratesCorrectSql"), ("""
            SELECT 
                GETDATE() AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.SQLite, "DateTimeNow_GeneratesCorrectSql"), ("""
            SELECT 
                datetime('now') AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.PostgreSQL, "DateTimeNow_GeneratesCorrectSql"), ("""
            SELECT 
                NOW() AS Proj0
            FROM 
                products a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DateTimeYear_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "DateTimeYear_GeneratesCorrectSql"), ("""
            SELECT 
                YEAR(a0.CreatedDate) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.SQLite, "DateTimeYear_GeneratesCorrectSql"), ("""
            SELECT 
                CAST(strftime('%Y', a0.CreatedDate) AS INTEGER) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.PostgreSQL, "DateTimeYear_GeneratesCorrectSql"), ("""
            SELECT 
                EXTRACT(YEAR FROM a0.CreatedDate) AS Proj0
            FROM 
                products a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DecimalCeiling_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "DecimalCeiling_GeneratesCorrectSql"), ("""
            SELECT 
                CEILING(a0.Price) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.SQLite, "DecimalCeiling_GeneratesCorrectSql"), ("""
            SELECT 
                CAST((CASE WHEN a0.Price = CAST(a0.Price AS INTEGER) THEN a0.Price ELSE CAST(a0.Price AS INTEGER) + 1 END) AS REAL) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.PostgreSQL, "DecimalCeiling_GeneratesCorrectSql"), ("""
            SELECT 
                CEIL(a0.Price) AS Proj0
            FROM 
                products a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DecimalFloor_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "DecimalFloor_GeneratesCorrectSql"), ("""
            SELECT 
                FLOOR(a0.Price) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.SQLite, "DecimalFloor_GeneratesCorrectSql"), ("""
            SELECT 
                CAST(CAST(a0.Price AS INTEGER) AS REAL) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.PostgreSQL, "DecimalFloor_GeneratesCorrectSql"), ("""
            SELECT 
                FLOOR(a0.Price) AS Proj0
            FROM 
                products a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DecimalRound_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "DecimalRound_GeneratesCorrectSql"), ("""
        SELECT 
            ROUND(a0.Price, @p0) AS Proj0
        FROM 
            products a0
        """, ["@p0"])),
            new((DatabaseType.SQLite, "DecimalRound_GeneratesCorrectSql"), ("""
        SELECT 
            ROUND(a0.Price, :p0) AS Proj0
        FROM 
            products a0
        """, [":p0"])),
            new((DatabaseType.PostgreSQL, "DecimalRound_GeneratesCorrectSql"), ("""
        SELECT 
            ROUND(a0.Price, :p0) AS Proj0
        FROM 
            products a0
        """, [":p0"]))
        ];

    
    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] SumAges_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "SumAges_GeneratesCorrectSql"), ("""
            SELECT 
                SUM(a0.Age) AS Proj0
            FROM 
                customers a0
            """, [])),
            new((DatabaseType.SQLite, "SumAges_GeneratesCorrectSql"), ("""
            SELECT 
                SUM(a0.Age) AS Proj0
            FROM 
                customers a0
            """, [])),
            new((DatabaseType.PostgreSQL, "SumAges_GeneratesCorrectSql"), ("""
            SELECT 
                SUM(a0.Age) AS Proj0
            FROM 
                customers a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] SumExpensivePrices_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "SumExpensivePrices_GeneratesCorrectSql"), ("""
            SELECT 
                SUM(a0.Price) AS Proj0
            FROM 
                products a0
            WHERE 
                a0.Price > @p0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "SumExpensivePrices_GeneratesCorrectSql"), ("""
            SELECT 
                SUM(a0.Price) AS Proj0
            FROM 
                products a0
            WHERE 
                a0.Price > :p0
            """, [":p0"])),
            new((DatabaseType.PostgreSQL, "SumExpensivePrices_GeneratesCorrectSql"), ("""
            SELECT 
                SUM(a0.Price) AS Proj0
            FROM 
                products a0
            WHERE 
                a0.Price > :p0
            """, [":p0"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] SumPrices_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "SumPrices_GeneratesCorrectSql"), ("""
            SELECT 
                SUM(a0.Price) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.SQLite, "SumPrices_GeneratesCorrectSql"), ("""
            SELECT 
                SUM(a0.Price) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.PostgreSQL, "SumPrices_GeneratesCorrectSql"), ("""
            SELECT 
                SUM(a0.Price) AS Proj0
            FROM 
                products a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] StringFunctionsInSelect_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "StringFunctionsInSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            UPPER(a0.Name) AS UpperName,
            LOWER(a0.Name) AS LowerName,
            TRIM(a0.Name) AS TrimmedName,
            LEN(a0.Name) AS NameLength,
            SUBSTRING(a0.Name, @p0, @p1) AS FirstThree
        FROM 
            customers a0
        """, ["p0", "p1"])),
            new((DatabaseType.SQLite, "StringFunctionsInSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            UPPER(a0.Name) AS UpperName,
            LOWER(a0.Name) AS LowerName,
            TRIM(a0.Name) AS TrimmedName,
            LENGTH(a0.Name) AS NameLength,
            SUBSTR(a0.Name, :p0, :p1) AS FirstThree
        FROM 
            customers a0
        """, ["p0", "p1"])),
            new((DatabaseType.PostgreSQL, "StringFunctionsInSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            UPPER(a0.Name) AS UpperName,
            LOWER(a0.Name) AS LowerName,
            TRIM(a0.Name) AS TrimmedName,
            LENGTH(a0.Name) AS NameLength,
            SUBSTRING(a0.Name, :p0, :p1) AS FirstThree
        FROM 
            customers a0
        """, ["p0", "p1"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] StringFunctionsInWhere_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "StringFunctionsInWhere_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            UPPER(a0.Name) = @p0 AND LEN(a0.Name) > @p1
        """, ["@p0", "@p1"])),
            new((DatabaseType.SQLite, "StringFunctionsInWhere_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            UPPER(a0.Name) = :p0 AND LENGTH(a0.Name) > :p1
        """, [":p0", ":p1"])),
            new((DatabaseType.PostgreSQL, "StringFunctionsInWhere_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        WHERE 
            UPPER(a0.Name) = :p0 AND LENGTH(a0.Name) > :p1
        """, [":p0", ":p1"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] StringLength_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "StringLength_GeneratesCorrectSql"), ("""
            SELECT 
                LEN(a0.ProductName) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.SQLite, "StringLength_GeneratesCorrectSql"), ("""
            SELECT 
                LENGTH(a0.ProductName) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.PostgreSQL, "StringLength_GeneratesCorrectSql"), ("""
            SELECT 
                LENGTH(a0.ProductName) AS Proj0
            FROM 
                products a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] StringLower_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "StringLower_GeneratesCorrectSql"), ("""
        SELECT 
            LOWER(a0.ProductName) AS Proj0
        FROM 
            products a0
        """, [])),
            new((DatabaseType.SQLite, "StringLower_GeneratesCorrectSql"), ("""
        SELECT 
            LOWER(a0.ProductName) AS Proj0
        FROM 
            products a0
        """, [])),
            new((DatabaseType.PostgreSQL, "StringLower_GeneratesCorrectSql"), ("""
        SELECT 
            LOWER(a0.ProductName) AS Proj0
        FROM 
            products a0
        """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] StringSubstring_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "StringSubstring_GeneratesCorrectSql"), ("""
            SELECT 
                SUBSTRING(a0.ProductName, @p0, @p1) AS Proj0
            FROM 
                products a0
            """, ["@p0", "@p1"])),
            new((DatabaseType.SQLite, "StringSubstring_GeneratesCorrectSql"), ("""
            SELECT 
                SUBSTR(a0.ProductName, :p0, :p1) AS Proj0
            FROM 
                products a0
            """, [":p0", ":p1"])),
            new((DatabaseType.PostgreSQL, "StringSubstring_GeneratesCorrectSql"), ("""
            SELECT 
                SUBSTRING(a0.ProductName, :p0, :p1) AS Proj0
            FROM 
                products a0
            """, [":p0", ":p1"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] StringTrim_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "StringTrim_GeneratesCorrectSql"), ("""
            SELECT 
                TRIM(a0.ProductName) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.SQLite, "StringTrim_GeneratesCorrectSql"), ("""
            SELECT 
                TRIM(a0.ProductName) AS Proj0
            FROM 
                products a0
            """, [])),
            new((DatabaseType.PostgreSQL, "StringTrim_GeneratesCorrectSql"), ("""
            SELECT 
                TRIM(a0.ProductName) AS Proj0
            FROM 
                products a0
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] StringUpper_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "StringUpper_GeneratesCorrectSql"), ("""
        SELECT 
            UPPER(a0.ProductName) AS Proj0
        FROM 
            products a0
        """, [])),
            new((DatabaseType.SQLite, "StringUpper_GeneratesCorrectSql"), ("""
        SELECT 
            UPPER(a0.ProductName) AS Proj0
        FROM 
            products a0
        """, [])),
            new((DatabaseType.PostgreSQL, "StringUpper_GeneratesCorrectSql"), ("""
        SELECT 
            UPPER(a0.ProductName) AS Proj0
        FROM 
            products a0
        """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DateTimeFunctionsInSelect_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "DateTimeFunctionsInSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            YEAR(a0.CreatedDate) AS CreatedYear,
            MONTH(a0.CreatedDate) AS CreatedMonth,
            DAY(a0.CreatedDate) AS CreatedDay,
            DATEADD(day, @p0, a0.CreatedDate) AS NextWeek,
            DATEADD(month, @p1, a0.CreatedDate) AS NextMonth,
            DATEDIFF(day, a0.CreatedDate, GETDATE()) AS DaysAgo
        FROM 
            products a0
        """, ["@p0", "@p1"])),
            new((DatabaseType.SQLite, "DateTimeFunctionsInSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            CAST(strftime('%Y', a0.CreatedDate) AS INTEGER) AS CreatedYear,
            CAST(strftime('%m', a0.CreatedDate) AS INTEGER) AS CreatedMonth,
            CAST(strftime('%d', a0.CreatedDate) AS INTEGER) AS CreatedDay,
            datetime(a0.CreatedDate, '+7 day') AS NextWeek,
            datetime(a0.CreatedDate, '+1 month') AS NextMonth,
            CAST((julianday(datetime('now')) - julianday(a0.CreatedDate)) AS INTEGER) AS DaysAgo
        FROM 
            products a0
        """, [])),
            new((DatabaseType.PostgreSQL, "DateTimeFunctionsInSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            EXTRACT(YEAR FROM a0.CreatedDate) AS CreatedYear,
            EXTRACT(MONTH FROM a0.CreatedDate) AS CreatedMonth,
            EXTRACT(DAY FROM a0.CreatedDate) AS CreatedDay,
            (a0.CreatedDate + INTERVAL '7 day') AS NextWeek,
            (a0.CreatedDate + INTERVAL '1 month') AS NextMonth,
            EXTRACT(DAY FROM (NOW() - a0.CreatedDate)) AS DaysAgo
        FROM 
            products a0
        """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DateTimeFunctionsInWhere_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "DateTimeFunctionsInWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.CreatedDate AS CreatedDate
            FROM 
                products a0
            WHERE 
                YEAR(a0.CreatedDate) = @p0 AND MONTH(a0.CreatedDate) > @p1
            """, ["@p0", "@p1"])),
            new((DatabaseType.SQLite, "DateTimeFunctionsInWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.CreatedDate AS CreatedDate
            FROM 
                products a0
            WHERE 
                CAST(strftime('%Y', a0.CreatedDate) AS INTEGER) = :p0 AND CAST(strftime('%m', a0.CreatedDate) AS INTEGER) > :p1
            """, [":p0", ":p1"])),
            new((DatabaseType.PostgreSQL, "DateTimeFunctionsInWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.CreatedDate AS CreatedDate
            FROM 
                products a0
            WHERE 
                EXTRACT(YEAR FROM a0.CreatedDate) = :p0 AND EXTRACT(MONTH FROM a0.CreatedDate) > :p1
            """, [":p0", ":p1"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereString_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "FromWhereString_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Name = @p0
        """, ["@p0"])),
            new((DatabaseType.SQLite, "FromWhereString_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Name = :p0
        """, [":p0"])),
            new((DatabaseType.PostgreSQL, "FromWhereString_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Name = :p0
        """, [":p0"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereUniqueIdEquals_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "FromWhereUniqueIdEquals_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.UniqueId = @p0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "FromWhereUniqueIdEquals_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.UniqueId = :p0
            """, [":p0"])),
            new((DatabaseType.PostgreSQL, "FromWhereUniqueIdEquals_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.UniqueId = :p0
            """, [":p0"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereUniqueIdIsNotNull_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "FromWhereUniqueIdIsNotNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.UniqueId IS NOT NULL
            """, [])),
            new((DatabaseType.SQLite, "FromWhereUniqueIdIsNotNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.UniqueId IS NOT NULL
            """, [])),
            new((DatabaseType.PostgreSQL, "FromWhereUniqueIdIsNotNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.UniqueId IS NOT NULL
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereUniqueIdIsNull_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "FromWhereUniqueIdIsNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.UniqueId IS NULL
            """, [])),
            new((DatabaseType.SQLite, "FromWhereUniqueIdIsNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.UniqueId IS NULL
            """, [])),
            new((DatabaseType.PostgreSQL, "FromWhereUniqueIdIsNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.UniqueId IS NULL
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereUniqueIdNotEquals_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "FromWhereUniqueIdNotEquals_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.ProductName AS ProductName,
            a0.Price AS Price,
            a0.CreatedDate AS CreatedDate,
            a0.UniqueId AS UniqueId
        FROM 
            products a0
        WHERE 
            a0.UniqueId != @p0
        """, ["@p0"])),
            new((DatabaseType.SQLite, "FromWhereUniqueIdNotEquals_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.ProductName AS ProductName,
            a0.Price AS Price,
            a0.CreatedDate AS CreatedDate,
            a0.UniqueId AS UniqueId
        FROM 
            products a0
        WHERE 
            a0.UniqueId != :p0
        """, [":p0"])),
            new((DatabaseType.PostgreSQL, "FromWhereUniqueIdNotEquals_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.ProductName AS ProductName,
            a0.Price AS Price,
            a0.CreatedDate AS CreatedDate,
            a0.UniqueId AS UniqueId
        FROM 
            products a0
        WHERE 
            a0.UniqueId != :p0
        """, [":p0"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] InnerJoinBasic_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "InnerJoinBasic_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a1.Amount AS Amount
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            """, [])),
            new((DatabaseType.SQLite, "InnerJoinBasic_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a1.Amount AS Amount
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            """, [])),
            new((DatabaseType.PostgreSQL, "InnerJoinBasic_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a1.Amount AS Amount
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] InnerJoinWithSelect_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "InnerJoinWithSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Name AS Name,
                a1.Amount AS Amount
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            """, [])),
            new((DatabaseType.SQLite, "InnerJoinWithSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Name AS Name,
                a1.Amount AS Amount
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            """, [])),
            new((DatabaseType.PostgreSQL, "InnerJoinWithSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Name AS Name,
                a1.Amount AS Amount
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] InnerJoinWithWhere_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "InnerJoinWithWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a1.Id AS CustomerId,
                a1.Name AS Name,
                a1.Age AS Age,
                a2.Id AS OrderId,
                a2.Amount AS Amount
            FROM 
                (SELECT 
                    a0.Id AS Id,
                    a0.Age AS Age,
                    a0.Name AS Name,
                    a0.IsActive AS IsActive
                FROM 
                    customers a0
                WHERE 
                    a0.Age >= @p0) a1
            INNER JOIN orders a2 ON a1.Id = a2.CustomerId
            WHERE 
                a2.Amount > @p1
            """, ["@p0", "@p1"])),
            new((DatabaseType.SQLite, "InnerJoinWithWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a1.Id AS CustomerId,
                a1.Name AS Name,
                a1.Age AS Age,
                a2.Id AS OrderId,
                a2.Amount AS Amount
            FROM 
                (SELECT 
                    a0.Id AS Id,
                    a0.Age AS Age,
                    a0.Name AS Name,
                    a0.IsActive AS IsActive
                FROM 
                    customers a0
                WHERE 
                    a0.Age >= :p0) a1
            INNER JOIN orders a2 ON a1.Id = a2.CustomerId
            WHERE 
                a2.Amount > :p1
            """, [":p0", ":p1"])),
            new((DatabaseType.PostgreSQL, "InnerJoinWithWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a1.Id AS CustomerId,
                a1.Name AS Name,
                a1.Age AS Age,
                a2.Id AS OrderId,
                a2.Amount AS Amount
            FROM 
                (SELECT 
                    a0.Id AS Id,
                    a0.Age AS Age,
                    a0.Name AS Name,
                    a0.IsActive AS IsActive
                FROM 
                    customers a0
                WHERE 
                    a0.Age >= :p0) a1
            INNER JOIN orders a2 ON a1.Id = a2.CustomerId
            WHERE 
                a2.Amount > :p1
            """, [":p0", ":p1"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] InnerJoinWithOrderBy_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "InnerJoinWithOrderBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a1.Amount AS Amount
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            ORDER BY 
                a0.Name ASC
            """, [])),
            new((DatabaseType.SQLite, "InnerJoinWithOrderBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a1.Amount AS Amount
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            ORDER BY 
                a0.Name ASC
            """, [])),
            new((DatabaseType.PostgreSQL, "InnerJoinWithOrderBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a1.Amount AS Amount
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            ORDER BY 
                a0.Name ASC
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] InnerJoinWithGroupBy_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "InnerJoinWithGroupBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS CustomerName,
                SUM(a1.Amount) AS TotalAmount
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            GROUP BY 
                a0.Id, a0.Name
            """, [])),
            new((DatabaseType.SQLite, "InnerJoinWithGroupBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS CustomerName,
                SUM(a1.Amount) AS TotalAmount
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            GROUP BY 
                a0.Id, a0.Name
            """, [])),
            new((DatabaseType.PostgreSQL, "InnerJoinWithGroupBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS CustomerName,
                SUM(a1.Amount) AS TotalAmount
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            GROUP BY 
                a0.Id, a0.Name
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] LeftJoinBasic_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "LeftJoinBasic_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a1.Amount AS Amount
            FROM 
                customers a0
            LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
            """, [])),
            new((DatabaseType.SQLite, "LeftJoinBasic_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a1.Amount AS Amount
            FROM 
                customers a0
            LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
            """, [])),
            new((DatabaseType.PostgreSQL, "LeftJoinBasic_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a1.Amount AS Amount
            FROM 
                customers a0
            LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] LikeWildcard_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "LikeWildcard_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Name LIKE @p0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "LikeWildcard_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Name LIKE :p0
            """, [":p0"])),
            new((DatabaseType.PostgreSQL, "LikeWildcard_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Name LIKE :p0
            """, [":p0"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] LikeBothWildcards_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "LikeBothWildcards_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Name LIKE @p0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "LikeBothWildcards_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Name LIKE :p0
            """, [":p0"])),
            new((DatabaseType.PostgreSQL, "LikeBothWildcards_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Name LIKE :p0
            """, [":p0"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] LikeExact_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "LikeExact_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Name LIKE @p0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "LikeExact_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Name LIKE :p0
            """, [":p0"])),
            new((DatabaseType.PostgreSQL, "LikeExact_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Name LIKE :p0
            """, [":p0"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] LikeSingleChar_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "LikeSingleChar_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Name LIKE @p0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "LikeSingleChar_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Name LIKE :p0
            """, [":p0"])),
            new((DatabaseType.PostgreSQL, "LikeSingleChar_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Name LIKE :p0
            """, [":p0"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] MathFunctionsInSelect_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "MathFunctionsInSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Price AS OriginalPrice,
                ROUND(a0.Price, @p0) AS RoundedPrice,
                CEILING(a0.Price) AS CeilingPrice,
                FLOOR(a0.Price) AS FloorPrice
            FROM 
                products a0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "MathFunctionsInSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Price AS OriginalPrice,
                ROUND(a0.Price, :p0) AS RoundedPrice,
                CAST((CASE WHEN a0.Price = CAST(a0.Price AS INTEGER) THEN a0.Price ELSE CAST(a0.Price AS INTEGER) + 1 END) AS REAL) AS CeilingPrice,
                CAST(CAST(a0.Price AS INTEGER) AS REAL) AS FloorPrice
            FROM 
                products a0
            """, [":p0"])),
            new((DatabaseType.PostgreSQL, "MathFunctionsInSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Price AS OriginalPrice,
                ROUND(a0.Price, :p0) AS RoundedPrice,
                CEIL(a0.Price) AS CeilingPrice,
                FLOOR(a0.Price) AS FloorPrice
            FROM 
                products a0
            """, [":p0"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] MathFunctionsInWhere_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "MathFunctionsInWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Price AS Price
            FROM 
                products a0
            WHERE 
                ROUND(a0.Price, @p0) > @p1 AND CEILING(a0.Price) < @p2
            """, ["@p0", "@p1", "@p2"])),
            new((DatabaseType.SQLite, "MathFunctionsInWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Price AS Price
            FROM 
                products a0
            WHERE 
                ROUND(a0.Price, :p0) > :p1 AND CAST((CASE WHEN a0.Price = CAST(a0.Price AS INTEGER) THEN a0.Price ELSE CAST(a0.Price AS INTEGER) + 1 END) AS REAL) < :p2
            """, [":p0", ":p1", ":p2"])),
            new((DatabaseType.PostgreSQL, "MathFunctionsInWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Price AS Price
            FROM 
                products a0
            WHERE 
                ROUND(a0.Price, :p0) > :p1 AND CEIL(a0.Price) < :p2
            """, [":p0", ":p1", ":p2"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] MixedJoinTypesFusion_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "MixedJoinTypesFusion_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a2.ProductName AS ProductName
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            LEFT JOIN products a2 ON a1.Amount = a2.Id
            """, [])),
            new((DatabaseType.SQLite, "MixedJoinTypesFusion_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a2.ProductName AS ProductName
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            LEFT JOIN products a2 ON a1.Amount = a2.Id
            """, [])),
            new((DatabaseType.PostgreSQL, "MixedJoinTypesFusion_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a2.ProductName AS ProductName
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            LEFT JOIN products a2 ON a1.Amount = a2.Id
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] MultipleInnerJoinsFusion_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "MultipleInnerJoinsFusion_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a2.ProductName AS ProductName
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            INNER JOIN products a2 ON a1.Amount = a2.Id
            """, [])),
            new((DatabaseType.SQLite, "MultipleInnerJoinsFusion_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a2.ProductName AS ProductName
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            INNER JOIN products a2 ON a1.Amount = a2.Id
            """, [])),
            new((DatabaseType.PostgreSQL, "MultipleInnerJoinsFusion_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a2.ProductName AS ProductName
            FROM 
                customers a0
            INNER JOIN orders a1 ON a0.Id = a1.CustomerId
            INNER JOIN products a2 ON a1.Amount = a2.Id
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] ParameterAsBoolParam_GeneratesCorrectSqlPair =
        [
            // NOTE: Only SQLite supported for boolean expression comparison
            // SQL Server and PostgreSQL excluded due to complex boolean parameter handling requirements
            new((DatabaseType.SQLite, "ParameterAsBoolParam_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age AS Age
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0 = :isAdult
            """, [":p0", ":isAdult"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] ParameterAsDateTimeParam_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "ParameterAsDateTimeParam_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.CreatedDate > @startDate
            """, ["@startDate"])),
            new((DatabaseType.SQLite, "ParameterAsDateTimeParam_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.CreatedDate > :startDate
            """, [":startDate"])),
            new((DatabaseType.PostgreSQL, "ParameterAsDateTimeParam_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.CreatedDate > :startDate
            """, [":startDate"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] ParameterAsDecimalParam_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "ParameterAsDecimalParam_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.Price > @minPrice
            """, ["@minPrice"])),
            new((DatabaseType.SQLite, "ParameterAsDecimalParam_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.Price > :minPrice
            """, [":minPrice"])),
            new((DatabaseType.PostgreSQL, "ParameterAsDecimalParam_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.Price > :minPrice
            """, [":minPrice"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] ParameterAsGuidParam_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "ParameterAsGuidParam_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.UniqueId = @targetId
            """, ["@targetId"])),
            new((DatabaseType.SQLite, "ParameterAsGuidParam_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.UniqueId = :targetId
            """, [":targetId"])),
            new((DatabaseType.PostgreSQL, "ParameterAsGuidParam_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.UniqueId = :targetId
            """, [":targetId"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] ParameterAsIntParam_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "ParameterAsIntParam_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age > @minAge
            """, ["@minAge"])),
            new((DatabaseType.SQLite, "ParameterAsIntParam_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age > :minAge
            """, [":minAge"])),
            new((DatabaseType.PostgreSQL, "ParameterAsIntParam_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age > :minAge
            """, [":minAge"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] ParameterAsStringParam_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "ParameterAsStringParam_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age
            FROM 
                customers a0
            WHERE 
                a0.Name = @customerName
            """, ["@customerName"])),
            new((DatabaseType.SQLite, "ParameterAsStringParam_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age
            FROM 
                customers a0
            WHERE 
                a0.Name = :customerName
            """, [":customerName"])),
            new((DatabaseType.PostgreSQL, "ParameterAsStringParam_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age
            FROM 
                customers a0
            WHERE 
                a0.Name = :customerName
            """, [":customerName"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] JoinFusionWithWhere_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "JoinFusionWithWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a1.Id AS Id,
                a1.Name AS Name,
                a2.Amount AS Amount,
                a3.ProductName AS ProductName
            FROM 
                (SELECT 
                    a0.Id AS Id,
                    a0.Age AS Age,
                    a0.Name AS Name,
                    a0.IsActive AS IsActive
                FROM 
                    customers a0
                WHERE 
                    a0.Age >= @p0) a1
            INNER JOIN orders a2 ON a1.Id = a2.CustomerId
            INNER JOIN products a3 ON a2.Amount = a3.Id
            WHERE 
                a2.Amount > @p1
            """, ["@p0", "@p1"])),
            new((DatabaseType.SQLite, "JoinFusionWithWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a1.Id AS Id,
                a1.Name AS Name,
                a2.Amount AS Amount,
                a3.ProductName AS ProductName
            FROM 
                (SELECT 
                    a0.Id AS Id,
                    a0.Age AS Age,
                    a0.Name AS Name,
                    a0.IsActive AS IsActive
                FROM 
                    customers a0
                WHERE 
                    a0.Age >= :p0) a1
            INNER JOIN orders a2 ON a1.Id = a2.CustomerId
            INNER JOIN products a3 ON a2.Amount = a3.Id
            WHERE 
                a2.Amount > :p1
            """, [":p0", ":p1"])),
            new((DatabaseType.PostgreSQL, "JoinFusionWithWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a1.Id AS Id,
                a1.Name AS Name,
                a2.Amount AS Amount,
                a3.ProductName AS ProductName
            FROM 
                (SELECT 
                    a0.Id AS Id,
                    a0.Age AS Age,
                    a0.Name AS Name,
                    a0.IsActive AS IsActive
                FROM 
                    customers a0
                WHERE 
                    a0.Age >= :p0) a1
            INNER JOIN orders a2 ON a1.Id = a2.CustomerId
            INNER JOIN products a3 ON a2.Amount = a3.Id
            WHERE 
                a2.Amount > :p1
            """, [":p0", ":p1"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] LeftJoinWithAggregates_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "LeftJoinWithAggregates_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                COUNT(*) AS OrderCount,
                SUM(a1.Amount) AS TotalSpent
            FROM 
                customers a0
            LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
            GROUP BY 
                a0.Id
            """, [])),
            new((DatabaseType.SQLite, "LeftJoinWithAggregates_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                COUNT(*) AS OrderCount,
                SUM(a1.Amount) AS TotalSpent
            FROM 
                customers a0
            LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
            GROUP BY 
                a0.Id
            """, [])),
            new((DatabaseType.PostgreSQL, "LeftJoinWithAggregates_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                COUNT(*) AS OrderCount,
                SUM(a1.Amount) AS TotalSpent
            FROM 
                customers a0
            LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
            GROUP BY 
                a0.Id
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] LeftJoinWithOrderBy_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "LeftJoinWithOrderBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a1.Amount AS Amount
            FROM 
                customers a0
            LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
            ORDER BY 
                a0.Name ASC, a1.Amount DESC
            """, [])),
            new((DatabaseType.SQLite, "LeftJoinWithOrderBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a1.Amount AS Amount
            FROM 
                customers a0
            LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
            ORDER BY 
                a0.Name ASC, a1.Amount DESC
            """, [])),
            new((DatabaseType.PostgreSQL, "LeftJoinWithOrderBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS Name,
                a1.Id AS OrderId,
                a1.Amount AS Amount
            FROM 
                customers a0
            LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
            ORDER BY 
                a0.Name ASC, a1.Amount DESC
            """, []))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] LeftJoinWithSelect_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "LeftJoinWithSelect_GeneratesCorrectSql"), ("""
            SELECT 
                CONCAT(a0.Name, @p0) AS Proj0,
                a1.Amount AS Amount
            FROM 
                customers a0
            LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
            """, ["@p0"])),
            new((DatabaseType.SQLite, "LeftJoinWithSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Name || :p0 AS Proj0,
                a1.Amount AS Amount
            FROM 
                customers a0
            LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
            """, [":p0"])),
            new((DatabaseType.PostgreSQL, "LeftJoinWithSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Name || :p0 AS Proj0,
                a1.Amount AS Amount
            FROM 
                customers a0
            LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
            """, [":p0"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] LeftJoinWithWhere_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "LeftJoinWithWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a1.Id AS CustomerId,
                a1.Name AS Name,
                a1.Age AS Age,
                a2.Id AS OrderId,
                a2.Amount AS Amount
            FROM 
                (SELECT 
                    a0.Id AS Id,
                    a0.Age AS Age,
                    a0.Name AS Name,
                    a0.IsActive AS IsActive
                FROM 
                    customers a0
                WHERE 
                    a0.Age >= @p0) a1
            LEFT JOIN orders a2 ON a1.Id = a2.CustomerId
            WHERE 
                a1.Age < @p1
            """, ["@p0", "@p1"])),
            new((DatabaseType.SQLite, "LeftJoinWithWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a1.Id AS CustomerId,
                a1.Name AS Name,
                a1.Age AS Age,
                a2.Id AS OrderId,
                a2.Amount AS Amount
            FROM 
                (SELECT 
                    a0.Id AS Id,
                    a0.Age AS Age,
                    a0.Name AS Name,
                    a0.IsActive AS IsActive
                FROM 
                    customers a0
                WHERE 
                    a0.Age >= :p0) a1
            LEFT JOIN orders a2 ON a1.Id = a2.CustomerId
            WHERE 
                a1.Age < :p1
            """, [":p0", ":p1"])),
            new((DatabaseType.PostgreSQL, "LeftJoinWithWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a1.Id AS CustomerId,
                a1.Name AS Name,
                a1.Age AS Age,
                a2.Id AS OrderId,
                a2.Amount AS Amount
            FROM 
                (SELECT 
                    a0.Id AS Id,
                    a0.Age AS Age,
                    a0.Name AS Name,
                    a0.IsActive AS IsActive
                FROM 
                    customers a0
                WHERE 
                    a0.Age >= :p0) a1
            LEFT JOIN orders a2 ON a1.Id = a2.CustomerId
            WHERE 
                a1.Age < :p1
            """, [":p0", ":p1"]))
        ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] BoolColumnLiteralFalse_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "BoolColumnLiteralFalse_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age AS Age,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.IsActive = @p0
            """, ["@p0"])),
        new((DatabaseType.SQLite, "BoolColumnLiteralFalse_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age AS Age,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.IsActive = :p0
            """, [":p0"])),
        new((DatabaseType.PostgreSQL, "BoolColumnLiteralFalse_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age AS Age,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.IsActive = false
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] BoolColumnLiteralTrue_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "BoolColumnLiteralTrue_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age AS Age,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.IsActive = @p0
            """, ["@p0"])),
        new((DatabaseType.SQLite, "BoolColumnLiteralTrue_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age AS Age,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.IsActive = :p0
            """, [":p0"])),
        new((DatabaseType.PostgreSQL, "BoolColumnLiteralTrue_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age AS Age,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.IsActive = true
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] CaseBoolExpression_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "CaseBoolExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                CASE WHEN a0.Age > @p0 THEN a0.IsActive ELSE @p1 END AS Proj0
            FROM 
                customers a0
            """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "CaseBoolExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                CASE WHEN a0.Age > :p0 THEN a0.IsActive ELSE :p1 END AS Proj0
            FROM 
                customers a0
            """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "CaseBoolExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                CASE WHEN a0.Age > :p0 THEN a0.IsActive ELSE false END AS Proj0
            FROM 
                customers a0
            """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] CaseDateTimeExpression_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "CaseDateTimeExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                CASE WHEN a0.CreatedDate < @p0 THEN @p1 ELSE CASE WHEN a0.CreatedDate < @p2 THEN @p3 ELSE @p4 END END AS Age
            FROM 
                products a0
            """, ["@p0", "@p1", "@p2", "@p3", "@p4"])),
        new((DatabaseType.SQLite, "CaseDateTimeExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                CASE WHEN a0.CreatedDate < :p0 THEN :p1 ELSE CASE WHEN a0.CreatedDate < :p2 THEN :p3 ELSE :p4 END END AS Age
            FROM 
                products a0
            """, [":p0", ":p1", ":p2", ":p3", ":p4"])),
        new((DatabaseType.PostgreSQL, "CaseDateTimeExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                CASE WHEN a0.CreatedDate < :p0 THEN :p1 ELSE CASE WHEN a0.CreatedDate < :p2 THEN :p3 ELSE :p4 END END AS Age
            FROM 
                products a0
            """, [":p0", ":p1", ":p2", ":p3", ":p4"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] CaseDecimalExpression_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "CaseDecimalExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                CASE WHEN a0.Price > @p0 THEN @p1 ELSE CASE WHEN a0.Price > @p2 THEN @p3 ELSE @p4 END END AS ExpensiveFlag
            FROM 
                products a0
            """, ["@p0", "@p1", "@p2", "@p3", "@p4"])),
        new((DatabaseType.SQLite, "CaseDecimalExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                CASE WHEN a0.Price > :p0 THEN :p1 ELSE CASE WHEN a0.Price > :p2 THEN :p3 ELSE :p4 END END AS ExpensiveFlag
            FROM 
                products a0
            """, [":p0", ":p1", ":p2", ":p3", ":p4"])),
        new((DatabaseType.PostgreSQL, "CaseDecimalExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                CASE WHEN a0.Price > :p0 THEN :p1 ELSE CASE WHEN a0.Price > :p2 THEN :p3 ELSE :p4 END END AS ExpensiveFlag
            FROM 
                products a0
            """, [":p0", ":p1", ":p2", ":p3", ":p4"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] CaseGuidExpression_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "CaseGuidExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                CASE WHEN a0.UniqueId = @p0 THEN @p1 ELSE @p2 END AS Status
            FROM 
                products a0
            """, ["@p0", "@p1", "@p2"])),
        new((DatabaseType.SQLite, "CaseGuidExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                CASE WHEN a0.UniqueId = :p0 THEN :p1 ELSE :p2 END AS Status
            FROM 
                products a0
            """, [":p0", ":p1", ":p2"])),
        new((DatabaseType.PostgreSQL, "CaseGuidExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                CASE WHEN a0.UniqueId = :p0 THEN :p1 ELSE :p2 END AS Status
            FROM 
                products a0
            """, [":p0", ":p1", ":p2"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] CaseIntExpression_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "CaseIntExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                CASE WHEN a0.Age > @p0 THEN @p1 ELSE @p2 END AS Proj0
            FROM 
                customers a0
            """, ["@p0", "@p1", "@p2"])),
        new((DatabaseType.SQLite, "CaseIntExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                CASE WHEN a0.Age > :p0 THEN :p1 ELSE :p2 END AS Proj0
            FROM 
                customers a0
            """, [":p0", ":p1", ":p2"])),
        new((DatabaseType.PostgreSQL, "CaseIntExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                CASE WHEN a0.Age > :p0 THEN :p1 ELSE :p2 END AS Proj0
            FROM 
                customers a0
            """, [":p0", ":p1", ":p2"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] CaseInWhere_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "CaseInWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                CASE WHEN a0.Age > @p0 THEN @p1 ELSE @p2 END = @p3
            """, ["@p0", "@p1", "@p2", "@p3"])),
        new((DatabaseType.SQLite, "CaseInWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                CASE WHEN a0.Age > :p0 THEN :p1 ELSE :p2 END = :p3
            """, [":p0", ":p1", ":p2", ":p3"])),
        new((DatabaseType.PostgreSQL, "CaseInWhere_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                CASE WHEN a0.Age > :p0 THEN :p1 ELSE :p2 END = :p3
            """, [":p0", ":p1", ":p2", ":p3"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] CaseStringExpression_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "CaseStringExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                CASE WHEN a0.Age > @p0 THEN @p1 ELSE @p2 END AS Proj0
            FROM 
                customers a0
            """, ["@p0", "@p1", "@p2"])),
        new((DatabaseType.SQLite, "CaseStringExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                CASE WHEN a0.Age > :p0 THEN :p1 ELSE :p2 END AS Proj0
            FROM 
                customers a0
            """, [":p0", ":p1", ":p2"])),
        new((DatabaseType.PostgreSQL, "CaseStringExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                CASE WHEN a0.Age > :p0 THEN :p1 ELSE :p2 END AS Proj0
            FROM 
                customers a0
            """, [":p0", ":p1", ":p2"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] ComplexJoinWhereGroupByHavingOrderBySelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "ComplexJoinWhereGroupByHavingOrderBySelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS CustomerId,
            a0.Name AS CustomerName,
            COUNT(*) AS TotalOrders,
            SUM(a1.Amount) AS TotalSpent,
            SUM(a1.Amount) / COUNT(*) AS AvgOrderValue
        FROM 
            customers a0
        INNER JOIN orders a1 ON a0.Id = a1.CustomerId
        WHERE 
            a0.Age >= @p0 AND a1.Amount > @p1
        GROUP BY 
            a0.Id, a0.Name
        HAVING 
            COUNT(*) > @p2 AND SUM(a1.Amount) > @p3
        ORDER BY 
            SUM(a1.Amount) DESC, COUNT(*) ASC
        """, ["p0", "p1", "p2", "p3"])),
        new((DatabaseType.SQLite, "ComplexJoinWhereGroupByHavingOrderBySelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS CustomerId,
            a0.Name AS CustomerName,
            COUNT(*) AS TotalOrders,
            SUM(a1.Amount) AS TotalSpent,
            SUM(a1.Amount) / COUNT(*) AS AvgOrderValue
        FROM 
            customers a0
        INNER JOIN orders a1 ON a0.Id = a1.CustomerId
        WHERE 
            a0.Age >= :p0 AND a1.Amount > :p1
        GROUP BY 
            a0.Id, a0.Name
        HAVING 
            COUNT(*) > :p2 AND SUM(a1.Amount) > :p3
        ORDER BY 
            SUM(a1.Amount) DESC, COUNT(*) ASC
        """, ["p0", "p1", "p2", "p3"])),
        new((DatabaseType.PostgreSQL, "ComplexJoinWhereGroupByHavingOrderBySelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS CustomerId,
            a0.Name AS CustomerName,
            COUNT(*) AS TotalOrders,
            SUM(a1.Amount) AS TotalSpent,
            SUM(a1.Amount) / COUNT(*) AS AvgOrderValue
        FROM 
            customers a0
        INNER JOIN orders a1 ON a0.Id = a1.CustomerId
        WHERE 
            a0.Age >= :p0 AND a1.Amount > :p1
        GROUP BY 
            a0.Id, a0.Name
        HAVING 
            COUNT(*) > :p2 AND SUM(a1.Amount) > :p3
        ORDER BY 
            SUM(a1.Amount) DESC, COUNT(*) ASC
        """, ["p0", "p1", "p2", "p3"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] ComplexLeftJoinWhereGroupByOrderBySelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "ComplexLeftJoinWhereGroupByOrderBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS CustomerName,
                COUNT(*) AS OrderCount,
                SUM(a1.Amount) AS TotalSpent
            FROM 
                customers a0
            LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
            WHERE 
                a0.Age >= @p0
            GROUP BY 
                a0.Id, a0.Name
            ORDER BY 
                SUM(a1.Amount) DESC, a0.Name ASC
            """, ["@p0"])),
        new((DatabaseType.SQLite, "ComplexLeftJoinWhereGroupByOrderBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS CustomerName,
                COUNT(*) AS OrderCount,
                SUM(a1.Amount) AS TotalSpent
            FROM 
                customers a0
            LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
            WHERE 
                a0.Age >= :p0
            GROUP BY 
                a0.Id, a0.Name
            ORDER BY 
                SUM(a1.Amount) DESC, a0.Name ASC
            """, [":p0"])),
        new((DatabaseType.PostgreSQL, "ComplexLeftJoinWhereGroupByOrderBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS CustomerId,
                a0.Name AS CustomerName,
                COUNT(*) AS OrderCount,
                SUM(a1.Amount) AS TotalSpent
            FROM 
                customers a0
            LEFT JOIN orders a1 ON a0.Id = a1.CustomerId
            WHERE 
                a0.Age >= :p0
            GROUP BY 
                a0.Id, a0.Name
            ORDER BY 
                SUM(a1.Amount) DESC, a0.Name ASC
            """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromGroupByMinMaxSelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromGroupByMinMaxSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.CustomerId AS CustomerId,
                MIN(a0.Amount) AS MinAmount,
                MAX(a0.Amount) AS MaxAmount,
                COUNT(*) AS OrderCount
            FROM 
                orders a0
            GROUP BY 
                a0.CustomerId
            """, [])),
        new((DatabaseType.SQLite, "FromGroupByMinMaxSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.CustomerId AS CustomerId,
                MIN(a0.Amount) AS MinAmount,
                MAX(a0.Amount) AS MaxAmount,
                COUNT(*) AS OrderCount
            FROM 
                orders a0
            GROUP BY 
                a0.CustomerId
            """, [])),
        new((DatabaseType.PostgreSQL, "FromGroupByMinMaxSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.CustomerId AS CustomerId,
                MIN(a0.Amount) AS MinAmount,
                MAX(a0.Amount) AS MaxAmount,
                COUNT(*) AS OrderCount
            FROM 
                orders a0
            GROUP BY 
                a0.CustomerId
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromGroupByAvgSelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromGroupByAvgSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.CustomerId AS CustomerId,
            AVG(a0.Amount) AS AvgAmount,
            COUNT(*) AS OrderCount
        FROM 
            orders a0
        GROUP BY 
            a0.CustomerId
        """, [])),
        new((DatabaseType.SQLite, "FromGroupByAvgSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.CustomerId AS CustomerId,
            AVG(a0.Amount) AS AvgAmount,
            COUNT(*) AS OrderCount
        FROM 
            orders a0
        GROUP BY 
            a0.CustomerId
        """, [])),
        new((DatabaseType.PostgreSQL, "FromGroupByAvgSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.CustomerId AS CustomerId,
            AVG(a0.Amount) AS AvgAmount,
            COUNT(*) AS OrderCount
        FROM 
            orders a0
        GROUP BY 
            a0.CustomerId
        """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromGroupByDecimalAggregatesSelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromGroupByDecimalAggregatesSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                SUM(a0.Price) AS TotalPrice,
                AVG(a0.Price) AS AvgPrice,
                MIN(a0.Price) AS MinPrice,
                MAX(a0.Price) AS MaxPrice,
                COUNT(*) AS ProductCount
            FROM 
                products a0
            GROUP BY 
                a0.ProductName
            """, [])),
        new((DatabaseType.SQLite, "FromGroupByDecimalAggregatesSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                SUM(a0.Price) AS TotalPrice,
                AVG(a0.Price) AS AvgPrice,
                MIN(a0.Price) AS MinPrice,
                MAX(a0.Price) AS MaxPrice,
                COUNT(*) AS ProductCount
            FROM 
                products a0
            GROUP BY 
                a0.ProductName
            """, [])),
        new((DatabaseType.PostgreSQL, "FromGroupByDecimalAggregatesSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                SUM(a0.Price) AS TotalPrice,
                AVG(a0.Price) AS AvgPrice,
                MIN(a0.Price) AS MinPrice,
                MAX(a0.Price) AS MaxPrice,
                COUNT(*) AS ProductCount
            FROM 
                products a0
            GROUP BY 
                a0.ProductName
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromGroupByDecimalAvgSelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromGroupByDecimalAvgSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                AVG(a0.Price) AS AvgPrice
            FROM 
                products a0
            GROUP BY 
                a0.ProductName
            """, [])),
        new((DatabaseType.SQLite, "FromGroupByDecimalAvgSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                AVG(a0.Price) AS AvgPrice
            FROM 
                products a0
            GROUP BY 
                a0.ProductName
            """, [])),
        new((DatabaseType.PostgreSQL, "FromGroupByDecimalAvgSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                AVG(a0.Price) AS AvgPrice
            FROM 
                products a0
            GROUP BY 
                a0.ProductName
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromGroupByDecimalSumSelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromGroupByDecimalSumSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.ProductName AS ProductName,
            SUM(a0.Price) AS TotalPrice
        FROM 
            products a0
        GROUP BY 
            a0.ProductName
        """, [])),
        new((DatabaseType.SQLite, "FromGroupByDecimalSumSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.ProductName AS ProductName,
            SUM(a0.Price) AS TotalPrice
        FROM 
            products a0
        GROUP BY 
            a0.ProductName
        """, [])),
        new((DatabaseType.PostgreSQL, "FromGroupByDecimalSumSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.ProductName AS ProductName,
            SUM(a0.Price) AS TotalPrice
        FROM 
            products a0
        GROUP BY 
            a0.ProductName
        """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromGroupByHavingOrderBySelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromGroupByHavingOrderBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.CustomerId AS CustomerId,
                SUM(a0.Amount) AS TotalAmount
            FROM 
                orders a0
            GROUP BY 
                a0.CustomerId
            HAVING 
                COUNT(*) > @p0
            ORDER BY 
                SUM(a0.Amount) DESC
            """, ["@p0"])),
        new((DatabaseType.SQLite, "FromGroupByHavingOrderBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.CustomerId AS CustomerId,
                SUM(a0.Amount) AS TotalAmount
            FROM 
                orders a0
            GROUP BY 
                a0.CustomerId
            HAVING 
                COUNT(*) > :p0
            ORDER BY 
                SUM(a0.Amount) DESC
            """, [":p0"])),
        new((DatabaseType.PostgreSQL, "FromGroupByHavingOrderBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.CustomerId AS CustomerId,
                SUM(a0.Amount) AS TotalAmount
            FROM 
                orders a0
            GROUP BY 
                a0.CustomerId
            HAVING 
                COUNT(*) > :p0
            ORDER BY 
                SUM(a0.Amount) DESC
            """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromGroupByHavingSelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromGroupByHavingSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Age AS Age,
                COUNT(*) AS Count
            FROM 
                customers a0
            GROUP BY 
                a0.Age
            HAVING 
                COUNT(*) > @p0
            """, ["@p0"])),
        new((DatabaseType.SQLite, "FromGroupByHavingSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Age AS Age,
                COUNT(*) AS Count
            FROM 
                customers a0
            GROUP BY 
                a0.Age
            HAVING 
                COUNT(*) > :p0
            """, [":p0"])),
        new((DatabaseType.PostgreSQL, "FromGroupByHavingSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Age AS Age,
                COUNT(*) AS Count
            FROM 
                customers a0
            GROUP BY 
                a0.Age
            HAVING 
                COUNT(*) > :p0
            """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromGroupByMultipleOrderBySelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromGroupByMultipleOrderBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Age AS Age,
                a0.Name AS Name,
                COUNT(*) AS Count
            FROM 
                customers a0
            GROUP BY 
                a0.Age, a0.Name
            ORDER BY 
                COUNT(*) DESC
            """, [])),
        new((DatabaseType.SQLite, "FromGroupByMultipleOrderBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Age AS Age,
                a0.Name AS Name,
                COUNT(*) AS Count
            FROM 
                customers a0
            GROUP BY 
                a0.Age, a0.Name
            ORDER BY 
                COUNT(*) DESC
            """, [])),
        new((DatabaseType.PostgreSQL, "FromGroupByMultipleOrderBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Age AS Age,
                a0.Name AS Name,
                COUNT(*) AS Count
            FROM 
                customers a0
            GROUP BY 
                a0.Age, a0.Name
            ORDER BY 
                COUNT(*) DESC
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromGroupByMultipleSelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromGroupByMultipleSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Age AS Age,
                a0.Name AS Name,
                COUNT(*) AS Count
            FROM 
                customers a0
            GROUP BY 
                a0.Age, a0.Name
            """, [])),
        new((DatabaseType.SQLite, "FromGroupByMultipleSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Age AS Age,
                a0.Name AS Name,
                COUNT(*) AS Count
            FROM 
                customers a0
            GROUP BY 
                a0.Age, a0.Name
            """, [])),
        new((DatabaseType.PostgreSQL, "FromGroupByMultipleSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Age AS Age,
                a0.Name AS Name,
                COUNT(*) AS Count
            FROM 
                customers a0
            GROUP BY 
                a0.Age, a0.Name
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromGroupByOrderByMultipleSelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromGroupByOrderByMultipleSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.CustomerId AS CustomerId,
                SUM(a0.Amount) AS TotalAmount,
                COUNT(*) AS OrderCount
            FROM 
                orders a0
            GROUP BY 
                a0.CustomerId
            ORDER BY 
                SUM(a0.Amount) DESC, COUNT(*) ASC
            """, [])),
        new((DatabaseType.SQLite, "FromGroupByOrderByMultipleSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.CustomerId AS CustomerId,
                SUM(a0.Amount) AS TotalAmount,
                COUNT(*) AS OrderCount
            FROM 
                orders a0
            GROUP BY 
                a0.CustomerId
            ORDER BY 
                SUM(a0.Amount) DESC, COUNT(*) ASC
            """, [])),
        new((DatabaseType.PostgreSQL, "FromGroupByOrderByMultipleSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.CustomerId AS CustomerId,
                SUM(a0.Amount) AS TotalAmount,
                COUNT(*) AS OrderCount
            FROM 
                orders a0
            GROUP BY 
                a0.CustomerId
            ORDER BY 
                SUM(a0.Amount) DESC, COUNT(*) ASC
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromGroupByOrderBySelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromGroupByOrderBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.CustomerId AS CustomerId,
                SUM(a0.Amount) AS TotalAmount
            FROM 
                orders a0
            GROUP BY 
                a0.CustomerId
            ORDER BY 
                SUM(a0.Amount) DESC
            """, [])),
        new((DatabaseType.SQLite, "FromGroupByOrderBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.CustomerId AS CustomerId,
                SUM(a0.Amount) AS TotalAmount
            FROM 
                orders a0
            GROUP BY 
                a0.CustomerId
            ORDER BY 
                SUM(a0.Amount) DESC
            """, [])),
        new((DatabaseType.PostgreSQL, "FromGroupByOrderBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.CustomerId AS CustomerId,
                SUM(a0.Amount) AS TotalAmount
            FROM 
                orders a0
            GROUP BY 
                a0.CustomerId
            ORDER BY 
                SUM(a0.Amount) DESC
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromGroupByOrderByThreeKeysSelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromGroupByOrderByThreeKeysSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.CustomerId AS CustomerId,
            SUM(a0.Amount) AS TotalAmount,
            COUNT(*) AS OrderCount
        FROM 
            orders a0
        GROUP BY 
            a0.CustomerId
        ORDER BY 
            SUM(a0.Amount) DESC, COUNT(*) ASC, a0.CustomerId ASC
        """, [])),
        new((DatabaseType.SQLite, "FromGroupByOrderByThreeKeysSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.CustomerId AS CustomerId,
            SUM(a0.Amount) AS TotalAmount,
            COUNT(*) AS OrderCount
        FROM 
            orders a0
        GROUP BY 
            a0.CustomerId
        ORDER BY 
            SUM(a0.Amount) DESC, COUNT(*) ASC, a0.CustomerId ASC
        """, [])),
        new((DatabaseType.PostgreSQL, "FromGroupByOrderByThreeKeysSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.CustomerId AS CustomerId,
            SUM(a0.Amount) AS TotalAmount,
            COUNT(*) AS OrderCount
        FROM 
            orders a0
        GROUP BY 
            a0.CustomerId
        ORDER BY 
            SUM(a0.Amount) DESC, COUNT(*) ASC, a0.CustomerId ASC
        """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromGroupBySelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromGroupBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Age AS Age,
                COUNT(*) AS Count
            FROM 
                customers a0
            GROUP BY 
                a0.Age
            """, [])),
        new((DatabaseType.SQLite, "FromGroupBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Age AS Age,
                COUNT(*) AS Count
            FROM 
                customers a0
            GROUP BY 
                a0.Age
            """, [])),
        new((DatabaseType.PostgreSQL, "FromGroupBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Age AS Age,
                COUNT(*) AS Count
            FROM 
                customers a0
            GROUP BY 
                a0.Age
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromOrderByAsc_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromOrderByAsc_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            ORDER BY 
                a0.Name ASC
            """, [])),
        new((DatabaseType.SQLite, "FromOrderByAsc_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            ORDER BY 
                a0.Name ASC
            """, [])),
        new((DatabaseType.PostgreSQL, "FromOrderByAsc_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            ORDER BY 
                a0.Name ASC
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromOrderByDesc_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromOrderByDesc_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        ORDER BY 
            a0.Age DESC
        """, [])),
        new((DatabaseType.SQLite, "FromOrderByDesc_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        ORDER BY 
            a0.Age DESC
        """, [])),
        new((DatabaseType.PostgreSQL, "FromOrderByDesc_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        ORDER BY 
            a0.Age DESC
        """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromOrderByDescendingThenBy_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromOrderByDescendingThenBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            ORDER BY 
                a0.Age DESC, a0.Name ASC
            """, [])),
        new((DatabaseType.SQLite, "FromOrderByDescendingThenBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            ORDER BY 
                a0.Age DESC, a0.Name ASC
            """, [])),
        new((DatabaseType.PostgreSQL, "FromOrderByDescendingThenBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            ORDER BY 
                a0.Age DESC, a0.Name ASC
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromOrderByMultiple_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromOrderByMultiple_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            ORDER BY 
                a0.Name ASC, a0.Age DESC, a0.Id ASC
            """, [])),
        new((DatabaseType.SQLite, "FromOrderByMultiple_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            ORDER BY 
                a0.Name ASC, a0.Age DESC, a0.Id ASC
            """, [])),
        new((DatabaseType.PostgreSQL, "FromOrderByMultiple_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            ORDER BY 
                a0.Name ASC, a0.Age DESC, a0.Id ASC
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromOrderByThenByDescending_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromOrderByThenByDescending_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            ORDER BY 
                a0.Name ASC, a0.Age DESC
            """, [])),
        new((DatabaseType.SQLite, "FromOrderByThenByDescending_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            ORDER BY 
                a0.Name ASC, a0.Age DESC
            """, [])),
        new((DatabaseType.PostgreSQL, "FromOrderByThenByDescending_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            ORDER BY 
                a0.Name ASC, a0.Age DESC
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromOrderByThenBySelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromOrderByThenBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            ORDER BY 
                a0.Name ASC, a0.Age ASC
            """, [])),
        new((DatabaseType.SQLite, "FromOrderByThenBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            ORDER BY 
                a0.Name ASC, a0.Age ASC
            """, [])),
        new((DatabaseType.PostgreSQL, "FromOrderByThenBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            ORDER BY 
                a0.Name ASC, a0.Age ASC
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromOrderByThenBy_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromOrderByThenBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            ORDER BY 
                a0.Name ASC, a0.Age ASC
            """, [])),
        new((DatabaseType.SQLite, "FromOrderByThenBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            ORDER BY 
                a0.Name ASC, a0.Age ASC
            """, [])),
        new((DatabaseType.PostgreSQL, "FromOrderByThenBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            ORDER BY 
                a0.Name ASC, a0.Age ASC
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromProductWhereSelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromProductWhereSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName
            FROM 
                products a0
            WHERE 
                a0.ProductName != @p0
            """, ["@p0"])),
        new((DatabaseType.SQLite, "FromProductWhereSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName
            FROM 
                products a0
            WHERE 
                a0.ProductName != :p0
            """, [":p0"])),
        new((DatabaseType.PostgreSQL, "FromProductWhereSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName
            FROM 
                products a0
            WHERE 
                a0.ProductName != :p0
            """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromSelectAvg_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromSelectAvg_GeneratesCorrectSql"), ("""
            SELECT 
                AVG(a0.Amount) AS Proj0
            FROM 
                orders a0
            """, [])),
        new((DatabaseType.SQLite, "FromSelectAvg_GeneratesCorrectSql"), ("""
            SELECT 
                AVG(a0.Amount) AS Proj0
            FROM 
                orders a0
            """, [])),
        new((DatabaseType.PostgreSQL, "FromSelectAvg_GeneratesCorrectSql"), ("""
            SELECT 
                AVG(a0.Amount) AS Proj0
            FROM 
                orders a0
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromSelectCreatedDateMinMax_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromSelectCreatedDateMinMax_GeneratesCorrectSql"), ("""
        SELECT 
            a0.ProductName AS ProductName,
            a0.CreatedDate AS EarliestDate,
            a0.CreatedDate AS LatestDate
        FROM 
            products a0
        """, [])),
        new((DatabaseType.SQLite, "FromSelectCreatedDateMinMax_GeneratesCorrectSql"), ("""
        SELECT 
            a0.ProductName AS ProductName,
            a0.CreatedDate AS EarliestDate,
            a0.CreatedDate AS LatestDate
        FROM 
            products a0
        """, [])),
        new((DatabaseType.PostgreSQL, "FromSelectCreatedDateMinMax_GeneratesCorrectSql"), ("""
        SELECT 
            a0.ProductName AS ProductName,
            a0.CreatedDate AS EarliestDate,
            a0.CreatedDate AS LatestDate
        FROM 
            products a0
        """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromSelectDecimalArithmetic_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromSelectDecimalArithmetic_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                a0.Price * @p0 AS Proj0,
                a0.Price + @p1 AS Proj1,
                a0.Price - @p2 AS Proj2
            FROM 
                products a0
            """, ["@p0", "@p1", "@p2"])),
        new((DatabaseType.SQLite, "FromSelectDecimalArithmetic_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                a0.Price * :p0 AS Proj0,
                a0.Price + :p1 AS Proj1,
                a0.Price - :p2 AS Proj2
            FROM 
                products a0
            """, [":p0", ":p1", ":p2"])),
        new((DatabaseType.PostgreSQL, "FromSelectDecimalArithmetic_GeneratesCorrectSql"), ("""
            SELECT 
                a0.ProductName AS ProductName,
                a0.Price * :p0 AS Proj0,
                a0.Price + :p1 AS Proj1,
                a0.Price - :p2 AS Proj2
            FROM 
                products a0
            """, [":p0", ":p1", ":p2"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromSelectExpression_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromSelectExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id * @p0 + a0.Age AS Proj0,
                CONCAT(a0.Name, @p1) AS Proj1
            FROM 
                customers a0
            """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "FromSelectExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id * :p0 + a0.Age AS Proj0,
                a0.Name || :p1 AS Proj1
            FROM 
                customers a0
            """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "FromSelectExpression_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id * :p0 + a0.Age AS Proj0,
                a0.Name || :p1 AS Proj1
            FROM 
                customers a0
            """, [":p0", ":p1"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromSelectMax_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromSelectMax_GeneratesCorrectSql"), ("""
            SELECT 
                MAX(a0.Amount) AS Proj0
            FROM 
                orders a0
            """, [])),
        new((DatabaseType.SQLite, "FromSelectMax_GeneratesCorrectSql"), ("""
            SELECT 
                MAX(a0.Amount) AS Proj0
            FROM 
                orders a0
            """, [])),
        new((DatabaseType.PostgreSQL, "FromSelectMax_GeneratesCorrectSql"), ("""
            SELECT 
                MAX(a0.Amount) AS Proj0
            FROM 
                orders a0
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromSelectMin_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromSelectMin_GeneratesCorrectSql"), ("""
        SELECT 
            MIN(a0.Amount) AS Proj0
        FROM 
            orders a0
        """, [])),
        new((DatabaseType.SQLite, "FromSelectMin_GeneratesCorrectSql"), ("""
        SELECT 
            MIN(a0.Amount) AS Proj0
        FROM 
            orders a0
        """, [])),
        new((DatabaseType.PostgreSQL, "FromSelectMin_GeneratesCorrectSql"), ("""
        SELECT 
            MIN(a0.Amount) AS Proj0
        FROM 
            orders a0
        """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromSelectOrderBy_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromSelectOrderBy_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a0.Age + @p0 AS Proj0
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC
        """, ["p0"])),
        new((DatabaseType.SQLite, "FromSelectOrderBy_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a0.Age + :p0 AS Proj0
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC
        """, ["p0"])),
        new((DatabaseType.PostgreSQL, "FromSelectOrderBy_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name,
            a0.Age + :p0 AS Proj0
        FROM 
            customers a0
        ORDER BY 
            a0.Name ASC
        """, ["p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromSelectSingle_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromSelectSingle_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Age AS Age
        FROM 
            customers a0
        """, [])),
        new((DatabaseType.SQLite, "FromSelectSingle_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Age AS Age
        FROM 
            customers a0
        """, [])),
        new((DatabaseType.PostgreSQL, "FromSelectSingle_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Age AS Age
        FROM 
            customers a0
        """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromSelectSum_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromSelectSum_GeneratesCorrectSql"), ("""
        SELECT 
            SUM(a0.Amount) AS Proj0
        FROM 
            orders a0
        """, [])),
        new((DatabaseType.SQLite, "FromSelectSum_GeneratesCorrectSql"), ("""
        SELECT 
            SUM(a0.Amount) AS Proj0
        FROM 
            orders a0
        """, [])),
        new((DatabaseType.PostgreSQL, "FromSelectSum_GeneratesCorrectSql"), ("""
        SELECT 
            SUM(a0.Amount) AS Proj0
        FROM 
            orders a0
        """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromSelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        """, [])),
        new((DatabaseType.SQLite, "FromSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        """, [])),
        new((DatabaseType.PostgreSQL, "FromSelect_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Name AS Name
        FROM 
            customers a0
        """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromStatic_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromStatic_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            """, [])),
        new((DatabaseType.SQLite, "FromStatic_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            """, [])),
        new((DatabaseType.PostgreSQL, "FromStatic_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromSubquery_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromSubquery_GeneratesCorrectSql"), ("""
        SELECT 
            a1.Id AS Id,
            a1.NewAge AS NewAge
        FROM 
            (SELECT 
                a0.Id AS Id,
                a0.Age + @p0 AS NewAge
            FROM 
                customers a0) a1
        """, ["@p0"])),
        new((DatabaseType.SQLite, "FromSubquery_GeneratesCorrectSql"), ("""
        SELECT 
            a1.Id AS Id,
            a1.NewAge AS NewAge
        FROM 
            (SELECT 
                a0.Id AS Id,
                a0.Age + :p0 AS NewAge
            FROM 
                customers a0) a1
        """, [":p0"])),
        new((DatabaseType.PostgreSQL, "FromSubquery_GeneratesCorrectSql"), ("""
        SELECT 
            a1.Id AS Id,
            a1.NewAge AS NewAge
        FROM 
            (SELECT 
                a0.Id AS Id,
                a0.Age + :p0 AS NewAge
            FROM 
                customers a0) a1
        """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereAgeGreaterThanAverageAge_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereAgeGreaterThanAverageAge_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > (SELECT 
                SUM(a1.Age) AS Proj0
            FROM 
                customers a1)
            """, [])),
        new((DatabaseType.SQLite, "FromWhereAgeGreaterThanAverageAge_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > (SELECT 
                SUM(a1.Age) AS Proj0
            FROM 
                customers a1)
            """, [])),
        new((DatabaseType.PostgreSQL, "FromWhereAgeGreaterThanAverageAge_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > (SELECT 
                SUM(a1.Age) AS Proj0
            FROM 
                customers a1)
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereAgeGreaterThanSum_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "FromWhereAgeGreaterThanSum_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > (SELECT 
                SUM(a1.Age) AS Proj0
            FROM 
                customers a1)
            """, [])),
            new((DatabaseType.SQLite, "FromWhereAgeGreaterThanSum_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > (SELECT 
                SUM(a1.Age) AS Proj0
            FROM 
                customers a1)
            """, [])),
            new((DatabaseType.PostgreSQL, "FromWhereAgeGreaterThanSum_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > (SELECT 
                SUM(a1.Age) AS Proj0
            FROM 
                customers a1)
            """, []))
        ];    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereAgeInSubqueryWithClosure_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereAgeInSubqueryWithClosure_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age IN (SELECT 
                a1.Age AS Age
            FROM 
                customers a1
            WHERE 
                a1.Name = CONCAT(a0.Name, @p0))
            """, ["@p0"])),
        new((DatabaseType.SQLite, "FromWhereAgeInSubqueryWithClosure_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age IN (SELECT 
                a1.Age AS Age
            FROM 
                customers a1
            WHERE 
                a1.Name = a0.Name || :p0)
            """, [":p0"])),
        new((DatabaseType.PostgreSQL, "FromWhereAgeInSubqueryWithClosure_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age IN (SELECT 
                a1.Age AS Age
            FROM 
                customers a1
            WHERE 
                a1.Name = a0.Name || :p0)
            """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereAgeInSubquery_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereAgeInSubquery_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age IN (SELECT 
            a1.Age AS Age
        FROM 
            customers a1
        WHERE 
            a1.Name = @p0)
        """, ["@p0"])),
        new((DatabaseType.SQLite, "FromWhereAgeInSubquery_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age IN (SELECT 
            a1.Age AS Age
        FROM 
            customers a1
        WHERE 
            a1.Name = :p0)
        """, [":p0"])),
        new((DatabaseType.PostgreSQL, "FromWhereAgeInSubquery_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age IN (SELECT 
            a1.Age AS Age
        FROM 
            customers a1
        WHERE 
            a1.Name = :p0)
        """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereAgeIn_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereAgeIn_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age IN (@p0, @p1, @p2, @p3)
            """, ["@p0", "@p1", "@p2", "@p3"])),
        new((DatabaseType.SQLite, "FromWhereAgeIn_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age IN (:p0, :p1, :p2, :p3)
            """, [":p0", ":p1", ":p2", ":p3"])),
        new((DatabaseType.PostgreSQL, "FromWhereAgeIn_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age IN (:p0, :p1, :p2, :p3)
            """, [":p0", ":p1", ":p2", ":p3"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereAndSelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereAndSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age >= @p0 AND a0.Name != @p1
            """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "FromWhereAndSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age >= :p0 AND a0.Name != :p1
            """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "FromWhereAndSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age >= :p0 AND a0.Name != :p1
            """, [":p0", ":p1"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereAnd_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereAnd_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > @p0 AND a0.Name = @p1
            """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "FromWhereAnd_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0 AND a0.Name = :p1
            """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "FromWhereAnd_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0 AND a0.Name = :p1
            """, [":p0", ":p1"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereCreatedDateComparison_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereCreatedDateComparison_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.CreatedDate > @p0
            """, ["@p0"])),
        new((DatabaseType.SQLite, "FromWhereCreatedDateComparison_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.CreatedDate > :p0
            """, [":p0"])),
        new((DatabaseType.PostgreSQL, "FromWhereCreatedDateComparison_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.CreatedDate > :p0
            """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereCreatedDateIsNotNull_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereCreatedDateIsNotNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.CreatedDate IS NOT NULL
            """, [])),
        new((DatabaseType.SQLite, "FromWhereCreatedDateIsNotNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.CreatedDate IS NOT NULL
            """, [])),
        new((DatabaseType.PostgreSQL, "FromWhereCreatedDateIsNotNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.CreatedDate IS NOT NULL
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereCreatedDateIsNull_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereCreatedDateIsNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.CreatedDate IS NULL
            """, [])),
        new((DatabaseType.SQLite, "FromWhereCreatedDateIsNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.CreatedDate IS NULL
            """, [])),
        new((DatabaseType.PostgreSQL, "FromWhereCreatedDateIsNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.CreatedDate IS NULL
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereDecimalComparison_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereDecimalComparison_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.Price > @p0
            """, ["@p0"])),
        new((DatabaseType.SQLite, "FromWhereDecimalComparison_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.Price > :p0
            """, [":p0"])),
        new((DatabaseType.PostgreSQL, "FromWhereDecimalComparison_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.Price > :p0
            """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereDecimalIsNotNull_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereDecimalIsNotNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.Price IS NOT NULL
            """, [])),
        new((DatabaseType.SQLite, "FromWhereDecimalIsNotNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.Price IS NOT NULL
            """, [])),
        new((DatabaseType.PostgreSQL, "FromWhereDecimalIsNotNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.Price IS NOT NULL
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereDecimalIsNull_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereDecimalIsNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.Price IS NULL
            """, [])),
        new((DatabaseType.SQLite, "FromWhereDecimalIsNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.Price IS NULL
            """, [])),
        new((DatabaseType.PostgreSQL, "FromWhereDecimalIsNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.ProductName AS ProductName,
                a0.Price AS Price,
                a0.CreatedDate AS CreatedDate,
                a0.UniqueId AS UniqueId
            FROM 
                products a0
            WHERE 
                a0.Price IS NULL
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereFusionThree_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereFusionThree_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > @p0 AND a0.Name != @p1 AND a0.Age < @p2
            """, ["@p0", "@p1", "@p2"])),
        new((DatabaseType.SQLite, "FromWhereFusionThree_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0 AND a0.Name != :p1 AND a0.Age < :p2
            """, [":p0", ":p1", ":p2"])),
        new((DatabaseType.PostgreSQL, "FromWhereFusionThree_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0 AND a0.Name != :p1 AND a0.Age < :p2
            """, [":p0", ":p1", ":p2"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereFusionTwo_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereFusionTwo_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > @p0 AND a0.Name != @p1
        """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "FromWhereFusionTwo_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0 AND a0.Name != :p1
        """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "FromWhereFusionTwo_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0 AND a0.Name != :p1
        """, [":p0", ":p1"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereFusionWithOrderBy_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereFusionWithOrderBy_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > @p0 AND a0.Name != @p1
        ORDER BY 
            a0.Name ASC
        """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "FromWhereFusionWithOrderBy_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0 AND a0.Name != :p1
        ORDER BY 
            a0.Name ASC
        """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "FromWhereFusionWithOrderBy_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0 AND a0.Name != :p1
        ORDER BY 
            a0.Name ASC
        """, [":p0", ":p1"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereFusionWithSelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereFusionWithSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age >= @p0 AND a0.Name != @p1
            """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "FromWhereFusionWithSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age >= :p0 AND a0.Name != :p1
            """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "FromWhereFusionWithSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age >= :p0 AND a0.Name != :p1
            """, [":p0", ":p1"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereGroupBySelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereGroupBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Age AS Age,
                COUNT(*) AS Count
            FROM 
                customers a0
            WHERE 
                a0.Age >= @p0
            GROUP BY 
                a0.Age
            """, ["@p0"])),
        new((DatabaseType.SQLite, "FromWhereGroupBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Age AS Age,
                COUNT(*) AS Count
            FROM 
                customers a0
            WHERE 
                a0.Age >= :p0
            GROUP BY 
                a0.Age
            """, [":p0"])),
        new((DatabaseType.PostgreSQL, "FromWhereGroupBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Age AS Age,
                COUNT(*) AS Count
            FROM 
                customers a0
            WHERE 
                a0.Age >= :p0
            GROUP BY 
                a0.Age
            """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereInt_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereInt_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > @p0
            """, ["@p0"])),
        new((DatabaseType.SQLite, "FromWhereInt_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0
            """, [":p0"])),
        new((DatabaseType.PostgreSQL, "FromWhereInt_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0
            """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereIsNotNullInt_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereIsNotNullInt_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age IS NOT NULL
        """, [])),
        new((DatabaseType.SQLite, "FromWhereIsNotNullInt_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age IS NOT NULL
        """, [])),
        new((DatabaseType.PostgreSQL, "FromWhereIsNotNullInt_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age IS NOT NULL
        """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereIsNotNull_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereIsNotNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Name IS NOT NULL
            """, [])),
        new((DatabaseType.SQLite, "FromWhereIsNotNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Name IS NOT NULL
            """, [])),
        new((DatabaseType.PostgreSQL, "FromWhereIsNotNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Name IS NOT NULL
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereIsNullCombined_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereIsNullCombined_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Name IS NULL AND a0.Age IS NOT NULL
            """, [])),
        new((DatabaseType.SQLite, "FromWhereIsNullCombined_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Name IS NULL AND a0.Age IS NOT NULL
            """, [])),
        new((DatabaseType.PostgreSQL, "FromWhereIsNullCombined_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Name IS NULL AND a0.Age IS NOT NULL
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereIsNullInt_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereIsNullInt_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age IS NULL
            """, [])),
        new((DatabaseType.SQLite, "FromWhereIsNullInt_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age IS NULL
            """, [])),
        new((DatabaseType.PostgreSQL, "FromWhereIsNullInt_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age IS NULL
            """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereIsNull_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "FromWhereIsNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Name IS NULL
            """, [])),
            new((DatabaseType.SQLite, "FromWhereIsNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Name IS NULL
            """, [])),
            new((DatabaseType.PostgreSQL, "FromWhereIsNull_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Name IS NULL
            """, []))
        ];    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereMultiple_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereMultiple_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > @p0 AND a0.Name != @p1
            """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "FromWhereMultiple_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0 AND a0.Name != :p1
            """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "FromWhereMultiple_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0 AND a0.Name != :p1
            """, [":p0", ":p1"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereOr_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereOr_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > @p0 AND a0.Age < @p1 OR a0.Name = @p2
            """, ["@p0", "@p1", "@p2"])),
        new((DatabaseType.SQLite, "FromWhereOr_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0 AND a0.Age < :p1 OR a0.Name = :p2
            """, [":p0", ":p1", ":p2"])),
        new((DatabaseType.PostgreSQL, "FromWhereOr_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0 AND a0.Age < :p1 OR a0.Name = :p2
            """, [":p0", ":p1", ":p2"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereOrderBySelect_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereOrderBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age + @p2 AS Proj0
            FROM 
                customers a0
            WHERE 
                a0.Age > @p0 AND a0.Name != @p1
            ORDER BY 
                a0.Age ASC
            """, ["@p0", "@p1", "@p2"])),
        new((DatabaseType.SQLite, "FromWhereOrderBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age + :p2 AS Proj0
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0 AND a0.Name != :p1
            ORDER BY 
                a0.Age ASC
            """, [":p0", ":p1", ":p2"])),
        new((DatabaseType.PostgreSQL, "FromWhereOrderBySelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name,
                a0.Age + :p2 AS Proj0
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0 AND a0.Name != :p1
            ORDER BY 
                a0.Age ASC
            """, [":p0", ":p1", ":p2"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereOrderByThenBy_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereOrderByThenBy_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > @p0
        ORDER BY 
            a0.Name ASC, a0.Age DESC
        """, ["@p0"])),
        new((DatabaseType.SQLite, "FromWhereOrderByThenBy_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0
        ORDER BY 
            a0.Name ASC, a0.Age DESC
        """, [":p0"])),
        new((DatabaseType.PostgreSQL, "FromWhereOrderByThenBy_GeneratesCorrectSql"), ("""
        SELECT 
            a0.Id AS Id,
            a0.Age AS Age,
            a0.Name AS Name,
            a0.IsActive AS IsActive
        FROM 
            customers a0
        WHERE 
            a0.Age > :p0
        ORDER BY 
            a0.Name ASC, a0.Age DESC
        """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereOrderBy_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereOrderBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > @p0 AND a0.Name != @p1
            ORDER BY 
                a0.Age ASC
            """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "FromWhereOrderBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0 AND a0.Name != :p1
            ORDER BY 
                a0.Age ASC
            """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "FromWhereOrderBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Age AS Age,
                a0.Name AS Name,
                a0.IsActive AS IsActive
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0 AND a0.Name != :p1
            ORDER BY 
                a0.Age ASC
            """, [":p0", ":p1"]))
    ];

    // WHERE SELECT Patterns
    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereSelectNamed_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereSelectNamed_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS OriginalId,
                a0.Id * @p1 + a0.Age AS ModifiedId,
                a0.Name AS CustomerName
            FROM 
                customers a0
            WHERE 
                a0.Age > @p0
            """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "FromWhereSelectNamed_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS OriginalId,
                a0.Id * :p1 + a0.Age AS ModifiedId,
                a0.Name AS CustomerName
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0
            """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "FromWhereSelectNamed_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS OriginalId,
                a0.Id * :p1 + a0.Age AS ModifiedId,
                a0.Name AS CustomerName
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0
            """, [":p0", ":p1"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereSelectOrderBy_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereSelectOrderBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id + @p1 AS Proj0,
                CONCAT(a0.Name, @p2) AS Proj1
            FROM 
                customers a0
            WHERE 
                a0.Age > @p0
            ORDER BY 
                a0.Name ASC
            """, ["@p0", "@p1", "@p2"])),
        new((DatabaseType.SQLite, "FromWhereSelectOrderBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id + :p1 AS Proj0,
                a0.Name || :p2 AS Proj1
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0
            ORDER BY 
                a0.Name ASC
            """, [":p0", ":p1", ":p2"])),
        new((DatabaseType.PostgreSQL, "FromWhereSelectOrderBy_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id + :p1 AS Proj0,
                a0.Name || :p2 AS Proj1
            FROM 
                customers a0
            WHERE 
                a0.Age > :p0
            ORDER BY 
                a0.Name ASC
            """, [":p0", ":p1", ":p2"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereSelectParameterized_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereSelectParameterized_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age >= @p0 AND a0.Age <= @p1
            """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "FromWhereSelectParameterized_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age >= :p0 AND a0.Age <= :p1
            """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "FromWhereSelectParameterized_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age >= :p0 AND a0.Age <= :p1
            """, [":p0", ":p1"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereSelectWhereFromNested_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereSelectWhereFromNested_GeneratesCorrectSql"), ("""
            SELECT 
                a1.Id AS Id,
                a1.Name AS Name
            FROM 
                (SELECT 
                    a0.Id AS Id,
                    a0.Name AS Name
                FROM 
                    customers a0
                WHERE 
                    a0.Age > @p0) a1
            WHERE 
                a1.Id > @p1
            """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "FromWhereSelectWhereFromNested_GeneratesCorrectSql"), ("""
            SELECT 
                a1.Id AS Id,
                a1.Name AS Name
            FROM 
                (SELECT 
                    a0.Id AS Id,
                    a0.Name AS Name
                FROM 
                    customers a0
                WHERE 
                    a0.Age > :p0) a1
            WHERE 
                a1.Id > :p1
            """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "FromWhereSelectWhereFromNested_GeneratesCorrectSql"), ("""
            SELECT 
                a1.Id AS Id,
                a1.Name AS Name
            FROM 
                (SELECT 
                    a0.Id AS Id,
                    a0.Name AS Name
                FROM 
                    customers a0
                WHERE 
                    a0.Age > :p0) a1
            WHERE 
                a1.Id > :p1
            """, [":p0", ":p1"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereSelectWhereNested_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "FromWhereSelectWhereNested_GeneratesCorrectSql"), ("""
            SELECT 
                a1.Id AS Id,
                a1.Name AS Name
            FROM 
                (SELECT 
                    a0.Id AS Id,
                    a0.Name AS Name
                FROM 
                    customers a0
                WHERE 
                    a0.Age > @p0) a1
            WHERE 
                a1.Id > @p1
            """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "FromWhereSelectWhereNested_GeneratesCorrectSql"), ("""
            SELECT 
                a1.Id AS Id,
                a1.Name AS Name
            FROM 
                (SELECT 
                    a0.Id AS Id,
                    a0.Name AS Name
                FROM 
                    customers a0
                WHERE 
                    a0.Age > :p0) a1
            WHERE 
                a1.Id > :p1
            """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "FromWhereSelectWhereNested_GeneratesCorrectSql"), ("""
            SELECT 
                a1.Id AS Id,
                a1.Name AS Name
            FROM 
                (SELECT 
                    a0.Id AS Id,
                    a0.Name AS Name
                FROM 
                    customers a0
                WHERE 
                    a0.Age > :p0) a1
            WHERE 
                a1.Id > :p1
            """, [":p0", ":p1"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] FromWhereSelect_GeneratesCorrectSqlPair =
        [
            new((DatabaseType.SqlServer, "FromWhereSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age >= @p0
            """, ["@p0"])),
            new((DatabaseType.SQLite, "FromWhereSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age >= :p0
            """, [":p0"])),
            new((DatabaseType.PostgreSQL, "FromWhereSelect_GeneratesCorrectSql"), ("""
            SELECT 
                a0.Id AS Id,
                a0.Name AS Name
            FROM 
                customers a0
            WHERE 
                a0.Age >= :p0
            """, [":p0"]))
        ];
    public static readonly Dictionary<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)> TestCases = 
        From_GeneratesCorrectSqlPair
            .Concat(AbsColumn_GeneratesCorrectSqlPair)
            .Concat(AbsExpression_GeneratesCorrectSqlPair)
            .Concat(AbsInWhere_GeneratesCorrectSqlPair)
            .Concat(AbsParameter_GeneratesCorrectSqlPair)
            .Concat(BoolColumnDirectComparison_GeneratesCorrectSqlPair)
            .Concat(BoolColumnLiteralFalse_GeneratesCorrectSqlPair)
            .Concat(BoolColumnLiteralTrue_GeneratesCorrectSqlPair)
            .Concat(CaseBoolExpression_GeneratesCorrectSqlPair)
            .Concat(CaseDateTimeExpression_GeneratesCorrectSqlPair)
            .Concat(CaseDecimalExpression_GeneratesCorrectSqlPair)
            .Concat(CaseGuidExpression_GeneratesCorrectSqlPair)
            .Concat(CaseIntExpression_GeneratesCorrectSqlPair)
            .Concat(CaseInWhere_GeneratesCorrectSqlPair)
            .Concat(CaseStringExpression_GeneratesCorrectSqlPair)
            .Concat(ComplexJoinWhereGroupByHavingOrderBySelect_GeneratesCorrectSqlPair)
            .Concat(ComplexLeftJoinWhereGroupByOrderBySelect_GeneratesCorrectSqlPair)
            .Concat(FromGroupByMinMaxSelect_GeneratesCorrectSqlPair)
            .Concat(FromGroupByAvgSelect_GeneratesCorrectSqlPair)
            .Concat(FromGroupByDecimalAggregatesSelect_GeneratesCorrectSqlPair)
            .Concat(FromGroupByDecimalAvgSelect_GeneratesCorrectSqlPair)
            .Concat(FromGroupByDecimalSumSelect_GeneratesCorrectSqlPair)
            .Concat(FromGroupByHavingOrderBySelect_GeneratesCorrectSqlPair)
            .Concat(FromGroupByHavingSelect_GeneratesCorrectSqlPair)
            .Concat(FromGroupByMultipleOrderBySelect_GeneratesCorrectSqlPair)
            .Concat(FromGroupByMultipleSelect_GeneratesCorrectSqlPair)
            .Concat(FromGroupByOrderByMultipleSelect_GeneratesCorrectSqlPair)
            .Concat(FromGroupByOrderBySelect_GeneratesCorrectSqlPair)
            .Concat(FromGroupByOrderByThreeKeysSelect_GeneratesCorrectSqlPair)
            .Concat(FromGroupBySelect_GeneratesCorrectSqlPair)
            .Concat(FromOrderByAsc_GeneratesCorrectSqlPair)
            .Concat(FromOrderByDesc_GeneratesCorrectSqlPair)
            .Concat(FromOrderByDescendingThenBy_GeneratesCorrectSqlPair)
            .Concat(FromOrderByMultiple_GeneratesCorrectSqlPair)
            .Concat(FromOrderByThenByDescending_GeneratesCorrectSqlPair)
            .Concat(FromOrderByThenBySelect_GeneratesCorrectSqlPair)
            .Concat(FromOrderByThenBy_GeneratesCorrectSqlPair)
            .Concat(FromProductWhereSelect_GeneratesCorrectSqlPair)
            .Concat(FromSelectAvg_GeneratesCorrectSqlPair)
            .Concat(FromSelectCreatedDateMinMax_GeneratesCorrectSqlPair)
            .Concat(FromSelectDecimalArithmetic_GeneratesCorrectSqlPair)
            .Concat(FromSelectExpression_GeneratesCorrectSqlPair)
            .Concat(FromSelectMax_GeneratesCorrectSqlPair)
            .Concat(FromSelectMin_GeneratesCorrectSqlPair)
            .Concat(FromSelectOrderBy_GeneratesCorrectSqlPair)
            .Concat(FromSelectSingle_GeneratesCorrectSqlPair)
            .Concat(FromSelectSum_GeneratesCorrectSqlPair)
            .Concat(FromSelect_GeneratesCorrectSqlPair)
            .Concat(FromStatic_GeneratesCorrectSqlPair)
            .Concat(FromSubquery_GeneratesCorrectSqlPair)
            .Concat(FromWhereAgeGreaterThanAverageAge_GeneratesCorrectSqlPair)
            .Concat(FromWhereAgeGreaterThanSum_GeneratesCorrectSqlPair)
            .Concat(FromWhereAgeInSubqueryWithClosure_GeneratesCorrectSqlPair)
            .Concat(FromWhereAgeInSubquery_GeneratesCorrectSqlPair)
            .Concat(FromWhereAgeIn_GeneratesCorrectSqlPair)
            .Concat(FromWhereAndSelect_GeneratesCorrectSqlPair)
            .Concat(FromWhereAnd_GeneratesCorrectSqlPair)
            .Concat(FromWhereCreatedDateComparison_GeneratesCorrectSqlPair)
            .Concat(FromWhereCreatedDateIsNotNull_GeneratesCorrectSqlPair)
            .Concat(FromWhereCreatedDateIsNull_GeneratesCorrectSqlPair)
            .Concat(FromWhereDecimalComparison_GeneratesCorrectSqlPair)
            .Concat(FromWhereDecimalIsNotNull_GeneratesCorrectSqlPair)
            .Concat(FromWhereDecimalIsNull_GeneratesCorrectSqlPair)
            .Concat(FromWhereFusionThree_GeneratesCorrectSqlPair)
            .Concat(FromWhereFusionTwo_GeneratesCorrectSqlPair)
            .Concat(FromWhereFusionWithOrderBy_GeneratesCorrectSqlPair)
            .Concat(FromWhereFusionWithSelect_GeneratesCorrectSqlPair)
            .Concat(FromWhereGroupBySelect_GeneratesCorrectSqlPair)
            .Concat(FromWhereInt_GeneratesCorrectSqlPair)
            .Concat(FromWhereIsNotNullInt_GeneratesCorrectSqlPair)
            .Concat(FromWhereIsNotNull_GeneratesCorrectSqlPair)
            .Concat(FromWhereIsNullCombined_GeneratesCorrectSqlPair)
            .Concat(FromWhereIsNullInt_GeneratesCorrectSqlPair)
            .Concat(FromWhereIsNull_GeneratesCorrectSqlPair)
            .Concat(FromWhereMultiple_GeneratesCorrectSqlPair)
            .Concat(FromWhereOr_GeneratesCorrectSqlPair)
            .Concat(FromWhereOrderBySelect_GeneratesCorrectSqlPair)
            .Concat(FromWhereOrderByThenBy_GeneratesCorrectSqlPair)
            .Concat(FromWhereOrderBy_GeneratesCorrectSqlPair)
            .Concat(FromWhereSelectNamed_GeneratesCorrectSqlPair)
            .Concat(FromWhereSelectOrderBy_GeneratesCorrectSqlPair)
            .Concat(FromWhereSelectParameterized_GeneratesCorrectSqlPair)
            .Concat(FromWhereSelectWhereFromNested_GeneratesCorrectSqlPair)
            .Concat(FromWhereSelectWhereNested_GeneratesCorrectSqlPair)
            .Concat(FromWhereSelect_GeneratesCorrectSqlPair)
            .Concat(AvgExpensivePrices_GeneratesCorrectSqlPair)
            .Concat(AvgPrices_GeneratesCorrectSqlPair)
            .Concat(MinPrice_GeneratesCorrectSqlPair)
            .Concat(MaxPrice_GeneratesCorrectSqlPair)            
            .Concat(CountActiveCustomers_GeneratesCorrectSqlPair)            
            .Concat(CountCustomers_GeneratesCorrectSqlPair)
            .Concat(DateTimeAddDays_GeneratesCorrectSqlPair)
            .Concat(DateTimeAddMonths_GeneratesCorrectSqlPair)
            .Concat(DateTimeAddYears_GeneratesCorrectSqlPair)
            .Concat(DateTimeDay_GeneratesCorrectSqlPair)
            .Concat(DateTimeDiffDays_GeneratesCorrectSqlPair)
            .Concat(DateTimeDiffMonths_GeneratesCorrectSqlPair)
            .Concat(DateTimeDiffYears_GeneratesCorrectSqlPair)
            .Concat(DateTimeMonth_GeneratesCorrectSqlPair)
            .Concat(DateTimeNow_GeneratesCorrectSqlPair)
            .Concat(DateTimeYear_GeneratesCorrectSqlPair)
            .Concat(DecimalCeiling_GeneratesCorrectSqlPair)
            .Concat(DecimalFloor_GeneratesCorrectSqlPair)
            .Concat(DecimalRound_GeneratesCorrectSqlPair)            
            .Concat(SumAges_GeneratesCorrectSqlPair)
            .Concat(SumExpensivePrices_GeneratesCorrectSqlPair)
            .Concat(SumPrices_GeneratesCorrectSqlPair)
            .Concat(StringFunctionsInSelect_GeneratesCorrectSqlPair)
            .Concat(StringFunctionsInWhere_GeneratesCorrectSqlPair)
            .Concat(StringLength_GeneratesCorrectSqlPair)
            .Concat(StringLower_GeneratesCorrectSqlPair)
            .Concat(StringSubstring_GeneratesCorrectSqlPair)
            .Concat(StringTrim_GeneratesCorrectSqlPair)
            .Concat(StringUpper_GeneratesCorrectSqlPair)
            .Concat(DateTimeFunctionsInSelect_GeneratesCorrectSqlPair)
            .Concat(DateTimeFunctionsInWhere_GeneratesCorrectSqlPair)
            .Concat(FromWhereString_GeneratesCorrectSqlPair)
            .Concat(FromWhereUniqueIdEquals_GeneratesCorrectSqlPair)
            .Concat(FromWhereUniqueIdIsNotNull_GeneratesCorrectSqlPair)
            .Concat(FromWhereUniqueIdIsNull_GeneratesCorrectSqlPair)
            .Concat(FromWhereUniqueIdNotEquals_GeneratesCorrectSqlPair)
            .Concat(InnerJoinBasic_GeneratesCorrectSqlPair)
            .Concat(InnerJoinWithSelect_GeneratesCorrectSqlPair)
            .Concat(InnerJoinWithWhere_GeneratesCorrectSqlPair)
            .Concat(InnerJoinWithOrderBy_GeneratesCorrectSqlPair)
            .Concat(InnerJoinWithGroupBy_GeneratesCorrectSqlPair)
            .Concat(LeftJoinBasic_GeneratesCorrectSqlPair)
            .Concat(LikeWildcard_GeneratesCorrectSqlPair)
            .Concat(LikeBothWildcards_GeneratesCorrectSqlPair)
            .Concat(LikeExact_GeneratesCorrectSqlPair)
            .Concat(LikeSingleChar_GeneratesCorrectSqlPair)
            .Concat(MathFunctionsInSelect_GeneratesCorrectSqlPair)
            .Concat(MathFunctionsInWhere_GeneratesCorrectSqlPair)
            .Concat(MixedJoinTypesFusion_GeneratesCorrectSqlPair)
            .Concat(MultipleInnerJoinsFusion_GeneratesCorrectSqlPair)
            .Concat(ParameterAsBoolParam_GeneratesCorrectSqlPair)
            .Concat(ParameterAsDateTimeParam_GeneratesCorrectSqlPair)
            .Concat(ParameterAsDecimalParam_GeneratesCorrectSqlPair)
            .Concat(ParameterAsGuidParam_GeneratesCorrectSqlPair)
            .Concat(ParameterAsIntParam_GeneratesCorrectSqlPair)
            .Concat(ParameterAsStringParam_GeneratesCorrectSqlPair)
            .Concat(JoinFusionWithWhere_GeneratesCorrectSqlPair)
            .Concat(LeftJoinWithAggregates_GeneratesCorrectSqlPair)
            .Concat(LeftJoinWithOrderBy_GeneratesCorrectSqlPair)
            .Concat(LeftJoinWithSelect_GeneratesCorrectSqlPair)
            .Concat(LeftJoinWithWhere_GeneratesCorrectSqlPair)
            .ToDictionary(kv => kv.Key, kv => kv.Value);


}
