# TypedSqlBuilder

A type-safe SQL expression builder DSL for C# that provides fluent syntax for constructing SQL expressions with compile-time type checking.

## Motivation

TypedSqlBuilder is designed to bridge the gap between raw SQL and complex ORMs. It's intended to be used with simple ORMs like **Dapper**, bringing some of the familiar LINQ experience to SQL construction without the complexity and overhead of Entity Framework.

**Why TypedSqlBuilder?**
- **Simple, direct translation**: Nearly one-to-one mapping between C# expressions and SQL - what you write is what you get
- Get LINQ-like type safety and IntelliSense when building SQL queries
- Work directly with SQL without losing compile-time checking
- Perfect companion for Dapper and other micro-ORMs
- Maintain full control over your SQL while eliminating string concatenation errors
- **Transparent SQL generation**: No query plan surprises, no hidden joins, no magic - just predictable SQL output
- No magic, no hidden queries, no performance surprises - just type-safe SQL building

## Features

- **Type Safety**: Compile-time checking ensures SQL expressions are type-safe
- **Fluent Syntax**: Use familiar C# operators (+, -, *, /, ==, !=, >, <, etc.) to build SQL expressions
- **Expression Types**: Support for integer, boolean, and string SQL expressions
- **Operator Overloading**: Natural C# syntax that maps to appropriate SQL constructs
- **Query Builder**: Fluent query building with From, Where, OrderBy, and Select operations
- **SQL Parameters**: Type-safe parameter creation with extension methods
- **SQL Functions**: Built-in support for common SQL functions like COUNT, SUM, AVG, ABS, LIKE

## Usage

### Query Building

```csharp
using TypedSqlBuilder.Core;

// Define table schema as a tuple
var customersTableName = "customers";
var customers = (
    Id: new SqlIntColumn(customersTableName, "id"),
    Name: new SqlStringColumn(customersTableName, "name"), 
    Age: new SqlIntColumn(customersTableName, "age")
);

// Build complex queries with fluent syntax
SqlQuery query =
    SqlQuery.From(customers)
            .Where(c => c.Age > ":age".AsIntParam())
            .OrderBy(c => c.Name)
            .Select(c => (c.Id + 1, c.Name + "!", c.Age > 20 ? "Adult" : "Minor"));
```
