# TypedSqlBuilder Examples

This document demonstrates various TypedSqlBuilder queries and their corresponding SQL Server output.

## Table Definitions

For these examples, we use the following table structure:

```csharp
public class Customer() : SqlTable("customers")
{
    [Column("Id")] public SqlIntColumn Id { get; set; } = default!;
    [Column("Age")] public SqlIntColumn Age { get; set; } = default!;
    [Column("Name")] public SqlStringColumn Name { get; set; } = default!;
    [Column("IsActive")] public SqlBoolColumn IsActive { get; set; } = default!;
}

public class Product() : SqlTable("products") 
{
    [Column("Id")] public SqlIntColumn Id { get; set; } = default!;
    [Column("ProductName")] public SqlStringColumn ProductName { get; set; } = default!;
    [Column("Price")] public SqlDecimalColumn Price { get; set; } = default!;
    [Column("CreatedDate")] public SqlDateTimeColumn CreatedDate { get; set; } = default!;
    [Column("UniqueId")] public SqlGuidColumn UniqueId { get; set; } = default!;
}

public class Order() : SqlTable("orders")
{
    [Column("Id")] public SqlIntColumn Id { get; set; } = default!;
    [Column("CustomerId")] public SqlIntColumn CustomerId { get; set; } = default!;
    [Column("ProductId")] public SqlIntColumn ProductId { get; set; } = default!;
    [Column("Amount")] public SqlIntColumn Amount { get; set; } = default!;
}
```

## Basic Queries

### Simple SELECT All Columns

**C# Code:**
```csharp
Db.Customers.From()
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Age] AS [Age],
    [a0].[Name] AS [Name],
    [a0].[IsActive] AS [IsActive]
FROM
    [customers] [a0]
```

### SELECT Specific Columns

**C# Code:**
```csharp
Db.Customers.From()
    .Select(c => (c.Id, c.Name))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Name] AS [Name]
FROM
    [customers] [a0]
```

## WHERE Clauses

### Simple WHERE Condition

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Age > 18)
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Age] AS [Age],
    [a0].[Name] AS [Name],
    [a0].[IsActive] AS [IsActive]
FROM
    [customers] [a0]
WHERE
    [a0].[Age] > @p0
```

### Multiple WHERE Conditions (AND)

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Age > 18 && c.Name != "Admin")
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Age] AS [Age],
    [a0].[Name] AS [Name],
    [a0].[IsActive] AS [IsActive]
FROM
    [customers] [a0]
WHERE
    [a0].[Age] > @p0 AND [a0].[Name] <> @p1
```

### Complex WHERE with OR Logic

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => (c.Age > 18 && c.Age < 65) ||
                c.Name == "VIP")
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Age] AS [Age],
    [a0].[Name] AS [Name],
    [a0].[IsActive] AS [IsActive]
FROM
    [customers] [a0]
WHERE
    ([a0].[Age] > @p0 AND [a0].[Age] < @p1) OR
    [a0].[Name] = @p2
```

## ORDER BY Clauses

### Simple ORDER BY

**C# Code:**
```csharp
Db.Customers.From()
    .OrderBy(c => (c.Name, Sort.Asc))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Age] AS [Age],
    [a0].[Name] AS [Name],
    [a0].[IsActive] AS [IsActive]
FROM
    [customers] [a0]
ORDER BY
    [a0].[Name] ASC
```

### Multiple ORDER BY Columns

**C# Code:**
```csharp
Db.Customers.From()
    .OrderBy(c => ((c.Name, Sort.Asc),
                   (c.Age, Sort.Desc)))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Age] AS [Age],
    [a0].[Name] AS [Name],
    [a0].[IsActive] AS [IsActive]
FROM
    [customers] [a0]
ORDER BY
    [a0].[Name] ASC, [a0].[Age] DESC
```

## Complex Queries with Multiple Clauses

### WHERE + ORDER BY + SELECT

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Age > 18)
    .OrderBy(c => (c.Name, Sort.Asc))
    .Select(c => (c.Id + 1, c.Name + "!"))
```

