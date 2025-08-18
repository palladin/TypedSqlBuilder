namespace TypedSqlBuilder.Core;

/// <summary>
/// Contains concrete implementations of SQL GUID expressions.
/// These classes represent various GUID operations and comparisons that can be performed in SQL queries,
/// including literal values and column references.
/// All classes inherit from SqlExprGuid and implement appropriate interfaces for type safety.
/// </summary>

/// <summary>
/// Represents a literal GUID value in SQL.
/// </summary>
internal class SqlGuidValue(Guid value) : SqlExprGuid
{
	public void Deconstruct(out Guid valueOut) => valueOut = value;
}

/// <summary>
/// Represents a reference to a GUID column in a SQL table.
/// This class is used for column references in SQL queries.
/// </summary>
public class SqlGuidColumn : SqlExprGuid, ISqlColumn
{
    internal SqlGuidColumn(string tableName, string columnName)
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
/// Represents a named GUID parameter in SQL expressions.
/// This class is used for parameterized queries where GUID values need to be bound at execution time.
/// </summary>
/// <param name="name">The name of the parameter (e.g., "@userId", ":id")</param>
internal class SqlParameterGuid(string name) : SqlExprGuid
{
	public void Deconstruct(out string nameOut) => nameOut = name;
}

/// <summary>
/// Represents a SQL CASE expression for conditional GUID values.
/// This class applies the SQL CASE WHEN condition THEN trueValue ELSE falseValue END construct.
/// </summary>
/// <param name="condition">The boolean condition to evaluate</param>
/// <param name="trueValue">The GUID expression returned when condition is true</param>
/// <param name="falseValue">The GUID expression returned when condition is false</param>
internal class SqlGuidCase(SqlExprBool condition, SqlExprGuid trueValue, SqlExprGuid falseValue) : SqlExprGuid
{
	public void Deconstruct(out SqlExprBool conditionOut, out SqlExprGuid trueValueOut, out SqlExprGuid falseValueOut) => 
		(conditionOut, trueValueOut, falseValueOut) = (condition, trueValue, falseValue);
}

/// <summary>
/// Represents a SQL NULL value for GUID expressions.
/// This class is used when setting GUID columns to NULL in SQL statements.
/// </summary>
internal class SqlGuidNull : SqlExprGuid, ISqlNullValue
{
	public static SqlGuidNull Value => new();
}
