using System.Collections.Immutable;

namespace TypedSqlBuilder.Core;

/// <summary>
/// Contains SQL integer expression implementations for the TypedSqlBuilder system.
/// This file defines all the concrete classes that represent various integer operations
/// and comparisons in SQL queries, providing type-safe expression building capabilities.
/// 
/// The classes in this file fall into several categories:
/// - Value expressions: SqlIntValue (literal integers)
/// - Unary operations: SqlIntMinus, SqlIntAbs (unary operators and functions)
/// - Binary arithmetic: SqlIntAdd, SqlIntSub, SqlIntMult, SqlIntDiv (mathematical operations)
/// - Comparison operations: SqlIntEquals, SqlIntNotEquals, SqlIntGreaterThan, SqlIntLessThan, SqlIntGreaterThanOrEqualTo, SqlIntLessThanOrEqualTo
/// - Projection expressions: SqlIntProjection, SqlIntColumn (column references and projections)
/// 
/// All classes inherit from SqlExprInt which provides operator overloading for intuitive
/// SQL expression building using standard C# operators (==, !=, +, -, *, /, >, <, etc.).
/// </summary>

// Integer expression implementations

/// <summary>
/// Represents a literal integer value in SQL expressions.
/// This class encapsulates a constant integer value that can be used in SQL queries.
/// </summary>
/// <param name="value">The integer value to be represented in the SQL expression</param>
public class SqlIntValue(int value) : SqlExprInt
{
	public void Deconstruct(out int valueOut) => valueOut = value;
}

/// <summary>
/// Represents a unary minus operation in SQL expressions (-value).
/// This class applies a unary minus operator to an integer expression, negating its value.
/// </summary>
/// <param name="value">The integer expression to apply the unary minus operator to</param>
public class SqlIntMinus(SqlExprInt value) : SqlExprInt
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
public class SqlIntAbs(SqlExprInt value) : SqlExprInt
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
public class SqlIntCount() : SqlExprInt
{
}

/// <summary>
/// Represents a SQL SUM() aggregate function in SQL expressions.
/// This class applies the SQL SUM function to an integer expression, summing all values.
/// </summary>
/// <param name="value">The integer expression to apply the sum function to</param>
public class SqlIntSum(SqlExprInt value) : SqlExprInt
{
	public void Deconstruct(out SqlExprInt valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL AVG() aggregate function in SQL expressions.
/// This class applies the SQL AVG function to an integer expression, calculating the average value.
/// </summary>
/// <param name="value">The integer expression to apply the average function to</param>
public class SqlIntAvg(SqlExprInt value) : SqlExprInt
{
	public void Deconstruct(out SqlExprInt valueOut) => valueOut = value;
}

/// <summary>
/// Represents an addition operation in SQL expressions (left + right).
/// This class performs binary addition between two integer expressions.
/// </summary>
/// <param name="left">The left operand of the addition</param>
/// <param name="right">The right operand of the addition</param>
public class SqlIntAdd(SqlExprInt left, SqlExprInt right) : SqlExprInt
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
public class SqlIntSub(SqlExprInt left, SqlExprInt right) : SqlExprInt
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
public class SqlIntMult(SqlExprInt left, SqlExprInt right) : SqlExprInt
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
public class SqlIntDiv(SqlExprInt left, SqlExprInt right) : SqlExprInt
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
public class SqlIntColumn : SqlExprInt, ISqlColumn<SqlIntColumn>
{    
    private SqlIntColumn(ISqlTable table, string columnName)
    {
        Table = table;
        ColumnName = columnName;        
    }
    
    public ISqlTable Table { get; }
    public string ColumnName { get; }
    
    public static SqlIntColumn Create(ISqlTable table, string columnName) => new SqlIntColumn(table, columnName);
    
    public void Deconstruct(out ISqlTable tableOut, out string nameOut)
    {
        tableOut = Table;
        nameOut = ColumnName;
    }
}

/// <summary>
/// Represents a named integer parameter in SQL expressions.
/// This class is used for parameterized queries where integer values need to be bound at execution time.
/// </summary>
/// <param name="name">The name of the parameter (e.g., "@userId", ":count")</param>
public class SqlParameterInt(string name) : SqlExprInt
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
public class SqlIntCase(SqlExprBool condition, SqlExprInt trueValue, SqlExprInt falseValue) : SqlExprInt
{
	public void Deconstruct(out SqlExprBool conditionOut, out SqlExprInt trueValueOut, out SqlExprInt falseValueOut) => 
		(conditionOut, trueValueOut, falseValueOut) = (condition, trueValue, falseValue);
}

/// <summary>
/// Represents a SQL NULL value for integer expressions.
/// This class is used when setting integer columns to NULL in SQL statements.
/// </summary>
public class SqlIntNull : SqlExprInt, ISqlNullValue
{
    public static SqlIntNull Value => new();
}