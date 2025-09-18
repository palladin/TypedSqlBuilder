# Dynamic Query Composition: TypedSqlBuilder vs Expression Trees

## The Scenario

You need to build dynamic queries where multiple search conditions are combined with OR logic. For example:

- Search customers by name OR email OR age range
- Apply filters based on user input (some filters may be empty)
- Compose predicates programmatically at runtime

The challenge: How do you combine multiple `Func<Customer, bool>` predicates into a single query condition?

## Expression Trees (Traditional Approach)

```csharp
// Build list of expression predicates
var predicates = new List<Expression<Func<Customer, bool>>>
{
    c => c.Name.Contains("John"),
    c => c.Email.Contains("gmail"), 
    c => c.Age > 18
};

// Complex composition - manual expression tree manipulation
Expression<Func<Customer, bool>> combined = predicates[0];
foreach (var predicate in predicates.Skip(1))
{    
    var parameter = Expression.Parameter(typeof(Customer), "c");
    var left = Expression.Invoke(combined, parameter);
    var right = Expression.Invoke(predicate, parameter);
    var orExpression = Expression.OrElse(left, right);
    combined = Expression.Lambda<Func<Customer, bool>>(orExpression, parameter);
}

// Use with EF Core
var results = await context.Customers.Where(combined).ToListAsync();
```

## TypedSqlBuilder (Functional Composition)

```csharp
// Build list of function predicates
var predicates = new List<Func<CustomerTable, SqlExprBool>>
{
    c => c.Name.Like("%John%"),
    c => c.Email.Like("%gmail%"), 
    c => c.Age > 18
};

// Simple composition with LINQ Aggregate
var composedCondition = predicates.Aggregate((acc, pred) => x => acc(x) || pred(x));

// Use with TypedSqlBuilder
var query = Db.Customers.From()
    .Where(c => composedCondition(c))
    .Select(c => (c.Id, c.Name, c.Email));

var (sql, parameters) = query.ToSqliteRaw();
var results = await connection.QueryAsync<dynamic>(sql, parameters);
```

**Generated SQL:**
```sql
SELECT "a0"."Id", "a0"."Name", "a0"."Email"
FROM "Customers" "a0"
WHERE "a0"."Name" LIKE :p0 OR "a0"."Email" LIKE :p1 OR "a0"."Age" > :p2
```

**Benefits:**
- One line composition: `predicates.Aggregate((acc, pred) => x => acc(x) || pred(x))`
- No expression tree complexity
- Predictable SQL output
- Functional programming approach

