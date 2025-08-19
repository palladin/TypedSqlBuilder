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
                (p, c) => (Product: p, Category: c))
            .LeftJoin(
                NorthwindDb.Suppliers,
                result => result.Product.SupplierID,
                s => s.SupplierID,
                (result, s) => (result.Product, result.Category, Supplier: s))
            .OrderBy(result => (result.Product.ProductName, Sort.Asc))
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
            ));
            
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
                (p, c) => (Product: p, Category: c))
            .LeftJoin(
                NorthwindDb.Suppliers,
                result => result.Product.SupplierID,
                s => s.SupplierID,
                (result, s) => (result.Product, result.Category, Supplier: s))
            .OrderBy(result => (result.Product.ProductName, Sort.Asc))
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
            ));
            
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
            .OrderBy(result => (result.Product.ProductName, Sort.Asc))
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
            ));
            
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
                (p, c) => (Product: p, Category: c))
            .LeftJoin(
                NorthwindDb.Suppliers,
                result => result.Product.SupplierID,
                s => s.SupplierID,
                (result, s) => (result.Product, result.Category, Supplier: s))
            .OrderBy(result => (result.Product.UnitsInStock, Sort.Asc))
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
            ));
            
        return ExecuteQuery<Product>(query);
    }
    
    private List<T> ExecuteQuery<T>(ISqlQuery query)
    {
        var (sql, parameters) = query.ToSqliteRaw();
        return _db.Connection.Query<T>(sql, parameters).ToList();
    }
}
