using System.Runtime.CompilerServices;

namespace TypedSqlBuilder.Core;

/// <summary>
/// Base interface for all SQL query representations in the TypedSqlBuilder system.
/// This interface provides the foundation for type-safe SQL query building.
/// </summary>
public interface ISqlQuery;

/// <summary>
/// Generic interface for SQL queries that produce a specific result type.
/// Enables type relationships for query composition.
/// </summary>
/// <typeparam name="TSource">The type of data that this query produces</typeparam>
public interface ISqlQuery<TSource> : ISqlQuery;

/// <summary>
/// Static factory class providing entry points for creating SQL queries.
/// Contains methods for initiating query chains with FROM clauses.
/// </summary>
public static class SqlQuery
{ 
    /// <summary>
    /// Creates a SQL query starting with a FROM clause using the provided column definitions.
    /// Uses a default table name of "table".
    /// </summary>
    /// <typeparam name="TColumns">The type representing the table's column structure</typeparam>
    /// <param name="columns">The column definitions for the table</param>
    /// <returns>A typed SQL query that can be further composed with other clauses</returns>
    /// <example>
    /// <code>
    /// var columns = (Id: new SqlIntColumn("users", "id"), Name: new SqlStringColumn("users", "name"));
    /// var query = SqlQuery.From(columns);
    /// </code>
    /// </example>
    public static ISqlQuery<TColumns> From<TColumns>(TColumns columns)
        where TColumns : ITuple
    {
        // For now, use a generic table name. In a real implementation, 
        // you might want to extract table names from the column definitions
        var table = new SqlTable<TColumns>("table", columns);
        return new FromClause<TColumns>(table);
    }
    
    /// <summary>
    /// Creates a SQL query starting with a FROM clause using the specified table name and column definitions.
    /// </summary>
    /// <typeparam name="TColumns">The type representing the table's column structure</typeparam>
    /// <param name="tableName">The name of the table to query from</param>
    /// <param name="columns">The column definitions for the table</param>
    /// <returns>A typed SQL query that can be further composed with other clauses</returns>
    /// <example>
    /// <code>
    /// var columns = (Id: new SqlIntColumn("users", "id"), Name: new SqlStringColumn("users", "name"));
    /// var query = SqlQuery.From("users", columns);
    /// </code>
    /// </example>
    public static ISqlQuery<TColumns> From<TColumns>(string tableName, TColumns columns)
        where TColumns : ITuple
    {
        var table = new SqlTable<TColumns>(tableName, columns);
        return new FromClause<TColumns>(table);
    }
}

/// <summary>
/// Abstract base record representing a SQL table reference.
/// Provides the foundation for all table representations in queries.
/// </summary>
/// <param name="Name">The name of the table</param>
public abstract record SqlTable(string Name) : ISqlQuery;

/// <summary>
/// Generic record representing a SQL table with strongly-typed column definitions.
/// Combines table metadata with type-safe column access.
/// </summary>
/// <typeparam name="TColumns">The type representing the table's column structure</typeparam>
/// <param name="Name">The name of the table</param>
/// <param name="Columns">The strongly-typed column definitions</param>
public record SqlTable<TColumns>(string Name, TColumns Columns) : SqlTable(Name), ISqlQuery<TColumns>
    where TColumns : ITuple;

/// <summary>
/// Abstract base record representing a SQL FROM clause.
/// Establishes the source table(s) for a query.
/// </summary>
public abstract record FromClause : ISqlQuery;

/// <summary>
/// Generic record representing a strongly-typed SQL FROM clause.
/// Links a query to a specific table with known column structure.
/// </summary>
/// <typeparam name="TColumns">The type representing the source table's column structure</typeparam>
/// <param name="Table">The table being queried from</param>
public record FromClause<TColumns>(SqlTable<TColumns> Table) : FromClause, ISqlQuery<TColumns>
    where TColumns : ITuple;

