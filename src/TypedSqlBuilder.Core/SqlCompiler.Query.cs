using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace TypedSqlBuilder.Core;

/// <summary>
/// SQL Compiler - Query compilation methods.
/// This partial class contains all methods related to compiling SQL queries.
/// </summary>
public static partial class SqlCompiler
{
    /// <summary>
    /// Normalizes a SQL query by applying fusion rules until a fixpoint is reached.
    /// Moves all fusion logic from extension methods to centralized normalization.
    /// Handles WHERE clause fusion, ORDER BY fusion, and SELECT composition.
    /// </summary>
    /// <param name="query">The SQL query to normalize</param>
    /// <returns>The normalized query with all possible fusions applied</returns>
    private static ISqlQuery Normalize(ISqlQuery query)
    {
        // Phase 1: Apply fusion rules until fixpoint
        var current = query;
        bool changed;
        
        do
        {
            (current, changed) = ApplyNormalizationRules(current);
        } 
        while (changed);

        // Phase 2: Apply canonical form normalization
        current = ApplyCanonicalForm(current);
        
        return current;
    }

    /// <summary>
    /// Applies normalization rules to a query, handling both fusion patterns and recursive subquery normalization.
    /// </summary>
    /// <param name="query">The query to apply normalization rules to</param>
    /// <returns>A tuple containing the normalized query and whether any changes were made</returns>
    private static (ISqlQuery Query, bool Changed) ApplyNormalizationRules(ISqlQuery query)
    {
        switch (query)
        {
            // WHERE clause fusion: WHERE(WHERE(q, p1), p2) → WHERE(q, p1 AND p2)
            case WhereClause(WhereClause(var innerQuery, var innerPred), var outerPred):
                return (new WhereClause(innerQuery, tuple => innerPred(tuple) && outerPred(tuple)), true);
            
            // ORDER BY fusion: OrderBy(OrderBy(q, keys1), keys2) → OrderBy(q, keys1 + keys2)  
            case OrderByClause(OrderByClause(var innerQuery, var innerKeys), var outerKeys):
                return (new OrderByClause(innerQuery, innerKeys.AddRange(outerKeys)), true);
            
            // SELECT composition: SELECT(SELECT(q, f1), f2) → SELECT(q, f2 ∘ f1)
            // Preserve outer Select's Aliases (projection names)
            case SelectClause(SelectClause(var innerQuery, var innerSelector, var innerAliases), var outerSelector, var outerAliases):
                return (new SelectClause(innerQuery, tuple => outerSelector(innerSelector(tuple)), outerAliases), true);

            case HavingClause(GroupByClause(var innerQuery, var keySelector, null), var predicate):
                return (new GroupByClause(innerQuery, keySelector, predicate), true);

            case JoinClause(JoinClause(var innerOuter, var innerJoinData), var outerJoinData):
                return (new JoinClause(innerOuter, [..innerJoinData, ..outerJoinData]), true);

            // Recursive normalization of subqueries - WHERE
            case WhereClause(var subQuery, var predicate):
                return ApplyToSubQuery(subQuery, q => new WhereClause(q, predicate));
            
            // Recursive normalization of subqueries - SELECT (preserve Aliases)
            case SelectClause(var subQuery, var selector, var aliases):
                return ApplyToSubQuery(subQuery, q => new SelectClause(q, selector, aliases));
            
            // Recursive normalization of subqueries - ORDER BY
            case OrderByClause(var subQuery, var keySelectors):
                return ApplyToSubQuery(subQuery, q => new OrderByClause(q, keySelectors));

            case GroupByClause(var subQuery, var keySelector, var havingPredicate):
                return ApplyToSubQuery(subQuery, q => new GroupByClause(q, keySelector, havingPredicate));

            case JoinClause(var outer, var joinData):
                return ApplyToSubQuery(outer, q => new JoinClause(q, joinData));

            // No changes needed
            default:
                return (query, false);
        }
    }

    /// <summary>
    /// Helper method to normalize subqueries and reconstruct parent queries.
    /// </summary>
    /// <param name="subQuery">The subquery to normalize</param>
    /// <param name="constructor">Function to reconstruct the parent query with the normalized subquery</param>
    /// <returns>A tuple containing the reconstructed query and whether any changes were made</returns>
    private static (ISqlQuery Query, bool Changed) ApplyToSubQuery(ISqlQuery subQuery, Func<ISqlQuery, ISqlQuery> constructor)
    {
        var (normalizedSub, changed) = ApplyNormalizationRules(subQuery);
        return (constructor(normalizedSub), changed);
    }

