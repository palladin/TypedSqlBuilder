using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace TypedSqlBuilder.Core;

/// <summary>
/// SQL Compiler - Expression compilation methods.
/// This partial class contains all methods related to compiling SQL expressions (SqlExpr and its derivatives).
/// </summary>
public static partial class SqlCompiler
{
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
                var (compiled, ctx) = Compile(operand, context, scopeLevel);
                return ($"NOT ({compiled})", ctx);
            }

            case SqlBoolAnd(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context, scopeLevel);
                var (rightSql, rightCtx) = Compile(right, leftCtx, scopeLevel);
                return ($"({leftSql}) AND ({rightSql})", rightCtx);
            }

            case SqlBoolOr(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context, scopeLevel);
                var (rightSql, rightCtx) = Compile(right, leftCtx, scopeLevel);
                return ($"({leftSql}) OR ({rightSql})", rightCtx);
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
                var (leftSql, leftCtx) = Compile(left, context, scopeLevel);
                var (rightSql, rightCtx) = Compile(right, leftCtx, scopeLevel);
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
                var (leftSql, leftCtx) = Compile(left, context, scopeLevel);
                var (rightSql, rightCtx) = Compile(right, leftCtx, scopeLevel);
                return ($"{leftSql} != {rightSql}", rightCtx);
            }

            case SqlGreaterThan(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context, scopeLevel);
                var (rightSql, rightCtx) = Compile(right, leftCtx, scopeLevel);
                return ($"{leftSql} > {rightSql}", rightCtx);
            }

            case SqlLessThan(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context, scopeLevel);
                var (rightSql, rightCtx) = Compile(right, leftCtx, scopeLevel);
                return ($"{leftSql} < {rightSql}", rightCtx);
            }

            case SqlGreaterThanOrEqualTo(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context, scopeLevel);
                var (rightSql, rightCtx) = Compile(right, leftCtx, scopeLevel);
                return ($"{leftSql} >= {rightSql}", rightCtx);
            }

            case SqlLessThanOrEqualTo(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context, scopeLevel);
                var (rightSql, rightCtx) = Compile(right, leftCtx, scopeLevel);
                return ($"{leftSql} <= {rightSql}", rightCtx);
            }

            // String pattern matching
            case SqlStringLike(var value, var pattern):
            {
                var (valueSql, valueCtx) = Compile(value, context, scopeLevel);
                var (patternParam, patternCtx) = valueCtx.GenerateParameter(pattern);
                return ($"{valueSql} LIKE {patternParam}", patternCtx);
            }

            // Parameters
            case SqlParameterBool(var name):
                return (name, context);

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
                var (compiled, ctx) = Compile(operand, context, scopeLevel);
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
                var (leftSql, leftCtx) = Compile(left, context, scopeLevel);
                var (rightSql, rightCtx) = Compile(right, leftCtx, scopeLevel);
                return ($"({leftSql} + {rightSql})", rightCtx);
            }

            case SqlIntSub(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context, scopeLevel);
                var (rightSql, rightCtx) = Compile(right, leftCtx, scopeLevel);
                return ($"({leftSql} - {rightSql})", rightCtx);
            }

            case SqlIntMult(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context, scopeLevel);
                var (rightSql, rightCtx) = Compile(right, leftCtx, scopeLevel);
                return ($"({leftSql} * {rightSql})", rightCtx);
            }

            case SqlIntDiv(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context, scopeLevel);
                var (rightSql, rightCtx) = Compile(right, leftCtx, scopeLevel);
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
                var (compiled, ctx) = Compile(operand, context, scopeLevel);
                return ($"SUM({compiled})", ctx);
            }

            case SqlIntAvg(var operand):
            {
                var (compiled, ctx) = Compile(operand, context, scopeLevel);
                return ($"AVG({compiled})", ctx);
            }

            // Aggregate functions that are also queries (need special handling)
            case SumSqlIntClause(var query):
            {
                var sumQuery = new SelectClause(query, tuple => ValueTuple.Create(new SqlIntSum((SqlExprInt)tuple[0]!)), ImmutableArray<string?>.Empty);
                var (sql, _, ctx) = Compile(sumQuery, context, scopeLevel);
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
                var (leftSql, leftCtx) = Compile(left, context, scopeLevel);
                var (rightSql, rightCtx) = Compile(right, leftCtx, scopeLevel);
                
                return context.Dialect.UsesConcatFunction 
                    ? ($"{context.Dialect.StringConcatOperator}({leftSql}, {rightSql})", rightCtx)
                    : ($"({leftSql} {context.Dialect.StringConcatOperator} {rightSql})", rightCtx);
            }

            // Parameters
            case SqlParameterString(var name):
                return (name, context);

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
                return context.GenerateParameter(value ? 1 : 0);
            
            // String concatenation - handle dialect differences
            case SqlStringConcat(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context, scopeLevel);
                var (rightSql, rightCtx) = Compile(right, leftCtx, scopeLevel);
                
                return context.Dialect.UsesConcatFunction 
                    ? ($"{context.Dialect.StringConcatOperator}({leftSql}, {rightSql})", rightCtx)
                    : ($"({leftSql} {context.Dialect.StringConcatOperator} {rightSql})", rightCtx);
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

            default:
                throw new NotSupportedException($"Expression type {expr.GetType().Name} is not supported");
        }
    }
}
