using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace TypedSqlBuilder.Core;

/// <summary>
/// Precedence constants for SQL expressions.
/// Higher values bind tighter (following standard SQL precedence).
/// Based on: 1=Parentheses, 2=Unary, 3=Multiplicative, 4=Additive, 5=StringConcat, 6=Comparison, 7=NOT, 8=AND, 9=OR
/// </summary>
internal static class SqlPrecedence
{
    public const int Primary = 10;         // Literals, columns, function calls (parentheses level)
    public const int UnaryMath = 9;        // +, - (unary), ~ (bitwise NOT), ABS()
    public const int Multiplicative = 8;   // *, /, %
    public const int Additive = 7;         // +, - (binary arithmetic)
    public const int StringConcat = 6;     // || (string concatenation)
    public const int Comparison = 5;       // =, !=, <, >, <=, >=, LIKE, IN, IS NULL, BETWEEN
    public const int LogicalNot = 4;       // NOT (logical)
    public const int LogicalAnd = 3;       // AND
    public const int LogicalOr = 2;        // OR (lowest precedence)
    public const int Lowest = 0;           // Used as initial context
}



/// <summary>
/// SQL Compiler - Expression compilation methods.
/// This partial class contains all methods related to compiling SQL expressions (SqlExpr and its derivatives).
/// </summary>
internal static partial class SqlCompiler
{
    /// <summary>
    /// Gets the precedence of a SQL expression by pattern matching.
    /// Follows standard SQL precedence rules.
    /// </summary>
    private static int GetPrecedence(SqlExpr expr) => expr switch
    {
        // Boolean expressions
        SqlBoolNot _ => SqlPrecedence.LogicalNot,
        SqlBoolAnd _ => SqlPrecedence.LogicalAnd,
        SqlBoolOr _ => SqlPrecedence.LogicalOr,
        SqlBoolValue _ or SqlBoolColumn _ => SqlPrecedence.Primary,
        
        // Integer expressions
        SqlIntMinus _ or SqlIntAbs _ => SqlPrecedence.UnaryMath,
        SqlIntMult _ or SqlIntDiv _ => SqlPrecedence.Multiplicative,
        SqlIntAdd _ or SqlIntSub _ => SqlPrecedence.Additive,
        SqlIntValue _ or SqlIntColumn _ => SqlPrecedence.Primary,
        
        // Comparison expressions
        SqlEquals _ or SqlNotEquals _ or SqlGreaterThan _ or SqlLessThan _ 
        or SqlGreaterThanOrEqualTo _ or SqlLessThanOrEqualTo _ or SqlStringLike _ => SqlPrecedence.Comparison,
        
        // String expressions
        SqlStringConcat _ => SqlPrecedence.StringConcat,
        SqlStringValue _ or SqlStringColumn _ => SqlPrecedence.Primary,
        
        // Decimal expressions
        SqlDecimalMinus _ => SqlPrecedence.UnaryMath,
        SqlDecimalMult _ or SqlDecimalDiv _ => SqlPrecedence.Multiplicative,
        SqlDecimalAdd _ or SqlDecimalSub _ => SqlPrecedence.Additive,
        SqlDecimalValue _ or SqlDecimalColumn _ => SqlPrecedence.Primary,
        
        // Default for other expressions (functions, aggregates, etc.)
        _ => SqlPrecedence.Primary
    };

    /// <summary>
    /// Compiles a sub-expression with appropriate parentheses based on precedence.
    /// </summary>
    private static (string, Context) CompileWithPrecedence(
        SqlExpr expr, 
        Context context, 
        int scopeLevel, 
        int minPrecedence)
    {
        var (sql, newContext) = Compile(expr, context, scopeLevel);
        
        // Wrap in parentheses if this expression's precedence is too low
        if (GetPrecedence(expr) < minPrecedence)
        {
            sql = $"({sql})";
        }
        
        return (sql, newContext);
    }

