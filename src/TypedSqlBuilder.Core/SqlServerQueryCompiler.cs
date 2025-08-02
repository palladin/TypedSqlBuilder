using System.Runtime.CompilerServices;

namespace TypedSqlBuilder.Core;

/// <summary>
/// SQL Server-specific SQL query compiler that handles SQL Server dialect differences.
/// </summary>
public class SqlServerQueryCompiler : SqlQueryCompilerBase
{
    /// <summary>
    /// SQL Server uses @ as parameter prefix (which is the default)
    /// </summary>
    protected override string ParameterPrefix => "@";

    /// <summary>
    /// Compiles boolean expressions with SQL Server-specific handling.
    /// </summary>
    public override (string, Context) Compile(SqlExprBool expr, Context context)
    {
        return expr switch
        {
            // SQL Server uses 1/0 for boolean literals instead of TRUE/FALSE
            SqlBoolValue(var value) => GenerateParameter(context, value ? 1 : 0),
            
            // For all other expressions, use base implementation
            _ => base.Compile(expr, context)
        };
    }

    // SQL Server uses CONCAT for string concatenation, which is already the default in base class
    // No need to override string compilation
}
