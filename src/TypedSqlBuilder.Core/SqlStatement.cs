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
public interface ISqlDeleteStatement : ISqlStatement
{
    ISqlTable Table { get; }
}

/// <summary>
/// Interface for SQL DELETE statements.
/// </summary>
/// <typeparam name="TSqlTable">The table type being deleted from</typeparam>
public interface ISqlDeleteStatement<TSqlTable> : ISqlDeleteStatement, ISqlStatement<TSqlTable>
    where TSqlTable : ISqlTable
{
    new TSqlTable Table { get; }
}

/// <summary>
/// Base interface for SQL DELETE statements with WHERE clause (non-generic).
/// </summary>
public interface ISqlDeleteWhereStatement : ISqlStatement
{
    ISqlTable Table { get; }
    Func<ISqlTable, SqlExprBool> Predicate { get; }
}

/// <summary>
/// Interface for SQL DELETE statements with WHERE clause.
/// </summary>
/// <typeparam name="TSqlTable">The table type being deleted from</typeparam>
public interface ISqlDeleteWhereStatement<TSqlTable> : ISqlDeleteWhereStatement, ISqlStatement<TSqlTable>
    where TSqlTable : ISqlTable
{
    new TSqlTable Table { get; }
    new Func<TSqlTable, SqlExprBool> Predicate { get; }
}

// ========== UPDATE INTERFACES AND IMPLEMENTATIONS ==========

/// <summary>
/// Base interface for SQL UPDATE statements (non-generic).
/// </summary>
public interface ISqlUpdateStatement : ISqlStatement
{
    ISqlTable Table { get; }
    ImmutableArray<SetClause> SetClauses { get; }
}

/// <summary>
/// Interface for SQL UPDATE statements.
/// </summary>
/// <typeparam name="TSqlTable">The table type being updated</typeparam>
public interface ISqlUpdateStatement<TSqlTable> : ISqlUpdateStatement, ISqlStatement<TSqlTable>
    where TSqlTable : ISqlTable
{
    new TSqlTable Table { get; }
}

/// <summary>
/// Base interface for SQL UPDATE statements with WHERE clause (non-generic).
/// </summary>
public interface ISqlUpdateWhereStatement : ISqlStatement
{
    ISqlTable Table { get; }
    ImmutableArray<SetClause> SetClauses { get; }
    Func<ISqlTable, SqlExprBool> Predicate { get; }
}

/// <summary>
/// Interface for SQL UPDATE statements with WHERE clause.
/// </summary>
/// <typeparam name="TSqlTable">The table type being updated</typeparam>
public interface ISqlUpdateWhereStatement<TSqlTable> : ISqlUpdateWhereStatement, ISqlStatement<TSqlTable>
    where TSqlTable : ISqlTable
{
    new TSqlTable Table { get; }
    new Func<TSqlTable, SqlExprBool> Predicate { get; }
}

/// <summary>
/// Represents a SET clause in an UPDATE statement.
/// </summary>
public abstract record SetClause;

/// <summary>
/// SET clause for integer columns.
/// </summary>
public record SetIntClause(Func<ISqlTable, SqlExprInt> ColumnSelector, SqlExprInt Value) : SetClause;

/// <summary>
/// SET clause for string columns.
/// </summary>
public record SetStringClause(Func<ISqlTable, SqlExprString> ColumnSelector, SqlExprString Value) : SetClause;

/// <summary>
/// SET clause for boolean columns.
/// </summary>
public record SetBoolClause(Func<ISqlTable, SqlExprBool> ColumnSelector, SqlExprBool Value) : SetClause;

/// <summary>
/// Represents a VALUE clause in an INSERT statement.
/// </summary>
public abstract record ValueClause;

/// <summary>
/// VALUE clause for integer columns.
/// </summary>
public record InsertIntClause(Func<ISqlTable, SqlExprInt> ColumnSelector, SqlExprInt Value) : ValueClause;

/// <summary>
/// VALUE clause for string columns.
/// </summary>
public record InsertStringClause(Func<ISqlTable, SqlExprString> ColumnSelector, SqlExprString Value) : ValueClause;

/// <summary>
/// VALUE clause for boolean columns.
/// </summary>
public record InsertBoolClause(Func<ISqlTable, SqlExprBool> ColumnSelector, SqlExprBool Value) : ValueClause;

/// <summary>
/// Base record representing a SQL DELETE statement.
/// </summary>
/// <param name="Table">The table being deleted from</param>
public abstract record DeleteStatement(ISqlTable Table) : ISqlStatement;

/// <summary>
/// Base record representing a SQL DELETE statement with WHERE clause.
/// </summary>
/// <param name="Table">The table being deleted from</param>
/// <param name="Predicate">Function that evaluates filtering conditions</param>
public abstract record DeleteWhereStatement(ISqlTable Table, Func<ISqlTable, SqlExprBool> Predicate) : ISqlStatement;

/// <summary>
/// Base record representing a SQL UPDATE statement.
/// </summary>
/// <param name="Table">The table being updated</param>
/// <param name="SetClauses">The SET clauses for the update</param>
public abstract record UpdateStatement(ISqlTable Table, ImmutableArray<SetClause> SetClauses) : ISqlStatement;

