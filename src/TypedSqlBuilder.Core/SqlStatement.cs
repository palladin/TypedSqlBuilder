using System.Collections.Immutable;

namespace TypedSqlBuilder.Core;

/// <summary>
/// Base interface for all SQL statement representations.
/// Provides the foundation for building type-safe SQL statements that don't return data.
/// </summary>
public interface ISqlStatement;

/// <summary>
/// Generic interface for SQL statements that operate on a specific table type.
/// Enables strongly-typed statement composition and transformation.
/// </summary>
/// <typeparam name="TTable">The type of table this statement operates on</typeparam>
public interface ISqlStatement<TTable> : ISqlStatement
    where TTable : ISqlTable;

// ========== DELETE INTERFACES AND IMPLEMENTATIONS ==========

/// <summary>
/// Base interface for SQL DELETE statements (non-generic).
/// </summary>
public interface ISqlDeleteStatement : ISqlStatement;

/// <summary>
/// Interface for SQL DELETE statements.
/// </summary>
/// <typeparam name="TSqlTable">The table type being deleted from</typeparam>
public interface ISqlDeleteStatement<TSqlTable> : ISqlDeleteStatement, ISqlStatement<TSqlTable>
    where TSqlTable : ISqlTable;

/// <summary>
/// Base interface for SQL DELETE statements with WHERE clause (non-generic).
/// </summary>
public interface ISqlDeleteWhereStatement : ISqlStatement;

/// <summary>
/// Interface for SQL DELETE statements with WHERE clause.
/// </summary>
/// <typeparam name="TSqlTable">The table type being deleted from</typeparam>
public interface ISqlDeleteWhereStatement<TSqlTable> : ISqlDeleteWhereStatement, ISqlStatement<TSqlTable>
    where TSqlTable : ISqlTable;

// ========== UPDATE INTERFACES AND IMPLEMENTATIONS ==========

/// <summary>
/// Base interface for SQL UPDATE statements (non-generic).
/// </summary>
public interface ISqlUpdateStatement : ISqlStatement;

/// <summary>
/// Interface for SQL UPDATE statements.
/// </summary>
/// <typeparam name="TSqlTable">The table type being updated</typeparam>
public interface ISqlUpdateStatement<TSqlTable> : ISqlUpdateStatement, ISqlStatement<TSqlTable>
    where TSqlTable : ISqlTable;

/// <summary>
/// Base interface for SQL UPDATE statements with WHERE clause (non-generic).
/// </summary>
public interface ISqlUpdateWhereStatement : ISqlStatement;

/// <summary>
/// Interface for SQL UPDATE statements with WHERE clause.
/// </summary>
/// <typeparam name="TSqlTable">The table type being updated</typeparam>
public interface ISqlUpdateWhereStatement<TSqlTable> : ISqlUpdateWhereStatement, ISqlStatement<TSqlTable>
    where TSqlTable : ISqlTable;

/// <summary>
/// Represents a SET clause in an UPDATE statement.
/// </summary>
internal abstract record SetClause(Func<ISqlTable, SqlExpr> ColumnSelector, SqlExpr Value);

/// <summary>
/// SET clause for integer columns.
/// </summary>
internal record SetIntClause(Func<ISqlTable, SqlExprInt> IntColumnSelector, SqlExprInt IntValue) : SetClause(table => IntColumnSelector(table), IntValue);

/// <summary>
/// SET clause for string columns.
/// </summary>
internal record SetStringClause(Func<ISqlTable, SqlExprString> StringColumnSelector, SqlExprString StringValue) : SetClause(table => StringColumnSelector(table), StringValue);

/// <summary>
/// SET clause for boolean columns.
/// </summary>
internal record SetBoolClause(Func<ISqlTable, SqlExprBool> BoolColumnSelector, SqlExprBool BoolValue) : SetClause(table => BoolColumnSelector(table), BoolValue);

/// <summary>
/// Represents a VALUE clause in an INSERT statement.
/// </summary>
internal abstract record ValueClause(Func<ISqlTable, SqlExpr> ColumnSelector, SqlExpr Value);

/// <summary>
/// VALUE clause for integer columns.
/// </summary>
internal record InsertIntClause(Func<ISqlTable, SqlExprInt> IntColumnSelector, SqlExprInt IntValue) : ValueClause(table => IntColumnSelector(table), IntValue);

/// <summary>
/// VALUE clause for string columns.
/// </summary>
internal record InsertStringClause(Func<ISqlTable, SqlExprString> StringColumnSelector, SqlExprString StringValue) : ValueClause(table => StringColumnSelector(table), StringValue);

/// <summary>
/// VALUE clause for boolean columns.
/// </summary>
internal record InsertBoolClause(Func<ISqlTable, SqlExprBool> BoolColumnSelector, SqlExprBool BoolValue) : ValueClause(table => BoolColumnSelector(table), BoolValue);

/// <summary>
/// Base record representing a SQL DELETE statement.
/// </summary>
/// <param name="Table">The table being deleted from</param>
internal abstract record DeleteStatement(ISqlTable Table) : ISqlStatement;

