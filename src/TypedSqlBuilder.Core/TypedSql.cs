using System.Collections.Immutable;
using System.Runtime.CompilerServices;

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
        return new FromTableClause<TSqlTable>(new TSqlTable());
    }
    
    /// <summary>
    /// Creates a query from a SqlTable instance using extension method syntax for more fluent method chaining.
    /// This extension method provides a more natural and readable way to start query chains by allowing
    /// table instances to be used directly in fluent syntax patterns.
    /// </summary>
    /// <typeparam name="TSqlTable">The SqlTable subclass defining the table structure</typeparam>
    /// <param name="_">The table instance (parameter is ignored, a fresh instance is created internally)</param>
    /// <returns>A typed SQL query that can be further composed with additional clauses</returns>
    /// <example>
    /// <code>
    /// var users = new UsersTable();
    /// var query = users.From()  // More fluent than TypedSql.From&lt;UsersTable&gt;()
    ///                  .Where(u => u.Age > 18)
    ///                  .Select(u => new { u.Name, u.Email });
    /// </code>
    /// </example>
    /// <remarks>
    /// This method creates a fresh instance of the table internally rather than using the passed instance.
    /// This is essential for proper alias disambiguation during SQL compilation, ensuring each table reference
    /// gets a unique alias (e.g., a0, a1, a2) and preventing conflicts when the same table type is used
    /// multiple times in complex queries or joins.
    /// </remarks>
    public static ISqlQuery<TSqlTable> From<TSqlTable>(this TSqlTable _)
        where TSqlTable : ISqlTable, new()
    {
        // Create a fresh instance of the table
        return new FromTableClause<TSqlTable>(new TSqlTable());
    }

    /// <summary>
    /// Creates a query from a subquery.
    /// Allows using the result of one query as the source for another query.
    /// </summary>
    /// <typeparam name="TSource">The tuple type representing the subquery's columns</typeparam>
    /// <param name="query">The subquery to use as the source</param>
    /// <returns>A typed SQL query that can be further composed</returns>
    public static ISqlQuery<TSource> From<TSource>(ISqlQuery<TSource> query)
        where TSource : ITuple
    {
        return new FromSubQueryClause<TSource>(query);
    }

    /// <summary>
    /// Creates a DELETE statement for the specified table.
    /// </summary>
    /// <typeparam name="TSqlTable">The SqlTable subclass defining the table structure</typeparam>
    /// <returns>A DELETE statement that can be further composed with WHERE clauses</returns>
    public static ISqlDeleteStatement<TSqlTable> Delete<TSqlTable>()
        where TSqlTable : ISqlTable, new()
    {
        return new DeleteStatement<TSqlTable>(new TSqlTable());
    }

    /// <summary>
    /// Adds a WHERE clause to a DELETE statement.
    /// </summary>
    /// <typeparam name="TSqlTable">The SqlTable subclass defining the table structure</typeparam>
    /// <param name="deleteStatement">The DELETE statement to add the WHERE clause to</param>
    /// <param name="predicate">A function that defines the filter condition</param>
    /// <returns>A DELETE statement with the WHERE clause applied</returns>
    public static ISqlDeleteWhereStatement<TSqlTable> Where<TSqlTable>(this ISqlDeleteStatement<TSqlTable> deleteStatement, Func<TSqlTable, SqlExprBool> predicate)
        where TSqlTable : ISqlTable, new()
    {
        return new DeleteWhereStatement<TSqlTable>(deleteStatement, predicate);
    }

    /// <summary>
    /// Creates an UPDATE statement for the specified table.
    /// </summary>
    /// <typeparam name="TSqlTable">The SqlTable subclass defining the table structure</typeparam>
    /// <returns>An UPDATE statement that can be further composed with SET and WHERE clauses</returns>
    public static ISqlUpdateStatement<TSqlTable> Update<TSqlTable>()
        where TSqlTable : ISqlTable, new()
    {
        return new UpdateStatement<TSqlTable>(new TSqlTable());
    }

    /// <summary>
    /// Adds a SET clause to an UPDATE statement for an integer column.
    /// </summary>
    /// <typeparam name="TSqlTable">The SqlTable subclass defining the table structure</typeparam>
    /// <param name="updateStatement">The UPDATE statement to add the SET clause to</param>
    /// <param name="columnSelector">A function that selects the integer column to update</param>
    /// <param name="value">The new value to set for the column</param>
    /// <returns>An UPDATE statement with the SET clause applied</returns>
    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprInt> columnSelector, SqlExprInt value)
        where TSqlTable : ISqlTable, new()
    {
        var setClause = new SetIntClause(t => columnSelector((TSqlTable)t), value);
        return new SetStatement<TSqlTable>(updateStatement, setClause);
    }

    /// <summary>
    /// Adds a SET clause to an UPDATE statement for a string column.
    /// </summary>
    /// <typeparam name="TSqlTable">The SqlTable subclass defining the table structure</typeparam>
    /// <param name="updateStatement">The UPDATE statement to add the SET clause to</param>
    /// <param name="columnSelector">A function that selects the string column to update</param>
    /// <param name="value">The new value to set for the column</param>
    /// <returns>An UPDATE statement with the SET clause applied</returns>
    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprString> columnSelector, SqlExprString value)
        where TSqlTable : ISqlTable, new()
    {
        var setClause = new SetStringClause(t => columnSelector((TSqlTable)t), value);
        return new SetStatement<TSqlTable>(updateStatement, setClause);
    }

    /// <summary>
    /// Adds a SET clause to an UPDATE statement for a boolean column.
    /// </summary>
    /// <typeparam name="TSqlTable">The SqlTable subclass defining the table structure</typeparam>
    /// <param name="updateStatement">The UPDATE statement to add the SET clause to</param>
    /// <param name="columnSelector">A function that selects the boolean column to update</param>
    /// <param name="value">The new value to set for the column</param>
    /// <returns>An UPDATE statement with the SET clause applied</returns>
    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprBool> columnSelector, SqlExprBool value)
        where TSqlTable : ISqlTable, new()
    {
        var setClause = new SetBoolClause(t => columnSelector((TSqlTable)t), value);
        return new SetStatement<TSqlTable>(updateStatement, setClause);
    }

    // Expression-based SET methods (two lambda approach)
    /// <summary>
    /// Adds a SET clause to an UPDATE statement for an integer column using an expression.
    /// </summary>
    /// <typeparam name="TSqlTable">The SqlTable subclass defining the table structure</typeparam>
    /// <param name="updateStatement">The UPDATE statement to add the SET clause to</param>
    /// <param name="columnSelector">A function that selects the integer column to update</param>
    /// <param name="valueSelector">A function that generates the new value expression for the column</param>
    /// <returns>An UPDATE statement with the SET clause applied</returns>
    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprInt> columnSelector, Func<TSqlTable, SqlExprInt> valueSelector)
        where TSqlTable : ISqlTable, new()
    {
        var value = valueSelector(new TSqlTable());
        var setClause = new SetIntClause(t => columnSelector((TSqlTable)t), value);
        return new SetStatement<TSqlTable>(updateStatement, setClause);
    }

    /// <summary>
    /// Adds a SET clause to an UPDATE statement for a string column using an expression.
    /// </summary>
    /// <typeparam name="TSqlTable">The SqlTable subclass defining the table structure</typeparam>
    /// <param name="updateStatement">The UPDATE statement to add the SET clause to</param>
    /// <param name="columnSelector">A function that selects the string column to update</param>
    /// <param name="valueSelector">A function that generates the new value expression for the column</param>
    /// <returns>An UPDATE statement with the SET clause applied</returns>
    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprString> columnSelector, Func<TSqlTable, SqlExprString> valueSelector)
        where TSqlTable : ISqlTable, new()
    {
        var value = valueSelector(new TSqlTable());
        var setClause = new SetStringClause(t => columnSelector((TSqlTable)t), value);
        return new SetStatement<TSqlTable>(updateStatement, setClause);
    }

    /// <summary>
    /// Adds a SET clause to an UPDATE statement for a boolean column using an expression.
    /// </summary>
    /// <typeparam name="TSqlTable">The SqlTable subclass defining the table structure</typeparam>
    /// <param name="updateStatement">The UPDATE statement to add the SET clause to</param>
    /// <param name="columnSelector">A function that selects the boolean column to update</param>
    /// <param name="valueSelector">A function that generates the new value expression for the column</param>
    /// <returns>An UPDATE statement with the SET clause applied</returns>
    public static ISqlUpdateStatement<TSqlTable> Set<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprBool> columnSelector, Func<TSqlTable, SqlExprBool> valueSelector)
        where TSqlTable : ISqlTable, new()
    {
        var value = valueSelector(new TSqlTable());
        var setClause = new SetBoolClause(t => columnSelector((TSqlTable)t), value);
        return new SetStatement<TSqlTable>(updateStatement, setClause);
    }

    /// <summary>
    /// Adds a WHERE clause to an UPDATE statement.
    /// </summary>
    /// <typeparam name="TSqlTable">The SqlTable subclass defining the table structure</typeparam>
    /// <param name="updateStatement">The UPDATE statement to add the WHERE clause to</param>
    /// <param name="predicate">A function that defines the filter condition</param>
    /// <returns>An UPDATE statement with the WHERE clause applied</returns>
    public static ISqlUpdateWhereStatement<TSqlTable> Where<TSqlTable>(this ISqlUpdateStatement<TSqlTable> updateStatement, Func<TSqlTable, SqlExprBool> predicate)
        where TSqlTable : ISqlTable, new()
    {
        return new UpdateWhereFromStatement<TSqlTable>(updateStatement, predicate);
    }

    /// <summary>
    /// Creates an INSERT statement for the specified table.
    /// </summary>
    /// <typeparam name="TSqlTable">The SqlTable subclass defining the table structure</typeparam>
    /// <returns>An INSERT statement that can be further composed with VALUE clauses</returns>
    public static ISqlInsertStatement<TSqlTable> Insert<TSqlTable>()
        where TSqlTable : ISqlTable, new()
    {
        return new InsertStatement<TSqlTable>(new TSqlTable());
    }

    /// <summary>
    /// Adds a VALUE clause to an INSERT statement for an integer column.
    /// </summary>
    /// <typeparam name="TSqlTable">The SqlTable subclass defining the table structure</typeparam>
    /// <param name="insertStatement">The INSERT statement to add the VALUE clause to</param>
    /// <param name="columnSelector">A function that selects the integer column to insert into</param>
    /// <param name="value">The value to insert for the column</param>
    /// <returns>An INSERT statement with the VALUE clause applied</returns>
    public static ISqlInsertStatement<TSqlTable> Value<TSqlTable>(this ISqlInsertStatement<TSqlTable> insertStatement, Func<TSqlTable, SqlExprInt> columnSelector, SqlExprInt value)
        where TSqlTable : ISqlTable, new()
    {
        var valueClause = new InsertIntClause(t => columnSelector((TSqlTable)t), value);
        return new ValueStatement<TSqlTable>(insertStatement, valueClause);
    }

    /// <summary>
    /// Adds a VALUE clause to an INSERT statement for a string column.
    /// </summary>
    /// <typeparam name="TSqlTable">The SqlTable subclass defining the table structure</typeparam>
    /// <param name="insertStatement">The INSERT statement to add the VALUE clause to</param>
    /// <param name="columnSelector">A function that selects the string column to insert into</param>
    /// <param name="value">The value to insert for the column</param>
    /// <returns>An INSERT statement with the VALUE clause applied</returns>
    public static ISqlInsertStatement<TSqlTable> Value<TSqlTable>(this ISqlInsertStatement<TSqlTable> insertStatement, Func<TSqlTable, SqlExprString> columnSelector, SqlExprString value)
        where TSqlTable : ISqlTable, new()
    {
        var valueClause = new InsertStringClause(t => columnSelector((TSqlTable)t), value);
        return new ValueStatement<TSqlTable>(insertStatement, valueClause);
    }

    /// <summary>
    /// Adds a VALUE clause to an INSERT statement for a boolean column.
    /// </summary>
    /// <typeparam name="TSqlTable">The SqlTable subclass defining the table structure</typeparam>
    /// <param name="insertStatement">The INSERT statement to add the VALUE clause to</param>
    /// <param name="columnSelector">A function that selects the boolean column to insert into</param>
    /// <param name="value">The value to insert for the column</param>
    /// <returns>An INSERT statement with the VALUE clause applied</returns>
    public static ISqlInsertStatement<TSqlTable> Value<TSqlTable>(this ISqlInsertStatement<TSqlTable> insertStatement, Func<TSqlTable, SqlExprBool> columnSelector, SqlExprBool value)
        where TSqlTable : ISqlTable, new()
    {
        var valueClause = new InsertBoolClause(t => columnSelector((TSqlTable)t), value);
        return new ValueStatement<TSqlTable>(insertStatement, valueClause);
    }
}
