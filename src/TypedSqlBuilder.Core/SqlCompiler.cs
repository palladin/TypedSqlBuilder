using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections.Immutable;

namespace TypedSqlBuilder.Core;

public record Context
{ 
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
                
            FromClause(var table) =>
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
    public virtual (string, Context) Compile(ISqlQuery query, Context context)
    {
        // First normalize the query to apply all fusion rules
        var normalizedQuery = Normalize(query);
        
        switch (normalizedQuery)
        {
            case SelectClause(FromClause(var table), var selector):
            {
                var selected = selector(table);
                var (projection, projectionCtx) = CompileProjection(selected, table, context);
                return ($"SELECT {projection} FROM {table.TableName}", projectionCtx);
            }

            case SelectClause(WhereClause(FromClause(var table), var predicate), var selector):
            {
                var (whereClause, whereCtx) = Compile(predicate(table), context);
                var selected = selector(table);
                var (projection, projectionCtx) = CompileProjection(selected, table, whereCtx);
                return ($"SELECT {projection} FROM {table.TableName} WHERE {whereClause}", projectionCtx);
            }

            case SelectClause(OrderByClause(FromClause(var table), var keySelectors), var selector):
            {
                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, table, context);
                var selected = selector(table);
                var (projection, projectionCtx) = CompileProjection(selected, table, orderCtx);
                return ($"SELECT {projection} FROM {table.TableName} ORDER BY {orderByClause}", projectionCtx);
            }

            case SelectClause(OrderByClause(WhereClause(FromClause(var table), var predicate), var keySelectors), var selector):
            {
                var (whereClause, whereCtx) = Compile(predicate(table), context);
                var (orderByClause, orderCtx) = CompileOrderBy(keySelectors, table, whereCtx);
                var selected = selector(table);
                var (projection, projectionCtx) = CompileProjection(selected, table, orderCtx);
                return ($"SELECT {projection} FROM {table.TableName} WHERE {whereClause} ORDER BY {orderByClause}", projectionCtx);
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

            case DeleteWhereStatement(var table, var predicate):
            {
                var (whereClause, whereCtx) = Compile(predicate(table), context);
                return ($"DELETE FROM {table.TableName} WHERE {whereClause}", whereCtx);
            }

            // ========== UPDATE STATEMENTS ==========
            case UpdateStatement(var table, var setClauses):
            {
                var (setClause, setCtx) = CompileSetClauses(setClauses, table, context);
                return ($"UPDATE {table.TableName} SET {setClause}", setCtx);
            }

            case UpdateWhereStatement(var table, var setClauses, var predicate):
            {
                var (setClause, setCtx) = CompileSetClauses(setClauses, table, context);
                var (whereClause, whereCtx) = Compile(predicate(table), setCtx);
                return ($"UPDATE {table.TableName} SET {setClause} WHERE {whereClause}", whereCtx);
            }

            // ========== INSERT STATEMENTS ==========
            case InsertStatement(var table, var valueClauses) when valueClauses.Length == 0:
                return ($"INSERT INTO {table.TableName} DEFAULT VALUES", context);

            case InsertStatement(var table, var valueClauses) when valueClauses.Length > 0:
            {
                var (columnsClause, valuesClause, valuesCtx) = CompileInsertValueClauses(valueClauses, table, context);
                return ($"INSERT INTO {table.TableName} ({columnsClause}) VALUES ({valuesClause})", valuesCtx);
            }

            default:
                throw new NotSupportedException($"Statement type {statement.GetType().Name} is not supported");
        }
    }

    /// <summary>
    /// Compiles boolean expressions to SQL string representation.
    /// </summary>
    /// <param name="expr">The boolean expression to compile</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The SQL string representation and updated context</returns>
    public virtual (string, Context) Compile(SqlExprBool expr, Context context)
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

            // Boolean equality/inequality
            case SqlBoolEquals(var leftExpr, SqlBoolNull _):
            {
                var (exprSql, exprCtx) = Compile(leftExpr, context);
                return ($"{exprSql} IS NULL", exprCtx);
            }

            case SqlBoolEquals(SqlBoolNull _, var rightExpr):
            {
                var (exprSql, exprCtx) = Compile(rightExpr, context);
                return ($"{exprSql} IS NULL", exprCtx);
            }

