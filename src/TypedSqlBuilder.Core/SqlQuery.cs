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
/// Interface for SQL table representations that support tuple-like indexing.
/// Extends ITuple to provide column access capabilities.
/// </summary>
public interface ISqlTable : ITuple
{
    string TableName { get; }
    object?[] Columns { get; }
}


/// <summary>
/// Abstract base class for strongly-typed SQL tables with two columns.
/// Provides a structured way to define tables with known column types.
/// </summary>
/// <typeparam name="TCol1">The type of the first column</typeparam>
/// <typeparam name="TCol2">The type of the second column</typeparam>
public abstract class SqlTable<TCol1, TCol2> : ISqlTable 
    where TCol1 : ISqlColumn<TCol1>
    where TCol2 : ISqlColumn<TCol2>
{
    private readonly object?[] columns;
    public object?[] Columns => columns;

    protected SqlTable(string tableName)
    {
        TableName = tableName;
        columns = new object?[2];
    }

    public object? this[int index] => columns[index];

    public TCol1 Column1(string columnName)
    {
        if (columns[0] == null)
        {
            columns[0] = TCol1.Create(TableName, columnName);
        }
        return (TCol1)columns[0]!;
    }

    public TCol2 Column2(string columnName) 
    {
        if (columns[1] == null)
        {
            columns[1] = TCol2.Create(TableName, columnName);
        }
        return (TCol2)columns[1]!;
    }
     
    public string TableName { get; }

    public int Length => columns.Length;
}

/// <summary>
/// Abstract base class for strongly-typed SQL tables with three columns.
/// Provides a structured way to define tables with known column types.
/// </summary>
/// <typeparam name="TCol1">The type of the first column</typeparam>
/// <typeparam name="TCol2">The type of the second column</typeparam>
/// <typeparam name="TCol3">The type of the third column</typeparam>
public abstract class SqlTable<TCol1, TCol2, TCol3> : ISqlTable 
    where TCol1 : ISqlColumn<TCol1>
    where TCol2 : ISqlColumn<TCol2>
    where TCol3 : ISqlColumn<TCol3>
{
    private readonly object?[] columns;
    public object?[] Columns => columns;

    protected SqlTable(string tableName)
    {
        TableName = tableName;
        columns = new object?[3];
    }

    public object? this[int index] => columns[index];

    public TCol1 Column1(string columnName)
    {
        if (columns[0] == null)
        {
            columns[0] = TCol1.Create(TableName, columnName);
        }
        return (TCol1)columns[0]!;
    }

    public TCol2 Column2(string columnName) 
    {
        if (columns[1] == null)
        {
            columns[1] = TCol2.Create(TableName, columnName);
        }
        return (TCol2)columns[1]!;
    }

    public TCol3 Column3(string columnName) 
    {
        if (columns[2] == null)
        {
            columns[2] = TCol3.Create(TableName, columnName);
        }
        return (TCol3)columns[2]!;
    }
     
    public string TableName { get; }

    public int Length => columns.Length;
}



/// <summary>
/// Base record representing a SQL FROM clause.
/// Establishes the source table for a query.
/// </summary>
/// <param name="Table">The table being queried</param>
internal record FromTableClause(ISqlTable Table) : ISqlQuery;

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
internal record FromTableClause<TColumns>(ISqlTable Table) : FromTableClause(Table), ISqlQuery<TColumns>
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
/// Base record representing a SQL SELECT clause with tuple projection.
/// Transforms query results into different tuple structures.
/// </summary>
/// <param name="Query">The source query</param>
/// <param name="Selector">Function that transforms input tuples to output tuples</param>
internal record SelectClause(ISqlQuery Query, Func<ITuple, ITuple> Selector, ImmutableArray<string?> Aliases) : ISqlQuery;

/// <summary>
/// Strongly-typed SELECT clause for tuple projections.
/// Provides type-safe transformation from source to result columns.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <typeparam name="TResult">The output tuple type after projection</typeparam>
/// <param name="TypedQuery">The strongly-typed source query</param>
/// <param name="TypedSelector">Function that transforms source tuples to result tuples</param>
internal record SelectClause<TSource, TResult>(ISqlQuery<TSource> TypedQuery, Func<TSource, TResult> TypedSelector, ImmutableArray<string?> Aliases) 
    : SelectClause(TypedQuery, x => TypedSelector((TSource) x), Aliases), ISqlQuery<TResult>
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

public interface ISqlOrderedQuery : ISqlQuery;

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
/// <param name="KeySelectors">Array of functions that extract grouping keys from input tuples</param>
internal record GroupByClause(ISqlQuery Query, Func<ITuple, ImmutableArray<SqlExpr>> KeySelector, Func<ITuple, SqlAggregateFunc, SqlExprBool>? Predicate) : ISqlGroupByQuery;

/// <summary>
/// Strongly-typed GROUP BY clause for grouping query results.
/// Provides type-safe grouping based on extracted key values.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <param name="TypedQuery">The strongly-typed source query</param>
/// <param name="TypedKeySelector">Function that extracts an array of grouping keys from source tuples</param>
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


public enum JoinType
{
    Inner,
    Left,    
}

internal record JoinClause(ISqlQuery Outer, ImmutableArray<(JoinType JoinType, ISqlTable Inner, Func<ITuple, SqlExpr> OuterKeySelector, Func<ITuple, SqlExpr> InnerKeySelector, Func<ITuple, ITuple, ITuple> ResultSelector, ImmutableArray<string?> Aliases)> JoinData) : ISqlQuery;

internal record JoinClause<TOuter, TInner, TKey, TResult>(JoinType JoinType, ISqlQuery<TOuter> TypedOuter, TInner Inner, Func<TOuter, TKey> OuterKeySelector, Func<TInner, TKey> InnerKeySelector, Func<TOuter, TInner, TResult> ResultSelector, ImmutableArray<string?> Aliases) 
    : JoinClause(TypedOuter, [(JoinType, Inner, x => OuterKeySelector((TOuter)x), x => InnerKeySelector((TInner)x), (x, y) => ResultSelector((TOuter)x, (TInner)y), Aliases)]), ISqlQuery<TResult>
        where TOuter : ITuple
        where TInner : ISqlTable
        where TKey : SqlExpr
        where TResult : ITuple;