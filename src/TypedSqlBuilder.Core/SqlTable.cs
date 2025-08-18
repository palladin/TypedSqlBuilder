using System.Collections.Immutable;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TypedSqlBuilder.Core;

/// <summary>
/// Abstract base class for SQL table representations.
/// Provides the foundation for type-safe table definitions with strongly-typed column mappings.
/// Implements ITuple to integrate with the tuple-based relational algebra system.
/// </summary>
/// <remarks>
/// This class uses reflection to automatically populate columns based on properties decorated 
/// with ColumnAttribute. Each property must be of a supported SQL expression type 
/// (SqlIntColumn, SqlStringColumn, SqlBoolColumn) and must have both getter and setter.
/// 
/// The table structure is cached for performance, making subsequent instantiations of the 
/// same table type efficient.
/// </remarks>
public abstract class SqlTable : ITuple
{
    internal ImmutableArray<SqlExpr> Columns = ImmutableArray<SqlExpr>.Empty;
    
    /// <summary>
    /// Gets the name of the SQL table.
    /// </summary>
    public string TableName { get; private set; }

    /// <summary>
    /// Gets the column at the specified index for ITuple implementation.
    /// </summary>
    /// <param name="index">The zero-based index of the column</param>
    /// <returns>The column expression at the specified index</returns>
    public object? this[int index] => Columns[index];

    /// <summary>
    /// Gets the number of columns in this table for ITuple implementation.
    /// </summary>
    public int Length => Columns.Length;

    /// <summary>
    /// Initializes a new instance of the SqlTable class with the specified table name.
    /// </summary>
    /// <param name="tableName">The name of the SQL table</param>
    protected SqlTable(string tableName)
    {
        TableName = tableName;
        PopulateColumns(); // Eager population
    }

    private void PopulateColumns()
    {
        // Use cached reflection results
        var properties = GetTableProperties(this.GetType());
                
        foreach (var prop in properties)
        {
            if (!prop.CanRead || !prop.CanWrite)
            {
                throw new InvalidOperationException($"Property {prop.Name} must have both get and set accessors.");
            }

            // prop must have a Column attribute or else exception
            if (Attribute.GetCustomAttribute(prop, typeof(ColumnAttribute)) is not ColumnAttribute columnAttr)
            {
                throw new InvalidOperationException($"Property {prop.Name} must have a Column attribute.");
            }
            if (string.IsNullOrEmpty(columnAttr.Name))
            {
                throw new InvalidOperationException($"Column attribute for property {prop.Name} must have a non-empty Name.");
            }

            var returnType = prop.PropertyType;
            SqlExpr column = returnType switch
            {
                _ when typeof(SqlIntColumn) == returnType => new SqlIntColumn(TableName, columnAttr.Name),
                _ when typeof(SqlStringColumn) == returnType => new SqlStringColumn(TableName, columnAttr.Name),
                _ when typeof(SqlBoolColumn) == returnType => new SqlBoolColumn(TableName, columnAttr.Name),
                _ when typeof(SqlDecimalColumn) == returnType => new SqlDecimalColumn(TableName, columnAttr.Name),
                _ when typeof(SqlDateTimeColumn) == returnType => new SqlDateTimeColumn(TableName, columnAttr.Name),
                _ when typeof(SqlGuidColumn) == returnType => new SqlGuidColumn(TableName, columnAttr.Name),
                _ => throw new NotSupportedException($"Property type {returnType} is not supported as a column type.")
            };

            prop.SetValue(this, column);
        }

        // Populate the Columns array for ITuple implementation
        var columnList = ImmutableArray.CreateBuilder<SqlExpr>();
        foreach (var prop in properties)
        {
            if (prop.GetValue(this) is SqlExpr column)
            {
                columnList.Add(column);
            }
        }
        Columns = columnList.ToImmutable();
    }

    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _tablePropertiesCache = new();

    private static PropertyInfo[] GetTableProperties(Type tableType)
    {
        return _tablePropertiesCache.GetOrAdd(tableType, type =>
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => Attribute.IsDefined(p, typeof(ColumnAttribute)))
                .ToArray()
        );
    }
}
