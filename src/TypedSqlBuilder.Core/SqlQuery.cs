using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypedSqlBuilder.Core;

/// <summary>
/// Represents the sort direction for ORDER BY clauses.
/// </summary>
public enum Sort
{
    /// <summary>
    /// Sort in ascending order (smallest to largest).
    /// </summary>
    Asc,
    
    /// <summary>
    /// Sort in descending order (largest to smallest).
    /// </summary>
    Desc
}

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
public interface ISqlQuery<TSource> : ISqlQuery
    where TSource : ITuple;

/// <summary>
/// Base interface for SQL queries that return a single scalar value rather than a set of tuples.
/// Used for aggregate functions like COUNT, SUM, AVG, MAX, MIN that produce single values.
/// </summary>
public interface ISqlScalarQuery;

/// <summary>
/// Generic interface for SQL queries that return a single scalar value of a specific type.
/// Extends the base ISqlScalarQuery interface with type information.
/// </summary>
/// <typeparam name="T">The type of scalar SQL expression returned</typeparam>
public interface ISqlScalarQuery<T> : ISqlScalarQuery where T : SqlExpr;

/// <summary>
/// Base interface for SQL queries that include a GROUP BY clause.
/// Represents queries that group rows by one or more expressions.
/// </summary>
public interface ISqlGroupByQuery : ISqlQuery;

/// <summary>
/// Generic interface for SQL queries that include a GROUP BY clause with typed source data.
/// Enables strongly-typed grouping operations and aggregate function usage.
/// </summary>
/// <typeparam name="TSource">The type of data produced by the grouped query</typeparam>
public interface ISqlGroupByQuery<TSource> : ISqlGroupByQuery, ISqlQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Interface for SQL queries that include GROUP BY with ORDER BY clauses.
/// Represents grouped queries with additional ordering on the grouped results.
/// This enables queries that group data and then sort the groups themselves.
/// </summary>
/// <typeparam name="TSource">The type of data produced by the ordered grouped query</typeparam>
public interface ISqlOrderedGroupByQuery<TSource> : ISqlGroupByQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Interface for SQL queries that include both GROUP BY and HAVING clauses.
/// Represents grouped queries with additional filtering on grouped results.
/// </summary>
/// <typeparam name="TSource">The type of data produced by the grouped query</typeparam>
public interface ISqlGroupByHavingQuery<TSource> : ISqlGroupByQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Base record representing a SQL FROM clause.
/// Establishes the source table for a query.
/// </summary>
/// <param name="Table">The table being queried</param>
internal record FromTableClause(SqlTable Table) : ISqlQuery;

/// <summary>
/// Base record representing a SQL FROM clause with a subquery.
/// Establishes a subquery as the source for a query.
/// </summary>
/// <param name="Query">The subquery being used as the source</param>
internal record FromSubQueryClause(ISqlQuery Query) : ISqlQuery;

/// <summary>
/// Strongly-typed FROM clause that preserves column type information.
/// Links a query to a specific table with known column structure.
/// </summary>
/// <typeparam name="TColumns">The tuple type representing the table's columns</typeparam>
/// <param name="Table">The table being queried</param>
internal record FromTableClause<TColumns>(SqlTable Table) : FromTableClause(Table), ISqlQuery<TColumns>
    where TColumns : ITuple;

/// <summary>
/// Strongly-typed FROM clause with a subquery that preserves column type information.
/// Links a query to a specific subquery with known column structure.
/// </summary>
/// <typeparam name="TSource">The tuple type representing the subquery's columns</typeparam>
/// <param name="query">The subquery being used as the source</param>
internal record FromSubQueryClause<TSource>(ISqlQuery<TSource> query) : FromSubQueryClause(query), ISqlQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Represents a SQL DISTINCT clause to eliminate duplicate rows from query results.
/// Base record type for distinct operations.
/// </summary>
/// <param name="Query">The source query to apply DISTINCT to</param>
internal record DistinctClause(ISqlQuery Query) : ISqlQuery;

