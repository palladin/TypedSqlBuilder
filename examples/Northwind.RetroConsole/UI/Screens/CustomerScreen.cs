using Northwind.RetroConsole.Models;
using Northwind.RetroConsole.Services;
using Northwind.RetroConsole.UI;
using Spectre.Console;

namespace Northwind.RetroConsole.UI.Screens;

public class CustomerScreen
{
    private readonly CustomerService _customerService;
    private readonly OrderService _orderService;
    
    public CustomerScreen(CustomerService customerService, OrderService orderService)
    {
        _customerService = customerService;
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
                    new MenuOption { Key = "list", Display = "F1", Description = "VIEW ALL CUSTOMERS" },
                    new MenuOption { Key = "search", Display = "F2", Description = "SEARCH CUSTOMERS" },
                    new MenuOption { Key = "add", Display = "F3", Description = "ADD NEW CUSTOMER" },
                    new MenuOption { Key = "back", Display = "ESC", Description = "BACK TO MAIN MENU" }
                };
                
                var selection = DosMenu.ShowMenuWithOptions("CUSTOMER MANAGEMENT", options);
                
                switch (selection.Key)
                {
                    case "list":
                        try
                        {
                            ShowCustomerList();
                        }
                        catch (OperationCanceledException)
                        {
                            // User pressed ESC, return to customer menu
                        }
                        break;
                    case "search":
                        try
                        {
                            SearchCustomers();
                        }
                        catch (OperationCanceledException)
                        {
                            // User pressed ESC, return to customer menu
                        }
                        break;
                    case "add":
                        try
                        {
                            AddCustomer();
                        }
                        catch (OperationCanceledException)
                        {
                            // User pressed ESC, return to customer menu
                        }
                        break;
                    case "back":
                        return;
                }
            }
            catch (OperationCanceledException)
            {
                // ESC pressed on main customer menu, return to main menu
                return;
            }
        }
    }
    
    private void ShowCustomerList()
    {
        try
        {
            // Show loading message
            var loadingDialog = DosUI.CreateDialog("CUSTOMER DATABASE", "LOADING CUSTOMER RECORDS...\n\nPlease wait...");
            DosUI.ShowCenteredDialog(loadingDialog);
            
            var customers = _customerService.GetAllCustomers().ToList();
            
            if (!customers.Any())
            {
                DosUI.ShowError("NO DATA", "No customers found in database.");
                return;
            }
            
            while (true)
            {
                try
                {
                    var selectedCustomer = DosMenu.SelectFromList(
                        "CUSTOMER DATABASE",
                        customers,
                        customer => $"  {customer.CustomerID,-6} {customer.CompanyName,-25} {customer.ContactName,-20} {customer.City,-15} {customer.Country}",
                        "↑↓ Navigate  ENTER=Details  F3=Orders  ESC=Back"
                    );
                    
                    ShowCustomerDetails(selectedCustomer);
                }
                catch (OperationCanceledException)
                {
                    // User pressed ESC, break out of the loop to go back
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            DosUI.ShowError("DATABASE ERROR", ex.Message);
        }
    }
    
    private void ShowCustomerDetails(Customer customer)
    {
        DosTheme.InitializeConsole();
        
        var content = $"Customer ID:     {customer.CustomerID}\n" +
                     $"Company Name:    {customer.CompanyName}\n" +
                     $"Contact Name:    {customer.ContactName}\n" +
                     $"Contact Title:   {customer.ContactTitle}\n" +
                     $"Address:         {customer.Address}\n" +
                     $"City:            {customer.City}\n" +
                     $"Region:          {customer.Region}\n" +
                     $"Postal Code:     {customer.PostalCode}\n" +
                     $"Country:         {customer.Country}\n" +
                     $"Phone:           {customer.Phone}\n" +
                     $"Fax:             {customer.Fax}\n\n" +
                     $"────────────────────────────────────────\n" +
                     $"F1=Edit  F2=Delete  F3=Orders  ESC=Back";
        
        var dialog = DosUI.CreateDialog("CUSTOMER DETAILS", content);
        DosUI.ShowCenteredDialog(dialog);
        
        while (true)
        {
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.F1:
                    EditCustomer(customer);
                    return;
                case ConsoleKey.F2:
                    if (DosUI.Confirm("DELETE CUSTOMER", $"Are you sure you want to delete {customer.CompanyName}?"))
                    {
                        DeleteCustomer(customer);
                        return;
                    }
                    break;
                case ConsoleKey.F3:
                    ShowCustomerOrders(customer);
                    return;
                case ConsoleKey.Escape:
                    return;
            }
        }
    }
    
    private void ShowCustomerOrders(Customer customer)
    {
        try
        {
            DosTheme.InitializeConsole();
            
            var orders = _orderService.GetCustomerOrders(customer.CustomerID);
            
            if (!orders.Any())
            {
                DosUI.ShowError("NO ORDERS", $"No orders found for {customer.CompanyName}.");
                return;
            }
            
            var selectedOrder = DosMenu.SelectFromList(
                $"ORDERS FOR {customer.CompanyName}",
                orders,
                order => $"  {order.OrderID,-8} {order.OrderDate:MM/dd/yy,-10} {order.Total,10:C} {order.Status}",
                "↑↓ Navigate  ENTER=Details  ESC=Back"
            );
            
            ShowOrderDetails(selectedOrder);
        }
        catch (Exception ex)
        {
            DosUI.ShowError("DATABASE ERROR", ex.Message);
        }
    }
    
    private void ShowOrderDetails(Order order)
    {
        DosTheme.InitializeConsole();
        
        var details = _orderService.GetOrderDetails(order.OrderID);
        
        var content = $"Order ID:        {order.OrderID}\n" +
                     $"Customer:        {order.CustomerName}\n" +
                     $"Employee:        {order.EmployeeName}\n" +
                     $"Order Date:      {order.OrderDate:MM/dd/yyyy}\n" +
                     $"Required Date:   {order.RequiredDate:MM/dd/yyyy}\n" +
                     $"Shipped Date:    {(order.ShippedDate?.ToString("MM/dd/yyyy") ?? "Not Shipped")}\n" +
                     $"Status:          {order.Status}\n" +
                     $"Freight:         {order.Freight:C}\n\n" +
                     $"───────────── ORDER DETAILS ─────────────\n";
        
        foreach (var detail in details)
        {
            content += $"{detail.ProductName,-20} {detail.Quantity,3} x {detail.UnitPrice,8:C} = {detail.LineTotal,10:C}\n";
        }
        
        content += $"\n                                    Total: {order.Total:C}";
        
        var dialog = DosUI.CreateDialog("ORDER DETAILS", content);
        DosUI.ShowCenteredDialog(dialog);
        
        Console.ReadKey(true);
    }
    
    private void SearchCustomers()
    {
        var fields = new List<FormField>
        {
            new() { Key = "CustomerID", Label = "Customer ID" },
            new() { Key = "CompanyName", Label = "Company Name" },
            new() { Key = "ContactName", Label = "Contact Name" },
            new() { Key = "ContactTitle", Label = "Contact Title" },
            new() { Key = "City", Label = "City" },
            new() { Key = "Country", Label = "Country" },
            new() { Key = "Phone", Label = "Phone" }
        };
        
        var values = DosUI.ShowFormDialog("SEARCH CUSTOMERS BY EXAMPLE", fields);
        
        if (!values.Any())
            return;
        
        // Filter out empty search criteria
        var searchCriteria = values.Where(kv => !string.IsNullOrWhiteSpace(kv.Value)).ToList();
        
        if (!searchCriteria.Any())
        {
            DosUI.ShowError("SEARCH ERROR", "Please enter at least one search criterion.");
            return;
        }
            
        try
        {
            var customers = _customerService.SearchCustomersByExample(searchCriteria);
            
            if (!customers.Any())
            {
                var criteriaText = string.Join(", ", searchCriteria.Select(c => $"{c.Key}='{c.Value}'"));
                DosUI.ShowError("NO RESULTS", $"No customers found matching: {criteriaText}");
                return;
            }
            
            var selectedCustomer = DosMenu.SelectFromList(
                $"SEARCH RESULTS ({customers.Count} found)",
                customers,
                customer => $"  {customer.CustomerID,-6} {customer.CompanyName,-25} {customer.ContactName,-20} {customer.City,-15} {customer.Country}",
                "↑↓ Navigate  ENTER=Details  ESC=Back"
            );
            
            ShowCustomerDetails(selectedCustomer);
        }
        catch (OperationCanceledException)
        {
            // User pressed ESC, just return to go back to menu
            return;
        }
        catch (Exception ex)
        {
            DosUI.ShowError("SEARCH ERROR", ex.Message);
        }
    }
    
    private void AddCustomer()
    {
        try
        {
            var fields = new List<FormField>
            {
                new() { Key = "CustomerID", Label = "Customer ID (5 chars)", Required = true },
                new() { Key = "CompanyName", Label = "Company Name", Required = true },
                new() { Key = "ContactName", Label = "Contact Name" },
                new() { Key = "ContactTitle", Label = "Contact Title" },
                new() { Key = "Address", Label = "Address" },
                new() { Key = "City", Label = "City" },
                new() { Key = "Region", Label = "Region" },
                new() { Key = "PostalCode", Label = "Postal Code" },
                new() { Key = "Country", Label = "Country" },
                new() { Key = "Phone", Label = "Phone" },
                new() { Key = "Fax", Label = "Fax" }
            };
            
            var values = DosUI.ShowFormDialog("ADD NEW CUSTOMER", fields);
            
            if (!values.Any()) return; // User cancelled
            
            var customer = new Customer
            {
                CustomerID = values["CustomerID"].ToUpper().PadRight(5)[..5],
                CompanyName = values["CompanyName"],
                ContactName = values["ContactName"],
                ContactTitle = values["ContactTitle"],
                Address = values["Address"],
                City = values["City"],
                Region = values["Region"],
                PostalCode = values["PostalCode"],
                Country = values["Country"],
                Phone = values["Phone"],
                Fax = values["Fax"]
            };
            
            if (DosUI.Confirm("ADD CUSTOMER", $"Add customer {customer.CompanyName}?"))
            {
                _customerService.AddCustomer(customer);
                
                var successDialog = DosUI.CreateDialog("SUCCESS", 
                    $"Customer {customer.CompanyName} added successfully!");
                DosUI.ShowCenteredDialog(successDialog);
                Console.ReadKey(true);
            }
        }
        catch (Exception ex)
        {
            DosUI.ShowError("ADD CUSTOMER ERROR", ex.Message);
        }
    }
    
    private void EditCustomer(Customer customer)
    {
        // Implementation would be similar to AddCustomer but with existing values
        DosUI.ShowError("NOT IMPLEMENTED", "Edit customer feature coming soon!");
    }
    
    private void DeleteCustomer(Customer customer)
    {
        try
        {
            _customerService.DeleteCustomer(customer.CustomerID);
            
            var successDialog = DosUI.CreateDialog("SUCCESS", 
                $"Customer {customer.CompanyName} deleted successfully!");
            DosUI.ShowCenteredDialog(successDialog);
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            DosUI.ShowError("DELETE ERROR", ex.Message);
        }
    }
}
