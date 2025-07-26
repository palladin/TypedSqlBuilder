using System.Security.Cryptography;
using TypedSqlBuilder.Core;

namespace TypedSqlBuilder.Tests;

public class Program
{
    public record Customer() : SqlTable<SqlIntColumn, SqlIntColumn, SqlStringColumn>("customers", new("Id"), new("Age"), new("Name"))
    { 
        public SqlIntColumn Id => Column1;
        public SqlIntColumn Age => Column2;
        public SqlStringColumn Name => Column3;
    }
    public static void Main(string[] args)
    {
        Console.WriteLine("TypedSqlBuilder Design Testing");

        ISqlQuery query =
            SqlQuery.From<Customer>()
                    .Where(c => c.Age > 18)
                    .OrderBy(c => c.Name)
                    .Select(c => (c.Id + 1, c.Name + "!"));
    }
}