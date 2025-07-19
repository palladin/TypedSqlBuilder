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

/// <summary>
/// Represents a projection of a string column or expression in SQL queries.
/// This class serves as a base for referencing string values from tables or subqueries,
/// typically used in SELECT clauses or other projection contexts.
/// </summary>
/// <param name="source">The table alias or source identifier</param>
/// <param name="name">The column or expression name</param>
public class SqlStringProjection(string source, string name) : SqlExprString
{
	public void Deconstruct(out string sourceOut, out string nameOut)
	{
		sourceOut = source;
		nameOut = name;
	}
}

/// <summary>
/// Represents a reference to a string column in a SQL table.
/// Inherits from SqlStringProjection to support column references in SQL queries.
/// </summary>
/// <param name="source">The table name or alias that contains the column</param>
/// <param name="name">The name of the column</param>
public class SqlStringColumn(string source, string name) : SqlStringProjection(source, name);
