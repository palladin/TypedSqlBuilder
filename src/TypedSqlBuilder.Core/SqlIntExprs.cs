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
/// Represents a greater than comparison operation in SQL expressions (left > right).
/// This class performs a binary comparison between two integer expressions, returning a boolean result.
/// </summary>
/// <param name="left">The left operand of the comparison</param>
/// <param name="right">The right operand of the comparison</param>
public class SqlIntGreaterThan(SqlExprInt left, SqlExprInt right) : SqlExprBool
{
	public void Deconstruct(out SqlExprInt leftOut, out SqlExprInt rightOut)
	{
		leftOut = left;
		rightOut = right;
	}

}

/// <summary>
/// Represents a less than comparison operation in SQL expressions (left &lt; right).
/// This class performs a binary comparison between two integer expressions, returning a boolean result.
/// </summary>
/// <param name="left">The left operand of the comparison</param>
/// <param name="right">The right operand of the comparison</param>
public class SqlIntLessThan(SqlExprInt left, SqlExprInt right) : SqlExprBool
{
	public void Deconstruct(out SqlExprInt leftOut, out SqlExprInt rightOut)
	{
		leftOut = left;
		rightOut = right;
	}

}

/// <summary>
/// Represents a greater than or equal to comparison operation in SQL expressions (left >= right).
/// This class performs a binary comparison between two integer expressions, returning a boolean result.
/// </summary>
/// <param name="left">The left operand of the comparison</param>
/// <param name="right">The right operand of the comparison</param>
public class SqlIntGreaterThanOrEqualTo(SqlExprInt left, SqlExprInt right) : SqlExprBool
{
	public void Deconstruct(out SqlExprInt leftOut, out SqlExprInt rightOut)
	{
		leftOut = left;
		rightOut = right;
	}

}

/// <summary>
/// Represents a less than or equal to comparison operation in SQL expressions (left &lt;= right).
/// This class performs a binary comparison between two integer expressions, returning a boolean result.
/// </summary>
/// <param name="left">The left operand of the comparison</param>
/// <param name="right">The right operand of the comparison</param>
public class SqlIntLessThanOrEqualTo(SqlExprInt left, SqlExprInt right) : SqlExprBool
{
	public void Deconstruct(out SqlExprInt leftOut, out SqlExprInt rightOut)
	{
		leftOut = left;
		rightOut = right;
	}

}

/// <summary>
/// Represents an equality comparison operation in SQL expressions (left = right).
/// This class performs a binary comparison between two integer expressions, returning a boolean result.
/// </summary>
/// <param name="left">The left operand of the equality comparison</param>
/// <param name="right">The right operand of the equality comparison</param>
public class SqlIntEquals(SqlExprInt left, SqlExprInt right) : SqlExprBool
{
	public void Deconstruct(out SqlExprInt leftOut, out SqlExprInt rightOut)
	{
		leftOut = left;
		rightOut = right;
	}

}

/// <summary>
/// Represents a not equal comparison operation in SQL expressions (left != right or left &lt;&gt; right).
/// This class performs a binary comparison between two integer expressions, returning a boolean result.
/// </summary>
/// <param name="left">The left operand of the inequality comparison</param>
/// <param name="right">The right operand of the inequality comparison</param>
public class SqlIntNotEquals(SqlExprInt left, SqlExprInt right) : SqlExprBool
{
	public void Deconstruct(out SqlExprInt leftOut, out SqlExprInt rightOut)
	{
		leftOut = left;
		rightOut = right;
	}

}


/// <summary>
/// Represents a projection of an integer column or expression in SQL queries.
/// This class serves as a base for referencing integer values from tables or subqueries,
/// typically used in SELECT clauses or other projection contexts.
/// </summary>
/// <param name="source">The table alias or source identifier</param>
/// <param name="name">The column or expression name</param>
public class SqlIntProjection(string source, string name) : SqlExprInt
{
	public void Deconstruct(out string sourceOut, out string nameOut)
	{
		sourceOut = source;
		nameOut = name;
	}
}

/// <summary>
/// Represents a reference to an integer column in a SQL table.
/// Inherits from SqlIntProjection to support column references in SQL queries.
/// </summary>
/// <param name="source">The table name or alias that contains the column</param>
/// <param name="name">The name of the column</param>
public class SqlIntColumn(string source, string name) : SqlIntProjection(source, name);

/// <summary>
/// Represents a named integer parameter in SQL expressions.
/// This class is used for parameterized queries where integer values need to be bound at execution time.
/// </summary>
/// <param name="name">The name of the parameter (e.g., "@userId", ":count")</param>
public class SqlParameterInt(string name) : SqlExprInt
{
	public void Deconstruct(out string nameOut) => nameOut = name;
}