using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections.Immutable;

namespace TypedSqlBuilder.Core;

public record SqlExprAlias(string Name, string Field);

public record Context
{
    public ImmutableDictionary<SqlExpr, SqlExprAlias> ProjectionAliases { get; init; } = ImmutableDictionary<SqlExpr, SqlExprAlias>.Empty;

    public int AliasIndex { get; init; } = -1;

    public ImmutableDictionary<ISqlTable, int> TableAliases { get; init; } = ImmutableDictionary<ISqlTable, int>.Empty;

    public ImmutableDictionary<string, object> Parameters { get; init; } = ImmutableDictionary<string, object>.Empty;

    public Context AddParameter(string name, object value)
    {
        return this with { Parameters = Parameters.Add(name, value) };
    }

    public (string paramName, Context newContext) GenerateParameter(object value, string prefix = "@")
    {
        var paramName = $"{prefix}p{Parameters.Count}";
        return (paramName, AddParameter(paramName, value));
    }

    public (int aliasIndex, Context newContext) GetOrAddTableAlias(ISqlTable table)
    {
        if (TableAliases.TryGetValue(table, out var existingAlias))
        {
            return (existingAlias, this);
        }
        
        var newAliasIndex = AliasIndex + 1;
        var newContext = this with 
        { 
            AliasIndex = newAliasIndex,
            TableAliases = TableAliases.Add(table, newAliasIndex)
        };
        return (newAliasIndex, newContext);
    }

    public (int aliasIndex, Context newContext) GetOrAddTableAliasForStatement(ISqlTable table)
    {
        if (TableAliases.TryGetValue(table, out var existingAlias))
        {
            return (existingAlias, this);
        }
        
        // For statements, use AliasIndex directly (not incremented)
        var newAliasIndex = AliasIndex + 1;
        var newContext = this with 
        { 
            TableAliases = TableAliases.Add(table, newAliasIndex)
        };
        return (newAliasIndex, newContext);
    }
}

/// <summary>
/// Synthetic table representation for subqueries in FROM clauses.
/// Allows the compiler to treat subqueries as tables with projected columns.
/// </summary>
internal class SubQueryTable : ISqlTable
{
    private readonly object[] _columns;
    
    public string TableName { get; }
    public ISqlQuery SubQuery { get; }
    
    public SubQueryTable(string tableAlias, ISqlQuery subQuery)
    {
        TableName = tableAlias;
        SubQuery = subQuery;
        
        // For now, create a placeholder array - we'll populate it based on the subquery's projection
        _columns = new object[10]; // arbitrary size, will be populated dynamically
    }
    
    public object? this[int index] => _columns[index];
    public int Length => _columns.Length;
}

/// <summary>
/// Abstract base class for SQL compilation that can properly handle typed queries and lambda expressions.
/// This version can evaluate selectors and predicates to generate correct SQL for different database providers.
/// </summary>
public abstract class SqlCompiler
{
    private static readonly SqliteCompiler _sqliteCompiler = new SqliteCompiler();
    private static readonly SqlServerCompiler _sqlServerCompiler = new SqlServerCompiler();

    /// <summary>
    /// Gets the SQLite-specific query compiler.
    /// </summary>
    public static SqliteCompiler Sqlite => _sqliteCompiler;

    /// <summary>
    /// Gets the SQL Server-specific query compiler.
    /// </summary>
    public static SqlServerCompiler SqlServer => _sqlServerCompiler;

    /// <summary>
    /// Gets the parameter prefix used by this database provider.
    /// </summary>
    protected virtual string ParameterPrefix => "@";

    /// <summary>
    /// Generates a parameter for the given value using the database-specific parameter prefix.
    /// </summary>
    protected (string paramName, Context newContext) GenerateParameter(Context context, object value)
    {
        return context.GenerateParameter(value, ParameterPrefix);
    }

