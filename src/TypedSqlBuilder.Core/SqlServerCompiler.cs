namespace TypedSqlBuilder.Core;

/// <summary>
/// SQL Server-specific SQL query compiler that handles SQL Server dialect differences.
/// </summary>
public class SqlServerCompiler : SqlCompiler
{
    /// <summary>
    /// SQL Server uses @ as parameter prefix (which is the default)
    /// </summary>
    protected override string ParameterPrefix => "@";

    /// <summary>
    /// Compiles SQL expressions with SQL Server-specific handling.
    /// </summary>
    public override (string, Context) Compile(SqlExpr expr, Context context)
    {
        // Check for projection aliases first (from base class)
        if (context.ProjectionAliases.TryGetValue(expr, out var alias))
        {
            return ($"{alias.Name}.{alias.Field}", context);
        }

        switch (expr)
        {
            // SQL Server uses 1/0 for boolean literals instead of TRUE/FALSE
            case SqlBoolValue(var value):
                return GenerateParameter(context, value ? 1 : 0);
            
            // For all other expressions, use base implementation
            default:
                return base.Compile(expr, context);
        }
    }

    public override (string, Context) Compile(ISqlStatement statement, Context context)
    {
        return base.Compile(statement, context);
    }

    // SQL Server uses CONCAT for string concatenation, which is already the default in base class
    // No need to override string compilation
}