**Generated SQL:**
```sql
SELECT
    ([a0].[Id] + @p0) AS [Proj0],
    ([a0].[Name] + @p1) AS [Proj1]
FROM
    [customers] [a0]
WHERE
    [a0].[Age] > @p2
ORDER BY
    [a0].[Name] ASC
```

## Named Projections

### Named Tuple Projections

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Age >= 21)
    .Select(c => (
        CustomerId: c.Id,
        CustomerInfo: c.Name + " (Customer)",
        AdjustedAge: c.Age + 5
    ))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [CustomerId],
    ([a0].[Name] + @p0) AS [CustomerInfo],
    ([a0].[Age] + @p1) AS [AdjustedAge]
FROM
    [customers] [a0]
WHERE
    [a0].[Age] >= @p2
```

## INNER JOIN

### Basic INNER JOIN

**C# Code:**
```csharp
Db.Customers.From()
    .InnerJoin(Db.Orders, 
               c => c.Id, 
               o => o.CustomerId,
               (c, o) => (c.Id, c.Name, o.Id, o.Amount))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [CustomerId],
    [a0].[Name] AS [Name],
    [a1].[Id] AS [OrderId],
    [a1].[Amount] AS [Amount]
FROM
    [customers] [a0]
INNER JOIN [orders] [a1] ON [a0].[Id] = [a1].[CustomerId]
```

### INNER JOIN with Named Projections

**C# Code:**
```csharp
Db.Customers.From()
    .InnerJoin(Db.Orders,
               c => c.Id,
               o => o.CustomerId,
               (c, o) => (
                   c.Name,
                   o.Amount
               ))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Name] AS [Name],
    [a1].[Amount] AS [Amount]
FROM
    [customers] [a0]
INNER JOIN [orders] [a1] ON [a0].[Id] = [a1].[CustomerId]
```

## INNER JOIN with WHERE

### Complex JOIN with Multiple WHERE Conditions

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Age >= 18)
    .InnerJoin(Db.Orders,
               c => c.Id,
               o => o.CustomerId,
               (c, o) => (c.Id, c.Name, c.Age,
                         o.Id, o.Amount))
    .Where(result => result.Amount > 100)
```

**Generated SQL:**
```sql
SELECT
    [a1].[Id] AS [CustomerId],
    [a1].[Name] AS [Name],
    [a1].[Age] AS [Age],
    [a2].[Id] AS [OrderId],
    [a2].[Amount] AS [Amount]
FROM
    (SELECT
        [a0].[Id] AS [Id],
        [a0].[Age] AS [Age],
        [a0].[Name] AS [Name],
        [a0].[IsActive] AS [IsActive]
    FROM
        [customers] [a0]
    WHERE
        [a0].[Age] >= @p0) [a1]
INNER JOIN [orders] [a2] ON [a1].[Id] = [a2].[CustomerId]
WHERE
    [a2].[Amount] > @p1
```

## LEFT JOIN

### Basic LEFT JOIN

**C# Code:**
```csharp
Db.Customers.From()
    .LeftJoin(Db.Orders,
              c => c.Id,
              o => o.CustomerId,
              (c, o) => (c.Id, c.Name, o.Id, o.Amount))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [CustomerId],
    [a0].[Name] AS [Name],
    [a1].[Id] AS [OrderId],
    [a1].[Amount] AS [Amount]
FROM
    [customers] [a0]
LEFT JOIN [orders] [a1] ON [a0].[Id] = [a1].[CustomerId]
```

## GROUP BY with Aggregates

### Simple GROUP BY with COUNT

**C# Code:**
```csharp
Db.Customers.From()
    .GroupBy(c => c.Age)
    .Select((c, agg) => (
        Age: c.Age,
        Count: agg.Count()
    ))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Age] AS [Age],
    COUNT(*) AS [Count]
FROM
    [customers] [a0]
GROUP BY
    [a0].[Age]
```

### Complex GROUP BY with JOIN and SUM

