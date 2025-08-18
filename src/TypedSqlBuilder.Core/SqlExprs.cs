using System.Runtime.CompilerServices;

namespace TypedSqlBuilder.Core;

// Note: All SQL expression base classes use abstract class instead of record
// to enable custom operator overloading that returns SQL expression types rather than primitive types.
// The problem was that records implement their own ==, != operators which would conflict with our SQL expression operators.
// This is essential for maintaining type safety in the fluent SQL expression building system.

/// <summary>
/// Abstract base class for all SQL expressions.
/// Provides the foundation for the SQL expression type hierarchy.
/// </summary>
public abstract class SqlExpr;

/// <summary>
/// Marker interface for SQL NULL value expressions.
/// Used to identify NULL values across all SQL expression types for simplified pattern matching.
/// </summary>
public interface ISqlNullValue
{
}



#pragma warning disable CS0660, CS0661
/// <summary>
/// Abstract base class for SQL boolean expressions.
/// </summary>
public abstract class SqlExprBool : SqlExpr
{
	/// <summary>
	/// Implicitly converts a boolean value to a SqlExprBool.
	/// </summary>
	/// <param name="value">The boolean value to convert</param>
	/// <returns>A SqlExprBool representing the boolean value</returns>
	public static implicit operator SqlExprBool(bool value) => new SqlBoolValue(value);

	/// <summary>
	/// Implements the equality operator (==) for boolean expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the equality comparison</returns>
	public static SqlExprBool operator ==(SqlExprBool left, SqlExprBool right) => new SqlEquals<SqlExprBool>(left, right);
	
	/// <summary>
	/// Implements the inequality operator (!=) for boolean expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the inequality comparison</returns>
	public static SqlExprBool operator !=(SqlExprBool left, SqlExprBool right) => new SqlNotEquals<SqlExprBool>(left, right);

	/// <summary>
	/// Implements the logical AND operator (&amp;) for boolean expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the logical AND operation</returns>
	public static SqlExprBool operator &(SqlExprBool left, SqlExprBool right) => new SqlBoolAnd(left, right);
	
	/// <summary>
	/// Implements the logical OR operator (|) for boolean expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the logical OR operation</returns>
	public static SqlExprBool operator |(SqlExprBool left, SqlExprBool right) => new SqlBoolOr(left, right);
	
	/// <summary>
	/// Implements the logical NOT operator (!) for boolean expressions.
	/// </summary>
	/// <param name="value">The operand to negate</param>
	/// <returns>A boolean expression representing the logical NOT operation</returns>
	public static SqlExprBool operator !(SqlExprBool value) => new SqlBoolNot(value);
	
	/// <summary>
	/// Required for short-circuit evaluation. Always returns false to prevent short-circuiting.
	/// </summary>
	/// <param name="_">The expression (unused)</param>
	/// <returns>Always false</returns>
	public static bool operator false(SqlExprBool _) => false;
	
	/// <summary>
	/// Required for short-circuit evaluation. Always returns false to prevent short-circuiting.
	/// </summary>
	/// <param name="_">The expression (unused)</param>
	/// <returns>Always false</returns>
	public static bool operator true(SqlExprBool _) => false;
}

#pragma warning disable CS0660, CS0661
/// <summary>
/// Abstract base class for SQL integer expressions.
/// </summary>
public abstract class SqlExprInt : SqlExpr
{
	/// <summary>
	/// Implicitly converts an integer value to a SqlExprInt.
	/// </summary>
	/// <param name="x">The integer value to convert</param>
	/// <returns>A SqlExprInt representing the integer value</returns>
	public static implicit operator SqlExprInt(int x) => new SqlIntValue(x);

	/// <summary>
	/// Implements the equality operator (==) for integer expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the equality comparison</returns>
	public static SqlExprBool operator ==(SqlExprInt left, SqlExprInt right) => new SqlEquals<SqlExprInt>(left, right);
	
	/// <summary>
	/// Implements the inequality operator (!=) for integer expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the inequality comparison</returns>
	public static SqlExprBool operator !=(SqlExprInt left, SqlExprInt right) => new SqlNotEquals<SqlExprInt>(left, right);

