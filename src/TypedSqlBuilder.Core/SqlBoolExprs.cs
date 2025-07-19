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
	public void Deconstruct(out bool valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL NOT operation (logical negation).
/// </summary>
public class SqlBoolNot(SqlExprBool value) : SqlExprBool
{
	public void Deconstruct(out SqlExprBool valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL AND operation (logical conjunction).
/// </summary>
public class SqlBoolAnd(SqlExprBool left, SqlExprBool right) : SqlExprBool
{
	public void Deconstruct(out SqlExprBool leftOut, out SqlExprBool rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a SQL OR operation (logical disjunction).
/// </summary>
public class SqlBoolOr(SqlExprBool left, SqlExprBool right) : SqlExprBool
{
	public void Deconstruct(out SqlExprBool leftOut, out SqlExprBool rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a SQL equality comparison between two boolean expressions (=).
/// </summary>
public class SqlBoolEquals(SqlExprBool left, SqlExprBool right) : SqlExprBool
{
	public void Deconstruct(out SqlExprBool leftOut, out SqlExprBool rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a SQL inequality comparison between two boolean expressions (!=).
/// </summary>
public class SqlBoolNotEquals(SqlExprBool left, SqlExprBool right) : SqlExprBool
{
	public void Deconstruct(out SqlExprBool leftOut, out SqlExprBool rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a projection of a boolean column or expression in SQL queries.
/// This class serves as a base for referencing boolean values from tables or subqueries,
/// typically used in SELECT clauses or other projection contexts.
/// </summary>
/// <param name="source">The table alias or source identifier</param>
/// <param name="name">The column or expression name</param>
public class SqlBoolProjection(string source, string name) : SqlExprBool
{
	public void Deconstruct(out string sourceOut, out string nameOut)
	{
		sourceOut = source;
		nameOut = name;
	}
}

/// <summary>
/// Represents a reference to a boolean column in a SQL table.
/// Inherits from SqlBoolProjection to support column references in SQL queries.
/// </summary>
/// <param name="source">The table name or alias that contains the column</param>
/// <param name="name">The name of the column</param>
public class SqlBoolColumn(string source, string name) : SqlBoolProjection(source, name);
