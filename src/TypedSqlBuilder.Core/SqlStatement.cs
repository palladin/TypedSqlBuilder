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
public abstract record SetClause(Func<ISqlTable, SqlExpr> ColumnSelector, SqlExpr Value);

/// <summary>
/// SET clause for integer columns.
/// </summary>
public record SetIntClause : SetClause
{
    public Func<ISqlTable, SqlExprInt> IntColumnSelector { get; }
    public SqlExprInt IntValue { get; }

    public SetIntClause(Func<ISqlTable, SqlExprInt> columnSelector, SqlExprInt value) 
        : base(table => columnSelector(table), value)
    {
        IntColumnSelector = columnSelector;
        IntValue = value;
    }
}

/// <summary>
/// SET clause for string columns.
/// </summary>
public record SetStringClause : SetClause
{
    public Func<ISqlTable, SqlExprString> StringColumnSelector { get; }
    public SqlExprString StringValue { get; }

    public SetStringClause(Func<ISqlTable, SqlExprString> columnSelector, SqlExprString value) 
        : base(table => columnSelector(table), value)
    {
        StringColumnSelector = columnSelector;
        StringValue = value;
    }
}

/// <summary>
/// SET clause for boolean columns.
/// </summary>
public record SetBoolClause : SetClause
{
    public Func<ISqlTable, SqlExprBool> BoolColumnSelector { get; }
    public SqlExprBool BoolValue { get; }

    public SetBoolClause(Func<ISqlTable, SqlExprBool> columnSelector, SqlExprBool value) 
        : base(table => columnSelector(table), value)
    {
        BoolColumnSelector = columnSelector;
        BoolValue = value;
    }
}

/// <summary>
/// Represents a VALUE clause in an INSERT statement.
/// </summary>
public abstract record ValueClause(Func<ISqlTable, SqlExpr> ColumnSelector, SqlExpr Value);

/// <summary>
/// VALUE clause for integer columns.
/// </summary>
public record InsertIntClause : ValueClause
{
    public Func<ISqlTable, SqlExprInt> IntColumnSelector { get; }
    public SqlExprInt IntValue { get; }

    public InsertIntClause(Func<ISqlTable, SqlExprInt> columnSelector, SqlExprInt value) 
        : base(table => columnSelector(table), value)
    {
        IntColumnSelector = columnSelector;
        IntValue = value;
    }
}

/// <summary>
/// VALUE clause for string columns.
/// </summary>
public record InsertStringClause : ValueClause
{
    public Func<ISqlTable, SqlExprString> StringColumnSelector { get; }
    public SqlExprString StringValue { get; }

    public InsertStringClause(Func<ISqlTable, SqlExprString> columnSelector, SqlExprString value) 
        : base(table => columnSelector(table), value)
    {
        StringColumnSelector = columnSelector;
        StringValue = value;
    }
}

/// <summary>
/// VALUE clause for boolean columns.
/// </summary>
public record InsertBoolClause : ValueClause
{
    public Func<ISqlTable, SqlExprBool> BoolColumnSelector { get; }
    public SqlExprBool BoolValue { get; }

    public InsertBoolClause(Func<ISqlTable, SqlExprBool> columnSelector, SqlExprBool value) 
        : base(table => columnSelector(table), value)
    {
        BoolColumnSelector = columnSelector;
        BoolValue = value;
    }
}

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
public record DeleteStatement<TSqlTable> : DeleteStatement, ISqlDeleteStatement<TSqlTable>
    where TSqlTable : ISqlTable
{
    public new TSqlTable Table { get; }

    public DeleteStatement(TSqlTable table) : base(table)
    {
        Table = table;
    }
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
}
