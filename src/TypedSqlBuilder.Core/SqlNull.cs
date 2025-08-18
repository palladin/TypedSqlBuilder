namespace TypedSqlBuilder.Core;

/// <summary>
/// A universal SQL NULL value that can be implicitly converted to any typed SQL expression.
/// This provides a clean API: SqlNull.Value instead of SqlIntNull.Value, SqlStringNull.Value, etc.
/// </summary>
public class SqlNull
{
    /// <summary>
    /// Gets a singleton instance of SqlNull that can be implicitly converted to any typed SQL null expression.
    /// </summary>
    public static SqlNull Value => new();
    
    /// <summary>
    /// Implicitly converts SqlNull to a typed integer null expression.
    /// </summary>
    /// <param name="_">The SqlNull instance (ignored)</param>
    /// <returns>A SqlIntNull instance</returns>
    public static implicit operator SqlExprInt(SqlNull _) => new SqlIntNull();
    
    /// <summary>
    /// Implicitly converts SqlNull to a typed string null expression.
    /// </summary>
    /// <param name="_">The SqlNull instance (ignored)</param>
    /// <returns>A SqlStringNull instance</returns>
    public static implicit operator SqlExprString(SqlNull _) => new SqlStringNull();
    
    /// <summary>
    /// Implicitly converts SqlNull to a typed boolean null expression.
    /// </summary>
    /// <param name="_">The SqlNull instance (ignored)</param>
    /// <returns>A SqlBoolNull instance</returns>
    public static implicit operator SqlExprBool(SqlNull _) => new SqlBoolNull();
}