/// <summary>
/// Base record representing a SQL UPDATE statement with WHERE clause.
/// </summary>
/// <param name="Table">The table being updated</param>
/// <param name="SetClauses">The SET clauses for the update</param>
/// <param name="Predicate">Function that evaluates filtering conditions</param>
public abstract record UpdateWhereStatement(ISqlTable Table, ImmutableArray<SetClause> SetClauses, Func<ISqlTable, SqlExprBool> Predicate) : ISqlStatement;

/// <summary>
/// Base record representing a SQL INSERT statement.
/// </summary>
/// <param name="Table">The table being inserted into</param>
/// <param name="ValueClauses">The VALUE clauses for the insert</param>
public abstract record InsertStatement(ISqlTable Table, ImmutableArray<ValueClause> ValueClauses) : ISqlStatement;

// ========== INSERT INTERFACES AND IMPLEMENTATIONS ==========

/// <summary>
/// Base interface for SQL INSERT statements (non-generic).
/// </summary>
public interface ISqlInsertStatement : ISqlStatement
{
    ISqlTable Table { get; }
    ImmutableArray<ValueClause> ValueClauses { get; }
}

/// <summary>
/// Interface for SQL INSERT statements.
/// </summary>
/// <typeparam name="TSqlTable">The table type being inserted into</typeparam>
public interface ISqlInsertStatement<TSqlTable> : ISqlInsertStatement, ISqlStatement<TSqlTable>
    where TSqlTable : ISqlTable
{
    new TSqlTable Table { get; }
}

/// <summary>
/// Implementation of a SQL DELETE statement.
/// </summary>
/// <typeparam name="TSqlTable">The table type being deleted from</typeparam>
public record DeleteStatement<TSqlTable> : DeleteStatement, ISqlDeleteStatement<TSqlTable>
    where TSqlTable : ISqlTable
{
    public new TSqlTable Table { get; }

    public DeleteStatement(TSqlTable table) : base(table)
    {
        Table = table;
    }

    ISqlTable ISqlDeleteStatement.Table => Table;
}

/// <summary>
/// Implementation of a SQL DELETE statement with WHERE clause.
/// </summary>
/// <typeparam name="TSqlTable">The table type being deleted from</typeparam>
public record DeleteWhereStatement<TSqlTable> : DeleteWhereStatement, ISqlDeleteWhereStatement<TSqlTable>
    where TSqlTable : ISqlTable
{
    public new TSqlTable Table { get; }
    public new Func<TSqlTable, SqlExprBool> Predicate { get; }

    public DeleteWhereStatement(TSqlTable table, Func<TSqlTable, SqlExprBool> predicate) : base(table, t => predicate((TSqlTable)t))
    {
        Table = table;
        Predicate = predicate;
    }

    ISqlTable ISqlDeleteWhereStatement.Table => Table;
    Func<ISqlTable, SqlExprBool> ISqlDeleteWhereStatement.Predicate => table => Predicate((TSqlTable)table);
}

/// <summary>
/// Implementation of a SQL UPDATE statement.
/// </summary>
/// <typeparam name="TSqlTable">The table type being updated</typeparam>
public record UpdateStatement<TSqlTable> : UpdateStatement, ISqlUpdateStatement<TSqlTable>
    where TSqlTable : ISqlTable
{
    public new TSqlTable Table { get; }

    public UpdateStatement(TSqlTable table) : base(table, ImmutableArray<SetClause>.Empty)
    {
        Table = table;
    }

    public UpdateStatement(TSqlTable table, ImmutableArray<SetClause> setClauses) : base(table, setClauses)
    {
        Table = table;
    }
    
    ISqlTable ISqlUpdateStatement.Table => Table;
}

/// <summary>
/// Implementation of a SQL UPDATE statement with WHERE clause.
/// </summary>
/// <typeparam name="TSqlTable">The table type being updated</typeparam>
public record UpdateWhereStatement<TSqlTable> : UpdateWhereStatement, ISqlUpdateWhereStatement<TSqlTable>
    where TSqlTable : ISqlTable
{
    public new TSqlTable Table { get; }
    public new Func<TSqlTable, SqlExprBool> Predicate { get; }

    public UpdateWhereStatement(TSqlTable table, ImmutableArray<SetClause> setClauses, Func<TSqlTable, SqlExprBool> predicate) : base(table, setClauses, t => predicate((TSqlTable)t))
    {
        Table = table;
        Predicate = predicate;
    }

    ISqlTable ISqlUpdateWhereStatement.Table => Table;
    Func<ISqlTable, SqlExprBool> ISqlUpdateWhereStatement.Predicate => table => Predicate((TSqlTable)table);
}

/// <summary>
/// Implementation of a SQL INSERT statement.
/// </summary>
/// <typeparam name="TSqlTable">The table type being inserted into</typeparam>
public record InsertStatement<TSqlTable> : InsertStatement, ISqlInsertStatement<TSqlTable>
    where TSqlTable : ISqlTable
{
    public new TSqlTable Table { get; }

    public InsertStatement(TSqlTable table) : base(table, ImmutableArray<ValueClause>.Empty)
    {
        Table = table;
    }

    public InsertStatement(TSqlTable table, ImmutableArray<ValueClause> valueClauses) : base(table, valueClauses)
    {
        Table = table;
    }
    
    ISqlTable ISqlInsertStatement.Table => Table;
}
