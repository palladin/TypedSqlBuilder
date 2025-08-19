using System.Collections.Immutable;
using System.Linq;

namespace TypedSqlBuilder.Core;

/// <summary>
/// Provides access to SQL aggregate functions for use in GROUP BY queries.
/// This class encapsulates all available aggregate operations that can be applied to grouped data.
/// </summary>
public class SqlAggregateFunc
{
    /// <summary>
    /// Internal constructor to prevent external instantiation.
    /// This class should only be instantiated by the TypedSqlBuilder framework.
    /// </summary>
    internal SqlAggregateFunc()
    {
    }

    /// <summary>
    /// Applies the COUNT aggregate function.
    /// Counts the number of rows in the current group.
    /// </summary>
    /// <returns>A SQL integer expression representing the count</returns>
    public SqlExprInt Count()
    {
        return new SqlIntCount();
    }

    /// <summary>
    /// Applies the SUM aggregate function to an integer expression.
    /// Computes the sum of all values in the current group.
    /// </summary>
    /// <param name="value">The integer expression to sum</param>
    /// <returns>A SQL integer expression representing the sum</returns>
    public SqlExprInt Sum(SqlExprInt value)
    {
        return new SqlIntSum(value);
    }

    /// <summary>
    /// Applies the AVG aggregate function to an integer expression.
    /// Computes the average of all values in the current group.
    /// </summary>
    /// <param name="value">The integer expression to average</param>
    /// <returns>A SQL integer expression representing the average</returns>
    public SqlExprInt Avg(SqlExprInt value)
    {
        return new SqlIntAvg(value);
    }

    /// <summary>
    /// Applies the MIN aggregate function to an integer expression.
    /// Finds the minimum value in the current group.
    /// </summary>
    /// <param name="value">The integer expression to find the minimum of</param>
    /// <returns>A SQL integer expression representing the minimum</returns>
    public SqlExprInt Min(SqlExprInt value)
    {
        return new SqlIntMin(value);
    }

    /// <summary>
    /// Applies the MAX aggregate function to an integer expression.
    /// Finds the maximum value in the current group.
    /// </summary>
    /// <param name="value">The integer expression to find the maximum of</param>
    /// <returns>A SQL integer expression representing the maximum</returns>
    public SqlExprInt Max(SqlExprInt value)
    {
        return new SqlIntMax(value);
    }

    /// <summary>
    /// Applies the SUM aggregate function to a decimal expression.
    /// Computes the sum of all values in the current group.
    /// </summary>
    /// <param name="value">The decimal expression to sum</param>
    /// <returns>A SQL decimal expression representing the sum</returns>
    public SqlExprDecimal Sum(SqlExprDecimal value)
    {
        return new SqlDecimalSum(value);
    }

    /// <summary>
    /// Applies the AVG aggregate function to a decimal expression.
    /// Computes the average of all values in the current group.
    /// </summary>
    /// <param name="value">The decimal expression to average</param>
    /// <returns>A SQL decimal expression representing the average</returns>
    public SqlExprDecimal Avg(SqlExprDecimal value)
    {
        return new SqlDecimalAvg(value);
    }

    /// <summary>
    /// Applies the MIN aggregate function to a decimal expression.
    /// Finds the minimum value in the current group.
    /// </summary>
    /// <param name="value">The decimal expression to find the minimum of</param>
    /// <returns>A SQL decimal expression representing the minimum</returns>
    public SqlExprDecimal Min(SqlExprDecimal value)
    {
        return new SqlDecimalMin(value);
    }

    /// <summary>
    /// Applies the MAX aggregate function to a decimal expression.
    /// Finds the maximum value in the current group.
    /// </summary>
    /// <param name="value">The decimal expression to find the maximum of</param>
    /// <returns>A SQL decimal expression representing the maximum</returns>
    public SqlExprDecimal Max(SqlExprDecimal value)
    {
        return new SqlDecimalMax(value);
    }
}

