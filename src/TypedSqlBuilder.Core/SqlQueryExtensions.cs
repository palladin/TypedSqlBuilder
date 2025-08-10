using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypedSqlBuilder.Core;

/// <summary>
/// Extension methods providing a fluent API for building SQL queries and compiling SQL.
/// Enables method chaining to construct complex queries in a readable manner.
/// </summary>
public static class SqlQueryExtensions
{
    /// <summary>
    /// Projects the query result to a different tuple type.
    /// Transforms query results while maintaining type safety.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <typeparam name="TResult">The output tuple type after projection</typeparam>
    /// <param name="query">The source query to project from</param>
    /// <param name="selector">Function that transforms source tuples to result tuples</param>
    /// <returns>A new query that produces the projected result type</returns>
    /// <example>
    /// <code>
    /// var projectedQuery = baseQuery.Select(row => (row.Name, row.Age));
    /// </code>
    /// </example>
    public static ISqlQuery<TResult> Select<TSource, TResult>(this ISqlQuery<TSource> query, Func<TSource, TResult> selector)
        where TSource : ITuple
        where TResult : ITuple
    {
        return new SelectClause<TSource, TResult>(query, selector);
    }

    /// <summary>
    /// Projects the query result to a SQL integer expression.
    /// Used for selecting computed integer values or preparing for aggregate operations.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <param name="query">The source query to project from</param>
    /// <param name="selector">Function that transforms source tuples to SQL integer expressions</param>
    /// <returns>A new query that produces SQL integer expressions</returns>
    /// <example>
    /// <code>
    /// var ageQuery = userQuery.Select(user => user.Age);
    /// </code>
    /// </example>
    public static ISqlQuery<ValueTuple<SqlExprInt>> Select<TSource>(this ISqlQuery<TSource> query, Func<TSource, SqlExprInt> selector)
        where TSource : ITuple
    {
        return new SelectClause<TSource, ValueTuple<SqlExprInt>>(query, x => ValueTuple.Create(selector(x)));
    }

    /// <summary>
    /// Projects the query result to a SQL boolean expression.
    /// Used for selecting computed boolean values or preparing conditional expressions.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <param name="query">The source query to project from</param>
    /// <param name="selector">Function that transforms source tuples to SQL boolean expressions</param>
    /// <returns>A new query that produces SQL boolean expressions</returns>
    /// <example>
    /// <code>
    /// var adultQuery = userQuery.Select(user => user.Age >= 18);
    /// </code>
    /// </example>
    public static ISqlQuery<ValueTuple<SqlExprBool>> Select<TSource>(this ISqlQuery<TSource> query, Func<TSource, SqlExprBool> selector)
        where TSource : ITuple
    {
        return new SelectClause<TSource, ValueTuple<SqlExprBool>>(query, x => ValueTuple.Create(selector(x)));
    }

    /// <summary>
    /// Projects the query result to a SQL string expression.
    /// Used for selecting computed string values or preparing string operations.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <param name="query">The source query to project from</param>
    /// <param name="selector">Function that transforms source tuples to SQL string expressions</param>
    /// <returns>A new query that produces SQL string expressions</returns>
    /// <example>
    /// <code>
    /// var nameQuery = userQuery.Select(user => user.FirstName + " " + user.LastName);
    /// </code>
    /// </example>
    public static ISqlQuery<ValueTuple<SqlExprString>> Select<TSource>(this ISqlQuery<TSource> query, Func<TSource, SqlExprString> selector)
        where TSource : ITuple
    {
        return new SelectClause<TSource, ValueTuple<SqlExprString>>(query, x => ValueTuple.Create(selector(x)));
    }

    /// <summary>
    /// Applies the SUM aggregate function to a query of integer expressions.
    /// Computes the total sum of all integer values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL integer expressions</param>
    /// <returns>A SQL scalar query representing the sum of all values</returns>
    /// <example>
    /// <code>
    /// var totalAge = userQuery.Select(user => user.Age).Sum();
    /// </code>
    /// </example>
    public static SumSqlIntClause Sum(this ISqlQuery<ValueTuple<SqlExprInt>> query)
    {
        return new SumSqlIntClause(query);
    }

    /// <summary>
    /// Applies the COUNT aggregate function to count rows in the query result.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <param name="query">The query whose rows will be counted</param>
    /// <returns>A SQL scalar query representing the count of rows</returns>
    /// <example>
    /// <code>
    /// var userCount = userQuery.Count();
    /// </code>
    /// </example>
    public static CountClause Count<TSource>(this ISqlQuery<TSource> query)
        where TSource : ITuple
    {
        return new CountClause<TSource>(query);
    }

    /// <summary>
    /// Filters the query result based on a boolean condition.
    /// Only rows that satisfy the predicate will be included in the result.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <param name="query">The source query to filter</param>
    /// <param name="predicate">Function that evaluates boolean conditions on source tuples</param>
    /// <returns>A new query that includes only rows satisfying the predicate</returns>
    /// <example>
    /// <code>
    /// var adultUsers = userQuery.Where(user => user.Age >= 18);
    /// </code>
    /// </example>
    public static ISqlQuery<TSource> Where<TSource>(this ISqlQuery<TSource> query, Func<TSource, SqlExprBool> predicate)
        where TSource : ITuple
    {
        return new WhereClause<TSource>(query, predicate);
    }

