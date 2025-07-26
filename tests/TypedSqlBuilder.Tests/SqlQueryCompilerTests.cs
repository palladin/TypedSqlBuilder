using System.Diagnostics;
using TypedSqlBuilder.Core;

namespace TypedSqlBuilder.Tests;

/// <summary>
/// Comprehensive tests for SQL query compilation to SQL strings.
/// These tests verify that the typed SQL builder generates correct SQL syntax.
/// </summary>
public static class SqlQueryCompilerTests
{
    /// <summary>
    /// Sample table definition for testing
    /// </summary>
    public record Customer() : SqlTable<SqlIntColumn, SqlIntColumn, SqlStringColumn>("customers", new("customers", "Id"), new("customers", "Age"), new("customers", "Name"))
    { 
        public SqlIntColumn Id => Column1;
        public SqlIntColumn Age => Column2;
        public SqlStringColumn Name => Column3;
    }

    /// <summary>
    /// Sample table with different column types for comprehensive testing
    /// </summary>
    public record Product() : SqlTable<SqlIntColumn, SqlStringColumn>("products", new("products", "ProductId"), new("products", "ProductName"))
    {
        public SqlIntColumn ProductId => Column1;
        public SqlStringColumn ProductName => Column2;
    }

    /// <summary>
    /// Test helper that compiles a query and compares it to expected SQL
    /// </summary>
    private static void AssertSql(ISqlQuery query, string expectedSql, string testName)
    {
        try
        {
            var actualSql = SqlQueryCompiler.Compile(query);
            
            if (actualSql == expectedSql)
            {
                Console.WriteLine($"✅ {testName}: PASSED");
                Console.WriteLine($"   Generated: {actualSql}");
            }
            else
            {
                Console.WriteLine($"❌ {testName}: FAILED");
                Console.WriteLine($"   Expected: {expectedSql}");
                Console.WriteLine($"   Actual:   {actualSql}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"💥 {testName}: EXCEPTION - {ex.Message}");
        }
        
        Console.WriteLine();
    }

    /// <summary>
    /// Test helper for SQL expressions
    /// </summary>
    private static void AssertSqlExpr(SqlExpr expr, string expectedSql, string testName)
    {
        try
        {
            var actualSql = SqlQueryCompiler.Compile(expr);
            
            if (actualSql == expectedSql)
            {
                Console.WriteLine($"✅ {testName}: PASSED");
                Console.WriteLine($"   Generated: {actualSql}");
            }
            else
            {
                Console.WriteLine($"❌ {testName}: FAILED");
                Console.WriteLine($"   Expected: {expectedSql}");
                Console.WriteLine($"   Actual:   {actualSql}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"💥 {testName}: EXCEPTION - {ex.Message}");
        }
        
        Console.WriteLine();
    }

    /// <summary>
    /// Runs all SQL compilation tests
    /// </summary>
    public static void RunAllTests()
    {
        Console.WriteLine("Running SQL Query Compiler Tests");
        Console.WriteLine("=================================");
        Console.WriteLine();

        TestBasicFromClause();
        TestSimpleSelectClause();
        TestWhereClause();
        TestOrderByClause();
        TestComplexQueries();
        TestAggregates();
        TestExpressions();
        TestStringExpressions();
        TestBooleanExpressions();
        TestArithmeticExpressions();
        
        Console.WriteLine("SQL Query Compiler Tests Complete");
        Console.WriteLine("=================================");
    }

    /// <summary>
    /// Test basic FROM clause generation
    /// </summary>
    private static void TestBasicFromClause()
    {
        Console.WriteLine("--- Basic FROM Clause Tests ---");

        // Simple FROM clause
        var fromQuery = SqlQuery.From<Customer>();
        AssertSql(fromQuery, "SELECT * FROM customers", "Basic FROM clause");
    }

    /// <summary>
    /// Test SELECT clause generation
    /// </summary>
    private static void TestSimpleSelectClause()
    {
        Console.WriteLine("--- SELECT Clause Tests ---");

        // SELECT with column projection
        var selectQuery = SqlQuery.From<Customer>()
            .Select(c => (c.Id, c.Name));
        AssertSql(selectQuery, "SELECT customers.Id, customers.Name FROM customers", "SELECT with column projection");

        // SELECT single column
        var selectSingleQuery = SqlQuery.From<Customer>()
            .Select(c => c.Age);
        AssertSql(selectSingleQuery, "SELECT customers.Age FROM customers", "SELECT single column");
    }

    /// <summary>
    /// Test WHERE clause generation
    /// </summary>
    private static void TestWhereClause()
    {
        Console.WriteLine("--- WHERE Clause Tests ---");

        // Simple WHERE clause with integer comparison
        var whereQuery = SqlQuery.From<Customer>()
            .Where(c => c.Age > 18);
        AssertSql(whereQuery, "SELECT * FROM customers WHERE customers.Age > 18", "WHERE with integer comparison");

        // WHERE clause with string equality
        var whereStringQuery = SqlQuery.From<Customer>()
            .Where(c => c.Name == "John");
        AssertSql(whereStringQuery, "SELECT * FROM customers WHERE customers.Name = 'John'", "WHERE with string equality");

        // WHERE clause with multiple conditions
        var whereMultipleQuery = SqlQuery.From<Customer>()
            .Where(c => c.Age > 18 & c.Name != "Admin");
        AssertSql(whereMultipleQuery, "SELECT * FROM customers WHERE (customers.Age > 18) AND (customers.Name != 'Admin')", "WHERE with multiple conditions");
    }

    /// <summary>
    /// Test ORDER BY clause generation
    /// </summary>
    private static void TestOrderByClause()
    {
        Console.WriteLine("--- ORDER BY Clause Tests ---");

        // ORDER BY ascending
        var orderByQuery = SqlQuery.From<Customer>()
            .OrderBy(c => c.Name);
        AssertSql(orderByQuery, "SELECT * FROM customers ORDER BY customers.Name ASC", "ORDER BY ascending");

        // ORDER BY descending
        var orderByDescQuery = SqlQuery.From<Customer>()
            .OrderByDescending(c => c.Age);
        AssertSql(orderByDescQuery, "SELECT * FROM customers ORDER BY customers.Age DESC", "ORDER BY descending");
    }

    /// <summary>
    /// Test complex query combinations
    /// </summary>
    private static void TestComplexQueries()
    {
        Console.WriteLine("--- Complex Query Tests ---");

        // Complex query with SELECT, WHERE, and ORDER BY
        var complexQuery = SqlQuery.From<Customer>()
            .Where(c => c.Age > 18)
            .OrderBy(c => c.Name)
            .Select(c => (c.Id + 1, c.Name + "!"));
        AssertSql(complexQuery, 
            "SELECT (customers.Id + 1), CONCAT(customers.Name, '!') FROM customers WHERE customers.Age > 18 ORDER BY customers.Name ASC", 
            "Complex query with SELECT, WHERE, ORDER BY");

        // Query with WHERE and SELECT
        var whereSelectQuery = SqlQuery.From<Customer>()
            .Where(c => c.Age >= 21)
            .Select(c => (c.Id, c.Name));
        AssertSql(whereSelectQuery, 
            "SELECT customers.Id, customers.Name FROM customers WHERE customers.Age >= 21", 
            "Query with WHERE and SELECT");
    }

    /// <summary>
    /// Test aggregate functions
    /// </summary>
    private static void TestAggregates()
    {
        Console.WriteLine("--- Aggregate Function Tests ---");

        // COUNT query
        var countQuery = SqlQuery.From<Customer>().Count();
        AssertSqlExpr(countQuery, "COUNT(*)", "COUNT aggregate");

        // SUM query
        var sumQuery = SqlQuery.From<Customer>()
            .Select(c => c.Age)
            .Sum();
        AssertSqlExpr(sumQuery, "SUM(customers.Age)", "SUM aggregate");

        // COUNT with WHERE
        var countWhereQuery = SqlQuery.From<Customer>()
            .Where(c => c.Age > 18)
            .Count();
        AssertSqlExpr(countWhereQuery, "COUNT(*)", "COUNT with WHERE");

        // Full query that SELECTs a COUNT with WHERE
        var fullCountQuery = SqlQuery.From<Customer>()
            .Where(c => c.Age > 18)
            .Select(c => SqlQuery.From<Customer>().Where(cc => cc.Age > 18).Count());
        // This is a complex case that would require more sophisticated handling
    }

    /// <summary>
    /// Test various SQL expressions
    /// </summary>
    private static void TestExpressions()
    {
        Console.WriteLine("--- Expression Tests ---");

        // Integer literal
        SqlExprInt intLiteral = 42;
        AssertSqlExpr(intLiteral, "42", "Integer literal");

        // String literal
        SqlExprString stringLiteral = "Hello World";
        AssertSqlExpr(stringLiteral, "'Hello World'", "String literal");

        // Boolean literal
        SqlExprBool boolLiteral = true;
        AssertSqlExpr(boolLiteral, "TRUE", "Boolean literal true");

        SqlExprBool boolLiteralFalse = false;
        AssertSqlExpr(boolLiteralFalse, "FALSE", "Boolean literal false");
    }

    /// <summary>
    /// Test string expression operations
    /// </summary>
    private static void TestStringExpressions()
    {
        Console.WriteLine("--- String Expression Tests ---");

        var customer = new Customer();

        // String concatenation
        var concatExpr = customer.Name + " (Customer)";
        AssertSqlExpr(concatExpr, "CONCAT(customers.Name, ' (Customer)')", "String concatenation");

        // String equality
        var stringEqualExpr = customer.Name == "John";
        AssertSqlExpr(stringEqualExpr, "customers.Name = 'John'", "String equality");

        // String inequality
        var stringNotEqualExpr = customer.Name != "Admin";
        AssertSqlExpr(stringNotEqualExpr, "customers.Name != 'Admin'", "String inequality");

        // String comparison
        var stringGreaterExpr = customer.Name > "A";
        AssertSqlExpr(stringGreaterExpr, "customers.Name > 'A'", "String greater than");
    }

    /// <summary>
    /// Test boolean expression operations
    /// </summary>
    private static void TestBooleanExpressions()
    {
        Console.WriteLine("--- Boolean Expression Tests ---");

        var customer = new Customer();

        // AND operation
        var andExpr = customer.Age > 18 & customer.Age < 65;
        AssertSqlExpr(andExpr, "(customers.Age > 18) AND (customers.Age < 65)", "Boolean AND");

        // OR operation
        var orExpr = customer.Age < 18 | customer.Age > 65;
        AssertSqlExpr(orExpr, "(customers.Age < 18) OR (customers.Age > 65)", "Boolean OR");

        // NOT operation
        var notExpr = !(customer.Age > 18);
        AssertSqlExpr(notExpr, "NOT (customers.Age > 18)", "Boolean NOT");

        // Complex boolean expression
        var complexBoolExpr = (customer.Age > 18 & customer.Name != "Admin") | customer.Id == 1;
        AssertSqlExpr(complexBoolExpr, "((customers.Age > 18) AND (customers.Name != 'Admin')) OR (customers.Id = 1)", "Complex boolean expression");
    }

    /// <summary>
    /// Test arithmetic expression operations
    /// </summary>
    private static void TestArithmeticExpressions()
    {
        Console.WriteLine("--- Arithmetic Expression Tests ---");

        var customer = new Customer();

        // Addition
        var addExpr = customer.Age + 5;
        AssertSqlExpr(addExpr, "(customers.Age + 5)", "Integer addition");

        // Subtraction
        var subExpr = customer.Age - 1;
        AssertSqlExpr(subExpr, "(customers.Age - 1)", "Integer subtraction");

        // Multiplication
        var multExpr = customer.Id * 2;
        AssertSqlExpr(multExpr, "(customers.Id * 2)", "Integer multiplication");

        // Division
        var divExpr = customer.Age / 2;
        AssertSqlExpr(divExpr, "(customers.Age / 2)", "Integer division");

        // Complex arithmetic
        var complexArithExpr = (customer.Age + 1) * 2 - customer.Id;
        AssertSqlExpr(complexArithExpr, "(((customers.Age + 1) * 2) - customers.Id)", "Complex arithmetic expression");

        // Unary minus
        var unaryMinusExpr = -customer.Age;
        AssertSqlExpr(unaryMinusExpr, "-customers.Age", "Unary minus");
    }
}
