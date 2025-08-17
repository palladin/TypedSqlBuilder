using System;

namespace TypedSqlBuilder.Core;

/// <summary>
/// Abstract base class for strongly-typed SQL tables with two columns.
/// Provides a structured way to define tables with known column types.
/// </summary>
/// <typeparam name="TCol1">The type of the first column</typeparam>
/// <typeparam name="TCol2">The type of the second column</typeparam>
public abstract class SqlTable<TCol1, TCol2> : ISqlTable 
    where TCol1 : ISqlColumn<TCol1>
    where TCol2 : ISqlColumn<TCol2>
{
    private readonly object?[] columns;
    public object?[] Columns => columns;

    protected SqlTable(string tableName)
    {
        TableName = tableName;
        columns = new object?[2];
    }

    public object? this[int index] => columns[index];

    public TCol1 Column1(string columnName)
    {
        if (columns[0] == null)
        {
            columns[0] = TCol1.Create(TableName, columnName);
        }
        return (TCol1)columns[0]!;
    }

    public TCol2 Column2(string columnName) 
    {
        if (columns[1] == null)
        {
            columns[1] = TCol2.Create(TableName, columnName);
        }
        return (TCol2)columns[1]!;
    }
     
    public string TableName { get; }
    public int Length => columns.Length;
}

/// <summary>
/// Abstract base class for strongly-typed SQL tables with three columns.
/// Provides a structured way to define tables with known column types.
/// </summary>
/// <typeparam name="TCol1">The type of the first column</typeparam>
/// <typeparam name="TCol2">The type of the second column</typeparam>
/// <typeparam name="TCol3">The type of the third column</typeparam>
public abstract class SqlTable<TCol1, TCol2, TCol3> : ISqlTable 
    where TCol1 : ISqlColumn<TCol1>
    where TCol2 : ISqlColumn<TCol2>
    where TCol3 : ISqlColumn<TCol3>
{
    private readonly object?[] columns;
    public object?[] Columns => columns;

    protected SqlTable(string tableName)
    {
        TableName = tableName;
        columns = new object?[3];
    }

    public object? this[int index] => columns[index];

    public TCol1 Column1(string columnName)
    {
        if (columns[0] == null)
        {
            columns[0] = TCol1.Create(TableName, columnName);
        }
        return (TCol1)columns[0]!;
    }

    public TCol2 Column2(string columnName) 
    {
        if (columns[1] == null)
        {
            columns[1] = TCol2.Create(TableName, columnName);
        }
        return (TCol2)columns[1]!;
    }

    public TCol3 Column3(string columnName) 
    {
        if (columns[2] == null)
        {
            columns[2] = TCol3.Create(TableName, columnName);
        }
        return (TCol3)columns[2]!;
    }
     
    public string TableName { get; }
    public int Length => columns.Length;
}

/// <summary>
/// Abstract base class for strongly-typed SQL tables with four columns.
/// </summary>
public abstract class SqlTable<TCol1, TCol2, TCol3, TCol4> : ISqlTable 
    where TCol1 : ISqlColumn<TCol1>
    where TCol2 : ISqlColumn<TCol2>
    where TCol3 : ISqlColumn<TCol3>
    where TCol4 : ISqlColumn<TCol4>
{
    private readonly object?[] columns;
    public object?[] Columns => columns;

    protected SqlTable(string tableName)
    {
        TableName = tableName;
        columns = new object?[4];
    }

    public object? this[int index] => columns[index];

    public TCol1 Column1(string columnName)
    {
        if (columns[0] == null)
        {
            columns[0] = TCol1.Create(TableName, columnName);
        }
        return (TCol1)columns[0]!;
    }

    public TCol2 Column2(string columnName) 
    {
        if (columns[1] == null)
        {
            columns[1] = TCol2.Create(TableName, columnName);
        }
        return (TCol2)columns[1]!;
    }

    public TCol3 Column3(string columnName) 
    {
        if (columns[2] == null)
        {
            columns[2] = TCol3.Create(TableName, columnName);
        }
        return (TCol3)columns[2]!;
    }

    public TCol4 Column4(string columnName) 
    {
        if (columns[3] == null)
        {
            columns[3] = TCol4.Create(TableName, columnName);
        }
        return (TCol4)columns[3]!;
    }
     
    public string TableName { get; }
    public int Length => columns.Length;
}

