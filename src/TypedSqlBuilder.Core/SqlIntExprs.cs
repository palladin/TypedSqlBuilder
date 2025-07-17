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


public class SqlIntProjection(string alias, string name) : SqlExprInt, ISqlProjectionExpr<ISqlInt>
{
	public string Source { get; } = alias;
	public string Name { get; } = name;
}

/// <summary>
/// Represents a reference to an integer column in a SQL table.
/// Inherits from SqlIntProjection to support column references in SQL queries.
/// </summary>
/// <param name="source">The table name or alias that contains the column</param>
/// <param name="name">The name of the column</param>
public class SqlIntColumn(string source, string name) : SqlIntProjection(source, name);