/// <summary>
/// Provides SQL function utilities and extension methods for creating SQL expressions.
/// This static class contains methods for creating SQL parameters, aggregate functions,
/// and various SQL operations in a type-safe manner.
/// </summary>
public static class SqlFunc
{
    /// <summary>
    /// Creates a SQL integer parameter from a string name.
    /// Extension method that allows string literals to be converted to SQL parameters.
    /// </summary>
    /// <param name="name">The parameter name (e.g., "@userId", ":count")</param>
    /// <returns>A SQL integer parameter expression</returns>
    public static SqlExprInt AsIntParam(this string name)
    {
        return new SqlParameterInt(name);
    }
    
    /// <summary>
    /// Creates a SQL string parameter from a string name.
    /// Extension method that allows string literals to be converted to SQL parameters.
    /// </summary>
    /// <param name="name">The parameter name (e.g., "@userName", ":email")</param>
    /// <returns>A SQL string parameter expression</returns>
    public static SqlExprString AsStringParam(this string name)
    {
        return new SqlParameterString(name);
    }

    /// <summary>
    /// Creates a SQL boolean parameter from a string name.
    /// Extension method that allows string literals to be converted to SQL parameters.
    /// </summary>
    /// <param name="name">The parameter name (e.g., "@isActive", ":enabled")</param>
    /// <returns>A SQL boolean parameter expression</returns>
    public static SqlExprBool AsBoolParam(this string name)
    {
        return new SqlParameterBool(name);
    }

    /// <summary>
    /// Creates a SQL decimal parameter from a string name.
    /// Extension method that allows string literals to be converted to SQL parameters.
    /// </summary>
    /// <param name="name">The parameter name (e.g., "@price", ":amount")</param>
    /// <returns>A SQL decimal parameter expression</returns>
    public static SqlExprDecimal AsDecimalParam(this string name)
    {
        return new SqlParameterDecimal(name);
    }

    /// <summary>
    /// Creates a SQL DateTime parameter from a string name.
    /// Extension method that allows string literals to be converted to SQL parameters.
    /// </summary>
    /// <param name="name">The parameter name (e.g., "@createdDate", ":timestamp")</param>
    /// <returns>A SQL DateTime parameter expression</returns>
    public static SqlExprDateTime AsDateTimeParam(this string name)
    {
        return new SqlParameterDateTime(name);
    }

    /// <summary>
    /// Creates a SQL Guid parameter from a string name.
    /// Extension method that allows string literals to be converted to SQL parameters.
    /// </summary>
    /// <param name="name">The parameter name (e.g., "@uniqueId", ":guid")</param>
    /// <returns>A SQL Guid parameter expression</returns>
    public static SqlExprGuid AsGuidParam(this string name)
    {
        return new SqlParameterGuid(name);
    }

    /// <summary>
    /// Creates a SQL ABS() function expression for the given integer expression.
    /// Returns the absolute (non-negative) value of the input.
    /// </summary>
    /// <param name="value">The integer expression to get the absolute value of</param>
    /// <returns>A SQL integer expression representing ABS(value)</returns>
    public static SqlExprInt Abs(this SqlExprInt value)
    {
        return new SqlIntAbs(value);
    }
    
    /// <summary>
    /// Creates a SQL LIKE pattern matching expression for string comparison.
    /// Supports SQL wildcard patterns: % (matches any sequence) and _ (matches single character).
    /// </summary>
    /// <param name="value">The string expression to match against the pattern</param>
    /// <param name="pattern">The pattern to match (can include wildcards % and _)</param>
    /// <returns>A SQL boolean expression representing value LIKE pattern</returns>
    public static SqlExprBool Like(this SqlExprString value, string pattern)
    {
        return new SqlStringLike(value, pattern);
    }

    // ========== STRING FUNCTIONS ==========
    
    /// <summary>
    /// Creates a SQL SUBSTRING function for extracting part of a string.
    /// Equivalent to: SUBSTRING(value, start, length)
    /// </summary>
    /// <param name="value">The string expression to extract from</param>
    /// <param name="start">The starting position (1-based)</param>
    /// <param name="length">The number of characters to extract</param>
    /// <returns>A SQL string expression representing the substring</returns>
    public static SqlExprString Substring(this SqlExprString value, SqlExprInt start, SqlExprInt length)
    {
        return new SqlStringSubstring(value, start, length);
    }