/// <summary>
/// Abstract base class for strongly-typed SQL tables with five columns.
/// </summary>
public abstract class SqlTable<TCol1, TCol2, TCol3, TCol4, TCol5> : ISqlTable 
    where TCol1 : ISqlColumn<TCol1>
    where TCol2 : ISqlColumn<TCol2>
    where TCol3 : ISqlColumn<TCol3>
    where TCol4 : ISqlColumn<TCol4>
    where TCol5 : ISqlColumn<TCol5>
{
    private readonly object?[] columns;
    public object?[] Columns => columns;

    protected SqlTable(string tableName)
    {
        TableName = tableName;
        columns = new object?[5];
    }

    public object? this[int index] => columns[index];

    public TCol1 Column1(string columnName)
    {
        if (columns[0] == null)
        {
            columns[0] = TCol1.Create(TableName, columnName);
        }
        return (TCol1)columns[0]!;
    }

    public TCol2 Column2(string columnName) 
    {
        if (columns[1] == null)
        {
            columns[1] = TCol2.Create(TableName, columnName);
        }
        return (TCol2)columns[1]!;
    }

    public TCol3 Column3(string columnName) 
    {
        if (columns[2] == null)
        {
            columns[2] = TCol3.Create(TableName, columnName);
        }
        return (TCol3)columns[2]!;
    }

    public TCol4 Column4(string columnName) 
    {
        if (columns[3] == null)
        {
            columns[3] = TCol4.Create(TableName, columnName);
        }
        return (TCol4)columns[3]!;
    }

    public TCol5 Column5(string columnName) 
    {
        if (columns[4] == null)
        {
            columns[4] = TCol5.Create(TableName, columnName);
        }
        return (TCol5)columns[4]!;
    }
     
    public string TableName { get; }
    public int Length => columns.Length;
}

/// <summary>
/// Abstract base class for strongly-typed SQL tables with six columns.
/// </summary>
public abstract class SqlTable<TCol1, TCol2, TCol3, TCol4, TCol5, TCol6> : ISqlTable 
    where TCol1 : ISqlColumn<TCol1>
    where TCol2 : ISqlColumn<TCol2>
    where TCol3 : ISqlColumn<TCol3>
    where TCol4 : ISqlColumn<TCol4>
    where TCol5 : ISqlColumn<TCol5>
    where TCol6 : ISqlColumn<TCol6>
{
    private readonly object?[] columns;
    public object?[] Columns => columns;

    protected SqlTable(string tableName)
    {
        TableName = tableName;
        columns = new object?[6];
    }

    public object? this[int index] => columns[index];

    public TCol1 Column1(string columnName)
    {
        if (columns[0] == null)
        {
            columns[0] = TCol1.Create(TableName, columnName);
        }
        return (TCol1)columns[0]!;
    }

    public TCol2 Column2(string columnName) 
    {
        if (columns[1] == null)
        {
            columns[1] = TCol2.Create(TableName, columnName);
        }
        return (TCol2)columns[1]!;
    }

    public TCol3 Column3(string columnName) 
    {
        if (columns[2] == null)
        {
            columns[2] = TCol3.Create(TableName, columnName);
        }
        return (TCol3)columns[2]!;
    }

    public TCol4 Column4(string columnName) 
    {
        if (columns[3] == null)
        {
            columns[3] = TCol4.Create(TableName, columnName);
        }
        return (TCol4)columns[3]!;
    }

    public TCol5 Column5(string columnName) 
    {
        if (columns[4] == null)
        {
            columns[4] = TCol5.Create(TableName, columnName);
        }
        return (TCol5)columns[4]!;
    }

    public TCol6 Column6(string columnName) 
    {
        if (columns[5] == null)
        {
            columns[5] = TCol6.Create(TableName, columnName);
        }
        return (TCol6)columns[5]!;
    }
     
    public string TableName { get; }
    public int Length => columns.Length;
}

/// <summary>
/// Abstract base class for strongly-typed SQL tables with seven columns.
/// </summary>
public abstract class SqlTable<TCol1, TCol2, TCol3, TCol4, TCol5, TCol6, TCol7> : ISqlTable 
    where TCol1 : ISqlColumn<TCol1>
    where TCol2 : ISqlColumn<TCol2>
    where TCol3 : ISqlColumn<TCol3>
    where TCol4 : ISqlColumn<TCol4>
    where TCol5 : ISqlColumn<TCol5>
    where TCol6 : ISqlColumn<TCol6>
    where TCol7 : ISqlColumn<TCol7>
{
    private readonly object?[] columns;
    public object?[] Columns => columns;

    protected SqlTable(string tableName)
    {
        TableName = tableName;
        columns = new object?[7];
    }

    public object? this[int index] => columns[index];

    public TCol1 Column1(string columnName)
    {
        if (columns[0] == null)
        {
            columns[0] = TCol1.Create(TableName, columnName);
        }
        return (TCol1)columns[0]!;
    }

    public TCol2 Column2(string columnName) 
    {
        if (columns[1] == null)
        {
            columns[1] = TCol2.Create(TableName, columnName);
        }
        return (TCol2)columns[1]!;
    }

    public TCol3 Column3(string columnName) 
    {
        if (columns[2] == null)
        {
            columns[2] = TCol3.Create(TableName, columnName);
        }
        return (TCol3)columns[2]!;
    }

    public TCol4 Column4(string columnName) 
    {
        if (columns[3] == null)
        {
            columns[3] = TCol4.Create(TableName, columnName);
        }
        return (TCol4)columns[3]!;
    }

    public TCol5 Column5(string columnName) 
    {
        if (columns[4] == null)
        {
            columns[4] = TCol5.Create(TableName, columnName);
        }
        return (TCol5)columns[4]!;
    }

    public TCol6 Column6(string columnName) 
    {
        if (columns[5] == null)
        {
            columns[5] = TCol6.Create(TableName, columnName);
        }
        return (TCol6)columns[5]!;
    }

    public TCol7 Column7(string columnName) 
    {
        if (columns[6] == null)
        {
            columns[6] = TCol7.Create(TableName, columnName);
        }
        return (TCol7)columns[6]!;
    }
     
    public string TableName { get; }
    public int Length => columns.Length;
}

