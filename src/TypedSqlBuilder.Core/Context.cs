using System.Collections.Immutable;

namespace TypedSqlBuilder.Core;

/// <summary>
/// Represents the different SQL database types supported by the compiler.
/// </summary>
public enum DatabaseType
{
    SQLite,
    SqlServer
}

/// <summary>
/// Represents a SQL dialect configuration that defines database-specific syntax differences.
/// Contains all the string-based differences between SQL dialects in a single record.
/// </summary>
/// <param name="Type">The database type this dialect represents</param>
/// <param name="ParameterPrefix">The prefix used for parameters (e.g., "@" for SQL Server, ":" for SQLite)</param>
/// <param name="StringConcatOperator">The operator or function name used for string concatenation</param>
/// <param name="UsesConcatFunction">Whether string concatenation uses a function syntax (true) or operator syntax (false)</param>
public record SqlDialect(
    DatabaseType Type,
    string ParameterPrefix,
    string StringConcatOperator,
    bool UsesConcatFunction
)
{
    /// <summary>
    /// SQLite dialect configuration.
    /// Uses colon prefix for parameters and || operator for string concatenation.
    /// </summary>
    public static readonly SqlDialect SQLite = new(
        Type: DatabaseType.SQLite,
        ParameterPrefix: ":",
        StringConcatOperator: "||",
        UsesConcatFunction: false
    );

    /// <summary>
    /// SQL Server dialect configuration.
    /// Uses @ prefix for parameters and CONCAT function for string concatenation.
    /// </summary>
    public static readonly SqlDialect SqlServer = new(
        Type: DatabaseType.SqlServer,
        ParameterPrefix: "@",
        StringConcatOperator: "CONCAT",
        UsesConcatFunction: true
    );
}

/// <summary>
/// Represents an alias mapping for SQL expressions in projection contexts.
/// Used to track how expressions from inner queries should be referenced in outer queries.
/// </summary>
/// <param name="Name">The table alias name (e.g., "a0", "a1")</param>
/// <param name="Field">The field name within the aliased table</param>
public record SqlExprAlias(string Name, string Field);

/// <summary>
/// Represents the compilation context used during SQL query compilation.
/// Tracks table aliases, projection aliases, parameters, dialect, and alias indices to ensure
/// consistent SQL generation across nested queries and complex expressions.
/// </summary>
public record Context
{
    /// <summary>
    /// Gets or sets the SQL dialect used for compilation.
    /// Determines database-specific syntax like parameter prefixes and string concatenation.
    /// </summary>
    public SqlDialect Dialect { get; init; } = SqlDialect.SqlServer;

    /// <summary>
    /// Gets or sets the mapping of SQL expressions to their projection aliases.
    /// Used to reference expressions in outer queries when they were projected in inner queries.
    /// </summary>
    public ImmutableDictionary<SqlExpr, SqlExprAlias> ProjectionAliases { get; init; } = ImmutableDictionary<SqlExpr, SqlExprAlias>.Empty;

    /// <summary>
    /// Gets or sets the current alias index used for generating unique table aliases.
    /// Incremented each time a new table alias is needed (e.g., a0, a1, a2, etc.).
    /// </summary>
    public int AliasIndex { get; init; } = -1;    

    /// <summary>
    /// Gets or sets the collection of named parameters and their values.
    /// Used to generate parameterized SQL queries to prevent SQL injection.
    /// </summary>
    public ImmutableDictionary<string, object> Parameters { get; init; } = ImmutableDictionary<string, object>.Empty;

    /// <summary>
    /// Adds a named parameter with its value to the context.
    /// </summary>
    /// <param name="name">The parameter name (including prefix like @p0)</param>
    /// <param name="value">The parameter value</param>
    /// <returns>A new context with the parameter added</returns>
    public Context AddParameter(string name, object value)
    {
        return this with { Parameters = Parameters.Add(name, value) };
    }

    /// <summary>
    /// Generates a unique parameter name using the dialect's parameter prefix and adds the value to the context.
    /// </summary>
    /// <param name="value">The parameter value</param>
    /// <returns>A tuple containing the generated parameter name and the updated context</returns>
    public (string paramName, Context newContext) GenerateParameter(object value)
    {
        var paramName = $"{Dialect.ParameterPrefix}p{Parameters.Count}";
        return (paramName, AddParameter(paramName, value));
    }

    /// <summary>
    /// Generates a unique parameter name and adds the value to the context.
    /// This overload allows specifying a custom prefix (for backwards compatibility).
    /// </summary>
    /// <param name="value">The parameter value</param>
    /// <param name="prefix">The parameter prefix (overrides dialect prefix)</param>
    /// <returns>A tuple containing the generated parameter name and the updated context</returns>
    public (string paramName, Context newContext) GenerateParameter(object value, string prefix)
    {
        var paramName = $"{prefix}p{Parameters.Count}";
        return (paramName, AddParameter(paramName, value));
    }

    
}
