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
	private readonly string _value = value;
	
	public void Deconstruct(out string value) => value = _value;
}

/// <summary>
/// Represents a SQL equality comparison between two string expressions (=).
/// </summary>
public class SqlStringEquals(SqlExprString left, SqlExprString right) : SqlExprBool
{
	private readonly SqlExprString _left = left;
	private readonly SqlExprString _right = right;
	
	public void Deconstruct(out SqlExprString left, out SqlExprString right) => (left, right) = (_left, _right);
}

/// <summary>
/// Represents a SQL inequality comparison between two string expressions (!=).
/// </summary>
public class SqlStringNotEquals(SqlExprString left, SqlExprString right) : SqlExprBool
{
	private readonly SqlExprString _left = left;
	private readonly SqlExprString _right = right;
	
	public void Deconstruct(out SqlExprString left, out SqlExprString right) => (left, right) = (_left, _right);
}

/// <summary>
/// Represents a SQL greater-than comparison between two string expressions (>).
/// Uses lexicographic (alphabetical) ordering.
/// </summary>
public class SqlStringGreaterThan(SqlExprString left, SqlExprString right) : SqlExprBool
{
	private readonly SqlExprString _left = left;
	private readonly SqlExprString _right = right;
	
	public void Deconstruct(out SqlExprString left, out SqlExprString right) => (left, right) = (_left, _right);
}

/// <summary>
/// Represents a SQL less-than comparison between two string expressions (<).
/// Uses lexicographic (alphabetical) ordering.
/// </summary>
public class SqlStringLessThan(SqlExprString left, SqlExprString right) : SqlExprBool
{
	private readonly SqlExprString _left = left;
	private readonly SqlExprString _right = right;
	
	public void Deconstruct(out SqlExprString left, out SqlExprString right) => (left, right) = (_left, _right);
}

/// <summary>
/// Represents a SQL greater-than-or-equal comparison between two string expressions (>=).
/// Uses lexicographic (alphabetical) ordering.
/// </summary>
public class SqlStringGreaterThanOrEqualTo(SqlExprString left, SqlExprString right) : SqlExprBool
{
	private readonly SqlExprString _left = left;
	private readonly SqlExprString _right = right;
	
	public void Deconstruct(out SqlExprString left, out SqlExprString right) => (left, right) = (_left, _right);
}

/// <summary>
/// Represents a SQL less-than-or-equal comparison between two string expressions (<=).
/// Uses lexicographic (alphabetical) ordering.
/// </summary>
public class SqlStringLessThanOrEqualTo(SqlExprString left, SqlExprString right) : SqlExprBool
{
	private readonly SqlExprString _left = left;
	private readonly SqlExprString _right = right;
	
	public void Deconstruct(out SqlExprString left, out SqlExprString right) => (left, right) = (_left, _right);
}

/// <summary>
/// Represents a SQL string concatenation operation between two string expressions.
/// Equivalent to the SQL CONCAT function or || operator in some databases.
/// </summary>
public class SqlStringConcat(SqlExprString left, SqlExprString right) : SqlExprString
{
	private readonly SqlExprString _left = left;
	private readonly SqlExprString _right = right;
	
	public void Deconstruct(out SqlExprString left, out SqlExprString right) => (left, right) = (_left, _right);
}
