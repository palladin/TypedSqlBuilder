using TypedSqlBuilder.Core;

namespace TypedSqlBuilder.Tests;

public class SqlExpressionOperatorTests
{
    // Helper method for deep tree value equality using pattern matching
    private static bool AreEqual(SqlExpr expected, SqlExpr actual) => (expected, actual) switch
    {
        // Value comparisons
        (SqlIntValue(var eVal), SqlIntValue(var aVal)) => eVal == aVal,
        (SqlBoolValue(var eVal), SqlBoolValue(var aVal)) => eVal == aVal,
        (SqlStringValue(var eVal), SqlStringValue(var aVal)) => eVal == aVal,
        
        // Column comparisons
        (SqlIntColumn(var eSource, var eName), SqlIntColumn(var aSource, var aName)) => 
            eSource == aSource && eName == aName,
        
        // Binary operations - all follow same pattern
        (SqlIntEquals(var eL, var eR), SqlIntEquals(var aL, var aR)) => 
            AreEqual(eL, aL) && AreEqual(eR, aR),
        (SqlIntNotEquals(var eL, var eR), SqlIntNotEquals(var aL, var aR)) => 
            AreEqual(eL, aL) && AreEqual(eR, aR),
        (SqlIntGreaterThan(var eL, var eR), SqlIntGreaterThan(var aL, var aR)) => 
            AreEqual(eL, aL) && AreEqual(eR, aR),
        (SqlIntLessThan(var eL, var eR), SqlIntLessThan(var aL, var aR)) => 
            AreEqual(eL, aL) && AreEqual(eR, aR),
        (SqlIntGreaterThanOrEqualTo(var eL, var eR), SqlIntGreaterThanOrEqualTo(var aL, var aR)) => 
            AreEqual(eL, aL) && AreEqual(eR, aR),
        (SqlIntLessThanOrEqualTo(var eL, var eR), SqlIntLessThanOrEqualTo(var aL, var aR)) => 
            AreEqual(eL, aL) && AreEqual(eR, aR),
        (SqlIntAdd(var eL, var eR), SqlIntAdd(var aL, var aR)) => 
            AreEqual(eL, aL) && AreEqual(eR, aR),
        (SqlIntSub(var eL, var eR), SqlIntSub(var aL, var aR)) => 
            AreEqual(eL, aL) && AreEqual(eR, aR),
        (SqlIntMult(var eL, var eR), SqlIntMult(var aL, var aR)) => 
            AreEqual(eL, aL) && AreEqual(eR, aR),
        (SqlIntDiv(var eL, var eR), SqlIntDiv(var aL, var aR)) => 
            AreEqual(eL, aL) && AreEqual(eR, aR),
        (SqlBoolAnd(var eL, var eR), SqlBoolAnd(var aL, var aR)) => 
            AreEqual(eL, aL) && AreEqual(eR, aR),
        (SqlBoolOr(var eL, var eR), SqlBoolOr(var aL, var aR)) => 
            AreEqual(eL, aL) && AreEqual(eR, aR),
        
        // Unary operations
        (SqlBoolNot(var eVal), SqlBoolNot(var aVal)) => AreEqual(eVal, aVal),
        
        // Different types or unhandled cases
        _ => false
    };

