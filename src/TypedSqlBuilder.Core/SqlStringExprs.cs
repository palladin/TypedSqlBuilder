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
	public void Deconstruct(out string valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL equality comparison between two string expressions (=).
/// </summary>
public class SqlStringEquals(SqlExprString left, SqlExprString right) : SqlExprBool
{
	public void Deconstruct(out SqlExprString leftOut, out SqlExprString rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a SQL inequality comparison between two string expressions (!=).
/// </summary>
public class SqlStringNotEquals(SqlExprString left, SqlExprString right) : SqlExprBool
{
	public void Deconstruct(out SqlExprString leftOut, out SqlExprString rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a SQL greater-than comparison between two string expressions (>).
/// Uses lexicographic (alphabetical) ordering.
/// </summary>
public class SqlStringGreaterThan(SqlExprString left, SqlExprString right) : SqlExprBool
{
	public void Deconstruct(out SqlExprString leftOut, out SqlExprString rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a SQL less-than comparison between two string expressions (<).
/// Uses lexicographic (alphabetical) ordering.
/// </summary>
public class SqlStringLessThan(SqlExprString left, SqlExprString right) : SqlExprBool
{
	public void Deconstruct(out SqlExprString leftOut, out SqlExprString rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a SQL greater-than-or-equal comparison between two string expressions (>=).
/// Uses lexicographic (alphabetical) ordering.
/// </summary>
public class SqlStringGreaterThanOrEqualTo(SqlExprString left, SqlExprString right) : SqlExprBool
{
	public void Deconstruct(out SqlExprString leftOut, out SqlExprString rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a SQL less-than-or-equal comparison between two string expressions (<=).
/// Uses lexicographic (alphabetical) ordering.
/// </summary>
public class SqlStringLessThanOrEqualTo(SqlExprString left, SqlExprString right) : SqlExprBool
{
	public void Deconstruct(out SqlExprString leftOut, out SqlExprString rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a SQL string concatenation operation between two string expressions.
/// Equivalent to the SQL CONCAT function or || operator in some databases.
/// </summary>
public class SqlStringConcat(SqlExprString left, SqlExprString right) : SqlExprString
{
	public void Deconstruct(out SqlExprString leftOut, out SqlExprString rightOut) => (leftOut, rightOut) = (left, right);
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
/// <param name="name">The name of the column</param>
public class SqlStringColumn : SqlStringProjection, ISqlColumn<SqlStringColumn>
{
	public SqlStringColumn(string name) : base("", name)
	{
		ColumnName = name;
	}
	
	/// <summary>
	/// Internal constructor for creating a column with both source and name.
	/// </summary>
	private SqlStringColumn(string source, string name, bool isInternal) : base(source, name)
	{
		ColumnName = name;
	}
	
	public string ColumnName { get; }
	
	public SqlStringColumn WithName(string source, string columnName) => new SqlStringColumn(source, columnName, true);
}

/// <summary>
/// Represents a named string parameter in SQL expressions.
/// This class is used for parameterized queries where string values need to be bound at execution time.
/// </summary>
/// <param name="name">The name of the parameter (e.g., "@userName", ":email")</param>
public class SqlParameterString(string name) : SqlExprString
{
	public void Deconstruct(out string nameOut) => nameOut = name;
}

/// <summary>
/// Represents a SQL LIKE operation for pattern matching in string expressions.
/// This class applies the SQL LIKE operator to compare a string expression against a pattern.
/// </summary>
/// <param name="value">The string expression to match against the pattern</param>
/// <param name="pattern">The pattern to match (can include wildcards like % and _)</param>
public class SqlStringLike(SqlExprString value, string pattern) : SqlExprBool
{
	public void Deconstruct(out SqlExprString valueOut, out string patternOut) => (valueOut, patternOut) = (value, pattern);
}

/// <summary>
/// Represents a SQL CASE expression for conditional string values.
/// This class applies the SQL CASE WHEN condition THEN trueValue ELSE falseValue END construct.
/// </summary>
/// <param name="condition">The boolean condition to evaluate</param>
/// <param name="trueValue">The string expression returned when condition is true</param>
/// <param name="falseValue">The string expression returned when condition is false</param>
public class SqlStringCase(SqlExprBool condition, SqlExprString trueValue, SqlExprString falseValue) : SqlExprString
{
	public void Deconstruct(out SqlExprBool conditionOut, out SqlExprString trueValueOut, out SqlExprString falseValueOut) => 
		(conditionOut, trueValueOut, falseValueOut) = (condition, trueValue, falseValue);
}