**C# Code:**
```csharp
Db.Customers.From()
    .InnerJoin(Db.Orders,
               c => c.Id,
               o => o.CustomerId,
               (c, o) => (c.Id, c.Name, o.Amount))
    .GroupBy(result => (result.Id, result.Name))
    .Select((result, agg) => (
        CustomerId: result.Id,
        CustomerName: result.Name,
        TotalAmount: agg.Sum(result.Amount)
    ))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [CustomerId],
    [a0].[Name] AS [CustomerName],
    SUM([a1].[Amount]) AS [TotalAmount]
FROM
    [customers] [a0]
INNER JOIN [orders] [a1] ON [a0].[Id] = [a1].[CustomerId]
GROUP BY
    [a0].[Id], [a0].[Name]
```

## GROUP BY with HAVING

### GROUP BY with HAVING Clause

**C# Code:**
```csharp
Db.Customers.From()
    .GroupBy(c => c.Age)
    .Having((c, agg) => agg.Count() > 1)
    .Select((c, agg) => (
        Age: c.Age,
        Count: agg.Count()
    ))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Age] AS [Age],
    COUNT(*) AS [Count]
FROM
    [customers] [a0]
GROUP BY
    [a0].[Age]
HAVING
    COUNT(*) > @p0
```

## Scalar Queries (Aggregates)

### Simple COUNT

**C# Code:**
```csharp
Db.Customers.From().Count()
```

**Generated SQL:**
```sql
SELECT
    COUNT(*) AS [Proj0]
FROM
    [customers] [a0]
```

### COUNT with WHERE

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Age >= 18)
    .Count()
```

**Generated SQL:**
```sql
SELECT
    COUNT(*) AS [Proj0]
FROM
    [customers] [a0]
WHERE
    [a0].[Age] >= @p0
```

### SUM

**C# Code:**
```csharp
Db.Products.From()
    .Select(p => p.Price)
    .Sum()
```

**Generated SQL:**
```sql
SELECT
    SUM([a0].[Price]) AS [Proj0]
FROM
    [products] [a0]
```

### AVG

**C# Code:**
```csharp
Db.Products.From()
    .Select(p => p.Price)
    .Avg()
```

**Generated SQL:**
```sql
SELECT
    AVG([a0].[Price]) AS [Proj0]
FROM
    [products] [a0]
```

### MIN

**C# Code:**
```csharp
Db.Products.From()
    .Select(p => p.Price)
    .Min()
```

**Generated SQL:**
```sql
SELECT
    MIN([a0].[Price]) AS [Proj0]
FROM
    [products] [a0]
```

### MAX

**C# Code:**
```csharp
Db.Products.From()
    .Select(p => p.Price)
    .Max()
```

**Generated SQL:**
```sql
SELECT
    MAX([a0].[Price]) AS [Proj0]
FROM
    [products] [a0]
```

### AVG with WHERE

**C# Code:**
```csharp
Db.Products.From()
    .Where(p => p.Price > 100m)
    .Select(p => p.Price)
    .Avg()
```

**Generated SQL:**
```sql
SELECT
    AVG([a0].[Price]) AS [Proj0]
FROM
    [products] [a0]
WHERE
    [a0].[Price] > @p0
```

## Built-in Functions

### Mathematical Functions

#### ABS Function

**C# Code:**
```csharp
Db.Customers.From()
    .Select(c => (c.Id, c.Age.Abs()))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    ABS([a0].[Age]) AS [Proj0]
FROM
    [customers] [a0]
```

#### ABS with Expression

**C# Code:**
```csharp
Db.Customers.From()
    .Select(c => (c.Id, (c.Age - 50).Abs()))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    ABS([a0].[Age] - @p0) AS [Proj0]
FROM
    [customers] [a0]
```

#### ABS in WHERE Clause

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Age.Abs() > 30)
    .Select(c => (c.Id, c.Name, c.Age))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Name] AS [Name],
    [a0].[Age] AS [Age]
FROM
    [customers] [a0]
WHERE
    ABS([a0].[Age]) > @p0
```

### String Functions (LIKE)

#### LIKE with Wildcard

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Name.Like("Jo%"))
    .Select(c => (c.Id, c.Name))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Name] AS [Name]
FROM
    [customers] [a0]
WHERE
    [a0].[Name] LIKE @p0
