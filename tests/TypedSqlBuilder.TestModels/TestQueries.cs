using TypedSqlBuilder.Core;
using TypedSqlBuilder.TestModels;

namespace TypedSqlBuilder.TestModels;

/// <summary>
/// Database-agnostic test queries organized by SQL clause structure.
/// Names reflect the actual clauses used in the query.
/// </summary>
public static class TestQueries
{
    // FROM only
    public static ISqlQuery From()
        => Db.Customers.From();

    // FROM using static method (old pattern)
    public static ISqlQuery FromStatic()
        => TypedSql.From<Customer>();

    // SELECT only  
    public static ISqlQuery FromSelect()
        => Db.Customers.From().Select(c => (c.Id, c.Name));

    public static ISqlQuery FromSelectSingle()
        => Db.Customers.From().Select(c => c.Age);

    public static ISqlQuery FromSelectExpression()
        => Db.Customers.From().Select(c => (c.Id * 100 + c.Age, c.Name + " - Customer"));

    // WHERE only
    public static ISqlQuery FromWhereInt()
        => Db.Customers.From().Where(c => c.Age > 18);

    public static ISqlQuery FromWhereString()
        => Db.Customers.From().Where(c => c.Name == "John");

    public static ISqlQuery FromWhereMultiple()
        => Db.Customers.From().Where(c => c.Age > 18 && c.Name != "Admin");

    public static ISqlQuery FromWhereOr()
        => Db.Customers.From().Where(c => (c.Age > 18 && c.Age < 65) || c.Name == "VIP");

    public static ISqlQuery FromWhereAnd()
        => Db.Customers.From().Where(c => c.Age > 18 && c.Name == "John");

    // ORDER BY only
    public static ISqlQuery FromOrderByAsc()
        => Db.Customers.From().OrderBy(c => (c.Name, Sort.Asc));

    public static ISqlQuery FromOrderByDesc()
        => Db.Customers.From().OrderBy(c => (c.Age, Sort.Desc));

    // WHERE + SELECT
    public static ISqlQuery FromWhereSelect()
        => Db.Customers.From()
            .Where(c => c.Age >= 21)
            .Select(c => (c.Id, c.Name));

    public static ISqlQuery FromWhereAndSelect()
        => Db.Customers.From()
            .Where(c => c.Age >= 21 && c.Name != "")
            .Select(c => (c.Id, c.Name));

    // WHERE + ORDER BY
    public static ISqlQuery FromWhereOrderBy()
        => Db.Customers.From()
            .Where(c => c.Age > 21 && c.Name != "")
            .OrderBy(c => (c.Age, Sort.Asc));

    // SELECT + ORDER BY
    public static ISqlQuery FromSelectOrderBy()
        => Db.Customers.From()
            .OrderBy(c => (c.Name, Sort.Asc))
            .Select(c => (c.Id, c.Name, c.Age + 5));

    // WHERE + SELECT + ORDER BY (all clauses)
    public static ISqlQuery FromWhereSelectOrderBy()
        => Db.Customers.From()
            .Where(c => c.Age > 18)
            .OrderBy(c => (c.Name, Sort.Asc))
            .Select(c => (c.Id + 1, c.Name + "!"));

    public static ISqlQuery FromWhereOrderBySelect()
        => Db.Customers.From()
            .Where(c => c.Age > 21 && c.Name != "")
            .OrderBy(c => (c.Age, Sort.Asc))
            .Select(c => (c.Id, c.Name, c.Age + 10));

    public static ISqlQuery FromWhereOrderBySelectNamed()
        => Db.Customers.From()
            .Where(c => c.Age >= 21 && c.Name != "")
            .OrderBy(c => (c.Name, Sort.Asc))
            .Select(c => (
                CustomerId: c.Id,
                CustomerInfo: c.Name + " (Customer)",
                AdjustedAge: c.Age + 5
            ));

    public static ISqlQuery FromWhereSelectNamed()
        => Db.Customers.From()
            .Where(c => c.Age > 18)
            .Select(c => (
                OriginalId: c.Id,
                ModifiedId: c.Id * 100 + c.Age,
                CustomerName: c.Name
            ));

