namespace TypedSqlBuilder.Core;

/// <summary>
/// A universal SQL NULL value that can be implicitly converted to any typed SQL expression.
/// This provides a clean API: SqlNull.Value instead of SqlIntNull.Value, SqlStringNull.Value, etc.
/// </summary>
public class SqlNull
{
    public static SqlNull Value => new();
    
    // Implicit conversions to specific typed null expressions
    public static implicit operator SqlExprInt(SqlNull _) => new SqlIntNull();
    public static implicit operator SqlExprString(SqlNull _) => new SqlStringNull();
    public static implicit operator SqlExprBool(SqlNull _) => new SqlBoolNull();
}