```

#### LIKE with Single Character Wildcard

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Name.Like("J_n"))
    .Select(c => (c.Id, c.Name))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Name] AS [Name]
FROM
    [customers] [a0]
WHERE
    [a0].[Name] LIKE @p0
```

#### LIKE with Multiple Wildcards

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Name.Like("%o_n%"))
    .Select(c => (c.Id, c.Name))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Name] AS [Name]
FROM
    [customers] [a0]
WHERE
    [a0].[Name] LIKE @p0
```

### Date Functions

#### AddDays

**C# Code:**
```csharp
Db.Products.From()
    .Select(p => p.CreatedDate.AddDays(30))
```

**Generated SQL:**
```sql
SELECT
    DATEADD(day, @p0, [a0].[CreatedDate]) AS [Proj0]
FROM
    [products] [a0]
```

#### AddMonths

**C# Code:**
```csharp
Db.Products.From()
    .Select(p => p.CreatedDate.AddMonths(6))
```

**Generated SQL:**
```sql
SELECT
    DATEADD(month, @p0, [a0].[CreatedDate]) AS [Proj0]
FROM
    [products] [a0]
```

#### AddYears

**C# Code:**
```csharp
Db.Products.From()
    .Select(p => p.CreatedDate.AddYears(1))
```

**Generated SQL:**
```sql
SELECT
    DATEADD(year, @p0, [a0].[CreatedDate]) AS [Proj0]
FROM
    [products] [a0]
```

#### Day Function

**C# Code:**
```csharp
Db.Products.From()
    .Select(p => p.CreatedDate.Day())
```

**Generated SQL:**
```sql
SELECT
    DAY([a0].[CreatedDate]) AS [Proj0]
FROM
    [products] [a0]
```

## NULL Checks

### IS NULL

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Name == SqlNull.Value)
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Age] AS [Age],
    [a0].[Name] AS [Name],
    [a0].[IsActive] AS [IsActive]
FROM
    [customers] [a0]
WHERE
    [a0].[Name] IS NULL
```

### IS NOT NULL

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Name != SqlNull.Value)
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Age] AS [Age],
    [a0].[Name] AS [Name],
    [a0].[IsActive] AS [IsActive]
FROM
    [customers] [a0]
WHERE
    [a0].[Name] IS NOT NULL
```

## IN Clauses

### IN with Literal Values

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Age.In(18, 21, 25, 30))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Age] AS [Age],
    [a0].[Name] AS [Name],
    [a0].[IsActive] AS [IsActive]
FROM
    [customers] [a0]
WHERE
    [a0].[Age] IN (@p0, @p1, @p2, @p3)
```

## Subqueries

### IN with Subquery

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Age.In(
        Db.Customers.From()
            .Where(x => x.Name == "VIP")
            .Select(x => x.Age)
    ))
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Age] AS [Age],
    [a0].[Name] AS [Name],
    [a0].[IsActive] AS [IsActive]
FROM
    [customers] [a0]
WHERE
    [a0].[Age] IN (
        SELECT
            [a1].[Age] AS [Age]
        FROM
            [customers] [a1]
        WHERE
            [a1].[Name] = @p0)
```

## Parameters

TypedSqlBuilder automatically parameterizes literal values to prevent SQL injection and improve query plan reuse. You can also explicitly define parameters:

### Named Parameters

**C# Code:**
```csharp
Db.Products.From()
    .Where(p => p.Price > "minPrice".AsDecimalParam())
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[ProductName] AS [ProductName],
    [a0].[Price] AS [Price],
    [a0].[CreatedDate] AS [CreatedDate],
    [a0].[UniqueId] AS [UniqueId]
FROM
    [products] [a0]
WHERE
    [a0].[Price] > @minPrice
```

### Named Parameters with Functions

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Age.Abs() > 
                "minAge".AsIntParam().Abs())
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Name] AS [Name],
    [a0].[Age] AS [Age]
FROM
    [customers] [a0]
WHERE
    ABS([a0].[Age]) > ABS(@minAge)
```

## DISTINCT Operations

### DISTINCT Example

