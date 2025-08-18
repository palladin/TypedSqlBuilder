using Northwind.RetroConsole.Models;
using Northwind.RetroConsole.Services;
using Northwind.RetroConsole.UI;
using Spectre.Console;

namespace Northwind.RetroConsole.UI.Screens;

public class OrderScreen
{
    private readonly OrderService _orderService;
    
    public OrderScreen(OrderService orderService)
    {
        _orderService = orderService;
    }
    
    public void Show()
    {
        while (true)
        {
            try
            {
                var options = new[]
                {
                    new MenuOption { Key = "recent", Display = "F1", Description = "RECENT ORDERS" },
                    new MenuOption { Key = "search", Display = "F2", Description = "SEARCH ORDERS" },
                    new MenuOption { Key = "pending", Display = "F3", Description = "PENDING ORDERS" },
                    new MenuOption { Key = "shipped", Display = "F4", Description = "SHIPPED ORDERS" },
                    new MenuOption { Key = "back", Display = "ESC", Description = "BACK TO MAIN MENU" }
                };
                
                var selection = DosMenu.ShowMenuWithOptions("ORDER MANAGEMENT", options);
                
                switch (selection.Key)
                {
                    case "recent":
                        try
                        {
                            ShowRecentOrders();
                        }
                        catch (OperationCanceledException)
                        {
                            // User pressed ESC, return to order menu
                        }
                        break;
                    case "search":
                        try
                        {
                            SearchOrders();
                        }
                        catch (OperationCanceledException)
                        {
                            // User pressed ESC, return to order menu
                        }
                        break;
                    case "pending":
                        try
                        {
                            ShowPendingOrders();
                        }
                        catch (OperationCanceledException)
                        {
                            // User pressed ESC, return to order menu
                        }
                        break;
                    case "shipped":
                        try
                        {
                            ShowShippedOrders();
                        }
                        catch (OperationCanceledException)
                        {
                            // User pressed ESC, return to order menu
                        }
                        break;
                    case "back":
                        return;
                }
            }
            catch (OperationCanceledException)
            {
                // ESC pressed on main order menu, return to main menu
                return;
            }
        }
    }
    
    private void ShowRecentOrders()
    {
        try
        {
            // Show loading message
            var loadingDialog = DosUI.CreateDialog("ORDER MANAGEMENT", "LOADING RECENT ORDERS...\n\nQUERYING ORDER DATABASE...\n\nPlease wait...");
            DosUI.ShowCenteredDialog(loadingDialog);
            
            var orders = _orderService.GetRecentOrders(50);
            
            if (!orders.Any())
            {
                DosUI.ShowError("NO DATA", "No orders found in database.");
                return;
            }
            
            var selectedOrder = DosMenu.SelectFromList(
                "RECENT ORDERS",
                orders,
                order => $"  {order.OrderID,-8} {order.CustomerName,-20} {order.OrderDate:MM/dd/yy,-10} {order.Total,10:C} {order.Status}",
                "↑↓ Navigate  ENTER=Details  ESC=Back"
            );
            
            ShowOrderDetails(selectedOrder);
        }
        catch (OperationCanceledException)
        {
            // User pressed ESC, just return to go back to menu
            return;
        }
        catch (Exception ex)
        {
            DosUI.ShowError("DATABASE ERROR", ex.Message);
        }
    }
    
