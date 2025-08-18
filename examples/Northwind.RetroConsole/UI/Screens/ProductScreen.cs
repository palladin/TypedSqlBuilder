using Northwind.RetroConsole.Models;
using Northwind.RetroConsole.Services;
using Northwind.RetroConsole.UI;
using Spectre.Console;

namespace Northwind.RetroConsole.UI.Screens;

public class ProductScreen
{
    private readonly ProductService _productService;
    
    public ProductScreen(ProductService productService)
    {
        _productService = productService;
    }
    
    public void Show()
    {
        while (true)
        {
            try
            {
                var options = new[]
                {
                    new MenuOption { Key = "list", Display = "F1", Description = "VIEW ALL PRODUCTS" },
                    new MenuOption { Key = "category", Display = "F2", Description = "BROWSE BY CATEGORY" },
                    new MenuOption { Key = "search", Display = "F3", Description = "SEARCH PRODUCTS" },
                    new MenuOption { Key = "lowstock", Display = "F4", Description = "LOW STOCK ALERT" },
                    new MenuOption { Key = "back", Display = "ESC", Description = "BACK TO MAIN MENU" }
                };
                
                var selection = DosMenu.ShowMenuWithOptions("PRODUCT CATALOG", options);
                
                switch (selection.Key)
                {
                    case "list":
                        try
                        {
                            ShowProductList();
                        }
                        catch (OperationCanceledException)
                        {
                            // User pressed ESC, return to product menu
                        }
                        break;
                    case "category":
                        try
                        {
                            BrowseByCategory();
                        }
                        catch (OperationCanceledException)
                        {
                            // User pressed ESC, return to product menu
                        }
                        break;
                    case "search":
                        try
                        {
                            SearchProducts();
                        }
                        catch (OperationCanceledException)
                        {
                            // User pressed ESC, return to product menu
                        }
                        break;
                    case "lowstock":
                        try
                        {
                            ShowLowStockProducts();
                        }
                        catch (OperationCanceledException)
                        {
                            // User pressed ESC, return to product menu
                        }
                        break;
                    case "back":
                        return;
                }
            }
            catch (OperationCanceledException)
            {
                // ESC pressed on main product menu, return to main menu
                return;
            }
        }
    }
    
    private void ShowProductList()
    {
        try
        {
            // Show loading message
            var loadingDialog = DosUI.CreateDialog("PRODUCT CATALOG", "LOADING PRODUCT RECORDS...\n\nPlease wait...");
            DosUI.ShowCenteredDialog(loadingDialog);
            
            var products = _productService.GetAllProducts();
            
            if (!products.Any())
            {
                DosUI.ShowError("NO DATA", "No products found in catalog.");
                return;
            }
            
            var selectedProduct = DosMenu.SelectFromList(
                "PRODUCT CATALOG",
                products,
                product => $"  {product.ProductID,-4} {product.ProductName,-25} {product.CategoryName,-15} {product.UnitPrice,8:C} {product.UnitsInStock,6}",
                "↑↓ Navigate  ENTER=Details  ESC=Back"
            );
            
            ShowProductDetails(selectedProduct);
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
    
    private void ShowProductDetails(Product product)
    {
        DosTheme.InitializeConsole();
        
        var stockStatus = product.UnitsInStock switch
        {
            <= 0 => "OUT OF STOCK",
            <= 10 => "LOW STOCK",
            _ => "IN STOCK"
        };
        
        var discontinuedStatus = product.Discontinued ? "DISCONTINUED" : "ACTIVE";
        
        var content = $"Product ID:       {product.ProductID}\n" +
                     $"Product Name:     {product.ProductName}\n" +
                     $"Category:         {product.CategoryName}\n" +
                     $"Supplier:         {product.SupplierName}\n" +
                     $"Quantity Per Unit:{product.QuantityPerUnit}\n" +
                     $"Unit Price:       {product.UnitPrice:C}\n" +
                     $"Units In Stock:   {product.UnitsInStock} {stockStatus}\n" +
                     $"Units On Order:   {product.UnitsOnOrder}\n" +
                     $"Reorder Level:    {product.ReorderLevel}\n" +
                     $"Status:           {discontinuedStatus}\n\n" +
                     $"────────────────────────────────────────\n\n";
        
        if (product.UnitsInStock <= product.ReorderLevel && !product.Discontinued)
        {
            content += $"⚠ REORDER ALERT: Stock below reorder level!";
        }
        
        var dialog = DosUI.CreateDialog("PRODUCT DETAILS", content);
        DosUI.ShowCenteredDialog(dialog);
        
        Console.ReadKey(true);
    }
    
    private void BrowseByCategory()
    {
        DosUI.ShowError("NOT IMPLEMENTED", "Category browsing feature coming soon!");
    }
    
    private void SearchProducts()
    {
        var fields = new List<FormField>
        {
            new() { Key = "SearchTerm", Label = "Search Term (product name, category, or supplier)", Required = true }
        };
        
        var values = DosUI.ShowFormDialog("SEARCH PRODUCTS", fields);
        
        if (!values.Any() || string.IsNullOrWhiteSpace(values["SearchTerm"]))
            return;
            
        var searchTerm = values["SearchTerm"];
            
        try
        {
            var products = _productService.SearchProducts(searchTerm);
            
            if (!products.Any())
            {
                DosUI.ShowError("NO RESULTS", $"No products found matching '{searchTerm}'.");
                return;
            }
            
            var selectedProduct = DosMenu.SelectFromList(
                $"SEARCH RESULTS: '{searchTerm}'",
                products,
                product => $"  {product.ProductID,-4} {product.ProductName,-25} {product.CategoryName,-15} {product.UnitPrice,8:C} {product.UnitsInStock,6}",
                "↑↓ Navigate  ENTER=Details  ESC=Back"
            );
            
            ShowProductDetails(selectedProduct);
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
    
    private void ShowLowStockProducts()
    {
        try
        {
            // Show loading message
            var loadingDialog = DosUI.CreateDialog("INVENTORY STATUS", "SCANNING INVENTORY...\n\nCHECKING STOCK LEVELS...\n\nPlease wait...");
            DosUI.ShowCenteredDialog(loadingDialog);
            
            var lowStockProducts = _productService.GetLowStockProducts(10);
            
            if (!lowStockProducts.Any())
            {
                var dialog = DosUI.CreateDialog("INVENTORY STATUS", 
                    "All products are adequately stocked!\n\nNo items below reorder threshold.");
                DosUI.ShowCenteredDialog(dialog);
                Console.ReadKey(true);
                return;
            }
            
            var selectedProduct = DosMenu.SelectFromList(
                $"LOW STOCK ALERT ({lowStockProducts.Count} ITEMS)",
                lowStockProducts,
                product => $"  {product.ProductID,-4} {product.ProductName,-25} {product.UnitsInStock,3} units {product.UnitPrice,8:C}",
                "↑↓ Navigate  ENTER=Details  ESC=Back"
            );
            
            ShowProductDetails(selectedProduct);
        }
        catch (OperationCanceledException)
        {
            // User pressed ESC, just return to go back to menu
            return;
        }
        catch (Exception ex)
        {
            DosUI.ShowError("INVENTORY ERROR", ex.Message);
        }
    }
}