            case SqlBoolEquals(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} = {rightSql}", rightCtx);
            }

            case SqlBoolNotEquals(var leftExpr, SqlBoolNull _):
            {
                var (exprSql, exprCtx) = Compile(leftExpr, context);
                return ($"{exprSql} IS NOT NULL", exprCtx);
            }

            case SqlBoolNotEquals(SqlBoolNull _, var rightExpr):
            {
                var (exprSql, exprCtx) = Compile(rightExpr, context);
                return ($"{exprSql} IS NOT NULL", exprCtx);
            }

            case SqlBoolNotEquals(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} != {rightSql}", rightCtx);
            }

            // Integer comparisons (return bool)
            case SqlIntEquals(var leftExpr, SqlIntNull _):
            {
                var (exprSql, exprCtx) = Compile(leftExpr, context);
                return ($"{exprSql} IS NULL", exprCtx);
            }

            case SqlIntEquals(SqlIntNull _, var rightExpr):
            {
                var (exprSql, exprCtx) = Compile(rightExpr, context);
                return ($"{exprSql} IS NULL", exprCtx);
            }

            case SqlIntEquals(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} = {rightSql}", rightCtx);
            }

            case SqlIntNotEquals(var leftExpr, SqlIntNull _):
            {
                var (exprSql, exprCtx) = Compile(leftExpr, context);
                return ($"{exprSql} IS NOT NULL", exprCtx);
            }

            case SqlIntNotEquals(SqlIntNull _, var rightExpr):
            {
                var (exprSql, exprCtx) = Compile(rightExpr, context);
                return ($"{exprSql} IS NOT NULL", exprCtx);
            }

            case SqlIntNotEquals(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} != {rightSql}", rightCtx);
            }

            case SqlIntGreaterThan(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} > {rightSql}", rightCtx);
            }

            case SqlIntLessThan(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} < {rightSql}", rightCtx);
            }

            case SqlIntGreaterThanOrEqualTo(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} >= {rightSql}", rightCtx);
            }

            case SqlIntLessThanOrEqualTo(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} <= {rightSql}", rightCtx);
            }

            // String comparisons (return bool)
            case SqlStringEquals(var leftExpr, SqlStringNull _):
            {
                var (exprSql, exprCtx) = Compile(leftExpr, context);
                return ($"{exprSql} IS NULL", exprCtx);
            }

            case SqlStringEquals(SqlStringNull _, var rightExpr):
            {
                var (exprSql, exprCtx) = Compile(rightExpr, context);
                return ($"{exprSql} IS NULL", exprCtx);
            }

            case SqlStringEquals(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} = {rightSql}", rightCtx);
            }

            case SqlStringNotEquals(var leftExpr, SqlStringNull _):
            {
                var (exprSql, exprCtx) = Compile(leftExpr, context);
                return ($"{exprSql} IS NOT NULL", exprCtx);
            }

            case SqlStringNotEquals(SqlStringNull _, var rightExpr):
            {
                var (exprSql, exprCtx) = Compile(rightExpr, context);
                return ($"{exprSql} IS NOT NULL", exprCtx);
            }

            case SqlStringNotEquals(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} != {rightSql}", rightCtx);
            }

            case SqlStringGreaterThan(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} > {rightSql}", rightCtx);
            }

            case SqlStringLessThan(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} < {rightSql}", rightCtx);
            }

            case SqlStringGreaterThanOrEqualTo(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} >= {rightSql}", rightCtx);
            }

            case SqlStringLessThanOrEqualTo(var left, var right):
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

            // Column references and projections
            case SqlBoolProjection(var source, var name):
                return ($"{source}.{name}", context);

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
    public virtual (string, Context) Compile(SqlExprInt expr, Context context)
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

            // Column references and projections (more specific patterns first)
            case SqlIntColumn(var source, var name):
                return ($"{source}.{name}", context);

            case SqlIntProjection(var source, var name):
                return ($"{source}.{name}", context);

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
            case SumSqlIntClause sumClause:
                throw new NotImplementedException();

            case CountClause countClause:
                throw new NotImplementedException();

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
    public virtual (string, Context) Compile(SqlExprString expr, Context context)
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

            // Column references and projections (more specific patterns first)
            case SqlStringColumn(var source, var name):
                return ($"{source}.{name}", context);

            case SqlStringProjection(var source, var name):
                return ($"{source}.{name}", context);

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
        switch (expr)
        {
            case SqlExprBool boolExpr:
                return Compile(boolExpr, context);

            case SqlExprInt intExpr:
                return Compile(intExpr, context);

            case SqlExprString stringExpr:
                return Compile(stringExpr, context);

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
    protected virtual (string, Context) CompileProjection(ITuple selected, ISqlTable table, Context context)
    {
        // If selector returns the same table object (identity selector), use SELECT *
        if (ReferenceEquals(selected, table))
        {
            return ("*", context);
        }
        
        return CompileTupleProjection(selected, context);
    }

    /// <summary>
    /// Compiles a tuple projection into a SELECT list.
    /// </summary>
    /// <param name="tuple">The tuple to compile</param>
    /// <param name="context">The compilation context</param>
    /// <returns>The SQL string representation and updated context</returns>
    protected virtual (string, Context) CompileTupleProjection(ITuple tuple, Context context)
    {
        var items = new List<string>();
        var ctx = context;

        for (int i = 0; i < tuple.Length; i++)
        {
            var item = tuple[i];
            switch (item)
            {
                case null:
                    // ignore null values
                    break;
                case SqlExpr expr:
                {
                    var (compiled, newCtx) = Compile(expr, ctx);
                    items.Add(compiled);
                    ctx = newCtx;
                    break;
                }
                case ITuple subTuple:
                {
                    var (compiled, newCtx) = CompileTupleProjection(subTuple, ctx);
                    items.Add(compiled);
                    ctx = newCtx;
                    break;
                }                
                default:
                    throw new NotSupportedException($"Tuple item type {item?.GetType().Name} is not supported in projections");
            }
        }

        return (string.Join(", ", items), ctx);
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
}