    /// <summary>
    /// Sorts the query result in ascending order based on the specified key.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <typeparam name="TKey">The type of the sorting key, must be a SQL expression</typeparam>
    /// <param name="query">The source query to sort</param>
    /// <param name="keySelector">Function that extracts the sorting key from source tuples</param>
    /// <returns>A new query with the specified ascending sort order</returns>
    /// <example>
    /// <code>
    /// var sortedUsers = userQuery.OrderBy(user => user.Name);
    /// </code>
    /// </example>
    public static ISqlOrderedQuery<TSource> OrderBy<TSource, TKey>(this ISqlQuery<TSource> query, Func<TSource, TKey> keySelector)
        where TSource : ITuple
        where TKey : SqlExpr
    {
        return new OrderByClause<TSource, TKey>(query, keySelector, false);
    }

    /// <summary>
    /// Sorts the query result in descending order based on the specified key.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <typeparam name="TKey">The type of the sorting key, must be a SQL expression</typeparam>
    /// <param name="query">The source query to sort</param>
    /// <param name="keySelector">Function that extracts the sorting key from source tuples</param>
    /// <returns>A new query with the specified descending sort order</returns>
    /// <example>
    /// <code>
    /// var usersByAgeDesc = userQuery.OrderByDescending(user => user.Age);
    /// </code>
    /// </example>
    public static ISqlOrderedQuery<TSource> OrderByDescending<TSource, TKey>(this ISqlQuery<TSource> query, Func<TSource, TKey> keySelector)
        where TSource : ITuple
        where TKey : SqlExpr
    {
        return new OrderByClause<TSource, TKey>(query, keySelector, true);
    }

    /// <summary>
    /// Adds a secondary ascending sort to an existing ORDER BY clause.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <typeparam name="TKey">The type of the sorting key, must be a SQL expression</typeparam>
    /// <param name="query">The source query with existing ORDER BY</param>
    /// <param name="keySelector">Function that extracts the secondary sorting key from source tuples</param>
    /// <returns>A new query with the additional ascending sort order</returns>
    /// <example>
    /// <code>
    /// var sortedUsers = userQuery.OrderBy(user => user.Name).ThenBy(user => user.Age);
    /// </code>
    /// </example>
    public static ISqlOrderedQuery<TSource> ThenBy<TSource, TKey>(this ISqlOrderedQuery<TSource> query, Func<TSource, TKey> keySelector)
        where TSource : ITuple
        where TKey : SqlExpr
    {
        return new OrderByClause<TSource, TKey>(query, keySelector, false);
    }

    /// <summary>
    /// Adds a secondary descending sort to an existing ORDER BY clause.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <typeparam name="TKey">The type of the sorting key, must be a SQL expression</typeparam>
    /// <param name="query">The source query with existing ORDER BY</param>
    /// <param name="keySelector">Function that extracts the secondary sorting key from source tuples</param>
    /// <returns>A new query with the additional descending sort order</returns>
    /// <example>
    /// <code>
    /// var sortedUsers = userQuery.OrderBy(user => user.Name).ThenByDescending(user => user.Age);
    /// </code>
    /// </example>
    public static ISqlOrderedQuery<TSource> ThenByDescending<TSource, TKey>(this ISqlOrderedQuery<TSource> query, Func<TSource, TKey> keySelector)
        where TSource : ITuple
        where TKey : SqlExpr
    {
        return new OrderByClause<TSource, TKey>(query, keySelector, true);
    }

    /// <summary>
    /// Compiles the SQL query using the specified SQL compiler.
    /// </summary>
    /// <param name="query">The SQL query to compile</param>
    /// <param name="compiler">The SQL compiler to use for compilation</param>
    /// <returns>A tuple containing the SQL string and parameter dictionary</returns>
    /// <example>
    /// <code>
    /// var (sql, parameters) = query.ToSqlRaw(SqlCompiler.SqlServer);
    /// var (sql2, parameters2) = query.ToSqlRaw(SqlCompiler.Sqlite);
    /// </code>
    /// </example>
    public static (string SqlRaw, ImmutableDictionary<string, object> Parameters) ToSqlRaw(this ISqlQuery query, SqlCompiler compiler)
    {
        var (sql, context) = compiler.Compile(query, new Context());
        return (sql, context.Parameters);
    }

    /// <summary>
    /// Compiles the SQL query to SQL Server syntax with parameters.
    /// </summary>
    /// <param name="query">The SQL query to compile</param>
    /// <returns>A tuple containing the SQL string and parameter dictionary</returns>
    /// <example>
    /// <code>
    /// var (sql, parameters) = query.ToSqlServerRaw();
    /// </code>
    /// </example>
    public static (string SqlRaw, ImmutableDictionary<string, object> Parameters) ToSqlServerRaw(this ISqlQuery query)
    {
        var (sql, context) = SqlCompiler.SqlServer.Compile(query, new Context());
        return (sql, context.Parameters);
    }

