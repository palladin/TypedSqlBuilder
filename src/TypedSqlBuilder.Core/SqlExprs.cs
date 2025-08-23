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

#pragma warning disable CS0660, CS0661
/// <summary>
/// Abstract base class for SQL long (bigint) expressions.
/// </summary>
public abstract class SqlExprLong : SqlExpr
{
	/// <summary>
	/// Implicitly converts a long value to a SqlExprLong.
	/// </summary>
	/// <param name="x">The long value to convert</param>
	/// <returns>A SqlExprLong representing the long value</returns>
	public static implicit operator SqlExprLong(long x) => new SqlLongValue(x);

	/// <summary>
	/// Implicitly converts an integer expression to a long expression.
	/// This enables seamless integration between integer and long expressions in SQL operations.
	/// </summary>
	/// <param name="x">The integer expression to convert</param>
	/// <returns>A SqlExprLong representing the integer expression as a long</returns>
	public static implicit operator SqlExprLong(SqlExprInt x) => new SqlLongImplicit(x);

	/// <summary>
	/// Implements the equality operator (==) for long expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the equality comparison</returns>
	public static SqlExprBool operator ==(SqlExprLong left, SqlExprLong right) => new SqlEquals<SqlExprLong>(left, right);
	
	/// <summary>
	/// Implements the inequality operator (!=) for long expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the inequality comparison</returns>
	public static SqlExprBool operator !=(SqlExprLong left, SqlExprLong right) => new SqlNotEquals<SqlExprLong>(left, right);

	/// <summary>
	/// Implements the addition operator (+) for long expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A long expression representing the addition</returns>
	public static SqlExprLong operator +(SqlExprLong left, SqlExprLong right) => new SqlLongAdd(left, right);
	
	/// <summary>
	/// Implements the subtraction operator (-) for long expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A long expression representing the subtraction</returns>
	public static SqlExprLong operator -(SqlExprLong left, SqlExprLong right) => new SqlLongSub(left, right);
	
	/// <summary>
	/// Implements the unary negation operator (-) for long expressions.
	/// </summary>
	/// <param name="value">The operand to negate</param>
	/// <returns>A long expression representing the negation</returns>
	public static SqlExprLong operator -(SqlExprLong value) => new SqlLongMinus(value);
	
	/// <summary>
	/// Implements the multiplication operator (*) for long expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A long expression representing the multiplication</returns>
	public static SqlExprLong operator *(SqlExprLong left, SqlExprLong right) => new SqlLongMult(left, right);
	
	/// <summary>
	/// Implements the division operator (/) for long expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A long expression representing the division</returns>
	public static SqlExprLong operator /(SqlExprLong left, SqlExprLong right) => new SqlLongDiv(left, right);

	/// <summary>
	/// Implements the greater than operator (&gt;) for long expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the greater than comparison</returns>
	public static SqlExprBool operator >(SqlExprLong left, SqlExprLong right) => new SqlGreaterThan<SqlExprLong>(left, right);
	
	/// <summary>
	/// Implements the less than operator (&lt;) for long expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the less than comparison</returns>
	public static SqlExprBool operator <(SqlExprLong left, SqlExprLong right) => new SqlLessThan<SqlExprLong>(left, right);

	/// <summary>
	/// Implements the greater than or equal operator (&gt;=) for long expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the greater than or equal comparison</returns>
	public static SqlExprBool operator >=(SqlExprLong left, SqlExprLong right) =>
		new SqlGreaterThanOrEqualTo<SqlExprLong>(left, right);

	/// <summary>
	/// Implements the less than or equal operator (&lt;=) for long expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the less than or equal comparison</returns>
	public static SqlExprBool operator <=(SqlExprLong left, SqlExprLong right) =>
		new SqlLessThanOrEqualTo<SqlExprLong>(left, right);
}

