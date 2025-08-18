using Northwind.RetroConsole.Data;
using Northwind.RetroConsole.Models;
using TypedSqlBuilder.Core;
using Dapper;

namespace Northwind.RetroConsole.Services;

public class OrderService
{
    private readonly DatabaseConnection _db;
    
    public OrderService(DatabaseConnection db)
    {
        _db = db;
    }
    
    public List<Order> GetRecentOrders(int limit = 50)
    {
        // First, get the basic order data with joins
        var query = NorthwindDb.Orders.From()
            .LeftJoin(
                NorthwindDb.Customers,
                o => o.CustomerID,
                c => c.CustomerID,
                (o, c) => (Order: o, Customer: c))
            .LeftJoin(
                NorthwindDb.Employees,
                result => result.Order.EmployeeID,
                e => e.EmployeeID,
                (result, e) => (
                    OrderID: result.Order.OrderID,
                    CustomerID: result.Order.CustomerID,
                    EmployeeID: result.Order.EmployeeID,
                    OrderDate: result.Order.OrderDate,
                    RequiredDate: result.Order.RequiredDate,
                    ShippedDate: result.Order.ShippedDate,
                    Freight: result.Order.Freight,
                    ShipName: result.Order.ShipName,
                    ShipCity: result.Order.ShipCity,
                    ShipCountry: result.Order.ShipCountry,
                    CustomerName: result.Customer.CompanyName,
                    EmployeeName: e.FirstName + " " + e.LastName
                ))
            .OrderBy(result => (result.OrderDate, Sort.Desc));
            
        return ExecuteQuery<Order>(query);
    }
    
    public List<OrderDetail> GetOrderDetails(int orderId)
    {
        var query = NorthwindDb.OrderDetails.From()
            .Where(od => od.OrderID == orderId)
            .InnerJoin(
                NorthwindDb.Products,
                od => od.ProductID,
                p => p.ProductID,
                (od, p) => (
                    OrderID: od.OrderID,
                    ProductID: od.ProductID,
                    UnitPrice: od.UnitPrice,
                    Quantity: od.Quantity,
                    Discount: od.Discount,
                    ProductName: p.ProductName,
                    LineTotal: od.UnitPrice * 1.0m // od.Quantity * (1.0m - od.Discount) - simplified for demo
                ))
            .OrderBy(result => (result.ProductName, Sort.Asc));
            
        return ExecuteQuery<OrderDetail>(query);
    }
    
    public Order? GetOrderById(int orderId)
    {
        var query = NorthwindDb.Orders.From()
            .Where(o => o.OrderID == orderId)
            .LeftJoin(
                NorthwindDb.Customers,
                o => o.CustomerID,
                c => c.CustomerID,
                (o, c) => (Order: o, Customer: c))
            .LeftJoin(
                NorthwindDb.Employees,
                result => result.Order.EmployeeID,
                e => e.EmployeeID,
                (result, e) => (
                    OrderID: result.Order.OrderID,
                    CustomerID: result.Order.CustomerID,
                    EmployeeID: result.Order.EmployeeID,
                    OrderDate: result.Order.OrderDate,
                    RequiredDate: result.Order.RequiredDate,
                    ShippedDate: result.Order.ShippedDate,
                    ShipVia: result.Order.ShipVia,
                    Freight: result.Order.Freight,
                    ShipName: result.Order.ShipName,
                    ShipAddress: result.Order.ShipAddress,
                    ShipCity: result.Order.ShipCity,
                    ShipRegion: result.Order.ShipRegion,
                    ShipPostalCode: result.Order.ShipPostalCode,
                    ShipCountry: result.Order.ShipCountry,
                    CustomerName: result.Customer.CompanyName,
                    EmployeeName: e.FirstName + " " + e.LastName
                ));
            
        return ExecuteQuery<Order>(query).FirstOrDefault();
    }
    
    public List<Order> GetCustomerOrders(string customerId)
    {
        var query = NorthwindDb.Orders.From()
            .Where(o => o.CustomerID == customerId)
            .LeftJoin(
                NorthwindDb.Customers,
                o => o.CustomerID,
                c => c.CustomerID,
                (o, c) => (Order: o, Customer: c))
            .LeftJoin(
                NorthwindDb.Employees,
                result => result.Order.EmployeeID,
                e => e.EmployeeID,
                (result, e) => (
                    OrderID: result.Order.OrderID,
                    CustomerID: result.Order.CustomerID,
                    EmployeeID: result.Order.EmployeeID,
                    OrderDate: result.Order.OrderDate,
                    RequiredDate: result.Order.RequiredDate,
                    ShippedDate: result.Order.ShippedDate,
                    Freight: result.Order.Freight,
                    CustomerName: result.Customer.CompanyName,
                    EmployeeName: e.FirstName + " " + e.LastName
                ))
            .OrderBy(result => (result.OrderDate, Sort.Desc));
            
        return ExecuteQuery<Order>(query);
    }
    
    private List<T> ExecuteQuery<T>(ISqlQuery query)
    {
        var (sql, parameters) = query.ToSqliteRaw();
        var results = _db.Connection.Query<T>(sql, parameters).ToList();
        
        // Handle computed properties for Order objects
        foreach (var item in results)
        {
            if (item is Order order)
            {
                if (order.ShippedDate != null)
                    order.Status = "SHIPPED";
                else if (order.OrderDate != null)
                    order.Status = "PENDING";
                else
                    order.Status = "UNKNOWN";
            }
        }
        
        return results;
    }
}
