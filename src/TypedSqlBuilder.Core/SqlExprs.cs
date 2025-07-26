using System.Runtime.CompilerServices;

namespace TypedSqlBuilder.Core;

// Note: All SQL expression base classes use abstract class instead of record
// to enable custom operator overloading that returns SQL expression types rather than primitive types.
// The problem was that records implement their own ==, != operators which would conflict with our SQL expression operators.
// This is essential for maintaining type safety in the fluent SQL expression building system.

public abstract class SqlExpr;



#pragma warning disable CS0660, CS0661
/// <summary>
/// Abstract base class for SQL boolean expressions.
/// </summary>
public abstract class SqlExprBool : SqlExpr,
	ISqlEqualityOperators<SqlExprBool, SqlExprBool>,
	ISqlLogicalOperators<SqlExprBool>,
	ISqlImplicitConversion<SqlExprBool, bool>
{
	public static implicit operator SqlExprBool(bool value) => new SqlBoolValue(value);

	public static SqlExprBool operator ==(SqlExprBool left, SqlExprBool right) => new SqlBoolEquals(left, right);
	public static SqlExprBool operator !=(SqlExprBool left, SqlExprBool right) => new SqlBoolNotEquals(left, right);

	public static SqlExprBool operator &(SqlExprBool left, SqlExprBool right) => new SqlBoolAnd(left, right);
	public static SqlExprBool operator |(SqlExprBool left, SqlExprBool right) => new SqlBoolOr(left, right);
	public static SqlExprBool operator !(SqlExprBool value) => new SqlBoolNot(value);
	public static bool operator false(SqlExprBool _) => false;
	public static bool operator true(SqlExprBool _) => false;
}

#pragma warning disable CS0660, CS0661
/// <summary>
/// Abstract base class for SQL integer expressions.
/// </summary>
public abstract class SqlExprInt : SqlExpr,
	ISqlEqualityOperators<SqlExprInt, SqlExprBool>,
	ISqlComparisonOperators<SqlExprInt, SqlExprBool>,
	ISqlArithmeticOperators<SqlExprInt>,
	ISqlImplicitConversion<SqlExprInt, int>
{
	public static implicit operator SqlExprInt(int x) => new SqlIntValue(x);

	public static SqlExprBool operator ==(SqlExprInt left, SqlExprInt right) => new SqlIntEquals(left, right);
	public static SqlExprBool operator !=(SqlExprInt left, SqlExprInt right) => new SqlIntNotEquals(left, right);

	public static SqlExprInt operator +(SqlExprInt left, SqlExprInt right) => new SqlIntAdd(left, right);
	public static SqlExprInt operator -(SqlExprInt left, SqlExprInt right) => new SqlIntSub(left, right);
	public static SqlExprInt operator *(SqlExprInt left, SqlExprInt right) => new SqlIntMult(left, right);
	public static SqlExprInt operator /(SqlExprInt left, SqlExprInt right) => new SqlIntDiv(left, right);

	public static SqlExprBool operator >(SqlExprInt left, SqlExprInt right) => new SqlIntGreaterThan(left, right);
	public static SqlExprBool operator <(SqlExprInt left, SqlExprInt right) => new SqlIntLessThan(left, right);

	public static SqlExprBool operator >=(SqlExprInt left, SqlExprInt right) =>
		new SqlIntGreaterThanOrEqualTo(left, right);

	public static SqlExprBool operator <=(SqlExprInt left, SqlExprInt right) =>
		new SqlIntLessThanOrEqualTo(left, right);
}

/// <summary>
/// Abstract base class for SQL string expressions.
/// </summary>
#pragma warning disable CS0660, CS0661
public abstract class SqlExprString : SqlExpr,
	ISqlEqualityOperators<SqlExprString, SqlExprBool>,
	ISqlComparisonOperators<SqlExprString, SqlExprBool>,
	ISqlConcatenationOperators<SqlExprString>,
	ISqlImplicitConversion<SqlExprString, string>
{
	public static implicit operator SqlExprString(string value) => new SqlStringValue(value);
	
	public static SqlExprBool operator ==(SqlExprString left, SqlExprString right) => new SqlStringEquals(left, right);
	public static SqlExprBool operator !=(SqlExprString left, SqlExprString right) => new SqlStringNotEquals(left, right);
	
	public static SqlExprBool operator >(SqlExprString left, SqlExprString right) => new SqlStringGreaterThan(left, right);
	public static SqlExprBool operator <(SqlExprString left, SqlExprString right) => new SqlStringLessThan(left, right);
	
	public static SqlExprBool operator >=(SqlExprString left, SqlExprString right) => new SqlStringGreaterThanOrEqualTo(left, right);
	public static SqlExprBool operator <=(SqlExprString left, SqlExprString right) => new SqlStringLessThanOrEqualTo(left, right);
	
	public static SqlExprString operator +(SqlExprString left, SqlExprString right) => new SqlStringConcat(left, right);
}

public interface ISqlColumn;