    /// <summary>
    /// Applies canonical form normalization to ensure queries match compiler patterns.
    /// Wraps non-SELECT queries with SELECT clauses and ensures proper structure.
    /// </summary>
    /// <param name="query">The query to normalize to canonical form</param>
    /// <returns>The query in canonical form</returns>
    private static ISqlQuery ApplyCanonicalForm(ISqlQuery query)
    {
        return query switch
        {
            // Wrap non-SELECT queries with SELECT * to match compiler patterns
            WhereClause =>
                new SelectClause(query, tuple => tuple, []),
                
            OrderByClause =>
                new SelectClause(query, tuple => tuple, []),

            FromTableClause =>
                new SelectClause(query, tuple => tuple, []),

            FromSubQueryClause =>
                new SelectClause(query, tuple => tuple, []),

            GroupByClause =>
                new SelectClause(query, tuple => tuple, []),

            JoinClause =>
                new SelectClause(query, tuple => tuple, []),

            // Already in canonical form
            _ => query
        };
    }

    /// <summary>
    /// Recursively flattens a tuple structure into a flat array of SQL expressions.
    /// Handles nested tuples and ignores null values, extracting only SqlExpr instances.
    /// </summary>
    /// <param name="tuple">The tuple to flatten</param>
    /// <returns>An immutable array containing all SqlExpr instances found in the tuple hierarchy</returns>
    /// <exception cref="NotSupportedException">Thrown when a tuple contains unsupported item types</exception>
    private static ImmutableArray<SqlExpr> FlattenTuple(ITuple tuple)
    {
        if (tuple is ISqlTable sqlTable)
        {
            return FlattenTable(sqlTable);
        }

        var flattened = ImmutableArray.CreateBuilder<SqlExpr>();
        for (int i = 0; i < tuple.Length; i++)
        {
            var item = tuple[i];
            switch (item)
            {                
                case null:
                    // ignore null values
                    break;
                case SqlExpr expr:
                    flattened.Add(expr);
                    break;
                case ISqlTable table:
                {
                    flattened.AddRange(FlattenTable(table));
                    break;
                }
                case ITuple subTuple:
                    // Recursively flatten nested tuples
                    flattened.AddRange(FlattenTuple(subTuple));
                    break;                
                default:
                    throw new NotSupportedException($"Tuple item type {item?.GetType().Name} is not supported");
            }
        }
        return flattened.ToImmutable();
    }

    private static ImmutableArray<SqlExpr> FlattenTable(ISqlTable sqlTable)
    {
        if (sqlTable.Columns.All(c => c is { } && c is SqlExpr sqlExpr))
        {
            return sqlTable.Columns.Cast<SqlExpr>().ToImmutableArray();
        }

        var flattened = ImmutableArray.CreateBuilder<SqlExpr>();
        // use reflection on sqlTable to get properties that return ISqlColumn and have no parameters
        var properties = sqlTable.GetType().GetProperties()
            .Where(prop => prop.GetIndexParameters().Length == 0 && typeof(ISqlColumn).IsAssignableFrom(prop.PropertyType));
            
        foreach (var prop in properties)
        {
            if (prop.GetValue(sqlTable) is SqlExpr sqlExpr)
            {
                flattened.Add(sqlExpr);
            }
        }
        return flattened.ToImmutable();
    }

          
    

    /// <summary>
    /// Updates projection aliases for tuple expressions when wrapping queries as subqueries.
    /// This method ensures that expressions from inner queries can be properly referenced
    /// in outer queries by updating their alias table references.
    /// </summary>
    /// <param name="tuple">The tuple containing expressions to update aliases for</param>
    /// <param name="context">The current compilation context</param>
    /// <returns>A new context with updated projection aliases and incremented alias index</returns>
    private static Context UpdateProjectionAliases(ITuple tuple, Context context)
    {
        // Update projection aliases for the tuple items        
        var ctx = context with { AliasIndex = context.AliasIndex + 1 };
        var aliasIndex = ctx.AliasIndex;

        foreach (var expr in FlattenTuple(tuple))
        {
            if (ctx.ProjectionAliases.TryGetValue(expr, out var existingAlias))
            {
                ctx = ctx with { ProjectionAliases = ctx.ProjectionAliases.SetItem(expr, new SqlExprAlias($"a{aliasIndex}", existingAlias.Field)) };
            }
            else if (expr is ISqlColumn sqlColumn)
            {
                ctx = ctx with { ProjectionAliases = ctx.ProjectionAliases.SetItem(expr, new SqlExprAlias($"a{aliasIndex}", sqlColumn.ColumnName)) };
            }
            else
                throw new InvalidOperationException($"Expression {expr} does not have a projection alias defined in the context.");
        }
        return ctx;
    }

