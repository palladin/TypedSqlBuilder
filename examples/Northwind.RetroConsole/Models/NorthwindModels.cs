using System.ComponentModel.DataAnnotations.Schema;
using TypedSqlBuilder.Core;

namespace Northwind.RetroConsole.Models;

// TypedSqlBuilder table definitions
public class CustomersTable() : SqlTable("Customers")
{
    [Column("CustomerID")]
    public SqlStringColumn CustomerID { get; set; } = default!;
    [Column("CompanyName")]
    public SqlStringColumn CompanyName { get; set; } = default!;
    [Column("ContactName")]
    public SqlStringColumn ContactName { get; set; } = default!;
    [Column("ContactTitle")]
    public SqlStringColumn ContactTitle { get; set; } = default!;
    [Column("Address")]
    public SqlStringColumn Address { get; set; } = default!;
    [Column("City")]
    public SqlStringColumn City { get; set; } = default!;
    [Column("Region")]
    public SqlStringColumn Region { get; set; } = default!;
    [Column("PostalCode")]
    public SqlStringColumn PostalCode { get; set; } = default!;
    [Column("Country")]
    public SqlStringColumn Country { get; set; } = default!;
    [Column("Phone")]
    public SqlStringColumn Phone { get; set; } = default!;
    [Column("Fax")]
    public SqlStringColumn Fax { get; set; } = default!;
}

public class ProductsTable() : SqlTable("Products")
{
    [Column("ProductID")]
    public SqlIntColumn ProductID { get; set; } = default!;
    [Column("ProductName")]
    public SqlStringColumn ProductName { get; set; } = default!;
    [Column("SupplierID")]
    public SqlIntColumn SupplierID { get; set; } = default!;
    [Column("CategoryID")]
    public SqlIntColumn CategoryID { get; set; } = default!;
    [Column("QuantityPerUnit")]
    public SqlStringColumn QuantityPerUnit { get; set; } = default!;
    [Column("UnitPrice")]
    public SqlDecimalColumn UnitPrice { get; set; } = default!;
    [Column("UnitsInStock")]
    public SqlIntColumn UnitsInStock { get; set; } = default!;
    [Column("UnitsOnOrder")]
    public SqlIntColumn UnitsOnOrder { get; set; } = default!;
    [Column("ReorderLevel")]
    public SqlIntColumn ReorderLevel { get; set; } = default!;
    [Column("Discontinued")]
    public SqlBoolColumn Discontinued { get; set; } = default!;
}

public class OrdersTable() : SqlTable("Orders")
{
    [Column("OrderID")]
    public SqlIntColumn OrderID { get; set; } = default!;
    [Column("CustomerID")]
    public SqlStringColumn CustomerID { get; set; } = default!;
    [Column("EmployeeID")]
    public SqlIntColumn EmployeeID { get; set; } = default!;
    [Column("OrderDate")]
    public SqlDateTimeColumn OrderDate { get; set; } = default!;
    [Column("RequiredDate")]
    public SqlDateTimeColumn RequiredDate { get; set; } = default!;
    [Column("ShippedDate")]
    public SqlDateTimeColumn ShippedDate { get; set; } = default!;
    [Column("ShipVia")]
    public SqlIntColumn ShipVia { get; set; } = default!;
    [Column("Freight")]
    public SqlDecimalColumn Freight { get; set; } = default!;
    [Column("ShipName")]
    public SqlStringColumn ShipName { get; set; } = default!;
    [Column("ShipAddress")]
    public SqlStringColumn ShipAddress { get; set; } = default!;
    [Column("ShipCity")]
    public SqlStringColumn ShipCity { get; set; } = default!;
    [Column("ShipRegion")]
    public SqlStringColumn ShipRegion { get; set; } = default!;
    [Column("ShipPostalCode")]
    public SqlStringColumn ShipPostalCode { get; set; } = default!;
    [Column("ShipCountry")]
    public SqlStringColumn ShipCountry { get; set; } = default!;
}

public class OrderDetailsTable() : SqlTable("OrderDetails")
{
    [Column("OrderID")]
    public SqlIntColumn OrderID { get; set; } = default!;
    [Column("ProductID")]
    public SqlIntColumn ProductID { get; set; } = default!;
    [Column("UnitPrice")]
    public SqlDecimalColumn UnitPrice { get; set; } = default!;
    [Column("Quantity")]
    public SqlIntColumn Quantity { get; set; } = default!;
    [Column("Discount")]
    public SqlDecimalColumn Discount { get; set; } = default!;
}

