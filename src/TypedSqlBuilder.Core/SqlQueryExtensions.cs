using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TypedSqlBuilder.Core;

/// <summary>
/// Extension methods providing a fluent API for building SQL queries and compiling SQL.
/// Enables method chaining to construct complex queries in a readable manner.
/// </summary>
public static class SqlQueryExtensions
{
    private static ImmutableArray<string?> GetTupleElementNames(Delegate @delegate)
    {
        var method = @delegate.Method;
        // Prefer ReturnParameter custom attributes which reliably carry TupleElementNamesAttribute
        var tupleNamesAttr = method.ReturnParameter?.GetCustomAttributes(typeof(TupleElementNamesAttribute), false)
            .Cast<TupleElementNamesAttribute>()
            .FirstOrDefault();

        return tupleNamesAttr?.TransformNames?.ToImmutableArray() ?? [];
    }


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
        // Capture tuple element names (if any) from the selector's return type
        var names = GetTupleElementNames(selector);
        return new SelectClause<TSource, TResult>(query, selector, names);
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
        return new SelectClause<TSource, ValueTuple<SqlExprInt>>(query, x => ValueTuple.Create(selector(x)), ImmutableArray<string?>.Empty);
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
        return new SelectClause<TSource, ValueTuple<SqlExprBool>>(query, x => ValueTuple.Create(selector(x)), ImmutableArray<string?>.Empty);
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
        return new SelectClause<TSource, ValueTuple<SqlExprString>>(query, x => ValueTuple.Create(selector(x)), ImmutableArray<string?>.Empty);
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
    public static SqlScalarQuery<SqlExprInt> Sum(this ISqlQuery<ValueTuple<SqlExprInt>> query)
    {
        return new SumSqlIntClause(query);
    }

    /// <summary>
    /// Applies the AVG aggregate function to a query of integer expressions.
    /// Computes the average of all integer values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL integer expressions</param>
    /// <returns>A SQL scalar query representing the average of all values</returns>
    /// <example>
    /// <code>
    /// var avgAge = userQuery.Select(user => user.Age).Avg();
    /// </code>
    /// </example>
    public static SqlScalarQuery<SqlExprInt> Avg(this ISqlQuery<ValueTuple<SqlExprInt>> query)
    {
        return new AvgSqlIntClause(query);
    }

    /// <summary>
    /// Applies the MIN aggregate function to a query of integer expressions.
    /// Finds the minimum value among all integer values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL integer expressions</param>
    /// <returns>A SQL scalar query representing the minimum value</returns>
    /// <example>
    /// <code>
    /// var minAge = userQuery.Select(user => user.Age).Min();
    /// </code>
    /// </example>
    public static SqlScalarQuery<SqlExprInt> Min(this ISqlQuery<ValueTuple<SqlExprInt>> query)
    {
        return new MinSqlIntClause(query);
    }