/// <summary>
/// Abstract base record representing a SQL SELECT clause that projects to tuple types.
/// Enables transformation of query results into different tuple structures.
/// </summary>
/// <param name="Query">The source query to select from</param>
/// <param name="Selector">Function that transforms input tuples to output tuples</param>
public abstract record SelectClause(ISqlQuery Query, Func<ITuple, ITuple> Selector) : ISqlQuery;

/// <summary>
/// Generic record representing a strongly-typed SQL SELECT clause for tuple projections.
/// Provides type-safe transformation from source columns to result columns.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <typeparam name="TResult">The output tuple type after projection</typeparam>
/// <param name="TypedQuery">The strongly-typed source query</param>
/// <param name="TypedSelector">Function that transforms source tuples to result tuples</param>
public record SelectClause<TSource, TResult>(ISqlQuery<TSource> TypedQuery, Func<TSource, TResult> TypedSelector) : SelectClause(TypedQuery, x => TypedSelector((TSource)x)), ISqlQuery<TResult>
    where TSource : ITuple
    where TResult : ITuple;

/// <summary>
/// Abstract base record representing a SQL SELECT clause that projects to integer expressions.
/// Used for selecting computed integer values or aggregate functions.
/// </summary>
/// <param name="Query">The source query to select from</param>
/// <param name="Selector">Function that transforms input tuples to SQL integer expressions</param>
public abstract record SelectSqlIntClause(ISqlQuery Query, Func<ITuple, SqlExprInt> Selector) : ISqlQuery;

/// <summary>
/// Generic record representing a strongly-typed SQL SELECT clause for integer expressions.
/// Enables type-safe selection of computed integer values from source data.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <param name="TypedQuery">The strongly-typed source query</param>
/// <param name="TypedSelector">Function that transforms source tuples to SQL integer expressions</param>
public record SelectSqlIntClause<TSource>(ISqlQuery<TSource> TypedQuery, Func<TSource, SqlExprInt> TypedSelector) 
    : SelectSqlIntClause(TypedQuery, x => TypedSelector((TSource)x)), ISqlQuery<SqlExprInt>
    where TSource : ITuple;

/// <summary>
/// Abstract base record representing a SQL WHERE clause with filtering predicates.
/// Applies boolean conditions to filter query results.
/// </summary>
/// <param name="Query">The source query to filter</param>
/// <param name="Predicate">Function that evaluates filtering conditions on input tuples</param>
public abstract record WhereClause(ISqlQuery Query, Func<ITuple, SqlExprBool> Predicate) : ISqlQuery;

/// <summary>
/// Generic record representing a strongly-typed SQL WHERE clause.
/// Provides type-safe filtering of query results based on boolean conditions.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <param name="TypedQuery">The strongly-typed source query</param>
/// <param name="TypedPredicate">Function that evaluates boolean conditions on source tuples</param>
public record WhereClause<TSource>(ISqlQuery<TSource> TypedQuery, Func<TSource, SqlExprBool> TypedPredicate) 
    : WhereClause(TypedQuery, x => TypedPredicate((TSource) x)), ISqlQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Class representing a SQL SUM aggregate function applied to integer queries.