    /// <summary>
    /// Creates a SQL UPPER function for converting string to uppercase.
    /// Equivalent to: UPPER(value)
    /// </summary>
    /// <param name="value">The string expression to convert to uppercase</param>
    /// <returns>A SQL string expression representing the uppercase string</returns>
    public static SqlExprString Upper(this SqlExprString value)
    {
        return new SqlStringUpper(value);
    }

    /// <summary>
    /// Creates a SQL LOWER function for converting string to lowercase.
    /// Equivalent to: LOWER(value)
    /// </summary>
    /// <param name="value">The string expression to convert to lowercase</param>
    /// <returns>A SQL string expression representing the lowercase string</returns>
    public static SqlExprString Lower(this SqlExprString value)
    {
        return new SqlStringLower(value);
    }

    /// <summary>
    /// Creates a SQL TRIM function for removing whitespace from both ends.
    /// Equivalent to: TRIM(value)
    /// </summary>
    /// <param name="value">The string expression to trim</param>
    /// <returns>A SQL string expression representing the trimmed string</returns>
    public static SqlExprString Trim(this SqlExprString value)
    {
        return new SqlStringTrim(value);
    }

    /// <summary>
    /// Creates a SQL LEN/LENGTH function for getting string length.
    /// Equivalent to: LEN(value) or LENGTH(value)
    /// </summary>
    /// <param name="value">The string expression to get the length of</param>
    /// <returns>A SQL integer expression representing the string length</returns>
    public static SqlExprInt Length(this SqlExprString value)
    {
        return new SqlStringLength(value);
    }

    // ========== DATE/TIME FUNCTIONS ==========
    
    /// <summary>
    /// Gets the current date and time using SQL GETDATE() or NOW() function.
    /// Equivalent to: GETDATE() (SQL Server) or NOW() (PostgreSQL/MySQL)
    /// </summary>
    /// <returns>A SQL DateTime expression representing the current date/time</returns>
    public static SqlExprDateTime Now()
    {
        return SqlDateTimeNow.Value;
    }

    /// <summary>
    /// Creates a SQL YEAR function for extracting year from date.
    /// Equivalent to: YEAR(date)
    /// </summary>
    /// <param name="value">The DateTime expression to extract year from</param>
    /// <returns>A SQL integer expression representing the year</returns>
    public static SqlExprInt Year(this SqlExprDateTime value)
    {
        return new SqlDateTimeYear(value);
    }

    /// <summary>
    /// Creates a SQL MONTH function for extracting month from date.
    /// Equivalent to: MONTH(date)
    /// </summary>
    /// <param name="value">The DateTime expression to extract month from</param>
    /// <returns>A SQL integer expression representing the month</returns>
    public static SqlExprInt Month(this SqlExprDateTime value)
    {
        return new SqlDateTimeMonth(value);
    }

    /// <summary>
    /// Creates a SQL DAY function for extracting day from date.
    /// Equivalent to: DAY(date)
    /// </summary>
    /// <param name="value">The DateTime expression to extract day from</param>
    /// <returns>A SQL integer expression representing the day</returns>
    public static SqlExprInt Day(this SqlExprDateTime value)
    {
        return new SqlDateTimeDay(value);
    }

    /// <summary>
    /// Creates a SQL DATEADD function for adding days to a date.
    /// Equivalent to: DATEADD(day, number, date)
    /// </summary>
    /// <param name="date">The DateTime expression to add to</param>
    /// <param name="days">The number of days to add</param>
    /// <returns>A SQL DateTime expression representing the new date</returns>
    public static SqlExprDateTime AddDays(this SqlExprDateTime date, SqlExprInt days)
    {
        return new SqlDateTimeAdd("day", days, date);
    }

    /// <summary>
    /// Creates a SQL DATEADD function for adding months to a date.
    /// Equivalent to: DATEADD(month, number, date)
    /// </summary>
    /// <param name="date">The DateTime expression to add to</param>
    /// <param name="months">The number of months to add</param>
    /// <returns>A SQL DateTime expression representing the new date</returns>
    public static SqlExprDateTime AddMonths(this SqlExprDateTime date, SqlExprInt months)
    {
        return new SqlDateTimeAdd("month", months, date);
    }

