namespace TypedSqlBuilder.Core;

/// <summary>
/// Contains concrete implementations of SQL boolean expressions.
/// These classes represent various boolean operations that can be performed in SQL queries,
/// including logical operations (AND, OR, NOT), equality comparisons, and literal values.
/// All classes inherit from SqlExprBool and implement appropriate interfaces for type safety.
/// </summary>

/// <summary>
/// Represents a literal boolean value in SQL (TRUE or FALSE).
/// </summary>
public class SqlBoolValue(bool value) : SqlExprBool
{
	public bool Value { get; } = value;
}

/// <summary>
/// Represents a SQL NOT operation (logical negation).
/// </summary>
public class SqlBoolNot(ISqlExpr<ISqlBool> value) : SqlExprBool, ISqlUnaryExpr<ISqlBool>
{
	public ISqlExpr<ISqlBool> Value { get; } = value;
}

/// <summary>
/// Represents a SQL AND operation (logical conjunction).
/// </summary>
public class SqlBoolAnd(ISqlExpr<ISqlBool> left, ISqlExpr<ISqlBool> right) : SqlExprBool, ISqlBinExpr<ISqlBool>
{
	public ISqlExpr<ISqlBool> Left { get; } = left;
	public ISqlExpr<ISqlBool> Right { get; } = right;
}

/// <summary>
/// Represents a SQL OR operation (logical disjunction).
/// </summary>
public class SqlBoolOr(ISqlExpr<ISqlBool> left, ISqlExpr<ISqlBool> right) : SqlExprBool, ISqlBinExpr<ISqlBool>
{
	public ISqlExpr<ISqlBool> Left { get; } = left;
	public ISqlExpr<ISqlBool> Right { get; } = right;
}

/// <summary>
/// Represents a SQL equality comparison between two boolean expressions (=).
/// </summary>
public class SqlBoolEquals(ISqlExpr<ISqlBool> left, ISqlExpr<ISqlBool> right) : SqlExprBool, ISqlBinExpr<ISqlBool>
{
	public ISqlExpr<ISqlBool> Left { get; } = left;
	public ISqlExpr<ISqlBool> Right { get; } = right;
}

/// <summary>
/// Represents a SQL inequality comparison between two boolean expressions (!=).
/// </summary>
public class SqlBoolNotEquals(ISqlExpr<ISqlBool> left, ISqlExpr<ISqlBool> right) : SqlExprBool, ISqlBinExpr<ISqlBool>
{
	public ISqlExpr<ISqlBool> Left { get; } = left;
	public ISqlExpr<ISqlBool> Right { get; } = right;
}
