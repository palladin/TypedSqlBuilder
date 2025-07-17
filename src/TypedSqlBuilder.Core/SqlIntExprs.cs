namespace TypedSqlBuilder.Core;

/// <summary>
/// Contains SQL integer expression implementations for the TypedSqlBuilder system.
/// This file defines all the concrete classes that represent various integer operations
/// and comparisons in SQL queries, providing type-safe expression building capabilities.
/// 
/// The classes in this file fall into several categories:
/// - Value expressions: SqlIntValue (literal integers)
/// - Unary operations: SqlIntPlus, SqlIntMinus, SqlIntAbs (unary operators and functions)
/// - Binary arithmetic: SqlIntAdd, SqlIntSub, SqlIntMult, SqlIntDiv (mathematical operations)
/// - Comparison operations: SqlIntEquals, SqlIntNotEquals, SqlIntGreaterThan, SqlIntLessThan, etc.
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
	/// <summary>
	/// Gets the integer value represented by this SQL expression.
	/// </summary>
	public int Value { get; } = value;
}

/// <summary>
/// Represents a unary plus operation in SQL expressions (+value).
/// This class applies a unary plus operator to an integer expression.
/// </summary>
/// <param name="value">The integer expression to apply the unary plus operator to</param>
public class SqlIntPlus(ISqlExpr<ISqlInt> value) : SqlExprInt, ISqlUnaryExpr<ISqlInt>
{
	/// <summary>
	/// Gets the integer expression that the unary plus operator is applied to.
	/// </summary>
	public ISqlExpr<ISqlInt> Value { get; } = value;
}

/// <summary>
/// Represents a unary minus operation in SQL expressions (-value).
/// This class applies a unary minus operator to an integer expression, negating its value.
/// </summary>
/// <param name="value">The integer expression to apply the unary minus operator to</param>
public class SqlIntMinus(ISqlExpr<ISqlInt> value) : SqlExprInt, ISqlUnaryExpr<ISqlInt>
{
	/// <summary>
	/// Gets the integer expression that the unary minus operator is applied to.
	/// </summary>
	public ISqlExpr<ISqlInt> Value { get; } = value;
}

/// <summary>
/// Represents an absolute value operation in SQL expressions (ABS(value)).
/// This class applies the SQL ABS function to an integer expression, returning its absolute value.
/// </summary>
/// <param name="value">The integer expression to apply the absolute value function to</param>
public class SqlIntAbs(ISqlExpr<ISqlInt> value) : SqlExprInt, ISqlUnaryExpr<ISqlInt>
{
	/// <summary>
	/// Gets the integer expression that the absolute value function is applied to.
	/// </summary>
	public ISqlExpr<ISqlInt> Value { get; } = value;
}

