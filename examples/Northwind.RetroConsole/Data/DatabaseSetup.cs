using Microsoft.Data.Sqlite;
using System.Data;
using Dapper;

namespace Northwind.RetroConsole.Data;

public class DoubleToDecimalTypeHandler : SqlMapper.TypeHandler<decimal>
{
    public override void SetValue(IDbDataParameter parameter, decimal value)
    {
        parameter.Value = value;
    }

    public override decimal Parse(object value)
    {
        return Convert.ToDecimal(value);
    }
}

public class DoubleToNullableDecimalTypeHandler : SqlMapper.TypeHandler<decimal?>
{
    public override void SetValue(IDbDataParameter parameter, decimal? value)
    {
        parameter.Value = value ?? (object)DBNull.Value;
    }

    public override decimal? Parse(object value)
    {
        if (value == null || value == DBNull.Value)
            return null;
        return Convert.ToDecimal(value);
    }
}

public class DatabaseConnection : IDisposable
{
    private readonly SqliteConnection _connection;
    private bool _disposed = false;
    private static bool _typeHandlersRegistered = false;

    public DatabaseConnection(string connectionString)
    {
        // Register type handlers only once
        if (!_typeHandlersRegistered)
        {
            SqlMapper.AddTypeHandler(new DoubleToDecimalTypeHandler());
            SqlMapper.AddTypeHandler(new DoubleToNullableDecimalTypeHandler());
            _typeHandlersRegistered = true;
        }

        _connection = new SqliteConnection(connectionString);
        _connection.Open();
    }

    public IDbConnection Connection => _connection;

    public void Dispose()
    {
        if (!_disposed)
        {
            _connection?.Dispose();
            _disposed = true;
        }
    }
}

public static class DatabaseSetup
{
    public static string GetConnectionString()
    {
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "northwind.db");
        
        // Create database if it doesn't exist
        if (!File.Exists(dbPath))
        {
            CreateDatabase(dbPath);
        }
        
