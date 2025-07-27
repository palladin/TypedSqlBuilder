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
            FromClause fromClause => CompileFromClause(fromClause),
            SelectClause selectClause => CompileSelectClause(selectClause),
            SelectSqlIntClause selectIntClause => CompileSelectSqlIntClause(selectIntClause),
            WhereClause whereClause => CompileWhereClause(whereClause),
            OrderByClause orderByClause => CompileOrderByClause(orderByClause),
            
            // Aggregate functions that are also queries
            SumSqlIntClause sumClause => CompileSumClause(sumClause),
            CountClause countClause => CompileCountClause(countClause),
            
            // Tables
            ISqlTable table => CompileTable(table),
            
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
            SqlBoolProjection(var source, var name) => FormatColumnReference(source, name),
            
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
            SqlIntColumn(var source, var name) => FormatColumnReference(source, name),
            SqlIntProjection(var source, var name) => FormatColumnReference(source, name),
            
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
            SqlStringColumn(var source, var name) => FormatColumnReference(source, name),
            SqlStringProjection(var source, var name) => FormatColumnReference(source, name),
            
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
    /// Compiles a FROM clause, handling both generic and non-generic versions.
    /// </summary>
    private static string CompileFromClause(FromClause fromClause)
    {
        var tableName = CompileTable(fromClause.Table);
        return $"SELECT * FROM {tableName}";
    }

    /// <summary>
    /// Compiles a SELECT clause by evaluating the selector function.
    /// </summary>
    private static string CompileSelectClause(SelectClause selectClause)
    {
        var baseQuery = Compile(selectClause.Query);
        
        // Extract SELECT part if the base query is a full SELECT statement
        var fromPart = ExtractFromPart(baseQuery);
        
        try
        {
            // Try to evaluate the selector with a sample tuple to extract the structure
            var sampleColumns = CreateSampleTuple(selectClause.Query);
            if (sampleColumns != null)
            {
                var result = selectClause.Selector(sampleColumns);
                var selectList = CompileTupleProjection(result);
                return $"SELECT {selectList}{fromPart}";
            }
        }
        catch
        {
            // Fall back to placeholder if evaluation fails
        }
        
        return $"SELECT *{fromPart}";
    }

    /// <summary>
    /// Compiles a SELECT clause that returns integer expressions.
    /// </summary>
    private static string CompileSelectSqlIntClause(SelectSqlIntClause selectIntClause)
    {
        var baseQuery = Compile(selectIntClause.Query);
        var fromPart = ExtractFromPart(baseQuery);
        
        try
        {
            var sampleColumns = CreateSampleTuple(selectIntClause.Query);
            if (sampleColumns != null)
            {
                var result = selectIntClause.Selector(sampleColumns);
                var selectExpr = Compile(result);
                return $"SELECT {selectExpr}{fromPart}";
            }
        }
        catch
        {
            // Fall back to placeholder
        }
        
        return $"SELECT *{fromPart}";
    }

    /// <summary>
    /// Compiles a WHERE clause by evaluating the predicate function.
    /// </summary>
    private static string CompileWhereClause(WhereClause whereClause)
    {
        var baseQuery = Compile(whereClause.Query);
        var fromPart = ExtractFromPart(baseQuery);
        
        try
        {
            var sampleColumns = CreateSampleTuple(whereClause.Query);
            if (sampleColumns != null)
            {
                var predicate = whereClause.Predicate(sampleColumns);
                var whereExpr = Compile(predicate);
                return $"SELECT *{fromPart} WHERE {whereExpr}";
            }
        }
        catch
        {
            // Fall back to placeholder
        }
        
        return $"SELECT *{fromPart} WHERE <predicate>";
    }

    /// <summary>
    /// Compiles an ORDER BY clause by evaluating the key selector function.
    /// </summary>
    private static string CompileOrderByClause(OrderByClause orderByClause)
    {
        var baseQuery = Compile(orderByClause.Query);
        var direction = orderByClause.Descending ? "DESC" : "ASC";
        
        try
        {
            var sampleColumns = CreateSampleTuple(orderByClause.Query);
            if (sampleColumns != null)
            {
                var keyExpr = orderByClause.KeySelector(sampleColumns);
                var orderByExpr = Compile(keyExpr);
                return $"{baseQuery} ORDER BY {orderByExpr} {direction}";
            }
        }
        catch
        {
            // Fall back to placeholder
        }
        
        return $"{baseQuery} ORDER BY <key> {direction}";
    }

    /// <summary>
    /// Creates a sample tuple that represents the columns available from a query.
    /// This is used to evaluate lambda expressions and extract SQL expressions.
    /// </summary>
    private static ITuple? CreateSampleTuple(ISqlQuery query)
    {
        try
        {
            // Handle different query types
            return query switch
            {
                FromClause fromClause => CreateSampleTupleFromTable(fromClause.Table),
                WhereClause whereClause => CreateSampleTuple(whereClause.Query),
                OrderByClause orderByClause => CreateSampleTuple(orderByClause.Query),
                SelectClause selectClause => CreateSampleTuple(selectClause.Query),
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }
    
    /// <summary>
    /// Creates a sample tuple from a table that represents the columns available.
    /// This is used to evaluate lambda expressions and extract SQL expressions.
    /// </summary>
    private static ITuple? CreateSampleTupleFromTable(ISqlTable table)
    {
        try
        {
            // Return the table itself since ISqlTable extends ITuple
            return table;
        }
        catch
        {
            return null;
        }
    }
    

    /// <summary>
    /// Extracts the FROM part of a SQL query string.
    /// </summary>
    private static string ExtractFromPart(string sqlQuery)
    {
        var fromIndex = sqlQuery.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
        if (fromIndex >= 0)
        {
            return sqlQuery.Substring(fromIndex);
        }
        return " FROM <table>";
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
            else
            {
                items.Add(item?.ToString() ?? "NULL");
            }
        }
        
        return string.Join(", ", items);
    }

    /// <summary>
    /// Compiles a SUM aggregate function.
    /// </summary>
    private static string CompileSumClause(SumSqlIntClause sumClause)
    {
        sumClause.Deconstruct(out var query);
        var subQuery = Compile(query);
        
        // Extract the SELECT expression from the subquery
        if (subQuery.StartsWith("SELECT "))
        {
            var selectPart = subQuery.Substring(7);
            var fromIndex = selectPart.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
            if (fromIndex >= 0)
            {
                var expression = selectPart.Substring(0, fromIndex);
                return $"SUM({expression})";
            }
        }
        
        return $"SUM(({subQuery}))";
    }

    /// <summary>
    /// Compiles a COUNT aggregate function.
    /// </summary>
    private static string CompileCountClause(CountClause countClause)
    {
        // Use reflection to get the query if available
        var type = countClause.GetType();
        var queryField = type.GetField("Query", BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (queryField != null && queryField.GetValue(countClause) is ISqlQuery query)
        {
            var baseQuery = Compile(query);
            var fromPart = ExtractFromPart(baseQuery);
            return $"COUNT(*){fromPart}";
        }
        
        return "COUNT(*)";
    }

    /// <summary>
    /// Compiles a table reference.
    /// </summary>
    private static string CompileTable(ISqlTable table)
    {
        return table.TableName;
    }

    /// <summary>
    /// Formats a column reference with proper table prefix.
    /// </summary>
    private static string FormatColumnReference(string source, string name)
    {
        if (string.IsNullOrEmpty(source))
        {
            return name;
        }
        return $"{source}.{name}";
    }

    /// <summary>
    /// Escapes special characters in SQL string literals.
    /// </summary>
    private static string EscapeSqlString(string value)
    {
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
