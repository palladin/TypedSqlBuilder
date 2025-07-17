namespace TypedSqlBuilder.Core;

// Integer expression implementations
public class SqlIntValue(int value) : SqlExprInt
{
	public int Value { get; } = value;
}

public class SqlIntPlus(ISqlExpr<ISqlInt> value) : SqlExprInt, ISqlUnaryExpr<ISqlInt>
{
	public ISqlExpr<ISqlInt> Value { get; } = value;
}

public class SqlIntMinus(ISqlExpr<ISqlInt> value) : SqlExprInt, ISqlUnaryExpr<ISqlInt>
{
	public ISqlExpr<ISqlInt> Value { get; } = value;
}

public class SqlIntAbs(ISqlExpr<ISqlInt> value) : SqlExprInt, ISqlUnaryExpr<ISqlInt>
{
	public ISqlExpr<ISqlInt> Value { get; } = value;
}

public class SqlIntAdd(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprInt, ISqlBinExpr<ISqlInt>
{
	public ISqlExpr<ISqlInt> Left { get; } = left;
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

public class SqlIntSub(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprInt, ISqlBinExpr<ISqlInt>
{
	public ISqlExpr<ISqlInt> Left { get; } = left;
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

public class SqlIntMult(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprInt, ISqlBinExpr<ISqlInt>
{
	public ISqlExpr<ISqlInt> Left { get; } = left;
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

public class SqlIntDiv(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprInt, ISqlBinExpr<ISqlInt>
{
	public ISqlExpr<ISqlInt> Left { get; } = left;
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

public class SqlIntGreaterThan(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprBool, ISqlBinExpr<ISqlInt>
{
	public ISqlExpr<ISqlInt> Left { get; } = left;
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

public class SqlIntLessThan(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprBool, ISqlBinExpr<ISqlInt>
{
	public ISqlExpr<ISqlInt> Left { get; } = left;
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

public class SqlIntGreaterThanOrEqualTo(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprBool, ISqlBinExpr<ISqlInt>
{
	public ISqlExpr<ISqlInt> Left { get; } = left;
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

public class SqlIntLessThanOrEqualTo(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprBool, ISqlBinExpr<ISqlInt>
{
	public ISqlExpr<ISqlInt> Left { get; } = left;
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

public class SqlIntEquals(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprBool, ISqlBinExpr<ISqlInt>
{
	public ISqlExpr<ISqlInt> Left { get; } = left;
	public ISqlExpr<ISqlInt> Right { get; } = right;
}

public class SqlIntNotEquals(ISqlExpr<ISqlInt> left, ISqlExpr<ISqlInt> right) : SqlExprBool, ISqlBinExpr<ISqlInt>
{
	public ISqlExpr<ISqlInt> Left { get; } = left;
	public ISqlExpr<ISqlInt> Right { get; } = right;
}
