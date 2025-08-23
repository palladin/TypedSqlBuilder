namespace TypedSqlBuilder.Core;

/// <summary>
/// Contains concrete implementations of SQL decimal expressions.
/// These classes represent various decimal operations and comparisons that can be performed in SQL queries,
/// including literal values, arithmetic operations, and column references.
/// All classes inherit from SqlExprDecimal and implement appropriate interfaces for type safety.
/// </summary>

/// <summary>
/// Represents a literal decimal value in SQL.
/// </summary>
internal class SqlDecimalValue(decimal value) : SqlExprDecimal
{
    public void Deconstruct(out decimal valueOut) => valueOut = value;
}

/// <summary>
/// Represents an implicit conversion to a decimal expression.
/// This class wraps other SQL expressions (typically int or long) for automatic type conversion to decimal.
/// The compiler unwraps this to compile the original expression directly.
/// </summary>
internal class SqlDecimalImplicit(SqlExpr value) : SqlExprDecimal
{
	public void Deconstruct(out SqlExpr valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL addition operation between two decimal expressions.
/// Equivalent to: left + right
/// </summary>
internal class SqlDecimalAdd(SqlExpr left, SqlExpr right) : SqlExprDecimal
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}


/// <summary>
/// Represents a SQL subtraction operation between two decimal expressions.
/// Equivalent to: left - right
/// </summary>
internal class SqlDecimalSub(SqlExpr left, SqlExpr right) : SqlExprDecimal
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a SQL unary negation operation on a decimal expression.
/// Equivalent to: -value
/// </summary>
internal class SqlDecimalMinus(SqlExprDecimal value) : SqlExprDecimal
{
	public void Deconstruct(out SqlExprDecimal valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL multiplication operation between two decimal expressions.
/// Equivalent to: left * right
/// </summary>
internal class SqlDecimalMult(SqlExpr left, SqlExpr right) : SqlExprDecimal
{
	public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a SQL division operation between two decimal expressions.
/// Equivalent to: left / right
/// </summary>
internal class SqlDecimalDiv(SqlExpr left, SqlExpr right) : SqlExprDecimal
{
	public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a reference to a decimal column in a SQL table.
/// This class is used for column references in SQL queries.
/// </summary>
public class SqlDecimalColumn : SqlExprDecimal, ISqlColumn
{
    internal SqlDecimalColumn(string tableName, string columnName)
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
/// Represents a named decimal parameter in SQL expressions.
/// This class is used for parameterized queries where decimal values need to be bound at execution time.
/// </summary>
/// <param name="name">The name of the parameter (e.g., "@price", ":amount")</param>
internal class SqlParameterDecimal(string name) : SqlExprDecimal
{
	public void Deconstruct(out string nameOut) => nameOut = name;
}

/// <summary>
/// Represents a SQL CASE expression for conditional decimal values.
/// This class applies the SQL CASE WHEN condition THEN trueValue ELSE falseValue END construct.
/// </summary>
/// <param name="condition">The boolean condition to evaluate</param>
/// <param name="trueValue">The decimal expression returned when condition is true</param>
/// <param name="falseValue">The decimal expression returned when condition is false</param>
internal class SqlDecimalCase(SqlExprBool condition, SqlExprDecimal trueValue, SqlExprDecimal falseValue) : SqlExprDecimal
{
	public void Deconstruct(out SqlExprBool conditionOut, out SqlExprDecimal trueValueOut, out SqlExprDecimal falseValueOut) => 
		(conditionOut, trueValueOut, falseValueOut) = (condition, trueValue, falseValue);
}

/// <summary>
/// Represents a SQL SUM() aggregate function in SQL expressions.
/// This class applies the SQL SUM function to a decimal expression, summing all values.
/// </summary>
/// <param name="value">The decimal expression to apply the sum function to</param>
internal class SqlDecimalSum(SqlExprDecimal value) : SqlExprDecimal
{
	public void Deconstruct(out SqlExprDecimal valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL AVG() aggregate function in SQL expressions.
/// This class applies the SQL AVG function to a decimal expression, calculating the average value.
/// </summary>
/// <param name="value">The decimal expression to apply the average function to</param>
internal class SqlDecimalAvg(SqlExprDecimal value) : SqlExprDecimal
{
	public void Deconstruct(out SqlExprDecimal valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL MIN() aggregate function in SQL expressions.
/// This class applies the SQL MIN function to a decimal expression, finding the minimum value.
/// </summary>
/// <param name="value">The decimal expression to apply the minimum function to</param>
internal class SqlDecimalMin(SqlExprDecimal value) : SqlExprDecimal
{
	public void Deconstruct(out SqlExprDecimal valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL MAX() aggregate function in SQL expressions.
/// This class applies the SQL MAX function to a decimal expression, finding the maximum value.
/// </summary>
/// <param name="value">The decimal expression to apply the maximum function to</param>
internal class SqlDecimalMax(SqlExprDecimal value) : SqlExprDecimal
{
	public void Deconstruct(out SqlExprDecimal valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL NULL value for decimal expressions.
/// This class is used when setting decimal columns to NULL in SQL statements.
/// </summary>
internal class SqlDecimalNull : SqlExprDecimal, ISqlNullValue
{
	public static SqlDecimalNull Value => new();
}
