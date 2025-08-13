using TypedSqlBuilder.Core;

namespace TypedSqlBuilder.Tests;

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
        => Db.Customers.From().OrderBy(c => c.Name);

    public static ISqlQuery FromOrderByDesc()
        => Db.Customers.From().OrderByDescending(c => c.Age);

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
            .OrderBy(c => c.Age);

    // SELECT + ORDER BY
    public static ISqlQuery FromSelectOrderBy()
        => Db.Customers.From()
            .OrderBy(c => c.Name)
            .Select(c => (c.Id, c.Name, c.Age + 5));

    // WHERE + SELECT + ORDER BY (all clauses)
    public static ISqlQuery FromWhereSelectOrderBy()
        => Db.Customers.From()
            .Where(c => c.Age > 18)
            .OrderBy(c => c.Name)
            .Select(c => (c.Id + 1, c.Name + "!"));

    public static ISqlQuery FromWhereOrderBySelect()
        => Db.Customers.From()
            .Where(c => c.Age > 21 && c.Name != "")
            .OrderBy(c => c.Age)
            .Select(c => (c.Id, c.Name, c.Age + 10));

    public static ISqlQuery FromWhereOrderBySelectNamed()
        => Db.Customers.From()
            .Where(c => c.Age >= 21 && c.Name != "")
            .OrderBy(c => c.Name)
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
            .OrderBy(c => c.Name);

    // Multiple ORDER BY tests - ThenBy functionality
    public static ISqlQuery FromOrderByThenBy()
        => Db.Customers.From()
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Age);

    public static ISqlQuery FromOrderByThenByDescending()
        => Db.Customers.From()
            .OrderBy(c => c.Name)
            .ThenByDescending(c => c.Age);

    public static ISqlQuery FromOrderByDescendingThenBy()
        => Db.Customers.From()
            .OrderByDescending(c => c.Age)
            .ThenBy(c => c.Name);

    public static ISqlQuery FromOrderByMultiple()
        => Db.Customers.From()
            .OrderBy(c => c.Name)
            .ThenByDescending(c => c.Age)
            .ThenBy(c => c.Id);

    public static ISqlQuery FromWhereOrderByThenBy()
        => Db.Customers.From()
            .Where(c => c.Age > 18)
            .OrderBy(c => c.Name)
            .ThenByDescending(c => c.Age);

    public static ISqlQuery FromOrderByThenBySelect()
        => Db.Customers.From()
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Age)
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
            .OrderBy(result => result.Name);

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
            .OrderBy(result => result.Name)
            .ThenByDescending(result => result.Amount);

    // Complex JOIN scenarios
    public static ISqlQuery InnerJoinWithGroupBy()
        => Db.Customers.From()
            .InnerJoin(
                Db.Orders,
                c => c.Id,
                o => o.CustomerId,
                (c, o) => (c.Id, c.Name, o.Amount))
            .GroupBy(result => result.Id)
            .Select((result, agg) => (CustomerId: result.Id, CustomerName: result.Name, TotalAmount: agg.Sum(result.Amount)));

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

}
