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
}