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