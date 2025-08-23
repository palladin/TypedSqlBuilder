using System.Collections.Immutable;

namespace TypedSqlBuilder.Core;

// Double expression implementations

/// <summary>
/// Represents a literal double value in SQL expressions.
/// This class encapsulates a constant double value that can be used in SQL queries.
/// </summary>
/// <param name="value">The double value to be represented in the SQL expression</param>
internal class SqlDoubleValue(double value) : SqlExprDouble
{
	public void Deconstruct(out double valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL addition operation between two double expressions.
/// Equivalent to: left + right
/// </summary>
internal class SqlDoubleAdd(SqlExpr left, SqlExpr right) : SqlExprDouble
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a SQL subtraction operation between two double expressions.
/// Equivalent to: left - right
/// </summary>
internal class SqlDoubleSub(SqlExpr left, SqlExpr right) : SqlExprDouble
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a unary minus operation in SQL expressions (-value).
/// This class applies a unary minus operator to a double expression, negating its value.
/// </summary>
/// <param name="value">The double expression to apply the unary minus operator to</param>
internal class SqlDoubleMinus(SqlExprDouble value) : SqlExprDouble
{
	public void Deconstruct(out SqlExprDouble valueOut)
	{
		valueOut = value;
	}
}

/// <summary>
/// Represents a SQL multiplication operation between two double expressions.
/// Equivalent to: left * right
/// </summary>
internal class SqlDoubleMult(SqlExpr left, SqlExpr right) : SqlExprDouble
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a SQL division operation between two double expressions.
/// Equivalent to: left / right
/// This class performs binary division between two double expressions.
/// </summary>
/// <param name="left">The left operand (dividend) of the division</param>
/// <param name="right">The right operand (divisor) of the division</param>
internal class SqlDoubleDiv(SqlExprDouble left, SqlExprDouble right) : SqlExprDouble
{
	public void Deconstruct(out SqlExprDouble leftOut, out SqlExprDouble rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents an absolute value operation in SQL expressions (ABS(value)).
/// This class applies the SQL ABS function to a double expression, returning its absolute value.
/// </summary>
/// <param name="value">The double expression to apply the absolute value function to</param>
internal class SqlDoubleAbs(SqlExprDouble value) : SqlExprDouble
{
	public void Deconstruct(out SqlExprDouble valueOut)
	{
		valueOut = value;
	}
}

/// <summary>
/// Represents a SQL SUM() aggregate function in SQL expressions.
/// This class applies the SQL SUM function to a double expression, summing all values.
/// </summary>
/// <param name="value">The double expression to apply the sum function to</param>
internal class SqlDoubleSum(SqlExprDouble value) : SqlExprDouble
{
	public void Deconstruct(out SqlExprDouble valueOut)
	{
		valueOut = value;
	}
}

/// <summary>
/// Represents a SQL AVG() aggregate function in SQL expressions.
/// This class applies the SQL AVG function to a double expression, calculating the average value.
/// </summary>
/// <param name="value">The double expression to apply the average function to</param>
internal class SqlDoubleAvg(SqlExprDouble value) : SqlExprDouble
{
	public void Deconstruct(out SqlExprDouble valueOut)
	{
		valueOut = value;
	}
}

/// <summary>
/// Represents a SQL MIN() aggregate function in SQL expressions.
/// This class applies the SQL MIN function to a double expression, finding the minimum value.
/// </summary>
/// <param name="value">The double expression to apply the minimum function to</param>
internal class SqlDoubleMin(SqlExprDouble value) : SqlExprDouble
{
	public void Deconstruct(out SqlExprDouble valueOut)
	{
		valueOut = value;
	}
}

/// <summary>
/// Represents a SQL MAX() aggregate function in SQL expressions.
/// This class applies the SQL MAX function to a double expression, finding the maximum value.
/// </summary>
/// <param name="value">The double expression to apply the maximum function to</param>
internal class SqlDoubleMax(SqlExprDouble value) : SqlExprDouble
{
	public void Deconstruct(out SqlExprDouble valueOut)
	{
		valueOut = value;
	}
}

/// <summary>
/// Represents a reference to a double column in a SQL table.
/// This class is used for column references in SQL queries.
/// </summary>
public class SqlDoubleColumn : SqlExprDouble, ISqlColumn
{    
    internal SqlDoubleColumn(string tableName, string columnName)
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
/// Represents a named double parameter in SQL expressions.
/// This class is used for parameterized queries where double values need to be bound at execution time.
/// </summary>
/// <param name="name">The name of the parameter (e.g., "@value", ":rate")</param>
internal class SqlParameterDouble(string name) : SqlExprDouble
{
	public void Deconstruct(out string nameOut) => nameOut = name;
}

/// <summary>
/// Represents a SQL CASE WHEN expression for double expressions.
/// This class applies the SQL CASE WHEN condition THEN trueValue ELSE falseValue END construct.
/// </summary>
/// <param name="condition">The boolean condition to evaluate</param>
/// <param name="trueValue">The double expression returned when condition is true</param>
/// <param name="falseValue">The double expression returned when condition is false</param>
internal class SqlDoubleCase(SqlExprBool condition, SqlExprDouble trueValue, SqlExprDouble falseValue) : SqlExprDouble
{
	public void Deconstruct(out SqlExprBool conditionOut, out SqlExprDouble trueValueOut, out SqlExprDouble falseValueOut)
	{
		conditionOut = condition;
		trueValueOut = trueValue;
		falseValueOut = falseValue;
	}
}

/// <summary>
/// Represents a SQL NULL value for double expressions.
/// This class is used when setting double columns to NULL in SQL statements.
/// </summary>
internal class SqlDoubleNull : SqlExprDouble, ISqlNullValue
{
    public static SqlDoubleNull Value => new();
}

/// <summary>
/// Represents a SQL ROUND function for rounding double values to specified precision.
/// Equivalent to: ROUND(value, precision)
/// </summary>
/// <param name="value">The double expression to round</param>
/// <param name="precision">The number of decimal places</param>
internal class SqlDoubleRound(SqlExprDouble value, SqlExprInt precision) : SqlExprDouble
{
	public void Deconstruct(out SqlExprDouble valueOut, out SqlExprInt precisionOut) => 
		(valueOut, precisionOut) = (value, precision);
}

/// <summary>
/// Represents a SQL CEILING function for rounding up double values.
/// Equivalent to: CEILING(value) or CEIL(value)
/// </summary>
/// <param name="value">The double expression to apply ceiling to</param>
internal class SqlDoubleCeiling(SqlExprDouble value) : SqlExprDouble
{
	public void Deconstruct(out SqlExprDouble valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL FLOOR function for rounding down double values.
/// Equivalent to: FLOOR(value)
/// </summary>
/// <param name="value">The double expression to apply floor to</param>
internal class SqlDoubleFloor(SqlExprDouble value) : SqlExprDouble
{
	public void Deconstruct(out SqlExprDouble valueOut) => valueOut = value;
}

/// <summary>
/// Abstract base class for SQL IN expressions with a list of double values.
/// Represents the SQL IN operator: expr IN (value1, value2, value3, ...)
/// This class serves as the foundation for type-safe IN operations with multiple double values.
/// </summary>
/// <param name="expression">The expression to test against the list of values</param>
/// <param name="values">The collection of double values to test the expression against</param>
internal class SqlDoubleInValues(SqlExprDouble expression, ImmutableArray<SqlExprDouble> values) : SqlExprBool
{
	public void Deconstruct(out SqlExprDouble expressionOut, out ImmutableArray<SqlExprDouble> valuesOut)
	{
		expressionOut = expression;
		valuesOut = values;
	}
}

/// <summary>
/// Represents a SQL IN expression with a subquery for double expressions.
/// Handles the SQL IN operator: expr IN (SELECT ...)
/// This class provides type-safe IN operations with subqueries returning double values.
/// </summary>
/// <param name="expression">The double expression to test for membership</param>
/// <param name="subquery">The subquery that returns double values to test against</param>
internal class SqlDoubleInSubquery(SqlExprDouble expression, ISqlQuery subquery) : SqlExprBool
{
	public void Deconstruct(out SqlExprDouble expressionOut, out ISqlQuery subqueryOut)
	{
		expressionOut = expression;
		subqueryOut = subquery;
	}
}