public class EmployeesTable() : SqlTable("Employees")
{
    [Column("EmployeeID")]
    public SqlIntColumn EmployeeID { get; set; } = default!;
    [Column("LastName")]
    public SqlStringColumn LastName { get; set; } = default!;
    [Column("FirstName")]
    public SqlStringColumn FirstName { get; set; } = default!;
    [Column("Title")]
    public SqlStringColumn Title { get; set; } = default!;
    [Column("TitleOfCourtesy")]
    public SqlStringColumn TitleOfCourtesy { get; set; } = default!;
    [Column("BirthDate")]
    public SqlDateTimeColumn BirthDate { get; set; } = default!;
    [Column("HireDate")]
    public SqlDateTimeColumn HireDate { get; set; } = default!;
    [Column("Address")]
    public SqlStringColumn Address { get; set; } = default!;
    [Column("City")]
    public SqlStringColumn City { get; set; } = default!;
    [Column("Region")]
    public SqlStringColumn Region { get; set; } = default!;
    [Column("PostalCode")]
    public SqlStringColumn PostalCode { get; set; } = default!;
    [Column("Country")]
    public SqlStringColumn Country { get; set; } = default!;
    [Column("HomePhone")]
    public SqlStringColumn HomePhone { get; set; } = default!;
    [Column("Extension")]
    public SqlStringColumn Extension { get; set; } = default!;
    [Column("ReportsTo")]
    public SqlIntColumn ReportsTo { get; set; } = default!;
}

public class CategoriesTable() : SqlTable("Categories")
{
    [Column("CategoryID")]
    public SqlIntColumn CategoryID { get; set; } = default!;
    [Column("CategoryName")]
    public SqlStringColumn CategoryName { get; set; } = default!;
    [Column("Description")]
    public SqlStringColumn Description { get; set; } = default!;
}

public class SuppliersTable() : SqlTable("Suppliers")
{
    [Column("SupplierID")]
    public SqlIntColumn SupplierID { get; set; } = default!;
    [Column("CompanyName")]
    public SqlStringColumn CompanyName { get; set; } = default!;
    [Column("ContactName")]
    public SqlStringColumn ContactName { get; set; } = default!;
    [Column("ContactTitle")]
    public SqlStringColumn ContactTitle { get; set; } = default!;
    [Column("Address")]
    public SqlStringColumn Address { get; set; } = default!;
    [Column("City")]
    public SqlStringColumn City { get; set; } = default!;
    [Column("Region")]
    public SqlStringColumn Region { get; set; } = default!;
    [Column("PostalCode")]
    public SqlStringColumn PostalCode { get; set; } = default!;
    [Column("Country")]
    public SqlStringColumn Country { get; set; } = default!;
    [Column("Phone")]
    public SqlStringColumn Phone { get; set; } = default!;
    [Column("Fax")]
    public SqlStringColumn Fax { get; set; } = default!;
    [Column("HomePage")]
    public SqlStringColumn HomePage { get; set; } = default!;
}

// Database instance with static table instances
public static class NorthwindDb
{
    public static CustomersTable Customers { get; } = new();
    public static ProductsTable Products { get; } = new();
    public static OrdersTable Orders { get; } = new();
    public static OrderDetailsTable OrderDetails { get; } = new();
    public static EmployeesTable Employees { get; } = new();
    public static CategoriesTable Categories { get; } = new();
    public static SuppliersTable Suppliers { get; } = new();
}

// Data transfer objects for results
public class Customer
{
    public string CustomerID { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactTitle { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
}

public class Product
{
    public int ProductID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int? SupplierID { get; set; }
    public int? CategoryID { get; set; }
    public string QuantityPerUnit { get; set; } = string.Empty;
    public decimal? UnitPrice { get; set; }
    public int? UnitsInStock { get; set; }
    public int? UnitsOnOrder { get; set; }
    public int? ReorderLevel { get; set; }
    public bool Discontinued { get; set; }
    
    // Navigation properties for display
    public string CategoryName { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
}

public class Order
{
    public int OrderID { get; set; }
    public string CustomerID { get; set; } = string.Empty;
    public int? EmployeeID { get; set; }
    public DateTime? OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public int? ShipVia { get; set; }
    public decimal? Freight { get; set; }
    public string ShipName { get; set; } = string.Empty;
    public string ShipAddress { get; set; } = string.Empty;
    public string ShipCity { get; set; } = string.Empty;
    public string ShipRegion { get; set; } = string.Empty;
    public string ShipPostalCode { get; set; } = string.Empty;
    public string ShipCountry { get; set; } = string.Empty;
    
    // Navigation properties
    public string CustomerName { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class OrderDetail
{
    public int OrderID { get; set; }
    public int ProductID { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    
    // Navigation properties
    public string ProductName { get; set; } = string.Empty;
    public decimal LineTotal { get; set; }
}

public class Employee
{
    public int EmployeeID { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string TitleOfCourtesy { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public DateTime? HireDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string HomePhone { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public int? ReportsTo { get; set; }
    
    public string FullName => $"{FirstName} {LastName}";
    public string ManagerName { get; set; } = string.Empty;
}

public class Category
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class Supplier
{
    public int SupplierID { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactTitle { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    public string HomePage { get; set; } = string.Empty;
}