/// <summary>
/// Generic DISTINCT clause that maintains type safety for specific result types.
/// </summary>
/// <typeparam name="TSource">The type of data produced by the query</typeparam>
/// <param name="query">The source query to apply DISTINCT to</param>
internal record DistinctClause<TSource>(ISqlQuery<TSource> query) : DistinctClause(query), ISqlQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Represents a SQL LIMIT clause to restrict the number of rows returned.
/// Base record type for limit operations with optional offset.
/// </summary>
/// <param name="Query">The source query to apply LIMIT to</param>
/// <param name="Limit">The maximum number of rows to return</param>
/// <param name="Offset">The number of rows to skip before returning results (optional)</param>
internal record LimitClause(ISqlQuery Query, long Limit, long? Offset) : ISqlQuery;

/// <summary>
/// Generic LIMIT clause that maintains type safety for specific result types.
/// </summary>
/// <typeparam name="TSource">The type of data produced by the query</typeparam>
/// <param name="query">The source query to apply LIMIT to</param>
/// <param name="Limit">The maximum number of rows to return</param>
/// <param name="Offset">The number of rows to skip before returning results (optional)</param>
internal record LimitClause<TSource>(ISqlQuery<TSource> query, long Limit, long? Offset) : LimitClause(query, Limit, Offset), ISqlQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Base record representing a SQL SELECT clause with tuple projection.
/// Transforms query results into different tuple structures.
/// </summary>
/// <param name="Query">The source query</param>
/// <param name="Selector">Function that transforms input tuples to output tuples</param>
/// <param name="Aliases">Optional column aliases for the result</param>
/// <param name="IsDistinct">Whether to eliminate duplicate rows from the result</param>
/// <param name="LimitOffset">Optional limit/offset parameters (Limit: number of rows to return, Offset: optional number of rows to skip)</param>
internal record SelectClause(ISqlQuery Query, Func<ITuple, ITuple> Selector, ImmutableArray<string?> Aliases, bool IsDistinct = false, (long Limit, long? Offset)? LimitOffset = null) : ISqlQuery;

/// <summary>
/// Strongly-typed SELECT clause for tuple projections.
/// Provides type-safe transformation from source to result columns.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <typeparam name="TResult">The output tuple type after projection</typeparam>
/// <param name="TypedQuery">The strongly-typed source query</param>
/// <param name="TypedSelector">Function that transforms source tuples to result tuples</param>
/// <param name="Aliases">Optional column aliases for the result</param>
/// <param name="IsDistinct">Whether to eliminate duplicate rows from the result</param>
/// <param name="LimitOffset">Optional limit/offset parameters (Limit: number of rows to return, Offset: optional number of rows to skip)</param>
internal record SelectClause<TSource, TResult>(ISqlQuery<TSource> TypedQuery, Func<TSource, TResult> TypedSelector, ImmutableArray<string?> Aliases, bool IsDistinct = false, (long Limit, long? Offset)? LimitOffset = null) 
    : SelectClause(TypedQuery, x => TypedSelector((TSource) x), Aliases, IsDistinct, LimitOffset), ISqlQuery<TResult>
    where TSource : ITuple
    where TResult : ITuple;


/// <summary>
/// Base record representing a SQL WHERE clause with filtering predicate.
/// Applies boolean conditions to filter query results.
/// </summary>
/// <param name="Query">The source query to filter</param>
/// <param name="Predicate">Function that produces a boolean expression for filtering</param>
internal record WhereClause(ISqlQuery Query, Func<ITuple, SqlExprBool> Predicate) : ISqlQuery;

/// <summary>
/// Strongly-typed WHERE clause for filtering with type-safe predicates.
/// Provides compile-time safety for column references in filter conditions.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <param name="TypedQuery">The strongly-typed source query</param>
/// <param name="TypedPredicate">Function that produces type-safe boolean expressions for filtering</param>
internal record WhereClause<TSource>(ISqlQuery<TSource> TypedQuery, Func<TSource, SqlExprBool> TypedPredicate) 
    : WhereClause(TypedQuery, x => TypedPredicate((TSource) x)), ISqlQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Base record representing a SQL UNION operation between two queries.
