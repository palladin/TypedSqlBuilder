using System.Runtime.CompilerServices;

namespace TypedSqlBuilder.Core;

public interface ISqlExpr
{
}

public interface ISqlExpr<TSqlType> : ISqlExpr where TSqlType : ISqlType
{
}

public interface ISqlUnaryExpr<TSqlType> : ISqlExpr<TSqlType> where TSqlType : ISqlType
{
	ISqlExpr<TSqlType> Value { get; }
}

public interface ISqlBinExpr<TSqlType> : ISqlExpr<TSqlType> where TSqlType : ISqlType
{
	ISqlExpr<TSqlType> Left { get; }
	ISqlExpr<TSqlType> Right { get; }
}

public interface ISqlColumnExpr : ISqlExpr { }

public abstract class SqlExprBool : ISqlExpr<ISqlBool>
{
	public static implicit operator SqlExprBool(bool value) => new SqlBoolValue(value);
	public static SqlExprBool operator &(SqlExprBool left, SqlExprBool right) => new SqlBoolAnd(left, right);
	public static SqlExprBool operator |(SqlExprBool left, SqlExprBool right) => new SqlBoolOr(left, right);
	public static SqlExprBool operator !(SqlExprBool value) => new SqlBoolNot(value);
	public static bool operator false(SqlExprBool _) => false;
	public static bool operator true(SqlExprBool _) => false;
}

#pragma warning disable CS0660, CS0661
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

public abstract class SqlExprString : ISqlExpr<ISqlString>
{
	public static implicit operator SqlExprString(string value) => new SqlStringValue(value);
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
