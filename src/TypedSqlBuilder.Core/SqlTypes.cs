namespace TypedSqlBuilder.Core;

/// <summary>
/// Marker interface for all SQL data types. This is the root type that identifies
/// valid SQL data types in the type system, enabling compile-time type checking.
/// </summary>
public interface ISqlType;

/// <summary>
/// Marker interface representing SQL string/text data types (VARCHAR, NVARCHAR, TEXT, etc.).
/// </summary>
public interface ISqlString : ISqlType;

/// <summary>
/// Marker interface representing SQL integer data types (INT, BIGINT, SMALLINT, etc.).
/// </summary>
public interface ISqlInt : ISqlType;

/// <summary>
/// Marker interface representing SQL boolean data types (BIT, BOOLEAN, etc.).
/// </summary>
public interface ISqlBool : ISqlType;
