# TypedSqlBuilder Testing Plan

## Current Status ✅
- **140 tests passing** - Excellent baseline!
- Good coverage of SQL Server vs SQLite differences
- Basic JOIN operations working correctly
- Core query operations (SELECT, WHERE, ORDER BY, GROUP BY) well tested

## Priority Areas for Additional Testing

### 1. **JOIN Fusion Testing** 🔥 **HIGH PRIORITY**

The new JOIN fusion rule needs comprehensive testing:
```csharp
// New fusion rule: JoinClause(JoinClause(innerOuter, innerJoinData), outerJoinData) 
//                  → JoinClause(innerOuter, [...innerJoinData, ...outerJoinData])
```

**Test Cases Needed:**
- Multiple INNER JOINs chained together
- Mixed JOIN types (INNER + LEFT)
- JOIN fusion with WHERE clauses
- JOIN fusion with ORDER BY
- JOIN fusion with GROUP BY/HAVING

**Example Test Query:**
```csharp
public static ISqlQuery MultipleJoinsFusion()
    => Db.Customers.From()
        .InnerJoin(Db.Orders, c => c.Id, o => o.CustomerId, (c, o) => (c, o))
        .InnerJoin(Db.OrderItems, joined => joined.o.OrderId, oi => oi.OrderId, (joined, oi) => (joined.c, joined.o, oi));
```

### 2. **Edge Cases & Error Handling** 🟡 **MEDIUM PRIORITY**

**Test Cases:**
- Empty result sets behavior
- NULL handling in JOINs
- Type mismatches in JOIN conditions
- Large query optimization limits
- Circular reference detection

### 3. **Performance & Optimization** 🟡 **MEDIUM PRIORITY**

**Test Cases:**
- Query normalization fixpoint behavior
- Deep nesting scenarios (10+ levels)
- Memory usage with large tuple projections
- Compilation performance benchmarks

### 4. **SQL Dialect Parity** 🟢 **LOW PRIORITY**

**Areas to verify:**
- Date/time functions (SQL Server `GETDATE()` vs SQLite `datetime()`)
- Advanced string functions
- Window functions (if supported)
- Common Table Expressions (CTEs)
- Stored procedure calls

### 5. **Integration Testing** 🟢 **LOW PRIORITY**

**Test Cases:**
- End-to-end database round trips
- Real connection string scenarios
- Transaction handling
- Bulk operations

## Immediate Next Steps

1. **Add JOIN fusion tests** - These are critical for the new functionality
2. **Create a JOIN stress test** - Multiple tables with complex projections
3. **Test SQL Server vs SQLite parity** on new JOIN scenarios
4. **Add performance benchmarks** for query compilation

## Test Structure Recommendations

```csharp
// In TestQueries.cs
public static class JoinFusionQueries 
{
    // Multiple sequential joins
    public static ISqlQuery ThreeTableInnerJoin() => ...;
    
    // Mixed join types
    public static ISqlQuery MixedJoinTypes() => ...;
    
    // Join fusion with other clauses
    public static ISqlQuery JoinFusionWithWhere() => ...;
}

// In SqlServerQueryTests.cs & SqliteQueryTests.cs
public class JoinFusionTests 
{
    [Fact] public void ThreeTableInnerJoin_GeneratesCorrectSql() => ...;
    [Fact] public void MixedJoinTypes_GeneratesCorrectSql() => ...;
    // etc.
}
```

## Success Metrics

- [ ] All JOIN fusion scenarios tested
- [ ] SQL Server/SQLite parity maintained
- [ ] No performance regressions
- [ ] Test coverage > 95%
- [ ] All edge cases handled gracefully
