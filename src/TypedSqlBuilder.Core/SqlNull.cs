namespace TypedSqlBuilder.Core;

/// <summary>
/// A universal SQL NULL value that can be implicitly converted to any typed SQL expression.
/// This provides a clean API: SqlNull.Value instead of SqlIntNull.Value, SqlStringNull.Value, etc.
/// </summary>
public class SqlNull
{
    public static SqlNull Value => new();
    
    // Implicit conversions to specific typed null expressions
    public static implicit operator SqlExprInt(SqlNull _) => new SqlIntNull();
    public static implicit operator SqlExprString(SqlNull _) => new SqlStringNull();
    public static implicit operator SqlExprBool(SqlNull _) => new SqlBoolNull();
}

/// <summary>
/// Extension methods for SQL null operations (IS NULL, IS NOT NULL).
/// These methods provide a fluent, discoverable API for testing null values
/// in SQL expressions while maintaining type safety.
/// </summary>
public static class SqlNullExtensions
{
	/// <summary>
	/// Creates a SQL IS NULL expression for testing if a string expression is null.
	/// </summary>
	/// <param name="expr">The string expression to test</param>
	/// <returns>A boolean expression representing 'expr IS NULL'</returns>
	public static SqlExprBool IsNull(this SqlExprString expr) => expr == SqlNull.Value;

	/// <summary>
	/// Creates a SQL IS NOT NULL expression for testing if a string expression is not null.
	/// </summary>
	/// <param name="expr">The string expression to test</param>
	/// <returns>A boolean expression representing 'expr IS NOT NULL'</returns>
	public static SqlExprBool IsNotNull(this SqlExprString expr) => expr != SqlNull.Value;

	/// <summary>
	/// Creates a SQL IS NULL expression for testing if an integer expression is null.
	/// </summary>
	/// <param name="expr">The integer expression to test</param>
	/// <returns>A boolean expression representing 'expr IS NULL'</returns>
	public static SqlExprBool IsNull(this SqlExprInt expr) => expr == SqlNull.Value;

	/// <summary>
	/// Creates a SQL IS NOT NULL expression for testing if an integer expression is not null.
	/// </summary>
	/// <param name="expr">The integer expression to test</param>
	/// <returns>A boolean expression representing 'expr IS NOT NULL'</returns>
	public static SqlExprBool IsNotNull(this SqlExprInt expr) => expr != SqlNull.Value;

	/// <summary>
	/// Creates a SQL IS NULL expression for testing if a boolean expression is null.
	/// </summary>
	/// <param name="expr">The boolean expression to test</param>
	/// <returns>A boolean expression representing 'expr IS NULL'</returns>
	public static SqlExprBool IsNull(this SqlExprBool expr) => expr == SqlNull.Value;

	/// <summary>
	/// Creates a SQL IS NOT NULL expression for testing if a boolean expression is not null.
	/// </summary>
	/// <param name="expr">The boolean expression to test</param>
	/// <returns>A boolean expression representing 'expr IS NOT NULL'</returns>
	public static SqlExprBool IsNotNull(this SqlExprBool expr) => expr != SqlNull.Value;
}
