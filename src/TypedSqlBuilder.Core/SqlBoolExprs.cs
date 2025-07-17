namespace TypedSqlBuilder.Core;

public class SqlBoolValue(bool value) : SqlExprBool
{
	public bool Value { get; } = value;
}

public class SqlBoolNot(ISqlExpr<ISqlBool> value) : SqlExprBool, ISqlUnaryExpr<ISqlBool>
{
	public ISqlExpr<ISqlBool> Value { get; } = value;
}

public class SqlBoolAnd(ISqlExpr<ISqlBool> left, ISqlExpr<ISqlBool> right) : SqlExprBool, ISqlBinExpr<ISqlBool>
{
	public ISqlExpr<ISqlBool> Left { get; } = left;
	public ISqlExpr<ISqlBool> Right { get; } = right;
}

public class SqlBoolOr(ISqlExpr<ISqlBool> left, ISqlExpr<ISqlBool> right) : SqlExprBool, ISqlBinExpr<ISqlBool>
{
	public ISqlExpr<ISqlBool> Left { get; } = left;
	public ISqlExpr<ISqlBool> Right { get; } = right;
}