	/// <summary>
	/// Implements the addition operator (+) for integer expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>An integer expression representing the addition</returns>
	public static SqlExprInt operator +(SqlExprInt left, SqlExprInt right) => new SqlIntAdd(left, right);
	
	/// <summary>
	/// Implements the subtraction operator (-) for integer expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>An integer expression representing the subtraction</returns>
	public static SqlExprInt operator -(SqlExprInt left, SqlExprInt right) => new SqlIntSub(left, right);
	
	/// <summary>
	/// Implements the unary negation operator (-) for integer expressions.
	/// </summary>
	/// <param name="value">The operand to negate</param>
	/// <returns>An integer expression representing the negation</returns>
	public static SqlExprInt operator -(SqlExprInt value) => new SqlIntMinus(value);
	
	/// <summary>
	/// Implements the multiplication operator (*) for integer expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>An integer expression representing the multiplication</returns>
	public static SqlExprInt operator *(SqlExprInt left, SqlExprInt right) => new SqlIntMult(left, right);
	
	/// <summary>
	/// Implements the division operator (/) for integer expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>An integer expression representing the division</returns>
	public static SqlExprInt operator /(SqlExprInt left, SqlExprInt right) => new SqlIntDiv(left, right);

	/// <summary>
	/// Implements the greater than operator (&gt;) for integer expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the greater than comparison</returns>
	public static SqlExprBool operator >(SqlExprInt left, SqlExprInt right) => new SqlGreaterThan<SqlExprInt>(left, right);
	
	/// <summary>
	/// Implements the less than operator (&lt;) for integer expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the less than comparison</returns>
	public static SqlExprBool operator <(SqlExprInt left, SqlExprInt right) => new SqlLessThan<SqlExprInt>(left, right);

	/// <summary>
	/// Implements the greater than or equal operator (&gt;=) for integer expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the greater than or equal comparison</returns>
	public static SqlExprBool operator >=(SqlExprInt left, SqlExprInt right) =>
		new SqlGreaterThanOrEqualTo<SqlExprInt>(left, right);

	/// <summary>
	/// Implements the less than or equal operator (&lt;=) for integer expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the less than or equal comparison</returns>
	public static SqlExprBool operator <=(SqlExprInt left, SqlExprInt right) =>
		new SqlLessThanOrEqualTo<SqlExprInt>(left, right);
}

/// <summary>
/// Abstract base class for SQL string expressions.
/// </summary>
#pragma warning disable CS0660, CS0661
public abstract class SqlExprString : SqlExpr
{
	/// <summary>
	/// Implicitly converts a string value to a SqlExprString.
	/// </summary>
	/// <param name="value">The string value to convert</param>
	/// <returns>A SqlExprString representing the string value</returns>
	public static implicit operator SqlExprString(string value) => new SqlStringValue(value);

	/// <summary>
	/// Implements the equality operator (==) for string expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the equality comparison</returns>
	public static SqlExprBool operator ==(SqlExprString left, SqlExprString right) => new SqlEquals<SqlExprString>(left, right);
	
	/// <summary>
	/// Implements the inequality operator (!=) for string expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the inequality comparison</returns>
	public static SqlExprBool operator !=(SqlExprString left, SqlExprString right) => new SqlNotEquals<SqlExprString>(left, right);

	/// <summary>
	/// Implements the greater than operator (&gt;) for string expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the greater than comparison</returns>
	public static SqlExprBool operator >(SqlExprString left, SqlExprString right) => new SqlGreaterThan<SqlExprString>(left, right);
	
	/// <summary>
	/// Implements the less than operator (&lt;) for string expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the less than comparison</returns>
	public static SqlExprBool operator <(SqlExprString left, SqlExprString right) => new SqlLessThan<SqlExprString>(left, right);