    /// <summary>
    /// Normalizes a SQL query by applying fusion rules until a fixpoint is reached.
    /// Moves all fusion logic from extension methods to centralized normalization.
    /// Handles WHERE clause fusion, ORDER BY fusion, and SELECT composition.
    /// </summary>
    /// <param name="query">The SQL query to normalize</param>
    /// <returns>The normalized query with all possible fusions applied</returns>
    public virtual ISqlQuery Normalize(ISqlQuery query)
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
    private (ISqlQuery Query, bool Changed) ApplyNormalizationRules(ISqlQuery query)
    {
        return query switch
        {
            // WHERE clause fusion: WHERE(WHERE(q, p1), p2) → WHERE(q, p1 AND p2)
            WhereClause(WhereClause(var innerQuery, var innerPred), var outerPred) =>
                (new WhereClause(innerQuery, tuple => innerPred(tuple) && outerPred(tuple)), true),
            
            // ORDER BY fusion: OrderBy(OrderBy(q, keys1), keys2) → OrderBy(q, keys1 + keys2)  
            OrderByClause(OrderByClause(var innerQuery, var innerKeys), var outerKeys) =>
                (new OrderByClause(innerQuery, innerKeys.AddRange(outerKeys)), true),
            
            // SELECT composition: SELECT(SELECT(q, f1), f2) → SELECT(q, f2 ∘ f1)
            SelectClause(SelectClause(var innerQuery, var innerSelector), var outerSelector) =>
                (new SelectClause(innerQuery, tuple => outerSelector(innerSelector(tuple))), true),
            
            // Recursive normalization of subqueries - WHERE
            WhereClause(var subQuery, var predicate) =>
                ApplyToSubQuery(subQuery, q => new WhereClause(q, predicate)),
            
            // Recursive normalization of subqueries - SELECT  
            SelectClause(var subQuery, var selector) =>
                ApplyToSubQuery(subQuery, q => new SelectClause(q, selector)),
            
            // Recursive normalization of subqueries - ORDER BY
            OrderByClause(var subQuery, var keySelectors) =>
                ApplyToSubQuery(subQuery, q => new OrderByClause(q, keySelectors)),
            
            // No changes needed
            _ => (query, false)
        };
    }

    /// <summary>
    /// Helper method to normalize subqueries and reconstruct parent queries.
    /// </summary>
    /// <param name="subQuery">The subquery to normalize</param>
    /// <param name="constructor">Function to reconstruct the parent query with the normalized subquery</param>
    /// <returns>A tuple containing the reconstructed query and whether any changes were made</returns>
    private (ISqlQuery Query, bool Changed) ApplyToSubQuery(ISqlQuery subQuery, Func<ISqlQuery, ISqlQuery> constructor)
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
    private ISqlQuery ApplyCanonicalForm(ISqlQuery query)
    {
        return query switch
        {
            // Wrap non-SELECT queries with SELECT * to match compiler patterns
            WhereClause(_, var predicate) =>
                new SelectClause(query, tuple => tuple),
                
            OrderByClause(_, var keySelectors) =>
                new SelectClause(query, tuple => tuple),
                
            FromTableClause(var table) =>
                new SelectClause(query, tuple => tuple),
            
            // Already in canonical form
            _ => query
        };
    }

