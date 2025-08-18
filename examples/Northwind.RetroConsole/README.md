# Northwind Retro Console Demo

A classic MS-DOS style console application that demonstrates the capabilities of TypedSqlBuilder with the famous Northwind database. This demo recreates the authentic 80's computing experience while showcasing modern type-safe SQL generation.

## 🎯 Features Demonstrated

### TypedSqlBuilder Capabilities
- **Type-safe SQL generation** - No string concatenation or SQL injection risks
- **Complex JOIN operations** - Multiple table relationships with proper typing
- **Dynamic query building** - Queries built based on user input
- **CRUD operations** - Create, Read, Update, Delete with full type safety
- **Aggregation queries** - SUM, COUNT, and other aggregate functions
- **Subqueries and CTEs** - Advanced query patterns
- **Parameterized queries** - Safe parameter binding

### Retro UI Features  
- **MS-DOS aesthetic** - Authentic blue background with white/yellow text
- **Cursor-driven navigation** - Arrow keys move highlight bar through menus
- **Modal dialogs** - Classic dialog boxes for details and confirmations
- **Status messages** - Loading indicators and user feedback
- **Function key shortcuts** - F1-F8 hotkeys for quick access
- **Box-drawing characters** - Traditional ASCII art borders

## 🖥️ Screenshots

The application recreates the classic MS-DOS look:

```
████████████████████████████████████████████████████████████████████████████████
██                                                                            ██
██              PALLADIN SHOP v1.0                               ██
██                                                                            ██
██════════════════════════════════════════════════════════════════════════════██
██                                                                            ██
██    CUSTOMER RECORDS                                                        ██
██████PRODUCT CATALOG██████████████████████████████████████████████████████████  
██    ORDER MANAGEMENT                                                        ██
██    EMPLOYEE DATABASE                                                       ██
██    SALES REPORTS                                                           ██
██    INVENTORY STATUS                                                        ██
██    SYSTEM UTILITIES                                                        ██
██    EXIT PROGRAM                                                            ██
██                                                                            ██
██  ↑↓ Navigate    ENTER Select    ESC Back                                   ██
██                                                                            ██
████████████████████████████████████████████████████████████████████████████████
```

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 or later
- Windows, macOS, or Linux

### Installation & Running

1. **Clone the repository:**
   ```bash
   git clone https://github.com/palladin/TypedSqlBuilder.git
   cd TypedSqlBuilder/examples/Northwind.RetroConsole
   ```

2. **Build the project:**
   ```bash
   dotnet build
   ```

3. **Run the application:**
   ```bash
   dotnet run
   ```

The application will:
- Automatically create a SQLite database with sample Northwind data
- Show a retro splash screen
- Launch into the main menu system

## 📊 Database Schema

The demo uses a simplified Northwind database with these tables:
- **Customers** - Customer information and contacts
- **Products** - Product catalog with categories and suppliers
- **Orders** - Order headers with customer and employee data
- **OrderDetails** - Order line items with products and quantities
- **Employees** - Employee records and hierarchy
- **Categories** - Product categories
- **Suppliers** - Supplier information

## 🎮 Navigation Guide

### Menu Navigation
- **↑/↓ Arrow Keys** - Move cursor highlight
- **ENTER** - Select highlighted item
- **ESC** - Go back or cancel
- **F1-F8** - Function key shortcuts

### Available Modules

#### Customer Records (F1)
- View all customers with paging
- Search customers by company, contact, city, or country  
- View customer details and order history
- Add new customers (CRUD demonstration)
- Edit and delete existing customers

#### Product Catalog (F2)
- Browse complete product inventory
- Search products by name, category, or supplier
- View detailed product information
- Low stock alerts and inventory management
- Category-based filtering

#### Order Management (F3)
- Recent orders with customer and totals
- Detailed order views with line items
- Pending vs shipped order filtering
- Order status tracking
- Print invoice simulation

#### Coming Soon
- Employee Database
- Sales Reports  
- Advanced Analytics
- Data Export Features

## 🔧 Code Structure