    /// <summary>
    /// Creates a SQL DATEADD function for adding years to a date.
    /// Equivalent to: DATEADD(year, number, date)
    /// </summary>
    /// <param name="date">The DateTime expression to add to</param>
    /// <param name="years">The number of years to add</param>
    /// <returns>A SQL DateTime expression representing the new date</returns>
    public static SqlExprDateTime AddYears(this SqlExprDateTime date, SqlExprInt years)
    {
        return new SqlDateTimeAdd("year", years, date);
    }

    /// <summary>
    /// Creates a SQL DATEDIFF function for calculating difference in days between dates.
    /// Equivalent to: DATEDIFF(day, startdate, enddate)
    /// </summary>
    /// <param name="startdate">The start DateTime expression</param>
    /// <param name="enddate">The end DateTime expression</param>
    /// <returns>A SQL integer expression representing the difference in days</returns>
    public static SqlExprInt DiffDays(SqlExprDateTime startdate, SqlExprDateTime enddate)
    {
        return new SqlDateTimeDiff("day", startdate, enddate);
    }

    /// <summary>
    /// Creates a SQL DATEDIFF function for calculating difference in months between dates.
    /// Equivalent to: DATEDIFF(month, startdate, enddate)
    /// </summary>
    /// <param name="startdate">The start DateTime expression</param>
    /// <param name="enddate">The end DateTime expression</param>
    /// <returns>A SQL integer expression representing the difference in months</returns>
    public static SqlExprInt DiffMonths(SqlExprDateTime startdate, SqlExprDateTime enddate)
    {
        return new SqlDateTimeDiff("month", startdate, enddate);
    }

    /// <summary>
    /// Creates a SQL DATEDIFF function for calculating difference in years between dates.
    /// Equivalent to: DATEDIFF(year, startdate, enddate)
    /// </summary>
    /// <param name="startdate">The start DateTime expression</param>
    /// <param name="enddate">The end DateTime expression</param>
    /// <returns>A SQL integer expression representing the difference in years</returns>
    public static SqlExprInt DiffYears(SqlExprDateTime startdate, SqlExprDateTime enddate)
    {
        return new SqlDateTimeDiff("year", startdate, enddate);
    }

    // ========== MATHEMATICAL FUNCTIONS ==========
    
    /// <summary>
    /// Creates a SQL ROUND function for rounding decimal values to specified precision.
    /// Equivalent to: ROUND(value, precision)
    /// </summary>
    /// <param name="value">The decimal expression to round</param>
    /// <param name="precision">The number of decimal places</param>
    /// <returns>A SQL decimal expression representing the rounded value</returns>
    public static SqlExprDecimal Round(this SqlExprDecimal value, SqlExprInt precision)
    {
        return new SqlDecimalRound(value, precision);
    }

    /// <summary>
    /// Creates a SQL CEILING function for rounding up to nearest integer.
    /// Equivalent to: CEILING(value) or CEIL(value)
    /// </summary>
    /// <param name="value">The decimal expression to round up</param>
    /// <returns>A SQL integer expression representing the ceiling value</returns>
    public static SqlExprInt Ceiling(this SqlExprDecimal value)
    {
        return new SqlDecimalCeiling(value);
    }

    /// <summary>
    /// Creates a SQL FLOOR function for rounding down to nearest integer.
    /// Equivalent to: FLOOR(value)
    /// </summary>
    /// <param name="value">The decimal expression to round down</param>
    /// <returns>A SQL integer expression representing the floor value</returns>
    public static SqlExprInt Floor(this SqlExprDecimal value)
    {
        return new SqlDecimalFloor(value);
    }

    /// <summary>
    /// Creates a SQL CASE expression for conditional string values.
    /// Equivalent to: CASE WHEN condition THEN trueValue ELSE falseValue END
    /// </summary>
    /// <param name="condition">The boolean condition to evaluate</param>
    /// <param name="trueValue">The value returned when condition is true</param>
    /// <param name="falseValue">The value returned when condition is false</param>
    /// <returns>A SQL string expression representing the CASE statement</returns>
    public static SqlExprString Case(SqlExprBool condition, SqlExprString trueValue, SqlExprString falseValue)
    {
        return new SqlStringCase(condition, trueValue, falseValue);
    }

