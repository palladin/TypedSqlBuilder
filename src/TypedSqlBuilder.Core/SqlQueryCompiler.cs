using System.Text;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace TypedSqlBuilder.Core;

/// <summary>
/// Enhanced SQL Compiler that can properly handle typed queries and lambda expressions.
/// This version can evaluate selectors and predicates to generate correct SQL.
/// </summary>
public static class SqlQueryCompiler
{
    /// <summary>
    /// Compiles SQL queries and clauses to SQL string representation.
    /// </summary>
    /// <param name="query">The SQL query or clause to compile</param>
    /// <returns>The SQL string representation</returns>
    public static string Compile(ISqlQuery query)
    {
        return query switch
        {
            FromClause(var table) => 
                $"SELECT * FROM {table.TableName}",
            SelectClause(FromClause(var table), var selector) =>
                $"SELECT {CompileTupleProjection(selector(table))} FROM {table.TableName}",
            SelectSqlScalarClause(FromClause(var table), var selector) =>
                $"SELECT {Compile(selector(table))} FROM {table.TableName}",
            WhereClause(FromClause(var table), var predicate) =>
                $"SELECT * FROM {table.TableName} WHERE {Compile(predicate(table))}",
            OrderByClause(FromClause(var table), var orderBy, var desc) =>
                $"SELECT * FROM {table.TableName} ORDER BY {Compile(orderBy(table))} {(desc ? "DESC" : "ASC")}",
            SelectClause(WhereClause(FromClause(var table), var predicate), var selector) =>
                $"SELECT {CompileTupleProjection(selector(table))} FROM {table.TableName} WHERE {Compile(predicate(table))}",
            SelectSqlScalarClause(WhereClause(FromClause(var table), var predicate), var selector) =>
                $"SELECT {Compile(selector(table))} FROM {table.TableName} WHERE {Compile(predicate(table))}",
            OrderByClause(WhereClause(FromClause(var table), var predicate), var orderBy, var desc) =>
                $"SELECT * FROM {table.TableName} WHERE {Compile(predicate(table))} ORDER BY {Compile(orderBy(table))} {(desc ? "DESC" : "ASC")}",
            SelectClause(OrderByClause(FromClause(var table), var orderBy, var desc), var selector) =>
                $"SELECT {CompileTupleProjection(selector(table))} FROM {table.TableName} ORDER BY {Compile(orderBy(table))} {(desc ? "DESC" : "ASC")}",
            SelectSqlScalarClause(OrderByClause(FromClause(var table), var orderBy, var desc), var selector) =>
                $"SELECT {Compile(selector(table))} FROM {table.TableName} ORDER BY {Compile(orderBy(table))} {(desc ? "DESC" : "ASC")}",
            SelectClause(OrderByClause(WhereClause(FromClause(var table), var predicate), var orderBy, var desc), var selector) =>
                $"SELECT {CompileTupleProjection(selector(table))} FROM {table.TableName} WHERE {Compile(predicate(table))} ORDER BY {Compile(orderBy(table))} {(desc ? "DESC" : "ASC")}",
            SelectSqlScalarClause(OrderByClause(WhereClause(FromClause(var table), var predicate), var orderBy, var desc), var selector) =>
                $"SELECT {Compile(selector(table))} FROM {table.TableName} WHERE {Compile(predicate(table))} ORDER BY {Compile(orderBy(table))} {(desc ? "DESC" : "ASC")}",

            _ => throw new NotSupportedException($"Query type {query.GetType().Name} is not supported")
        };
    }

    /// <summary>
    /// Compiles boolean expressions to SQL string representation.
    /// </summary>
    /// <param name="expr">The boolean expression to compile</param>
    /// <returns>The SQL string representation</returns>
    public static string Compile(SqlExprBool expr)
    {
        return expr switch
        {
            // Literal values
            SqlBoolValue(var value) => value ? "TRUE" : "FALSE",
            
            // Logical operations
            SqlBoolNot(var operand) => $"NOT ({Compile(operand)})",
            SqlBoolAnd(var left, var right) => $"({Compile(left)}) AND ({Compile(right)})",
            SqlBoolOr(var left, var right) => $"({Compile(left)}) OR ({Compile(right)})",
            
            // Boolean equality/inequality
            SqlBoolEquals(var left, var right) => $"{Compile(left)} = {Compile(right)}",
            SqlBoolNotEquals(var left, var right) => $"{Compile(left)} != {Compile(right)}",
            
            // Integer comparisons (return bool)
            SqlIntEquals(var left, var right) => $"{Compile(left)} = {Compile(right)}",
            SqlIntNotEquals(var left, var right) => $"{Compile(left)} != {Compile(right)}",
            SqlIntGreaterThan(var left, var right) => $"{Compile(left)} > {Compile(right)}",
            SqlIntLessThan(var left, var right) => $"{Compile(left)} < {Compile(right)}",
            SqlIntGreaterThanOrEqualTo(var left, var right) => $"{Compile(left)} >= {Compile(right)}",
            SqlIntLessThanOrEqualTo(var left, var right) => $"{Compile(left)} <= {Compile(right)}",
            
            // String comparisons (return bool)
            SqlStringEquals(var left, var right) => $"{Compile(left)} = {Compile(right)}",
            SqlStringNotEquals(var left, var right) => $"{Compile(left)} != {Compile(right)}",
            SqlStringGreaterThan(var left, var right) => $"{Compile(left)} > {Compile(right)}",
            SqlStringLessThan(var left, var right) => $"{Compile(left)} < {Compile(right)}",
            SqlStringGreaterThanOrEqualTo(var left, var right) => $"{Compile(left)} >= {Compile(right)}",
            SqlStringLessThanOrEqualTo(var left, var right) => $"{Compile(left)} <= {Compile(right)}",
            
            // String pattern matching
            SqlStringLike(var value, var pattern) => $"{Compile(value)} LIKE '{EscapeSqlString(pattern)}'",
            
            // Column references and projections
            SqlBoolProjection(var source, var name) => $"{source}.{name}",

            // Parameters
            SqlParameterBool(var name) => name,
            
            // CASE expressions
            SqlBoolCase(var condition, var trueValue, var falseValue) => 
                $"CASE WHEN {Compile(condition)} THEN {Compile(trueValue)} ELSE {Compile(falseValue)} END",
            
            _ => throw new NotSupportedException($"Boolean expression type {expr.GetType().Name} is not supported")
        };
    }

