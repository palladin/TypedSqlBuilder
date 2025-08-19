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
internal class SqlStringValue(string value) : SqlExprString
{
	public void Deconstruct(out string valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL string concatenation operation between two string expressions.
/// Equivalent to the SQL CONCAT function or || operator in some databases.
/// </summary>
internal class SqlStringConcat(SqlExprString left, SqlExprString right) : SqlExprString
{
	public void Deconstruct(out SqlExprString leftOut, out SqlExprString rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a reference to a string column in a SQL table.
/// This class is used for column references in SQL queries.
/// </summary>
public class SqlStringColumn : SqlExprString, ISqlColumn
{
    internal SqlStringColumn(string tableName, string columnName)
    {
        TableName = tableName;
        ColumnName = columnName;        
    }
    
    /// <summary>
    /// Gets the name of the table this column belongs to.
    /// </summary>
    public string TableName { get; }
    
    /// <summary>
    /// Gets the name of the column within the table.
    /// </summary>
    public string ColumnName { get; }    

    /// <summary>
    /// Deconstructs the column into its table name and column name components.
    /// </summary>
    /// <param name="tableNameOut">The name of the table</param>
    /// <param name="columnNameOut">The name of the column</param>
    public void Deconstruct(out string tableNameOut, out string columnNameOut)
    {
        tableNameOut = TableName;
        columnNameOut = ColumnName;
    }
}

/// <summary>
/// Represents a named string parameter in SQL expressions.
/// This class is used for parameterized queries where string values need to be bound at execution time.
/// </summary>
/// <param name="name">The name of the parameter (e.g., "@userName", ":email")</param>
internal class SqlParameterString(string name) : SqlExprString
{
	public void Deconstruct(out string nameOut) => nameOut = name;
}

/// <summary>
/// Represents a SQL LIKE operation for pattern matching in string expressions.
/// This class applies the SQL LIKE operator to compare a string expression against a pattern.
/// </summary>
/// <param name="value">The string expression to match against the pattern</param>
/// <param name="pattern">The pattern to match (can include wildcards like % and _)</param>
internal class SqlStringLike(SqlExprString value, string pattern) : SqlExprBool
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
internal class SqlStringCase(SqlExprBool condition, SqlExprString trueValue, SqlExprString falseValue) : SqlExprString
{
	public void Deconstruct(out SqlExprBool conditionOut, out SqlExprString trueValueOut, out SqlExprString falseValueOut) => 
		(conditionOut, trueValueOut, falseValueOut) = (condition, trueValue, falseValue);
}

/// <summary>
/// Represents a SQL NULL value for string expressions.
/// This class is used when setting string columns to NULL in SQL statements.
/// </summary>
internal class SqlStringNull : SqlExprString, ISqlNullValue
{
	public static SqlStringNull Value => new();
}

/// <summary>
/// Represents a SQL SUBSTRING function for extracting part of a string.
/// Equivalent to: SUBSTRING(value, start, length)
/// </summary>
/// <param name="value">The string expression to extract from</param>
/// <param name="start">The starting position (1-based)</param>
/// <param name="length">The number of characters to extract</param>
internal class SqlStringSubstring(SqlExprString value, SqlExprInt start, SqlExprInt length) : SqlExprString
{
	public void Deconstruct(out SqlExprString valueOut, out SqlExprInt startOut, out SqlExprInt lengthOut) => 
		(valueOut, startOut, lengthOut) = (value, start, length);
}

/// <summary>
/// Represents a SQL UPPER function for converting string to uppercase.
/// Equivalent to: UPPER(value)
/// </summary>
/// <param name="value">The string expression to convert to uppercase</param>
internal class SqlStringUpper(SqlExprString value) : SqlExprString
{
	public void Deconstruct(out SqlExprString valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL LOWER function for converting string to lowercase.
/// Equivalent to: LOWER(value)
/// </summary>
/// <param name="value">The string expression to convert to lowercase</param>
internal class SqlStringLower(SqlExprString value) : SqlExprString
{
	public void Deconstruct(out SqlExprString valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL TRIM function for removing whitespace from both ends.
/// Equivalent to: TRIM(value)
/// </summary>
/// <param name="value">The string expression to trim</param>
internal class SqlStringTrim(SqlExprString value) : SqlExprString
{
	public void Deconstruct(out SqlExprString valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL LEN/LENGTH function for getting string length.
/// Equivalent to: LEN(value) or LENGTH(value)
/// </summary>
/// <param name="value">The string expression to get the length of</param>
internal class SqlStringLength(SqlExprString value) : SqlExprInt
{
	public void Deconstruct(out SqlExprString valueOut) => valueOut = value;
}