#pragma warning disable CS0660, CS0661
/// <summary>
/// Abstract base class for SQL double expressions.
/// </summary>
public abstract class SqlExprDouble : SqlExpr
{
	/// <summary>
	/// Implicitly converts a double value to a SqlExprDouble.
	/// </summary>
	/// <param name="x">The double value to convert</param>
	/// <returns>A SqlExprDouble representing the double value</returns>
	public static implicit operator SqlExprDouble(double x) => new SqlDoubleValue(x);

	/// <summary>
	/// Implements the equality operator (==) for double expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the equality comparison</returns>
	public static SqlExprBool operator ==(SqlExprDouble left, SqlExprDouble right) => new SqlEquals<SqlExprDouble>(left, right);
	
	/// <summary>
	/// Implements the inequality operator (!=) for double expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the inequality comparison</returns>
	public static SqlExprBool operator !=(SqlExprDouble left, SqlExprDouble right) => new SqlNotEquals<SqlExprDouble>(left, right);

	/// <summary>
	/// Implements the addition operator (+) for double expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A double expression representing the addition</returns>
	public static SqlExprDouble operator +(SqlExprDouble left, SqlExprDouble right) => new SqlDoubleAdd(left, right);
	
	/// <summary>
	/// Implements the subtraction operator (-) for double expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A double expression representing the subtraction</returns>
	public static SqlExprDouble operator -(SqlExprDouble left, SqlExprDouble right) => new SqlDoubleSub(left, right);
	
	/// <summary>
	/// Implements the unary negation operator (-) for double expressions.
	/// </summary>
	/// <param name="value">The operand to negate</param>
	/// <returns>A double expression representing the negation</returns>
	public static SqlExprDouble operator -(SqlExprDouble value) => new SqlDoubleMinus(value);
	
	/// <summary>
	/// Implements the multiplication operator (*) for double expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A double expression representing the multiplication</returns>
	public static SqlExprDouble operator *(SqlExprDouble left, SqlExprDouble right) => new SqlDoubleMult(left, right);
	
	/// <summary>
	/// Implements the division operator (/) for double expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A double expression representing the division</returns>
	public static SqlExprDouble operator /(SqlExprDouble left, SqlExprDouble right) => new SqlDoubleDiv(left, right);

	/// <summary>
	/// Implements the greater than operator (&gt;) for double expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the greater than comparison</returns>
	public static SqlExprBool operator >(SqlExprDouble left, SqlExprDouble right) => new SqlGreaterThan<SqlExprDouble>(left, right);
	
	/// <summary>
	/// Implements the less than operator (&lt;) for double expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the less than comparison</returns>
	public static SqlExprBool operator <(SqlExprDouble left, SqlExprDouble right) => new SqlLessThan<SqlExprDouble>(left, right);

	/// <summary>
	/// Implements the greater than or equal operator (&gt;=) for double expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the greater than or equal comparison</returns>
	public static SqlExprBool operator >=(SqlExprDouble left, SqlExprDouble right) =>
		new SqlGreaterThanOrEqualTo<SqlExprDouble>(left, right);

	/// <summary>
	/// Implements the less than or equal operator (&lt;=) for double expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the less than or equal comparison</returns>
	public static SqlExprBool operator <=(SqlExprDouble left, SqlExprDouble right) =>
		new SqlLessThanOrEqualTo<SqlExprDouble>(left, right);
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
/// Abstract base class for SQL decimal expressions.
/// </summary>
#pragma warning disable CS0660, CS0661
public abstract class SqlExprDecimal : SqlExpr
{
	/// <summary>
	/// Implicitly converts a decimal value to a SqlExprDecimal.
	/// </summary>
	/// <param name="value">The decimal value to convert</param>
	/// <returns>A SqlExprDecimal representing the decimal value</returns>
	public static implicit operator SqlExprDecimal(decimal value) => new SqlDecimalValue(value);

	/// <summary>
	/// Implicitly converts an integer expression to a decimal expression.
	/// This enables seamless integration between integer and decimal expressions in SQL operations.
	/// </summary>
	/// <param name="value">The integer expression to convert</param>
	/// <returns>A SqlExprDecimal representing the integer expression as a decimal</returns>
	public static implicit operator SqlExprDecimal(SqlExprInt value) => new SqlDecimalImplicit(value);
	
