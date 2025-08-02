namespace TypedSqlBuilder.Core;

/// <summary>
/// Static factory class for SQL query compilation with backward compatibility.
/// Provides easy access to different database-specific compilers.
/// </summary>
public static class SqlQueryCompiler
{
    private static readonly SqlQueryCompilerBase _defaultCompiler = new SqlServerQueryCompiler();
    private static readonly SqliteQueryCompiler _sqliteCompiler = new SqliteQueryCompiler();
    private static readonly SqlServerQueryCompiler _sqlServerCompiler = new SqlServerQueryCompiler();

    /// <summary>
    /// Gets the SQLite-specific query compiler.
    /// </summary>
    public static SqliteQueryCompiler Sqlite => _sqliteCompiler;

    /// <summary>
    /// Gets the SQL Server-specific query compiler.
    /// </summary>
    public static SqlServerQueryCompiler SqlServer => _sqlServerCompiler;

    /// <summary>
    /// Compiles SQL queries using the default compiler (SQL Server).
    /// Maintained for backward compatibility.
    /// </summary>
    /// <param name="query">The SQL query to compile</param>
    /// <returns>The SQL string representation</returns>
    public static string Compile(ISqlQuery query)
    {
        var (result, _) = _defaultCompiler.Compile(query, new Context());
        return result;
    }

    /// <summary>
    /// Compiles boolean expressions using the default compiler (SQL Server).
    /// Maintained for backward compatibility.
    /// </summary>
    /// <param name="expr">The boolean expression to compile</param>
    /// <returns>The SQL string representation</returns>
    public static string Compile(SqlExprBool expr)
    {
        var (result, _) = _defaultCompiler.Compile(expr, new Context());
        return result;
    }

    /// <summary>
    /// Compiles integer expressions using the default compiler (SQL Server).
    /// Maintained for backward compatibility.
    /// </summary>
    /// <param name="expr">The integer expression to compile</param>
    /// <returns>The SQL string representation</returns>
    public static string Compile(SqlExprInt expr)
    {
        var (result, _) = _defaultCompiler.Compile(expr, new Context());
        return result;
    }

    /// <summary>
    /// Compiles string expressions using the default compiler (SQL Server).
    /// Maintained for backward compatibility.
    /// </summary>
    /// <param name="expr">The string expression to compile</param>
    /// <returns>The SQL string representation</returns>
    public static string Compile(SqlExprString expr)
    {
        var (result, _) = _defaultCompiler.Compile(expr, new Context());
        return result;
    }

    /// <summary>
    /// Compiles any SQL expression using the default compiler (SQL Server).
    /// Maintained for backward compatibility.
    /// </summary>
    /// <param name="expr">The SQL expression to compile</param>
    /// <returns>The SQL string representation</returns>
    public static string Compile(SqlExpr expr)
    {
        var (result, _) = _defaultCompiler.Compile(expr, new Context());
        return result;
    }
}
