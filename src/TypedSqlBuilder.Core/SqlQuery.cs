using System.Runtime.CompilerServices;

namespace TypedSqlBuilder.Core;

/// <summary>
/// Base interface for all SQL query representations.
/// Provides the foundation for building type-safe SQL queries.
/// </summary>
public interface ISqlQuery;

/// <summary>
/// Generic interface for SQL queries that produce a specific result type.
/// Enables strongly-typed query composition and transformation.
/// </summary>
/// <typeparam name="TSource">The type of data produced by this query</typeparam>
public interface ISqlQuery<TSource> : ISqlQuery;

/// <summary>
/// Factory class for creating SQL queries.
/// Provides entry points for building type-safe SQL queries starting with FROM clauses.
/// </summary>
public static class SqlQuery
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
        where TSqlTable : SqlTable, new()
    {        
        return new FromClause<TSqlTable>(new TSqlTable());
    }
}

/// <summary>
/// Interface for SQL table representations that support tuple-like indexing.
/// Extends ITuple to provide column access capabilities.
/// </summary>
public interface ISqlTable : ITuple;

/// <summary>
/// Base record representing a SQL table with a name and column definitions.
/// Implements tuple indexing to access columns by position.
/// </summary>
/// <param name="TableName">The name of the table</param>
/// <param name="Columns">The tuple containing column definitions</param>
public record SqlTable(string TableName, ITuple Columns) : ISqlTable
{
    /// <summary>
    /// Gets the column at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the column</param>
    /// <returns>The column at the specified index</returns>
    public object? this[int index] => Columns[index];
    
    /// <summary>
    /// Gets the number of columns in this table.
    /// </summary>
    public int Length => Columns.Length;
}

/// <summary>
/// Abstract base class for strongly-typed SQL tables with two columns.
/// Provides a structured way to define tables with known column types.
/// </summary>
/// <typeparam name="TCol1">The type of the first column</typeparam>
/// <typeparam name="TCol2">The type of the second column</typeparam>
/// <param name="Name">The name of the table</param>
/// <param name="Column1">The first column definition</param>
/// <param name="Column2">The second column definition</param>
public abstract record SqlTable<TCol1, TCol2>(string Name, TCol1 Column1, TCol2 Column2) : SqlTable(Name, (Column1, Column2))
    where TCol1 : ISqlColumn
    where TCol2 : ISqlColumn;

/// <summary>
/// Abstract base class for strongly-typed SQL tables with three columns.
/// Provides a structured way to define tables with known column types.
/// </summary>
/// <typeparam name="TCol1">The type of the first column</typeparam>
/// <typeparam name="TCol2">The type of the second column</typeparam>
/// <typeparam name="TCol3">The type of the third column</typeparam>
/// <param name="Name">The name of the table</param>
/// <param name="Column1">The first column definition</param>
/// <param name="Column2">The second column definition</param>
/// <param name="Column3">The third column definition</param>
public abstract record SqlTable<TCol1, TCol2, TCol3>(string TableName, TCol1 Column1, TCol2 Column2, TCol3 Column3) : SqlTable(TableName, (Column1, Column2, Column3))
    where TCol1 : ISqlColumn
    where TCol2 : ISqlColumn
    where TCol3 : ISqlColumn;



/// <summary>
/// Base record representing a SQL FROM clause.
/// Establishes the source table for a query.
/// </summary>
/// <param name="Table">The table being queried</param>
public abstract record FromClause(SqlTable Table) : ISqlQuery;

/// <summary>
/// Strongly-typed FROM clause that preserves column type information.
/// Links a query to a specific table with known column structure.
/// </summary>
/// <typeparam name="TColumns">The tuple type representing the table's columns</typeparam>
/// <param name="Table">The table being queried</param>
public record FromClause<TColumns>(SqlTable Table) : FromClause(Table), ISqlQuery<TColumns>
    where TColumns : ITuple;

/// <summary>
/// Base record representing a SQL SELECT clause with tuple projection.
/// Transforms query results into different tuple structures.
/// </summary>
/// <param name="Query">The source query</param>
/// <param name="Selector">Function that transforms input tuples to output tuples</param>
public abstract record SelectClause(ISqlQuery Query, Func<ITuple, ITuple> Selector) : ISqlQuery;

/// <summary>
/// Strongly-typed SELECT clause for tuple projections.
/// Provides type-safe transformation from source to result columns.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <typeparam name="TResult">The output tuple type after projection</typeparam>
/// <param name="TypedQuery">The strongly-typed source query</param>
/// <param name="TypedSelector">Function that transforms source tuples to result tuples</param>
public record SelectClause<TSource, TResult>(ISqlQuery<TSource> TypedQuery, Func<TSource, TResult> TypedSelector) : SelectClause(TypedQuery, x => TypedSelector((TSource) x)), ISqlQuery<TResult>
    where TSource : ITuple
    where TResult : ITuple;