/// <summary>
/// Represents an addition operation in SQL expressions (left + right).
/// This class performs binary addition between two integer expressions.
/// </summary>
/// <param name="left">The left operand of the addition</param>
/// <param name="right">The right operand of the addition</param>
public class SqlIntAdd(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprInt, ISqlBinExpr<ISqlInt>
{
	/// <summary>
	/// Gets the left operand of the addition operation.
	/// </summary>
	public ISqlExpr<ISqlInt> Left { get; } = left;
	/// <summary>
	/// Gets the right operand of the addition operation.
	/// </summary>
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

/// <summary>
/// Represents a subtraction operation in SQL expressions (left - right).
/// This class performs binary subtraction between two integer expressions.
/// </summary>
/// <param name="left">The left operand of the subtraction</param>
/// <param name="right">The right operand of the subtraction</param>
public class SqlIntSub(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprInt, ISqlBinExpr<ISqlInt>
{
	/// <summary>
	/// Gets the left operand of the subtraction operation.
	/// </summary>
	public ISqlExpr<ISqlInt> Left { get; } = left;
	/// <summary>
	/// Gets the right operand of the subtraction operation.
	/// </summary>
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

/// <summary>
/// Represents a multiplication operation in SQL expressions (left * right).
/// This class performs binary multiplication between two integer expressions.
/// </summary>
/// <param name="left">The left operand of the multiplication</param>
/// <param name="right">The right operand of the multiplication</param>
public class SqlIntMult(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprInt, ISqlBinExpr<ISqlInt>
{
	/// <summary>
	/// Gets the left operand of the multiplication operation.
	/// </summary>
	public ISqlExpr<ISqlInt> Left { get; } = left;
	/// <summary>
	/// Gets the right operand of the multiplication operation.
	/// </summary>
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

/// <summary>
/// Represents a division operation in SQL expressions (left / right).
/// This class performs binary division between two integer expressions.
/// </summary>
/// <param name="left">The left operand (dividend) of the division</param>
/// <param name="right">The right operand (divisor) of the division</param>
public class SqlIntDiv(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprInt, ISqlBinExpr<ISqlInt>
{
	/// <summary>
	/// Gets the left operand (dividend) of the division operation.
	/// </summary>
	public ISqlExpr<ISqlInt> Left { get; } = left;
	/// <summary>
	/// Gets the right operand (divisor) of the division operation.
	/// </summary>
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

/// <summary>
/// Represents a greater than comparison operation in SQL expressions (left > right).
/// This class performs a binary comparison between two integer expressions, returning a boolean result.
/// </summary>
/// <param name="left">The left operand of the comparison</param>
/// <param name="right">The right operand of the comparison</param>
public class SqlIntGreaterThan(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprBool, ISqlBinExpr<ISqlInt>
{
	/// <summary>
	/// Gets the left operand of the greater than comparison.
	/// </summary>
	public ISqlExpr<ISqlInt> Left { get; } = left;
	/// <summary>
	/// Gets the right operand of the greater than comparison.
	/// </summary>
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

/// <summary>
/// Represents a less than comparison operation in SQL expressions (left &lt; right).
/// This class performs a binary comparison between two integer expressions, returning a boolean result.
/// </summary>
/// <param name="left">The left operand of the comparison</param>
/// <param name="right">The right operand of the comparison</param>
public class SqlIntLessThan(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprBool, ISqlBinExpr<ISqlInt>
{
	/// <summary>
	/// Gets the left operand of the less than comparison.
	/// </summary>
	public ISqlExpr<ISqlInt> Left { get; } = left;
	/// <summary>
	/// Gets the right operand of the less than comparison.
	/// </summary>
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

/// <summary>
/// Represents a greater than or equal to comparison operation in SQL expressions (left >= right).
/// This class performs a binary comparison between two integer expressions, returning a boolean result.
/// </summary>
/// <param name="left">The left operand of the comparison</param>
/// <param name="right">The right operand of the comparison</param>
public class SqlIntGreaterThanOrEqualTo(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprBool, ISqlBinExpr<ISqlInt>
{
	/// <summary>
	/// Gets the left operand of the greater than or equal to comparison.
	/// </summary>
	public ISqlExpr<ISqlInt> Left { get; } = left;
	/// <summary>
	/// Gets the right operand of the greater than or equal to comparison.
	/// </summary>
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

/// <summary>
/// Represents a less than or equal to comparison operation in SQL expressions (left &lt;= right).
/// This class performs a binary comparison between two integer expressions, returning a boolean result.
/// </summary>
/// <param name="left">The left operand of the comparison</param>
/// <param name="right">The right operand of the comparison</param>
public class SqlIntLessThanOrEqualTo(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprBool, ISqlBinExpr<ISqlInt>
{
	/// <summary>
	/// Gets the left operand of the less than or equal to comparison.
	/// </summary>
	public ISqlExpr<ISqlInt> Left { get; } = left;
	/// <summary>
	/// Gets the right operand of the less than or equal to comparison.
	/// </summary>
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

/// <summary>
/// Represents an equality comparison operation in SQL expressions (left = right).
/// This class performs a binary comparison between two integer expressions, returning a boolean result.
/// </summary>
/// <param name="left">The left operand of the equality comparison</param>
/// <param name="right">The right operand of the equality comparison</param>
public class SqlIntEquals(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprBool, ISqlBinExpr<ISqlInt>
{
	/// <summary>
	/// Gets the left operand of the equality comparison.
	/// </summary>
	public ISqlExpr<ISqlInt> Left { get; } = left;
	/// <summary>
	/// Gets the right operand of the equality comparison.
	/// </summary>
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

/// <summary>
/// Represents a not equal comparison operation in SQL expressions (left != right or left &lt;&gt; right).
/// This class performs a binary comparison between two integer expressions, returning a boolean result.
/// </summary>
/// <param name="left">The left operand of the inequality comparison</param>
/// <param name="right">The right operand of the inequality comparison</param>
public class SqlIntNotEquals(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprBool, ISqlBinExpr<ISqlInt>
{
	/// <summary>
	/// Gets the left operand of the not equal comparison.
	/// </summary>
	public ISqlExpr<ISqlInt> Left { get; } = left;
	/// <summary>
	/// Gets the right operand of the not equal comparison.
	/// </summary>
	public ISqlExpr<ISqlInt> Right { get; } = right;
}


/// <summary>
/// Represents a projection of an integer column or expression in SQL queries.
/// This class serves as a base for referencing integer values from tables or subqueries,
/// typically used in SELECT clauses or other projection contexts.
/// </summary>
/// <param name="alias">The table alias or source identifier</param>
/// <param name="name">The column or expression name</param>
public class SqlIntProjection(string alias, string name) : SqlExprInt, ISqlProjectionExpr<ISqlInt>
{
	/// <summary>
	/// Gets the source (table name or alias) that contains the projected integer value.
	/// </summary>
	public string Source { get; } = alias;
	/// <summary>
	/// Gets the name of the column or expression being projected.
	/// </summary>
	public string Name { get; } = name;
}

/// <summary>
/// Represents a reference to an integer column in a SQL table.
/// Inherits from SqlIntProjection to support column references in SQL queries.
/// </summary>
/// <param name="source">The table name or alias that contains the column</param>
/// <param name="name">The name of the column</param>
public class SqlIntColumn(string source, string name) : SqlIntProjection(source, name);