/// Combines rows from two queries, removing duplicates.
/// </summary>
/// <param name="Query1">The first query</param>
/// <param name="Query2">The second query</param>
internal record UnionClause(ISqlQuery Query1, ISqlQuery Query2) : ISqlQuery;

/// <summary>
/// Strongly-typed UNION clause for combining queries with identical result types.
/// Provides type-safe union operations ensuring both queries return the same structure.
/// </summary>
/// <typeparam name="TSource">The tuple type that both queries must produce</typeparam>
/// <param name="TypedQuery1">The first strongly-typed query</param>
/// <param name="TypedQuery2">The second strongly-typed query</param>
internal record UnionClause<TSource>(ISqlQuery<TSource> TypedQuery1, ISqlQuery<TSource> TypedQuery2) 
    : UnionClause(TypedQuery1, TypedQuery2), ISqlQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Base record representing a SQL INTERSECT operation between two queries.
/// Returns only rows that exist in both queries.
/// </summary>
/// <param name="Query1">The first query</param>
/// <param name="Query2">The second query</param>
internal record IntersectClause(ISqlQuery Query1, ISqlQuery Query2) : ISqlQuery;

/// <summary>
/// Strongly-typed INTERSECT clause for finding common rows between queries.
/// Provides type-safe intersect operations ensuring both queries return the same structure.
/// </summary>
/// <typeparam name="TSource">The tuple type that both queries must produce</typeparam>
/// <param name="TypedQuery1">The first strongly-typed query</param>
/// <param name="TypedQuery2">The second strongly-typed query</param>
internal record IntersectClause<TSource>(ISqlQuery<TSource> TypedQuery1, ISqlQuery<TSource> TypedQuery2) 
    : IntersectClause(TypedQuery1, TypedQuery2), ISqlQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Base record representing a SQL EXCEPT operation between two queries.
/// Returns rows that exist in the first query but not in the second query.
/// </summary>
/// <param name="Query1">The first query</param>
/// <param name="Query2">The second query</param>
internal record ExceptClause(ISqlQuery Query1, ISqlQuery Query2) : ISqlQuery;

/// <summary>
/// Strongly-typed EXCEPT clause for finding rows unique to the first query.
/// Provides type-safe except operations ensuring both queries return the same structure.
/// </summary>
/// <typeparam name="TSource">The tuple type that both queries must produce</typeparam>
/// <param name="TypedQuery1">The first strongly-typed query</param>
/// <param name="TypedQuery2">The second strongly-typed query</param>
internal record ExceptClause<TSource>(ISqlQuery<TSource> TypedQuery1, ISqlQuery<TSource> TypedQuery2) 
    : ExceptClause(TypedQuery1, TypedQuery2), ISqlQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Abstract base class for SQL scalar queries that return single values.
/// Extends SqlExprInt to enable scalar queries to be used as expressions in other queries.
/// This allows subqueries that return single values to be embedded in larger expressions.
/// </summary>
/// <typeparam name="TExpr">The type of SQL expression this scalar query returns</typeparam>
public abstract class SqlScalarQuery<TExpr> : SqlExprInt, ISqlScalarQuery<TExpr>
    where TExpr : SqlExpr
{
}