```
Northwind.RetroConsole/
├── Program.cs                 # Main application entry point
├── Data/
│   └── DatabaseSetup.cs      # SQLite database creation and seeding
├── Models/
│   └── NorthwindModels.cs    # Entity classes for Northwind data
├── Services/
│   ├── CustomerService.cs    # Customer CRUD operations
│   ├── ProductService.cs     # Product queries and operations  
│   └── OrderService.cs       # Order management and reporting
└── UI/
    ├── DosUI.cs              # MS-DOS style UI framework
    └── Screens/
        ├── CustomerScreen.cs # Customer management interface
        ├── ProductScreen.cs  # Product catalog interface
        └── OrderScreen.cs    # Order management interface
```

## 💡 TypedSqlBuilder Examples

### Simple Query
```csharp
var customers = new SqlTable("Customers", "c");

var query = TypedSql
    .Select(customers.AllColumns())
    .From(customers)
    .Where(customers.Column("Country").Equals("Germany"))
    .OrderBy(customers.Column("CompanyName"));
```

### Complex Join Query
```csharp
var orders = new SqlTable("Orders", "o");
var customers = new SqlTable("Customers", "c");
var employees = new SqlTable("Employees", "e");

var query = TypedSql
    .Select(
        orders.Column("OrderID"),
        customers.Column("CompanyName"),
        SqlExpr.Raw("(e.FirstName || ' ' || e.LastName)").As("EmployeeName")
    )
    .From(orders)
    .InnerJoin(customers).On(orders.Column("CustomerID").Equals(customers.Column("CustomerID")))
    .LeftJoin(employees).On(orders.Column("EmployeeID").Equals(employees.Column("EmployeeID")))
    .Where(orders.Column("OrderDate").GreaterThan(DateTime.Now.AddDays(-30)))
    .OrderBy(orders.Column("OrderDate").Desc());
```

### Aggregate Query
```csharp
var orderDetails = new SqlTable("OrderDetails", "od");

var query = TypedSql
    .Select(
        orderDetails.Column("OrderID"),
        SqlFunctions.Sum(
            orderDetails.Column("UnitPrice")
                .Multiply(orderDetails.Column("Quantity"))
                .Multiply(SqlExpr.Raw("1 - od.Discount"))
        ).As("OrderTotal")
    )
    .From(orderDetails)
    .GroupBy(orderDetails.Column("OrderID"));
```

## 🎨 Customization

### Changing Colors
Modify `DosTheme.cs` to change the color scheme:
```csharp
public static readonly Style Background = new(Color.Blue);           // Background color
public static readonly Style CursorBar = new(Color.Black, Color.Yellow); // Cursor highlight
public static readonly Style Header = new(Color.Yellow, Color.Blue);     // Headers
```

### Adding New Screens
1. Create a new screen class in `UI/Screens/`
2. Add menu option in `Program.cs` 
3. Implement cursor navigation with `DosMenu.SelectFromList()`
4. Use `DosUI.CreateDialog()` for detail views

### Adding New Queries
1. Add methods to appropriate service class
2. Use TypedSqlBuilder query syntax
3. Handle results with generic `ExecuteQuery<T>()` method

## 🔍 Educational Value

This demo is perfect for:
- **Learning TypedSqlBuilder** - See real-world query patterns
- **Console UI development** - Modern techniques for retro aesthetics  
- **Database design** - Well-structured relational schema
- **CRUD operations** - Complete data lifecycle examples
- **90's nostalgia** - Authentic computing experience

## 📝 License

This demo project is part of the TypedSqlBuilder repository and follows the same license terms.

## 🤝 Contributing

Contributions are welcome! Ideas for enhancements:
- Additional report screens
- More complex queries
- Data export features
- Alternative color themes
- Sound effects (beeps and bloops!)
- Network/multi-user simulation

## 🎵 Easter Eggs

Try these hidden features:
- Press `Ctrl+Alt+?` in any screen for developer information
- The loading screens include authentic timing delays
- Error messages use classic DOS-style formatting
- Function keys are mapped like real DOS applications

---

*Experience the nostalgia of 80's computing while learning modern, type-safe database programming!*
