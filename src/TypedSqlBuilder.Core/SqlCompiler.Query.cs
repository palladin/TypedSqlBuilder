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
    /// Helper method to generate indentation strings for SQL formatting.
    /// </summary>
    /// <param name="scopeLevel">The current scope level</param>
    /// <returns>A tuple containing the base indent and sub-indent strings</returns>
    private static (string Indent, string SubIndent) GetIndentation(int scopeLevel)
    {
        var indent = new string(' ', scopeLevel * 4);
        var subIndent = new string(' ', (scopeLevel + 1) * 4);
        return (indent, subIndent);
    }
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
                return (new OrderByClause(innerQuery, tuple => innerKeys(tuple).AddRange(outerKeys(tuple))), true);
            
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
    /// <param name="baseScopeLevel">The base scope level for the SQL statement</param>
    /// <returns>The SQL string representation and updated context</returns>
    private static (string, Context) CompileTupleProjection(ITuple tuple, ImmutableArray<string?> aliases, Context context, int baseScopeLevel)
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
            var (compiled, newCtx) = Compile(expr, ctx, baseScopeLevel);
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

        // Format projections with proper indentation - each on its own line
        // Use the base scope level passed from the caller, not the context's scope level
        var (_, subIndent) = GetIndentation(baseScopeLevel);
        var projectionLines = items.Select((item, index) => 
        {
            var comma = index < items.Count - 1 ? "," : "";
            return $"{subIndent}{item.Projection} AS {item.Alias}{comma}";
        });
        
        return (string.Join("\n", projectionLines), ctx);
    }
    /// <summary>
    /// Compiles SQL queries and clauses to SQL string representation.
    /// First normalizes the query to apply fusion rules, then pattern matches
    /// to generate appropriate SQL for each query structure.
    /// </summary>
    /// <param name="query">The SQL query or clause to compile</param>
    /// <param name="context">The compilation context for tracking aliases and parameters</param>
    /// <param name="scopeLevel">The nesting scope level for the SQL statement</param>
    /// <returns>A tuple containing the SQL string, the selected tuple, and updated context</returns>
    public static (string, ITuple, Context) Compile(ISqlQuery query, Context context, int scopeLevel)
    {
        // First normalize the query to apply all fusion rules
        var normalizedQuery = Normalize(query);            
        switch (normalizedQuery)
        {            
            // ========================================
            // JOIN-OPTIMIZED PATTERNS (most specific)
            // ========================================
            
            // JOIN + WHERE: Optimized case for WHERE clause applied directly to JOIN
            case SelectClause(WhereClause(JoinClause(var outer, var joinData), var predicate), var selector, var aliases):
            {
                // Start with the outer query
                var (fromClause, currentTuple, currentContext) = CompileFrom(outer, context, scopeLevel);                    
                var (joinClauses, updatedTuple, joinContext) = CompileJoin(joinData, currentTuple, currentContext, scopeLevel);
                
                // Apply WHERE clause to the joined result
                var (whereClause, whereCtx) = Compile(predicate(updatedTuple), joinContext, scopeLevel);
                var selected = selector(updatedTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, whereCtx, scopeLevel);
                
                // Format SQL with proper indentation
                var (indent, subIndent) = GetIndentation(scopeLevel);
                var joinSql = $$"""
                {{indent}}SELECT 
                {{projection}}
                {{indent}}FROM 
                {{subIndent}}{{fromClause}}
                {{indent}}{{string.Join($"\n{indent}", joinClauses)}}
                {{indent}}WHERE 
                {{subIndent}}{{whereClause}}
                """;
                
                return (joinSql, selected, projectionCtx);
            }

            // JOIN + GROUP BY: Optimized case for GROUP BY applied directly to JOIN
            case SelectClause(GroupByClause(JoinClause(var outer, var joinData), var keySelector, var havingPredicate), var selector, var aliases):
            {
                // Start with the outer query
                var (fromClause, currentTuple, currentContext) = CompileFrom(outer, context, scopeLevel);                    
                var (joinClauses, updatedTuple, joinContext) = CompileJoin(joinData, currentTuple, currentContext, scopeLevel);
                
                // Apply GROUP BY directly to the joined result
                var (groupByClause, groupCtx) = CompileGroupBy(keySelector, updatedTuple, joinContext, scopeLevel);
                var selected = selector(updatedTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, groupCtx, scopeLevel);

                // Format SQL with proper indentation
                var (indent, subIndent) = GetIndentation(scopeLevel);
                var joinSql = $$"""
                {{indent}}SELECT 
                {{projection}}
                {{indent}}FROM 
                {{subIndent}}{{fromClause}}
                {{indent}}{{string.Join($"\n{indent}", joinClauses)}}
                {{indent}}GROUP BY 
                {{subIndent}}{{groupByClause}}
                """;
                
                if (havingPredicate is null)
                    return (joinSql, selected, projectionCtx);
                else
                {
                    var (havingSql, havingCtx) = Compile(havingPredicate(updatedTuple, new SqlAggregateFunc()), groupCtx, scopeLevel);
                    return ($"{joinSql}\n{indent}HAVING {havingSql}", selected, havingCtx);
                }
            }

            // JOIN + ORDER BY: Optimized case for ORDER BY applied directly to JOIN
            case SelectClause(OrderByClause(JoinClause(var outer, var joinData), var keySelectors), var selector, var aliases):
            {
                // Start with the outer query
                var (fromClause, currentTuple, currentContext) = CompileFrom(outer, context, scopeLevel);                    
                var (joinClauses, updatedTuple, joinContext) = CompileJoin(joinData, currentTuple, currentContext, scopeLevel);
                
                // Apply ORDER BY directly to the joined result
                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, updatedTuple, joinContext, scopeLevel);
                var selected = selector(updatedTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, orderCtx, scopeLevel);

                // Format SQL with proper indentation
                var (indent, subIndent) = GetIndentation(scopeLevel);
                var joinSql = $$"""
                {{indent}}SELECT 
                {{projection}}
                {{indent}}FROM 
                {{subIndent}}{{fromClause}}
                {{indent}}{{string.Join($"\n{indent}", joinClauses)}}
                {{indent}}ORDER BY 
                {{subIndent}}{{orderByClause}}
                """;
                return (joinSql, selected, projectionCtx);
            }

            // Basic JOIN: Standard JOIN clause compilation
            case SelectClause(JoinClause(var outer, var joinData), var selector, var aliases):
            {            
                // Start with the outer query
                var (fromClause, currentTuple, currentContext) = CompileFrom(outer, context, scopeLevel);                    
                var (joinClauses, updatedTuple, finalContext) = CompileJoin(joinData, currentTuple, currentContext, scopeLevel);                    
                // Apply the selector to get the final selected tuple
                var selected = selector(updatedTuple);

                // For JOINs, we never want SELECT * - always project the individual fields
                // even if the selector is an identity function
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, finalContext, scopeLevel);

                // Format SQL with proper indentation
                var (indent, subIndent) = GetIndentation(scopeLevel);
                var joinSql = $$"""
                {{indent}}SELECT 
                {{projection}}
                {{indent}}FROM 
                {{subIndent}}{{fromClause}}
                {{indent}}{{string.Join($"\n{indent}", joinClauses)}}
                """;
                return (joinSql, selected, projectionCtx);
            }

            // ========================================
            // GROUP BY PATTERNS (specific to general)
            // ========================================

            // GROUP BY + WHERE: GROUP BY with WHERE clause applied to any query type
            case SelectClause(GroupByClause(WhereClause(var innerQuery, var predicate), var keySelector, var havingPredicate), var selector, var aliases):
            {
                var (fromClause, innerTuple, innerContext) = CompileFrom(innerQuery, context, scopeLevel);
                
                // Apply WHERE clause to the result
                var (whereClause, whereCtx) = Compile(predicate(innerTuple), innerContext, scopeLevel);
                var (groupByClause, groupCtx) = CompileGroupBy(keySelector, innerTuple, whereCtx, scopeLevel);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, groupCtx, scopeLevel);
                
                if (havingPredicate is null)
                {
                    // Format SQL with proper indentation
                    var (indent, subIndent) = GetIndentation(scopeLevel);
                    var sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}WHERE 
                    {{subIndent}}{{whereClause}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    """;
                    return (sql, selected, projectionCtx);
                }
                else
                {
                    var (havingSql, havingCtx) = Compile(havingPredicate(innerTuple, new SqlAggregateFunc()), groupCtx, scopeLevel);
                    var (indent, subIndent) = GetIndentation(scopeLevel);
                    var sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}WHERE 
                    {{subIndent}}{{whereClause}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    {{indent}}HAVING 
                    {{subIndent}}{{havingSql}}
                    """;
                    return (sql, selected, havingCtx);
                }
            }

            // Basic GROUP BY: GROUP BY clause applied to any query type
            case SelectClause(GroupByClause(var innerQuery, var keySelector, var havingPredicate), var selector, var aliases):
            {
                var (fromClause, innerTuple, innerContext) = CompileFrom(innerQuery, context, scopeLevel);
                
                var (groupByClause, groupCtx) = CompileGroupBy(keySelector, innerTuple, innerContext, scopeLevel);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, groupCtx, scopeLevel);

                if (havingPredicate is null)
                {
                    // Format SQL with proper indentation
                    var (indent, subIndent) = GetIndentation(scopeLevel);
                    var sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    """;
                    return (sql, selected, projectionCtx);
                }
                else
                {
                    var (havingSql, havingCtx) = Compile(havingPredicate(innerTuple, new SqlAggregateFunc()), groupCtx, scopeLevel);
                    var (indent, subIndent) = GetIndentation(scopeLevel);
                    var sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    {{indent}}HAVING 
                    {{subIndent}}{{havingSql}}
                    """;
                    return (sql, selected, havingCtx);
                }
            }

            // ========================================
            // ORDER BY PATTERNS (specific to general)
            // ========================================

            // ORDER BY + WHERE: ORDER BY with WHERE clause applied to any query type
            case SelectClause(OrderByClause(WhereClause(var innerQuery, var predicate), var keySelectors), var selector, var aliases):
            {
                var (fromClause, innerTuple, innerContext) = CompileFrom(innerQuery, context, scopeLevel);
                
                // Apply WHERE clause to the result
                var (whereClause, whereCtx) = Compile(predicate(innerTuple), innerContext, scopeLevel);
                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, innerTuple, whereCtx, scopeLevel);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, orderCtx, scopeLevel);
                
                // Format SQL with proper indentation
                var (indent, subIndent) = GetIndentation(scopeLevel);
                var sql = $$"""
                {{indent}}SELECT 
                {{projection}}
                {{indent}}FROM 
                {{subIndent}}{{fromClause}}
                {{indent}}WHERE 
                {{subIndent}}{{whereClause}}
                {{indent}}ORDER BY 
                {{subIndent}}{{orderByClause}}
                """;
                return (sql, selected, projectionCtx);
            }

            // Basic ORDER BY: ORDER BY clause applied to any query type
            case SelectClause(OrderByClause(var innerQuery, var keySelectors), var selector, var aliases):
            {
                var (fromClause, innerTuple, innerContext) = CompileFrom(innerQuery, context, scopeLevel);
                
                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, innerTuple, innerContext, scopeLevel);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, orderCtx, scopeLevel);
                
                // Format SQL with proper indentation
                var (indent, subIndent) = GetIndentation(scopeLevel);
                var sql = $$"""
                {{indent}}SELECT 
                {{projection}}
                {{indent}}FROM 
                {{subIndent}}{{fromClause}}
                {{indent}}ORDER BY 
                {{subIndent}}{{orderByClause}}
                """;
                return (sql, selected, projectionCtx);
            }

            // ========================================
            // WHERE PATTERNS
            // ========================================

            // Basic WHERE: WHERE clause applied to any query type
            case SelectClause(WhereClause(var innerQuery, var predicate), var selector, var aliases):
            {
                var (fromClause, innerTuple, innerContext) = CompileFrom(innerQuery, context, scopeLevel);
                
                // Apply WHERE clause to the result
                var (whereClause, whereCtx) = Compile(predicate(innerTuple), innerContext, scopeLevel);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, whereCtx, scopeLevel);
                
                // Format SQL with proper indentation
                var (indent, subIndent) = GetIndentation(scopeLevel);
                var sql = $$"""
                {{indent}}SELECT 
                {{projection}}
                {{indent}}FROM 
                {{subIndent}}{{fromClause}}
                {{indent}}WHERE 
                {{subIndent}}{{whereClause}}
                """;
                return (sql, selected, projectionCtx);
            }

            // ========================================
            // GENERAL PATTERNS
            // ========================================

            // Basic SELECT: General SELECT clause pattern
            case SelectClause(var innerQuery, var selector, var aliases):
            {
                var (innerSql, innerTuple, innerContext) = CompileFrom(innerQuery, context, scopeLevel);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, innerContext, scopeLevel);
                
                // Format SQL with proper indentation
                var (indent, subIndent) = GetIndentation(scopeLevel);
                var sql = $$"""
                {{indent}}SELECT 
                {{projection}}
                {{indent}}FROM 
                {{subIndent}}{{innerSql}}
                """;
                
                return (sql, selected, projectionCtx);
            }


            default:
                throw new NotSupportedException($"Query type {query.GetType().Name} is not supported");
        }
    }

    private static (string, ITuple, Context) CompileFrom(ISqlQuery query, Context context, int scopeLevel)
    {
        if (query is FromTableClause fromTable)
        {            
            var newContext = UpdateProjectionAliases(fromTable.Table, context);
            var aliasIndex = newContext.AliasIndex;
            return ($"{fromTable.Table.TableName} a{aliasIndex}", fromTable.Table, newContext);
        }
        if (query is FromSubQueryClause(var subQuery))
        {
            // Compile the subquery with increased indentation
            var (subQuerySql, tuple, subQueryCtx) = Compile(subQuery, context, scopeLevel + 1);
            var newContext = UpdateProjectionAliases(tuple, subQueryCtx);
            var aliasIndex = newContext.AliasIndex;
            return ($"({subQuerySql.TrimStart()}) a{aliasIndex}", tuple, newContext);
        }
        else
        {
            // Compile as subquery with increased indentation
            var (currentQuerySql, currentTuple, currentContext) = Compile(query, context, scopeLevel + 1);
            var newContext = UpdateProjectionAliases(currentTuple, currentContext);
            var aliasIndex = newContext.AliasIndex;
            return ($"({currentQuerySql.TrimStart()}) a{aliasIndex}", currentTuple, newContext);
        }
    }

    private static (ImmutableArray<string> JoinClauses, ITuple Tuple, Context Context) CompileJoin(ImmutableArray<(JoinType JoinType, ISqlTable Inner, Func<ITuple, SqlExpr> OuterKeySelector, Func<ITuple, SqlExpr> InnerKeySelector, Func<ITuple, ITuple, ITuple> ResultSelector, ImmutableArray<string?> Aliases)> joinData, ITuple currentTuple, Context currentContext, int scopeLevel)
    {
        if (joinData.IsEmpty)
            throw new InvalidOperationException("JoinClause must contain at least one join operation");

        var joinClauses = ImmutableArray.CreateBuilder<string>();
        var context = currentContext;

        // Process each join in sequence
        foreach (var (joinType, inner, outerKeySelector, innerKeySelector, resultSelector, joinAliases) in joinData)
        {                
            var innerContext = UpdateProjectionAliases(inner, context);
            var innerAliasIndex = innerContext.AliasIndex;

            // Extract the join condition using current tuple state
            var outerKey = outerKeySelector(currentTuple);
            var innerKey = innerKeySelector(inner);

            var (outerKeySql, outerKeyCtx) = Compile(outerKey, innerContext, scopeLevel);
            var (innerKeySql, innerKeyCtx) = Compile(innerKey, outerKeyCtx, scopeLevel);

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
            context = innerKeyCtx;
        }

        return (joinClauses.ToImmutable(), currentTuple, context);
    }

    /// <summary>
    /// Compiles SQL scalar queries to standalone SQL SELECT statements (without parentheses).
    /// Used for compiling aggregate functions as standalone queries.
    /// </summary>
    /// <param name="scalarQuery">The scalar query to compile</param>
    /// <param name="context">The compilation context</param>
    /// <param name="scopeLevel">The nesting scope level for the SQL statement</param>
    /// <returns>The SQL string representation and updated context</returns>
    public static (string, Context) Compile(ISqlScalarQuery scalarQuery, Context context, int scopeLevel)
    {
        switch (scalarQuery)
        {
            case SumSqlIntClause(var query):
            {
                var sumQuery = new SelectClause(query, tuple => ValueTuple.Create(new SqlIntSum((SqlExprInt)tuple[0]!)), []);
                var (sql, _, ctx) = Compile(sumQuery, context, scopeLevel);
                return (sql, ctx);
            }        

            case CountClause(var query):
            {
                var countQuery = new SelectClause(query, _ => ValueTuple.Create(new SqlIntCount()), []);
                var (sql, _, ctx) = Compile(countQuery, context, scopeLevel);
                return (sql, ctx);
            }

            default:
                throw new NotSupportedException($"Scalar query type {scalarQuery.GetType().Name} is not supported");
        }
    }

    /// <summary>
    /// Compiles ORDER BY clauses with key selector function.
    /// </summary>
    /// <param name="keySelector">Function that returns array of SQL expressions with sort direction</param>
    /// <param name="table">The table being queried</param>
    /// <param name="context">The compilation context</param>
    /// <param name="scopeLevel">The nesting scope level for the SQL statement</param>
    /// <returns>The compiled ORDER BY clause and updated context</returns>
    private static (string, Context) CompileOrderBy(Func<ITuple, ImmutableArray<(SqlExpr, Sort)>> keySelector, ITuple table, Context context, int scopeLevel)
    {
        var orderItems = new List<string>();
        var ctx = context;

        var keys = keySelector(table);
        foreach (var (expr, direction) in keys)
        {
            var (orderByClause, orderCtx) = Compile(expr, ctx, scopeLevel);
            orderItems.Add($"{orderByClause} {(direction == Sort.Desc ? "DESC" : "ASC")}");
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
    /// <param name="scopeLevel">The nesting scope level for the SQL statement</param>
    /// <returns>The compiled GROUP BY clause and updated context</returns>
    private static (string, Context) CompileGroupBy(Func<ITuple, ImmutableArray<SqlExpr>> keySelector, ITuple table, Context context, int scopeLevel)
    {
        var groupItems = new List<string>();
        var ctx = context;

        var keys = keySelector(table);
        foreach (var key in keys)
        {
            var (groupByClause, groupCtx) = Compile(key, ctx, scopeLevel);
            groupItems.Add(groupByClause);
            ctx = groupCtx;
        }

        return (string.Join(", ", groupItems), ctx);
    }
}
