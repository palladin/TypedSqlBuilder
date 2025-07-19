# TypedSqlBuilder

A type-safe SQL expression builder DSL for C# that provides fluent syntax for constructing SQL expressions with compile-time type checking.

## Features

- **Type Safety**: Compile-time checking ensures SQL expressions are type-safe
- **Fluent Syntax**: Use familiar C# operators (+, -, *, /, ==, !=, >, <, etc.) to build SQL expressions
- **Expression Types**: Support for integer, boolean, and string SQL expressions
- **Operator Overloading**: Natural C# syntax that maps to appropriate SQL constructs

## Usage

```csharp
using TypedSqlBuilder.Core;

// Create expressions using natural C# syntax
SqlExprInt age = new SqlIntColumn("users", "age");
SqlExprInt minAge = 18;

// Build complex expressions
SqlExprBool isAdult = age >= minAge;
SqlExprInt bonus = (age - 18) * 100;

// Combine expressions
SqlExprBool complexCondition = (age > 21) & (bonus < 1000);
```
