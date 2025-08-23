using System.Collections.Immutable;

namespace TypedSqlBuilder.Core;

// Long expression implementations

/// <summary>
/// Represents a literal long value in SQL expressions.
/// This class encapsulates a constant long value that can be used in SQL queries.
/// </summary>
/// <param name="value">The long value to be represented in the SQL expression</param>
internal class SqlLongValue(long value) : SqlExprLong
{
    public void Deconstruct(out long valueOut) => valueOut = value;
}

/// <summary>
/// Represents an implicit conversion to a long expression.
/// This class wraps other SQL expressions (typically int) for automatic type conversion to long.
/// The compiler unwraps this to compile the original expression directly.
/// </summary>
internal class SqlLongImplicit(SqlExpr value) : SqlExprLong
{
	public void Deconstruct(out SqlExpr valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL addition operation between two long expressions.
/// Equivalent to: left + right
/// </summary>
internal class SqlLongAdd(SqlExpr left, SqlExpr right) : SqlExprLong
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a SQL subtraction operation between two long expressions.
/// Equivalent to: left - right
/// </summary>
internal class SqlLongSub(SqlExpr left, SqlExpr right) : SqlExprLong
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a unary minus operation in SQL expressions (-value).
/// This class applies a unary minus operator to a long expression, negating its value.
/// </summary>
/// <param name="value">The long expression to apply the unary minus operator to</param>
internal class SqlLongMinus(SqlExprLong value) : SqlExprLong
{
	public void Deconstruct(out SqlExprLong valueOut)
	{
		valueOut = value;
	}
}

/// <summary>
/// Represents a SQL multiplication operation between two long expressions.
/// Equivalent to: left * right
/// </summary>
internal class SqlLongMult(SqlExpr left, SqlExpr right) : SqlExprLong
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a SQL division operation between two long expressions.
/// Equivalent to: left / right
/// This class performs binary division between two long expressions.
/// </summary>
/// <param name="left">The left operand (dividend) of the division</param>
/// <param name="right">The right operand (divisor) of the division</param>
internal class SqlLongDiv(SqlExprLong left, SqlExprLong right) : SqlExprLong
{
	public void Deconstruct(out SqlExprLong leftOut, out SqlExprLong rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents an absolute value operation in SQL expressions (ABS(value)).
/// This class applies the SQL ABS function to a long expression, returning its absolute value.
/// </summary>
/// <param name="value">The long expression to apply the absolute value function to</param>
internal class SqlLongAbs(SqlExprLong value) : SqlExprLong
{
	public void Deconstruct(out SqlExprLong valueOut)
	{
		valueOut = value;
	}
}

/// <summary>
/// Represents a SQL SUM() aggregate function in SQL expressions.
/// This class applies the SQL SUM function to a long expression, summing all values.
/// </summary>
/// <param name="value">The long expression to apply the sum function to</param>
internal class SqlLongSum(SqlExprLong value) : SqlExprLong
{
	public void Deconstruct(out SqlExprLong valueOut)
	{
		valueOut = value;
	}
}

/// <summary>
/// Represents a SQL AVG() aggregate function in SQL expressions.
/// This class applies the SQL AVG function to a long expression, calculating the average value.
/// </summary>
/// <param name="value">The long expression to apply the average function to</param>
internal class SqlLongAvg(SqlExprLong value) : SqlExprLong
{
	public void Deconstruct(out SqlExprLong valueOut)
	{
		valueOut = value;
	}
}

/// <summary>
/// Represents a SQL MIN() aggregate function in SQL expressions.
/// This class applies the SQL MIN function to a long expression, finding the minimum value.
/// </summary>
/// <param name="value">The long expression to apply the minimum function to</param>
internal class SqlLongMin(SqlExprLong value) : SqlExprLong
{
	public void Deconstruct(out SqlExprLong valueOut)
	{
		valueOut = value;
	}
}

/// <summary>
/// Represents a SQL MAX() aggregate function in SQL expressions.
/// This class applies the SQL MAX function to a long expression, finding the maximum value.
/// </summary>
/// <param name="value">The long expression to apply the maximum function to</param>
internal class SqlLongMax(SqlExprLong value) : SqlExprLong
{
	public void Deconstruct(out SqlExprLong valueOut)
	{
		valueOut = value;
	}
}

/// <summary>
/// Represents a reference to a long column in a SQL table.
/// This class is used for column references in SQL queries.
/// </summary>
public class SqlLongColumn : SqlExprLong, ISqlColumn
{    
    internal SqlLongColumn(string tableName, string columnName)
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
/// Represents a named long parameter in SQL expressions.
/// This class is used for parameterized queries where long values need to be bound at execution time.
/// </summary>
/// <param name="name">The name of the parameter (e.g., "@userId", ":count")</param>
internal class SqlParameterLong(string name) : SqlExprLong
{
	public void Deconstruct(out string nameOut) => nameOut = name;
}

/// <summary>
/// Represents a SQL CASE WHEN expression for long expressions.
/// This class applies the SQL CASE WHEN condition THEN trueValue ELSE falseValue END construct.
/// </summary>
/// <param name="condition">The boolean condition to evaluate</param>
/// <param name="trueValue">The long expression returned when condition is true</param>
/// <param name="falseValue">The long expression returned when condition is false</param>
internal class SqlLongCase(SqlExprBool condition, SqlExprLong trueValue, SqlExprLong falseValue) : SqlExprLong
{
	public void Deconstruct(out SqlExprBool conditionOut, out SqlExprLong trueValueOut, out SqlExprLong falseValueOut)
	{
		conditionOut = condition;
		trueValueOut = trueValue;
		falseValueOut = falseValue;
	}
}

/// <summary>
/// Represents a SQL NULL value for long expressions.
/// This class is used when setting long columns to NULL in SQL statements.
/// </summary>
internal class SqlLongNull : SqlExprLong, ISqlNullValue
{
    public static SqlLongNull Value => new();
}

/// <summary>
/// Abstract base class for SQL IN expressions with a list of long values.
/// Represents the SQL IN operator: expr IN (value1, value2, value3, ...)
/// This class serves as the foundation for type-safe IN operations with multiple long values.
/// </summary>
/// <param name="expression">The expression to test against the list of values</param>
/// <param name="values">The collection of long values to test the expression against</param>
internal class SqlLongInValues(SqlExprLong expression, ImmutableArray<SqlExprLong> values) : SqlExprBool
{
	public void Deconstruct(out SqlExprLong expressionOut, out ImmutableArray<SqlExprLong> valuesOut)
	{
		expressionOut = expression;
		valuesOut = values;
	}
}

/// <summary>
/// Represents a SQL IN expression with a subquery for long expressions.
/// Handles the SQL IN operator: expr IN (SELECT ...)
/// This class provides type-safe IN operations with subqueries returning long values.
/// </summary>
/// <param name="expression">The long expression to test for membership</param>
/// <param name="subquery">The subquery that returns long values to test against</param>
internal class SqlLongInSubquery(SqlExprLong expression, ISqlQuery subquery) : SqlExprBool
{
	public void Deconstruct(out SqlExprLong expressionOut, out ISqlQuery subqueryOut)
	{
		expressionOut = expression;
		subqueryOut = subquery;
	}
}
