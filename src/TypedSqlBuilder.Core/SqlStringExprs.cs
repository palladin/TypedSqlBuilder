namespace TypedSqlBuilder.Core;

/// <summary>
/// Contains concrete implementations of SQL string expressions.
/// These classes represent various string operations and comparisons that can be performed in SQL queries,
/// including literal values, comparison operations, and string concatenation.
/// String comparison classes inherit from SqlExprBool since they return boolean results,
/// while string manipulation classes inherit from SqlExprString.
/// </summary>

/// <summary>
/// Represents a literal string value in SQL.
/// </summary>
public class SqlStringValue(string value) : SqlExprString
{
	public string Value { get; } = value;
}

/// <summary>
/// Represents a SQL equality comparison between two string expressions (=).
/// </summary>
public class SqlStringEquals(ISqlExpr<ISqlString> left, ISqlExpr<ISqlString> right) : SqlExprBool, ISqlBinExpr<ISqlString>
{
	public ISqlExpr<ISqlString> Left { get; } = left;
	public ISqlExpr<ISqlString> Right { get; } = right;
}

/// <summary>
/// Represents a SQL inequality comparison between two string expressions (!=).
/// </summary>
public class SqlStringNotEquals(ISqlExpr<ISqlString> left, ISqlExpr<ISqlString> right) : SqlExprBool, ISqlBinExpr<ISqlString>
{
	public ISqlExpr<ISqlString> Left { get; } = left;
	public ISqlExpr<ISqlString> Right { get; } = right;
}

/// <summary>
/// Represents a SQL greater-than comparison between two string expressions (>).
/// Uses lexicographic (alphabetical) ordering.
/// </summary>
public class SqlStringGreaterThan(ISqlExpr<ISqlString> left, ISqlExpr<ISqlString> right) : SqlExprBool, ISqlBinExpr<ISqlString>
{
	public ISqlExpr<ISqlString> Left { get; } = left;
	public ISqlExpr<ISqlString> Right { get; } = right;
}

/// <summary>
/// Represents a SQL less-than comparison between two string expressions (<).
/// Uses lexicographic (alphabetical) ordering.
/// </summary>
public class SqlStringLessThan(ISqlExpr<ISqlString> left, ISqlExpr<ISqlString> right) : SqlExprBool, ISqlBinExpr<ISqlString>
{
	public ISqlExpr<ISqlString> Left { get; } = left;
	public ISqlExpr<ISqlString> Right { get; } = right;
}

/// <summary>
/// Represents a SQL greater-than-or-equal comparison between two string expressions (>=).
/// Uses lexicographic (alphabetical) ordering.
/// </summary>
public class SqlStringGreaterThanOrEqualTo(ISqlExpr<ISqlString> left, ISqlExpr<ISqlString> right) : SqlExprBool, ISqlBinExpr<ISqlString>
{
	public ISqlExpr<ISqlString> Left { get; } = left;
	public ISqlExpr<ISqlString> Right { get; } = right;
}

/// <summary>
/// Represents a SQL less-than-or-equal comparison between two string expressions (<=).
/// Uses lexicographic (alphabetical) ordering.
/// </summary>
public class SqlStringLessThanOrEqualTo(ISqlExpr<ISqlString> left, ISqlExpr<ISqlString> right) : SqlExprBool, ISqlBinExpr<ISqlString>
{
	public ISqlExpr<ISqlString> Left { get; } = left;
	public ISqlExpr<ISqlString> Right { get; } = right;
}

/// <summary>
/// Represents a SQL string concatenation operation between two string expressions.
/// Equivalent to the SQL CONCAT function or || operator in some databases.
/// </summary>
public class SqlStringConcat(ISqlExpr<ISqlString> left, ISqlExpr<ISqlString> right) : SqlExprString, ISqlBinExpr<ISqlString>
{
	public ISqlExpr<ISqlString> Left { get; } = left;
	public ISqlExpr<ISqlString> Right { get; } = right;
}
