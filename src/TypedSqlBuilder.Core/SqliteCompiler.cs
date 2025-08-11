namespace TypedSqlBuilder.Core;

/// <summary>
/// SQLite-specific SQL query compiler that handles SQLite dialect differences.
/// </summary>
public class SqliteCompiler : SqlCompiler
{
    /// <summary>
    /// SQLite uses : as parameter prefix instead of @
    /// </summary>
    protected override string ParameterPrefix => ":";

    /// <summary>
    /// Compiles SQL expressions with SQLite-specific handling.
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
            // SQLite uses 1/0 for boolean literals instead of TRUE/FALSE
            case SqlBoolValue(var value):
                return GenerateParameter(context, value ? 1 : 0);
            
            // SQLite uses || operator for string concatenation
            case SqlStringConcat(var left, var right):
            {
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"({leftSql} || {rightSql})", rightCtx);
            }
            
            // For all other expressions, use base implementation
            default:
                return base.Compile(expr, context);
        }
    }

    public override (string, Context) Compile(ISqlStatement statement, Context context)
    {
        return base.Compile(statement, context);
    }
}