    /// <summary>
    /// Compiles integer expressions to SQL string representation.
    /// </summary>
    /// <param name="expr">The integer expression to compile</param>
    /// <returns>The SQL string representation</returns>
    public static string Compile(SqlExprInt expr)
    {
        return expr switch
        {
            // Literal values
            SqlIntValue(var value) => value.ToString(),
            
            // Unary operations
            SqlIntMinus(var operand) => $"-{Compile(operand)}",
            SqlIntAbs(var operand) => $"ABS({Compile(operand)})",
            
            // Binary arithmetic operations
            SqlIntAdd(var left, var right) => $"({Compile(left)} + {Compile(right)})",
            SqlIntSub(var left, var right) => $"({Compile(left)} - {Compile(right)})",
            SqlIntMult(var left, var right) => $"({Compile(left)} * {Compile(right)})",
            SqlIntDiv(var left, var right) => $"({Compile(left)} / {Compile(right)})",
            
            // Column references and projections (more specific patterns first)
            SqlIntColumn(var source, var name) => $"{source}.{name}",
            SqlIntProjection(var source, var name) => $"{source}.{name}",

            // Parameters
            SqlParameterInt(var name) => name,
            
            // Aggregate functions
            SqlIntCount => "COUNT(*)",
            SqlIntSum(var operand) => $"SUM({Compile(operand)})",
            SqlIntAvg(var operand) => $"AVG({Compile(operand)})",
            
            // Aggregate functions that are also queries (need special handling)
            SumSqlIntClause sumClause => throw new NotImplementedException(),
            CountClause countClause => throw new NotImplementedException(),
            
            // CASE expressions
            SqlIntCase(var condition, var trueValue, var falseValue) => 
                $"CASE WHEN {Compile(condition)} THEN {Compile(trueValue)} ELSE {Compile(falseValue)} END",
            
            _ => throw new NotSupportedException($"Integer expression type {expr.GetType().Name} is not supported")
        };
    }

    /// <summary>
    /// Compiles string expressions to SQL string representation.
    /// </summary>
    /// <param name="expr">The string expression to compile</param>
    /// <returns>The SQL string representation</returns>
    public static string Compile(SqlExprString expr)
    {
        return expr switch
        {
            // Literal values
            SqlStringValue(var value) => $"'{EscapeSqlString(value)}'",
            
            // String concatenation - use CONCAT function for better compatibility
            SqlStringConcat(var left, var right) => $"CONCAT({Compile(left)}, {Compile(right)})",
            
            // Column references and projections (more specific patterns first)
            SqlStringColumn(var source, var name) => $"{source}.{name}",
            SqlStringProjection(var source, var name) => $"{source}.{name}",
            
            // Parameters
            SqlParameterString(var name) => name,
            
            // CASE expressions
            SqlStringCase(var condition, var trueValue, var falseValue) => 
                $"CASE WHEN {Compile(condition)} THEN {Compile(trueValue)} ELSE {Compile(falseValue)} END",
            
            _ => throw new NotSupportedException($"String expression type {expr.GetType().Name} is not supported")
        };
    }

    /// <summary>
    /// Compiles any SQL expression to SQL string representation.
    /// This method uses pattern matching to determine the specific expression type.
    /// </summary>
    /// <param name="expr">The SQL expression to compile</param>
    /// <returns>The SQL string representation</returns>
    public static string Compile(SqlExpr expr)
    {
        return expr switch
        {
            SqlExprBool boolExpr => Compile(boolExpr),
            SqlExprInt intExpr => Compile(intExpr),
            SqlExprString stringExpr => Compile(stringExpr),
            _ => throw new NotSupportedException($"Expression type {expr.GetType().Name} is not supported")
        };
    }
    

    /// <summary>
    /// Compiles a tuple projection into a SELECT list.
    /// </summary>
    private static string CompileTupleProjection(ITuple tuple)
    {
        var items = new List<string>();

        for (int i = 0; i < tuple.Length; i++)
        {
            var item = tuple[i];
            if (item is SqlExpr expr)
            {
                items.Add(Compile(expr));
            }
            else if (item is ITuple subTuple)
            {
                items.Add(CompileTupleProjection(subTuple));
            }
            else
            {
                throw new NotSupportedException($"Tuple item type {item?.GetType().Name} is not supported in projections");
            }
        }
        
        return string.Join(", ", items);
    }

   

    /// <summary>
    /// Escapes special characters in SQL string literals.
    /// </summary>
    private static string EscapeSqlString(string value)
    {
        return value.Replace("'", "''");
    }
}
