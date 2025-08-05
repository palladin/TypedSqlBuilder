using System.Collections.Immutable;

namespace TypedSqlBuilder.Core;

/// <summary>
/// Factory class for creating SQL queries and statements.
/// Provides entry points for building type-safe SQL queries and data modification statements.
/// </summary>
public static class TypedSql
{
    /// <summary>
    /// Creates a query from a predefined SqlTable subclass.
    /// The table structure is defined by the SqlTable implementation.
    /// </summary>
    /// <typeparam name="TSqlTable">The SqlTable subclass defining the table structure</typeparam>
    /// <returns>A typed SQL query that can be further composed</returns>
    /// <example>
    /// <code>
    /// var query = SqlQuery.From&lt;UsersTable&gt;();
    /// </code>
    /// </example>
    public static ISqlQuery<TSqlTable> From<TSqlTable>()
        where TSqlTable : ISqlTable, new()
    {
        return new FromClause<TSqlTable>(new TSqlTable());
    }

    public static ISqlDeleteStatement<TSqlTable> Delete<TSqlTable>()
        where TSqlTable : ISqlTable, new()
    {
        return new DeleteStatement<TSqlTable>(new TSqlTable());
    }

    public static ISqlDeleteWhereStatement<TSqlTable> Where<TSqlTable>(this ISqlDeleteStatement<TSqlTable> deleteStatement, Func<TSqlTable, SqlExprBool> predicate)
        where TSqlTable : ISqlTable, new()
    {
        return new DeleteWhereStatement<TSqlTable>(deleteStatement.Table, predicate);
    }

    public static ISqlUpdateStatement<TSqlTable> Update<TSqlTable>()
        where TSqlTable : ISqlTable, new()
    {
        return new UpdateStatement<TSqlTable>(new TSqlTable());
    }

    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprInt> columnSelector, SqlExprInt value)
        where TSqlTable : ISqlTable, new()
    {
        var setClause = new SetIntClause(table => columnSelector((TSqlTable)table), value);
        var newSetClauses = updateStatement.SetClauses.Append(setClause).ToImmutableArray();
        return new UpdateStatement<TSqlTable>(updateStatement.Table, newSetClauses);
    }

    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprString> columnSelector, SqlExprString value)
        where TSqlTable : ISqlTable, new()
    {
        var setClause = new SetStringClause(table => columnSelector((TSqlTable)table), value);
        var newSetClauses = updateStatement.SetClauses.Append(setClause).ToImmutableArray();
        return new UpdateStatement<TSqlTable>(updateStatement.Table, newSetClauses);
    }

    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprBool> columnSelector, SqlExprBool value)
        where TSqlTable : ISqlTable, new()
    {
        var setClause = new SetBoolClause(table => columnSelector((TSqlTable)table), value);
        var newSetClauses = updateStatement.SetClauses.Append(setClause).ToImmutableArray();
        return new UpdateStatement<TSqlTable>(updateStatement.Table, newSetClauses);
    }

    // Expression-based SET methods (two lambda approach)
    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprInt> columnSelector, Func<TSqlTable, SqlExprInt> valueSelector)
        where TSqlTable : ISqlTable, new()
    {
        var value = valueSelector(new TSqlTable());
        var setClause = new SetIntClause(table => columnSelector((TSqlTable)table), value);
        var newSetClauses = updateStatement.SetClauses.Append(setClause).ToImmutableArray();
        return new UpdateStatement<TSqlTable>(updateStatement.Table, newSetClauses);
    }

    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprString> columnSelector, Func<TSqlTable, SqlExprString> valueSelector)
        where TSqlTable : ISqlTable, new()
    {
        var value = valueSelector(new TSqlTable());
        var setClause = new SetStringClause(table => columnSelector((TSqlTable)table), value);
        var newSetClauses = updateStatement.SetClauses.Append(setClause).ToImmutableArray();
        return new UpdateStatement<TSqlTable>(updateStatement.Table, newSetClauses);
    }

    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprBool> columnSelector, Func<TSqlTable, SqlExprBool> valueSelector)
        where TSqlTable : ISqlTable, new()
    {
        var value = valueSelector(new TSqlTable());
        var setClause = new SetBoolClause(table => columnSelector((TSqlTable)table), value);
        var newSetClauses = updateStatement.SetClauses.Append(setClause).ToImmutableArray();
        return new UpdateStatement<TSqlTable>(updateStatement.Table, newSetClauses);
    }

    public static ISqlUpdateWhereStatement<TSqlTable> Where<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprBool> predicate)
        where TSqlTable : ISqlTable, new()
    {
        return new UpdateWhereStatement<TSqlTable>(updateStatement.Table, updateStatement.SetClauses, predicate);
    }

    public static ISqlInsertStatement<TSqlTable> Insert<TSqlTable>()
        where TSqlTable : ISqlTable, new()
    {
        return new InsertStatement<TSqlTable>(new TSqlTable());
    }

    public static ISqlInsertStatement<TSqlTable> Value<TSqlTable>(this ISqlInsertStatement<TSqlTable> insertStatement, Func<TSqlTable, SqlExprInt> columnSelector, SqlExprInt value)
        where TSqlTable : ISqlTable, new()
    {
        var valueClause = new InsertIntClause(table => columnSelector((TSqlTable)table), value);
        var newValueClauses = insertStatement.ValueClauses.Append(valueClause).ToImmutableArray();
        return new InsertStatement<TSqlTable>(insertStatement.Table, newValueClauses);
    }

    public static ISqlInsertStatement<TSqlTable> Value<TSqlTable>(this ISqlInsertStatement<TSqlTable> insertStatement, Func<TSqlTable, SqlExprString> columnSelector, SqlExprString value)
        where TSqlTable : ISqlTable, new()
    {
        var valueClause = new InsertStringClause(table => columnSelector((TSqlTable)table), value);
        var newValueClauses = insertStatement.ValueClauses.Append(valueClause).ToImmutableArray();
        return new InsertStatement<TSqlTable>(insertStatement.Table, newValueClauses);
    }

    public static ISqlInsertStatement<TSqlTable> Value<TSqlTable>(this ISqlInsertStatement<TSqlTable> insertStatement, Func<TSqlTable, SqlExprBool> columnSelector, SqlExprBool value)
        where TSqlTable : ISqlTable, new()
    {
        var valueClause = new InsertBoolClause(table => columnSelector((TSqlTable)table), value);
        var newValueClauses = insertStatement.ValueClauses.Append(valueClause).ToImmutableArray();
        return new InsertStatement<TSqlTable>(insertStatement.Table, newValueClauses);
    }
}
