using Northwind.RetroConsole.Data;
using Northwind.RetroConsole.Services;
using Northwind.RetroConsole.UI;
using Northwind.RetroConsole.UI.Screens;
using Spectre.Console;
using System.Globalization;

namespace Northwind.RetroConsole;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Set invariant culture to ensure consistent decimal parsing
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            
            // Initialize the classic DOS theme
            DosTheme.InitializeConsole();
            
            // Show splash screen
            ShowSplashScreen();
            
            // Initialize database connection
            var connectionString = DatabaseSetup.GetConnectionString();
            using var database = new DatabaseConnection(connectionString);
            
            // Initialize services
            var customerService = new CustomerService(database);
            var productService = new ProductService(database);
            var orderService = new OrderService(database);
            
            // Initialize screens
            var customerScreen = new CustomerScreen(customerService, orderService);
            var productScreen = new ProductScreen(productService);
            var orderScreen = new OrderScreen(orderService);
            
            // Main application loop
            RunMainLoop(customerScreen, productScreen, orderScreen);
        }
        catch (Exception ex)
        {
            DosTheme.InitializeConsole();
            DosUI.ShowError("SYSTEM ERROR", $"Application failed to start:\n{ex.Message}");
        }
        finally
        {
            DosTheme.RestoreConsole();
        }
    }
    
    private static void ShowSplashScreen()
    {
        var splashContent = @"
██████╗  █████╗ ██╗     ██╗      █████╗ ██████╗ ██╗███╗   ██╗
██╔══██╗██╔══██╗██║     ██║     ██╔══██╗██╔══██╗██║████╗  ██║
██████╔╝███████║██║     ██║     ███████║██║  ██║██║██╔██╗ ██║
██╔═══╝ ██╔══██║██║     ██║     ██╔══██║██║  ██║██║██║╚██╗██║
██║     ██║  ██║███████╗███████╗██║  ██║██████╔╝██║██║ ╚████║
╚═╝     ╚═╝  ╚═╝╚══════╝╚══════╝╚═╝  ╚═╝╚═════╝ ╚═╝╚═╝  ╚═══╝
                                                             
              ███████╗██╗  ██╗ ██████╗ ██████╗              
              ██╔════╝██║  ██║██╔═══██╗██╔══██╗             
              ███████╗███████║██║   ██║██████╔╝             
              ╚════██║██╔══██║██║   ██║██╔═══╝              
              ███████║██║  ██║╚██████╔╝██║                  
              ╚══════╝╚═╝  ╚═╝ ╚═════╝ ╚═╝                  

    MANAGEMENT SYSTEM v1.0 - Powered by TypedSqlBuilder

    A retro demonstration of modern type-safe SQL building
      
      
Loading system components...

Press any key to continue...";
        
        var panel = DosUI.CreateDialog("PALLADIN SHOP SYSTEM", splashContent);
        DosUI.ShowCenteredDialogUp(panel);
        Console.ReadKey(true);
    }
    
    private static void RunMainLoop(CustomerScreen customerScreen, ProductScreen productScreen, OrderScreen orderScreen)
    {
        while (true)
        {
            try
            {
                var selection = DosMenu.ShowMainMenu();
                
                switch (selection)
                {
                    case "CUSTOMER RECORDS":
                        try
                        {
                            customerScreen.Show();
                        }
                        catch (OperationCanceledException)
                        {
                            // User pressed ESC in submenu, just return to main menu
                        }
                        break;
                        
                    case "PRODUCT CATALOG":
                        try
                        {
                            productScreen.Show();
                        }
                        catch (OperationCanceledException)
                        {
                            // User pressed ESC in submenu, just return to main menu
                        }
                        break;
                        
                    case "ORDER MANAGEMENT":
                        try
                        {
                            orderScreen.Show();
                        }
                        catch (OperationCanceledException)
                        {
                            // User pressed ESC in submenu, just return to main menu
                        }
                        break;
                        
                    case "EMPLOYEE DATABASE":
                        ShowComingSoon("EMPLOYEE DATABASE");
                        break;
                        
                    case "SALES REPORTS":
                        ShowComingSoon("SALES REPORTS");
                        break;
                        
                    case "INVENTORY STATUS":
                        ShowComingSoon("INVENTORY STATUS");
                        break;
                        
                    case "SYSTEM UTILITIES":
                        try
                        {
                            ShowSystemUtilities();
                        }
                        catch (OperationCanceledException)
                        {
                            // User pressed ESC in submenu, just return to main menu
                        }
                        break;
                        
                    case "EXIT PROGRAM":
                        if (ConfirmExit())
                            return;
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                // ESC pressed on main menu - confirm exit
                if (ConfirmExit())
                    return;
            }
            catch (Exception ex)
            {
                DosUI.ShowError("APPLICATION ERROR", ex.Message);
            }
        }
    }
    
    private static void ShowComingSoon(string feature)
    {
        var content = $"The {feature} module is under development.\n\n" +
                     $"Coming soon:\n" +
                     $"• Advanced reporting capabilities\n" +
                     $"• Real-time data analysis\n" +
                     $"• Export to various formats\n" +
                     $"• Custom query builder\n\n" +
                     $"Stay tuned for updates!\n\n" +
                     $"Press any key to continue...";
        
        var dialog = DosUI.CreateDialog($"{feature} - COMING SOON", content);
        DosUI.ShowCenteredDialog(dialog);
        Console.ReadKey(true);
    }
    
    private static void ShowSystemUtilities()
    {
        var options = new[]
        {
            new MenuOption { Key = "about", Display = "F1", Description = "ABOUT THIS SYSTEM" },
            new MenuOption { Key = "database", Display = "F2", Description = "DATABASE INFO" },
            new MenuOption { Key = "help", Display = "F3", Description = "HELP & SUPPORT" },
            new MenuOption { Key = "back", Display = "ESC", Description = "BACK TO MAIN MENU" }
        };
        
        while (true)
        {
            try
            {
                var selection = DosMenu.ShowMenuWithOptions("SYSTEM UTILITIES", options);
                
                switch (selection.Key)
                {
                    case "about":
                        ShowAboutDialog();
                        break;
                    case "database":
                        ShowDatabaseInfo();
                        break;
                    case "help":
                        ShowHelpDialog();
                        break;
                    case "back":
                        return;
                }
            }
            catch (OperationCanceledException)
            {
                // User pressed ESC, return to main menu
                return;
            }
        }
    }
    
    private static void ShowAboutDialog()
    {
        var content = $"PALLADIN SHOP\n" +
                     $"Management System v1.0\n\n" +
                     $"Built with:\n" +
                     $"• TypedSqlBuilder - Type-safe SQL generation\n" +
                     $"• Spectre.Console - Rich console UI\n" +
                     $"• SQLite - Embedded database\n" +
                     $"• .NET 8.0 - Modern runtime\n\n" +
                     $"A demonstration of retro-style console applications\n" +
                     $"with modern development practices.\n\n" +
                     $"© 2025 TypedSqlBuilder Project\n\n" +
                     $"Press any key to continue...";
        
        var dialog = DosUI.CreateDialog("ABOUT NORTHWIND SYSTEM", content);
        DosUI.ShowCenteredDialog(dialog);
        Console.ReadKey(true);
    }
    
    private static void ShowDatabaseInfo()
    {
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "northwind.db");
        var fileInfo = new FileInfo(dbPath);
        
        var content = $"Database File:    {dbPath}\n" +
                     $"File Size:        {fileInfo.Length / 1024:N0} KB\n" +
                     $"Last Modified:    {fileInfo.LastWriteTime:MMM dd, yyyy HH:mm}\n" +
                     $"Database Type:    SQLite 3.x\n" +
                     $"Connection:       Active\n\n" +
                     $"Sample Data Included:\n" +
                     $"• 8 Categories\n" +
                     $"• 8 Products\n" +
                     $"• 8 Customers\n" +
                     $"• 8 Employees\n" +
                     $"• 5 Orders with details\n\n" +
                     $"Database ready for demonstration!\n\n" +
                     $"Press any key to continue...";
        
        var dialog = DosUI.CreateDialog("DATABASE INFORMATION", content);
        DosUI.ShowCenteredDialog(dialog);
        Console.ReadKey(true);
    }
    
    private static void ShowHelpDialog()
    {
        var content = $"NORTHWIND SYSTEM HELP\n\n" +
                     $"Navigation:\n" +
                     $"• Use ↑↓ arrow keys to navigate menus\n" +
                     $"• Press ENTER to select highlighted item\n" +
                     $"• Press ESC to go back or cancel\n" +
                     $"• Function keys (F1-F8) for shortcuts\n\n" +
                     $"Features Demonstrated:\n" +
                     $"• Customer management (CRUD operations)\n" +
                     $"• Product catalog browsing\n" +
                     $"• Order processing and tracking\n" +
                     $"• Complex SQL queries with joins\n" +
                     $"• Type-safe database operations\n\n" +
                     $"This demo showcases TypedSqlBuilder capabilities\n" +
                     $"in a classic MS-DOS style interface.\n\n" +
                     $"Press any key to continue...";
        
        var dialog = DosUI.CreateDialog("HELP & SUPPORT", content);
        DosUI.ShowCenteredDialog(dialog);
        Console.ReadKey(true);
    }
    
    private static bool ConfirmExit()
    {
        return DosUI.Confirm("EXIT PROGRAM", "Are you sure you want to exit Northwind System?");
    }
}