    // Different table type
    public static ISqlQuery FromProductWhereSelect()
        => Db.Products.From()
            .Where(p => p.ProductName != "Discontinued")
            .Select(p => (p.ProductId, p.ProductName));

    // Parameterized queries
    public static ISqlQuery FromWhereSelectParameterized(int minAge, int maxAge)
        => Db.Customers.From()
            .Where(c => c.Age >= minAge && c.Age <= maxAge)
            .Select(c => (c.Id, c.Name));

    // WHERE clause fusion tests - multiple WHERE calls should be combined with AND
    public static ISqlQuery FromWhereFusionTwo()
        => Db.Customers.From()
            .Where(c => c.Age > 18)
            .Where(c => c.Name != "Admin");

    public static ISqlQuery FromWhereFusionThree()
        => Db.Customers.From()
            .Where(c => c.Age > 18)
            .Where(c => c.Name != "Admin")
            .Where(c => c.Age < 65);

    public static ISqlQuery FromWhereFusionWithSelect()
        => Db.Customers.From()
            .Where(c => c.Age >= 21)
            .Where(c => c.Name != "")
            .Select(c => (c.Id, c.Name));

    public static ISqlQuery FromWhereFusionWithOrderBy()
        => Db.Customers.From()
            .Where(c => c.Age > 18)
            .Where(c => c.Name != "Admin")
            .OrderBy(c => (c.Name, Sort.Asc));

    // Multiple ORDER BY tests - ThenBy functionality
    public static ISqlQuery FromOrderByThenBy()
        => Db.Customers.From()
            .OrderBy(c => ((c.Name, Sort.Asc), (c.Age, Sort.Asc)));

    public static ISqlQuery FromOrderByThenByDescending()
        => Db.Customers.From()
            .OrderBy(c => ((c.Name, Sort.Asc), (c.Age, Sort.Desc)));

    public static ISqlQuery FromOrderByDescendingThenBy()
        => Db.Customers.From()
            .OrderBy(c => ((c.Age, Sort.Desc), (c.Name, Sort.Asc)));

    public static ISqlQuery FromOrderByMultiple()
        => Db.Customers.From()
            .OrderBy(c => ((c.Name, Sort.Asc), (c.Age, Sort.Desc), (c.Id, Sort.Asc)));

    public static ISqlQuery FromWhereOrderByThenBy()
        => Db.Customers.From()
            .Where(c => c.Age > 18)
            .OrderBy(c => ((c.Name, Sort.Asc), (c.Age, Sort.Desc)));

    public static ISqlQuery FromOrderByThenBySelect()
        => Db.Customers.From()
            .OrderBy(c => ((c.Name, Sort.Asc), (c.Age, Sort.Asc)))
            .Select(c => (c.Id, c.Name));

        // NULL tests
    public static ISqlQuery FromWhereIsNull()
        => Db.Customers.From()
            .Where(c => c.Name == SqlNull.Value);

    public static ISqlQuery FromWhereIsNotNull()
        => Db.Customers.From()
            .Where(c => c.Name != SqlNull.Value);

    public static ISqlQuery FromWhereIsNullInt()
        => Db.Customers.From()
            .Where(c => c.Age == SqlNull.Value);

    public static ISqlQuery FromWhereIsNotNullInt()
        => Db.Customers.From()
            .Where(c => c.Age != SqlNull.Value);

    public static ISqlQuery FromWhereIsNullCombined()
        => Db.Customers.From()
            .Where(c => c.Name == SqlNull.Value && c.Age != SqlNull.Value);

    // Scalar aggregate queries 
    public static ISqlScalarQuery<SqlExprInt> SumAgesWithDb()
        => Db.Customers.From().Select(c => c.Age).Sum();

    public static ISqlScalarQuery<SqlExprInt> CountCustomersWithDb()
        => Db.Customers.From().Count();

    public static ISqlScalarQuery<SqlExprInt> CountActiveCustomersWithDb()
        => Db.Customers.From().Where(c => c.Age >= 18).Count();

    public static ISqlQuery FromWhereAgeGreaterThanSum()
        => Db.Customers.From()
            .Where(c => c.Age > Db.Customers.From().Select(x => x.Age).Sum());