    /// <summary>
    /// Compiles the SQL query to SQLite syntax with parameters.
    /// </summary>
    /// <param name="query">The SQL query to compile</param>
    /// <returns>A tuple containing the SQL string and parameter dictionary</returns>
    /// <example>
    /// <code>
    /// var (sql, parameters) = query.ToSqliteRaw();
    /// </code>
    /// </example>
    public static (string SqlRaw, ImmutableDictionary<string, object> Parameters) ToSqliteRaw(this ISqlQuery query)
    {
        var (sql, context) = SqlCompiler.Sqlite.Compile(query, new Context());
        return (sql, context.Parameters);
    }

    /// <summary>
    /// Compiles the SQL statement using the specified SQL compiler.
    /// </summary>
    /// <param name="statement">The SQL statement to compile</param>
    /// <param name="compiler">The SQL compiler to use for compilation</param>
    /// <returns>A tuple containing the SQL string and parameter dictionary</returns>
    /// <example>
    /// <code>
    /// var (sql, parameters) = statement.ToSqlRaw(SqlCompiler.SqlServer);
    /// var (sql2, parameters2) = statement.ToSqlRaw(SqlCompiler.Sqlite);
    /// </code>
    /// </example>
    public static (string SqlRaw, ImmutableDictionary<string, object> Parameters) ToSqlRaw(this ISqlStatement statement, SqlCompiler compiler)
    {
        var (sql, context) = compiler.Compile(statement, new Context());
        return (sql, context.Parameters);
    }

    /// <summary>
    /// Compiles the SQL statement to SQL Server syntax with parameters.
    /// </summary>
    /// <param name="statement">The SQL statement to compile</param>
    /// <returns>A tuple containing the SQL string and parameter dictionary</returns>
    /// <example>
    /// <code>
    /// var (sql, parameters) = statement.ToSqlServerRaw();
    /// </code>
    /// </example>
    public static (string SqlRaw, ImmutableDictionary<string, object> Parameters) ToSqlServerRaw(this ISqlStatement statement)
    {
        var (sql, context) = SqlCompiler.SqlServer.Compile(statement, new Context());
        return (sql, context.Parameters);
    }

    /// <summary>
    /// Compiles the SQL statement to SQLite syntax with parameters.
    /// </summary>
    /// <param name="statement">The SQL statement to compile</param>
    /// <returns>A tuple containing the SQL string and parameter dictionary</returns>
    /// <example>
    /// <code>
    /// var (sql, parameters) = statement.ToSqliteRaw();
    /// </code>
    /// </example>
    public static (string SqlRaw, ImmutableDictionary<string, object> Parameters) ToSqliteRaw(this ISqlStatement statement)
    {
        var (sql, context) = SqlCompiler.Sqlite.Compile(statement, new Context());
        return (sql, context.Parameters);
    }

    /// <summary>
    /// Compiles the SQL scalar query using the specified SQL compiler.
    /// </summary>
    /// <param name="scalarQuery">The SQL scalar query to compile</param>
    /// <param name="compiler">The SQL compiler to use for compilation</param>
    /// <returns>A tuple containing the SQL string and parameter dictionary</returns>
    /// <example>
    /// <code>
    /// var (sql, parameters) = countQuery.ToSqlRaw(SqlCompiler.SqlServer);
    /// var (sql2, parameters2) = countQuery.ToSqlRaw(SqlCompiler.Sqlite);
    /// </code>
    /// </example>
    public static (string SqlRaw, ImmutableDictionary<string, object> Parameters) ToSqlRaw(this ISqlScalarQuery scalarQuery, SqlCompiler compiler)
    {
        var (sql, context) = compiler.Compile(scalarQuery, new Context());
        return (sql, context.Parameters);
    }

    /// <summary>
    /// Compiles the SQL scalar query to SQL Server syntax with parameters.
    /// </summary>
    /// <param name="scalarQuery">The SQL scalar query to compile</param>
    /// <returns>A tuple containing the SQL string and parameter dictionary</returns>
    /// <example>
    /// <code>
    /// var (sql, parameters) = countQuery.ToSqlServerRaw();
    /// </code>
    /// </example>
    public static (string SqlRaw, ImmutableDictionary<string, object> Parameters) ToSqlServerRaw(this ISqlScalarQuery scalarQuery)
    {
        var (sql, context) = SqlCompiler.SqlServer.Compile(scalarQuery, new Context());
        return (sql, context.Parameters);
    }

    /// <summary>
    /// Compiles the SQL scalar query to SQLite syntax with parameters.
    /// </summary>
    /// <param name="scalarQuery">The SQL scalar query to compile</param>
    /// <returns>A tuple containing the SQL string and parameter dictionary</returns>
    /// <example>
    /// <code>
    /// var (sql, parameters) = countQuery.ToSqliteRaw();
    /// </code>
    /// </example>
    public static (string SqlRaw, ImmutableDictionary<string, object> Parameters) ToSqliteRaw(this ISqlScalarQuery scalarQuery)
    {
        var (sql, context) = SqlCompiler.Sqlite.Compile(scalarQuery, new Context());
        return (sql, context.Parameters);
    }
}
