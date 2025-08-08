using System.Collections.Immutable;
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
/// Interface for SQL table representations that support tuple-like indexing.
/// Extends ITuple to provide column access capabilities.
/// </summary>
public interface ISqlTable : ITuple
{ 
    string TableName { get; }
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
    private readonly object[] columns;

    protected SqlTable(string tableName)
    {
        TableName = tableName;
        columns = new object[2];
    }

    public object? this[int index] => columns[index];

    public TCol1 Column1(string columnName)
    {
        var column1 = TCol1.Create(TableName, columnName);
        columns[0] = column1;
        return column1;
    }
    
    public TCol2 Column2(string columnName)
    {
        var column2 = TCol2.Create(TableName, columnName);
        columns[1] = column2;
        return column2;
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
    private readonly object[] columns;

    protected SqlTable(string tableName)
    {
        TableName = tableName;
        columns = new object[3];
    }

    public object? this[int index] => columns[index];

    public TCol1 Column1(string columnName)
    {
        var column1 = TCol1.Create(TableName, columnName);
        columns[0] = column1;
        return column1;
    }
    
    public TCol2 Column2(string columnName)
    {
        var column2 = TCol2.Create(TableName, columnName);
        columns[1] = column2;
        return column2;
    }
    
    public TCol3 Column3(string columnName)
    {
        var column3 = TCol3.Create(TableName, columnName);
        columns[2] = column3;
        return column3;
    }

    public string TableName { get; }

    public int Length => columns.Length;
}



/// <summary>
/// Base record representing a SQL FROM clause.
/// Establishes the source table for a query.
/// </summary>
/// <param name="Table">The table being queried</param>
public abstract record FromClause(ISqlTable Table) : ISqlQuery;

/// <summary>
/// Strongly-typed FROM clause that preserves column type information.
/// Links a query to a specific table with known column structure.
/// </summary>
/// <typeparam name="TColumns">The tuple type representing the table's columns</typeparam>
/// <param name="Table">The table being queried</param>
public record FromClause<TColumns>(ISqlTable Table) : FromClause(Table), ISqlQuery<TColumns>
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
public record SelectClause<TSource, TResult>(ISqlQuery<TSource> TypedQuery, Func<TSource, TResult> TypedSelector) 
    : SelectClause(TypedQuery, x => TypedSelector((TSource) x)), ISqlQuery<TResult>
    where TSource : ITuple
    where TResult : ITuple;


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

public interface ISqlOrderedQuery : ISqlQuery;

public interface ISqlOrderedQuery<TSource> : ISqlOrderedQuery, ISqlQuery<TSource>;

/// <summary>
/// Base record representing a SQL ORDER BY clause for result sorting.
/// Defines the ordering criteria and direction for query results.
/// </summary>
/// <param name="Query">The source query to sort</param>
/// <param name="KeySelector">Function that extracts the sorting key from input tuples</param>
/// <param name="Descending">Whether to sort in descending order</param>
public abstract record OrderByClause(ISqlQuery Query, ImmutableArray<(Func<ITuple, SqlExpr> KeySelector, bool Descending)> KeySelectors) : ISqlOrderedQuery;

/// <summary>
/// Concrete implementation of ORDER BY clause for multiple sorting criteria.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <param name="TypedQuery">The strongly-typed source query</param>
/// <param name="KeySelectors">Array of key selectors and their sort directions</param>
public record OrderByClause<TSource>(ISqlQuery<TSource> TypedQuery, ImmutableArray<(Func<ITuple, SqlExpr> KeySelector, bool Descending)> KeySelectors) 
    : OrderByClause(TypedQuery, KeySelectors), ISqlOrderedQuery<TSource>
    where TSource : ITuple;

/// <summary>
/// Strongly-typed ORDER BY clause for sorting query results.
/// Provides type-safe sorting based on extracted key values.
/// </summary>
/// <typeparam name="TSource">The input tuple type from the source query</typeparam>
/// <typeparam name="TKey">The type of the sorting key, must be a SQL expression</typeparam>
/// <param name="TypedQuery">The strongly-typed source query</param>
/// <param name="TypedKeySelector">Function that extracts strongly-typed sorting keys from source tuples</param>
/// <param name="Descending">Whether to sort in descending order</param>
public record OrderByClause<TSource, TKey>(ISqlQuery<TSource> TypedQuery, Func<TSource, TKey> TypedKeySelector, bool Descending) 
    : OrderByClause<TSource>(TypedQuery, ImmutableArray.Create<(Func<ITuple, SqlExpr>, bool)>((x => TypedKeySelector((TSource) x), Descending)))
    where TSource : ITuple
    where TKey : SqlExpr;    