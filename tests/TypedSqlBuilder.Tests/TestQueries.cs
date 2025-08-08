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
        => TypedSql.From<Customer>();

    // SELECT only  
    public static ISqlQuery FromSelect() 
        => TypedSql.From<Customer>().Select(c => (c.Id, c.Name));

    public static ISqlQuery FromSelectSingle() 
        => TypedSql.From<Customer>().Select(c => c.Age);

    public static ISqlQuery FromSelectExpression() 
        => TypedSql.From<Customer>().Select(c => (c.Id * 100 + c.Age, c.Name + " - Customer"));

    // WHERE only
    public static ISqlQuery FromWhereInt() 
        => TypedSql.From<Customer>().Where(c => c.Age > 18);

    public static ISqlQuery FromWhereString() 
        => TypedSql.From<Customer>().Where(c => c.Name == "John");

    public static ISqlQuery FromWhereMultiple() 
        => TypedSql.From<Customer>().Where(c => c.Age > 18 && c.Name != "Admin");

    public static ISqlQuery FromWhereOr() 
        => TypedSql.From<Customer>().Where(c => (c.Age > 18 && c.Age < 65) || c.Name == "VIP");

    public static ISqlQuery FromWhereAnd() 
        => TypedSql.From<Customer>().Where(c => c.Age > 18 && c.Name == "John");

    // ORDER BY only
    public static ISqlQuery FromOrderByAsc() 
        => TypedSql.From<Customer>().OrderBy(c => c.Name);

    public static ISqlQuery FromOrderByDesc() 
        => TypedSql.From<Customer>().OrderByDescending(c => c.Age);

    // WHERE + SELECT
    public static ISqlQuery FromWhereSelect() 
        => TypedSql.From<Customer>()
            .Where(c => c.Age >= 21)
            .Select(c => (c.Id, c.Name));

    public static ISqlQuery FromWhereAndSelect() 
        => TypedSql.From<Customer>()
            .Where(c => c.Age >= 21 && c.Name != "")
            .Select(c => (c.Id, c.Name));

    // WHERE + ORDER BY
    public static ISqlQuery FromWhereOrderBy() 
        => TypedSql.From<Customer>()
            .Where(c => c.Age > 21 && c.Name != "")
            .OrderBy(c => c.Age);

    // SELECT + ORDER BY
    public static ISqlQuery FromSelectOrderBy() 
        => TypedSql.From<Customer>()
            .OrderBy(c => c.Name)
            .Select(c => (c.Id, c.Name, c.Age + 5));

    // WHERE + SELECT + ORDER BY (all clauses)
    public static ISqlQuery FromWhereSelectOrderBy() 
        => TypedSql.From<Customer>()
            .Where(c => c.Age > 18)
            .OrderBy(c => c.Name)
            .Select(c => (c.Id + 1, c.Name + "!"));

    public static ISqlQuery FromWhereOrderBySelect() 
        => TypedSql.From<Customer>()
            .Where(c => c.Age > 21 && c.Name != "")
            .OrderBy(c => c.Age)
            .Select(c => (c.Id, c.Name, c.Age + 10));

    public static ISqlQuery FromWhereOrderBySelectNamed() 
        => TypedSql.From<Customer>()
            .Where(c => c.Age >= 21 && c.Name != "")
            .OrderBy(c => c.Name)
            .Select(c => (
                CustomerId: c.Id,
                CustomerInfo: c.Name + " (Customer)",
                AdjustedAge: c.Age + 5
            ));

    public static ISqlQuery FromWhereSelectNamed() 
        => TypedSql.From<Customer>()
            .Where(c => c.Age > 18)
            .Select(c => (
                OriginalId: c.Id,
                ModifiedId: c.Id * 100 + c.Age,
                CustomerName: c.Name
            ));

    // Different table type
    public static ISqlQuery FromProductWhereSelect() 
        => TypedSql.From<Product>()
            .Where(p => p.ProductName != "Discontinued")
            .Select(p => (p.ProductId, p.ProductName));

    // Parameterized queries
    public static ISqlQuery FromWhereSelectParameterized(int minAge, int maxAge) 
        => TypedSql.From<Customer>()
            .Where(c => c.Age >= minAge && c.Age <= maxAge)
            .Select(c => (c.Id, c.Name));

    // WHERE clause fusion tests - multiple WHERE calls should be combined with AND
    public static ISqlQuery FromWhereFusionTwo() 
        => TypedSql.From<Customer>()
            .Where(c => c.Age > 18)
            .Where(c => c.Name != "Admin");

    public static ISqlQuery FromWhereFusionThree() 
        => TypedSql.From<Customer>()
            .Where(c => c.Age > 18)
            .Where(c => c.Name != "Admin")
            .Where(c => c.Age < 65);

    public static ISqlQuery FromWhereFusionWithSelect() 
        => TypedSql.From<Customer>()
            .Where(c => c.Age >= 21)
            .Where(c => c.Name != "")
            .Select(c => (c.Id, c.Name));

    public static ISqlQuery FromWhereFusionWithOrderBy() 
        => TypedSql.From<Customer>()
            .Where(c => c.Age > 18)
            .Where(c => c.Name != "Admin")
            .OrderBy(c => c.Name);

    // Multiple ORDER BY tests - ThenBy functionality
    public static ISqlQuery FromOrderByThenBy() 
        => TypedSql.From<Customer>()
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Age);

    public static ISqlQuery FromOrderByThenByDescending() 
        => TypedSql.From<Customer>()
            .OrderBy(c => c.Name)
            .ThenByDescending(c => c.Age);

    public static ISqlQuery FromOrderByDescendingThenBy() 
        => TypedSql.From<Customer>()
            .OrderByDescending(c => c.Age)
            .ThenBy(c => c.Name);

    public static ISqlQuery FromOrderByMultiple() 
        => TypedSql.From<Customer>()
            .OrderBy(c => c.Name)
            .ThenByDescending(c => c.Age)
            .ThenBy(c => c.Id);

    public static ISqlQuery FromWhereOrderByThenBy() 
        => TypedSql.From<Customer>()
            .Where(c => c.Age > 18)
            .OrderBy(c => c.Name)
            .ThenByDescending(c => c.Age);

    public static ISqlQuery FromOrderByThenBySelect() 
        => TypedSql.From<Customer>()
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Age)
            .Select(c => (c.Id, c.Name));
}
