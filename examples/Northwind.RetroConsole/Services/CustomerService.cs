using Northwind.RetroConsole.Data;
using Northwind.RetroConsole.Models;
using TypedSqlBuilder.Core;
using System.Data;
using Dapper;

namespace Northwind.RetroConsole.Services;

public class CustomerService
{
    private readonly DatabaseConnection _db;
    
    public CustomerService(DatabaseConnection db)
    {
        _db = db;
    }
    
    public List<Customer> GetAllCustomers()
    {
        var query = NorthwindDb.Customers.From()
            .OrderBy(c => (c.CompanyName, Sort.Asc));
            
        return ExecuteQuery<Customer>(query);
    }
    
    public List<Customer> SearchCustomers(string searchTerm)
    {
        var query = NorthwindDb.Customers.From()
            .Where(c => c.CompanyName.Like($"%{searchTerm}%") ||
                       c.ContactName.Like($"%{searchTerm}%") ||
                       c.City.Like($"%{searchTerm}%") ||
                       c.Country.Like($"%{searchTerm}%"))
            .OrderBy(c => (c.CompanyName, Sort.Asc));
            
        return ExecuteQuery<Customer>(query);
    }
    
    public List<Customer> SearchCustomersByExample(List<KeyValuePair<string, string>> searchCriteria)
    {
        var query = NorthwindDb.Customers.From();
        
        // Build dynamic where conditions based on the provided criteria
        foreach (var criterion in searchCriteria)
        {
            switch (criterion.Key)
            {
                case "CustomerID":
                    query = query.Where(c => c.CustomerID.Like($"%{criterion.Value}%"));
                    break;
                case "CompanyName":
                    query = query.Where(c => c.CompanyName.Like($"%{criterion.Value}%"));
                    break;
                case "ContactName":
                    query = query.Where(c => c.ContactName.Like($"%{criterion.Value}%"));
                    break;
                case "ContactTitle":
                    query = query.Where(c => c.ContactTitle.Like($"%{criterion.Value}%"));
                    break;
                case "City":
                    query = query.Where(c => c.City.Like($"%{criterion.Value}%"));
                    break;
                case "Country":
                    query = query.Where(c => c.Country.Like($"%{criterion.Value}%"));
                    break;
                case "Phone":
                    query = query.Where(c => c.Phone.Like($"%{criterion.Value}%"));
                    break;
            }
        }
        
        query = query.OrderBy(c => (c.CompanyName, Sort.Asc));
        return ExecuteQuery<Customer>(query);
    }
    
    public Customer? GetCustomerById(string customerId)
    {
        var query = NorthwindDb.Customers.From()
            .Where(c => c.CustomerID == customerId);
            
        return ExecuteQuery<Customer>(query).FirstOrDefault();
    }
    
    public void AddCustomer(Customer customer)
    {
        var statement = TypedSql.Insert<CustomersTable>()
            .Value(c => c.CustomerID, customer.CustomerID)
            .Value(c => c.CompanyName, customer.CompanyName)
            .Value(c => c.ContactName, customer.ContactName)
            .Value(c => c.ContactTitle, customer.ContactTitle)
            .Value(c => c.Address, customer.Address)
            .Value(c => c.City, customer.City)
            .Value(c => c.Region, customer.Region)
            .Value(c => c.PostalCode, customer.PostalCode)
            .Value(c => c.Country, customer.Country)
            .Value(c => c.Phone, customer.Phone)
            .Value(c => c.Fax, customer.Fax);
            
        ExecuteStatement(statement);
    }
    
    public void UpdateCustomer(Customer customer)
    {
        var statement = TypedSql.Update<CustomersTable>()
            .Set(c => c.CompanyName, customer.CompanyName)
            .Set(c => c.ContactName, customer.ContactName)
            .Set(c => c.ContactTitle, customer.ContactTitle)
            .Set(c => c.Address, customer.Address)
            .Set(c => c.City, customer.City)
            .Set(c => c.Region, customer.Region)
            .Set(c => c.PostalCode, customer.PostalCode)
            .Set(c => c.Country, customer.Country)
            .Set(c => c.Phone, customer.Phone)
            .Set(c => c.Fax, customer.Fax)
            .Where(c => c.CustomerID == customer.CustomerID);
            
        ExecuteStatement(statement);
    }
    
    public void DeleteCustomer(string customerId)
    {
        var statement = TypedSql.Delete<CustomersTable>()
            .Where(c => c.CustomerID == customerId);
            
        ExecuteStatement(statement);
    }
    
    private List<T> ExecuteQuery<T>(ISqlQuery query)
    {
        var (sql, parameters) = query.ToSqliteRaw();
        return _db.Connection.Query<T>(sql, parameters).ToList();
    }
    
    private void ExecuteStatement(ISqlStatement statement)
    {
        var (sql, parameters) = statement.ToSqliteRaw();
        _db.Connection.Execute(sql, parameters);
    }
}
