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
        if (deleteStatement is DeleteStatement<TSqlTable>(var table))
        {
            return new DeleteWhereStatement<TSqlTable>((TSqlTable)table, predicate);
        }
        throw new NotSupportedException($"Delete statement type {deleteStatement.GetType().Name} is not supported");
    }

    public static ISqlUpdateStatement<TSqlTable> Update<TSqlTable>()
        where TSqlTable : ISqlTable, new()
    {
        return new UpdateStatement<TSqlTable>(new TSqlTable());
    }

    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprInt> columnSelector, SqlExprInt value)
        where TSqlTable : ISqlTable, new()
    {
        if (updateStatement is UpdateStatement<TSqlTable>(var table, var setClauses))
        {
            var setClause = new SetIntClause(t => columnSelector((TSqlTable)t), value);
            var newSetClauses = setClauses.Append(setClause).ToImmutableArray();
            return new UpdateStatement<TSqlTable>((TSqlTable)table, newSetClauses);
        }
        throw new NotSupportedException($"Update statement type {updateStatement.GetType().Name} is not supported");
    }

    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprString> columnSelector, SqlExprString value)
        where TSqlTable : ISqlTable, new()
    {
        if (updateStatement is UpdateStatement<TSqlTable>(var table, var setClauses))
        {
            var setClause = new SetStringClause(t => columnSelector((TSqlTable)t), value);
            var newSetClauses = setClauses.Append(setClause).ToImmutableArray();
            return new UpdateStatement<TSqlTable>((TSqlTable)table, newSetClauses);
        }
        throw new NotSupportedException($"Update statement type {updateStatement.GetType().Name} is not supported");
    }

    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprBool> columnSelector, SqlExprBool value)
        where TSqlTable : ISqlTable, new()
    {
        if (updateStatement is UpdateStatement<TSqlTable>(var table, var setClauses))
        {
            var setClause = new SetBoolClause(t => columnSelector((TSqlTable)t), value);
            var newSetClauses = setClauses.Append(setClause).ToImmutableArray();
            return new UpdateStatement<TSqlTable>((TSqlTable)table, newSetClauses);
        }
        throw new NotSupportedException($"Update statement type {updateStatement.GetType().Name} is not supported");
    }

    // Expression-based SET methods (two lambda approach)
    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprInt> columnSelector, Func<TSqlTable, SqlExprInt> valueSelector)
        where TSqlTable : ISqlTable, new()
    {
        if (updateStatement is UpdateStatement<TSqlTable>(var table, var setClauses))
        {
            var value = valueSelector(new TSqlTable());
            var setClause = new SetIntClause(t => columnSelector((TSqlTable)t), value);
            var newSetClauses = setClauses.Append(setClause).ToImmutableArray();
            return new UpdateStatement<TSqlTable>((TSqlTable)table, newSetClauses);
        }
        throw new NotSupportedException($"Update statement type {updateStatement.GetType().Name} is not supported");
    }

    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprString> columnSelector, Func<TSqlTable, SqlExprString> valueSelector)
        where TSqlTable : ISqlTable, new()
    {
        if (updateStatement is UpdateStatement<TSqlTable>(var table, var setClauses))
        {
            var value = valueSelector(new TSqlTable());
            var setClause = new SetStringClause(t => columnSelector((TSqlTable)t), value);
            var newSetClauses = setClauses.Append(setClause).ToImmutableArray();
            return new UpdateStatement<TSqlTable>((TSqlTable)table, newSetClauses);
        }
        throw new NotSupportedException($"Update statement type {updateStatement.GetType().Name} is not supported");
    }

    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprBool> columnSelector, Func<TSqlTable, SqlExprBool> valueSelector)
        where TSqlTable : ISqlTable, new()
    {
        if (updateStatement is UpdateStatement<TSqlTable>(var table, var setClauses))
        {
            var value = valueSelector(new TSqlTable());
            var setClause = new SetBoolClause(t => columnSelector((TSqlTable)t), value);
            var newSetClauses = setClauses.Append(setClause).ToImmutableArray();
            return new UpdateStatement<TSqlTable>((TSqlTable)table, newSetClauses);
        }
        throw new NotSupportedException($"Update statement type {updateStatement.GetType().Name} is not supported");
    }

    public static ISqlUpdateWhereStatement<TSqlTable> Where<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprBool> predicate)
        where TSqlTable : ISqlTable, new()
    {
        if (updateStatement is UpdateStatement<TSqlTable>(var table, var setClauses))
        {
            return new UpdateWhereStatement<TSqlTable>((TSqlTable)table, setClauses, predicate);
        }
        throw new NotSupportedException($"Update statement type {updateStatement.GetType().Name} is not supported");
    }

    public static ISqlInsertStatement<TSqlTable> Insert<TSqlTable>()
        where TSqlTable : ISqlTable, new()
    {
        return new InsertStatement<TSqlTable>(new TSqlTable());
    }

    public static ISqlInsertStatement<TSqlTable> Value<TSqlTable>(this ISqlInsertStatement<TSqlTable> insertStatement, Func<TSqlTable, SqlExprInt> columnSelector, SqlExprInt value)
        where TSqlTable : ISqlTable, new()
    {
        if (insertStatement is InsertStatement<TSqlTable>(var table, var valueClauses))
        {
            var valueClause = new InsertIntClause(t => columnSelector((TSqlTable)t), value);
            var newValueClauses = valueClauses.Append(valueClause).ToImmutableArray();
            return new InsertStatement<TSqlTable>((TSqlTable)table, newValueClauses);
        }
        throw new NotSupportedException($"Insert statement type {insertStatement.GetType().Name} is not supported");
    }

    public static ISqlInsertStatement<TSqlTable> Value<TSqlTable>(this ISqlInsertStatement<TSqlTable> insertStatement, Func<TSqlTable, SqlExprString> columnSelector, SqlExprString value)
        where TSqlTable : ISqlTable, new()
    {
        if (insertStatement is InsertStatement<TSqlTable>(var table, var valueClauses))
        {
            var valueClause = new InsertStringClause(t => columnSelector((TSqlTable)t), value);
            var newValueClauses = valueClauses.Append(valueClause).ToImmutableArray();
            return new InsertStatement<TSqlTable>((TSqlTable)table, newValueClauses);
        }
        throw new NotSupportedException($"Insert statement type {insertStatement.GetType().Name} is not supported");
    }

    public static ISqlInsertStatement<TSqlTable> Value<TSqlTable>(this ISqlInsertStatement<TSqlTable> insertStatement, Func<TSqlTable, SqlExprBool> columnSelector, SqlExprBool value)
        where TSqlTable : ISqlTable, new()
    {
        if (insertStatement is InsertStatement<TSqlTable>(var table, var valueClauses))
        {
            var valueClause = new InsertBoolClause(t => columnSelector((TSqlTable)t), value);
            var newValueClauses = valueClauses.Append(valueClause).ToImmutableArray();
            return new InsertStatement<TSqlTable>((TSqlTable)table, newValueClauses);
        }
        throw new NotSupportedException($"Insert statement type {insertStatement.GetType().Name} is not supported");
    }
}