    /// <summary>
    /// Compiles boolean expressions to SQL string representation.
    /// </summary>
    /// <param name="expr">The boolean expression to compile</param>
    /// <param name="context">The compilation context</param>
    /// <param name="scopeLevel">The nesting scope level for the SQL statement</param>
    /// <returns>The SQL string representation and updated context</returns>
    private static (string, Context) CompileExprBool(SqlExprBool expr, Context context, int scopeLevel)
    {
        switch (expr)
        {
            // Literal values - already handled in main Compile method for dialect differences
            case SqlBoolValue(var value):
            {
                var (paramName, newContext) = context.GenerateParameter(value ? 1 : 0);
                return (paramName, newContext);
            }

            // Logical operations
            case SqlBoolNot(var operand):
            {
                var (compiled, ctx) = CompileWithPrecedence(
                    operand, context, scopeLevel, SqlPrecedence.LogicalNot);
                return ($"NOT {compiled}", ctx);
            }

            case SqlBoolAnd(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, SqlPrecedence.LogicalAnd);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.LogicalAnd);
                return ($"{leftSql} AND {rightSql}", rightCtx);
            }

            case SqlBoolOr(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, SqlPrecedence.LogicalOr);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.LogicalOr);
                return ($"{leftSql} OR {rightSql}", rightCtx);
            }

            // Generic equality/inequality comparisons - handles ALL types (Bool, Int, String)
            case SqlEquals(var leftExpr, ISqlNullValue _):
            {
                var (exprSql, exprCtx) = Compile(leftExpr, context, scopeLevel);
                return ($"{exprSql} IS NULL", exprCtx);
            }

            case SqlEquals(ISqlNullValue _, var rightExpr):
            {
                var (exprSql, exprCtx) = Compile(rightExpr, context, scopeLevel);
                return ($"{exprSql} IS NULL", exprCtx);
            }

            case SqlEquals(var left, var right):
            {
                // PostgreSQL requires parentheses for chained comparisons like (a > b) = c
                // SQL Server and SQLite handle this correctly without parentheses
                var leftPrecedence = context.Dialect.Type == DatabaseType.PostgreSQL && 
                                   GetPrecedence(left) == SqlPrecedence.Comparison 
                    ? SqlPrecedence.Comparison + 1  // Force parentheses for PostgreSQL comparison operations
                    : SqlPrecedence.Comparison;
                
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, leftPrecedence);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.Comparison);
                return ($"{leftSql} = {rightSql}", rightCtx);
            }

            case SqlNotEquals(var leftExpr, ISqlNullValue _):
            {
                var (exprSql, exprCtx) = Compile(leftExpr, context, scopeLevel);
                return ($"{exprSql} IS NOT NULL", exprCtx);
            }

            case SqlNotEquals(ISqlNullValue _, var rightExpr):
            {
                var (exprSql, exprCtx) = Compile(rightExpr, context, scopeLevel);
                return ($"{exprSql} IS NOT NULL", exprCtx);
            }

            case SqlNotEquals(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, SqlPrecedence.Comparison);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.Comparison);
                return ($"{leftSql} != {rightSql}", rightCtx);
            }

            case SqlGreaterThan(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, SqlPrecedence.Comparison);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.Comparison);
                return ($"{leftSql} > {rightSql}", rightCtx);
            }

            case SqlLessThan(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, SqlPrecedence.Comparison);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.Comparison);
                return ($"{leftSql} < {rightSql}", rightCtx);
            }

            case SqlGreaterThanOrEqualTo(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, SqlPrecedence.Comparison);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.Comparison); 
                return ($"{leftSql} >= {rightSql}", rightCtx);
            }

            case SqlLessThanOrEqualTo(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, SqlPrecedence.Comparison);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.Comparison); 
                return ($"{leftSql} <= {rightSql}", rightCtx);
            }

            // String pattern matching
            case SqlStringLike(var value, var pattern):
            {
                var (valueSql, valueCtx) = CompileWithPrecedence(value, context, scopeLevel, SqlPrecedence.Comparison);
                var (patternParam, patternCtx) = valueCtx.GenerateParameter(pattern);
                return ($"{valueSql} LIKE {patternParam}", patternCtx);
            }

            // Parameters
            case SqlParameterBool(var name):
            {
                var paramKey = $"{context.Dialect.ParameterPrefix}{name}";
                var updatedContext = context.Parameters.ContainsKey(paramKey) 
                    ? context 
                    : context with { Parameters = context.Parameters.Add(paramKey, null!) };
                return (paramKey, updatedContext);
            }

            // CASE expressions
            case SqlBoolCase(var condition, var trueValue, var falseValue):
            {
                var (conditionSql, conditionCtx) = Compile(condition, context, scopeLevel);
                var (trueSql, trueCtx) = Compile(trueValue, conditionCtx, scopeLevel);
                var (falseSql, falseCtx) = Compile(falseValue, trueCtx, scopeLevel);
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
                    
                var (exprSql, ctx) = Compile(expression, context, scopeLevel);
                var valuesSql = new List<string>();
                var currentCtx = ctx;
                
                foreach (var value in values)
                {
                    var (valueSql, nextCtx) = Compile(value, currentCtx, scopeLevel);
                    valuesSql.Add(valueSql);
                    currentCtx = nextCtx;
                }
                
                return ($"{exprSql} IN ({string.Join(", ", valuesSql)})", currentCtx);
            }

            // IN clause with subquery
            case SqlInSubQuery(var expression, var subQuery):
            {
                var (exprSql, ctx) = Compile(expression, context, scopeLevel);
                var (subQuerySql, _, finalCtx) = Compile((ISqlQuery)subQuery, ctx, scopeLevel);
                
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
    /// <param name="scopeLevel">The nesting scope level for the SQL statement</param>
    /// <returns>The SQL string representation and updated context</returns>
    private static (string, Context) CompileExprInt(SqlExprInt expr, Context context, int scopeLevel)
    {
        switch (expr)
        {
            // Literal values
            case SqlIntValue(var value):
            {
                var (paramName, newContext) = context.GenerateParameter(value);
                return (paramName, newContext);
            }

            // Unary operations
            case SqlIntMinus(var operand):
            {
                var (compiled, ctx) = CompileWithPrecedence(
                    operand, context, scopeLevel, SqlPrecedence.UnaryMath);
                return ($"-{compiled}", ctx);
            }

            case SqlIntAbs(var operand):
            {
                var (compiled, ctx) = Compile(operand, context, scopeLevel);
                return ($"ABS({compiled})", ctx);
            }

            // Binary arithmetic operations
            case SqlIntAdd(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, SqlPrecedence.Additive);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.Additive);
                return ($"{leftSql} + {rightSql}", rightCtx);
            }

            case SqlIntSub(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, SqlPrecedence.Additive);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.Additive);
                return ($"{leftSql} - {rightSql}", rightCtx);
            }

            case SqlIntMult(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, SqlPrecedence.Multiplicative);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.Multiplicative);
                return ($"{leftSql} * {rightSql}", rightCtx);
            }

            case SqlIntDiv(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, SqlPrecedence.Multiplicative);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.Multiplicative);
                return ($"{leftSql} / {rightSql}", rightCtx);
            }            

            // Parameters
            case SqlParameterInt(var name):
            {
                var paramKey = $"{context.Dialect.ParameterPrefix}{name}";
                var updatedContext = context.Parameters.ContainsKey(paramKey) 
                    ? context 
                    : context with { Parameters = context.Parameters.Add(paramKey, null!) };
                return (paramKey, updatedContext);
            }

            // Aggregate functions
            case SqlIntCount:
                return ("COUNT(*)", context);

            case SqlIntSum(var operand):
            {
                var (compiled, ctx) = Compile(operand, context, scopeLevel);
                return ($"SUM({compiled})", ctx);
            }

            case SqlIntAvg(var operand):
            {
                var (compiled, ctx) = Compile(operand, context, scopeLevel);
                return ($"AVG({compiled})", ctx);
            }

            case SqlIntMin(var operand):
            {
                var (compiled, ctx) = Compile(operand, context, scopeLevel);
                return ($"MIN({compiled})", ctx);
            }

            case SqlIntMax(var operand):
            {
                var (compiled, ctx) = Compile(operand, context, scopeLevel);
                return ($"MAX({compiled})", ctx);
            }

            // Aggregate functions that are also queries (need special handling)
            case SumSqlIntClause(var query):
            {
                var sumQuery = new SelectClause(query, tuple => ValueTuple.Create(new SqlIntSum((SqlExprInt)tuple[0]!)), ImmutableArray<string?>.Empty);
                var (sql, _, ctx) = Compile(sumQuery, context, scopeLevel);
                return ($"({sql})", ctx);
            }

            case AvgSqlIntClause(var query):
            {
                var avgQuery = new SelectClause(query, tuple => ValueTuple.Create(new SqlIntAvg((SqlExprInt)tuple[0]!)), ImmutableArray<string?>.Empty);
                var (sql, _, ctx) = Compile(avgQuery, context, scopeLevel);
                return ($"({sql})", ctx);
            }

            case MinSqlIntClause(var query):
            {
                var minQuery = new SelectClause(query, tuple => ValueTuple.Create(new SqlIntMin((SqlExprInt)tuple[0]!)), ImmutableArray<string?>.Empty);
                var (sql, _, ctx) = Compile(minQuery, context, scopeLevel);
                return ($"({sql})", ctx);
            }

            case MaxSqlIntClause(var query):
            {
                var maxQuery = new SelectClause(query, tuple => ValueTuple.Create(new SqlIntMax((SqlExprInt)tuple[0]!)), ImmutableArray<string?>.Empty);
                var (sql, _, ctx) = Compile(maxQuery, context, scopeLevel);
                return ($"({sql})", ctx);
            }

            case CountClause(var query):
            {
                var countQuery = new SelectClause(query, _ => ValueTuple.Create(new SqlIntCount()), ImmutableArray<string?>.Empty);
                var (sql, _, ctx) = Compile(countQuery, context, scopeLevel);
                return ($"({sql})", ctx);
            }

            // CASE expressions
            case SqlIntCase(var condition, var trueValue, var falseValue):
            {
                var (conditionSql, conditionCtx) = Compile(condition, context, scopeLevel);
                var (trueSql, trueCtx) = Compile(trueValue, conditionCtx, scopeLevel);
                var (falseSql, falseCtx) = Compile(falseValue, trueCtx, scopeLevel);
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
    /// <param name="scopeLevel">The nesting scope level for the SQL statement</param>
    /// <returns>The SQL string representation and updated context</returns>
    private static (string, Context) CompileExprString(SqlExprString expr, Context context, int scopeLevel)
    {
        switch (expr)
        {
            // Literal values
            case SqlStringValue(var value):
            {
                var (paramName, newContext) = context.GenerateParameter(value);
                return (paramName, newContext);
            }

            // String concatenation - now handled in main Compile method for dialect differences
            case SqlStringConcat(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, SqlPrecedence.Additive);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.Additive);
                
                return context.Dialect.UsesConcatFunction 
                    ? ($"{context.Dialect.StringConcatOperator}({leftSql}, {rightSql})", rightCtx)
                    : ($"{leftSql} {context.Dialect.StringConcatOperator} {rightSql}", rightCtx);
            }

            // Parameters
            case SqlParameterString(var name):
            {
                var paramKey = $"{context.Dialect.ParameterPrefix}{name}";
                var updatedContext = context.Parameters.ContainsKey(paramKey) 
                    ? context 
                    : context with { Parameters = context.Parameters.Add(paramKey, null!) };
                return (paramKey, updatedContext);
            }

            // CASE expressions
            case SqlStringCase(var condition, var trueValue, var falseValue):
            {
                var (conditionSql, conditionCtx) = Compile(condition, context, scopeLevel);
                var (trueSql, trueCtx) = Compile(trueValue, conditionCtx, scopeLevel);
                var (falseSql, falseCtx) = Compile(falseValue, trueCtx, scopeLevel);
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
    /// Compiles decimal expressions to SQL string representation.
    /// </summary>
    /// <param name="expr">The decimal expression to compile</param>
    /// <param name="context">The compilation context</param>
    /// <param name="scopeLevel">The nesting scope level for the SQL statement</param>
    /// <returns>The SQL string representation and updated context</returns>
    private static (string, Context) CompileExprDecimal(SqlExprDecimal expr, Context context, int scopeLevel)
    {
        switch (expr)
        {
            // Literal values
            case SqlDecimalValue(var value):
            {
                var (paramName, newContext) = context.GenerateParameter(value);
                return (paramName, newContext);
            }

            // Arithmetic operations
            case SqlDecimalAdd(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, SqlPrecedence.Additive);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.Additive);
                return ($"{leftSql} + {rightSql}", rightCtx);
            }

            case SqlDecimalSub(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, SqlPrecedence.Additive);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.Additive); 
                return ($"{leftSql} - {rightSql}", rightCtx);
            }

            case SqlDecimalMinus(var operand):
            {
                var (compiled, ctx) = CompileWithPrecedence(
                    operand, context, scopeLevel, SqlPrecedence.UnaryMath);
                return ($"-{compiled}", ctx);
            }

            case SqlDecimalMult(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, SqlPrecedence.Multiplicative);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.Multiplicative);
                return ($"{leftSql} * {rightSql}", rightCtx);
            }

            case SqlDecimalDiv(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(
                    left, context, scopeLevel, SqlPrecedence.Multiplicative);
                var (rightSql, rightCtx) = CompileWithPrecedence(
                    right, leftCtx, scopeLevel, SqlPrecedence.Multiplicative); 
                return ($"{leftSql} / {rightSql}", rightCtx);
            }

            // Parameters
            case SqlParameterDecimal(var name):
            {
                var paramKey = $"{context.Dialect.ParameterPrefix}{name}";
                var updatedContext = context.Parameters.ContainsKey(paramKey) 
                    ? context 
                    : context with { Parameters = context.Parameters.Add(paramKey, null!) };
                return (paramKey, updatedContext);
            }

            // CASE expressions
            case SqlDecimalCase(var condition, var trueValue, var falseValue):
            {
                var (conditionSql, conditionCtx) = Compile(condition, context, scopeLevel);
                var (trueSql, trueCtx) = Compile(trueValue, conditionCtx, scopeLevel);
                var (falseSql, falseCtx) = Compile(falseValue, trueCtx, scopeLevel);
                return ($"CASE WHEN {conditionSql} THEN {trueSql} ELSE {falseSql} END", falseCtx);
            }

            // Aggregate functions
            case SqlDecimalSum(var operand):
            {
                var (compiled, ctx) = Compile(operand, context, scopeLevel);
                return ($"SUM({compiled})", ctx);
            }

            case SqlDecimalAvg(var operand):
            {
                var (compiled, ctx) = Compile(operand, context, scopeLevel);
                return ($"AVG({compiled})", ctx);
            }

            case SqlDecimalMin(var operand):
            {
                var (compiled, ctx) = Compile(operand, context, scopeLevel);
                return ($"MIN({compiled})", ctx);
            }

            case SqlDecimalMax(var operand):
            {
                var (compiled, ctx) = Compile(operand, context, scopeLevel);
                return ($"MAX({compiled})", ctx);
            }

            // NULL value
            case SqlDecimalNull:
                return ("NULL", context);

            default:
                throw new NotSupportedException($"Decimal expression type {expr.GetType().Name} is not supported");
        }
    }

    /// <summary>
    /// Compiles DateTime expressions to SQL string representation.
    /// </summary>
    /// <param name="expr">The DateTime expression to compile</param>
    /// <param name="context">The compilation context</param>
    /// <param name="scopeLevel">The nesting scope level for the SQL statement</param>
    /// <returns>The SQL string representation and updated context</returns>
    private static (string, Context) CompileExprDateTime(SqlExprDateTime expr, Context context, int scopeLevel)
    {
        switch (expr)
        {
            // Literal values
            case SqlDateTimeValue(var value):
            {
                var (paramName, newContext) = context.GenerateParameter(value);
                return (paramName, newContext);
            }

            // Parameters
            case SqlParameterDateTime(var name):
            {
                var paramKey = $"{context.Dialect.ParameterPrefix}{name}";
                var updatedContext = context.Parameters.ContainsKey(paramKey) 
                    ? context 
                    : context with { Parameters = context.Parameters.Add(paramKey, null!) };
                return (paramKey, updatedContext);
            }

            // CASE expressions
            case SqlDateTimeCase(var condition, var trueValue, var falseValue):
            {
                var (conditionSql, conditionCtx) = Compile(condition, context, scopeLevel);
                var (trueSql, trueCtx) = Compile(trueValue, conditionCtx, scopeLevel);
                var (falseSql, falseCtx) = Compile(falseValue, trueCtx, scopeLevel);
                return ($"CASE WHEN {conditionSql} THEN {trueSql} ELSE {falseSql} END", falseCtx);
            }

            // NULL value
            case SqlDateTimeNull:
                return ("NULL", context);

            default:
                throw new NotSupportedException($"DateTime expression type {expr.GetType().Name} is not supported");
        }
    }

    /// <summary>
    /// Compiles GUID expressions to SQL string representation.
    /// </summary>
    /// <param name="expr">The GUID expression to compile</param>
    /// <param name="context">The compilation context</param>
    /// <param name="scopeLevel">The nesting scope level for the SQL statement</param>
    /// <returns>The SQL string representation and updated context</returns>
    private static (string, Context) CompileExprGuid(SqlExprGuid expr, Context context, int scopeLevel)
    {
        switch (expr)
        {
            // Literal values
            case SqlGuidValue(var value):
            {
                var (paramName, newContext) = context.GenerateParameter(value);
                return (paramName, newContext);
            }

            // Parameters
            case SqlParameterGuid(var name):
            {
                var paramKey = $"{context.Dialect.ParameterPrefix}{name}";
                var updatedContext = context.Parameters.ContainsKey(paramKey) 
                    ? context 
                    : context with { Parameters = context.Parameters.Add(paramKey, null!) };
                return (paramKey, updatedContext);
            }

            // CASE expressions
            case SqlGuidCase(var condition, var trueValue, var falseValue):
            {
                var (conditionSql, conditionCtx) = Compile(condition, context, scopeLevel);
                var (trueSql, trueCtx) = Compile(trueValue, conditionCtx, scopeLevel);
                var (falseSql, falseCtx) = Compile(falseValue, trueCtx, scopeLevel);
                return ($"CASE WHEN {conditionSql} THEN {trueSql} ELSE {falseSql} END", falseCtx);
            }

            // NULL value
            case SqlGuidNull:
                return ("NULL", context);

            default:
                throw new NotSupportedException($"GUID expression type {expr.GetType().Name} is not supported");
        }
    }

    /// <summary>
    /// Compiles any SQL expression to SQL string representation.
    /// This method uses pattern matching to determine the specific expression type.
    /// Handles dialect-specific differences based on the context's dialect configuration.
    /// </summary>
    /// <param name="expr">The SQL expression to compile</param>
    /// <param name="context">The compilation context</param>
    /// <param name="scopeLevel">The nesting scope level for the SQL statement</param>
    /// <returns>The SQL string representation and updated context</returns>
    private static (string, Context) Compile(SqlExpr expr, Context context, int scopeLevel)
    {
        if (context.ProjectionAliases.TryGetValue(expr, out var alias))
        {
            // If the expression has a projection alias, use it directly
            return ($"{alias.Name}.{alias.Field}", context);
        }

        // Handle dialect-specific expressions first
        switch (expr)
        {
            // Boolean literals - handle dialect differences for true/false values
            case SqlBoolValue(var value):
                return context.Dialect.Type switch
                {
                    DatabaseType.PostgreSQL => (value ? "true" : "false", context),
                    _ => context.GenerateParameter(value ? 1 : 0)
                };
            
            // String concatenation - handle dialect differences
            case SqlStringConcat(var left, var right):
            {
                var (leftSql, leftCtx) = CompileWithPrecedence(left, context, scopeLevel, SqlPrecedence.Additive);
                var (rightSql, rightCtx) = CompileWithPrecedence(right, leftCtx, scopeLevel, SqlPrecedence.Additive);
                
                return context.Dialect.UsesConcatFunction 
                    ? ($"{context.Dialect.StringConcatOperator}({leftSql}, {rightSql})", rightCtx)
                    : ($"{leftSql} {context.Dialect.StringConcatOperator} {rightSql}", rightCtx);
            }
        }

        // Handle all other expressions (common between dialects)
        switch (expr)
        {
            case ISqlScalarQuery scalarQuery:
            {
                // When scalar queries are used as expressions, wrap them in parentheses
                var (sql, ctx) = Compile(scalarQuery, context, scopeLevel);
                return ($"({sql})", ctx);
            }
            case ISqlColumn column:
                return ($"{column.TableName}.{column.ColumnName}", context);

            case SqlExprBool boolExpr:
                return CompileExprBool(boolExpr, context, scopeLevel);

            case SqlExprInt intExpr:
                return CompileExprInt(intExpr, context, scopeLevel);

            case SqlExprString stringExpr:
                return CompileExprString(stringExpr, context, scopeLevel);

            case SqlExprDecimal decimalExpr:
                return CompileExprDecimal(decimalExpr, context, scopeLevel);

            case SqlExprDateTime dateTimeExpr:
                return CompileExprDateTime(dateTimeExpr, context, scopeLevel);

            case SqlExprGuid guidExpr:
                return CompileExprGuid(guidExpr, context, scopeLevel);

            default:
                throw new NotSupportedException($"Expression type {expr.GetType().Name} is not supported");
        }
    }
}
