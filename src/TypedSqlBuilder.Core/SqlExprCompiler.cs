using System.Text;
using System.Runtime.CompilerServices;

namespace TypedSqlBuilder.Core;

/// <summary>
/// SQL Compiler - Compiles SQL expressions and clauses to SQL string representation.
/// This class handles both SQL expressions and SQL clauses like SELECT, FROM, WHERE, ORDER BY.
/// It uses pattern matching to traverse the expression tree and generate appropriate SQL text.
/// </summary>
public static class SqlExprCompiler
{
    /// <summary>
    /// Compiles any SQL query or expression to its SQL string representation.
    /// This is the main entry point that handles both expressions and clauses.
    /// </summary>
    /// <param name="item">The SQL query, clause, or expression to compile</param>
    /// <returns>The SQL string representation</returns>
    public static string Compile(object item)
    {
        return item switch
        {
            // SQL expressions
            SqlExprBool boolExpr => CompileBoolExpr(boolExpr),
            SqlExprInt intExpr => CompileIntExpr(intExpr),
            SqlExprString stringExpr => CompileStringExpr(stringExpr),
            
            // SQL clauses and queries
            ISqlQuery query => CompileQuery(query),
            
            _ => throw new NotSupportedException($"Type {item.GetType().Name} is not supported")
        };
    }

    /// <summary>
    /// Compiles any SQL expression to its SQL string representation.
    /// This is the main entry point for expression compilation.
    /// </summary>
    /// <param name="expr">The SQL expression to compile</param>
    /// <returns>The SQL string representation of the expression</returns>
    public static string Compile(SqlExpr expr)
    {
        return expr switch
        {
            // Boolean expressions
            SqlExprBool boolExpr => CompileBoolExpr(boolExpr),
            
            // Integer expressions  
            SqlExprInt intExpr => CompileIntExpr(intExpr),
            
            // String expressions
            SqlExprString stringExpr => CompileStringExpr(stringExpr),
            
            _ => throw new NotSupportedException($"Expression type {expr.GetType().Name} is not supported")
        };
    }

    /// <summary>
    /// Compiles SQL queries and clauses to SQL string representation.
    /// </summary>
    /// <param name="query">The SQL query or clause to compile</param>
    /// <returns>The SQL string representation</returns>
    private static string CompileQuery(ISqlQuery query)
    {
        return query switch
        {
            // Use more flexible pattern matching for generic types
            FromClause fromClause => CompileFromClauseGeneric(fromClause),
            SelectClause selectClause => CompileSelectClause(selectClause),
            SelectSqlIntClause selectIntClause => CompileSelectSqlIntClause(selectIntClause),
            WhereClause whereClause => CompileWhereClause(whereClause),
            OrderByClause orderByClause => CompileOrderByClause(orderByClause),
            
            // Aggregate functions that are also queries
            SumSqlIntClause sumClause => CompileSumClause(sumClause),
            CountClause countClause => CompileCountClause(countClause),
            
            // Tables
            SqlTable table => CompileTable(table),
            
            _ => throw new NotSupportedException($"Query type {query.GetType().Name} is not supported")
        };
    }

    /// <summary>
    /// Compiles a FROM clause using reflection to handle any generic type.
    /// </summary>
    private static string CompileFromClauseGeneric(FromClause fromClause)
    {
        // Use reflection to access the Table property from the generic type
        var type = fromClause.GetType();
        var tableProperty = type.GetProperty("Table");
        
        if (tableProperty != null)
        {
            var table = tableProperty.GetValue(fromClause);
            if (table is SqlTable sqlTable)
            {
                return $"SELECT * FROM {CompileTable(sqlTable)}";
            }
        }
        
        // Fallback for abstract base type
        return "SELECT * FROM <table>";
    }

    /// <summary>
    /// Compiles a SELECT clause.
    /// </summary>
    private static string CompileSelectClause(SelectClause selectClause)
    {
        var fromSql = CompileQuery(selectClause.Query);
        
        // Try to extract the selector and compile it
        try
        {
            // For now, we'll assume we're selecting from a table and use a placeholder
            // In a real implementation, we'd need to evaluate the selector with sample data
            // to understand what columns are being selected
            return $"SELECT <columns> FROM ({fromSql})";
        }
        catch
        {
            return $"SELECT * FROM ({fromSql})";
        }
    }

