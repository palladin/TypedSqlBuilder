using TypedSqlBuilder.IntegrationTests;

Console.WriteLine("=== Debug Test Data ===");

Console.WriteLine("Customer Data:");
foreach (var c in TestDataConstants.Customers)
{
    Console.WriteLine($"  {c}");
}

Console.WriteLine("\nCustomer Tuples:");
foreach (var t in TestDataConstants.CustomerTuples)
{
    Console.WriteLine($"  {t}");
}

Console.WriteLine("\nOrder Data:");
foreach (var o in TestDataConstants.Orders)
{
    Console.WriteLine($"  {o}");
}

Console.WriteLine("\nOrder Tuples:");
foreach (var t in TestDataConstants.OrderTuples)
{
    Console.WriteLine($"  {t}");
}

Console.WriteLine("\nProduct Data:");
foreach (var p in TestDataConstants.Products)
{
    Console.WriteLine($"  {p}");
}

Console.WriteLine("\nProduct Tuples:");
foreach (var t in TestDataConstants.ProductTuples)
{
    Console.WriteLine($"  {t}");
}
