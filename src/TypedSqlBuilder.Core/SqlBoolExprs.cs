using System.Collections.Immutable;

namespace TypedSqlBuilder.Core;

/// <summary>
/// Contains concrete implementations of SQL boolean expressions.
/// These classes represent various boolean operations that can be performed in SQL queries,
/// including logical operations (AND, OR, NOT), equality comparisons, and literal values.
/// All classes inherit from SqlExprBool and implement appropriate interfaces for type safety.
/// </summary>

/// <summary>
/// Represents a literal boolean value in SQL (TRUE or FALSE).
/// </summary>
public class SqlBoolValue(bool value) : SqlExprBool
{
	public void Deconstruct(out bool valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL NOT operation (logical negation).
/// </summary>
public class SqlBoolNot(SqlExprBool value) : SqlExprBool
{
	public void Deconstruct(out SqlExprBool valueOut) => valueOut = value;
}

/// <summary>
/// Represents a SQL AND operation (logical conjunction).
/// </summary>
public class SqlBoolAnd(SqlExprBool left, SqlExprBool right) : SqlExprBool
{
	public void Deconstruct(out SqlExprBool leftOut, out SqlExprBool rightOut) => (leftOut, rightOut) = (left, right);
}

/// <summary>
/// Represents a SQL OR operation (logical disjunction).
/// </summary>
public class SqlBoolOr(SqlExprBool left, SqlExprBool right) : SqlExprBool
{
	public void Deconstruct(out SqlExprBool leftOut, out SqlExprBool rightOut) => (leftOut, rightOut) = (left, right);
}


/// <summary>
/// Represents a reference to a boolean column in a SQL table.
/// This class is used for column references in SQL queries.
/// </summary>
public class SqlBoolColumn : SqlExprBool, ISqlColumn<SqlBoolColumn>
{    
    private SqlBoolColumn(ISqlTable table, string columnName)
    {
        Table = table;
        ColumnName = columnName;        
    }
    
    public ISqlTable Table { get; }
    public string ColumnName { get; }
    
    public static SqlBoolColumn Create(ISqlTable table, string columnName) => new SqlBoolColumn(table, columnName);
    
    public void Deconstruct(out ISqlTable tableOut, out string nameOut)
    {
        tableOut = Table;
        nameOut = ColumnName;
    }
}

/// <summary>
/// Represents a named boolean parameter in SQL expressions.
/// This class is used for parameterized queries where boolean values need to be bound at execution time.
/// </summary>
/// <param name="name">The name of the parameter (e.g., "@isActive", ":enabled")</param>
public class SqlParameterBool(string name) : SqlExprBool
{
	public void Deconstruct(out string nameOut) => nameOut = name;
}

/// <summary>
/// Represents a SQL CASE expression for conditional boolean values.
/// This class applies the SQL CASE WHEN condition THEN trueValue ELSE falseValue END construct.
/// </summary>
/// <param name="condition">The boolean condition to evaluate</param>
/// <param name="trueValue">The boolean expression returned when condition is true</param>
/// <param name="falseValue">The boolean expression returned when condition is false</param>
public class SqlBoolCase(SqlExprBool condition, SqlExprBool trueValue, SqlExprBool falseValue) : SqlExprBool
{
	public void Deconstruct(out SqlExprBool conditionOut, out SqlExprBool trueValueOut, out SqlExprBool falseValueOut) => 
		(conditionOut, trueValueOut, falseValueOut) = (condition, trueValue, falseValue);
}

/// <summary>
/// Represents a SQL NULL value for boolean expressions.
/// This class is used when setting boolean columns to NULL in SQL statements.
/// </summary>
public class SqlBoolNull : SqlExprBool, ISqlNullValue
{
	public static SqlBoolNull Value => new();
}

/// <summary>
/// Abstract base class for SQL IN expressions with a list of values.
/// Represents the SQL IN operator: expr IN (value1, value2, value3, ...)
/// This class serves as the foundation for type-safe IN operations with multiple values.
/// </summary>
/// <param name="expression">The expression to test against the list of values</param>
/// <param name="values">The collection of values to test the expression against</param>
public abstract class SqlInValues(SqlExpr expression, ImmutableArray<SqlExpr> values) : SqlExprBool
{
	public void Deconstruct(out SqlExpr expressionOut, out ImmutableArray<SqlExpr> valuesOut)
	{
		expressionOut = expression;
		valuesOut = values;
	}
}

/// <summary>
/// Strongly-typed SQL IN expression with a list of values.
/// Provides type-safe IN operations where the expression and all values are of the same SQL expression type.
/// Equivalent to: expr IN (value1, value2, value3, ...)
/// </summary>
/// <typeparam name="TExpr">The type of SQL expression being tested and the values in the list</typeparam>
/// <param name="expression">The expression to test against the list of values</param>
/// <param name="values">The strongly-typed collection of values to test the expression against</param>
public class SqlInValues<TExpr>(TExpr expression, ImmutableArray<TExpr> values) : SqlInValues(expression, values.Cast<SqlExpr>().ToImmutableArray())
	where TExpr : SqlExpr
{

}

/// <summary>
/// Represents a SQL IN expression with a subquery.
/// Equivalent to: expr IN (SELECT ... FROM ...)
/// </summary>
/// <param name="expression">The expression to test against the subquery results</param>
/// <param name="subQuery">The subquery that provides the values to test against</param>
public abstract class SqlInSubQuery(SqlExpr expression, ISqlQuery subQuery) : SqlExprBool
{
	public void Deconstruct(out SqlExpr expressionOut, out ISqlQuery subQueryOut)
	{
		expressionOut = expression;
		subQueryOut = subQuery;
	}
}

/// <summary>
/// Strongly-typed SQL IN expression with a subquery.
/// Provides type-safe IN operations with scalar subqueries.
/// Equivalent to: expr IN (SELECT column FROM table WHERE condition)
/// </summary>
/// <typeparam name="TExpr">The type of SQL expression being tested</typeparam>
/// <param name="expression">The expression to test against the subquery results</param>
/// <param name="subQuery">The strongly-typed subquery that returns values of type TExpr to test against</param>
public class SqlInSubQuery<TExpr>(TExpr expression, ISqlQuery<ValueTuple<TExpr>> subQuery) : SqlInSubQuery(expression, subQuery)
	where TExpr : SqlExpr
{

}

