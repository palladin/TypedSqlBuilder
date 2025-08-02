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
        => SqlQuery.From<Customer>();

    // SELECT only  
    public static ISqlQuery FromSelect() 
        => SqlQuery.From<Customer>().Select(c => (c.Id, c.Name));

    public static ISqlQuery FromSelectSingle() 
        => SqlQuery.From<Customer>().Select(c => c.Age);

    public static ISqlQuery FromSelectExpression() 
        => SqlQuery.From<Customer>().Select(c => (c.Id * 100 + c.Age, c.Name + " - Customer"));

    // WHERE only
    public static ISqlQuery FromWhereInt() 
        => SqlQuery.From<Customer>().Where(c => c.Age > 18);

    public static ISqlQuery FromWhereString() 
        => SqlQuery.From<Customer>().Where(c => c.Name == "John");

    public static ISqlQuery FromWhereMultiple() 
        => SqlQuery.From<Customer>().Where(c => c.Age > 18 && c.Name != "Admin");

    public static ISqlQuery FromWhereOr() 
        => SqlQuery.From<Customer>().Where(c => (c.Age > 18 && c.Age < 65) || c.Name == "VIP");

    public static ISqlQuery FromWhereAnd() 
        => SqlQuery.From<Customer>().Where(c => c.Age > 18 && c.Name == "John");

    // ORDER BY only
    public static ISqlQuery FromOrderByAsc() 
        => SqlQuery.From<Customer>().OrderBy(c => c.Name);

    public static ISqlQuery FromOrderByDesc() 
        => SqlQuery.From<Customer>().OrderByDescending(c => c.Age);

    // WHERE + SELECT
    public static ISqlQuery FromWhereSelect() 
        => SqlQuery.From<Customer>()
            .Where(c => c.Age >= 21)
            .Select(c => (c.Id, c.Name));

    public static ISqlQuery FromWhereAndSelect() 
        => SqlQuery.From<Customer>()
            .Where(c => c.Age >= 21 && c.Name != "")
            .Select(c => (c.Id, c.Name));

    // WHERE + ORDER BY
    public static ISqlQuery FromWhereOrderBy() 
        => SqlQuery.From<Customer>()
            .Where(c => c.Age > 21 && c.Name != "")
            .OrderBy(c => c.Age);

    // SELECT + ORDER BY
    public static ISqlQuery FromSelectOrderBy() 
        => SqlQuery.From<Customer>()
            .OrderBy(c => c.Name)
            .Select(c => (c.Id, c.Name, c.Age + 5));

    // WHERE + SELECT + ORDER BY (all clauses)
    public static ISqlQuery FromWhereSelectOrderBy() 
        => SqlQuery.From<Customer>()
            .Where(c => c.Age > 18)
            .OrderBy(c => c.Name)
            .Select(c => (c.Id + 1, c.Name + "!"));

    public static ISqlQuery FromWhereOrderBySelect() 
        => SqlQuery.From<Customer>()
            .Where(c => c.Age > 21 && c.Name != "")
            .OrderBy(c => c.Age)
            .Select(c => (c.Id, c.Name, c.Age + 10));

    public static ISqlQuery FromWhereOrderBySelectNamed() 
        => SqlQuery.From<Customer>()
            .Where(c => c.Age >= 21 && c.Name != "")
            .OrderBy(c => c.Name)
            .Select(c => (
                CustomerId: c.Id,
                CustomerInfo: c.Name + " (Customer)",
                AdjustedAge: c.Age + 5
            ));

    public static ISqlQuery FromWhereSelectNamed() 
        => SqlQuery.From<Customer>()
            .Where(c => c.Age > 18)
            .Select(c => (
                OriginalId: c.Id,
                ModifiedId: c.Id * 100 + c.Age,
                CustomerName: c.Name
            ));

    // Different table type
    public static ISqlQuery FromProductWhereSelect() 
        => SqlQuery.From<Product>()
            .Where(p => p.ProductName != "Discontinued")
            .Select(p => (p.ProductId, p.ProductName));

    // Parameterized queries
    public static ISqlQuery FromWhereSelectParameterized(int minAge, int maxAge) 
        => SqlQuery.From<Customer>()
            .Where(c => c.Age >= minAge && c.Age <= maxAge)
            .Select(c => (c.Id, c.Name));
}
