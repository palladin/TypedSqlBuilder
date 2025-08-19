using System.Collections.Immutable;

namespace TypedSqlBuilder.Core;

// Integer expression implementations

/// <summary>
/// Represents a literal integer value in SQL expressions.
/// This class encapsulates a constant integer value that can be used in SQL queries.
/// </summary>
/// <param name="value">The integer value to be represented in the SQL expression</param>
internal class SqlIntValue(int value) : SqlExprInt
{
	public void Deconstruct(out int valueOut) => valueOut = value;
}

/// <summary>
/// Represents a unary minus operation in SQL expressions (-value).
/// This class applies a unary minus operator to an integer expression, negating its value.
/// </summary>
/// <param name="value">The integer expression to apply the unary minus operator to</param>
internal class SqlIntMinus(SqlExprInt value) : SqlExprInt
{
	public void Deconstruct(out SqlExprInt valueOut)
	{
		valueOut = value;
	}

}

/// <summary>
/// Represents an absolute value operation in SQL expressions (ABS(value)).
/// This class applies the SQL ABS function to an integer expression, returning its absolute value.
/// </summary>
/// <param name="value">The integer expression to apply the absolute value function to</param>
internal class SqlIntAbs(SqlExprInt value) : SqlExprInt
{
	public void Deconstruct(out SqlExprInt valueOut)
	{
		valueOut = value;
	}

}

/// <summary>
/// Represents a SQL COUNT() aggregate function.
/// This class applies the SQL COUNT function to count rows or non-null values.
/// </summary>
internal class SqlIntCount() : SqlExprInt
{
}