/// <summary>
/// Base record representing a SQL SELECT clause that projects to integer expressions.
/// Used for selecting computed integer values or preparing for aggregation.
/// </summary>
/// <param name="Query">The source query</param>
/// <param name="Selector">Function that transforms input tuples to SQL integer expressions</param>
public abstract record SelectSqlIntClause(ISqlQuery Query, Func<ITuple, SqlExprInt> Selector) : ISqlQuery;

/// <summary>
/// Strongly-typed SELECT clause for integer expressions.
/// Enables type-safe selection of computed integer values from source data.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <param name="TypedQuery">The strongly-typed source query</param>
/// <param name="TypedSelector">Function that transforms source tuples to SQL integer expressions</param>
public record SelectSqlIntClause<TSource>(ISqlQuery<TSource> TypedQuery, Func<TSource, SqlExprInt> TypedSelector) 
    : SelectSqlIntClause(TypedQuery, x => TypedSelector((TSource) x)), ISqlQuery<SqlExprInt>
    where TSource : ITuple;

/// <summary>
/// Base record representing a SQL WHERE clause with filtering conditions.
/// Applies boolean predicates to filter query results.
/// </summary>
/// <param name="Query">The source query to filter</param>
/// <param name="Predicate">Function that evaluates filtering conditions on input tuples</param>
public abstract record WhereClause(ISqlQuery Query, Func<ITuple, SqlExprBool> Predicate) : ISqlQuery;

/// <summary>
/// Strongly-typed WHERE clause for filtering query results.
/// Provides type-safe filtering based on boolean conditions.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <param name="TypedQuery">The strongly-typed source query</param>
/// <param name="TypedPredicate">Function that evaluates boolean conditions on source tuples</param>
public record WhereClause<TSource>(ISqlQuery<TSource> TypedQuery, Func<TSource, SqlExprBool> TypedPredicate) 
    : WhereClause(TypedQuery, x => TypedPredicate((TSource) x)), ISqlQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Represents a SQL SUM aggregate function applied to integer queries.
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
/// Base class for SQL COUNT aggregate functions.
/// Provides the foundation for counting operations that return integer values.
/// </summary>
public abstract class CountClause : SqlExprInt;

/// <summary>
/// Strongly-typed COUNT aggregate function for SQL queries.
/// Counts the number of rows in a query result and returns an integer expression.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <param name="Query">The query whose rows will be counted</param>
public class CountClause<TSource>(ISqlQuery<TSource> Query) : CountClause
    where TSource : ITuple
{
    /// <summary>
    /// Deconstructs the COUNT clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being counted</param>
    public void Deconstruct(out ISqlQuery<TSource> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Base record representing a SQL ORDER BY clause for result sorting.
/// Defines the ordering criteria and direction for query results.
/// </summary>
/// <param name="Query">The source query to sort</param>
/// <param name="KeySelector">Function that extracts the sorting key from input tuples</param>
/// <param name="Descending">Whether to sort in descending order</param>
public abstract record OrderByClause(ISqlQuery Query, Func<ITuple, SqlExpr> KeySelector, bool Descending) : ISqlQuery;

/// <summary>
/// Strongly-typed ORDER BY clause for sorting query results.
/// Provides type-safe sorting based on extracted key values.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <typeparam name="TKey">The type of the sorting key, must be a SQL expression</typeparam>
/// <param name="TypedQuery">The strongly-typed source query</param>
/// <param name="TypedKeySelector">Function that extracts strongly-typed sorting keys from source tuples</param>
/// <param name="Descending">Whether to sort in descending order</param>
public record OrderByClause<TSource, TKey>(ISqlQuery<TSource> TypedQuery, Func<TSource, TKey> TypedKeySelector, bool Descending) : OrderByClause(TypedQuery, x => TypedKeySelector((TSource) x), Descending), ISqlQuery<TSource>
    where TSource : ITuple
    where TKey : SqlExpr;

/// <summary>
/// Extension methods providing a fluent API for building SQL queries.
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
    /// Applies the COUNT aggregate function to count rows in the query result.
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
    public static ISqlQuery<TSource> OrderBy<TSource, TKey>(this ISqlQuery<TSource> query, Func<TSource, TKey> keySelector)
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
    public static ISqlQuery<TSource> OrderByDescending<TSource, TKey>(this ISqlQuery<TSource> query, Func<TSource, TKey> keySelector)
        where TSource : ITuple
        where TKey : SqlExpr
    {
        return new OrderByClause<TSource, TKey>(query, keySelector, true);
    }
}    