	/// <summary>
	/// Implements the greater than or equal operator (&gt;=) for string expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the greater than or equal comparison</returns>
	public static SqlExprBool operator >=(SqlExprString left, SqlExprString right) => new SqlGreaterThanOrEqualTo<SqlExprString>(left, right);
	
	/// <summary>
	/// Implements the less than or equal operator (&lt;=) for string expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the less than or equal comparison</returns>
	public static SqlExprBool operator <=(SqlExprString left, SqlExprString right) => new SqlLessThanOrEqualTo<SqlExprString>(left, right);

	/// <summary>
	/// Implements the concatenation operator (+) for string expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A string expression representing the concatenation</returns>
	public static SqlExprString operator +(SqlExprString left, SqlExprString right) => new SqlStringConcat(left, right);
}

/// <summary>
/// Interface for SQL column expressions.
/// Provides access to table and column name information for all column types.
/// </summary>
public interface ISqlColumn
{ 
	/// <summary>
	/// Gets the name of the table this column belongs to.
	/// </summary>
	public string TableName { get; }
	
	/// <summary>
	/// Gets the name of the column within the table.
	/// </summary>
	public string ColumnName { get; }
}

// Generic comparison base classes for reducing compiler repetition and eliminating type-specific classes

/// <summary>
/// Abstract base class for SQL equality comparisons (=).
/// Handles NULL comparison logic and provides consistent equality semantics across all SQL types.
/// </summary>
internal abstract class SqlEquals(SqlExpr left, SqlExpr right) : SqlExprBool
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Generic implementation of SQL equality comparisons (=).
/// </summary>
internal class SqlEquals<T>(T left, T right) : SqlEquals(left, right) where T : SqlExpr
{
    public void Deconstruct(out T leftOut, out T rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Abstract base class for SQL inequality comparisons (!=).
/// Handles NULL comparison logic and provides consistent inequality semantics across all SQL types.
/// </summary>
internal abstract class SqlNotEquals(SqlExpr left, SqlExpr right) : SqlExprBool
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Generic implementation of SQL inequality comparisons (!=).
/// </summary>
internal class SqlNotEquals<T>(T left, T right) : SqlNotEquals(left, right) where T : SqlExpr
{
    public void Deconstruct(out T leftOut, out T rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Abstract base class for SQL greater than comparisons (>).
/// </summary>
internal abstract class SqlGreaterThan(SqlExpr left, SqlExpr right) : SqlExprBool
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Generic implementation of SQL greater than comparisons (>).
/// </summary>
internal class SqlGreaterThan<T>(T left, T right) : SqlGreaterThan(left, right) where T : SqlExpr
{
    public void Deconstruct(out T leftOut, out T rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Abstract base class for SQL less than comparisons (&lt;).
/// </summary>
internal abstract class SqlLessThan(SqlExpr left, SqlExpr right) : SqlExprBool
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Generic implementation of SQL less than comparisons (&lt;).
/// </summary>
internal class SqlLessThan<T>(T left, T right) : SqlLessThan(left, right) where T : SqlExpr
{
    public void Deconstruct(out T leftOut, out T rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Abstract base class for SQL greater than or equal comparisons (>=).
/// </summary>
internal abstract class SqlGreaterThanOrEqualTo(SqlExpr left, SqlExpr right) : SqlExprBool
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Generic implementation of SQL greater than or equal comparisons (>=).
/// </summary>
internal class SqlGreaterThanOrEqualTo<T>(T left, T right) : SqlGreaterThanOrEqualTo(left, right) where T : SqlExpr
{
    public void Deconstruct(out T leftOut, out T rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Abstract base class for SQL less than or equal comparisons (&lt;=).
/// </summary>
internal abstract class SqlLessThanOrEqualTo(SqlExpr left, SqlExpr right) : SqlExprBool
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Generic implementation of SQL less than or equal comparisons (&lt;=).
/// </summary>
internal class SqlLessThanOrEqualTo<T>(T left, T right) : SqlLessThanOrEqualTo(left, right) where T : SqlExpr
{
    public void Deconstruct(out T leftOut, out T rightOut) => (leftOut, rightOut) = (left, right);
}