    /// <summary>
    /// Applies the MAX aggregate function to a query of integer expressions.
    /// Finds the maximum value among all integer values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL integer expressions</param>
    /// <returns>A SQL scalar query representing the maximum value</returns>
    /// <example>
    /// <code>
    /// var maxAge = userQuery.Select(user => user.Age).Max();
    /// </code>
    /// </example>
    public static SqlScalarQuery<SqlExprInt> Max(this ISqlQuery<ValueTuple<SqlExprInt>> query)
    {
        return new MaxSqlIntClause(query);
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
    public static SqlScalarQuery<SqlExprInt> Count<TSource>(this ISqlQuery<TSource> query)
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
    /// Sorts the query result based on the specified key and direction.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <param name="query">The source query to sort</param>
    /// <param name="keySelector">Function that extracts the sorting key and direction from source tuples</param>
    /// <returns>A new query with the specified sort order</returns>
    /// <example>
    /// <code>
    /// var sortedUsers = userQuery.OrderBy(user => (user.Name, Sort.Asc));
    /// var sortedUsersDesc = userQuery.OrderBy(user => (user.Age, Sort.Desc));
    /// </code>
    /// </example>
    public static ISqlQuery<TSource> OrderBy<TSource>(this ISqlQuery<TSource> query, Func<TSource, (SqlExpr Key, Sort Direction)> keySelector)
        where TSource : ITuple
    {
        return new OrderByClause<TSource>(query, x => [keySelector(x)]);
    }    

    /// <summary>
    /// Sorts the query result by multiple keys with their respective directions.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <param name="query">The source query to sort</param>
    /// <param name="keySelector">Function that extracts two sorting keys with their directions from source tuples</param>
    /// <returns>A new query with the specified multi-key sort order</returns>
    /// <example>
    /// <code>
    /// var sortedUsers = userQuery.OrderBy(user => ((user.Name, Sort.Asc), (user.Age, Sort.Desc)));
    /// </code>
    /// </example>
    public static ISqlQuery<TSource> OrderBy<TSource>(this ISqlQuery<TSource> query, Func<TSource, ((SqlExpr Key, Sort Direction), (SqlExpr Key, Sort Direction))> keySelector)
        where TSource : ITuple
    {
        return new OrderByClause<TSource>(query, x =>
        {
            var (key1, key2) = keySelector(x);
            return [key1, key2];
        });
    }    

    /// <summary>
    /// Sorts the query result by three keys with their respective directions.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <param name="query">The source query to sort</param>
    /// <param name="keySelector">Function that extracts three sorting keys with their directions from source tuples</param>
    /// <returns>A new query with the specified three-key sort order</returns>
    /// <example>
    /// <code>
    /// var sortedUsers = userQuery.OrderBy(user => ((user.Name, Sort.Asc), (user.Age, Sort.Desc), (user.Id, Sort.Asc)));
    /// </code>
    /// </example>
    public static ISqlQuery<TSource> OrderBy<TSource>(this ISqlQuery<TSource> query, Func<TSource, ((SqlExpr Key, Sort Direction), (SqlExpr Key, Sort Direction), (SqlExpr Key, Sort Direction))> keySelector)
        where TSource : ITuple
    {
        return new OrderByClause<TSource>(query, x =>
        {
            var (key1, key2, key3) = keySelector(x);
            return [key1, key2, key3];
        });
    }    

    /// <summary>
    /// Groups query results by a single key expression.
    /// Creates a grouped query that can be used with aggregate functions.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <param name="query">The source query to group</param>
    /// <param name="keySelector">Function that extracts the grouping key from source tuples</param>
    /// <returns>A grouped query that supports aggregate operations</returns>
    /// <example>
    /// <code>
    /// var groupedByAge = userQuery.GroupBy(user => user.Age);
    /// </code>
    /// </example>
    public static ISqlGroupByQuery<TSource> GroupBy<TSource>(this ISqlQuery<TSource> query, Func<TSource, SqlExpr> keySelector)
        where TSource : ITuple
    {
        return new GroupByClause<TSource>(query, x => [keySelector(x)]);
    }

    /// <summary>
    /// Groups query results by multiple key expressions.
    /// Creates a grouped query using a tuple of grouping keys.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <param name="query">The source query to group</param>
    /// <param name="keySelector">Function that extracts multiple grouping keys as a tuple</param>
    /// <returns>A grouped query that supports aggregate operations</returns>
    /// <example>
    /// <code>
    /// var groupedByRegionAndYear = salesQuery.GroupBy(sale => (sale.Region, sale.Year));
    /// </code>
    /// </example>
    public static ISqlGroupByQuery<TSource> GroupBy<TSource>(this ISqlQuery<TSource> query, Func<TSource, (SqlExpr, SqlExpr)> keySelector)
        where TSource : ITuple
    {
        return new GroupByClause<TSource>(query, x => {
            var (key1, key2) = keySelector(x);
            return [key1, key2];
        });
    }

    /// <summary>
    /// Adds a HAVING clause to filter grouped query results.
    /// Applies aggregate-based predicates to filter groups.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source grouped query</typeparam>
    /// <param name="query">The grouped query to filter</param>
    /// <param name="predicate">Function that creates aggregate-based filter conditions</param>
    /// <returns>A grouped query with HAVING clause that supports aggregate operations</returns>
    /// <example>
    /// <code>
    /// var highValueGroups = groupedQuery.Having((row, agg) => agg.Sum(row.Amount) > 1000);
    /// </code>
    /// </example>
    public static ISqlGroupByHavingQuery<TSource> Having<TSource>(this ISqlGroupByQuery<TSource> query, Func<TSource, SqlAggregateFunc, SqlExprBool> predicate)
        where TSource : ITuple
    {
        return new HavingClause<TSource>(query, predicate);
    }

    /// <summary>
    /// Adds ORDER BY clause to a grouped query with a single ordering key.
    /// Enables sorting of grouped results by aggregate expressions or grouping keys.
    /// </summary>
    /// <typeparam name="TSource">The source tuple type</typeparam>
    /// <param name="query">The grouped query to add ordering to</param>
    /// <param name="keySelector">Function that selects the ordering key and direction using aggregate functions</param>
    /// <returns>An ordered grouped query that can be further extended</returns>
    public static ISqlOrderedGroupByQuery<TSource> OrderBy<TSource>(this ISqlGroupByQuery<TSource> query, Func<TSource, SqlAggregateFunc, (SqlExpr Key, Sort Direction)> keySelector)
        where TSource : ITuple
    {
        return new OrderByClause<TSource>(query, x => [keySelector(x, new SqlAggregateFunc())]);
    }

    /// <summary>
    /// Adds ORDER BY clause to a grouped query with two ordering keys.
    /// Enables sorting of grouped results by multiple aggregate expressions or grouping keys.
    /// </summary>
    /// <typeparam name="TSource">The source tuple type</typeparam>
    /// <param name="query">The grouped query to add ordering to</param>
    /// <param name="keySelector">Function that selects two ordering keys and their directions using aggregate functions</param>
    /// <returns>An ordered grouped query that can be further extended</returns>
    public static ISqlOrderedGroupByQuery<TSource> OrderBy<TSource>(this ISqlGroupByQuery<TSource> query, Func<TSource, SqlAggregateFunc, ((SqlExpr Key, Sort Direction), (SqlExpr Key, Sort Direction))> keySelector)
        where TSource : ITuple
    {
        return new OrderByClause<TSource>(query, x =>
        {
            var (key1, key2) = keySelector(x, new SqlAggregateFunc());
            return [key1, key2];
        });
    }

    /// <summary>
    /// Adds ORDER BY clause to a grouped query with three ordering keys.
    /// Enables sorting of grouped results by multiple aggregate expressions or grouping keys.
    /// </summary>
    /// <typeparam name="TSource">The source tuple type</typeparam>
    /// <param name="query">The grouped query to add ordering to</param>
    /// <param name="keySelector">Function that selects three ordering keys and their directions using aggregate functions</param>
    /// <returns>An ordered grouped query that can be further extended</returns>
    public static ISqlOrderedGroupByQuery<TSource> OrderBy<TSource>(this ISqlGroupByQuery<TSource> query, Func<TSource, SqlAggregateFunc, ((SqlExpr Key, Sort Direction), (SqlExpr Key, Sort Direction), (SqlExpr Key, Sort Direction))> keySelector)
        where TSource : ITuple
    {
        return new OrderByClause<TSource>(query, x =>
        {
            var (key1, key2, key3) = keySelector(x, new SqlAggregateFunc());
            return [key1, key2, key3];
        });
    }

    /// <summary>
    /// Applies a SELECT clause to a grouped query with aggregate functions.
    /// Transforms grouped results using aggregate operations.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source grouped query</typeparam>
    /// <typeparam name="TResult">The output tuple type after projection with aggregates</typeparam>
    /// <param name="query">The grouped query to project from</param>
    /// <param name="selector">Function that transforms source tuples using aggregate functions</param>
    /// <returns>A query that produces the projected result with aggregates</returns>
    /// <example>
    /// <code>
    /// var results = groupedQuery.Select((row, agg) => (row.Category, Total: agg.Sum(row.Amount)));
    /// </code>
    /// </example>
    public static ISqlQuery<TResult> Select<TSource, TResult>(this ISqlGroupByQuery<TSource> query, Func<TSource, SqlAggregateFunc, TResult> selector)
        where TSource : ITuple
        where TResult : ITuple
    {
        // Capture tuple element names from the original selector that constructs the tuple
        var names = GetTupleElementNames(selector);
        return new SelectClause<TSource, TResult>(query, t => selector(t, new SqlAggregateFunc()), names);
    }

    /// <summary>
    /// Performs an INNER JOIN between the outer query and an inner table.
    /// Returns only rows where there are matching keys in both the outer query and inner table.
    /// </summary>
    /// <typeparam name="TOuter">The tuple type from the outer query</typeparam>
    /// <typeparam name="TInner">The type of the inner table to join with</typeparam>
    /// <typeparam name="TKey">The type of the join key, must be a SQL expression</typeparam>
    /// <typeparam name="TResult">The result tuple type after joining</typeparam>
    /// <param name="outer">The outer query to join from</param>
    /// <param name="inner">The inner table to join with</param>
    /// <param name="outerKeySelector">Function to extract the join key from the outer query</param>
    /// <param name="innerKeySelector">Function to extract the join key from the inner table</param>
    /// <param name="resultSelector">Function to combine outer and inner rows into the result</param>
    /// <returns>A new query representing the INNER JOIN operation</returns>
    /// <example>
    /// <code>
    /// var joinedQuery = customerQuery.InnerJoin(
    ///     Db.Orders,
    ///     c => c.Id,
    ///     o => o.CustomerId,
    ///     (c, o) => (c.Name, o.OrderId, o.Amount));
    /// </code>
    /// </example>
    public static ISqlQuery<TResult> InnerJoin<TOuter, TInner, TKey, TResult>(this ISqlQuery<TOuter> outer, TInner inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        where TOuter : ITuple
        where TInner : SqlTable, new()
        where TKey : SqlExpr
        where TResult : ITuple        
    {
        var names = GetTupleElementNames(resultSelector); 
        return new JoinClause<TOuter, TInner, TKey, TResult>(JoinType.Inner, outer, new TInner(), outerKeySelector, innerKeySelector, resultSelector, names);
    }

    /// <summary>
    /// Performs a LEFT JOIN between the outer query and an inner table.
    /// Returns all rows from the outer query and matching rows from the inner table.
    /// If no match is found, NULL values are used for the inner table columns.
    /// </summary>
    /// <typeparam name="TOuter">The tuple type from the outer query</typeparam>
    /// <typeparam name="TInner">The type of the inner table to join with</typeparam>
    /// <typeparam name="TKey">The type of the join key, must be a SQL expression</typeparam>
    /// <typeparam name="TResult">The result tuple type after joining</typeparam>
    /// <param name="outer">The outer query to join from</param>
    /// <param name="inner">The inner table to join with</param>
    /// <param name="outerKeySelector">Function to extract the join key from the outer query</param>
    /// <param name="innerKeySelector">Function to extract the join key from the inner table</param>
    /// <param name="resultSelector">Function to combine outer and inner rows into the result</param>
    /// <returns>A new query representing the LEFT JOIN operation</returns>
    /// <example>
    /// <code>
    /// var joinedQuery = customerQuery.LeftJoin(
    ///     Db.Orders,
    ///     c => c.Id,
    ///     o => o.CustomerId,
    ///     (c, o) => (c.Name, o.OrderId, o.Amount));
    /// </code>
    /// </example>
    public static ISqlQuery<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this ISqlQuery<TOuter> outer, TInner inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        where TOuter : ITuple
        where TInner : SqlTable, new()
        where TKey : SqlExpr
        where TResult : ITuple        
    {
        var names = GetTupleElementNames(resultSelector); 
        return new JoinClause<TOuter, TInner, TKey, TResult>(JoinType.Left, outer, new TInner(), outerKeySelector, innerKeySelector, resultSelector, names);
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
        var context = new Context { Dialect = SqlDialect.SqlServer };
        var (sql, _, resultContext) = SqlCompiler.Compile(query, context, 0);
        return (sql, resultContext.Parameters);
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
        var context = new Context { Dialect = SqlDialect.SQLite };
        var (sql, _, resultContext) = SqlCompiler.Compile(query, context, 0);
        return (sql, resultContext.Parameters);
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
        var context = new Context { Dialect = SqlDialect.SqlServer };
        var (sql, resultContext) = SqlCompiler.Compile(statement, context, 0);
        return (sql, resultContext.Parameters);
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
        var context = new Context { Dialect = SqlDialect.SQLite };
        var (sql, resultContext) = SqlCompiler.Compile(statement, context, 0);
        return (sql, resultContext.Parameters);
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
        var context = new Context { Dialect = SqlDialect.SqlServer };
        var (sql, resultContext) = SqlCompiler.Compile(scalarQuery, context, 0);
        return (sql, resultContext.Parameters);
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
        var context = new Context { Dialect = SqlDialect.SQLite };
        var (sql, resultContext) = SqlCompiler.Compile(scalarQuery, context, 0);
        return (sql, resultContext.Parameters);
    }

    /// <summary>
    /// Compiles the SQL query to PostgreSQL syntax with parameters.
    /// </summary>
    /// <param name="query">The SQL query to compile</param>
    /// <returns>A tuple containing the SQL string and parameter dictionary</returns>
    /// <example>
    /// <code>
    /// var (sql, parameters) = query.ToPostgreSqlRaw();
    /// </code>
    /// </example>
    public static (string SqlRaw, ImmutableDictionary<string, object> Parameters) ToPostgreSqlRaw(this ISqlQuery query)
    {
        var context = new Context { Dialect = SqlDialect.PostgreSQL };
        var (sql, _, resultContext) = SqlCompiler.Compile(query, context, 0);
        return (sql, resultContext.Parameters);
    }

    /// <summary>
    /// Compiles the SQL statement to PostgreSQL syntax with parameters.
    /// </summary>
    /// <param name="statement">The SQL statement to compile</param>
    /// <returns>A tuple containing the SQL string and parameter dictionary</returns>
    /// <example>
    /// <code>
    /// var (sql, parameters) = statement.ToPostgreSqlRaw();
    /// </code>
    /// </example>
    public static (string SqlRaw, ImmutableDictionary<string, object> Parameters) ToPostgreSqlRaw(this ISqlStatement statement)
    {
        var context = new Context { Dialect = SqlDialect.PostgreSQL };
        var (sql, resultContext) = SqlCompiler.Compile(statement, context, 0);
        return (sql, resultContext.Parameters);
    }

    /// <summary>
    /// Compiles the SQL scalar query to PostgreSQL syntax with parameters.
    /// </summary>
    /// <param name="scalarQuery">The SQL scalar query to compile</param>
    /// <returns>A tuple containing the SQL string and parameter dictionary</returns>
    /// <example>
    /// <code>
    /// var (sql, parameters) = countQuery.ToPostgreSqlRaw();
    /// </code>
    /// </example>
    public static (string SqlRaw, ImmutableDictionary<string, object> Parameters) ToPostgreSqlRaw(this ISqlScalarQuery scalarQuery)
    {
        var context = new Context { Dialect = SqlDialect.PostgreSQL };
        var (sql, resultContext) = SqlCompiler.Compile(scalarQuery, context, 0);
        return (sql, resultContext.Parameters);
    }
}
