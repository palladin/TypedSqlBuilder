using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace TypedSqlBuilder.Core;

/// <summary>
/// SQL Compiler - Statement compilation methods.
/// This partial class contains all methods related to compiling SQL statements (INSERT, UPDATE, DELETE).
/// </summary>
internal static partial class SqlCompiler
{
    /// <summary>
    /// Compiles SQL statements to SQL string representation.
    /// </summary>
    /// <param name="statement">The SQL statement to compile</param>
    /// <param name="context">The compilation context</param>
    /// <param name="scopeLevel">The nesting scope level for the SQL statement</param>
    /// <returns>The SQL string representation and updated context</returns>
    internal static (string, Context) Compile(ISqlStatement statement, Context context, int scopeLevel)
    {
        switch (statement)
        {
            // ========== DELETE STATEMENTS ==========
            case DeleteStatement(var table):
                return ($"DELETE FROM {table.TableName}", context);

            case DeleteWhereStatement(DeleteStatement(var table), var predicate):
            {                
                var (whereClause, whereCtx) = Compile(predicate(table), context, scopeLevel);
                return ($"DELETE FROM {table.TableName} WHERE {whereClause}", whereCtx);
            }

            // ========== UPDATE STATEMENTS ==========
            // Note: Bare UpdateStatement without SET clauses should not be compiled
            // UPDATE statements must have at least one SET clause via SetStatement fusion
            case UpdateStatement(var table):
                throw new InvalidOperationException($"UPDATE statement for table {table.TableName} must have at least one SET clause. Use .Set(...) to add SET clauses.");

            // ========== INSERT STATEMENTS ==========
            // Note: Bare InsertStatement without VALUE clauses should not be compiled
            // INSERT statements must have at least one VALUE clause via ValueStatement fusion
            case InsertStatement(var table):
                throw new InvalidOperationException($"INSERT statement for table {table.TableName} must have at least one VALUE clause. Use .Value(...) to add VALUE clauses.");

            // ========== STATEMENT FUSION PATTERNS ==========
            // SET fusion: SET(SET(...), setClause) → SET(..., setClause) (recursive)
            // Also handles SET(UPDATE(table), setClause) → UPDATE(table, [setClause])
            case SetStatement(var innerStatement, var setClause):
            {
                var (table, setClauses) = ExtractUpdateTableAndClauses(innerStatement);                
                var newSetClauses = setClauses.Append(setClause).ToImmutableArray();
                var (setClauseSql, setCtx) = CompileSetClauses(newSetClauses, table, context, scopeLevel);
                return ($"UPDATE {table.TableName} SET {setClauseSql}", setCtx);
            }    

            // WHERE fusion for UPDATE: WHERE(UPDATE(...), predicate) or WHERE(SET(...), predicate)
            case UpdateWhereFromStatement(var updateStatement, var predicate):
            {
                var (table, setClauses) = ExtractUpdateTableAndClauses(updateStatement);                
                var (setClauseSql, setCtx) = CompileSetClauses(setClauses, table, context, scopeLevel);
                var (whereClause, whereCtx) = Compile(predicate(table), setCtx, scopeLevel);
                return ($"UPDATE {table.TableName} SET {setClauseSql} WHERE {whereClause}", whereCtx);
            }            // ========== STATEMENT FUSION PATTERNS ==========
            case ValueStatement(var innerStatement, var valueClause):
            {
                var (table, valueClauses) = ExtractInsertTableAndClauses(innerStatement);
                var newValueClauses = valueClauses.Append(valueClause).ToImmutableArray();                
                var (columnsClause, valuesClause, valuesCtx) = CompileInsertValueClauses(newValueClauses, table, context, scopeLevel);
                return ($"INSERT INTO {table.TableName} ({columnsClause}) VALUES ({valuesClause})", valuesCtx);            
            }            

            default:
                throw new NotSupportedException($"Statement type {statement.GetType().Name} is not supported");
        }
    }

