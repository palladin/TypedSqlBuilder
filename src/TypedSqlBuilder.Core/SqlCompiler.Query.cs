using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections.Immutable;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;

namespace TypedSqlBuilder.Core;

/// <summary>
/// SQL Compiler - Query compilation methods.
/// This partial class contains all methods related to compiling SQL queries.
/// </summary>
internal static partial class SqlCompiler
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
        if (tuple is SqlTable sqlTable)
        {
            return sqlTable.Columns;
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
                case SqlTable table:
                {
                    flattened.AddRange(table.Columns);
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
    /// Generates a table-prefixed alias for a column to resolve naming conflicts.
    /// Converts table names like "customers" to "Customer" and combines with column name.
    /// </summary>
    /// <param name="sqlColumn">The SQL column to generate an alias for</param>
    /// <returns>A table-prefixed alias like "CustomerId"</returns>
    private static string GenerateTablePrefixedAlias(ISqlColumn sqlColumn)
    {
        var tableName = sqlColumn.TableName;
        var singularTable = tableName.TrimEnd('s'); // "customers" -> "customer"
        var capitalizedTable = char.ToUpper(singularTable[0]) + singularTable[1..]; // "Customer"
        return $"{capitalizedTable}{sqlColumn.ColumnName}"; // "CustomerId"
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

        // Track alias usage for O(1) conflict detection
        var aliasTracker = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);

        // Single pass: compile all expressions and track alias usage
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
            else if (expr is ISqlColumn sqlColumn)
            {
                var columnName = sqlColumn.ColumnName;
                fieldAlias = columnName; // Start with column name, will resolve conflicts later
            }
            else
            {
                fieldAlias = $"Proj{projectionCount++}";
            }
            
            items.Add((compiled, fieldAlias));
            
            // Track this alias usage
            if (!aliasTracker.TryGetValue(fieldAlias, out var indices))
            {
                indices = new List<int>();
                aliasTracker[fieldAlias] = indices;
            }
            indices.Add(i);
        }
        
        // Resolve conflicts for aliases used multiple times
        foreach (var (alias, indices) in aliasTracker)
        {
            if (indices.Count > 1)
            {
                // Multiple items use the same alias - apply table prefixing to resolve conflicts
                foreach (var index in indices)
                {
                    if (flattenedExprs[index] is ISqlColumn sqlColumn)
                    {
                        var (projection, _) = items[index];
                        var newAlias = GenerateTablePrefixedAlias(sqlColumn);
                        items[index] = (projection, newAlias);
                    }
                }
            }
        }

        // Second pass: update projection aliases after all expressions are compiled
        foreach (var (expr, (_, fieldAlias)) in flattenedExprs.Zip(items))
        {
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

    internal static (string, ITuple, Context) Compile(ISqlQuery query, Context context, int scopeLevel)
    {
        // First normalize the query to apply all fusion rules
        var normalizedQuery = Normalize(query);
        
        // Implement pattern matches in reverse order (most specific to most general)
        switch (normalizedQuery)
        {
            // 16. select(orderby(groupby(where(join(rest)))))
            case SelectClause(OrderByClause(GroupByClause(WhereClause(JoinClause(var outer, var joinData), var predicate), var keySelector, var havingPredicate), var keySelectors), var selector, var aliases):
            {
                var (fromClause, currentTuple, currentContext) = CompileFrom(outer, context, scopeLevel);
                var (joinClauses, updatedTuple, joinContext) = CompileJoin(joinData, currentTuple, currentContext, scopeLevel);
                var (whereClause, whereCtx) = Compile(predicate(updatedTuple), joinContext, scopeLevel);
                var (groupByClause, groupCtx) = CompileGroupBy(keySelector, updatedTuple, whereCtx, scopeLevel);
                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, updatedTuple, groupCtx, scopeLevel);
                var selected = selector(updatedTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, orderCtx, scopeLevel);

                var (indent, subIndent) = GetIndentation(scopeLevel);
                string sql;
                Context finalCtx;

                if (havingPredicate is not null)
                {
                    var (havingSql, havingCtx) = Compile(havingPredicate(updatedTuple, new SqlAggregateFunc()), groupCtx, scopeLevel);
                    sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}{{string.Join($"\n{indent}", joinClauses)}}
                    {{indent}}WHERE 
                    {{subIndent}}{{whereClause}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    {{indent}}HAVING 
                    {{subIndent}}{{havingSql}}
                    {{indent}}ORDER BY 
                    {{subIndent}}{{orderByClause}}
                    """;
                    finalCtx = havingCtx;
                }
                else
                {
                    sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}{{string.Join($"\n{indent}", joinClauses)}}
                    {{indent}}WHERE 
                    {{subIndent}}{{whereClause}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    {{indent}}ORDER BY 
                    {{subIndent}}{{orderByClause}}
                    """;
                    finalCtx = projectionCtx;
                }
                return (sql, selected, finalCtx);
            }

            // 15. select(orderby(groupby(where(rest))))
            case SelectClause(OrderByClause(GroupByClause(WhereClause(var innerQuery, var predicate), var keySelector, var havingPredicate), var keySelectors), var selector, var aliases):
            {
                var (fromClause, innerTuple, innerContext) = CompileFrom(innerQuery, context, scopeLevel);
                var (whereClause, whereCtx) = Compile(predicate(innerTuple), innerContext, scopeLevel);
                var (groupByClause, groupCtx) = CompileGroupBy(keySelector, innerTuple, whereCtx, scopeLevel);
                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, innerTuple, groupCtx, scopeLevel);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, orderCtx, scopeLevel);

                var (indent, subIndent) = GetIndentation(scopeLevel);
                string sql;
                Context finalCtx;

                if (havingPredicate is not null)
                {
                    var (havingSql, havingCtx) = Compile(havingPredicate(innerTuple, new SqlAggregateFunc()), groupCtx, scopeLevel);
                    sql = $$"""
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
                    {{indent}}ORDER BY 
                    {{subIndent}}{{orderByClause}}
                    """;
                    finalCtx = havingCtx;
                }
                else
                {
                    sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}WHERE 
                    {{subIndent}}{{whereClause}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    {{indent}}ORDER BY 
                    {{subIndent}}{{orderByClause}}
                    """;
                    finalCtx = projectionCtx;
                }
                return (sql, selected, finalCtx);
            }

            // 14. select(orderby(groupby(join(rest))))
            case SelectClause(OrderByClause(GroupByClause(JoinClause(var outer, var joinData), var keySelector, var havingPredicate), var keySelectors), var selector, var aliases):
            {
                var (fromClause, currentTuple, currentContext) = CompileFrom(outer, context, scopeLevel);
                var (joinClauses, updatedTuple, joinContext) = CompileJoin(joinData, currentTuple, currentContext, scopeLevel);
                var (groupByClause, groupCtx) = CompileGroupBy(keySelector, updatedTuple, joinContext, scopeLevel);
                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, updatedTuple, groupCtx, scopeLevel);
                var selected = selector(updatedTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, orderCtx, scopeLevel);

                var (indent, subIndent) = GetIndentation(scopeLevel);
                string sql;
                Context finalCtx;

                if (havingPredicate is not null)
                {
                    var (havingSql, havingCtx) = Compile(havingPredicate(updatedTuple, new SqlAggregateFunc()), groupCtx, scopeLevel);
                    sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}{{string.Join($"\n{indent}", joinClauses)}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    {{indent}}HAVING 
                    {{subIndent}}{{havingSql}}
                    {{indent}}ORDER BY 
                    {{subIndent}}{{orderByClause}}
                    """;
                    finalCtx = havingCtx;
                }
                else
                {
                    sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}{{string.Join($"\n{indent}", joinClauses)}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    {{indent}}ORDER BY 
                    {{subIndent}}{{orderByClause}}
                    """;
                    finalCtx = projectionCtx;
                }
                return (sql, selected, finalCtx);
            }

            // 13. select(orderby(groupby(rest)))
            case SelectClause(OrderByClause(GroupByClause(var innerQuery, var keySelector, var havingPredicate), var keySelectors), var selector, var aliases):
            {
                var (fromClause, innerTuple, innerContext) = CompileFrom(innerQuery, context, scopeLevel);
                var (groupByClause, groupCtx) = CompileGroupBy(keySelector, innerTuple, innerContext, scopeLevel);
                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, innerTuple, groupCtx, scopeLevel);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, orderCtx, scopeLevel);

                var (indent, subIndent) = GetIndentation(scopeLevel);
                string sql;
                Context finalCtx;

                if (havingPredicate is not null)
                {
                    var (havingSql, havingCtx) = Compile(havingPredicate(innerTuple, new SqlAggregateFunc()), groupCtx, scopeLevel);
                    sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    {{indent}}HAVING 
                    {{subIndent}}{{havingSql}}
                    {{indent}}ORDER BY 
                    {{subIndent}}{{orderByClause}}
                    """;
                    finalCtx = havingCtx;
                }
                else
                {
                    sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    {{indent}}ORDER BY 
                    {{subIndent}}{{orderByClause}}
                    """;
                    finalCtx = projectionCtx;
                }
                return (sql, selected, finalCtx);
            }

            // 12. select(orderby(where(join(rest))))
            case SelectClause(OrderByClause(WhereClause(JoinClause(var outer, var joinData), var predicate), var keySelectors), var selector, var aliases):
            {
                var (fromClause, currentTuple, currentContext) = CompileFrom(outer, context, scopeLevel);
                var (joinClauses, updatedTuple, joinContext) = CompileJoin(joinData, currentTuple, currentContext, scopeLevel);
                var (whereClause, whereCtx) = Compile(predicate(updatedTuple), joinContext, scopeLevel);
                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, updatedTuple, whereCtx, scopeLevel);
                var selected = selector(updatedTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, orderCtx, scopeLevel);

                var (indent, subIndent) = GetIndentation(scopeLevel);
                var sql = $$"""
                {{indent}}SELECT 
                {{projection}}
                {{indent}}FROM 
                {{subIndent}}{{fromClause}}
                {{indent}}{{string.Join($"\n{indent}", joinClauses)}}
                {{indent}}WHERE 
                {{subIndent}}{{whereClause}}
                {{indent}}ORDER BY 
                {{subIndent}}{{orderByClause}}
                """;
                return (sql, selected, projectionCtx);
            }

            // 11. select(orderby(where(rest)))
            case SelectClause(OrderByClause(WhereClause(var innerQuery, var predicate), var keySelectors), var selector, var aliases):
            {
                var (fromClause, innerTuple, innerContext) = CompileFrom(innerQuery, context, scopeLevel);
                var (whereClause, whereCtx) = Compile(predicate(innerTuple), innerContext, scopeLevel);
                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, innerTuple, whereCtx, scopeLevel);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, orderCtx, scopeLevel);

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

            // 10. select(orderby(join(rest)))
            case SelectClause(OrderByClause(JoinClause(var outer, var joinData), var keySelectors), var selector, var aliases):
            {
                var (fromClause, currentTuple, currentContext) = CompileFrom(outer, context, scopeLevel);
                var (joinClauses, updatedTuple, joinContext) = CompileJoin(joinData, currentTuple, currentContext, scopeLevel);
                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, updatedTuple, joinContext, scopeLevel);
                var selected = selector(updatedTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, orderCtx, scopeLevel);

                var (indent, subIndent) = GetIndentation(scopeLevel);
                var sql = $$"""
                {{indent}}SELECT 
                {{projection}}
                {{indent}}FROM 
                {{subIndent}}{{fromClause}}
                {{indent}}{{string.Join($"\n{indent}", joinClauses)}}
                {{indent}}ORDER BY 
                {{subIndent}}{{orderByClause}}
                """;
                return (sql, selected, projectionCtx);
            }

            // 9. select(orderby(rest))
            case SelectClause(OrderByClause(var innerQuery, var keySelectors), var selector, var aliases):
            {
                var (fromClause, innerTuple, innerContext) = CompileFrom(innerQuery, context, scopeLevel);
                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, innerTuple, innerContext, scopeLevel);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, orderCtx, scopeLevel);

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

            // 8. select(groupby(where(join(rest))))
            case SelectClause(GroupByClause(WhereClause(JoinClause(var outer, var joinData), var predicate), var keySelector, var havingPredicate), var selector, var aliases):
            {
                var (fromClause, currentTuple, currentContext) = CompileFrom(outer, context, scopeLevel);
                var (joinClauses, updatedTuple, joinContext) = CompileJoin(joinData, currentTuple, currentContext, scopeLevel);
                var (whereClause, whereCtx) = Compile(predicate(updatedTuple), joinContext, scopeLevel);
                var (groupByClause, groupCtx) = CompileGroupBy(keySelector, updatedTuple, whereCtx, scopeLevel);
                var selected = selector(updatedTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, groupCtx, scopeLevel);

                var (indent, subIndent) = GetIndentation(scopeLevel);
                string sql;
                Context finalCtx;

                if (havingPredicate is not null)
                {
                    var (havingSql, havingCtx) = Compile(havingPredicate(updatedTuple, new SqlAggregateFunc()), groupCtx, scopeLevel);
                    sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}{{string.Join($"\n{indent}", joinClauses)}}
                    {{indent}}WHERE 
                    {{subIndent}}{{whereClause}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    {{indent}}HAVING 
                    {{subIndent}}{{havingSql}}
                    """;
                    finalCtx = havingCtx;
                }
                else
                {
                    sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}{{string.Join($"\n{indent}", joinClauses)}}
                    {{indent}}WHERE 
                    {{subIndent}}{{whereClause}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    """;
                    finalCtx = projectionCtx;
                }
                return (sql, selected, finalCtx);
            }

            // 7. select(groupby(where(rest)))
            case SelectClause(GroupByClause(WhereClause(var innerQuery, var predicate), var keySelector, var havingPredicate), var selector, var aliases):
            {
                var (fromClause, innerTuple, innerContext) = CompileFrom(innerQuery, context, scopeLevel);
                var (whereClause, whereCtx) = Compile(predicate(innerTuple), innerContext, scopeLevel);
                var (groupByClause, groupCtx) = CompileGroupBy(keySelector, innerTuple, whereCtx, scopeLevel);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, groupCtx, scopeLevel);

                var (indent, subIndent) = GetIndentation(scopeLevel);
                string sql;
                Context finalCtx;

                if (havingPredicate is not null)
                {
                    var (havingSql, havingCtx) = Compile(havingPredicate(innerTuple, new SqlAggregateFunc()), groupCtx, scopeLevel);
                    sql = $$"""
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
                    finalCtx = havingCtx;
                }
                else
                {
                    sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}WHERE 
                    {{subIndent}}{{whereClause}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    """;
                    finalCtx = projectionCtx;
                }
                return (sql, selected, finalCtx);
                }

            // 6. select(groupby(join(rest)))
            case SelectClause(GroupByClause(JoinClause(var outer, var joinData), var keySelector, var havingPredicate), var selector, var aliases):
            {
                var (fromClause, currentTuple, currentContext) = CompileFrom(outer, context, scopeLevel);
                var (joinClauses, updatedTuple, joinContext) = CompileJoin(joinData, currentTuple, currentContext, scopeLevel);
                var (groupByClause, groupCtx) = CompileGroupBy(keySelector, updatedTuple, joinContext, scopeLevel);
                var selected = selector(updatedTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, groupCtx, scopeLevel);

                var (indent, subIndent) = GetIndentation(scopeLevel);
                string sql;
                Context finalCtx;

                if (havingPredicate is not null)
                {
                    var (havingSql, havingCtx) = Compile(havingPredicate(updatedTuple, new SqlAggregateFunc()), groupCtx, scopeLevel);
                    sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}{{string.Join($"\n{indent}", joinClauses)}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    {{indent}}HAVING 
                    {{subIndent}}{{havingSql}}
                    """;
                    finalCtx = havingCtx;
                }
                else
                {
                    sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}{{string.Join($"\n{indent}", joinClauses)}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    """;
                    finalCtx = projectionCtx;
                }
                return (sql, selected, finalCtx);
            }

            // 5. select(groupby(rest))
            case SelectClause(GroupByClause(var innerQuery, var keySelector, var havingPredicate), var selector, var aliases):
            {
                var (fromClause, innerTuple, innerContext) = CompileFrom(innerQuery, context, scopeLevel);
                var (groupByClause, groupCtx) = CompileGroupBy(keySelector, innerTuple, innerContext, scopeLevel);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, groupCtx, scopeLevel);

                var (indent, subIndent) = GetIndentation(scopeLevel);
                string sql;
                Context finalCtx;

                if (havingPredicate is not null)
                {
                    var (havingSql, havingCtx) = Compile(havingPredicate(innerTuple, new SqlAggregateFunc()), groupCtx, scopeLevel);
                    sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    {{indent}}HAVING 
                    {{subIndent}}{{havingSql}}
                    """;
                    finalCtx = havingCtx;
                }
                else
                {
                    sql = $$"""
                    {{indent}}SELECT 
                    {{projection}}
                    {{indent}}FROM 
                    {{subIndent}}{{fromClause}}
                    {{indent}}GROUP BY 
                    {{subIndent}}{{groupByClause}}
                    """;
                    finalCtx = projectionCtx;
                }
                return (sql, selected, finalCtx);
            }

            // 4. select(where(join(rest)))
            case SelectClause(WhereClause(JoinClause(var outer, var joinData), var predicate), var selector, var aliases):
            {
                var (fromClause, currentTuple, currentContext) = CompileFrom(outer, context, scopeLevel);
                var (joinClauses, updatedTuple, joinContext) = CompileJoin(joinData, currentTuple, currentContext, scopeLevel);
                var (whereClause, whereCtx) = Compile(predicate(updatedTuple), joinContext, scopeLevel);
                var selected = selector(updatedTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, whereCtx, scopeLevel);

                var (indent, subIndent) = GetIndentation(scopeLevel);
                var sql = $$"""
                {{indent}}SELECT 
                {{projection}}
                {{indent}}FROM 
                {{subIndent}}{{fromClause}}
                {{indent}}{{string.Join($"\n{indent}", joinClauses)}}
                {{indent}}WHERE 
                {{subIndent}}{{whereClause}}
                """;
                return (sql, selected, projectionCtx);
            }

            // 3. select(where(rest))
            case SelectClause(WhereClause(var innerQuery, var predicate), var selector, var aliases):
            {
                var (fromClause, innerTuple, innerContext) = CompileFrom(innerQuery, context, scopeLevel);
                var (whereClause, whereCtx) = Compile(predicate(innerTuple), innerContext, scopeLevel);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, whereCtx, scopeLevel);

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

            // 2. select(join(rest))
            case SelectClause(JoinClause(var outer, var joinData), var selector, var aliases):
            {
                var (fromClause, currentTuple, currentContext) = CompileFrom(outer, context, scopeLevel);
                var (joinClauses, updatedTuple, finalContext) = CompileJoin(joinData, currentTuple, currentContext, scopeLevel);
                var selected = selector(updatedTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, finalContext, scopeLevel);

                var (indent, subIndent) = GetIndentation(scopeLevel);
                var sql = $$"""
                {{indent}}SELECT 
                {{projection}}
                {{indent}}FROM 
                {{subIndent}}{{fromClause}}
                {{indent}}{{string.Join($"\n{indent}", joinClauses)}}
                """;
                return (sql, selected, projectionCtx);
            }

            // 1. select(rest)
            case SelectClause(var innerQuery, var selector, var aliases):
            {
                var (innerSql, innerTuple, innerContext) = CompileFrom(innerQuery, context, scopeLevel);
                var selected = selector(innerTuple);
                var (projection, projectionCtx) = CompileTupleProjection(selected, aliases, innerContext, scopeLevel);

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
                throw new NotSupportedException($"Query type {normalizedQuery.GetType().Name} is not supported");
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

    private static (ImmutableArray<string> JoinClauses, ITuple Tuple, Context Context) CompileJoin(ImmutableArray<(JoinType JoinType, SqlTable Inner, Func<ITuple, SqlExpr> OuterKeySelector, Func<ITuple, SqlExpr> InnerKeySelector, Func<ITuple, ITuple, ITuple> ResultSelector, ImmutableArray<string?> Aliases)> joinData, ITuple currentTuple, Context currentContext, int scopeLevel)
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

            case AvgSqlIntClause(var query):
            {
                var avgQuery = new SelectClause(query, tuple => ValueTuple.Create(new SqlIntAvg((SqlExprInt)tuple[0]!)), []);
                var (sql, _, ctx) = Compile(avgQuery, context, scopeLevel);
                return (sql, ctx);
            }

            case MinSqlIntClause(var query):
            {
                var minQuery = new SelectClause(query, tuple => ValueTuple.Create(new SqlIntMin((SqlExprInt)tuple[0]!)), []);
                var (sql, _, ctx) = Compile(minQuery, context, scopeLevel);
                return (sql, ctx);
            }

            case MaxSqlIntClause(var query):
            {
                var maxQuery = new SelectClause(query, tuple => ValueTuple.Create(new SqlIntMax((SqlExprInt)tuple[0]!)), []);
                var (sql, _, ctx) = Compile(maxQuery, context, scopeLevel);
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