/// <summary>
/// Abstract base class for strongly-typed SQL tables with eight columns.
/// </summary>
public abstract class SqlTable<TCol1, TCol2, TCol3, TCol4, TCol5, TCol6, TCol7, TCol8> : ISqlTable 
    where TCol1 : ISqlColumn<TCol1>
    where TCol2 : ISqlColumn<TCol2>
    where TCol3 : ISqlColumn<TCol3>
    where TCol4 : ISqlColumn<TCol4>
    where TCol5 : ISqlColumn<TCol5>
    where TCol6 : ISqlColumn<TCol6>
    where TCol7 : ISqlColumn<TCol7>
    where TCol8 : ISqlColumn<TCol8>
{
    private readonly object?[] columns;
    public object?[] Columns => columns;

    protected SqlTable(string tableName)
    {
        TableName = tableName;
        columns = new object?[8];
    }

    public object? this[int index] => columns[index];

    public TCol1 Column1(string columnName)
    {
        if (columns[0] == null)
        {
            columns[0] = TCol1.Create(TableName, columnName);
        }
        return (TCol1)columns[0]!;
    }

    public TCol2 Column2(string columnName) 
    {
        if (columns[1] == null)
        {
            columns[1] = TCol2.Create(TableName, columnName);
        }
        return (TCol2)columns[1]!;
    }

    public TCol3 Column3(string columnName) 
    {
        if (columns[2] == null)
        {
            columns[2] = TCol3.Create(TableName, columnName);
        }
        return (TCol3)columns[2]!;
    }

    public TCol4 Column4(string columnName) 
    {
        if (columns[3] == null)
        {
            columns[3] = TCol4.Create(TableName, columnName);
        }
        return (TCol4)columns[3]!;
    }

    public TCol5 Column5(string columnName) 
    {
        if (columns[4] == null)
        {
            columns[4] = TCol5.Create(TableName, columnName);
        }
        return (TCol5)columns[4]!;
    }

    public TCol6 Column6(string columnName) 
    {
        if (columns[5] == null)
        {
            columns[5] = TCol6.Create(TableName, columnName);
        }
        return (TCol6)columns[5]!;
    }

    public TCol7 Column7(string columnName) 
    {
        if (columns[6] == null)
        {
            columns[6] = TCol7.Create(TableName, columnName);
        }
        return (TCol7)columns[6]!;
    }

    public TCol8 Column8(string columnName) 
    {
        if (columns[7] == null)
        {
            columns[7] = TCol8.Create(TableName, columnName);
        }
        return (TCol8)columns[7]!;
    }
     
    public string TableName { get; }
    public int Length => columns.Length;
}