    /// <summary>
    /// Compiles a SELECT clause that returns integer expressions.
    /// </summary>
    private static string CompileSelectSqlIntClause(SelectSqlIntClause selectClause)
    {
        var fromSql = CompileQuery(selectClause.Query);
        
        try
        {
            // Try to compile the selector - this is tricky without runtime evaluation
            return $"SELECT <int_expr> FROM ({fromSql})";
        }
        catch
        {
            return $"SELECT * FROM ({fromSql})";
        }
    }

    /// <summary>
    /// Compiles a WHERE clause.
    /// </summary>
    private static string CompileWhereClause(WhereClause whereClause)
    {
        var querySql = CompileQuery(whereClause.Query);
        
        try
        {
            // Try to compile the predicate - this requires evaluating the lambda with sample data
            // For now, we'll use a placeholder
            return $"{querySql} WHERE <predicate>";
        }
        catch
        {
            return $"{querySql} WHERE <predicate>";
        }
    }

    /// <summary>
    /// Compiles an ORDER BY clause.
    /// </summary>
    private static string CompileOrderByClause(OrderByClause orderByClause)
    {
        var querySql = CompileQuery(orderByClause.Query);
        var direction = orderByClause.Descending ? "DESC" : "ASC";
        
        try
        {
            // Try to compile the key selector
            return $"{querySql} ORDER BY <key> {direction}";
        }
        catch
        {
            return $"{querySql} ORDER BY <key> {direction}";
        }
    }

    /// <summary>
    /// Compiles a SUM aggregate function.
    /// </summary>
    private static string CompileSumClause(SumSqlIntClause sumClause)
    {
        sumClause.Deconstruct(out var query);
        var subQuery = CompileQuery(query);
        return $"SUM(({subQuery}))";
    }

    /// <summary>
    /// Compiles a COUNT aggregate function.
    /// </summary>
    private static string CompileCountClause(CountClause countClause)
    {
        return "COUNT(*)";
    }

    /// <summary>
    /// Compiles a table reference.
    /// </summary>
    private static string CompileTable(SqlTable table)
    {
        return table.TableName;
    }

