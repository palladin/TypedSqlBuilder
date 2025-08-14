using System.Runtime.CompilerServices;

namespace TypedSqlBuilder.Core;

// Note: All SQL expression base classes use abstract class instead of record
// to enable custom operator overloading that returns SQL expression types rather than primitive types.
// The problem was that records implement their own ==, != operators which would conflict with our SQL expression operators.
// This is essential for maintaining type safety in the fluent SQL expression building system.

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
public abstract class SqlExprBool : SqlExpr,
	ISqlEqualityOperators<SqlExprBool, SqlExprBool>,
	ISqlLogicalOperators<SqlExprBool>,
	ISqlImplicitConversion<SqlExprBool, bool>
{
	public static implicit operator SqlExprBool(bool value) => new SqlBoolValue(value);

	public static SqlExprBool operator ==(SqlExprBool left, SqlExprBool right) => new SqlEquals<SqlExprBool>(left, right);
	public static SqlExprBool operator !=(SqlExprBool left, SqlExprBool right) => new SqlNotEquals<SqlExprBool>(left, right);

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

	public static SqlExprBool operator ==(SqlExprInt left, SqlExprInt right) => new SqlEquals<SqlExprInt>(left, right);
	public static SqlExprBool operator !=(SqlExprInt left, SqlExprInt right) => new SqlNotEquals<SqlExprInt>(left, right);

	public static SqlExprInt operator +(SqlExprInt left, SqlExprInt right) => new SqlIntAdd(left, right);
	public static SqlExprInt operator -(SqlExprInt left, SqlExprInt right) => new SqlIntSub(left, right);
	public static SqlExprInt operator -(SqlExprInt value) => new SqlIntMinus(value);
	public static SqlExprInt operator *(SqlExprInt left, SqlExprInt right) => new SqlIntMult(left, right);
	public static SqlExprInt operator /(SqlExprInt left, SqlExprInt right) => new SqlIntDiv(left, right);

	public static SqlExprBool operator >(SqlExprInt left, SqlExprInt right) => new SqlGreaterThan<SqlExprInt>(left, right);
	public static SqlExprBool operator <(SqlExprInt left, SqlExprInt right) => new SqlLessThan<SqlExprInt>(left, right);

	public static SqlExprBool operator >=(SqlExprInt left, SqlExprInt right) =>
		new SqlGreaterThanOrEqualTo<SqlExprInt>(left, right);

	public static SqlExprBool operator <=(SqlExprInt left, SqlExprInt right) =>
		new SqlLessThanOrEqualTo<SqlExprInt>(left, right);
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

	public static SqlExprBool operator ==(SqlExprString left, SqlExprString right) => new SqlEquals<SqlExprString>(left, right);
	public static SqlExprBool operator !=(SqlExprString left, SqlExprString right) => new SqlNotEquals<SqlExprString>(left, right);

	public static SqlExprBool operator >(SqlExprString left, SqlExprString right) => new SqlGreaterThan<SqlExprString>(left, right);
	public static SqlExprBool operator <(SqlExprString left, SqlExprString right) => new SqlLessThan<SqlExprString>(left, right);

	public static SqlExprBool operator >=(SqlExprString left, SqlExprString right) => new SqlGreaterThanOrEqualTo<SqlExprString>(left, right);
	public static SqlExprBool operator <=(SqlExprString left, SqlExprString right) => new SqlLessThanOrEqualTo<SqlExprString>(left, right);

	public static SqlExprString operator +(SqlExprString left, SqlExprString right) => new SqlStringConcat(left, right);
}

public interface ISqlColumn
{ 
	public string TableName { get; }
	public string ColumnName { get; }
}

public interface ISqlColumn<TCol> : ISqlColumn where TCol : ISqlColumn<TCol>
{	
	static abstract TCol Create(string tableName, string columnName);
}

// Generic comparison base classes for reducing compiler repetition and eliminating type-specific classes

/// <summary>
/// Abstract base class for SQL equality comparisons (=).
/// Handles NULL comparison logic and provides consistent equality semantics across all SQL types.
/// </summary>
public abstract class SqlEquals(SqlExpr left, SqlExpr right) : SqlExprBool
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Generic implementation of SQL equality comparisons (=).
/// </summary>
public class SqlEquals<T>(T left, T right) : SqlEquals(left, right) where T : SqlExpr
{
    public void Deconstruct(out T leftOut, out T rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Abstract base class for SQL inequality comparisons (!=).
/// Handles NULL comparison logic and provides consistent inequality semantics across all SQL types.
/// </summary>
public abstract class SqlNotEquals(SqlExpr left, SqlExpr right) : SqlExprBool
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Generic implementation of SQL inequality comparisons (!=).
/// </summary>
public class SqlNotEquals<T>(T left, T right) : SqlNotEquals(left, right) where T : SqlExpr
{
    public void Deconstruct(out T leftOut, out T rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Abstract base class for SQL greater than comparisons (>).
/// </summary>
public abstract class SqlGreaterThan(SqlExpr left, SqlExpr right) : SqlExprBool
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Generic implementation of SQL greater than comparisons (>).
/// </summary>
public class SqlGreaterThan<T>(T left, T right) : SqlGreaterThan(left, right) where T : SqlExpr
{
    public void Deconstruct(out T leftOut, out T rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Abstract base class for SQL less than comparisons (<).
/// </summary>
public abstract class SqlLessThan(SqlExpr left, SqlExpr right) : SqlExprBool
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Generic implementation of SQL less than comparisons (<).
/// </summary>
public class SqlLessThan<T>(T left, T right) : SqlLessThan(left, right) where T : SqlExpr
{
    public void Deconstruct(out T leftOut, out T rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Abstract base class for SQL greater than or equal comparisons (>=).
/// </summary>
public abstract class SqlGreaterThanOrEqualTo(SqlExpr left, SqlExpr right) : SqlExprBool
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Generic implementation of SQL greater than or equal comparisons (>=).
/// </summary>
public class SqlGreaterThanOrEqualTo<T>(T left, T right) : SqlGreaterThanOrEqualTo(left, right) where T : SqlExpr
{
    public void Deconstruct(out T leftOut, out T rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Abstract base class for SQL less than or equal comparisons (<=).
/// </summary>
public abstract class SqlLessThanOrEqualTo(SqlExpr left, SqlExpr right) : SqlExprBool
{
    public void Deconstruct(out SqlExpr leftOut, out SqlExpr rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Generic implementation of SQL less than or equal comparisons (<=).
/// </summary>
public class SqlLessThanOrEqualTo<T>(T left, T right) : SqlLessThanOrEqualTo(left, right) where T : SqlExpr
{
    public void Deconstruct(out T leftOut, out T rightOut) => (leftOut, rightOut) = (left, right);
}