    /// <summary>
    /// Compiles SQL queries and clauses to SQL string representation.
    /// </summary>
    /// <param name="query">The SQL query or clause to compile</param>
    /// <returns>The SQL string representation</returns>
    public virtual (string, ITuple, Context) Compile(ISqlQuery query, Context context)
    {
        // First normalize the query to apply all fusion rules
        var normalizedQuery = Normalize(query);
        
        switch (normalizedQuery)
        {
            case SelectClause(FromTableClause(var table), var selector):
            {
                var (aliasIndex, newContext) = context.GetOrAddTableAlias(table);
                var selected = selector(table);
                var (projection, projectionCtx) = CompileProjection(selected, table, aliasIndex, newContext);
                return ($"SELECT {projection} FROM {table.TableName} a{aliasIndex}", selected, projectionCtx);
            }

            case SelectClause(FromSubQueryClause(var subQuery), var selector):
            {
                // Compile the subquery
                var (subQuerySql, tuple, subQueryCtx) = Compile(subQuery, context);                              
                var newContext = UpdateProjectionAliases(tuple, subQueryCtx);
                var aliasIndex = newContext.AliasIndex;

                var selected = selector(tuple);
                var (projection, projectionCtx) = CompileProjection(selected, tuple, aliasIndex, newContext);
                return ($"SELECT {projection} FROM ({subQuerySql}) a{aliasIndex}", selected, projectionCtx);
            }

            case SelectClause(WhereClause(FromTableClause(var table), var predicate), var selector):
            {
                var (aliasIndex, newContext) = context.GetOrAddTableAlias(table);

                var (whereClause, whereCtx) = Compile(predicate(table), newContext);
                var selected = selector(table);
                var (projection, projectionCtx) = CompileProjection(selected, table, aliasIndex, whereCtx);
                return ($"SELECT {projection} FROM {table.TableName} a{aliasIndex} WHERE {whereClause}", selected, projectionCtx);
            }

            case SelectClause(OrderByClause(FromTableClause(var table), var keySelectors), var selector):
            {
                var (aliasIndex, newContext) = context.GetOrAddTableAlias(table);

                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, table, newContext);
                var selected = selector(table);
                var (projection, projectionCtx) = CompileProjection(selected, table, aliasIndex, orderCtx);
                return ($"SELECT {projection} FROM {table.TableName} a{aliasIndex} ORDER BY {orderByClause}", selected, projectionCtx);
            }

            case SelectClause(OrderByClause(WhereClause(FromTableClause(var table), var predicate), var keySelectors), var selector):
            {
                var (aliasIndex, newContext) = context.GetOrAddTableAlias(table);

                var (whereClause, whereCtx) = Compile(predicate(table), newContext);
                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, table, whereCtx);
                var selected = selector(table);
                var (projection, projectionCtx) = CompileProjection(selected, table, aliasIndex, orderCtx);
                return ($"SELECT {projection} FROM {table.TableName} a{aliasIndex} WHERE {whereClause} ORDER BY {orderByClause}", selected, projectionCtx);
            }