/// <summary>
/// Represents a SQL SUM() aggregate function in SQL expressions.
/// This class applies the SQL SUM function to an integer expression, summing all values.
/// </summary>
/// <param name="value">The integer expression to apply the sum function to</param>
internal class SqlIntSum(SqlExprInt value) : SqlExprInt
{
	public void Deconstruct(out SqlExprInt valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL AVG() aggregate function in SQL expressions.
/// This class applies the SQL AVG function to an integer expression, calculating the average value.
/// </summary>
/// <param name="value">The integer expression to apply the average function to</param>
internal class SqlIntAvg(SqlExprInt value) : SqlExprInt
{
	public void Deconstruct(out SqlExprInt valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL MIN() aggregate function in SQL expressions.
/// This class applies the SQL MIN function to an integer expression, finding the minimum value.
/// </summary>
/// <param name="value">The integer expression to apply the minimum function to</param>
internal class SqlIntMin(SqlExprInt value) : SqlExprInt
{
	public void Deconstruct(out SqlExprInt valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL MAX() aggregate function in SQL expressions.
/// This class applies the SQL MAX function to an integer expression, finding the maximum value.
/// </summary>
/// <param name="value">The integer expression to apply the maximum function to</param>
internal class SqlIntMax(SqlExprInt value) : SqlExprInt
{
	public void Deconstruct(out SqlExprInt valueOut) => valueOut = value;
}

/// <summary>
/// Represents an addition operation in SQL expressions (left + right).
/// This class performs binary addition between two integer expressions.
/// </summary>
/// <param name="left">The left operand of the addition</param>
/// <param name="right">The right operand of the addition</param>
internal class SqlIntAdd(SqlExprInt left, SqlExprInt right) : SqlExprInt
{
	public void Deconstruct(out SqlExprInt leftOut, out SqlExprInt rightOut)
	{
		leftOut = left;
		rightOut = right;
	}
}

/// <summary>
/// Represents a subtraction operation in SQL expressions (left - right).
/// This class performs binary subtraction between two integer expressions.
/// </summary>
/// <param name="left">The left operand of the subtraction</param>
/// <param name="right">The right operand of the subtraction</param>
internal class SqlIntSub(SqlExprInt left, SqlExprInt right) : SqlExprInt
{
	public void Deconstruct(out SqlExprInt leftOut, out SqlExprInt rightOut)
	{
		leftOut = left;
		rightOut = right;
	}
}

/// <summary>
/// Represents a multiplication operation in SQL expressions (left * right).
/// This class performs binary multiplication between two integer expressions.
/// </summary>
/// <param name="left">The left operand of the multiplication</param>
/// <param name="right">The right operand of the multiplication</param>
internal class SqlIntMult(SqlExprInt left, SqlExprInt right) : SqlExprInt
{
	public void Deconstruct(out SqlExprInt leftOut, out SqlExprInt rightOut)
	{
		leftOut = left;
		rightOut = right;
	}

}

/// <summary>
/// Represents a division operation in SQL expressions (left / right).
/// This class performs binary division between two integer expressions.
/// </summary>
/// <param name="left">The left operand (dividend) of the division</param>
/// <param name="right">The right operand (divisor) of the division</param>
internal class SqlIntDiv(SqlExprInt left, SqlExprInt right) : SqlExprInt
{
	public void Deconstruct(out SqlExprInt leftOut, out SqlExprInt rightOut)
	{
		leftOut = left;
		rightOut = right;
	}

}

/// <summary>
/// Represents a reference to an integer column in a SQL table.
/// This class is used for column references in SQL queries.
/// </summary>
public class SqlIntColumn : SqlExprInt, ISqlColumn
{    
    internal SqlIntColumn(string tableName, string columnName)
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
/// Represents a named integer parameter in SQL expressions.
/// This class is used for parameterized queries where integer values need to be bound at execution time.
/// </summary>
/// <param name="name">The name of the parameter (e.g., "@userId", ":count")</param>
internal class SqlParameterInt(string name) : SqlExprInt
{
	public void Deconstruct(out string nameOut) => nameOut = name;
}

/// <summary>
/// Represents a SQL CASE expression for conditional integer values.
/// This class applies the SQL CASE WHEN condition THEN trueValue ELSE falseValue END construct.
/// </summary>
/// <param name="condition">The boolean condition to evaluate</param>
/// <param name="trueValue">The integer expression returned when condition is true</param>
/// <param name="falseValue">The integer expression returned when condition is false</param>
internal class SqlIntCase(SqlExprBool condition, SqlExprInt trueValue, SqlExprInt falseValue) : SqlExprInt
{
	public void Deconstruct(out SqlExprBool conditionOut, out SqlExprInt trueValueOut, out SqlExprInt falseValueOut) => 
		(conditionOut, trueValueOut, falseValueOut) = (condition, trueValue, falseValue);
}

/// <summary>
/// Represents a SQL NULL value for integer expressions.
/// This class is used when setting integer columns to NULL in SQL statements.
/// </summary>
internal class SqlIntNull : SqlExprInt, ISqlNullValue
{
    public static SqlIntNull Value => new();
}

/// <summary>
/// Represents a SQL ROUND function for rounding decimal values to specified precision.
/// Equivalent to: ROUND(value, precision)
/// </summary>
/// <param name="value">The decimal expression to round</param>
/// <param name="precision">The number of decimal places</param>
internal class SqlDecimalRound(SqlExprDecimal value, SqlExprInt precision) : SqlExprDecimal
{
	public void Deconstruct(out SqlExprDecimal valueOut, out SqlExprInt precisionOut) => 
		(valueOut, precisionOut) = (value, precision);
}

/// <summary>
/// Represents a SQL CEILING function for rounding up to nearest integer.
/// Equivalent to: CEILING(value) or CEIL(value)
/// </summary>
/// <param name="value">The decimal expression to round up</param>
internal class SqlDecimalCeiling(SqlExprDecimal value) : SqlExprInt
{
	public void Deconstruct(out SqlExprDecimal valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL FLOOR function for rounding down to nearest integer.
/// Equivalent to: FLOOR(value)
/// </summary>
/// <param name="value">The decimal expression to round down</param>
internal class SqlDecimalFloor(SqlExprDecimal value) : SqlExprInt
{
	public void Deconstruct(out SqlExprDecimal valueOut) => valueOut = value;
}