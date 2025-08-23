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
            .Select(p => (p.Id, p.ProductName));

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

    // Decimal Aggregate functions
    public static ISqlScalarQuery<SqlExprDecimal> SumPrices()
        => Db.Products.From().Select(p => p.Price).Sum();

    public static ISqlScalarQuery<SqlExprDecimal> AvgPrices()
        => Db.Products.From().Select(p => p.Price).Avg();

    public static ISqlScalarQuery<SqlExprDecimal> MinPrice()
        => Db.Products.From().Select(p => p.Price).Min();

    public static ISqlScalarQuery<SqlExprDecimal> MaxPrice()
        => Db.Products.From().Select(p => p.Price).Max();

    // Decimal aggregates with WHERE clauses
    public static ISqlScalarQuery<SqlExprDecimal> SumExpensivePrices()
        => Db.Products.From().Where(p => p.Price > 100m).Select(p => p.Price).Sum();

    public static ISqlScalarQuery<SqlExprDecimal> AvgExpensivePrices()
        => Db.Products.From().Where(p => p.Price > 100m).Select(p => p.Price).Avg();

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
                (c, o) => (c.Id, c.Name, o.Id, o.Amount));

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
                (c, o) => (c.Id, c.Name, c.Age, o.Id, o.Amount))
            .Where(result => result.Amount > 100);

    public static ISqlQuery InnerJoinWithOrderBy()
        => Db.Customers.From()
            .InnerJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (c.Id, c.Name, o.Id, o.Amount))
            .OrderBy(result => (result.Name, Sort.Asc));

    // JOIN test cases - LEFT JOIN
    public static ISqlQuery LeftJoinBasic()
        => Db.Customers.From()
            .LeftJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (c.Id, c.Name, o.Id, o.Amount));

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
                (c, o) => (c.Id, c.Name, c.Age, OrderId: o.Id, OrderAmount: o.Amount))
            .Where(result => result.Age < 65);

    public static ISqlQuery LeftJoinWithOrderBy()
        => Db.Customers.From()
            .LeftJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (c.Id, c.Name, o.Id, o.Amount))
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
                joined => joined.Order.ProductId,
                p => p.Id,
                (joined, p) => (joined.Customer.Id, joined.Customer.Name, joined.Order.Id, p.ProductName));

    public static ISqlQuery MixedJoinTypesFusion()
        => Db.Customers.From()
            .InnerJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (Customer: c, Order: o))
            .LeftJoin(
                Db.Products,
                joined => joined.Order.ProductId,
                p => p.Id,
                (joined, p) => (joined.Customer.Id, joined.Customer.Name, joined.Order.Id, ProductName: p.ProductName));

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
                joined => joined.Order.ProductId,
                p => p.Id,
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

    // Test for decimal aggregate functions in GROUP BY
    public static ISqlQuery FromGroupByDecimalAggregatesSelect()
        => Db.Products.From()
            .GroupBy(p => p.ProductName)
            .Select((result, agg) => (
                ProductName: result.ProductName,
                TotalPrice: agg.Sum(result.Price),
                AvgPrice: agg.Avg(result.Price),
                MinPrice: agg.Min(result.Price),
                MaxPrice: agg.Max(result.Price),
                ProductCount: agg.Count()
            ));

    public static ISqlQuery FromGroupByDecimalSumSelect()
        => Db.Products.From()
            .GroupBy(p => p.ProductName)
            .Select((result, agg) => (
                ProductName: result.ProductName,
                TotalPrice: agg.Sum(result.Price)
            ));

    public static ISqlQuery FromGroupByDecimalAvgSelect()
        => Db.Products.From()
            .GroupBy(p => p.ProductName)
            .Select((result, agg) => (
                ProductName: result.ProductName,
                AvgPrice: agg.Avg(result.Price)
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
            .Select(c => (c.Id, c.Name, c.Age, c.IsActive));

    // Test boolean column with literal false
    public static ISqlQuery BoolColumnLiteralFalse()
        => Db.Customers.From()
            .Where(c => c.IsActive == false)
            .Select(c => (c.Id, c.Name, c.Age, c.IsActive));

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

    // ========== NEW COLUMN TYPES TESTS - DECIMAL ==========
    
    // Test decimal comparison
    public static ISqlQuery FromWhereDecimalComparison()
        => Db.Products.From().Where(p => p.Price > 100.50m);

    // Test decimal arithmetic operations
    public static ISqlQuery FromSelectDecimalArithmetic()
        => Db.Products.From().Select(p => (p.ProductName, p.Price * 1.1m, p.Price + 10.0m, p.Price - 5.0m));

    // Test decimal null checks
    public static ISqlQuery FromWhereDecimalIsNull()
        => Db.Products.From().Where(p => p.Price == SqlNull.Value);

    public static ISqlQuery FromWhereDecimalIsNotNull()
        => Db.Products.From().Where(p => p.Price != SqlNull.Value);

    // Test decimal case expression
    public static ISqlQuery CaseDecimalExpression()
        => Db.Products.From()
            .Select(p => (p.ProductName, ExpensiveFlag: SqlFunc.Case(p.Price > 1000m, "Expensive", SqlFunc.Case(p.Price > 100m, "Moderate", "Cheap"))));

    // Test decimal parameter
    public static ISqlQuery ParameterAsDecimalParam()
        => Db.Products.From().Where(p => p.Price > "minPrice".AsDecimalParam());

    // ========== NEW COLUMN TYPES TESTS - DATETIME ==========
    
    // Test DateTime comparison
    public static ISqlQuery FromWhereCreatedDateComparison()
        => Db.Products.From().Where(p => p.CreatedDate > new DateTime(2024, 1, 1));

    // Test DateTime null checks
    public static ISqlQuery FromWhereCreatedDateIsNull()
        => Db.Products.From().Where(p => p.CreatedDate == SqlNull.Value);

    public static ISqlQuery FromWhereCreatedDateIsNotNull()
        => Db.Products.From().Where(p => p.CreatedDate != SqlNull.Value);

    // Test DateTime aggregates (using existing pattern)
    public static ISqlQuery FromSelectCreatedDateMinMax()
        => Db.Products.From()
            .Select(p => (p.ProductName, EarliestDate: p.CreatedDate, LatestDate: p.CreatedDate));

    // Test DateTime case expression
    public static ISqlQuery CaseDateTimeExpression()
        => Db.Products.From()
            .Select(p => (p.ProductName, Age: SqlFunc.Case(p.CreatedDate < new DateTime(2020, 1, 1), "Old", SqlFunc.Case(p.CreatedDate < new DateTime(2024, 1, 1), "Recent", "New"))));

    // Test DateTime parameter
    public static ISqlQuery ParameterAsDateTimeParam()
        => Db.Products.From().Where(p => p.CreatedDate > "startDate".AsDateTimeParam());

    // ========== NEW COLUMN TYPES TESTS - GUID ==========
    
    // Test Guid equality
    public static ISqlQuery FromWhereUniqueIdEquals()
        => Db.Products.From().Where(p => p.UniqueId == Guid.Parse("12345678-1234-1234-1234-123456789012"));

    // Test Guid inequality
    public static ISqlQuery FromWhereUniqueIdNotEquals()
        => Db.Products.From().Where(p => p.UniqueId != Guid.Empty);

    // Test Guid null checks
    public static ISqlQuery FromWhereUniqueIdIsNull()
        => Db.Products.From().Where(p => p.UniqueId == SqlNull.Value);

    public static ISqlQuery FromWhereUniqueIdIsNotNull()
        => Db.Products.From().Where(p => p.UniqueId != SqlNull.Value);

    // Test Guid case expression
    public static ISqlQuery CaseGuidExpression()
        => Db.Products.From()
            .Select(p => (p.ProductName, Status: SqlFunc.Case(p.UniqueId == Guid.Empty, "Empty", "HasId")));

    // Test Guid parameter
    public static ISqlQuery ParameterAsGuidParam()
        => Db.Products.From().Where(p => p.UniqueId == "targetId".AsGuidParam());

    // ========== STRING FUNCTIONS TESTS ==========
    
    // Test SUBSTRING function
    public static ISqlQuery StringSubstring()
        => Db.Products.From()
            .Select(p => p.ProductName.Substring(1, 5));

    // Test UPPER function
    public static ISqlQuery StringUpper()
        => Db.Products.From()
            .Select(p => p.ProductName.Upper());

    // Test LOWER function
    public static ISqlQuery StringLower()
        => Db.Products.From()
            .Select(p => p.ProductName.Lower());

    // Test TRIM function
    public static ISqlQuery StringTrim()
        => Db.Products.From()
            .Select(p => p.ProductName.Trim());

    // Test LEN/LENGTH function
    public static ISqlQuery StringLength()
        => Db.Products.From()
            .Select(p => p.ProductName.Length());

    // Test string functions in WHERE clause
    public static ISqlQuery StringFunctionsInWhere()
        => Db.Customers.From()
            .Where(c => c.Name.Upper() == "JOHN" && c.Name.Length() > 3)
            .Select(c => (c.Id, c.Name));

    // Test multiple string functions in SELECT
    public static ISqlQuery StringFunctionsInSelect()
        => Db.Customers.From()
            .Select(c => (
                c.Id, 
                UpperName: c.Name.Upper(),
                LowerName: c.Name.Lower(),
                TrimmedName: c.Name.Trim(),
                NameLength: c.Name.Length(),
                FirstThree: c.Name.Substring(1, 3)
            ));

    // ========== DATE/TIME FUNCTIONS TESTS ==========
    
    // Test NOW()/GETDATE() function
    public static ISqlQuery DateTimeNow()
        => Db.Products.From()
            .Select(p => SqlFunc.Now());

    // Test YEAR function
    public static ISqlQuery DateTimeYear()
        => Db.Products.From()
            .Select(p => p.CreatedDate.Year());

    // Test MONTH function
    public static ISqlQuery DateTimeMonth()
        => Db.Products.From()
            .Select(p => p.CreatedDate.Month());

    // Test DAY function
    public static ISqlQuery DateTimeDay()
        => Db.Products.From()
            .Select(p => p.CreatedDate.Day());

    // Test DATEADD for days
    public static ISqlQuery DateTimeAddDays()
        => Db.Products.From()
            .Select(p => p.CreatedDate.AddDays(30));

    // Test DATEADD for months
    public static ISqlQuery DateTimeAddMonths()
        => Db.Products.From()
            .Select(p => p.CreatedDate.AddMonths(6));

    // Test DATEADD for years
    public static ISqlQuery DateTimeAddYears()
        => Db.Products.From()
            .Select(p => p.CreatedDate.AddYears(1));

    // Test DATEDIFF for days
    public static ISqlQuery DateTimeDiffDays()
        => Db.Products.From()
            .Select(p => SqlFunc.DiffDays(p.CreatedDate, DateTime.Now));

    // Test DATEDIFF for months
    public static ISqlQuery DateTimeDiffMonths()
        => Db.Products.From()
            .Select(p => SqlFunc.DiffMonths(p.CreatedDate, DateTime.Now));

    // Test DATEDIFF for years
    public static ISqlQuery DateTimeDiffYears()
        => Db.Products.From()
            .Select(p => SqlFunc.DiffYears(p.CreatedDate, DateTime.Now));

    // Test date functions in WHERE clause
    public static ISqlQuery DateTimeFunctionsInWhere()
        => Db.Products.From()
            .Where(p => p.CreatedDate.Year() == 2024 && p.CreatedDate.Month() > 6)
            .Select(p => (p.Id, p.CreatedDate));

    // Test multiple date functions in SELECT
    public static ISqlQuery DateTimeFunctionsInSelect()
        => Db.Products.From()
            .Select(p => (
                p.Id,
                CreatedYear: p.CreatedDate.Year(),
                CreatedMonth: p.CreatedDate.Month(),
                CreatedDay: p.CreatedDate.Day(),
                NextWeek: p.CreatedDate.AddDays(7),
                NextMonth: p.CreatedDate.AddMonths(1),
                DaysAgo: SqlFunc.DiffDays(p.CreatedDate, SqlFunc.Now())
            ));

    // ========== MATHEMATICAL FUNCTIONS TESTS ==========
    
    // Test ROUND function
    public static ISqlQuery DecimalRound()
        => Db.Products.From()
            .Select(p => p.Price.Round(2));

    // Test CEILING function
    public static ISqlQuery DecimalCeiling()
        => Db.Products.From()
            .Select(p => p.Price.Ceiling());

    // Test FLOOR function
    public static ISqlQuery DecimalFloor()
        => Db.Products.From()
            .Select(p => p.Price.Floor());

    // Test math functions in WHERE clause
    public static ISqlQuery MathFunctionsInWhere()
        => Db.Products.From()
            .Where(p => p.Price.Round(0) > 100 && p.Price.Ceiling() < 1000)
            .Select(p => (p.Id, p.Price));

    // Test multiple math functions in SELECT
    public static ISqlQuery MathFunctionsInSelect()
        => Db.Products.From()
            .Select(p => (
                p.Id,
                OriginalPrice: p.Price,
                RoundedPrice: p.Price.Round(2),
                CeilingPrice: p.Price.Ceiling(),
                FloorPrice: p.Price.Floor()
            ));

    // LimitOffset Tests
    public static ISqlQuery FromLimitOffset()
        => Db.Customers.From()
            .OrderBy(c => (c.Id, Sort.Asc))
            .Select(c => c, limitOffset: (5L, 10L));

    public static ISqlQuery FromSelectLimitOffset()
        => Db.Customers.From()
            .OrderBy(c => (c.Id, Sort.Asc))
            .Select(c => (c.Id, c.Name), limitOffset: (3L, 5L));

    public static ISqlQuery FromWhereLimitOffset()
        => Db.Customers.From()
            .Where(c => c.Age > 18)
            .OrderBy(c => (c.Id, Sort.Asc))
            .Select(c => c, limitOffset: (10L, 0L));

    public static ISqlQuery FromWhereSelectLimitOffset()
        => Db.Customers.From()
            .Where(c => c.Age >= 21)
            .OrderBy(c => (c.Id, Sort.Asc))
            .Select(c => (c.Id, c.Name, c.Age), limitOffset: (5L, 15L));

    public static ISqlQuery FromOrderByLimitOffset()
        => Db.Customers.From()
            .OrderBy(c => (c.Name, Sort.Asc))
            .Select(c => c, limitOffset: (10L, 5L));

    public static ISqlQuery FromWhereOrderByLimitOffset()
        => Db.Customers.From()
            .Where(c => c.Age > 18)
            .OrderBy(c => (c.Age, Sort.Desc))
            .Select(c => c, limitOffset: (20L, 10L));

    public static ISqlQuery FromWhereOrderBySelectLimitOffset()
        => Db.Customers.From()
            .Where(c => c.Name != "")
            .OrderBy(c => ((c.Name, Sort.Asc), (c.Age, Sort.Desc)))
            .Select(c => (c.Id, c.Name, c.Age), limitOffset: (5L, 0L));

    public static ISqlQuery FromLimitOffsetOnly()
        => Db.Customers.From()
            .OrderBy(c => (c.Id, Sort.Asc))
            .Select(c => c, limitOffset: (10L, null));

    public static ISqlQuery FromOffsetOnly()
        => Db.Customers.From()
            .OrderBy(c => (c.Id, Sort.Asc))
            .Select(c => c, limitOffset: (long.MaxValue, 5L));

    // Special test without ORDER BY - should work for PostgreSQL/SQLite but not SQL Server
    public static ISqlQuery FromLimitOffsetWithoutOrderBy()
        => Db.Customers.From()
            .Select(c => c, limitOffset: (10L, null));

    // DISTINCT Tests
    public static ISqlQuery FromSelectDistinct()
        => Db.Customers.From()
            .Select(c => c.Name, distinct: true);

    public static ISqlQuery FromSelectDistinctWhere()
        => Db.Customers.From()
            .Where(c => c.Age > 18)
            .Select(c => c.Name, distinct: true);

    public static ISqlQuery FromSelectDistinctOrderBy()
        => Db.Customers.From()
            .OrderBy(c => (c.Name, Sort.Asc))
            .Select(c => c.Name, distinct: true);

    public static ISqlQuery FromSelectDistinctMultipleColumns()
        => Db.Customers.From()
            .OrderBy(c => (c.Name, Sort.Asc))
            .Select(c => (c.Name, c.Age), distinct: true);
}
