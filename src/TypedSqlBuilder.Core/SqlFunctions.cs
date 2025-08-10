using System.Collections.Immutable;
using System.Linq;

namespace TypedSqlBuilder.Core;

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
    public static SqlParameterInt AsIntParam(this string name)
    {
        return new SqlParameterInt(name);
    }
    
    /// <summary>
    /// Creates a SQL string parameter from a string name.
    /// Extension method that allows string literals to be converted to SQL parameters.
    /// </summary>
    /// <param name="name">The parameter name (e.g., "@userName", ":email")</param>
    /// <returns>A SQL string parameter expression</returns>
    public static SqlParameterString AsStringParam(this string name)
    {
        return new SqlParameterString(name);
    }

    /// <summary>
    /// Creates a SQL boolean parameter from a string name.
    /// Extension method that allows string literals to be converted to SQL parameters.
    /// </summary>
    /// <param name="name">The parameter name (e.g., "@isActive", ":enabled")</param>
    /// <returns>A SQL boolean parameter expression</returns>
    public static SqlParameterBool AsBoolParam(this string name)
    {
        return new SqlParameterBool(name);
    }

    /// <summary>
    /// Creates a SQL COUNT(*) aggregate function expression.
    /// </summary>
    /// <returns>A SQL integer expression representing COUNT(*)</returns>
    public static SqlExprInt Count()
    {
        return new SqlIntCount();
    }

    /// <summary>
    /// Creates a SQL SUM() aggregate function expression for the given integer expression.
    /// </summary>
    /// <param name="value">The integer expression to sum</param>
    /// <returns>A SQL integer expression representing SUM(value)</returns>
    public static SqlExprInt Sum(this SqlExprInt value)
    {
        return new SqlIntSum(value);
    }

    /// <summary>
    /// Creates a SQL AVG() aggregate function expression for the given integer expression.
    /// </summary>
    /// <param name="value">The integer expression to calculate the average of</param>
    /// <returns>A SQL integer expression representing AVG(value)</returns>
    public static SqlExprInt Avg(this SqlExprInt value)
    {
        return new SqlIntAvg(value);
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
    /// Creates a SQL CASE expression for conditional string values using string literals.
    /// Equivalent to: CASE WHEN condition THEN trueValue ELSE falseValue END
    /// </summary>
    /// <param name="condition">The boolean condition to evaluate</param>
    /// <param name="trueValue">The string literal returned when condition is true</param>
    /// <param name="falseValue">The string literal returned when condition is false</param>
    /// <returns>A SQL string expression representing the CASE statement</returns>
    public static SqlExprString Case(SqlExprBool condition, string trueValue, string falseValue)
    {
        return new SqlStringCase(condition, trueValue, falseValue);
    }

    /// <summary>
    /// Creates a SQL CASE expression for conditional integer values using integer literals.
    /// Equivalent to: CASE WHEN condition THEN trueValue ELSE falseValue END
    /// </summary>
    /// <param name="condition">The boolean condition to evaluate</param>
    /// <param name="trueValue">The integer literal returned when condition is true</param>
    /// <param name="falseValue">The integer literal returned when condition is false</param>
    /// <returns>A SQL integer expression representing the CASE statement</returns>
    public static SqlExprInt Case(SqlExprBool condition, int trueValue, int falseValue)
    {
        return new SqlIntCase(condition, trueValue, falseValue);
    }

    /// <summary>
    /// Creates a SQL CASE expression for conditional boolean values using boolean literals.
    /// Equivalent to: CASE WHEN condition THEN trueValue ELSE falseValue END
    /// </summary>
    /// <param name="condition">The boolean condition to evaluate</param>
    /// <param name="trueValue">The boolean literal returned when condition is true</param>
    /// <param name="falseValue">The boolean literal returned when condition is false</param>
    /// <returns>A SQL boolean expression representing the CASE statement</returns>
    public static SqlExprBool Case(SqlExprBool condition, bool trueValue, bool falseValue)
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
    public static SqlExprBool In(this SqlExprInt expr, params ImmutableArray<SqlExprInt> values)
    {
        return new SqlInValues<SqlExprInt>(expr, values);
    }

    /// <summary>
    /// Creates a SQL IN expression for string values.
    /// Equivalent to: expr IN (value1, value2, value3, ...)
    /// </summary>
    /// <param name="expr">The string expression to test</param>
    /// <param name="values">The values to test against</param>
    /// <returns>A SQL boolean expression representing the IN operation</returns>
    public static SqlExprBool In(this SqlExprString expr, params ImmutableArray<SqlExprString> values)
    {
        return new SqlInValues<SqlExprString>(expr, values);
    }

    /// <summary>
    /// Creates a SQL IN expression for boolean values.
    /// Equivalent to: expr IN (value1, value2, value3, ...)
    /// </summary>
    /// <param name="expr">The boolean expression to test</param>
    /// <param name="values">The values to test against</param>
    /// <returns>A SQL boolean expression representing the IN operation</returns>
    public static SqlExprBool In(this SqlExprBool expr, params ImmutableArray<SqlExprBool> values)
    {
        return new SqlInValues<SqlExprBool>(expr, values);
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
}