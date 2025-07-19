using System.Runtime.CompilerServices;

namespace TypedSqlBuilder.Core;

public abstract record SqlQuery
{ 
    public static SqlQuery<TColumns> From<TColumns>(TColumns columns)
        where TColumns : ITuple
    {
        throw new NotImplementedException("This method should be implemented in derived classes.");
    }
}
public abstract record SqlQuery<TSource> : SqlQuery;

public record SqlTable<TColumns>(string Name, TColumns columns) : SqlQuery<TColumns>
    where TColumns : ITuple;


public static class SqlQueryExtensions
{
    public static SqlQuery<TSource> Select<TSource, TResult>(this SqlQuery<TSource> query, Func<TSource, TResult> selector)
        where TSource : ITuple
        where TResult : ITuple
    {
        throw new NotImplementedException("This method should be implemented in derived classes.");
    }

    public static SqlQuery<SqlExprInt> Select<TSource>(this SqlQuery<TSource> query, Func<TSource, SqlExprInt> selector)
        where TSource : ITuple
    {
        throw new NotImplementedException("This method should be implemented in derived classes.");
    }

    public static SqlExprInt Sum(this SqlQuery<SqlExprInt> query)          
    {
        throw new NotImplementedException("This method should be implemented in derived classes.");
    }

    public static SqlExprInt Count<TSource>(this SqlQuery<TSource> query) 
        where TSource : ITuple
    {
        throw new NotImplementedException("This method should be implemented in derived classes.");
    }       
    
    public static SqlQuery<TSource> Where<TSource>(this SqlQuery<TSource> query, Func<TSource, SqlExprBool> predicate)
        where TSource : ITuple
    {
        throw new NotImplementedException("This method should be implemented in derived classes.");
    }

    public static SqlQuery<TSource> OrderBy<TSource, TKey>(this SqlQuery<TSource> query, Func<TSource, TKey> keySelector)
        where TSource : ITuple
        where TKey : SqlExpr
    {
        throw new NotImplementedException("This method should be implemented in derived classes.");
    }

    public static SqlQuery<TSource> OrderByDescending<TSource, TKey>(this SqlQuery<TSource> query, Func<TSource, TKey> keySelector)
        where TSource : ITuple
        where TKey : SqlExpr
    {
        throw new NotImplementedException("This method should be implemented in derived classes.");
    }
}    