	/// <summary>
	/// Implicitly converts a long expression to a decimal expression.
	/// This enables seamless integration between long and decimal expressions in SQL operations.
	/// </summary>
	/// <param name="value">The long expression to convert</param>
	/// <returns>A SqlExprDecimal representing the long expression as a decimal</returns>
	public static implicit operator SqlExprDecimal(SqlExprLong value) => new SqlDecimalImplicit(value);
	
	/// <summary>
	/// Implicitly converts an integer value to a decimal expression.
	/// The integer is first wrapped in a SqlIntValue, then converted to decimal.
	/// </summary>
	/// <param name="value">The integer value to convert</param>
	/// <returns>A SqlExprDecimal representing the integer value as a decimal</returns>
	public static implicit operator SqlExprDecimal(int value) => new SqlDecimalImplicit(new SqlIntValue(value));
	
	/// <summary>
	/// Implicitly converts a long value to a decimal expression.
	/// The long is first wrapped in a SqlLongValue, then converted to decimal.
	/// </summary>
	/// <param name="value">The long value to convert</param>
	/// <returns>A SqlExprDecimal representing the long value as a decimal</returns>
	public static implicit operator SqlExprDecimal(long value) => new SqlDecimalImplicit(new SqlLongValue(value));

	/// <summary>
	/// Implements the equality operator (==) for decimal expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the equality comparison</returns>
	public static SqlExprBool operator ==(SqlExprDecimal left, SqlExprDecimal right) => new SqlEquals<SqlExprDecimal>(left, right);
	
	/// <summary>
	/// Implements the inequality operator (!=) for decimal expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the inequality comparison</returns>
	public static SqlExprBool operator !=(SqlExprDecimal left, SqlExprDecimal right) => new SqlNotEquals<SqlExprDecimal>(left, right);

	/// <summary>
	/// Implements the addition operator (+) for decimal expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A decimal expression representing the addition</returns>
	public static SqlExprDecimal operator +(SqlExprDecimal left, SqlExprDecimal right) => new SqlDecimalAdd(left, right);

	/// <summary>
	/// Implements the subtraction operator (-) for decimal expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A decimal expression representing the subtraction</returns>
	public static SqlExprDecimal operator -(SqlExprDecimal left, SqlExprDecimal right) => new SqlDecimalSub(left, right);
	
	/// <summary>
	/// Implements the unary negation operator (-) for decimal expressions.
	/// </summary>
	/// <param name="value">The operand to negate</param>
	/// <returns>A decimal expression representing the negation</returns>
	public static SqlExprDecimal operator -(SqlExprDecimal value) => new SqlDecimalMinus(value);
	
	
	/// <summary>
	/// Implements the multiplication operator (*) for decimal expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A decimal expression representing the multiplication</returns>
	public static SqlExprDecimal operator *(SqlExprDecimal left, SqlExprDecimal right) => new SqlDecimalMult(left, right);
	
	
	/// <summary>
	/// Implements the division operator (/) for decimal expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A decimal expression representing the division</returns>
	public static SqlExprDecimal operator /(SqlExprDecimal left, SqlExprDecimal right) => new SqlDecimalDiv(left, right);
	
	/// <summary>
	/// Implements the greater than operator (>) for decimal expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the greater than comparison</returns>
	public static SqlExprBool operator >(SqlExprDecimal left, SqlExprDecimal right) => new SqlGreaterThan<SqlExprDecimal>(left, right);
	
	/// <summary>
	/// Implements the less than operator (&lt;) for decimal expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the less than comparison</returns>
	public static SqlExprBool operator <(SqlExprDecimal left, SqlExprDecimal right) => new SqlLessThan<SqlExprDecimal>(left, right);