    [Fact]
    public void DemonstrateUserFriendlyOperators()
    {
        // Create some SQL expressions
        SqlExprInt age = new SqlIntColumn("users", "age");
        SqlExprInt salary = new SqlIntColumn("users", "salary");
        
        // Now you can use natural C# operators for SQL expressions!
        
        // Equality comparisons - these now create SQL expression trees
        SqlExprBool isAdult = age == 18;           // Creates SqlIntEquals
        SqlExprBool isNotZero = age != 0;          // Creates SqlIntNotEquals
        
        // Arithmetic operations
        SqlExprInt totalCompensation = salary + 5000;  // Creates SqlIntAdd
        SqlExprInt afterTax = salary - 1000;           // Creates SqlIntSub
        
        // Complex expressions with multiple operators
        SqlExprBool isWellPaid = salary > 50000 && age >= 25;  // Creates complex boolean expression
        SqlExprBool isEligible = (age >= 18) && (salary != 0); // Parentheses work naturally
        
        // Verify the types are correct
        Assert.IsType<SqlIntEquals>(isAdult);
        Assert.IsType<SqlIntNotEquals>(isNotZero);
        Assert.IsType<SqlIntAdd>(totalCompensation);
        Assert.IsType<SqlIntSub>(afterTax);
        Assert.IsType<SqlBoolAnd>(isWellPaid);
        Assert.IsType<SqlBoolAnd>(isEligible);
    }
    
    [Fact]
    public void VerifyComplexExpressionTree()
    {
        // Create columns
        SqlExprInt id = new SqlIntColumn("table", "id");
        SqlExprInt count = new SqlIntColumn("table", "count");
        
        // Create expression: (id == 1) || (count > 10 && count != 100)
        SqlExprBool actual = (id == 1) || (count > 10 && count != 100);
        
        // Build expected tree manually
        var expected = new SqlBoolOr(
            new SqlIntEquals(id, new SqlIntValue(1)),
            new SqlBoolAnd(
                new SqlIntGreaterThan(count, new SqlIntValue(10)),
                new SqlIntNotEquals(count, new SqlIntValue(100))
            )
        );
        
        // Verify deep equality
        Assert.True(AreEqual(expected, actual));
    }
    
    [Fact]
    public void VerifyArithmeticExpressionTree()
    {
        // Create columns
        SqlExprInt salary = new SqlIntColumn("employees", "salary");
        SqlExprInt bonus = new SqlIntColumn("employees", "bonus");
        
        // Create expression: (salary + bonus) * 2 - 1000
        SqlExprInt actual = (salary + bonus) * 2 - 1000;
        
        // Build expected tree manually
        var expected = new SqlIntSub(
            new SqlIntMult(
                new SqlIntAdd(salary, bonus),
                new SqlIntValue(2)
            ),
            new SqlIntValue(1000)
        );
        
        // Verify deep equality
        Assert.True(AreEqual(expected, actual));
    }
    
    [Fact]
    public void VerifyNestedBooleanExpressionTree()
    {
        // Create columns
        SqlExprInt id = new SqlIntColumn("records", "id");
        SqlExprInt status = new SqlIntColumn("records", "status");
        SqlExprInt priority = new SqlIntColumn("records", "priority");
        
        // Create expression: (id == 1 || status == 2) && priority > 5
        SqlExprBool actual = (id == 1 || status == 2) && priority > 5;
        
        // Build expected tree manually
        var expected = new SqlBoolAnd(
            new SqlBoolOr(
                new SqlIntEquals(id, new SqlIntValue(1)),
                new SqlIntEquals(status, new SqlIntValue(2))
            ),
            new SqlIntGreaterThan(priority, new SqlIntValue(5))
        );
        
        // Verify deep equality
        Assert.True(AreEqual(expected, actual));
    }
    
    [Fact]
    public void VerifyNaturalCSharpSyntaxWithDeepEquality()
    {
        // Create columns
        SqlExprInt id = new SqlIntColumn("data", "id");
        SqlExprInt count = new SqlIntColumn("data", "count");
        
        // Create expression using natural C# syntax
        SqlExprBool actual = (id == 1) || (count > 10 && count != 100);

        // Build expected tree manually
        var expected = new SqlBoolOr(
            new SqlIntEquals(id, new SqlIntValue(1)),
            new SqlBoolAnd(
                new SqlIntGreaterThan(count, new SqlIntValue(10)),
                new SqlIntNotEquals(count, new SqlIntValue(100))
            )
        );

        // Verify deep equality with one line!
        Assert.True(AreEqual(expected, actual));
    }
}