/// Inherits from SqlExprInt to be used as an integer expression in larger queries.
/// </summary>
/// <param name="Query">The query containing integer values to sum</param>
public class SumSqlIntClause(ISqlQuery<SqlExprInt> Query) : SqlExprInt
{
    /// <summary>
    /// Deconstructs the SUM clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being summed</param>
    public void Deconstruct(out ISqlQuery<SqlExprInt> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Abstract base class representing a SQL COUNT aggregate function.
/// Provides the foundation for counting operations that return integer values.
/// </summary>
public abstract class CountClause : SqlExprInt;

/// <summary>
/// Generic class representing a SQL COUNT aggregate function for strongly-typed queries.
/// Counts the number of rows in a query result and returns an integer expression.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <param name="Query">The query whose rows will be counted</param>
public class CountClause<TSource>(ISqlQuery<TSource> Query) : SqlExprInt
    where TSource : ITuple
{
    /// <summary>
    /// Deconstructs the COUNT clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being counted</param>
    public void Deconstruct(out ISqlQuery<TSource> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Abstract base record representing a SQL ORDER BY clause for result sorting.
/// Defines the ordering criteria and direction for query results.
/// </summary>
/// <param name="Query">The source query to sort</param>
/// <param name="KeySelector">Function that extracts the sorting key from input tuples</param>
/// <param name="Descending">Whether to sort in descending order (true) or ascending order (false)</param>
public abstract record OrderByClause(ISqlQuery Query, Func<ITuple, SqlExpr> KeySelector, bool Descending) : ISqlQuery;

/// <summary>
/// Generic record representing a strongly-typed SQL ORDER BY clause.
/// Provides type-safe sorting of query results based on extracted key values.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <typeparam name="TKey">The type of the sorting key, must be a SQL expression</typeparam>
/// <param name="TypedQuery">The strongly-typed source query</param>
/// <param name="TypedKeySelector">Function that extracts strongly-typed sorting keys from source tuples</param>
/// <param name="Descending">Whether to sort in descending order (true) or ascending order (false)</param>
public record OrderByClause<TSource, TKey>(ISqlQuery<TSource> TypedQuery, Func<TSource, TKey> TypedKeySelector, bool Descending) : OrderByClause(TypedQuery, x => TypedKeySelector((TSource)x), Descending), ISqlQuery<TSource>
    where TSource : ITuple
    where TKey : SqlExpr;


/// <summary>
/// Extension methods that provide a fluent API for building SQL queries.
/// These methods enable method chaining to construct complex queries in a readable manner.
/// </summary>
public static class SqlQueryExtensions
{
    /// <summary>
    /// Adds a SELECT clause that projects the query result to a different tuple type.
    /// Enables transformation of query results while maintaining type safety.
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
    /// Adds a SELECT clause that projects the query result to a SQL integer expression.
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
    public static ISqlQuery<SqlExprInt> Select<TSource>(this ISqlQuery<TSource> query, Func<TSource, SqlExprInt> selector)
        where TSource : ITuple
    {
        return new SelectSqlIntClause<TSource>(query, selector);
    }

    /// <summary>
    /// Applies the SUM aggregate function to a query of integer expressions.
    /// Computes the total sum of all integer values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL integer expressions</param>
    /// <returns>A SQL integer expression representing the sum of all values</returns>
    /// <example>
    /// <code>
    /// var totalAge = userQuery.Select(user => user.Age).Sum();
    /// </code>
    /// </example>
    public static SqlExprInt Sum(this ISqlQuery<SqlExprInt> query)
    {
        return new SumSqlIntClause(query);
    }

    /// <summary>
    /// Applies the COUNT aggregate function to a query.
    /// Counts the number of rows in the query result.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <param name="query">The query whose rows will be counted</param>
    /// <returns>A SQL integer expression representing the count of rows</returns>
    /// <example>
    /// <code>
    /// var userCount = userQuery.Count();
    /// </code>
    /// </example>
    public static SqlExprInt Count<TSource>(this ISqlQuery<TSource> query)
        where TSource : ITuple
    {
        return new CountClause<TSource>(query);
    }

    /// <summary>
    /// Adds a WHERE clause that filters the query result based on a boolean condition.
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
    /// Adds an ORDER BY clause that sorts the query result in ascending order.
    /// Rows will be ordered based on the values returned by the key selector function.
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
    public static ISqlQuery<TSource> OrderBy<TSource, TKey>(this ISqlQuery<TSource> query, Func<TSource, TKey> keySelector)
        where TSource : ITuple
        where TKey : SqlExpr
    {
        return new OrderByClause<TSource, TKey>(query, keySelector, false);
    }

    /// <summary>
    /// Adds an ORDER BY clause that sorts the query result in descending order.
    /// Rows will be ordered based on the values returned by the key selector function, with highest values first.
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
    public static ISqlQuery<TSource> OrderByDescending<TSource, TKey>(this ISqlQuery<TSource> query, Func<TSource, TKey> keySelector)
        where TSource : ITuple
        where TKey : SqlExpr
    {
        return new OrderByClause<TSource, TKey>(query, keySelector, true);
    }
}    