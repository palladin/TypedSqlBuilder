# TypedSqlBuilder

A type-safe SQL expression builder DSL for C# that provides fluent syntax for constructing SQL expressions with compile-time type checking.

## Usage

```csharp
using TypedSqlBuilder.Core;

// Define table schema 
public record Customer() : SqlTable<SqlIntColumn, SqlIntColumn, SqlStringColumn>("customers", new("Id"), new("Age"), new("Name"))
{ 
    public SqlIntColumn Id => Column1;
    public SqlIntColumn Age => Column2;
    public SqlStringColumn Name => Column3;
}

// Build complex queries with fluent syntax
ISqlQuery query =
    SqlQuery.From<Customer>()
            .Where(c => c.Age > 18)
            .OrderBy(c => c.Name)
            .Select(c => (c.Id + 1, c.Name + "!"));
```

## Motivation

TypedSqlBuilder is designed to bridge the gap between raw SQL and complex ORMs. It's intended to be used with simple ORMs like **Dapper**, bringing some of the familiar LINQ experience to SQL construction without the complexity and overhead of Entity Framework.

**Why TypedSqlBuilder?**
- **Simple, transparent translation**: Nearly one-to-one mapping between C# expressions and SQL - what you write is what you get, with no query plan surprises or hidden joins
- **Type safety**: Get LINQ-like compile-time checking and IntelliSense when building SQL queries
- **Perfect for micro-ORMs**: Ideal companion for Dapper and other lightweight ORMs
- **Full control**: Maintain complete control over your SQL while eliminating string concatenation errors

## Design

TypedSqlBuilder takes a unique approach to SQL generation that avoids the complexity of IQueryable expression trees or source generators. Instead, it leverages C#'s operator overloading capabilities to build a minimal, composable SQL expression system.

**Key Design Principles:**

- **No Expression Tree Analysis**: Unlike LINQ providers that parse and translate complex expression trees, TypedSqlBuilder uses direct operator overloading on custom SQL types
- **Composable SQL Expressions**: Each SQL operation returns a typed expression object that can be further composed, following functional programming principles
- **Compile-time Type Safety**: Types are preserved throughout the expression building process, ensuring SQL operations are valid for their operand types
- **Minimal Runtime Overhead**: No reflection, no runtime code generation - just simple object composition that translates directly to SQL strings

**How It Works:**

Instead of analyzing `Expression<Func<T, bool>>` trees, TypedSqlBuilder evaluates lambdas like `Func<SqlExpr, SqlExpr>` to compose and collect the SQL expression tree in a similar way that staging-capable languages like MetaOCaml compose their expression trees. The library defines SQL-specific types like `SqlIntColumn`, `SqlStringColumn`, etc., each with overloaded operators (`+`, `>`, `==`, etc.) that return appropriate SQL expression objects. This approach provides LINQ-like syntax while maintaining a direct, predictable mapping to SQL.