    /// <summary>
    /// Compiles a tuple projection into a SELECT list with proper aliasing.
    /// Flattens nested tuples, generates appropriate field aliases, and updates projection aliases in context.
    /// </summary>
    /// <param name="tuple">The tuple to compile</param>
    /// <param name="aliases">Optional array of explicit field aliases</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The SQL string representation and updated context</returns>
    private static (string, Context) CompileTupleProjection(ITuple tuple, ImmutableArray<string?> aliases, Context context)
    {
        var items = new List<(string Projection, string Alias)>();
        var ctx = context;        
        int projectionCount = 0;
        
        var flattenedExprs = FlattenTuple(tuple);
        
        // Only use provided aliases if the count matches exactly
        bool useProvidedAliases = !aliases.IsDefault && aliases.Length == flattenedExprs.Length;

        for (int i = 0; i < flattenedExprs.Length; i++)
        {
            var expr = flattenedExprs[i];
            var (compiled, newCtx) = Compile(expr, ctx);
            ctx = newCtx;
            
            // Determine field alias
            string fieldAlias;
            if (useProvidedAliases && !string.IsNullOrWhiteSpace(aliases[i]))
            {
                fieldAlias = aliases[i]!;
            }
            else
            {
                // Default alias behavior
                fieldAlias = expr is ISqlColumn sqlColumn ? sqlColumn.ColumnName : $"prj{projectionCount++}";
            }
            
            items.Add((compiled, fieldAlias));
            ctx = ctx with { ProjectionAliases = ctx.ProjectionAliases.SetItem(expr, new SqlExprAlias($"a{ctx.AliasIndex}", fieldAlias)) };
        }

        return (string.Join(", ", items.Select(i => $"{i.Projection} AS {i.Alias}")), ctx);
    }
    /// <summary>
    /// Compiles SQL queries and clauses to SQL string representation.
    /// First normalizes the query to apply fusion rules, then pattern matches
    /// to generate appropriate SQL for each query structure.
    /// </summary>
    /// <param name="query">The SQL query or clause to compile</param>
    /// <param name="context">The compilation context for tracking aliases and parameters</param>
    /// <returns>A tuple containing the SQL string, the selected tuple, and updated context</returns>
    public static (string, ITuple, Context) Compile(ISqlQuery query, Context context)
    {
        // First normalize the query to apply all fusion rules
        var normalizedQuery = Normalize(query);            
        switch (normalizedQuery)
        {
            // ========== FROM TABLE CASES ==========
            case SelectClause(FromTableClause(var table), var selector, var aliases):
            {
                var (aliasIndex, newContext) = context.GetOrAddTableAlias(table);
                var selected = selector(table);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, newContext);
                return ($"SELECT {projection} FROM {table.TableName} a{aliasIndex}", selected, projectionCtx);
            }

            case SelectClause(WhereClause(FromTableClause(var table), var predicate), var selector, var aliases):
            {
                var (aliasIndex, newContext) = context.GetOrAddTableAlias(table);

                var (whereClause, whereCtx) = Compile(predicate(table), newContext);
                var selected = selector(table);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, whereCtx);
                return ($"SELECT {projection} FROM {table.TableName} a{aliasIndex} WHERE {whereClause}", selected, projectionCtx);
            }