	/// <summary>
	/// Implements the greater than or equal operator (>=) for decimal expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the greater than or equal comparison</returns>
	public static SqlExprBool operator >=(SqlExprDecimal left, SqlExprDecimal right) =>
		new SqlGreaterThanOrEqualTo<SqlExprDecimal>(left, right);

	/// <summary>
	/// Implements the less than or equal operator (&lt;=) for decimal expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the less than or equal comparison</returns>
	public static SqlExprBool operator <=(SqlExprDecimal left, SqlExprDecimal right) =>
		new SqlLessThanOrEqualTo<SqlExprDecimal>(left, right);
}

/// <summary>
/// Abstract base class for SQL DateTime expressions.
/// </summary>
#pragma warning disable CS0660, CS0661
public abstract class SqlExprDateTime : SqlExpr
{
	/// <summary>
	/// Implicitly converts a DateTime value to a SqlExprDateTime.
	/// </summary>
	/// <param name="value">The DateTime value to convert</param>
	/// <returns>A SqlExprDateTime representing the DateTime value</returns>
	public static implicit operator SqlExprDateTime(DateTime value) => new SqlDateTimeValue(value);

	/// <summary>
	/// Implements the equality operator (==) for DateTime expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the equality comparison</returns>
	public static SqlExprBool operator ==(SqlExprDateTime left, SqlExprDateTime right) => new SqlEquals<SqlExprDateTime>(left, right);
	
	/// <summary>
	/// Implements the inequality operator (!=) for DateTime expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the inequality comparison</returns>
	public static SqlExprBool operator !=(SqlExprDateTime left, SqlExprDateTime right) => new SqlNotEquals<SqlExprDateTime>(left, right);

	/// <summary>
	/// Implements the greater than operator (>) for DateTime expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the greater than comparison</returns>
	public static SqlExprBool operator >(SqlExprDateTime left, SqlExprDateTime right) => new SqlGreaterThan<SqlExprDateTime>(left, right);
	
	/// <summary>
	/// Implements the less than operator (&lt;) for DateTime expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the less than comparison</returns>
	public static SqlExprBool operator <(SqlExprDateTime left, SqlExprDateTime right) => new SqlLessThan<SqlExprDateTime>(left, right);

	/// <summary>
	/// Implements the greater than or equal operator (>=) for DateTime expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the greater than or equal comparison</returns>
	public static SqlExprBool operator >=(SqlExprDateTime left, SqlExprDateTime right) =>
		new SqlGreaterThanOrEqualTo<SqlExprDateTime>(left, right);

	/// <summary>
	/// Implements the less than or equal operator (&lt;=) for DateTime expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the less than or equal comparison</returns>
	public static SqlExprBool operator <=(SqlExprDateTime left, SqlExprDateTime right) =>
		new SqlLessThanOrEqualTo<SqlExprDateTime>(left, right);
}

/// <summary>
/// Abstract base class for SQL GUID expressions.
/// </summary>
#pragma warning disable CS0660, CS0661
public abstract class SqlExprGuid : SqlExpr
{
	/// <summary>
	/// Implicitly converts a Guid value to a SqlExprGuid.
	/// </summary>
	/// <param name="value">The Guid value to convert</param>
	/// <returns>A SqlExprGuid representing the Guid value</returns>
	public static implicit operator SqlExprGuid(Guid value) => new SqlGuidValue(value);

	/// <summary>
	/// Implements the equality operator (==) for GUID expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the equality comparison</returns>
	public static SqlExprBool operator ==(SqlExprGuid left, SqlExprGuid right) => new SqlEquals<SqlExprGuid>(left, right);
	
	/// <summary>
	/// Implements the inequality operator (!=) for GUID expressions.
	/// </summary>
	/// <param name="left">The left operand</param>
	/// <param name="right">The right operand</param>
	/// <returns>A boolean expression representing the inequality comparison</returns>
	public static SqlExprBool operator !=(SqlExprGuid left, SqlExprGuid right) => new SqlNotEquals<SqlExprGuid>(left, right);
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

