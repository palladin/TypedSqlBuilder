using TypedSqlBuilder.Core;

Console.WriteLine("TypedSqlBuilder Design Testing");

// Test table schema as a tuple - this is your Customers alias concept
var customersTableName = "customers";
var customers = (
    Id: new SqlIntColumn(customersTableName, "id"),
    Name: new SqlStringColumn(customersTableName, "name"), 
    Age: new SqlIntColumn(customersTableName, "age")
);

ISqlQuery query =
    SqlQuery.From(customers)
            .Where(c => c.Age > ":age".AsIntParam())
            .OrderBy(c => c.Name)
            .Select(c => (c.Id + 1, c.Name + "!", c.Age > 20 ? "Adult" : "Minor"));
    