            default:
                throw new NotSupportedException($"Query type {query.GetType().Name} is not supported");
        }
    }

    /// <summary>
    /// Compiles SQL statements to SQL string representation.
    /// </summary>
    /// <param name="statement">The SQL statement to compile</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The SQL string representation and updated context</returns>
    public virtual (string, Context) Compile(ISqlStatement statement, Context context)
    {
        switch (statement)
        {
            // ========== DELETE STATEMENTS ==========
            case DeleteStatement(var table):
                return ($"DELETE FROM {table.TableName}", context);

            case DeleteWhereStatement(DeleteStatement(var table), var predicate):
            {                
                var (whereClause, whereCtx) = Compile(predicate(table), context);
                return ($"DELETE FROM {table.TableName} WHERE {whereClause}", whereCtx);
            }

            // ========== UPDATE STATEMENTS ==========
            // Note: Bare UpdateStatement without SET clauses should not be compiled
            // UPDATE statements must have at least one SET clause via SetStatement fusion
            case UpdateStatement(var table):
                throw new InvalidOperationException($"UPDATE statement for table {table.TableName} must have at least one SET clause. Use .Set(...) to add SET clauses.");

            // ========== INSERT STATEMENTS ==========
            // Note: Bare InsertStatement without VALUE clauses should not be compiled
            // INSERT statements must have at least one VALUE clause via ValueStatement fusion
            case InsertStatement(var table):
                throw new InvalidOperationException($"INSERT statement for table {table.TableName} must have at least one VALUE clause. Use .Value(...) to add VALUE clauses.");

            // ========== STATEMENT FUSION PATTERNS ==========
            // SET fusion: SET(SET(...), setClause) → SET(..., setClause) (recursive)
            // Also handles SET(UPDATE(table), setClause) → UPDATE(table, [setClause])
            case SetStatement(var innerStatement, var setClause):
            {
                var (table, setClauses) = ExtractUpdateTableAndClauses(innerStatement);                
                var newSetClauses = setClauses.Append(setClause).ToImmutableArray();
                var (setClauseSql, setCtx) = CompileSetClauses(newSetClauses, table, context);
                return ($"UPDATE {table.TableName} SET {setClauseSql}", setCtx);
            }    

            // WHERE fusion for UPDATE: WHERE(UPDATE(...), predicate) or WHERE(SET(...), predicate)
            case UpdateWhereFromStatement(var updateStatement, var predicate):
            {
                var (table, setClauses) = ExtractUpdateTableAndClauses(updateStatement);                
                var (setClauseSql, setCtx) = CompileSetClauses(setClauses, table, context);
                var (whereClause, whereCtx) = Compile(predicate(table), setCtx);
                return ($"UPDATE {table.TableName} SET {setClauseSql} WHERE {whereClause}", whereCtx);
            }        

            // ========== STATEMENT FUSION PATTERNS ==========
            case ValueStatement(var innerStatement, var valueClause):
            {
                var (table, valueClauses) = ExtractInsertTableAndClauses(innerStatement);
                var newValueClauses = valueClauses.Append(valueClause).ToImmutableArray();                
                var (columnsClause, valuesClause, valuesCtx) = CompileInsertValueClauses(newValueClauses, table, context);
                return ($"INSERT INTO {table.TableName} ({columnsClause}) VALUES ({valuesClause})", valuesCtx);            
            }            

            default:
                throw new NotSupportedException($"Statement type {statement.GetType().Name} is not supported");
        }
    }

    /// <summary>
    /// Compiles SQL scalar queries to standalone SQL SELECT statements (without parentheses).
    /// Used for compiling aggregate functions as standalone queries.
    /// </summary>
    /// <param name="scalarQuery">The scalar query to compile</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The SQL string representation and updated context</returns>
    public virtual (string, Context) Compile(ISqlScalarQuery scalarQuery, Context context)
    {
        switch (scalarQuery)
        {
            case SumSqlIntClause(var query):
            {
                var sumQuery = new SelectClause(query, tuple => ValueTuple.Create(new SqlIntSum((SqlExprInt)tuple[0]!)));
                var (sql, _, ctx) = Compile(sumQuery, context);
                return (sql, ctx);
            }        

            case CountClause(var query):
            {
                var countQuery = new SelectClause(query, _ => ValueTuple.Create(new SqlIntCount()));
                var (sql, _, ctx) = Compile(countQuery, context);
                return (sql, ctx);
            }

            default:
                throw new NotSupportedException($"Scalar query type {scalarQuery.GetType().Name} is not supported");
        }
    }

    /// <summary>
    /// Compiles boolean expressions to SQL string representation.
    /// </summary>
    /// <param name="expr">The boolean expression to compile</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The SQL string representation and updated context</returns>
    protected virtual (string, Context) CompileExprBool(SqlExprBool expr, Context context)
    {
        switch (expr)
        {
            // Literal values
            case SqlBoolValue(var value):
            {
                var (paramName, newContext) = GenerateParameter(context, value);
                return (paramName, newContext);
            }

            // Logical operations
            case SqlBoolNot(var operand):
            {
                var (compiled, ctx) = Compile(operand, context);
                return ($"NOT ({compiled})", ctx);
            }

            case SqlBoolAnd(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"({leftSql}) AND ({rightSql})", rightCtx);
            }

            case SqlBoolOr(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"({leftSql}) OR ({rightSql})", rightCtx);
            }

            // Generic equality/inequality comparisons - handles ALL types (Bool, Int, String)
            case SqlEquals(var leftExpr, ISqlNullValue _):
            {
                var (exprSql, exprCtx) = Compile(leftExpr, context);
                return ($"{exprSql} IS NULL", exprCtx);
            }

            case SqlEquals(ISqlNullValue _, var rightExpr):
            {
                var (exprSql, exprCtx) = Compile(rightExpr, context);
                return ($"{exprSql} IS NULL", exprCtx);
            }

            case SqlEquals(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} = {rightSql}", rightCtx);
            }

            case SqlNotEquals(var leftExpr, ISqlNullValue _):
            {
                var (exprSql, exprCtx) = Compile(leftExpr, context);
                return ($"{exprSql} IS NOT NULL", exprCtx);
            }

            case SqlNotEquals(ISqlNullValue _, var rightExpr):
            {
                var (exprSql, exprCtx) = Compile(rightExpr, context);
                return ($"{exprSql} IS NOT NULL", exprCtx);
            }

            case SqlNotEquals(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} != {rightSql}", rightCtx);
            }

            case SqlGreaterThan(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} > {rightSql}", rightCtx);
            }

            case SqlLessThan(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} < {rightSql}", rightCtx);
            }

            case SqlGreaterThanOrEqualTo(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} >= {rightSql}", rightCtx);
            }

            case SqlLessThanOrEqualTo(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} <= {rightSql}", rightCtx);
            }

            // String pattern matching
            case SqlStringLike(var value, var pattern):
            {
                var (valueSql, valueCtx) = Compile(value, context);
                var (patternParam, patternCtx) = GenerateParameter(valueCtx, pattern);
                return ($"{valueSql} LIKE {patternParam}", patternCtx);
            }

            // Parameters
            case SqlParameterBool(var name):
                return (name, context);

            // CASE expressions
            case SqlBoolCase(var condition, var trueValue, var falseValue):
            {
                var (conditionSql, conditionCtx) = Compile(condition, context);
                var (trueSql, trueCtx) = Compile(trueValue, conditionCtx);
                var (falseSql, falseCtx) = Compile(falseValue, trueCtx);
                return ($"CASE WHEN {conditionSql} THEN {trueSql} ELSE {falseSql} END", falseCtx);
            }

            // NULL value
            case SqlBoolNull:
                return ("NULL", context);

            // IN clause
            case SqlInValues(var expression, var values):
            {
                if (values.IsEmpty)
                    return ("FALSE", context); // Empty IN clause is always false
                    
                var (exprSql, ctx) = Compile(expression, context);
                var valuesSql = new List<string>();
                var currentCtx = ctx;
                
                foreach (var value in values)
                {
                    var (valueSql, nextCtx) = Compile(value, currentCtx);
                    valuesSql.Add(valueSql);
                    currentCtx = nextCtx;
                }
                
                return ($"{exprSql} IN ({string.Join(", ", valuesSql)})", currentCtx);
            }

            // IN clause with subquery
            case SqlInSubQuery(var expression, var subQuery):
            {
                var (exprSql, ctx) = Compile(expression, context);
                var (subQuerySql, _, finalCtx) = Compile((ISqlQuery)subQuery, ctx);
                
                return ($"{exprSql} IN ({subQuerySql})", finalCtx);
            }

            default:
                throw new NotSupportedException($"Boolean expression type {expr.GetType().Name} is not supported");
        }
    }

    /// <summary>
    /// Compiles integer expressions to SQL string representation.
    /// </summary>
    /// <param name="expr">The integer expression to compile</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The SQL string representation and updated context</returns>
    protected virtual (string, Context) CompileExprInt(SqlExprInt expr, Context context)
    {
        switch (expr)
        {
            // Literal values
            case SqlIntValue(var value):
            {
                var (paramName, newContext) = GenerateParameter(context, value);
                return (paramName, newContext);
            }

            // Unary operations
            case SqlIntMinus(var operand):
            {
                var (compiled, ctx) = Compile(operand, context);
                return ($"-{compiled}", ctx);
            }

            case SqlIntAbs(var operand):
            {
                var (compiled, ctx) = Compile(operand, context);
                return ($"ABS({compiled})", ctx);
            }

            // Binary arithmetic operations
            case SqlIntAdd(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"({leftSql} + {rightSql})", rightCtx);
            }

            case SqlIntSub(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"({leftSql} - {rightSql})", rightCtx);
            }

            case SqlIntMult(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"({leftSql} * {rightSql})", rightCtx);
            }

            case SqlIntDiv(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"({leftSql} / {rightSql})", rightCtx);
            }            

            // Parameters
            case SqlParameterInt(var name):
                return (name, context);

            // Aggregate functions
            case SqlIntCount:
                return ("COUNT(*)", context);

            case SqlIntSum(var operand):
            {
                var (compiled, ctx) = Compile(operand, context);
                return ($"SUM({compiled})", ctx);
            }

            case SqlIntAvg(var operand):
            {
                var (compiled, ctx) = Compile(operand, context);
                return ($"AVG({compiled})", ctx);
            }

            // Aggregate functions that are also queries (need special handling)
            case SumSqlIntClause(var query):
            {
                var sumQuery = new SelectClause(query, tuple => ValueTuple.Create(new SqlIntSum((SqlExprInt)tuple[0]!)));
                var (sql, _, ctx) = Compile(sumQuery, context);
                return ($"({sql})", ctx);
            }

            case CountClause(var query):
            {
                var countQuery = new SelectClause(query, _ => ValueTuple.Create(new SqlIntCount()));
                var (sql, _, ctx) = Compile(countQuery, context);
                return ($"({sql})", ctx);
            }

            // CASE expressions
            case SqlIntCase(var condition, var trueValue, var falseValue):
            {
                var (conditionSql, conditionCtx) = Compile(condition, context);
                var (trueSql, trueCtx) = Compile(trueValue, conditionCtx);
                var (falseSql, falseCtx) = Compile(falseValue, trueCtx);
                return ($"CASE WHEN {conditionSql} THEN {trueSql} ELSE {falseSql} END", falseCtx);
            }

            // NULL value
            case SqlIntNull:
                return ("NULL", context);

            default:
                throw new NotSupportedException($"Integer expression type {expr.GetType().Name} is not supported");
        }
    }

    /// <summary>
    /// Compiles string expressions to SQL string representation.
    /// </summary>
    /// <param name="expr">The string expression to compile</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The SQL string representation and updated context</returns>
    protected virtual (string, Context) CompileExprString(SqlExprString expr, Context context)
    {
        switch (expr)
        {
            // Literal values
            case SqlStringValue(var value):
            {
                var (paramName, newContext) = GenerateParameter(context, value);
                return (paramName, newContext);
            }

            // String concatenation - use CONCAT function for better compatibility
            case SqlStringConcat(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"CONCAT({leftSql}, {rightSql})", rightCtx);
            }          

            // Parameters
            case SqlParameterString(var name):
                return (name, context);

            // CASE expressions
            case SqlStringCase(var condition, var trueValue, var falseValue):
            {
                var (conditionSql, conditionCtx) = Compile(condition, context);
                var (trueSql, trueCtx) = Compile(trueValue, conditionCtx);
                var (falseSql, falseCtx) = Compile(falseValue, trueCtx);
                return ($"CASE WHEN {conditionSql} THEN {trueSql} ELSE {falseSql} END", falseCtx);
            }

            // NULL value
            case SqlStringNull:
                return ("NULL", context);

            default:
                throw new NotSupportedException($"String expression type {expr.GetType().Name} is not supported");
        }
    }

    /// <summary>
    /// Compiles any SQL expression to SQL string representation.
    /// This method uses pattern matching to determine the specific expression type.
    /// </summary>
    /// <param name="expr">The SQL expression to compile</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The SQL string representation and updated context</returns>
    public virtual (string, Context) Compile(SqlExpr expr, Context context)
    {
        if (context.ProjectionAliases.TryGetValue(expr, out var alias))
        {
            // If the expression has a projection alias, use it directly
            return ($"{alias.Name}.{alias.Field}", context);
        }
        switch (expr)
        {
            case ISqlScalarQuery scalarQuery:
                {
                    // When scalar queries are used as expressions, wrap them in parentheses
                    var (sql, ctx) = Compile(scalarQuery, context);
                    return ($"({sql})", ctx);
                }
            case ISqlColumn column:
                {
                    
                    // Check if this table is already registered
                    if (context.TableAliases.TryGetValue(column.Table, out var existingAlias))
                    {
                        return ($"a{existingAlias}.{column.ColumnName}", context);
                    }
                    return ($"{column.Table.TableName}.{column.ColumnName}", context);
                }

            case SqlExprBool boolExpr:
                return CompileExprBool(boolExpr, context);

            case SqlExprInt intExpr:
                return CompileExprInt(intExpr, context);

            case SqlExprString stringExpr:
                return CompileExprString(stringExpr, context);

            default:
                throw new NotSupportedException($"Expression type {expr.GetType().Name} is not supported");
        }
    }


    /// <summary>
    /// Compiles a projection based on the selected value, handling both identity selectors (SELECT *) and tuple projections.
    /// </summary>
    /// <param name="selected">The selected value from the selector - either the table itself or a tuple projection</param>
    /// <param name="table">The table being selected from</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The projection SQL string and updated context</returns>
    protected virtual (string, Context) CompileProjection(ITuple selected, ISqlTable table, int aliasIndex, Context context)
    {
        // If selector returns the same table object (identity selector), use SELECT *
        if (ReferenceEquals(selected, table))
        {
            return ("*", context);
        }

        return CompileTupleProjection(selected, aliasIndex, context);
    }

    protected virtual (string, Context) CompileProjection(ITuple selected, ITuple table, int aliasIndex, Context context)
    {
        // If selector returns the same table object (identity selector), use SELECT *
        if (ReferenceEquals(selected, table))
        {
            return ("*", context);
        }

        return CompileTupleProjection(selected, aliasIndex, context);
    }

    private ImmutableArray<SqlExpr> FlattenTuple(ITuple tuple)
    {
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

    private Context UpdateProjectionAliases(ITuple tuple, Context context)
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
            else
                throw new InvalidOperationException($"Expression {expr} does not have a projection alias defined in the context.");         
        }
        return ctx;
    }

    /// <summary>
    /// Compiles a tuple projection into a SELECT list.
    /// </summary>
    /// <param name="tuple">The tuple to compile</param>
    /// <param name="table">The table being selected from (for alias resolution)</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The SQL string representation and updated context</returns>
    protected virtual (string, Context) CompileTupleProjection(ITuple tuple, int aliasIndex, Context context)
    {
        var items = new List<(string Projection, string Alias)>();
        var ctx = context;        
        int projectionCount = 0;

        foreach (var expr in FlattenTuple(tuple))
        {            
            var (compiled, newCtx) = Compile(expr, ctx);
            ctx = newCtx;

            string fieldAlias = expr is ISqlColumn sqlColumn ? sqlColumn.ColumnName : $"prj{projectionCount++}";
            items.Add((compiled, fieldAlias));

            ctx = ctx with { ProjectionAliases = ctx.ProjectionAliases.SetItem(expr, new SqlExprAlias($"a{ctx.AliasIndex}", fieldAlias)) };
         
        }

        return (string.Join(", ", items.Select(i => $"{i.Projection} AS {i.Alias}")), ctx);
    }

    /// <summary>
    /// Compiles ORDER BY clauses with multiple key selectors.
    /// </summary>
    /// <param name="keySelectors">The key selectors and their sort directions</param>
    /// <param name="table">The table being queried</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The compiled ORDER BY clause and updated context</returns>
    protected virtual (string, Context) CompileOrderBy(ImmutableArray<(Func<ITuple, SqlExpr> KeySelector, bool Descending)> keySelectors, ISqlTable table, Context context)
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
    /// Compiles SET clauses for UPDATE statements.
    /// </summary>
    /// <param name="setClauses">The SET clauses to compile</param>
    /// <param name="table">The table being updated</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The SQL string representation and updated context</returns>
    protected virtual (string, Context) CompileSetClauses(ImmutableArray<SetClause> setClauses, ISqlTable table, Context context)
    {
        var items = new List<string>();
        var ctx = context;

        foreach (var setClause in setClauses)
        {
            var column = setClause.ColumnSelector(table);
            var (columnSql, columnCtx) = Compile(column, ctx);
            var (valueSql, valueCtx) = Compile(setClause.Value, columnCtx);
            items.Add($"{columnSql} = {valueSql}");
            ctx = valueCtx;
        }

        return (string.Join(", ", items), ctx);
    }    

    /// <summary>
    /// Compiles INSERT VALUE clauses.
    /// </summary>
    /// <param name="valueClauses">The VALUE clauses to compile</param>
    /// <param name="table">The table being inserted into</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The columns clause, values clause, and updated context</returns>
    protected virtual (string, string, Context) CompileInsertValueClauses(ImmutableArray<ValueClause> valueClauses, ISqlTable table, Context context)
    {
        var columns = new List<string>();
        var valuesSql = new List<string>();
        var ctx = context;

        foreach (var valueClause in valueClauses)
        {
            var column = valueClause.ColumnSelector(table);
            var (valueSql, valueCtx) = Compile(valueClause.Value, ctx);

            if (column is ISqlColumn sqlColumn)
                columns.Add(sqlColumn.ColumnName);
            else
                throw new NotSupportedException($"Column must implement ISqlColumn");
            
            valuesSql.Add(valueSql);
            ctx = valueCtx;
        }

        return (string.Join(", ", columns), string.Join(", ", valuesSql), ctx);
    }

    /// <summary>
    /// Helper method to extract table and set clauses from an UPDATE statement chain.
    /// </summary>
    private (ISqlTable table, ImmutableArray<SetClause> setClauses) ExtractUpdateTableAndClauses(ISqlStatement statement)
    {
        switch (statement)
        {
            case UpdateStatement(var table):
                return (table, ImmutableArray<SetClause>.Empty);
            case SetStatement(var innerStatement, var setClause):
                var (innerTable, innerSetClauses) = ExtractUpdateTableAndClauses(innerStatement);
                return (innerTable, innerSetClauses.Append(setClause).ToImmutableArray());
            default:
                throw new NotSupportedException($"Cannot extract UPDATE table and clauses from statement type {statement.GetType().Name}");
        }
    }

    /// <summary>
    /// Helper method to extract table and value clauses from an INSERT statement chain.
    /// </summary>
    private (ISqlTable table, ImmutableArray<ValueClause> valueClauses) ExtractInsertTableAndClauses(ISqlStatement statement)
    {
        switch (statement)
        {
            case InsertStatement(var table):
                return (table, ImmutableArray<ValueClause>.Empty);
            case ValueStatement(var innerStatement, var valueClause):
                var (innerTable, innerValueClauses) = ExtractInsertTableAndClauses(innerStatement);
                return (innerTable, innerValueClauses.Add(valueClause));
            default:
                throw new NotSupportedException($"Cannot extract INSERT table and clauses from statement type {statement.GetType().Name}");
        }
    }
}
