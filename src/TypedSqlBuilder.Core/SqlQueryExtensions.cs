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

        return tupleNamesAttr?.TransformNames?.Where(x => x is not null).ToImmutableArray() ?? [];
    }

    /// <summary>
    /// Adds a DISTINCT clause to the query to eliminate duplicate rows.
    /// </summary>
    /// <typeparam name="TSource">The tuple type of the query results</typeparam>
    /// <param name="query">The source query to make distinct</param>
    /// <returns>A new query that eliminates duplicate rows</returns>
    /// <example>
    /// <code>
    /// var distinctNames = userQuery.Select(user => user.Name).Distinct();
    /// var distinctAges = userQuery.Select(user => user.Age).Distinct();
    /// </code>
    /// </example>
    public static ISqlQuery<TSource> Distinct<TSource>(this ISqlQuery<TSource> query)
        where TSource : ITuple
    {
        return new DistinctClause<TSource>(query);
    }

    /// <summary>
    /// Adds a LIMIT clause to restrict the number of rows returned by the query.
    /// Optionally includes an OFFSET to skip a specified number of rows.
    /// </summary>
    /// <typeparam name="TSource">The tuple type of the query results</typeparam>
    /// <param name="query">The source query to limit</param>
    /// <param name="limit">The maximum number of rows to return</param>
    /// <param name="offset">The number of rows to skip before returning results (optional)</param>
    /// <returns>A new query that limits the number of rows returned</returns>
    /// <example>
    /// <code>
    /// var firstTen = userQuery.Limit(10);
    /// var nextTen = userQuery.Limit(10, 10); // Skip 10, take 10
    /// var page = userQuery.OrderBy(u => u.Name).Limit(20, 40); // Page 3 with 20 items per page
    /// </code>
    /// </example>
    public static ISqlQuery<TSource> Limit<TSource>(this ISqlQuery<TSource> query, long limit, long? offset = null)
        where TSource : ITuple
    {
        return new LimitClause<TSource>(query, limit, offset);
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
    /// var pagedQuery = baseQuery.Select(row => (row.Name, row.Age)).Limit(10, 20); // Limit 10, Offset 20
    /// var limitOnlyQuery = baseQuery.Select(row => (row.Name, row.Age)).Limit(10); // Limit 10, no offset
    /// var distinctQuery = baseQuery.Select(row => (row.Name, row.Age)).Distinct();
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
    /// var distinctAges = userQuery.Select(user => user.Age).Distinct();
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
    /// var distinctAdults = userQuery.Select(user => user.Age >= 18).Distinct();
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
    /// var distinctNames = userQuery.Select(user => user.FirstName).Distinct();
    /// </code>
    /// </example>
    public static ISqlQuery<ValueTuple<SqlExprString>> Select<TSource>(this ISqlQuery<TSource> query, Func<TSource, SqlExprString> selector)
        where TSource : ITuple
    {
        return new SelectClause<TSource, ValueTuple<SqlExprString>>(query, x => ValueTuple.Create(selector(x)), ImmutableArray<string?>.Empty);
    }

    /// <summary>
    /// Projects the query result to a SQL decimal expression.
    /// Used for selecting computed decimal values or preparing for aggregate operations.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <param name="query">The source query to project from</param>
    /// <param name="selector">Function that transforms source tuples to SQL decimal expressions</param>
    /// <returns>A new query that produces SQL decimal expressions</returns>
    /// <example>
    /// <code>
    /// var priceQuery = productQuery.Select(product => product.UnitPrice * product.Quantity);
    /// var distinctPrices = productQuery.Select(product => product.UnitPrice).Distinct();
    /// </code>
    /// </example>
    public static ISqlQuery<ValueTuple<SqlExprDecimal>> Select<TSource>(this ISqlQuery<TSource> query, Func<TSource, SqlExprDecimal> selector)
        where TSource : ITuple
    {
        return new SelectClause<TSource, ValueTuple<SqlExprDecimal>>(query, x => ValueTuple.Create(selector(x)), ImmutableArray<string?>.Empty);
    }

    /// <summary>
    /// Projects the query result to a SQL DateTime expression.
    /// Used for selecting computed DateTime values or preparing for date operations.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <param name="query">The source query to project from</param>
    /// <param name="selector">Function that transforms source tuples to SQL DateTime expressions</param>
    /// <returns>A new query that produces SQL DateTime expressions</returns>
    /// <example>
    /// <code>
    /// var dateQuery = orderQuery.Select(order => order.OrderDate);
    /// var distinctDates = orderQuery.Select(order => order.OrderDate).Distinct();
    /// </code>
    /// </example>
    public static ISqlQuery<ValueTuple<SqlExprDateTime>> Select<TSource>(this ISqlQuery<TSource> query, Func<TSource, SqlExprDateTime> selector)
        where TSource : ITuple
    {
        return new SelectClause<TSource, ValueTuple<SqlExprDateTime>>(query, x => ValueTuple.Create(selector(x)), ImmutableArray<string?>.Empty);
    }

    /// <summary>
    /// Projects the query result to a SQL GUID expression.
    /// Used for selecting computed GUID values or preparing for identifier operations.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <param name="query">The source query to project from</param>
    /// <param name="selector">Function that transforms source tuples to SQL GUID expressions</param>
    /// <returns>A new query that produces SQL GUID expressions</returns>
    /// <example>
    /// <code>
    /// var idQuery = userQuery.Select(user => user.Id);
    /// var distinctIds = userQuery.Select(user => user.Id).Distinct();
    /// </code>
    /// </example>
    public static ISqlQuery<ValueTuple<SqlExprGuid>> Select<TSource>(this ISqlQuery<TSource> query, Func<TSource, SqlExprGuid> selector)
        where TSource : ITuple
    {
        return new SelectClause<TSource, ValueTuple<SqlExprGuid>>(query, x => ValueTuple.Create(selector(x)), ImmutableArray<string?>.Empty);
    }

    /// <summary>
    /// Projects the query result to a SQL long expression.
    /// Used for selecting computed long values or preparing for aggregate operations.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <param name="query">The source query to project from</param>
    /// <param name="selector">Function that transforms source tuples to SQL long expressions</param>
    /// <returns>A new query that produces SQL long expressions</returns>
    /// <example>
    /// <code>
    /// var sizeQuery = fileQuery.Select(file => file.SizeBytes);
    /// var distinctSizes = fileQuery.Select(file => file.SizeBytes).Distinct();
    /// </code>
    /// </example>
    public static ISqlQuery<ValueTuple<SqlExprLong>> Select<TSource>(this ISqlQuery<TSource> query, Func<TSource, SqlExprLong> selector)
        where TSource : ITuple
    {
        return new SelectClause<TSource, ValueTuple<SqlExprLong>>(query, x => ValueTuple.Create(selector(x)), ImmutableArray<string?>.Empty);
    }

    /// <summary>
    /// Projects the query result to a SQL double expression.
    /// Used for selecting computed double values or preparing for aggregate operations.
    /// </summary>
    /// <typeparam name="TSource">The input tuple type from the source query</typeparam>
    /// <param name="query">The source query to project from</param>
    /// <param name="selector">Function that transforms source tuples to SQL double expressions</param>
    /// <returns>A new query that produces SQL double expressions</returns>
    /// <example>
    /// <code>
    /// var distanceQuery = routeQuery.Select(route => route.Distance);
    /// var distinctDistances = routeQuery.Select(route => route.Distance).Distinct();
    /// </code>
    /// </example>
    public static ISqlQuery<ValueTuple<SqlExprDouble>> Select<TSource>(this ISqlQuery<TSource> query, Func<TSource, SqlExprDouble> selector)
        where TSource : ITuple
    {
        return new SelectClause<TSource, ValueTuple<SqlExprDouble>>(query, x => ValueTuple.Create(selector(x)), ImmutableArray<string?>.Empty);
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
    /// Applies the SUM aggregate function to a query of decimal expressions.
    /// Computes the total sum of all decimal values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL decimal expressions</param>
    /// <returns>A SQL scalar query representing the sum of all values</returns>
    /// <example>
    /// <code>
    /// var totalPrice = productQuery.Select(product => product.UnitPrice * product.Quantity).Sum();
    /// </code>
    /// </example>
    public static SqlScalarQuery<SqlExprDecimal> Sum(this ISqlQuery<ValueTuple<SqlExprDecimal>> query)
    {
        return new SumSqlDecimalClause(query);
    }

    /// <summary>
    /// Applies the AVG aggregate function to a query of decimal expressions.
    /// Computes the average of all decimal values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL decimal expressions</param>
    /// <returns>A SQL scalar query representing the average of all values</returns>
    /// <example>
    /// <code>
    /// var avgPrice = productQuery.Select(product => product.UnitPrice).Avg();
    /// </code>
    /// </example>
    public static SqlScalarQuery<SqlExprDecimal> Avg(this ISqlQuery<ValueTuple<SqlExprDecimal>> query)
    {
        return new AvgSqlDecimalClause(query);
    }

    /// <summary>
    /// Applies the MIN aggregate function to a query of decimal expressions.
    /// Finds the minimum value among all decimal values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL decimal expressions</param>
    /// <returns>A SQL scalar query representing the minimum value</returns>
    /// <example>
    /// <code>
    /// var minPrice = productQuery.Select(product => product.UnitPrice).Min();
    /// </code>
    /// </example>
    public static SqlScalarQuery<SqlExprDecimal> Min(this ISqlQuery<ValueTuple<SqlExprDecimal>> query)
    {
        return new MinSqlDecimalClause(query);
    }

    /// <summary>
    /// Applies the MAX aggregate function to a query of decimal expressions.
    /// Finds the maximum value among all decimal values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL decimal expressions</param>
    /// <returns>A SQL scalar query representing the maximum value</returns>
    /// <example>
    /// <code>
    /// var maxPrice = productQuery.Select(product => product.UnitPrice).Max();
    /// </code>
    /// </example>
    public static SqlScalarQuery<SqlExprDecimal> Max(this ISqlQuery<ValueTuple<SqlExprDecimal>> query)
    {
        return new MaxSqlDecimalClause(query);
    }

    /// <summary>
    /// Applies the SUM aggregate function to a query of long expressions.
    /// Computes the total sum of all long values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL long expressions</param>
    /// <returns>A SQL scalar query representing the sum of all values</returns>
    /// <example>
    /// <code>
    /// var totalBytes = fileQuery.Select(file => file.SizeBytes).Sum();
    /// </code>
    /// </example>
    public static SqlScalarQuery<SqlExprLong> Sum(this ISqlQuery<ValueTuple<SqlExprLong>> query)
    {
        return new SumSqlLongClause(query);
    }

    /// <summary>
    /// Applies the AVG aggregate function to a query of long expressions.
    /// Computes the average of all long values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL long expressions</param>
    /// <returns>A SQL scalar query representing the average of all values</returns>
    /// <example>
    /// <code>
    /// var avgBytes = fileQuery.Select(file => file.SizeBytes).Avg();
    /// </code>
    /// </example>
    public static SqlScalarQuery<SqlExprLong> Avg(this ISqlQuery<ValueTuple<SqlExprLong>> query)
    {
        return new AvgSqlLongClause(query);
    }

    /// <summary>
    /// Applies the MIN aggregate function to a query of long expressions.
    /// Finds the minimum value among all long values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL long expressions</param>
    /// <returns>A SQL scalar query representing the minimum value</returns>
    /// <example>
    /// <code>
    /// var minBytes = fileQuery.Select(file => file.SizeBytes).Min();
    /// </code>
    /// </example>
    public static SqlScalarQuery<SqlExprLong> Min(this ISqlQuery<ValueTuple<SqlExprLong>> query)
    {
        return new MinSqlLongClause(query);
    }

    /// <summary>
    /// Applies the MAX aggregate function to a query of long expressions.
    /// Finds the maximum value among all long values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL long expressions</param>
    /// <returns>A SQL scalar query representing the maximum value</returns>
    /// <example>
    /// <code>
    /// var maxBytes = fileQuery.Select(file => file.SizeBytes).Max();
    /// </code>
    /// </example>
    public static SqlScalarQuery<SqlExprLong> Max(this ISqlQuery<ValueTuple<SqlExprLong>> query)
    {
        return new MaxSqlLongClause(query);
    }

    /// <summary>
    /// Applies the SUM aggregate function to a query of double expressions.
    /// Computes the total sum of all double values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL double expressions</param>
    /// <returns>A SQL scalar query representing the sum of all values</returns>
    /// <example>
    /// <code>
    /// var totalDistance = routeQuery.Select(route => route.Distance).Sum();
    /// </code>
    /// </example>
    public static SqlScalarQuery<SqlExprDouble> Sum(this ISqlQuery<ValueTuple<SqlExprDouble>> query)
    {
        return new SumSqlDoubleClause(query);
    }

    /// <summary>
    /// Applies the AVG aggregate function to a query of double expressions.
    /// Computes the average of all double values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL double expressions</param>
    /// <returns>A SQL scalar query representing the average of all values</returns>
    /// <example>
    /// <code>
    /// var avgDistance = routeQuery.Select(route => route.Distance).Avg();
    /// </code>
    /// </example>
    public static SqlScalarQuery<SqlExprDouble> Avg(this ISqlQuery<ValueTuple<SqlExprDouble>> query)
    {
        return new AvgSqlDoubleClause(query);
    }

    /// <summary>
    /// Applies the MIN aggregate function to a query of double expressions.
    /// Finds the minimum value among all double values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL double expressions</param>
    /// <returns>A SQL scalar query representing the minimum value</returns>
    /// <example>
    /// <code>
    /// var minDistance = routeQuery.Select(route => route.Distance).Min();
    /// </code>
    /// </example>
    public static SqlScalarQuery<SqlExprDouble> Min(this ISqlQuery<ValueTuple<SqlExprDouble>> query)
    {
        return new MinSqlDoubleClause(query);
    }

    /// <summary>
    /// Applies the MAX aggregate function to a query of double expressions.
    /// Finds the maximum value among all double values in the query result.
    /// </summary>
    /// <param name="query">A query that produces SQL double expressions</param>
    /// <returns>A SQL scalar query representing the maximum value</returns>
    /// <example>
    /// <code>
    /// var maxDistance = routeQuery.Select(route => route.Distance).Max();
    /// </code>
    /// </example>
    public static SqlScalarQuery<SqlExprDouble> Max(this ISqlQuery<ValueTuple<SqlExprDouble>> query)
    {
        return new MaxSqlDoubleClause(query);
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
        return new GroupByClause<TSource>(query, x =>
        {
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
        var context = new Context { DatabaseType = DatabaseType.SqlServer };
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
        var context = new Context { DatabaseType = DatabaseType.SQLite };
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
        var context = new Context { DatabaseType = DatabaseType.SqlServer };
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
        var context = new Context { DatabaseType = DatabaseType.SQLite };
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
        var context = new Context { DatabaseType = DatabaseType.SqlServer };
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
        var context = new Context { DatabaseType = DatabaseType.SQLite };
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
        var context = new Context { DatabaseType = DatabaseType.PostgreSQL };
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
        var context = new Context { DatabaseType = DatabaseType.PostgreSQL };
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
        var context = new Context { DatabaseType = DatabaseType.PostgreSQL };
        var (sql, resultContext) = SqlCompiler.Compile(scalarQuery, context, 0);
        return (sql, resultContext.Parameters);
    }

    /// <summary>
    /// Converts a scalar query to raw SQL and parameters for the specified database type.
    /// </summary>
    /// <param name="scalarQuery">The scalar query to convert</param>
    /// <param name="databaseType">The target database type (SqlServer, SQLite, or PostgreSQL)</param>
    /// <returns>A tuple containing the raw SQL string and parameters dictionary</returns>
    public static (string SqlRaw, ImmutableDictionary<string, object> Parameters) ToSqlRaw(this ISqlScalarQuery scalarQuery, DatabaseType databaseType)
    {
        var context = new Context { DatabaseType = databaseType };
        var (sql, resultContext) = SqlCompiler.Compile(scalarQuery, context, 0);
        return (sql, resultContext.Parameters);
    }

    /// <summary>
    /// Converts a query to raw SQL and parameters for the specified database type.
    /// </summary>
    /// <param name="query">The query to convert</param>
    /// <param name="databaseType">The target database type (SqlServer, SQLite, or PostgreSQL)</param>
    /// <returns>A tuple containing the raw SQL string and parameters dictionary</returns>
    public static (string SqlRaw, ImmutableDictionary<string, object> Parameters) ToSqlRaw(this ISqlQuery query, DatabaseType databaseType)
    {
        var context = new Context { DatabaseType = databaseType };
        var (sql, _, resultContext) = SqlCompiler.Compile(query, context, 0);
        return (sql, resultContext.Parameters);
    }

    /// <summary>
    /// Converts a statement to raw SQL and parameters for the specified database type.
    /// </summary>
    /// <param name="statement">The statement to convert</param>
    /// <param name="databaseType">The target database type (SqlServer, SQLite, or PostgreSQL)</param>
    /// <returns>A tuple containing the raw SQL string and parameters dictionary</returns>
    public static (string SqlRaw, ImmutableDictionary<string, object> Parameters) ToSqlRaw(this ISqlStatement statement, DatabaseType databaseType)
    {
        var context = new Context { DatabaseType = databaseType };
        var (sql, resultContext) = SqlCompiler.Compile(statement, context, 0);
        return (sql, resultContext.Parameters);
    }

    
}
