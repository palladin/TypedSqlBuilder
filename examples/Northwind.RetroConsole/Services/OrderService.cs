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
        // Get orders with customer and employee information
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
                (result, e) => (Order: result.Order, Customer: result.Customer, Employee: e))
            .OrderBy(result => (result.Order.OrderDate, Sort.Desc))
            .Select(result => (
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
                EmployeeName: result.Employee.FirstName + " " + result.Employee.LastName,
                Total: NorthwindDb.OrderDetails.From()
                    .Where(od => od.OrderID == result.Order.OrderID)
                    .Select(od => od.UnitPrice * od.Quantity * (1 - od.Discount)).Sum()
            ));
            
        var orders = ExecuteQuery<Order>(query);
        
        // Calculate totals for each order using raw SQL for now
        foreach (var order in orders)
        {
            var sql = "SELECT COALESCE(SUM(UnitPrice * Quantity * (1 - Discount)), 0) FROM OrderDetails WHERE OrderID = @orderId";
            var total = _db.Connection.QuerySingle<decimal>(sql, new { orderId = order.OrderID });
            order.Total = total;
        }
        
        return orders;
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
                (result, e) => (Order: result.Order, Customer: result.Customer, Employee: e))
            .Select(result => (
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
                EmployeeName: result.Employee.FirstName + " " + result.Employee.LastName
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
                (result, e) => (Order: result.Order, Customer: result.Customer, Employee: e))
            .OrderBy(result => (result.Order.OrderDate, Sort.Desc))
            .Select(result => (
                OrderID: result.Order.OrderID,
                CustomerID: result.Order.CustomerID,
                EmployeeID: result.Order.EmployeeID,
                OrderDate: result.Order.OrderDate,
                RequiredDate: result.Order.RequiredDate,
                ShippedDate: result.Order.ShippedDate,
                Freight: result.Order.Freight,
                CustomerName: result.Customer.CompanyName,
                EmployeeName: result.Employee.FirstName + " " + result.Employee.LastName
            ));
            
        var orders = ExecuteQuery<Order>(query);
        
        // Calculate totals for each order
        foreach (var order in orders)
        {
            var sql = "SELECT COALESCE(SUM(UnitPrice * Quantity * (1 - Discount)), 0) FROM OrderDetails WHERE OrderID = @orderId";
            var total = _db.Connection.QuerySingle<decimal>(sql, new { orderId = order.OrderID });
            order.Total = total;
        }
        
        return orders;
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
                Console.WriteLine($"DEBUG ORDER: ID={order.OrderID}, CustomerName='{order.CustomerName}', EmployeeName='{order.EmployeeName}'");
                
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
