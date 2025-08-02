using System.Runtime.CompilerServices;

namespace TypedSqlBuilder.Core;

/// <summary>
/// SQLite-specific SQL query compiler that handles SQLite dialect differences.
/// </summary>
public class SqliteQueryCompiler : SqlQueryCompilerBase
{
    /// <summary>
    /// SQLite uses : as parameter prefix instead of @
    /// </summary>
    protected override string ParameterPrefix => ":";

    /// <summary>
    /// Compiles boolean expressions with SQLite-specific handling.
    /// </summary>
    public override (string, Context) Compile(SqlExprBool expr, Context context)
    {
        return expr switch
        {
            // SQLite uses 1/0 for boolean literals instead of TRUE/FALSE
            SqlBoolValue(var value) => GenerateParameter(context, value ? 1 : 0),
            
            // For all other expressions, use base implementation
            _ => base.Compile(expr, context)
        };
    }

    /// <summary>
    /// Compiles string expressions with SQLite-specific handling.
    /// </summary>
    public override (string, Context) Compile(SqlExprString expr, Context context)
    {
        switch (expr)
        {
            // SQLite uses || operator for string concatenation
            case SqlStringConcat(var left, var right):
                var (leftSql, leftCtx) = Compile(left, context);
                var (rightSql, rightCtx) = Compile(right, leftCtx);
                return ($"({leftSql} || {rightSql})", rightCtx);
            
            // For all other expressions, use base implementation
            default:
                return base.Compile(expr, context);
        }
    }
}