            case SelectClause(OrderByClause(FromTableClause(var table), var keySelectors), var selector, var aliases):
            {
                var (aliasIndex, newContext) = context.GetOrAddTableAlias(table);

                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, table, newContext);
                var selected = selector(table);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, orderCtx);
                return ($"SELECT {projection} FROM {table.TableName} a{aliasIndex} ORDER BY {orderByClause}", selected, projectionCtx);
            }

            case SelectClause(OrderByClause(WhereClause(FromTableClause(var table), var predicate), var keySelectors), var selector, var aliases):
            {
                var (aliasIndex, newContext) = context.GetOrAddTableAlias(table);

                var (whereClause, whereCtx) = Compile(predicate(table), newContext);
                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, table, whereCtx);
                var selected = selector(table);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, orderCtx);
                return ($"SELECT {projection} FROM {table.TableName} a{aliasIndex} WHERE {whereClause} ORDER BY {orderByClause}", selected, projectionCtx);
            }
            
            case SelectClause(GroupByClause(FromTableClause(var table), var keySelector, var havingPredicate), var selector, var aliases):
            {
                var (aliasIndex, newContext) = context.GetOrAddTableAlias(table);

                var (groupByClause, groupCtx) = CompileGroupBy(keySelector, table, newContext);
                var selected = selector(table);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, groupCtx);

                if (havingPredicate is null)
                    return ($"SELECT {projection} FROM {table.TableName} a{aliasIndex} GROUP BY {groupByClause}", selected, projectionCtx);
                else
                {
                    var (havingSql, havingCtx) = Compile(havingPredicate(table, new SqlAggregateFunc()), groupCtx);
                    return ($"SELECT {projection} FROM {table.TableName} a{aliasIndex} GROUP BY {groupByClause} HAVING {havingSql}", selected, havingCtx);
                }
            }

            case SelectClause(GroupByClause(WhereClause(FromTableClause(var table), var predicate), var keySelector, var havingPredicate), var selector, var aliases):
            {
                var (aliasIndex, newContext) = context.GetOrAddTableAlias(table);

                var (whereClause, whereCtx) = Compile(predicate(table), newContext);
                var (groupByClause, groupCtx) = CompileGroupBy(keySelector, table, whereCtx);
                var selected = selector(table);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, groupCtx);
                
                if (havingPredicate is null)
                    return ($"SELECT {projection} FROM {table.TableName} a{aliasIndex} WHERE {whereClause} GROUP BY {groupByClause}", selected, projectionCtx);
                else
                {
                    var (havingSql, havingCtx) = Compile(havingPredicate(table, new SqlAggregateFunc()), groupCtx);
                    return ($"SELECT {projection} FROM {table.TableName} a{aliasIndex} WHERE {whereClause} GROUP BY {groupByClause} HAVING {havingSql}", selected, havingCtx);
                }

            }

            // ========== GENERAL CASES ==========
            // These cases handle any inner query type (including subqueries) by automatically wrapping them

            // General case: SELECT from subquery (FromSubQueryClause)
            case SelectClause(FromSubQueryClause(var subQuery), var selector, var aliases):
            {
                // Compile the subquery directly (no double-wrapping needed)
                var (subQuerySql, tuple, subQueryCtx) = Compile(subQuery, context);                              
                var newContext = UpdateProjectionAliases(tuple, subQueryCtx);
                var aliasIndex = newContext.AliasIndex;

                var selected = selector(tuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, newContext);
                return ($"SELECT {projection} FROM ({subQuerySql}) a{aliasIndex}", selected, projectionCtx);
            }
            
            // General case: WHERE clause applied to any query type (including FromSubQueryClause)
            // This automatically wraps the inner query as a subquery
            case SelectClause(WhereClause(var innerQuery, var predicate), var selector, var aliases):
            {
                // If innerQuery is FromSubQueryClause, extract its inner query to avoid double-wrapping
                var actualInnerQuery = innerQuery is FromSubQueryClause(var subQuery) ? subQuery : innerQuery;
                var (innerQuerySql, innerTuple, innerQueryCtx) = Compile(actualInnerQuery, context);
                var newContext = UpdateProjectionAliases(innerTuple, innerQueryCtx);
                var aliasIndex = newContext.AliasIndex;

                // Apply WHERE clause to the subquery result
                var (whereClause, whereCtx) = Compile(predicate(innerTuple), newContext);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, whereCtx);
                return ($"SELECT {projection} FROM ({innerQuerySql}) a{aliasIndex} WHERE {whereClause}", selected, projectionCtx);
            }

            // General case: ORDER BY + WHERE clause applied to any other query type
            // This automatically wraps the inner query as a subquery
            case SelectClause(OrderByClause(WhereClause(var innerQuery, var predicate), var keySelectors), var selector, var aliases):
            {
                // If innerQuery is FromSubQueryClause, extract its inner query to avoid double-wrapping
                var actualInnerQuery = innerQuery is FromSubQueryClause(var subQuery) ? subQuery : innerQuery;
                var (innerQuerySql, innerTuple, innerQueryCtx) = Compile(actualInnerQuery, context);
                var newContext = UpdateProjectionAliases(innerTuple, innerQueryCtx);
                var aliasIndex = newContext.AliasIndex;

                // Apply WHERE clause to the subquery result
                var (whereClause, whereCtx) = Compile(predicate(innerTuple), newContext);
                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, innerTuple, whereCtx);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, orderCtx);
                return ($"SELECT {projection} FROM ({innerQuerySql}) a{aliasIndex} WHERE {whereClause} ORDER BY {orderByClause}", selected, projectionCtx);
            }

            // General case: ORDER BY clause applied to any other query type  
            // This automatically wraps the inner query as a subquery
            case SelectClause(OrderByClause(var innerQuery, var keySelectors), var selector, var aliases):
            {
                // If innerQuery is FromSubQueryClause, extract its inner query to avoid double-wrapping
                var actualInnerQuery = innerQuery is FromSubQueryClause(var subQuery) ? subQuery : innerQuery;
                var (innerQuerySql, innerTuple, innerQueryCtx) = Compile(actualInnerQuery, context);
                var newContext = UpdateProjectionAliases(innerTuple, innerQueryCtx);
                var aliasIndex = newContext.AliasIndex;

                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, innerTuple, newContext);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, orderCtx);
                return ($"SELECT {projection} FROM ({innerQuerySql}) a{aliasIndex} ORDER BY {orderByClause}", selected, projectionCtx);
            }

            // General case: GROUP BY + WHERE clause applied to any other query type
            // This automatically wraps the inner query as a subquery
            case SelectClause(GroupByClause(WhereClause(var innerQuery, var predicate), var keySelector, var havingPredicate), var selector, var aliases):
            {
                // If innerQuery is FromSubQueryClause, extract its inner query to avoid double-wrapping
                var actualInnerQuery = innerQuery is FromSubQueryClause(var subQuery) ? subQuery : innerQuery;
                var (innerQuerySql, innerTuple, innerQueryCtx) = Compile(actualInnerQuery, context);
                var newContext = UpdateProjectionAliases(innerTuple, innerQueryCtx);
                var aliasIndex = newContext.AliasIndex;

                // Apply WHERE and GROUP BY clauses to the subquery result
                var (whereClause, whereCtx) = Compile(predicate(innerTuple), newContext);
                var (groupByClause, groupCtx) = CompileGroupBy(keySelector, innerTuple, whereCtx);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, groupCtx);
                
                if (havingPredicate is null)
                    return ($"SELECT {projection} FROM ({innerQuerySql}) a{aliasIndex} WHERE {whereClause} GROUP BY {groupByClause}", selected, projectionCtx);
                else
                {
                    var (havingSql, havingCtx) = Compile(havingPredicate(innerTuple, new SqlAggregateFunc()), groupCtx);
                    return ($"SELECT {projection} FROM ({innerQuerySql}) a{aliasIndex} WHERE {whereClause} GROUP BY {groupByClause} HAVING {havingSql}", selected, havingCtx);
                }
            }

            // ========== JOIN CASES ==========
            // JOIN clauses are always wrapped with SelectClause by canonical form normalization
            case SelectClause(JoinClause(var outer, var joinData), var selector, var aliases):
                {
                    if (joinData.IsEmpty)
                        throw new InvalidOperationException("JoinClause must contain at least one join operation");

                    // Start with the outer query
                    var (currentQuerySql, currentTuple, currentContext) = Compile(outer, context);

                    // Build the FROM clause - handle if outer is a table or subquery
                    string fromClause;
                    if (outer is FromTableClause fromTable)
                    {
                        var (outerAliasIndex, _) = context.GetOrAddTableAlias(fromTable.Table);
                        fromClause = $"{fromTable.Table.TableName} a{outerAliasIndex}";
                    }
                    else
                    {
                        // Outer query is a subquery, wrap it
                        var outerAliasIndex = currentContext.AliasIndex;
                        fromClause = $"({currentQuerySql}) a{outerAliasIndex}";
                        currentContext = currentContext with { AliasIndex = currentContext.AliasIndex + 1 };
                    }

                    List<string> joinClauses;
                    Context finalContext;
                    currentTuple = CompileJoin(joinData, currentTuple, currentContext, out joinClauses, out finalContext);

                    // Apply the selector to get the final selected tuple
                    var selected = selector(currentTuple);

                    // For JOINs, we never want SELECT * - always project the individual fields
                    // even if the selector is an identity function
                    var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, finalContext);

                    // Combine FROM clause with all JOIN clauses
                    var joinSql = $"SELECT {projection} FROM {fromClause} {string.Join(" ", joinClauses)}";

                    return (joinSql, selected, projectionCtx);
                }

            default:
                throw new NotSupportedException($"Query type {query.GetType().Name} is not supported");
        }
    }

    private static ITuple CompileJoin(ImmutableArray<(JoinType JoinType, ISqlTable Inner, Func<ITuple, SqlExpr> OuterKeySelector, Func<ITuple, SqlExpr> InnerKeySelector, Func<ITuple, ITuple, ITuple> ResultSelector, ImmutableArray<string?> Aliases)> joinData, ITuple currentTuple, Context currentContext, out List<string> joinClauses, out Context finalContext)
    {
        joinClauses = new List<string>();
        finalContext = currentContext;

        // Process each join in sequence
        foreach (var (joinType, inner, outerKeySelector, innerKeySelector, resultSelector, joinAliases) in joinData)
        {
            // Get or add alias for the inner table
            var (innerAliasIndex, innerContext) = finalContext.GetOrAddTableAlias(inner);

            // Extract the join condition using current tuple state
            var outerKey = outerKeySelector(currentTuple);
            var innerKey = innerKeySelector(inner);

            var (outerKeySql, outerKeyCtx) = Compile(outerKey, innerContext);
            var (innerKeySql, innerKeyCtx) = Compile(innerKey, outerKeyCtx);

            // Determine join type SQL
            var joinTypeSql = joinType switch
            {
                JoinType.Inner => "INNER JOIN",
                JoinType.Left => "LEFT JOIN",
                _ => throw new NotSupportedException($"Join type {joinType} is not supported")
            };

            // Add this join clause
            joinClauses.Add($"{joinTypeSql} {inner.TableName} a{innerAliasIndex} ON {outerKeySql} = {innerKeySql}");

            // Update the current tuple by applying the result selector
            currentTuple = resultSelector(currentTuple, inner);
            finalContext = innerKeyCtx;
        }

        return currentTuple;
    }

    /// <summary>
    /// Compiles SQL scalar queries to standalone SQL SELECT statements (without parentheses).
    /// Used for compiling aggregate functions as standalone queries.
    /// </summary>
    /// <param name="scalarQuery">The scalar query to compile</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The SQL string representation and updated context</returns>
    public static (string, Context) Compile(ISqlScalarQuery scalarQuery, Context context)
    {
        switch (scalarQuery)
        {
            case SumSqlIntClause(var query):
            {
                var sumQuery = new SelectClause(query, tuple => ValueTuple.Create(new SqlIntSum((SqlExprInt)tuple[0]!)), []);
                var (sql, _, ctx) = Compile(sumQuery, context);
                return (sql, ctx);
            }        

            case CountClause(var query):
            {
                var countQuery = new SelectClause(query, _ => ValueTuple.Create(new SqlIntCount()), []);
                var (sql, _, ctx) = Compile(countQuery, context);
                return (sql, ctx);
            }

            default:
                throw new NotSupportedException($"Scalar query type {scalarQuery.GetType().Name} is not supported");
        }
    }

    /// <summary>
    /// Compiles ORDER BY clauses with multiple key selectors.
    /// </summary>
    /// <param name="keySelectors">The key selectors and their sort directions</param>
    /// <param name="table">The table being queried</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The compiled ORDER BY clause and updated context</returns>
    private static (string, Context) CompileOrderBy(ImmutableArray<(Func<ITuple, SqlExpr> KeySelector, bool Descending)> keySelectors, ITuple table, Context context)
    {
        var orderItems = new List<string>();
        var ctx = context;

        foreach (var (keySelector, descending) in keySelectors)
        {
            var (orderByClause, orderCtx) = Compile(keySelector(table), ctx);
            orderItems.Add($"{orderByClause} {(descending ? "DESC" : "ASC")}");
            ctx = orderCtx;
        }

        return (string.Join(", ", orderItems), ctx);
    }

    /// <summary>
    /// Compiles GROUP BY clauses with key selectors.
    /// </summary>
    /// <param name="keySelector">The function that extracts grouping keys from input tuples</param>
    /// <param name="table">The table being queried</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The compiled GROUP BY clause and updated context</returns>
    private static (string, Context) CompileGroupBy(Func<ITuple, ImmutableArray<SqlExpr>> keySelector, ITuple table, Context context)
    {
        var groupItems = new List<string>();
        var ctx = context;

        var keys = keySelector(table);
        foreach (var key in keys)
        {
            var (groupByClause, groupCtx) = Compile(key, ctx);
            groupItems.Add(groupByClause);
            ctx = groupCtx;
        }

        return (string.Join(", ", groupItems), ctx);
    }
}