**C# Code:**
```csharp
Db.Customers.From()
    .Select(c => c.Age)
    .Distinct()
```

**Generated SQL:**
```sql
SELECT DISTINCT
    [a0].[Age] AS [Age]
FROM
    [customers] [a0]
```

## LIMIT Operations

### LIMIT Example (Limit Only)

**C# Code:**
```csharp
Db.Customers.From()
    .Limit(5)
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Age] AS [Age],
    [a0].[Name] AS [Name],
    [a0].[IsActive] AS [IsActive]
FROM
    [customers] [a0]
ORDER BY
    [a0].[Id] ASC
OFFSET 0 ROWS
FETCH NEXT 5 ROWS ONLY
```

### LIMIT with OFFSET Example (Pagination)

**C# Code:**
```csharp
Db.Customers.From()
    .OrderBy(c => (c.Id, Sort.Asc))
    .Limit(10, 20)
```

**Generated SQL:**
```sql
SELECT
    [a0].[Id] AS [Id],
    [a0].[Age] AS [Age],
    [a0].[Name] AS [Name],
    [a0].[IsActive] AS [IsActive]
FROM
    [customers] [a0]
ORDER BY
    [a0].[Id] ASC
OFFSET 20 ROWS
FETCH NEXT 10 ROWS ONLY
```

## Combining DISTINCT and LIMIT

### DISTINCT with LIMIT Example

**C# Code:**
```csharp
Db.Customers.From()
    .Select(c => c.Age)
    .Distinct()
    .Limit(10)
```

**Generated SQL:**
```sql
SELECT DISTINCT
    [a0].[Age] AS [Age]
FROM
    [customers] [a0]
ORDER BY
    [a0].[Age] ASC
OFFSET 0 ROWS
FETCH NEXT 10 ROWS ONLY
```

## Set Operations

TypedSqlBuilder supports SQL set operations to combine results from multiple queries.

### UNION

UNION combines the result sets of two SELECT statements, removing duplicates.

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Age > 30)
    .Select(c => (c.Id, c.Name))
    .Union(
        Db.Customers.From()
            .Where(c => c.Name == "Alice")
            .Select(c => (c.Id, c.Name))
    )
```

**Generated SQL:**
```sql
SELECT 
    [a0].[Id] AS [Id],
    [a0].[Name] AS [Name]
FROM 
    [customers] [a0]
WHERE 
    [a0].[Age] > @p0
UNION
SELECT 
    [a1].[Id] AS [Id],
    [a1].[Name] AS [Name]
FROM 
    [customers] [a1]
WHERE 
    [a1].[Name] = @p1
```

### INTERSECT

INTERSECT returns the common records from two SELECT statements.

**C# Code:**
```csharp
Db.Customers.From()
    .Where(c => c.Age > 25)
    .Select(c => (c.Id, c.Name))
    .Intersect(
        Db.Customers.From()
            .Where(c => c.Name == "John")
            .Select(c => (c.Id, c.Name))
    )
```

**Generated SQL:**
```sql
SELECT 
    [a0].[Id] AS [Id],
    [a0].[Name] AS [Name]
FROM 
    [customers] [a0]
WHERE 
    [a0].[Age] > @p0
INTERSECT
SELECT 
    [a1].[Id] AS [Id],
    [a1].[Name] AS [Name]
FROM 
    [customers] [a1]
WHERE 
    [a1].[Name] = @p1
```

### EXCEPT

EXCEPT returns records from the first SELECT statement that are not found in the second.

**C# Code:**
```csharp
Db.Customers.From()
    .Select(c => (c.Id, c.Name))
    .Except(
        Db.Customers.From()
            .Where(c => c.Age < 18)
            .Select(c => (c.Id, c.Name))
    )
```

**Generated SQL:**
```sql
SELECT 
    [a0].[Id] AS [Id],
    [a0].[Name] AS [Name]
FROM 
    [customers] [a0]
EXCEPT
SELECT 
    [a1].[Id] AS [Id],
    [a1].[Name] AS [Name]
FROM 
    [customers] [a1]
WHERE 
    [a1].[Age] < @p0
```