    /// <summary>
    /// Creates a SQL CASE expression for conditional integer values.
    /// Equivalent to: CASE WHEN condition THEN trueValue ELSE falseValue END
    /// </summary>
    /// <param name="condition">The boolean condition to evaluate</param>
    /// <param name="trueValue">The integer value returned when condition is true</param>
    /// <param name="falseValue">The integer value returned when condition is false</param>
    /// <returns>A SQL integer expression representing the CASE statement</returns>
    public static SqlExprInt Case(SqlExprBool condition, SqlExprInt trueValue, SqlExprInt falseValue)
    {
        return new SqlIntCase(condition, trueValue, falseValue);
    }

    /// <summary>
    /// Creates a SQL CASE expression for conditional boolean values.
    /// Equivalent to: CASE WHEN condition THEN trueValue ELSE falseValue END
    /// </summary>
    /// <param name="condition">The boolean condition to evaluate</param>
    /// <param name="trueValue">The boolean value returned when condition is true</param>
    /// <param name="falseValue">The boolean value returned when condition is false</param>
    /// <returns>A SQL boolean expression representing the CASE statement</returns>
    public static SqlExprBool Case(SqlExprBool condition, SqlExprBool trueValue, SqlExprBool falseValue)
    {
        return new SqlBoolCase(condition, trueValue, falseValue);
    }

    /// <summary>
    /// Creates a SQL IN expression for integer values.
    /// Equivalent to: expr IN (value1, value2, value3, ...)
    /// </summary>
    /// <param name="expr">The integer expression to test</param>
    /// <param name="values">The values to test against</param>
    /// <returns>A SQL boolean expression representing the IN operation</returns>
    public static SqlExprBool In(this SqlExprInt expr, params SqlExprInt[] values)
    {
        return new SqlInValues<SqlExprInt>(expr, values.ToImmutableArray());
    }

    /// <summary>
    /// Creates a SQL IN expression for string values.
    /// Equivalent to: expr IN (value1, value2, value3, ...)
    /// </summary>
    /// <param name="expr">The string expression to test</param>
    /// <param name="values">The values to test against</param>
    /// <returns>A SQL boolean expression representing the IN operation</returns>
    public static SqlExprBool In(this SqlExprString expr, params SqlExprString[] values)
    {
        return new SqlInValues<SqlExprString>(expr, values.ToImmutableArray());
    }

    /// <summary>
    /// Creates a SQL IN expression for boolean values.
    /// Equivalent to: expr IN (value1, value2, value3, ...)
    /// </summary>
    /// <param name="expr">The boolean expression to test</param>
    /// <param name="values">The values to test against</param>
    /// <returns>A SQL boolean expression representing the IN operation</returns>
    public static SqlExprBool In(this SqlExprBool expr, params SqlExprBool[] values)
    {
        return new SqlInValues<SqlExprBool>(expr, values.ToImmutableArray());
    }

    /// <summary>
    /// Creates a SQL IN expression with an integer subquery.
    /// Equivalent to: expr IN (SELECT ... FROM ...)
    /// </summary>
    /// <param name="expr">The integer expression to test</param>
    /// <param name="subQuery">The subquery that returns integer values</param>
    /// <returns>A SQL boolean expression representing the IN operation with subquery</returns>
    public static SqlExprBool In(this SqlExprInt expr, ISqlQuery<ValueTuple<SqlExprInt>> subQuery)
    {
        return new SqlInSubQuery<SqlExprInt>(expr, subQuery);
    }

    /// <summary>
    /// Creates a SQL IN expression with a string subquery.
    /// Equivalent to: expr IN (SELECT ... FROM ...)
    /// </summary>
    /// <param name="expr">The string expression to test</param>
    /// <param name="subQuery">The subquery that returns string values</param>
    /// <returns>A SQL boolean expression representing the IN operation with subquery</returns>
    public static SqlExprBool In(this SqlExprString expr, ISqlQuery<ValueTuple<SqlExprString>> subQuery)
    {
        return new SqlInSubQuery<SqlExprString>(expr, subQuery);
    }

