namespace TypedSqlBuilder.Core;

/// <summary>
/// Contains concrete implementations of SQL DateTime expressions.
/// These classes represent various DateTime operations and comparisons that can be performed in SQL queries,
/// including literal values and column references.
/// All classes inherit from SqlExprDateTime and implement appropriate interfaces for type safety.
/// </summary>

/// <summary>
/// Represents a literal DateTime value in SQL.
/// </summary>
internal class SqlDateTimeValue(DateTime value) : SqlExprDateTime
{
	public void Deconstruct(out DateTime valueOut) => valueOut = value;
}

/// <summary>
/// Represents a reference to a DateTime column in a SQL table.
/// This class is used for column references in SQL queries.
/// </summary>
public class SqlDateTimeColumn : SqlExprDateTime, ISqlColumn
{
    internal SqlDateTimeColumn(string tableName, string columnName)
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
/// Represents a named DateTime parameter in SQL expressions.
/// This class is used for parameterized queries where DateTime values need to be bound at execution time.
/// </summary>
/// <param name="name">The name of the parameter (e.g., "@createdDate", ":timestamp")</param>
internal class SqlParameterDateTime(string name) : SqlExprDateTime
{
	public void Deconstruct(out string nameOut) => nameOut = name;
}

/// <summary>
/// Represents a SQL CASE expression for conditional DateTime values.
/// This class applies the SQL CASE WHEN condition THEN trueValue ELSE falseValue END construct.
/// </summary>
/// <param name="condition">The boolean condition to evaluate</param>
/// <param name="trueValue">The DateTime expression returned when condition is true</param>
/// <param name="falseValue">The DateTime expression returned when condition is false</param>
internal class SqlDateTimeCase(SqlExprBool condition, SqlExprDateTime trueValue, SqlExprDateTime falseValue) : SqlExprDateTime
{
	public void Deconstruct(out SqlExprBool conditionOut, out SqlExprDateTime trueValueOut, out SqlExprDateTime falseValueOut) => 
		(conditionOut, trueValueOut, falseValueOut) = (condition, trueValue, falseValue);
}

/// <summary>
/// Represents a SQL NULL value for DateTime expressions.
/// This class is used when setting DateTime columns to NULL in SQL statements.
/// </summary>
internal class SqlDateTimeNull : SqlExprDateTime, ISqlNullValue
{
	public static SqlDateTimeNull Value => new();
}

/// <summary>
/// Represents a SQL GETDATE() or NOW() function for getting current date/time.
/// Equivalent to: GETDATE() (SQL Server) or NOW() (PostgreSQL/MySQL)
/// </summary>
internal class SqlDateTimeNow : SqlExprDateTime
{
	public static SqlDateTimeNow Value => new();
}

/// <summary>
/// Represents a SQL YEAR function for extracting year from date.
/// Equivalent to: YEAR(date)
/// </summary>
/// <param name="value">The DateTime expression to extract year from</param>
internal class SqlDateTimeYear(SqlExprDateTime value) : SqlExprInt
{
	public void Deconstruct(out SqlExprDateTime valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL MONTH function for extracting month from date.
/// Equivalent to: MONTH(date)
/// </summary>
/// <param name="value">The DateTime expression to extract month from</param>
internal class SqlDateTimeMonth(SqlExprDateTime value) : SqlExprInt
{
	public void Deconstruct(out SqlExprDateTime valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL DAY function for extracting day from date.
/// Equivalent to: DAY(date)
/// </summary>
/// <param name="value">The DateTime expression to extract day from</param>
internal class SqlDateTimeDay(SqlExprDateTime value) : SqlExprInt
{
	public void Deconstruct(out SqlExprDateTime valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL DATEADD function for adding time intervals to dates.
/// Equivalent to: DATEADD(datepart, number, date)
/// </summary>
/// <param name="datepart">The part of date to add to (e.g., "day", "month", "year")</param>
/// <param name="number">The number of units to add</param>
/// <param name="date">The DateTime expression to add to</param>
internal class SqlDateTimeAdd(string datepart, SqlExprInt number, SqlExprDateTime date) : SqlExprDateTime
{
	public void Deconstruct(out string datepartOut, out SqlExprInt numberOut, out SqlExprDateTime dateOut) => 
		(datepartOut, numberOut, dateOut) = (datepart, number, date);
}

/// <summary>
/// Represents a SQL DATEDIFF function for calculating difference between dates.
/// Equivalent to: DATEDIFF(datepart, startdate, enddate)
/// </summary>
/// <param name="datepart">The part of date to calculate difference in (e.g., "day", "month", "year")</param>
/// <param name="startdate">The start DateTime expression</param>
/// <param name="enddate">The end DateTime expression</param>
internal class SqlDateTimeDiff(string datepart, SqlExprDateTime startdate, SqlExprDateTime enddate) : SqlExprInt
{
	public void Deconstruct(out string datepartOut, out SqlExprDateTime startdateOut, out SqlExprDateTime enddateOut) => 
		(datepartOut, startdateOut, enddateOut) = (datepart, startdate, enddate);
}