    // Aggregate functions
    public static ISqlScalarQuery<SqlExprInt> SumAges()
        => Db.Customers.From().Select(c => c.Age).Sum();

    public static ISqlScalarQuery<SqlExprInt> CountCustomers()
        => Db.Customers.From().Count();

    public static ISqlScalarQuery<SqlExprInt> CountActiveCustomers()
        => Db.Customers.From().Where(c => c.Age >= 18).Count();

    // Test case for scalar queries used as expressions (should have parentheses)
    public static ISqlQuery FromWhereAgeGreaterThanAverageAge()
        => Db.Customers.From().Where(c => c.Age > Db.Customers.From().Select(x => x.Age).Sum());

    // Test case for IN clause with integer values
    public static ISqlQuery FromWhereAgeIn()
        => Db.Customers.From().Where(c => c.Age.In(18, 21, 25, 30));

    // Test cases for IN clause with subqueries
    public static ISqlQuery FromWhereAgeInSubquery()
        => Db.Customers.From()
            .Where(c => c.Age.In(
                Db.Customers.From()
                    .Where(x => x.Name == "VIP")
                    .Select(x => x.Age)
            ));

    // Test case for closure semantics - outer variable captured in subquery
    public static ISqlQuery FromWhereAgeInSubqueryWithClosure()
        => Db.Customers.From()
            .Where(c => c.Age.In(
                Db.Customers.From()
                    .Where(x => x.Name == c.Name + "_VIP")  // Capturing 'c' from outer scope
                    .Select(x => x.Age)
            ));

    public static ISqlQuery FromSubquery()
        => TypedSql.From(Db.Customers.From().Select(x => (x.Id, NewAge: x.Age + 1)))
            .Select(x => (x.Id, x.NewAge));

    // Nested subquery test: select(where(select(where(from))))
    public static ISqlQuery FromWhereSelectWhereFromNested()
        => TypedSql.From(
                Db.Customers.From()
                    .Where(c => c.Age > 18)
                    .Select(c => (c.Id, c.Name))
            )
            .Where(x => x.Id > 100)
            .Select(x => (x.Id, x.Name));
            
    public static ISqlQuery FromWhereSelectWhereNested() =>
        Db.Customers.From()
                .Where(c => c.Age > 18)
                .Select(c => (c.Id, c.Name))
                .Where(x => x.Id > 100)
                .Select(x => (x.Id, x.Name));

    // GROUP BY test cases
    public static ISqlQuery FromGroupBySelect()
        => Db.Customers.From()
            .GroupBy(c => c.Age)
            .Select((c, agg) => (Age: c.Age, Count: agg.Count()));

    public static ISqlQuery FromGroupByMultipleSelect()
        => Db.Customers.From()
            .GroupBy(c => (c.Age, c.Name))
            .Select((c, agg) => (c.Age, c.Name, Count: agg.Count()));

    public static ISqlQuery FromGroupByHavingSelect()
        => Db.Customers.From()
            .GroupBy(c => c.Age)
            .Having((c, agg) => agg.Count() > 1)
            .Select((c, agg) => (Age: c.Age, Count: agg.Count()));

    public static ISqlQuery FromWhereGroupBySelect()
        => Db.Customers.From()
            .Where(c => c.Age >= 18)
            .GroupBy(c => c.Age)
            .Select((c, agg) => (Age: c.Age, Count: agg.Count()));