    /// <summary>
    /// Compiles SET clauses for UPDATE statements.
    /// </summary>
    /// <param name="setClauses">The SET clauses to compile</param>
    /// <param name="table">The table being updated</param>
    /// <param name="context">The compilation context</param>
    /// <param name="scopeLevel">The nesting scope level for the SQL statement</param>
    /// <returns>The SQL string representation and updated context</returns>
    private static (string, Context) CompileSetClauses(ImmutableArray<SetClause> setClauses, SqlTable table, Context context, int scopeLevel)
    {
        var items = new List<string>();
        var ctx = context;

        foreach (var setClause in setClauses)
        {
            var column = setClause.ColumnSelector(table);
            var columnSql = 
                column is ISqlColumn sqlColumn ?
                    sqlColumn.ColumnName :
                    throw new NotSupportedException($"Column must implement ISqlColumn");

            var (valueSql, valueCtx) = Compile(setClause.Value, ctx, scopeLevel);
            items.Add($"{columnSql} = {valueSql}");
            ctx = valueCtx;
        }

        return (string.Join(", ", items), ctx);
    }    

    /// <summary>
    /// Compiles INSERT VALUE clauses.
    /// </summary>
    /// <param name="valueClauses">The VALUE clauses to compile</param>
    /// <param name="table">The table being inserted into</param>
    /// <param name="context">The compilation context</param>
    /// <param name="scopeLevel">The nesting scope level for the SQL statement</param>
    /// <returns>The columns clause, values clause, and updated context</returns>
    private static (string, string, Context) CompileInsertValueClauses(ImmutableArray<ValueClause> valueClauses, SqlTable table, Context context, int scopeLevel)
    {
        var columns = new List<string>();
        var valuesSql = new List<string>();
        var ctx = context;

        foreach (var valueClause in valueClauses)
        {
            var column = valueClause.ColumnSelector(table);
            var (valueSql, valueCtx) = SqlCompiler.Compile(valueClause.Value, ctx, scopeLevel);

            if (column is ISqlColumn sqlColumn)
                columns.Add(sqlColumn.ColumnName);
            else
                throw new NotSupportedException($"Column must implement ISqlColumn");
            
            valuesSql.Add(valueSql);
            ctx = valueCtx;
        }

        return (string.Join(", ", columns), string.Join(", ", valuesSql), ctx);
    }

    /// <summary>
    /// Helper method to extract table and SET clauses from an UPDATE statement chain.
    /// Recursively processes nested SetStatement structures to build the complete list of SET clauses.
    /// </summary>
    /// <param name="statement">The UPDATE or SET statement to extract from</param>
    /// <returns>A tuple containing the target table and all SET clauses</returns>
    /// <exception cref="NotSupportedException">Thrown when the statement type is not supported for extraction</exception>
    private static (SqlTable table, ImmutableArray<SetClause> setClauses) ExtractUpdateTableAndClauses(ISqlStatement statement)
    {
        switch (statement)
        {
            case UpdateStatement(var table):
                return (table, ImmutableArray<SetClause>.Empty);
            case SetStatement(var innerStatement, var setClause):
                var (innerTable, innerSetClauses) = ExtractUpdateTableAndClauses(innerStatement);
                return (innerTable, innerSetClauses.Append(setClause).ToImmutableArray());
            default:
                throw new NotSupportedException($"Cannot extract UPDATE table and clauses from statement type {statement.GetType().Name}");
        }
    }

    /// <summary>
    /// Helper method to extract table and VALUE clauses from an INSERT statement chain.
    /// Recursively processes nested ValueStatement structures to build the complete list of VALUE clauses.
    /// </summary>
    /// <param name="statement">The INSERT or VALUE statement to extract from</param>
    /// <returns>A tuple containing the target table and all VALUE clauses</returns>
    /// <exception cref="NotSupportedException">Thrown when the statement type is not supported for extraction</exception>
    private static (SqlTable table, ImmutableArray<ValueClause> valueClauses) ExtractInsertTableAndClauses(ISqlStatement statement)
    {
        switch (statement)
        {
            case InsertStatement(var table):
                return (table, ImmutableArray<ValueClause>.Empty);
            case ValueStatement(var innerStatement, var valueClause):
                var (innerTable, innerValueClauses) = ExtractInsertTableAndClauses(innerStatement);
                return (innerTable, innerValueClauses.Add(valueClause));
            default:
                throw new NotSupportedException($"Cannot extract INSERT table and clauses from statement type {statement.GetType().Name}");
        }
    }
}