        return $"Data Source={dbPath};";
    }
    
    private static void CreateDatabase(string dbPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        
        using var connection = new SqliteConnection($"Data Source={dbPath};");
        connection.Open();
        
        // Create tables with sample Northwind schema
        var createTablesScript = @"
            CREATE TABLE Categories (
                CategoryID INTEGER PRIMARY KEY,
                CategoryName TEXT NOT NULL,
                Description TEXT
            );
            
            CREATE TABLE Suppliers (
                SupplierID INTEGER PRIMARY KEY,
                CompanyName TEXT NOT NULL,
                ContactName TEXT,
                ContactTitle TEXT,
                Address TEXT,
                City TEXT,
                Region TEXT,
                PostalCode TEXT,
                Country TEXT,
                Phone TEXT,
                Fax TEXT,
                HomePage TEXT
            );
            
            CREATE TABLE Products (
                ProductID INTEGER PRIMARY KEY,
                ProductName TEXT NOT NULL,
                SupplierID INTEGER,
                CategoryID INTEGER,
                QuantityPerUnit TEXT,
                UnitPrice DECIMAL(10,4),
                UnitsInStock INTEGER,
                UnitsOnOrder INTEGER,
                ReorderLevel INTEGER,
                Discontinued INTEGER DEFAULT 0,
                FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
                FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID)
            );
            
            CREATE TABLE Customers (
                CustomerID TEXT PRIMARY KEY,
                CompanyName TEXT NOT NULL,
                ContactName TEXT,
                ContactTitle TEXT,
                Address TEXT,
                City TEXT,
                Region TEXT,
                PostalCode TEXT,
                Country TEXT,
                Phone TEXT,
                Fax TEXT
            );
            
            CREATE TABLE Employees (
                EmployeeID INTEGER PRIMARY KEY,
                LastName TEXT NOT NULL,
                FirstName TEXT NOT NULL,
                Title TEXT,
                TitleOfCourtesy TEXT,
                BirthDate DATETIME,
                HireDate DATETIME,
                Address TEXT,
                City TEXT,
                Region TEXT,
                PostalCode TEXT,
                Country TEXT,
                HomePhone TEXT,
                Extension TEXT,
                ReportsTo INTEGER,
                FOREIGN KEY (ReportsTo) REFERENCES Employees(EmployeeID)
            );
            
            CREATE TABLE Orders (
                OrderID INTEGER PRIMARY KEY,
                CustomerID TEXT,
                EmployeeID INTEGER,
                OrderDate DATETIME,
                RequiredDate DATETIME,
                ShippedDate DATETIME,
                ShipVia INTEGER,
                Freight DECIMAL(10,4),
                ShipName TEXT,
                ShipAddress TEXT,
                ShipCity TEXT,
                ShipRegion TEXT,
                ShipPostalCode TEXT,
                ShipCountry TEXT,
                FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
                FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
            );
            
            CREATE TABLE OrderDetails (
                OrderID INTEGER,
                ProductID INTEGER,
                UnitPrice DECIMAL(10,4) NOT NULL,
                Quantity INTEGER NOT NULL,
                Discount REAL DEFAULT 0,
                PRIMARY KEY (OrderID, ProductID),
                FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
                FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
            );";

        using var command = new SqliteCommand(createTablesScript, connection);
        command.ExecuteNonQuery();
        
        // Insert sample data
        InsertSampleData(connection);
    }
    
    private static void InsertSampleData(SqliteConnection connection)
    {
        var sampleData = @"
            -- Categories
            INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES
            (1, 'Beverages', 'Soft drinks, coffees, teas, beers, and ales'),
            (2, 'Condiments', 'Sweet and savory sauces, relishes, spreads, and seasonings'),
            (3, 'Dairy Products', 'Cheeses'),
            (4, 'Grains/Cereals', 'Breads, crackers, pasta, and cereal'),
            (5, 'Meat/Poultry', 'Prepared meats'),
            (6, 'Produce', 'Dried fruit and bean curd'),
            (7, 'Seafood', 'Seaweed and fish'),
            (8, 'Confections', 'Desserts, candies, and sweet breads');
            
            -- Suppliers
            INSERT INTO Suppliers (SupplierID, CompanyName, ContactName, City, Country, Phone) VALUES
            (1, 'Exotic Liquids', 'Charlotte Cooper', 'London', 'UK', '(171) 555-2222'),
            (2, 'New Orleans Cajun Delights', 'Shelley Burke', 'New Orleans', 'USA', '(100) 555-4822'),
            (3, 'Grandma Kellys Homestead', 'Regina Murphy', 'Ann Arbor', 'USA', '(313) 555-5735'),
            (4, 'Tokyo Traders', 'Yoshi Nagase', 'Tokyo', 'Japan', '(03) 3555-5011'),
            (5, 'Cooperativa de Quesos Las Cabras', 'Antonio del Valle Saavedra', 'Oviedo', 'Spain', '(98) 598 76 54');
            
            -- Products
            INSERT INTO Products (ProductID, ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued) VALUES
            (1, 'Chai', 1, 1, '10 boxes x 20 bags', 18.00, 39, 0, 10, 0),
            (2, 'Chang', 1, 1, '24 - 12 oz bottles', 19.00, 17, 40, 25, 0),
            (3, 'Aniseed Syrup', 1, 2, '12 - 550 ml bottles', 10.00, 13, 70, 25, 0),
            (4, 'Chef Antons Cajun Seasoning', 2, 2, '48 - 6 oz jars', 22.00, 53, 0, 0, 0),
            (5, 'Chef Antons Gumbo Mix', 2, 2, '36 boxes', 21.35, 0, 0, 0, 1),
            (6, 'Grandmas Boysenberry Spread', 3, 2, '12 - 8 oz jars', 25.00, 120, 0, 25, 0),
            (7, 'Uncle Bobs Organic Dried Pears', 3, 7, '12 - 1 lb pkgs.', 30.00, 15, 0, 10, 0),
            (8, 'Northwoods Cranberry Sauce', 3, 2, '12 - 12 oz jars', 40.00, 6, 0, 0, 0);
            
            -- Customers
            INSERT INTO Customers (CustomerID, CompanyName, ContactName, City, Country, Phone) VALUES
            ('ALFKI', 'Alfreds Futterkiste', 'Maria Anders', 'Berlin', 'Germany', '030-0074321'),
            ('ANATR', 'Ana Trujillo', 'Ana Trujillo', 'México D.F.', 'Mexico', '(5) 555-4729'),
            ('ANTON', 'Antonio Moreno Taquería', 'Antonio Moreno', 'México D.F.', 'Mexico', '(5) 555-3932'),
            ('AROUT', 'Around the Horn', 'Thomas Hardy', 'London', 'UK', '(171) 555-7788'),
            ('BERGS', 'Berglunds snabbköp', 'Christina Berglund', 'Luleå', 'Sweden', '0921-12 34 65'),
            ('BLAUS', 'Blauer See Delikatessen', 'Hanna Moos', 'Mannheim', 'Germany', '0621-08460'),
            ('BLONP', 'Blondesddsl père et fils', 'Frédérique Citeaux', 'Strasbourg', 'France', '88.60.15.31'),
            ('BOLID', 'Bólido Comidas preparadas', 'Martín Sommer', 'Madrid', 'Spain', '(91) 555 22 82');
            
            -- Employees
            INSERT INTO Employees (EmployeeID, LastName, FirstName, Title, City, Country, HireDate) VALUES
            (1, 'Davolio', 'Nancy', 'Sales Representative', 'Seattle', 'USA', '1992-05-01'),
            (2, 'Fuller', 'Andrew', 'Vice President, Sales', 'Tacoma', 'USA', '1992-08-14'),
            (3, 'Leverling', 'Janet', 'Sales Representative', 'Kirkland', 'USA', '1992-04-01'),
            (4, 'Peacock', 'Margaret', 'Sales Representative', 'Redmond', 'USA', '1993-05-03'),
            (5, 'Buchanan', 'Steven', 'Sales Manager', 'London', 'UK', '1993-10-17'),
            (6, 'Suyama', 'Michael', 'Sales Representative', 'London', 'UK', '1993-10-17'),
            (7, 'King', 'Robert', 'Sales Representative', 'London', 'UK', '1994-01-02'),
            (8, 'Callahan', 'Laura', 'Inside Sales Coordinator', 'Seattle', 'USA', '1994-03-05');
            
            -- Orders
            INSERT INTO Orders (OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, Freight, ShipName, ShipCity, ShipCountry) VALUES
            (10248, 'ALFKI', 5, '1996-07-04', '1996-08-01', '1996-07-16', 32.38, 'Alfreds Futterkiste', 'Berlin', 'Germany'),
            (10249, 'ANATR', 6, '1996-07-05', '1996-08-16', '1996-07-10', 11.61, 'Ana Trujillo Emparedados y helados', 'México D.F.', 'Mexico'),
            (10250, 'ANTON', 4, '1996-07-08', '1996-08-05', '1996-07-12', 65.83, 'Antonio Moreno Taquería', 'México D.F.', 'Mexico'),
            (10251, 'ALFKI', 3, '1996-07-08', '1996-08-05', '1996-07-15', 41.34, 'Alfreds Futterkiste', 'Berlin', 'Germany'),
            (10252, 'AROUT', 4, '1996-07-09', '1996-08-06', '1996-07-11', 51.30, 'Around the Horn', 'London', 'UK');
            
            -- Order Details
            INSERT INTO OrderDetails (OrderID, ProductID, UnitPrice, Quantity, Discount) VALUES
            (10248, 1, 18.00, 12, 0.0),
            (10248, 2, 19.00, 10, 0.0),
            (10248, 3, 10.00, 5, 0.0),
            (10249, 1, 18.00, 9, 0.0),
            (10249, 2, 19.00, 40, 0.0),
            (10250, 1, 18.00, 10, 0.15),
            (10250, 6, 25.00, 35, 0.15),
            (10250, 7, 30.00, 15, 0.0),
            (10251, 1, 18.00, 6, 0.05),
            (10251, 8, 40.00, 10, 0.05);";

        using var command = new SqliteCommand(sampleData, connection);
        command.ExecuteNonQuery();
    }
}