/// <summary>
/// Base record representing a SQL DELETE statement with WHERE clause.
/// </summary>
/// <param name="Statement">The DELETE statement being extended</param>
/// <param name="Predicate">Function that evaluates filtering conditions</param>
internal abstract record DeleteWhereStatement(ISqlDeleteStatement Statement, Func<ISqlTable, SqlExprBool> Predicate) : ISqlStatement;

/// <summary>
/// Base record representing a SET operation in an UPDATE statement.
/// </summary>
/// <param name="Statement">The UPDATE statement being extended</param>
/// <param name="SetClause">The SET clause being added</param>
internal abstract record SetStatement(ISqlUpdateStatement Statement, SetClause SetClause) : ISqlStatement;

/// <summary>
/// Base record representing a VALUE operation in an INSERT statement.
/// </summary>
/// <param name="Statement">The INSERT statement being extended</param>
/// <param name="ValueClause">The VALUE clause being added</param>
internal abstract record ValueStatement(ISqlInsertStatement Statement, ValueClause ValueClause) : ISqlStatement;

/// <summary>
/// Base record representing a WHERE operation in an UPDATE statement.
/// </summary>
/// <param name="Statement">The UPDATE statement being extended</param>
/// <param name="Predicate">Function that evaluates filtering conditions</param>
internal abstract record UpdateWhereFromStatement(ISqlUpdateStatement Statement, Func<ISqlTable, SqlExprBool> Predicate) : ISqlStatement;

/// <summary>
/// Base record representing a SQL UPDATE statement.
/// </summary>
/// <param name="Table">The table being updated</param>
internal abstract record UpdateStatement(ISqlTable Table) : ISqlStatement;


/// <summary>
/// Base record representing a SQL INSERT statement.
/// </summary>
/// <param name="Table">The table being inserted into</param>
internal abstract record InsertStatement(ISqlTable Table) : ISqlStatement;

// ========== INSERT INTERFACES AND IMPLEMENTATIONS ==========

/// <summary>
/// Base interface for SQL INSERT statements (non-generic).
/// </summary>
public interface ISqlInsertStatement : ISqlStatement;

/// <summary>
/// Interface for SQL INSERT statements.
/// </summary>
/// <typeparam name="TSqlTable">The table type being inserted into</typeparam>
public interface ISqlInsertStatement<TSqlTable> : ISqlInsertStatement, ISqlStatement<TSqlTable>
    where TSqlTable : ISqlTable;

/// <summary>
/// Implementation of a SQL DELETE statement.
/// </summary>
/// <typeparam name="TSqlTable">The table type being deleted from</typeparam>
internal record DeleteStatement<TSqlTable>(TSqlTable table) : DeleteStatement(table), ISqlDeleteStatement<TSqlTable>
    where TSqlTable : ISqlTable;

/// <summary>
/// Implementation of a SQL DELETE statement with WHERE clause.
/// </summary>
/// <typeparam name="TSqlTable">The table type being deleted from</typeparam>
internal record DeleteWhereStatement<TSqlTable>(ISqlDeleteStatement<TSqlTable> statement, Func<TSqlTable, SqlExprBool> predicate) : DeleteWhereStatement(statement, t => predicate((TSqlTable)t)), ISqlDeleteWhereStatement<TSqlTable>
    where TSqlTable : ISqlTable;

/// <summary>
/// Implementation of a SET operation in an UPDATE statement.
/// </summary>
/// <typeparam name="TSqlTable">The table type being updated</typeparam>
internal record SetStatement<TSqlTable>(ISqlUpdateStatement<TSqlTable> statement, SetClause setClause) : SetStatement(statement, setClause), ISqlUpdateStatement<TSqlTable>
    where TSqlTable : ISqlTable;

/// <summary>
/// Implementation of a VALUE operation in an INSERT statement.
/// </summary>
/// <typeparam name="TSqlTable">The table type being inserted into</typeparam>
internal record ValueStatement<TSqlTable>(ISqlInsertStatement<TSqlTable> statement, ValueClause valueClause) : ValueStatement(statement, valueClause), ISqlInsertStatement<TSqlTable>
    where TSqlTable : ISqlTable;

/// <summary>
/// Implementation of a WHERE operation in an UPDATE statement.
/// </summary>
/// <typeparam name="TSqlTable">The table type being updated</typeparam>
internal record UpdateWhereFromStatement<TSqlTable>(ISqlUpdateStatement<TSqlTable> statement, Func<TSqlTable, SqlExprBool> predicate) : UpdateWhereFromStatement(statement, t => predicate((TSqlTable)t)), ISqlUpdateWhereStatement<TSqlTable>
    where TSqlTable : ISqlTable;

/// <summary>
/// Implementation of a SQL UPDATE statement.
/// </summary>
/// <typeparam name="TSqlTable">The table type being updated</typeparam>
internal record UpdateStatement<TSqlTable>(TSqlTable table) : UpdateStatement(table), ISqlUpdateStatement<TSqlTable>
    where TSqlTable : ISqlTable;

/// <summary>
/// Implementation of a SQL INSERT statement.
/// </summary>
/// <typeparam name="TSqlTable">The table type being inserted into</typeparam>
internal record InsertStatement<TSqlTable>(TSqlTable table) : InsertStatement(table), ISqlInsertStatement<TSqlTable>
    where TSqlTable : ISqlTable;