    // JOIN test cases - INNER JOIN
    public static ISqlQuery InnerJoinBasic()
        => Db.Customers.From()
            .InnerJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (c.Id, c.Name, o.OrderId, o.Amount));

    public static ISqlQuery InnerJoinWithSelect()
        => Db.Customers.From()
            .InnerJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (CustomerName: c.Name, OrderAmount: o.Amount));

    public static ISqlQuery InnerJoinWithWhere()
        => Db.Customers.From()
            .Where(c => c.Age >= 18)
            .InnerJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (c.Id, c.Name, c.Age, o.OrderId, o.Amount))
            .Where(result => result.Amount > 100);

    public static ISqlQuery InnerJoinWithOrderBy()
        => Db.Customers.From()
            .InnerJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (c.Id, c.Name, o.OrderId, o.Amount))
            .OrderBy(result => (result.Name, Sort.Asc));

    // JOIN test cases - LEFT JOIN
    public static ISqlQuery LeftJoinBasic()
        => Db.Customers.From()
            .LeftJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (c.Id, c.Name, o.OrderId, o.Amount));

    public static ISqlQuery LeftJoinWithSelect()
        => Db.Customers.From()
            .LeftJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (CustomerInfo: c.Name + " (Customer)", OrderAmount: o.Amount));

    public static ISqlQuery LeftJoinWithWhere()
        => Db.Customers.From()
            .Where(c => c.Age >= 21)
            .LeftJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (c.Id, c.Name, c.Age, OrderId: o.OrderId, OrderAmount: o.Amount))
            .Where(result => result.Age < 65);

    public static ISqlQuery LeftJoinWithOrderBy()
        => Db.Customers.From()
            .LeftJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (c.Id, c.Name, o.OrderId, o.Amount))
            .OrderBy(result => ((result.Name, Sort.Asc), (result.Amount, Sort.Desc)));

    // Complex JOIN scenarios
    public static ISqlQuery InnerJoinWithGroupBy()
        => Db.Customers.From()
            .InnerJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (CustomerId: c.Id, CustomerName: c.Name, Amount: o.Amount))
            .GroupBy(result => (result.CustomerId, result.CustomerName))
            .Select((result, agg) => (CustomerId: result.CustomerId, CustomerName: result.CustomerName, TotalAmount: agg.Sum(result.Amount)));

    public static ISqlQuery LeftJoinWithAggregates()
        => Db.Customers.From()
            .LeftJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (c.Id, c.Name, c.Age, o.Amount))
            .GroupBy(result => result.Id)
            .Select((result, agg) => (
                CustomerId: result.Id,
                CustomerName: result.Name,
                CustomerAge: result.Age,
                OrderCount: agg.Count(),
                TotalSpent: agg.Sum(result.Amount)
            ));

    // JOIN Fusion test cases - tests the new JoinClause fusion rule
    public static ISqlQuery MultipleInnerJoinsFusion()
        => Db.Customers.From()
            .InnerJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (Customer: c, Order: o))
            .InnerJoin(
                Db.Products,
                joined => joined.Order.Amount, // Using Amount as a simple join key for testing
                p => p.ProductId,
                (joined, p) => (joined.Customer.Id, joined.Customer.Name, joined.Order.OrderId, p.ProductName));

    public static ISqlQuery MixedJoinTypesFusion()
        => Db.Customers.From()
            .InnerJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (Customer: c, Order: o))
            .LeftJoin(
                Db.Products,
                joined => joined.Order.Amount,
                p => p.ProductId,
                (joined, p) => (joined.Customer.Id, joined.Customer.Name, joined.Order.OrderId, ProductName: p.ProductName));

    public static ISqlQuery JoinFusionWithWhere()
        => Db.Customers.From()
            .Where(c => c.Age >= 18)
            .InnerJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (Customer: c, Order: o))
            .InnerJoin(
                Db.Products,
                joined => joined.Order.Amount,
                p => p.ProductId,
                (joined, p) => (joined.Customer.Id, joined.Customer.Name, joined.Order.Amount, p.ProductName))
            .Where(result => result.Amount > 100);

    // GROUP BY with ORDER BY test cases
    public static ISqlQuery FromGroupByOrderBySelect()
        => Db.Orders.From()
            .GroupBy(o => o.CustomerId)
            .OrderBy((o, agg) => (agg.Sum(o.Amount), Sort.Desc))
            .Select((o, agg) => (CustomerId: o.CustomerId, TotalAmount: agg.Sum(o.Amount)));

    public static ISqlQuery FromGroupByOrderByMultipleSelect()
        => Db.Orders.From()
            .GroupBy(o => o.CustomerId)
            .OrderBy((o, agg) => ((agg.Sum(o.Amount), Sort.Desc), (agg.Count(), Sort.Asc)))
            .Select((o, agg) => (CustomerId: o.CustomerId, TotalAmount: agg.Sum(o.Amount), OrderCount: agg.Count()));

    public static ISqlQuery FromGroupByOrderByThreeKeysSelect()
        => Db.Orders.From()
            .GroupBy(o => o.CustomerId)
            .OrderBy((o, agg) => ((agg.Sum(o.Amount), Sort.Desc), (agg.Count(), Sort.Asc), (o.CustomerId, Sort.Asc)))
            .Select((o, agg) => (CustomerId: o.CustomerId, TotalAmount: agg.Sum(o.Amount), OrderCount: agg.Count()));

    public static ISqlQuery FromGroupByMultipleOrderBySelect()
        => Db.Customers.From()
            .GroupBy(c => (c.Age, c.Name))
            .OrderBy((c, agg) => (agg.Count(), Sort.Desc))
            .Select((c, agg) => (c.Age, c.Name, Count: agg.Count()));

    public static ISqlQuery FromGroupByHavingOrderBySelect()
        => Db.Orders.From()
            .GroupBy(o => o.CustomerId)
            .Having((o, agg) => agg.Count() > 1)
            .OrderBy((o, agg) => (agg.Sum(o.Amount), Sort.Desc))
            .Select((o, agg) => (CustomerId: o.CustomerId, TotalAmount: agg.Sum(o.Amount)));

    // Complex case: JOIN -> WHERE -> GROUP BY -> HAVING -> ORDER BY -> SELECT
    public static ISqlQuery ComplexJoinWhereGroupByHavingOrderBySelect()
        => Db.Customers.From()
            .InnerJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (Customer: c, Order: o))
            .Where(result => result.Customer.Age >= 18 && result.Order.Amount > 50)
            .GroupBy(result => (result.Customer.Id, result.Customer.Name))
            .Having((result, agg) => agg.Count() > 2 && agg.Sum(result.Order.Amount) > 500)
            .OrderBy((result, agg) => ((agg.Sum(result.Order.Amount), Sort.Desc), (agg.Count(), Sort.Asc)))
            .Select((result, agg) => (
                CustomerId: result.Customer.Id,
                CustomerName: result.Customer.Name,
                TotalOrders: agg.Count(),
                TotalSpent: agg.Sum(result.Order.Amount),
                AvgOrderValue: agg.Sum(result.Order.Amount) / agg.Count()
            ));

    // Another complex case with LEFT JOIN
    public static ISqlQuery ComplexLeftJoinWhereGroupByOrderBySelect()
        => Db.Customers.From()
            .LeftJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (Customer: c, Order: o))
            .Where(result => result.Customer.Age >= 21)
            .GroupBy(result => (result.Customer.Id, result.Customer.Name))
            .OrderBy((result, agg) => ((agg.Sum(result.Order.Amount), Sort.Desc), (result.Customer.Name, Sort.Asc)))
            .Select((result, agg) => (
                CustomerId: result.Customer.Id,
                CustomerName: result.Customer.Name,
                OrderCount: agg.Count(),
                TotalSpent: agg.Sum(result.Order.Amount)
            ));

    // Test for new MIN/MAX aggregate functions
    public static ISqlQuery FromGroupByMinMaxSelect()
        => Db.Orders.From()
            .GroupBy(o => o.CustomerId)
            .Select((result, agg) => (
                CustomerId: result.CustomerId,
                MinAmount: agg.Min(result.Amount),
                MaxAmount: agg.Max(result.Amount),
                OrderCount: agg.Count()
            ));

    // Test for AVG aggregate function
    public static ISqlQuery FromGroupByAvgSelect()
        => Db.Orders.From()
            .GroupBy(o => o.CustomerId)
            .Select((result, agg) => (
                CustomerId: result.CustomerId,
                AvgAmount: agg.Avg(result.Amount),
                OrderCount: agg.Count()
            ));

    // Test extension methods for scalar aggregate queries
    public static ISqlScalarQuery FromSelectSum()
        => Db.Orders.From().Select(o => o.Amount).Sum();

    public static ISqlScalarQuery FromSelectAvg()
        => Db.Orders.From().Select(o => o.Amount).Avg();

    public static ISqlScalarQuery FromSelectMin()
        => Db.Orders.From().Select(o => o.Amount).Min();

    public static ISqlScalarQuery FromSelectMax()
        => Db.Orders.From().Select(o => o.Amount).Max();

    // Test parameter creation extension methods
    public static ISqlQuery ParameterAsIntParam()
        => Db.Customers.From()
            .Where(c => c.Age > "minAge".AsIntParam())
            .Select(c => (c.Id, c.Name));

    public static ISqlQuery ParameterAsStringParam()
        => Db.Customers.From()
            .Where(c => c.Name == "customerName".AsStringParam())
            .Select(c => (c.Id, c.Age));

    public static ISqlQuery ParameterAsBoolParam()
        => Db.Customers.From()
            .Where(c => (c.Age > 18) == "isAdult".AsBoolParam())
            .Select(c => (c.Id, c.Name, c.Age));

    // Test direct boolean column comparison (should work better than boolean expression comparison)
    public static ISqlQuery BoolColumnDirectComparison()
        => Db.Customers.From()
            .Where(c => c.IsActive == "isActive".AsBoolParam())
            .Select(c => (c.Id, c.Name, c.Age, c.IsActive));

    // Test boolean column with literal value
    public static ISqlQuery BoolColumnLiteralTrue()
        => Db.Customers.From()
            .Where(c => c.IsActive == true)
            .Select(c => (c.Id, c.Name, c.IsActive));

    // Test boolean column with literal false
    public static ISqlQuery BoolColumnLiteralFalse()
        => Db.Customers.From()
            .Where(c => c.IsActive == false)
            .Select(c => (c.Id, c.Name, c.IsActive));

    // Test Case function with string values
    public static ISqlQuery CaseStringExpression()
        => Db.Customers.From()
            .Select(c => (c.Id, SqlFunc.Case(c.Age > 18, "Adult", "Minor")));

    // Test Case function with integer values
    public static ISqlQuery CaseIntExpression()
        => Db.Customers.From()
            .Select(c => (c.Id, SqlFunc.Case(c.Age > 65, 1, 0)));

    // Test Case function with boolean values
    public static ISqlQuery CaseBoolExpression()
        => Db.Customers.From()
            .Select(c => (c.Id, SqlFunc.Case(c.Age > 18, c.IsActive, false)));

    // Test Case function in WHERE clause
    public static ISqlQuery CaseInWhere()
        => Db.Customers.From()
            .Where(c => SqlFunc.Case(c.Age > 18, "Adult", "Minor") == "Adult")
            .Select(c => (c.Id, c.Name));

    // Test Like function with wildcard patterns
    public static ISqlQuery LikeWildcard()
        => Db.Customers.From()
            .Where(c => c.Name.Like("Jo%"))
            .Select(c => (c.Id, c.Name));

    // Test Like function with single character wildcard
    public static ISqlQuery LikeSingleChar()
        => Db.Customers.From()
            .Where(c => c.Name.Like("J_n"))
            .Select(c => (c.Id, c.Name));

    // Test Like function with both wildcards
    public static ISqlQuery LikeBothWildcards()
        => Db.Customers.From()
            .Where(c => c.Name.Like("%o_n%"))
            .Select(c => (c.Id, c.Name));

    // Test Like function with exact pattern
    public static ISqlQuery LikeExact()
        => Db.Customers.From()
            .Where(c => c.Name.Like("John"))
            .Select(c => (c.Id, c.Name));

    // Test Abs function on column
    public static ISqlQuery AbsColumn()
        => Db.Customers.From()
            .Select(c => (c.Id, c.Age.Abs()));

    // Test Abs function in WHERE clause
    public static ISqlQuery AbsInWhere()
        => Db.Customers.From()
            .Where(c => c.Age.Abs() > 30)
            .Select(c => (c.Id, c.Name, c.Age));

    // Test Abs function with expression
    public static ISqlQuery AbsExpression()
        => Db.Customers.From()
            .Select(c => (c.Id, (c.Age - 50).Abs()));

    // Test Abs function with parameter
    public static ISqlQuery AbsParameter()
        => Db.Customers.From()
            .Where(c => c.Age.Abs() > "minAge".AsIntParam().Abs())
            .Select(c => (c.Id, c.Name, c.Age));
}