/// <summary>
/// Abstract base class for strongly-typed SQL tables with nine columns.
/// </summary>
public abstract class SqlTable<TCol1, TCol2, TCol3, TCol4, TCol5, TCol6, TCol7, TCol8, TCol9> : ISqlTable 
    where TCol1 : ISqlColumn<TCol1>
    where TCol2 : ISqlColumn<TCol2>
    where TCol3 : ISqlColumn<TCol3>
    where TCol4 : ISqlColumn<TCol4>
    where TCol5 : ISqlColumn<TCol5>
    where TCol6 : ISqlColumn<TCol6>
    where TCol7 : ISqlColumn<TCol7>
    where TCol8 : ISqlColumn<TCol8>
    where TCol9 : ISqlColumn<TCol9>
{
    private readonly object?[] columns;
    public object?[] Columns => columns;

    protected SqlTable(string tableName)
    {
        TableName = tableName;
        columns = new object?[9];
    }

    public object? this[int index] => columns[index];

    public TCol1 Column1(string columnName)
    {
        if (columns[0] == null)
        {
            columns[0] = TCol1.Create(TableName, columnName);
        }
        return (TCol1)columns[0]!;
    }

    public TCol2 Column2(string columnName) 
    {
        if (columns[1] == null)
        {
            columns[1] = TCol2.Create(TableName, columnName);
        }
        return (TCol2)columns[1]!;
    }

    public TCol3 Column3(string columnName) 
    {
        if (columns[2] == null)
        {
            columns[2] = TCol3.Create(TableName, columnName);
        }
        return (TCol3)columns[2]!;
    }

    public TCol4 Column4(string columnName) 
    {
        if (columns[3] == null)
        {
            columns[3] = TCol4.Create(TableName, columnName);
        }
        return (TCol4)columns[3]!;
    }

    public TCol5 Column5(string columnName) 
    {
        if (columns[4] == null)
        {
            columns[4] = TCol5.Create(TableName, columnName);
        }
        return (TCol5)columns[4]!;
    }

    public TCol6 Column6(string columnName) 
    {
        if (columns[5] == null)
        {
            columns[5] = TCol6.Create(TableName, columnName);
        }
        return (TCol6)columns[5]!;
    }

    public TCol7 Column7(string columnName) 
    {
        if (columns[6] == null)
        {
            columns[6] = TCol7.Create(TableName, columnName);
        }
        return (TCol7)columns[6]!;
    }

    public TCol8 Column8(string columnName) 
    {
        if (columns[7] == null)
        {
            columns[7] = TCol8.Create(TableName, columnName);
        }
        return (TCol8)columns[7]!;
    }

    public TCol9 Column9(string columnName) 
    {
        if (columns[8] == null)
        {
            columns[8] = TCol9.Create(TableName, columnName);
        }
        return (TCol9)columns[8]!;
    }
     
    public string TableName { get; }
    public int Length => columns.Length;
}

/// <summary>
/// Abstract base class for strongly-typed SQL tables with ten columns.
/// </summary>
public abstract class SqlTable<TCol1, TCol2, TCol3, TCol4, TCol5, TCol6, TCol7, TCol8, TCol9, TCol10> : ISqlTable 
    where TCol1 : ISqlColumn<TCol1>
    where TCol2 : ISqlColumn<TCol2>
    where TCol3 : ISqlColumn<TCol3>
    where TCol4 : ISqlColumn<TCol4>
    where TCol5 : ISqlColumn<TCol5>
    where TCol6 : ISqlColumn<TCol6>
    where TCol7 : ISqlColumn<TCol7>
    where TCol8 : ISqlColumn<TCol8>
    where TCol9 : ISqlColumn<TCol9>
    where TCol10 : ISqlColumn<TCol10>
{
    private readonly object?[] columns;
    public object?[] Columns => columns;

    protected SqlTable(string tableName)
    {
        TableName = tableName;
        columns = new object?[10];
    }

    public object? this[int index] => columns[index];

    public TCol1 Column1(string columnName)
    {
        if (columns[0] == null)
        {
            columns[0] = TCol1.Create(TableName, columnName);
        }
        return (TCol1)columns[0]!;
    }

    public TCol2 Column2(string columnName) 
    {
        if (columns[1] == null)
        {
            columns[1] = TCol2.Create(TableName, columnName);
        }
        return (TCol2)columns[1]!;
    }

    public TCol3 Column3(string columnName) 
    {
        if (columns[2] == null)
        {
            columns[2] = TCol3.Create(TableName, columnName);
        }
        return (TCol3)columns[2]!;
    }

    public TCol4 Column4(string columnName) 
    {
        if (columns[3] == null)
        {
            columns[3] = TCol4.Create(TableName, columnName);
        }
        return (TCol4)columns[3]!;
    }

    public TCol5 Column5(string columnName) 
    {
        if (columns[4] == null)
        {
            columns[4] = TCol5.Create(TableName, columnName);
        }
        return (TCol5)columns[4]!;
    }

    public TCol6 Column6(string columnName) 
    {
        if (columns[5] == null)
        {
            columns[5] = TCol6.Create(TableName, columnName);
        }
        return (TCol6)columns[5]!;
    }

    public TCol7 Column7(string columnName) 
    {
        if (columns[6] == null)
        {
            columns[6] = TCol7.Create(TableName, columnName);
        }
        return (TCol7)columns[6]!;
    }

    public TCol8 Column8(string columnName) 
    {
        if (columns[7] == null)
        {
            columns[7] = TCol8.Create(TableName, columnName);
        }
        return (TCol8)columns[7]!;
    }

    public TCol9 Column9(string columnName) 
    {
        if (columns[8] == null)
        {
            columns[8] = TCol9.Create(TableName, columnName);
        }
        return (TCol9)columns[8]!;
    }

    public TCol10 Column10(string columnName) 
    {
        if (columns[9] == null)
        {
            columns[9] = TCol10.Create(TableName, columnName);
        }
        return (TCol10)columns[9]!;
    }
     
    public string TableName { get; }
    public int Length => columns.Length;
}
