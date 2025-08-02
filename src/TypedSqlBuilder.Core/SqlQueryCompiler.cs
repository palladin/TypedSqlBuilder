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
public abstract class SqlQueryCompilerBase
{
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
    /// Compiles SQL queries and clauses to SQL string representation.
    /// </summary>
    /// <param name="query">The SQL query or clause to compile</param>
    /// <returns>The SQL string representation</returns>
    public virtual (string, Context) Compile(ISqlQuery query, Context context)
    {
        switch (query)
        {
            case SelectClause(FromClause(var table), var selector):
            {
                var (projection, projectionCtx) = CompileTupleProjection(selector(table), context);
                return ($"SELECT {projection} FROM {table.TableName}", projectionCtx);
            }

            case SelectClause(WhereClause(FromClause(var table), var predicate), var selector):
            {
                var (whereClause, whereCtx) = Compile(predicate(table), context);
                var (projection, projectionCtx) = CompileTupleProjection(selector(table), whereCtx);
                return ($"SELECT {projection} FROM {table.TableName} WHERE {whereClause}", projectionCtx);
            }

            case SelectClause(OrderByClause(FromClause(var table), var orderBy, var desc), var selector):
            {
                var (orderByClause, orderCtx) = Compile(orderBy(table), context);
                var (projection, projectionCtx) = CompileTupleProjection(selector(table), orderCtx);
                return ($"SELECT {projection} FROM {table.TableName} ORDER BY {orderByClause} {(desc ? "DESC" : "ASC")}", projectionCtx);
            }

            case SelectClause(OrderByClause(WhereClause(FromClause(var table), var predicate), var orderBy, var desc), var selector):
            {
                var (whereClause, whereCtx) = Compile(predicate(table), context);
                var (orderByClause, orderCtx) = Compile(orderBy(table), whereCtx);
                var (projection, projectionCtx) = CompileTupleProjection(selector(table), orderCtx);
                return ($"SELECT {projection} FROM {table.TableName} WHERE {whereClause} ORDER BY {orderByClause} {(desc ? "DESC" : "ASC")}", projectionCtx);
            }

            case WhereClause(FromClause(var table), var predicate):
            {
                var (whereClause, whereCtx) = Compile(predicate(table), context);
                return ($"SELECT * FROM {table.TableName} WHERE {whereClause}", whereCtx);
            }

            case OrderByClause(FromClause(var table), var orderBy, var desc):
            {
                var (orderByClause, orderCtx) = Compile(orderBy(table), context);
                return ($"SELECT * FROM {table.TableName} ORDER BY {orderByClause} {(desc ? "DESC" : "ASC")}", orderCtx);
            }

            case OrderByClause(WhereClause(FromClause(var table), var predicate), var orderBy, var desc):
            {
                var (whereClause, whereCtx) = Compile(predicate(table), context);
                var (orderByClause, orderCtx) = Compile(orderBy(table), whereCtx);
                return ($"SELECT * FROM {table.TableName} WHERE {whereClause} ORDER BY {orderByClause} {(desc ? "DESC" : "ASC")}", orderCtx);
            }

            case FromClause(var table):
                return ($"SELECT * FROM {table.TableName}", context);

            default:
                throw new NotSupportedException($"Query type {query.GetType().Name} is not supported");
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
            case SqlBoolEquals(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} = {rightSql}", rightCtx);
            }

            case SqlBoolNotEquals(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} != {rightSql}", rightCtx);
            }

            // Integer comparisons (return bool)
            case SqlIntEquals(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} = {rightSql}", rightCtx);
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
            case SqlStringEquals(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"{leftSql} = {rightSql}", rightCtx);
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
}