    /// <summary>
    /// Compiles boolean expressions to SQL.
    /// </summary>
    private static string CompileBoolExpr(SqlExprBool expr)
    {
        return expr switch
        {
            // Literal values
            SqlBoolValue(var value) => value ? "TRUE" : "FALSE",
            
            // Logical operations
            SqlBoolNot(var operand) => $"NOT ({Compile(operand)})",
            SqlBoolAnd(var left, var right) => $"({Compile(left)} AND {Compile(right)})",
            SqlBoolOr(var left, var right) => $"({Compile(left)} OR {Compile(right)})",
            
            // Boolean equality/inequality
            SqlBoolEquals(var left, var right) => $"({Compile(left)} = {Compile(right)})",
            SqlBoolNotEquals(var left, var right) => $"({Compile(left)} != {Compile(right)})",
            
            // Integer comparisons (return bool)
            SqlIntEquals(var left, var right) => $"({Compile(left)} = {Compile(right)})",
            SqlIntNotEquals(var left, var right) => $"({Compile(left)} != {Compile(right)})",
            SqlIntGreaterThan(var left, var right) => $"({Compile(left)} > {Compile(right)})",
            SqlIntLessThan(var left, var right) => $"({Compile(left)} < {Compile(right)})",
            SqlIntGreaterThanOrEqualTo(var left, var right) => $"({Compile(left)} >= {Compile(right)})",
            SqlIntLessThanOrEqualTo(var left, var right) => $"({Compile(left)} <= {Compile(right)})",
            
            // String comparisons (return bool)
            SqlStringEquals(var left, var right) => $"({Compile(left)} = {Compile(right)})",
            SqlStringNotEquals(var left, var right) => $"({Compile(left)} != {Compile(right)})",
            SqlStringGreaterThan(var left, var right) => $"({Compile(left)} > {Compile(right)})",
            SqlStringLessThan(var left, var right) => $"({Compile(left)} < {Compile(right)})",
            SqlStringGreaterThanOrEqualTo(var left, var right) => $"({Compile(left)} >= {Compile(right)})",
            SqlStringLessThanOrEqualTo(var left, var right) => $"({Compile(left)} <= {Compile(right)})",
            
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
    /// Compiles integer expressions to SQL.
    /// </summary>
    private static string CompileIntExpr(SqlExprInt expr)
    {
        return expr switch
        {
            // Literal values
            SqlIntValue(var value) => value.ToString(),
            
            // Unary operations
            SqlIntMinus(var operand) => $"-({Compile(operand)})",
            SqlIntAbs(var operand) => $"ABS({Compile(operand)})",
            
            // Binary arithmetic operations
            SqlIntAdd(var left, var right) => $"({Compile(left)} + {Compile(right)})",
            SqlIntSub(var left, var right) => $"({Compile(left)} - {Compile(right)})",
            SqlIntMult(var left, var right) => $"({Compile(left)} * {Compile(right)})",
            SqlIntDiv(var left, var right) => $"({Compile(left)} / {Compile(right)})",
            
            // Column references and projections
            SqlIntProjection(var source, var name) => $"{source}.{name}",
            
            // Parameters
            SqlParameterInt(var name) => name,
            
            // Aggregate functions
            SqlIntCount => "COUNT(*)",
            SqlIntSum(var operand) => $"SUM({Compile(operand)})",
            SqlIntAvg(var operand) => $"AVG({Compile(operand)})",
            
            // Aggregate functions that are also queries (need special handling)
            SumSqlIntClause sumClause => CompileSumClause(sumClause),
            CountClause countClause => CompileCountClause(countClause),
            
            // CASE expressions
            SqlIntCase(var condition, var trueValue, var falseValue) => 
                $"CASE WHEN {Compile(condition)} THEN {Compile(trueValue)} ELSE {Compile(falseValue)} END",
            
            _ => throw new NotSupportedException($"Integer expression type {expr.GetType().Name} is not supported")
        };
    }

    /// <summary>
    /// Compiles string expressions to SQL.
    /// </summary>
    private static string CompileStringExpr(SqlExprString expr)
    {
        return expr switch
        {
            // Literal values
            SqlStringValue(var value) => $"'{EscapeSqlString(value)}'",
            
            // String concatenation
            SqlStringConcat(var left, var right) => $"({Compile(left)} || {Compile(right)})",
            
            // Column references and projections
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
    /// Escapes special characters in SQL string literals.
    /// This prevents SQL injection and ensures proper string formatting.
    /// </summary>
    /// <param name="value">The string value to escape</param>
    /// <returns>The escaped string safe for SQL</returns>
    private static string EscapeSqlString(string value)
    {
        // Basic SQL string escaping - replace single quotes with doubled single quotes
        return value.Replace("'", "''");
    }

    /// <summary>
    /// Compiles a collection of expressions with a separator.
    /// Useful for SELECT lists, parameter lists, etc.
    /// </summary>
    /// <param name="expressions">The expressions to compile</param>
    /// <param name="separator">The separator between expressions (default: ", ")</param>
    /// <returns>The compiled expressions joined by the separator</returns>
    public static string CompileList(IEnumerable<SqlExpr> expressions, string separator = ", ")
    {
        return string.Join(separator, expressions.Select(Compile));
    }

    /// <summary>
    /// Compiles expressions with optional aliasing.
    /// Used for SELECT clause compilation where expressions can have aliases.
    /// </summary>
    /// <param name="expressionsWithAliases">Tuples of (expression, alias)</param>
    /// <returns>The compiled expression list with aliases</returns>
    public static string CompileSelectList(IEnumerable<(SqlExpr expr, string? alias)> expressionsWithAliases)
    {
        var compiledExpressions = expressionsWithAliases.Select(item =>
        {
            var compiledExpr = Compile(item.expr);
            return string.IsNullOrEmpty(item.alias) ? compiledExpr : $"{compiledExpr} AS {item.alias}";
        });
        
        return string.Join(", ", compiledExpressions);
    }
}