    /// <summary>
    /// Creates a SQL IN expression with a boolean subquery.
    /// Equivalent to: expr IN (SELECT ... FROM ...)
    /// </summary>
    /// <param name="expr">The boolean expression to test</param>
    /// <param name="subQuery">The subquery that returns boolean values</param>
    /// <returns>A SQL boolean expression representing the IN operation with subquery</returns>
    public static SqlExprBool In(this SqlExprBool expr, ISqlQuery<ValueTuple<SqlExprBool>> subQuery)
    {
        return new SqlInSubQuery<SqlExprBool>(expr, subQuery);
    }

    /// <summary>
    /// Creates a SQL IN expression for decimal values.
    /// Equivalent to: expr IN (value1, value2, value3, ...)
    /// </summary>
    /// <param name="expr">The decimal expression to test</param>
    /// <param name="values">The values to test against</param>
    /// <returns>A SQL boolean expression representing the IN operation</returns>
    public static SqlExprBool In(this SqlExprDecimal expr, params SqlExprDecimal[] values)
    {
        return new SqlInValues<SqlExprDecimal>(expr, values.ToImmutableArray());
    }

    /// <summary>
    /// Creates a SQL IN expression with a decimal subquery.
    /// Equivalent to: expr IN (SELECT ... FROM ...)
    /// </summary>
    /// <param name="expr">The decimal expression to test</param>
    /// <param name="subQuery">The subquery that returns decimal values</param>
    /// <returns>A SQL boolean expression representing the IN operation with subquery</returns>
    public static SqlExprBool In(this SqlExprDecimal expr, ISqlQuery<ValueTuple<SqlExprDecimal>> subQuery)
    {
        return new SqlInSubQuery<SqlExprDecimal>(expr, subQuery);
    }

    /// <summary>
    /// Creates a SQL IN expression for DateTime values.
    /// Equivalent to: expr IN (value1, value2, value3, ...)
    /// </summary>
    /// <param name="expr">The DateTime expression to test</param>
    /// <param name="values">The values to test against</param>
    /// <returns>A SQL boolean expression representing the IN operation</returns>
    public static SqlExprBool In(this SqlExprDateTime expr, params SqlExprDateTime[] values)
    {
        return new SqlInValues<SqlExprDateTime>(expr, values.ToImmutableArray());
    }

    /// <summary>
    /// Creates a SQL IN expression with a DateTime subquery.
    /// Equivalent to: expr IN (SELECT ... FROM ...)
    /// </summary>
    /// <param name="expr">The DateTime expression to test</param>
    /// <param name="subQuery">The subquery that returns DateTime values</param>
    /// <returns>A SQL boolean expression representing the IN operation with subquery</returns>
    public static SqlExprBool In(this SqlExprDateTime expr, ISqlQuery<ValueTuple<SqlExprDateTime>> subQuery)
    {
        return new SqlInSubQuery<SqlExprDateTime>(expr, subQuery);
    }

    /// <summary>
    /// Creates a SQL IN expression for GUID values.
    /// Equivalent to: expr IN (value1, value2, value3, ...)
    /// </summary>
    /// <param name="expr">The GUID expression to test</param>
    /// <param name="values">The values to test against</param>
    /// <returns>A SQL boolean expression representing the IN operation</returns>
    public static SqlExprBool In(this SqlExprGuid expr, params SqlExprGuid[] values)
    {
        return new SqlInValues<SqlExprGuid>(expr, values.ToImmutableArray());
    }

    /// <summary>
    /// Creates a SQL IN expression with a GUID subquery.
    /// Equivalent to: expr IN (SELECT ... FROM ...)
    /// </summary>
    /// <param name="expr">The GUID expression to test</param>
    /// <param name="subQuery">The subquery that returns GUID values</param>
    /// <returns>A SQL boolean expression representing the IN operation with subquery</returns>
    public static SqlExprBool In(this SqlExprGuid expr, ISqlQuery<ValueTuple<SqlExprGuid>> subQuery)
    {
        return new SqlInSubQuery<SqlExprGuid>(expr, subQuery);
    }
}