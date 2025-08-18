using Northwind.RetroConsole.Data;
using Northwind.RetroConsole.Models;
using TypedSqlBuilder.Core;
using Dapper;

namespace Northwind.RetroConsole.Services;

public class ProductService
{
    private readonly DatabaseConnection _db;
    
    public ProductService(DatabaseConnection db)
    {
        _db = db;
    }
    
    public List<Product> GetAllProducts()
    {
        var query = NorthwindDb.Products.From()
            .LeftJoin(
                NorthwindDb.Categories,
                p => p.CategoryID,
                c => c.CategoryID,
                (p, c) => (
                    ProductID: p.ProductID,
                    ProductName: p.ProductName,
                    SupplierID: p.SupplierID,
                    CategoryID: p.CategoryID,
                    QuantityPerUnit: p.QuantityPerUnit,
                    UnitPrice: p.UnitPrice,
                    UnitsInStock: p.UnitsInStock,
                    UnitsOnOrder: p.UnitsOnOrder,
                    ReorderLevel: p.ReorderLevel,
                    Discontinued: p.Discontinued,
                    CategoryName: c.CategoryName
                ))
            .LeftJoin(
                NorthwindDb.Suppliers,
                result => result.SupplierID,
                s => s.SupplierID,
                (result, s) => (
                    ProductID: result.ProductID,
                    ProductName: result.ProductName,
                    SupplierID: result.SupplierID,
                    CategoryID: result.CategoryID,
                    QuantityPerUnit: result.QuantityPerUnit,
                    UnitPrice: result.UnitPrice,
                    UnitsInStock: result.UnitsInStock,
                    UnitsOnOrder: result.UnitsOnOrder,
                    ReorderLevel: result.ReorderLevel,
                    Discontinued: result.Discontinued,
                    CategoryName: result.CategoryName,
                    SupplierName: s.CompanyName
                ))
            .OrderBy(result => (result.ProductName, Sort.Asc));
            
        return ExecuteQuery<Product>(query);
    }
    
    public List<Product> GetProductsByCategory(int categoryId)
    {
        var query = NorthwindDb.Products.From()
            .Where(p => p.CategoryID == categoryId)
            .LeftJoin(
                NorthwindDb.Categories,
                p => p.CategoryID,
                c => c.CategoryID,
                (p, c) => (
                    ProductID: p.ProductID,
                    ProductName: p.ProductName,
                    SupplierID: p.SupplierID,
                    CategoryID: p.CategoryID,
                    QuantityPerUnit: p.QuantityPerUnit,
                    UnitPrice: p.UnitPrice,
                    UnitsInStock: p.UnitsInStock,
                    UnitsOnOrder: p.UnitsOnOrder,
                    ReorderLevel: p.ReorderLevel,
                    Discontinued: p.Discontinued,
                    CategoryName: c.CategoryName
                ))
            .LeftJoin(
                NorthwindDb.Suppliers,
                result => result.SupplierID,
                s => s.SupplierID,
                (result, s) => (
                    ProductID: result.ProductID,
                    ProductName: result.ProductName,
                    SupplierID: result.SupplierID,
                    CategoryID: result.CategoryID,
                    QuantityPerUnit: result.QuantityPerUnit,
                    UnitPrice: result.UnitPrice,
                    UnitsInStock: result.UnitsInStock,
                    UnitsOnOrder: result.UnitsOnOrder,
                    ReorderLevel: result.ReorderLevel,
                    Discontinued: result.Discontinued,
                    CategoryName: result.CategoryName,
                    SupplierName: s.CompanyName
                ))
            .OrderBy(result => (result.ProductName, Sort.Asc));
            
        return ExecuteQuery<Product>(query);
    }
    
    public List<Product> SearchProducts(string searchTerm)
    {
        var query = NorthwindDb.Products.From()
            .LeftJoin(
                NorthwindDb.Categories,
                p => p.CategoryID,
                c => c.CategoryID,
                (p, c) => (Product: p, Category: c))
            .LeftJoin(
                NorthwindDb.Suppliers,
                result => result.Product.SupplierID,
                s => s.SupplierID,
                (result, s) => (result.Product, result.Category, Supplier: s))
            .Where(result => result.Product.ProductName.Like($"%{searchTerm}%") ||
                            result.Category.CategoryName.Like($"%{searchTerm}%") ||
                            result.Supplier.CompanyName.Like($"%{searchTerm}%"))
            .Select(result => (
                ProductID: result.Product.ProductID,
                ProductName: result.Product.ProductName,
                SupplierID: result.Product.SupplierID,
                CategoryID: result.Product.CategoryID,
                QuantityPerUnit: result.Product.QuantityPerUnit,
                UnitPrice: result.Product.UnitPrice,
                UnitsInStock: result.Product.UnitsInStock,
                UnitsOnOrder: result.Product.UnitsOnOrder,
                ReorderLevel: result.Product.ReorderLevel,
                Discontinued: result.Product.Discontinued,
                CategoryName: result.Category.CategoryName,
                SupplierName: result.Supplier.CompanyName
            ))
            .OrderBy(result => (result.ProductName, Sort.Asc));
            
        return ExecuteQuery<Product>(query);
    }
    
    public List<Product> GetLowStockProducts(int threshold = 10)
    {
        var query = NorthwindDb.Products.From()
            .Where(p => p.UnitsInStock <= threshold && p.Discontinued == false)
            .LeftJoin(
                NorthwindDb.Categories,
                p => p.CategoryID,
                c => c.CategoryID,
                (p, c) => (
                    ProductID: p.ProductID,
                    ProductName: p.ProductName,
                    SupplierID: p.SupplierID,
                    CategoryID: p.CategoryID,
                    QuantityPerUnit: p.QuantityPerUnit,
                    UnitPrice: p.UnitPrice,
                    UnitsInStock: p.UnitsInStock,
                    UnitsOnOrder: p.UnitsOnOrder,
                    ReorderLevel: p.ReorderLevel,
                    Discontinued: p.Discontinued,
                    CategoryName: c.CategoryName
                ))
            .LeftJoin(
                NorthwindDb.Suppliers,
                result => result.SupplierID,
                s => s.SupplierID,
                (result, s) => (
                    ProductID: result.ProductID,
                    ProductName: result.ProductName,
                    SupplierID: result.SupplierID,
                    CategoryID: result.CategoryID,
                    QuantityPerUnit: result.QuantityPerUnit,
                    UnitPrice: result.UnitPrice,
                    UnitsInStock: result.UnitsInStock,
                    UnitsOnOrder: result.UnitsOnOrder,
                    ReorderLevel: result.ReorderLevel,
                    Discontinued: result.Discontinued,
                    CategoryName: result.CategoryName,
                    SupplierName: s.CompanyName
                ))
            .OrderBy(result => (result.UnitsInStock, Sort.Asc));
            
        return ExecuteQuery<Product>(query);
    }
    
    private List<T> ExecuteQuery<T>(ISqlQuery query)
    {
        var (sql, parameters) = query.ToSqliteRaw();
        return _db.Connection.Query<T>(sql, parameters).ToList();
    }
}
