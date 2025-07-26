namespace TypedSqlBuilder.Core;

/// <summary>
/// Interface for SQL expressions that support equality operations (== and !=).
/// </summary>
/// <typeparam name="TSelf">The implementing type</typeparam>
/// <typeparam name="TResult">The result type of equality operations (typically SqlExprBool)</typeparam>
public interface ISqlEqualityOperators<TSelf, TResult>
    where TSelf : ISqlEqualityOperators<TSelf, TResult>
{
    static abstract TResult operator ==(TSelf left, TSelf right);
    static abstract TResult operator !=(TSelf left, TSelf right);
}

/// <summary>
/// Interface for SQL expressions that support comparison operations (<, >, <=, >=).
/// </summary>
/// <typeparam name="TSelf">The implementing type</typeparam>
/// <typeparam name="TResult">The result type of comparison operations (typically SqlExprBool)</typeparam>
public interface ISqlComparisonOperators<TSelf, TResult>
    where TSelf : ISqlComparisonOperators<TSelf, TResult>
{
    static abstract TResult operator <(TSelf left, TSelf right);
    static abstract TResult operator >(TSelf left, TSelf right);
    static abstract TResult operator <=(TSelf left, TSelf right);
    static abstract TResult operator >=(TSelf left, TSelf right);
}

/// <summary>
/// Interface for SQL expressions that support arithmetic operations (+, -, *, /).
/// </summary>
/// <typeparam name="TSelf">The implementing type</typeparam>
public interface ISqlArithmeticOperators<TSelf>
    where TSelf : ISqlArithmeticOperators<TSelf>
{
    static abstract TSelf operator +(TSelf left, TSelf right);
    static abstract TSelf operator -(TSelf left, TSelf right);
    static abstract TSelf operator *(TSelf left, TSelf right);
    static abstract TSelf operator /(TSelf left, TSelf right);
    static abstract TSelf operator -(TSelf value);
}

/// <summary>
/// Interface for SQL expressions that support logical operations (&, |, !).
/// </summary>
/// <typeparam name="TSelf">The implementing type</typeparam>
public interface ISqlLogicalOperators<TSelf>
    where TSelf : ISqlLogicalOperators<TSelf>
{
    static abstract TSelf operator &(TSelf left, TSelf right);
    static abstract TSelf operator |(TSelf left, TSelf right);
    static abstract TSelf operator !(TSelf value);
    static abstract bool operator false(TSelf value);
    static abstract bool operator true(TSelf value);
}

/// <summary>
/// Interface for SQL expressions that support string concatenation.
/// </summary>
/// <typeparam name="TSelf">The implementing type</typeparam>
public interface ISqlConcatenationOperators<TSelf>
    where TSelf : ISqlConcatenationOperators<TSelf>
{
    static abstract TSelf operator +(TSelf left, TSelf right);
}

/// <summary>
/// Interface for SQL expressions that support implicit conversion from a primitive type.
/// </summary>
/// <typeparam name="TSelf">The implementing SQL expression type</typeparam>
/// <typeparam name="TValue">The primitive type to convert from</typeparam>
public interface ISqlImplicitConversion<TSelf, TValue>
    where TSelf : ISqlImplicitConversion<TSelf, TValue>
{
    static abstract implicit operator TSelf(TValue value);
}