    private void ShowOrderDetails(Order order)
    {
        try
        {
            // Get the complete order information including customer and employee details
            var fullOrder = _orderService.GetOrderById(order.OrderID);
            if (fullOrder == null)
            {
                DosUI.ShowError("ORDER ERROR", "Order not found in database.");
                return;
            }
            
            var orderDetails = _orderService.GetOrderDetails(fullOrder.OrderID);
            
            var content = $"Order #:         {fullOrder.OrderID}\n" +
                         $"Customer:        {fullOrder.CustomerName}\n" +
                         $"Employee:        {fullOrder.EmployeeName ?? "N/A"}\n" +
                         $"Order Date:      {fullOrder.OrderDate:MMM dd, yyyy}\n" +
                         $"Required Date:   {fullOrder.RequiredDate:MMM dd, yyyy}\n" +
                         $"Shipped Date:    {(fullOrder.ShippedDate?.ToString("MMM dd, yyyy") ?? "Not Shipped")}\n" +
                         $"Status:          {fullOrder.Status}\n" +
                         $"Freight:         {fullOrder.Freight:C}\n" +
                         $"Ship To:         {fullOrder.ShipName}\n" +
                         $"                 {fullOrder.ShipAddress}\n" +
                         $"                 {fullOrder.ShipCity}, {fullOrder.ShipCountry}\n\n" +
                         $"──────────────── ORDER ITEMS ────────────────\n";
            
            decimal totalAmount = 0;
            foreach (var detail in orderDetails)
            {
                var lineTotal = detail.UnitPrice * detail.Quantity * (decimal)(1 - detail.Discount);
                totalAmount += lineTotal;
                
                var discountText = detail.Discount > 0 ? $" ({detail.Discount:P0} disc)" : "";
                content += $"{detail.ProductName,-25} {detail.Quantity,3} x {detail.UnitPrice,8:C}{discountText} = {lineTotal,10:C}\n";
            }
            
            content += $"────────────────────────────────────────────\n";
            content += $"                              Subtotal: {totalAmount,10:C}\n";
            content += $"                               Freight: {fullOrder.Freight,10:C}\n";
            content += $"                                 TOTAL: {totalAmount + (fullOrder.Freight ?? 0),10:C}";
            
            var dialog = DosUI.CreateDialog($"ORDER #{fullOrder.OrderID}", content);
            DosUI.ShowCenteredDialog(dialog);
            
            
            while (true)
            {
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.F1:
                        PrintInvoice(order);
                        return;
                    case ConsoleKey.F2:
                        EditOrder(order);
                        return;
                    case ConsoleKey.Escape:
                        return;
                }
            }
        }
        catch (Exception ex)
        {
            DosUI.ShowError("ORDER DETAILS ERROR", ex.Message);
        }
    }
    
    private void PrintInvoice(Order order)
    {
        var content = $"INVOICE PRINTED TO PRINTER LPT1:\n\n" +
                     $"Order #{order.OrderID} sent to default printer.\n" +
                     $"Customer: {order.CustomerName}\n" +
                     $"Date: {DateTime.Now:MM/dd/yyyy HH:mm}\n\n" +
                     $"✓ Print job completed successfully!";
        
        var dialog = DosUI.CreateDialog("PRINT INVOICE", content);
        DosUI.ShowCenteredDialog(dialog);
        Console.ReadKey(true);
    }
    
    private void EditOrder(Order order)
    {
        DosUI.ShowError("NOT IMPLEMENTED", "Order editing feature coming soon!");
    }
    
    private void SearchOrders()
    {
        DosUI.ShowError("NOT IMPLEMENTED", "Order search feature coming soon!");
    }
    
    private void ShowPendingOrders()
    {
        try
        {
            var orders = _orderService.GetRecentOrders(100);
            var pendingOrders = orders.Where(o => o.Status == "PENDING").ToList();
            
            if (!pendingOrders.Any())
            {
                var dialog = DosUI.CreateDialog("PENDING ORDERS", 
                    "No pending orders found!\n\nAll orders have been processed.");
                DosUI.ShowCenteredDialog(dialog);
                Console.ReadKey(true);
                return;
            }
            
            var selectedOrder = DosMenu.SelectFromList(
                $"PENDING ORDERS ({pendingOrders.Count} ITEMS)",
                pendingOrders,
                order => $"  {order.OrderID,-8} {order.CustomerName,-20} {order.OrderDate:MM/dd/yy,-10} {order.Total,10:C} {order.Status}",
                "↑↓ Navigate  ENTER=Details  ESC=Back"
            );
            
            ShowOrderDetails(selectedOrder);
        }
        catch (Exception ex)
        {
            DosUI.ShowError("PENDING ORDERS ERROR", ex.Message);
        }
    }
    
    private void ShowShippedOrders()
    {
        try
        {
            var orders = _orderService.GetRecentOrders(100);
            var shippedOrders = orders.Where(o => o.Status == "SHIPPED").ToList();
            
            if (!shippedOrders.Any())
            {
                var dialog = DosUI.CreateDialog("RECENT SHIPMENTS", 
                    "No shipped orders found in recent history.");
                DosUI.ShowCenteredDialog(dialog);
                Console.ReadKey(true);
                return;
            }
            
            var selectedOrder = DosMenu.SelectFromList(
                $"SHIPPED ORDERS ({shippedOrders.Count} ITEMS)",
                shippedOrders,
                order => $"  {order.OrderID,-8} {order.CustomerName,-20} {order.ShippedDate:MM/dd/yy,-10} {order.Total,10:C} {order.Status}",
                "↑↓ Navigate  ENTER=Details  ESC=Back"
            );
            
            ShowOrderDetails(selectedOrder);
        }
        catch (Exception ex)
        {
            DosUI.ShowError("SHIPPED ORDERS ERROR", ex.Message);
        }
    }
}
