using System.Runtime.CompilerServices;

namespace TypedSqlBuilder.Core;

public abstract record SqlQuery
{ 
    public static SqlQuery<T> From<T>(T columns)
        where T : ITuple
    {
        throw new NotImplementedException("This method should be implemented in derived classes.");
    }
}
public abstract record SqlQuery<T> : SqlQuery where T : ITuple;

public record SqlTable<T>(string Name, T columns) : SqlQuery<T>
    where T : ITuple;


public static class SqlQueryExtensions
{
    public static SqlQuery<T> Select<T, R>(this SqlQuery<T> query, Func<T, R> selector)
        where T : ITuple
        where R : ITuple
    {
        throw new NotImplementedException("This method should be implemented in derived classes.");
    }

    public static SqlQuery<T> Where<T>(this SqlQuery<T> query, Func<T, SqlExprBool> predicate)
        where T : ITuple
    {
        throw new NotImplementedException("This method should be implemented in derived classes.");
    }

    public static SqlQuery<T> OrderBy<T, TKey>(this SqlQuery<T> query, Func<T, TKey> keySelector)
        where T : ITuple
        where TKey : SqlExpr
    {
        throw new NotImplementedException("This method should be implemented in derived classes.");
    }

    public static SqlQuery<T> OrderByDescending<T, TKey>(this SqlQuery<T> query, Func<T, TKey> keySelector)
        where T : ITuple
        where TKey : SqlExpr
    {
        throw new NotImplementedException("This method should be implemented in derived classes.");
    }
}    