using TypedSqlBuilder.Core;

namespace TypedSqlBuilder.Tests;

public class OperatorOverloadingTests
{
    [Fact]
    public void TestEquality_Operators()
    {
        // Create some integer expressions
        SqlExprInt a = 5;
        SqlExprInt b = 10;
        SqlExprInt c = new SqlIntColumn("table", "id");
        
        // Test the == operator (should create SqlIntEquals)
        SqlExprBool equals = a == b;
        Assert.IsType<SqlIntEquals>(equals);
        
        // Test the != operator (should create SqlIntNotEquals)
        SqlExprBool notEquals = a != c;
        Assert.IsType<SqlIntNotEquals>(notEquals);
        
        // Test with complex expressions
        SqlExprBool complexEquals = (a + b) == c;
        Assert.IsType<SqlIntEquals>(complexEquals);
        
        SqlExprBool complexNotEquals = (a * 2) != (b - 1);
        Assert.IsType<SqlIntNotEquals>(complexNotEquals);
    }

    [Fact]
    public void TestOther_Operators()
    {
        SqlExprInt a = 5;
        SqlExprInt b = 10;
        
        // Test that other operators still work
        SqlExprBool gt = a > b;
        SqlExprBool lt = a < b;
        SqlExprBool gte = a >= b;
        SqlExprBool lte = a <= b;
        
        Assert.IsType<SqlIntGreaterThan>(gt);
        Assert.IsType<SqlIntLessThan>(lt);
        Assert.IsType<SqlIntGreaterThanOrEqualTo>(gte);
        Assert.IsType<SqlIntLessThanOrEqualTo>(lte);
    }

    [Fact]
    public void TestArithmetic_Operators()
    {
        SqlExprInt a = 5;
        SqlExprInt b = 10;
        
        // Test arithmetic operators
        SqlExprInt add = a + b;
        SqlExprInt sub = a - b;
        SqlExprInt mult = a * b;
        SqlExprInt div = a / b;
        
        Assert.IsType<SqlIntAdd>(add);
        Assert.IsType<SqlIntSub>(sub);
        Assert.IsType<SqlIntMult>(mult);
        Assert.IsType<SqlIntDiv>(div);
    }
}
