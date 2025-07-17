using System.Runtime.CompilerServices;

namespace TypedSqlBuilder.Core;

/// <summary>
/// Marker interface for all SQL expressions. This is the base type that identifies
/// any object that can represent a SQL expression in the builder system.
/// </summary>
public interface ISqlExpr;

/// <summary>
/// Marker interface for typed SQL expressions. Extends ISqlExpr to provide compile-time
/// type safety by associating each expression with a specific SQL data type.
/// </summary>
/// <typeparam name="TSqlType">The SQL data type this expression represents (e.g., ISqlInt, ISqlString, ISqlBool)</typeparam>
public interface ISqlExpr<TSqlType> : ISqlExpr where TSqlType : ISqlType;

/// <summary>
/// Interface for unary SQL expressions that operate on a single operand.
/// Examples: NOT, unary minus (-), etc.
/// </summary>
/// <typeparam name="TSqlType">The SQL data type of both the operand and result</typeparam>
public interface ISqlUnaryExpr<TSqlType> : ISqlExpr<TSqlType> where TSqlType : ISqlType
{
	ISqlExpr<TSqlType> Value { get; }
}

/// <summary>
/// Interface for binary SQL expressions that operate on two operands.
/// Examples: AND, OR, +, -, =, !=, >, <, etc.
/// </summary>
/// <typeparam name="TSqlType">The SQL data type of both operands and result</typeparam>
public interface ISqlBinExpr<TSqlType> : ISqlExpr<TSqlType> where TSqlType : ISqlType
{
	ISqlExpr<TSqlType> Left { get; }
	ISqlExpr<TSqlType> Right { get; }
}

/// <summary>
/// Marker interface for SQL expressions that represent database columns.
/// This allows the system to distinguish between column references and other expressions.
/// </summary>
public interface ISqlColumnExpr : ISqlExpr;

// Note: All SQL expression base classes use abstract class instead of record
// to enable custom operator overloading that returns SQL expression types rather than primitive types.
// This is essential for maintaining type safety in the fluent SQL expression building system.

#pragma warning disable CS0660, CS0661
/// <summary>
/// Abstract base class for SQL boolean expressions.
/// </summary>
public abstract class SqlExprBool : ISqlExpr<ISqlBool>
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
public abstract class SqlExprInt : ISqlExpr<ISqlInt>
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
public abstract class SqlExprString : ISqlExpr<ISqlString>
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

public class SqlIntColumn(string name) : SqlExprInt, ISqlColumnExpr
{
	public string Name { get; } = name;
}

public class SqlIntProjection(string alias, string name) : SqlExprInt
{
	public string Alias { get; } = alias;
	public string Name { get; } = name;
}
