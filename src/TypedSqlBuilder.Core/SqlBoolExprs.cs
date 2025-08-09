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
/// This class is used for column references in SQL queries.
/// </summary>
public class SqlBoolColumn : SqlExprBool, ISqlColumn<SqlBoolColumn>
{	
	public SqlBoolColumn(string columnName)
	{
		ColumnName = columnName;
	}		
	private SqlBoolColumn(string tableName, string columnName)
	{
		TableName = tableName;
		ColumnName = columnName;		
	}
	public string TableName { get; } = string.Empty;
	public string ColumnName { get; }
	
	public static SqlBoolColumn Create(string source, string columnName) => new SqlBoolColumn(source, columnName);
	
	public void Deconstruct(out string sourceOut, out string nameOut)
	{
		sourceOut = TableName;
		nameOut = ColumnName;
	}
}

/// <summary>
/// Represents a named boolean parameter in SQL expressions.
/// This class is used for parameterized queries where boolean values need to be bound at execution time.
/// </summary>
/// <param name="name">The name of the parameter (e.g., "@isActive", ":enabled")</param>
public class SqlParameterBool(string name) : SqlExprBool
{
	public void Deconstruct(out string nameOut) => nameOut = name;
}

/// <summary>
/// Represents a SQL CASE expression for conditional boolean values.
/// This class applies the SQL CASE WHEN condition THEN trueValue ELSE falseValue END construct.
/// </summary>
/// <param name="condition">The boolean condition to evaluate</param>
/// <param name="trueValue">The boolean expression returned when condition is true</param>
/// <param name="falseValue">The boolean expression returned when condition is false</param>
public class SqlBoolCase(SqlExprBool condition, SqlExprBool trueValue, SqlExprBool falseValue) : SqlExprBool
{
	public void Deconstruct(out SqlExprBool conditionOut, out SqlExprBool trueValueOut, out SqlExprBool falseValueOut) => 
		(conditionOut, trueValueOut, falseValueOut) = (condition, trueValue, falseValue);
}

/// <summary>
/// Represents a SQL NULL value for boolean expressions.
/// This class is used when setting boolean columns to NULL in SQL statements.
/// </summary>
public class SqlBoolNull : SqlExprBool
{
	public static SqlBoolNull Value => new();
}