/// <summary>
/// Represents a SQL SUM aggregate function applied to integer queries.
/// Inherits from SqlExprInt to be used as an integer expression in larger queries.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
/// <param name="Query">The query containing integer values to sum</param>
internal class SumSqlIntClause(ISqlQuery<ValueTuple<SqlExprInt>> Query) : SqlScalarQuery<SqlExprInt>
{
    /// <summary>
    /// Deconstructs the SUM clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being summed</param>
    public void Deconstruct(out ISqlQuery<ValueTuple<SqlExprInt>> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Represents a SQL AVG aggregate function applied to integer queries.
/// Inherits from SqlExprInt to be used as an integer expression in larger queries.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
/// <param name="Query">The query containing integer values to average</param>
internal class AvgSqlIntClause(ISqlQuery<ValueTuple<SqlExprInt>> Query) : SqlScalarQuery<SqlExprInt>
{
    /// <summary>
    /// Deconstructs the AVG clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being averaged</param>
    public void Deconstruct(out ISqlQuery<ValueTuple<SqlExprInt>> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Represents a SQL MIN aggregate function applied to integer queries.
/// Inherits from SqlExprInt to be used as an integer expression in larger queries.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
/// <param name="Query">The query containing integer values to find minimum of</param>
internal class MinSqlIntClause(ISqlQuery<ValueTuple<SqlExprInt>> Query) : SqlScalarQuery<SqlExprInt>
{
    /// <summary>
    /// Deconstructs the MIN clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being evaluated for minimum</param>
    public void Deconstruct(out ISqlQuery<ValueTuple<SqlExprInt>> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Represents a SQL MAX aggregate function applied to integer queries.
/// Inherits from SqlExprInt to be used as an integer expression in larger queries.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
/// <param name="Query">The query containing integer values to find maximum of</param>
internal class MaxSqlIntClause(ISqlQuery<ValueTuple<SqlExprInt>> Query) : SqlScalarQuery<SqlExprInt>
{
    /// <summary>
    /// Deconstructs the MAX clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being evaluated for maximum</param>
    public void Deconstruct(out ISqlQuery<ValueTuple<SqlExprInt>> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Represents a SQL SUM aggregate function applied to decimal queries.
/// Inherits from SqlExprDecimal to be used as a decimal expression in larger queries.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
/// <param name="Query">The query containing decimal values to sum</param>
internal class SumSqlDecimalClause(ISqlQuery<ValueTuple<SqlExprDecimal>> Query) : SqlScalarQuery<SqlExprDecimal>
{
    /// <summary>
    /// Deconstructs the SUM clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being summed</param>
    public void Deconstruct(out ISqlQuery<ValueTuple<SqlExprDecimal>> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Represents a SQL AVG aggregate function applied to decimal queries.
/// Inherits from SqlExprDecimal to be used as a decimal expression in larger queries.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
/// <param name="Query">The query containing decimal values to average</param>
internal class AvgSqlDecimalClause(ISqlQuery<ValueTuple<SqlExprDecimal>> Query) : SqlScalarQuery<SqlExprDecimal>
{
    /// <summary>
    /// Deconstructs the AVG clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being averaged</param>
    public void Deconstruct(out ISqlQuery<ValueTuple<SqlExprDecimal>> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Represents a SQL MIN aggregate function applied to decimal queries.
/// Inherits from SqlExprDecimal to be used as a decimal expression in larger queries.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
/// <param name="Query">The query containing decimal values to find minimum of</param>
internal class MinSqlDecimalClause(ISqlQuery<ValueTuple<SqlExprDecimal>> Query) : SqlScalarQuery<SqlExprDecimal>
{
    /// <summary>
    /// Deconstructs the MIN clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being evaluated for minimum</param>
    public void Deconstruct(out ISqlQuery<ValueTuple<SqlExprDecimal>> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Represents a SQL MAX aggregate function applied to decimal queries.
/// Inherits from SqlExprDecimal to be used as a decimal expression in larger queries.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
/// <param name="Query">The query containing decimal values to find maximum of</param>
internal class MaxSqlDecimalClause(ISqlQuery<ValueTuple<SqlExprDecimal>> Query) : SqlScalarQuery<SqlExprDecimal>
{
    /// <summary>
    /// Deconstructs the MAX clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being evaluated for maximum</param>
    public void Deconstruct(out ISqlQuery<ValueTuple<SqlExprDecimal>> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Represents a SQL SUM aggregate function applied to long queries.
/// Inherits from SqlExprLong to be used as a long expression in larger queries.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
/// <param name="Query">The query containing long values to sum</param>
internal class SumSqlLongClause(ISqlQuery<ValueTuple<SqlExprLong>> Query) : SqlScalarQuery<SqlExprLong>
{
    /// <summary>
    /// Deconstructs the SUM clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being summed</param>
    public void Deconstruct(out ISqlQuery<ValueTuple<SqlExprLong>> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Represents a SQL AVG aggregate function applied to long queries.
/// Inherits from SqlExprLong to be used as a long expression in larger queries.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
/// <param name="Query">The query containing long values to average</param>
internal class AvgSqlLongClause(ISqlQuery<ValueTuple<SqlExprLong>> Query) : SqlScalarQuery<SqlExprLong>
{
    /// <summary>
    /// Deconstructs the AVG clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being averaged</param>
    public void Deconstruct(out ISqlQuery<ValueTuple<SqlExprLong>> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Represents a SQL MIN aggregate function applied to long queries.
/// Inherits from SqlExprLong to be used as a long expression in larger queries.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
/// <param name="Query">The query containing long values to find minimum of</param>
internal class MinSqlLongClause(ISqlQuery<ValueTuple<SqlExprLong>> Query) : SqlScalarQuery<SqlExprLong>
{
    /// <summary>
    /// Deconstructs the MIN clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being evaluated for minimum</param>
    public void Deconstruct(out ISqlQuery<ValueTuple<SqlExprLong>> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Represents a SQL MAX aggregate function applied to long queries.
/// Inherits from SqlExprLong to be used as a long expression in larger queries.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
/// <param name="Query">The query containing long values to find maximum of</param>
internal class MaxSqlLongClause(ISqlQuery<ValueTuple<SqlExprLong>> Query) : SqlScalarQuery<SqlExprLong>
{
    /// <summary>
    /// Deconstructs the MAX clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being evaluated for maximum</param>
    public void Deconstruct(out ISqlQuery<ValueTuple<SqlExprLong>> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Represents a SQL SUM aggregate function applied to double queries.
/// Inherits from SqlExprDouble to be used as a double expression in larger queries.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
/// <param name="Query">The query containing double values to sum</param>
internal class SumSqlDoubleClause(ISqlQuery<ValueTuple<SqlExprDouble>> Query) : SqlScalarQuery<SqlExprDouble>
{
    /// <summary>
    /// Deconstructs the SUM clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being summed</param>
    public void Deconstruct(out ISqlQuery<ValueTuple<SqlExprDouble>> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Represents a SQL AVG aggregate function applied to double queries.
/// Inherits from SqlExprDouble to be used as a double expression in larger queries.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
/// <param name="Query">The query containing double values to average</param>
internal class AvgSqlDoubleClause(ISqlQuery<ValueTuple<SqlExprDouble>> Query) : SqlScalarQuery<SqlExprDouble>
{
    /// <summary>
    /// Deconstructs the AVG clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being averaged</param>
    public void Deconstruct(out ISqlQuery<ValueTuple<SqlExprDouble>> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Represents a SQL MIN aggregate function applied to double queries.
/// Inherits from SqlExprDouble to be used as a double expression in larger queries.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
/// <param name="Query">The query containing double values to find minimum of</param>
internal class MinSqlDoubleClause(ISqlQuery<ValueTuple<SqlExprDouble>> Query) : SqlScalarQuery<SqlExprDouble>
{
    /// <summary>
    /// Deconstructs the MIN clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being evaluated for minimum</param>
    public void Deconstruct(out ISqlQuery<ValueTuple<SqlExprDouble>> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Represents a SQL MAX aggregate function applied to double queries.
/// Inherits from SqlExprDouble to be used as a double expression in larger queries.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
/// <param name="Query">The query containing double values to find maximum of</param>
internal class MaxSqlDoubleClause(ISqlQuery<ValueTuple<SqlExprDouble>> Query) : SqlScalarQuery<SqlExprDouble>
{
    /// <summary>
    /// Deconstructs the MAX clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being evaluated for maximum</param>
    public void Deconstruct(out ISqlQuery<ValueTuple<SqlExprDouble>> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Base class for SQL COUNT aggregate functions.
/// Provides the foundation for counting operations that return integer scalar values.
/// Implements ISqlScalarQuery to indicate it returns a single scalar value.
/// </summary>
internal abstract class CountClause(ISqlQuery Query) : SqlScalarQuery<SqlExprInt>
{
    /// <summary>
    /// Deconstructs the COUNT clause to extract the underlying query.
    /// </summary>
    /// <param name="QueryOut">The query being counted</param>
    public void Deconstruct(out ISqlQuery QueryOut) => QueryOut = Query;
}

/// <summary>
/// Strongly-typed COUNT aggregate function for SQL queries.
/// Counts the number of rows in a query result and returns an integer expression.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <param name="Query">The query whose rows will be counted</param>
internal class CountClause<TSource>(ISqlQuery<TSource> Query) : CountClause(Query)
    where TSource : ITuple
{
    /// <summary>
    /// Deconstructs the COUNT clause to extract the underlying typed query.
    /// </summary>
    /// <param name="QueryOut">The query being counted</param>
    public void Deconstruct(out ISqlQuery<TSource> QueryOut) => QueryOut = Query;
}

/// <summary>
/// Base interface for SQL queries that include an ORDER BY clause.
/// Represents queries that have specified row ordering but maintain their source structure.
/// </summary>
public interface ISqlOrderedQuery : ISqlQuery;

/// <summary>
/// Generic interface for SQL queries that include an ORDER BY clause with typed source data.
/// Enables strongly-typed ordering operations while preserving result type information.
/// </summary>
/// <typeparam name="TSource">The type of data produced by the ordered query</typeparam>
public interface ISqlOrderedQuery<TSource> : ISqlOrderedQuery, ISqlQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Base record representing a SQL ORDER BY clause for result sorting.
/// Defines the ordering criteria and direction for query results.
/// </summary>
/// <param name="Query">The source query to sort</param>
/// <param name="KeySelectors">Function that extracts sorting keys with their directions from input tuples</param>
internal record OrderByClause(ISqlQuery Query, Func<ITuple, ImmutableArray<(SqlExpr, Sort)>> KeySelectors) : ISqlOrderedQuery;


/// <summary>
/// Strongly-typed ORDER BY clause for sorting query results.
/// Provides type-safe sorting based on extracted key values and directions.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <param name="TypedQuery">The strongly-typed source query</param>
/// <param name="TypedKeySelector">Function that extracts strongly-typed sorting keys with directions from source tuples</param>
internal record OrderByClause<TSource>(ISqlQuery<TSource> TypedQuery, Func<TSource, ImmutableArray<(SqlExpr, Sort)>> TypedKeySelector) 
    : OrderByClause(TypedQuery, x => TypedKeySelector((TSource)x)), ISqlOrderedGroupByQuery<TSource>, ISqlQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Base record representing a SQL GROUP BY clause for result grouping.
/// Defines the grouping criteria for aggregating query results.
/// </summary>
/// <param name="Query">The source query to group</param>
/// <param name="KeySelector">Function that extracts an array of grouping keys from input tuples</param>
/// <param name="Predicate">Optional predicate function for HAVING clause filtering</param>
internal record GroupByClause(ISqlQuery Query, Func<ITuple, ImmutableArray<SqlExpr>> KeySelector, Func<ITuple, SqlAggregateFunc, SqlExprBool>? Predicate) : ISqlGroupByQuery;

/// <summary>
/// Strongly-typed GROUP BY clause for grouping query results.
/// Provides type-safe grouping based on extracted key values.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <param name="TypedQuery">The strongly-typed source query</param>
/// <param name="TypedKeySelector">Function that extracts an array of grouping keys from source tuples</param>
/// <param name="TypedPredicate">Optional strongly-typed predicate function for HAVING clause filtering</param>
internal record GroupByClause<TSource>(ISqlQuery<TSource> TypedQuery, Func<TSource, ImmutableArray<SqlExpr>> TypedKeySelector,
                                                                    Func<TSource, SqlAggregateFunc, SqlExprBool>? TypedPredicate = null) 
    : GroupByClause(TypedQuery, x => TypedKeySelector((TSource)x), TypedPredicate is { } ? ((x, agg) => TypedPredicate((TSource)x, agg)) : null), ISqlGroupByQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Base record representing a SQL HAVING clause for filtering grouped query results.
/// Applies aggregate-based predicates to filter grouped data.
/// </summary>
/// <param name="GroupedQuery">The grouped query to filter</param>
/// <param name="Predicate">Function that evaluates aggregate-based filtering conditions on input tuples</param>
internal record HavingClause(ISqlQuery GroupedQuery, Func<ITuple, SqlAggregateFunc, SqlExprBool> Predicate) 
    : ISqlQuery;

/// <summary>
/// Strongly-typed HAVING clause for filtering grouped query results.
/// Provides type-safe aggregate-based filtering.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source grouped query</typeparam>
/// <param name="TypedGroupedQuery">The grouped query to filter</param>
/// <param name="TypedPredicate">Function that evaluates aggregate-based filtering conditions</param>
internal record HavingClause<TSource>(ISqlGroupByQuery<TSource> TypedGroupedQuery, Func<TSource, SqlAggregateFunc, SqlExprBool> TypedPredicate) 
    : HavingClause(TypedGroupedQuery, (x, agg) => TypedPredicate((TSource) x, agg)), ISqlGroupByHavingQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Enumeration of supported SQL JOIN types.
/// Defines the different ways tables can be joined in SQL queries.
/// </summary>
public enum JoinType
{
    /// <summary>
    /// INNER JOIN - Returns only rows that have matching values in both tables.
    /// </summary>
    Inner,
    
    /// <summary>
    /// LEFT OUTER JOIN - Returns all rows from the left table and matched rows from the right table.
    /// </summary>
    Left,    
}

/// <summary>
/// Base record representing a SQL JOIN clause for combining multiple tables.
/// Handles the non-typed aspects of join operations.
/// </summary>
/// <param name="Outer">The outer (left) query being joined</param>
/// <param name="JoinData">Array containing join specifications: join type, inner table, key selectors, result transformation, and column aliases</param>
internal record JoinClause(ISqlQuery Outer, ImmutableArray<(JoinType JoinType, SqlTable Inner, Func<ITuple, SqlExpr> OuterKeySelector, Func<ITuple, SqlExpr> InnerKeySelector, Func<ITuple, ITuple, ITuple> ResultSelector, ImmutableArray<string?> Aliases)> JoinData) : ISqlQuery;

/// <summary>
/// Strongly-typed JOIN clause for combining queries with type safety.
/// Provides compile-time verification of join keys and result structure.
/// </summary>
/// <typeparam name="TOuter">The tuple type of the outer (left) query</typeparam>
/// <typeparam name="TInner">The tuple type of the inner (right) table</typeparam>
/// <typeparam name="TKey">The type of the join key used for matching rows</typeparam>
/// <typeparam name="TResult">The tuple type of the joined result</typeparam>
/// <param name="JoinType">The type of join operation to perform</param>
/// <param name="TypedOuter">The strongly-typed outer query</param>
/// <param name="Inner">The inner table to join with</param>
/// <param name="OuterKeySelector">Function to extract the join key from outer query rows</param>
/// <param name="InnerKeySelector">Function to extract the join key from inner table rows</param>
/// <param name="ResultSelector">Function to combine outer and inner rows into the result type</param>
/// <param name="Aliases">Optional column aliases for the result</param>
internal record JoinClause<TOuter, TInner, TKey, TResult>(JoinType JoinType, ISqlQuery<TOuter> TypedOuter, TInner Inner, Func<TOuter, TKey> OuterKeySelector, Func<TInner, TKey> InnerKeySelector, Func<TOuter, TInner, TResult> ResultSelector, ImmutableArray<string?> Aliases) 
    : JoinClause(TypedOuter, [(JoinType, Inner, x => OuterKeySelector((TOuter)x), x => InnerKeySelector((TInner)x), (x, y) => ResultSelector((TOuter)x, (TInner)y), Aliases)]), ISqlQuery<TResult>
        where TOuter : ITuple
        where TInner : SqlTable
        where TKey : SqlExpr
        where TResult : ITuple;