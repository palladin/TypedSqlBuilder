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
	private readonly bool _value = value;
	
	public void Deconstruct(out bool value) => value = _value;
}

/// <summary>
/// Represents a SQL NOT operation (logical negation).
/// </summary>
public class SqlBoolNot(SqlExprBool value) : SqlExprBool
{
	private readonly SqlExprBool _value = value;
	
	public void Deconstruct(out SqlExprBool value) => value = _value;
}

/// <summary>
/// Represents a SQL AND operation (logical conjunction).
/// </summary>
public class SqlBoolAnd(SqlExprBool left, SqlExprBool right) : SqlExprBool
{
	private readonly SqlExprBool _left = left;
	private readonly SqlExprBool _right = right;
	
	public void Deconstruct(out SqlExprBool left, out SqlExprBool right) => (left, right) = (_left, _right);
}

/// <summary>
/// Represents a SQL OR operation (logical disjunction).
/// </summary>
public class SqlBoolOr(SqlExprBool left, SqlExprBool right) : SqlExprBool
{
	private readonly SqlExprBool _left = left;
	private readonly SqlExprBool _right = right;
	
	public void Deconstruct(out SqlExprBool left, out SqlExprBool right) => (left, right) = (_left, _right);
}

/// <summary>
/// Represents a SQL equality comparison between two boolean expressions (=).
/// </summary>
public class SqlBoolEquals(SqlExprBool left, SqlExprBool right) : SqlExprBool
{
	private readonly SqlExprBool _left = left;
	private readonly SqlExprBool _right = right;
	
	public void Deconstruct(out SqlExprBool left, out SqlExprBool right) => (left, right) = (_left, _right);
}

/// <summary>
/// Represents a SQL inequality comparison between two boolean expressions (!=).
/// </summary>
public class SqlBoolNotEquals(SqlExprBool left, SqlExprBool right) : SqlExprBool
{
	private readonly SqlExprBool _left = left;
	private readonly SqlExprBool _right = right;
	
	public void Deconstruct(out SqlExprBool left, out SqlExprBool right) => (left, right) = (_left, _right);
}
