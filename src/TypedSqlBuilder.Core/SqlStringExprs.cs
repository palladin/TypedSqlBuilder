namespace TypedSqlBuilder.Core;

// String expression implementations
public class SqlStringValue(string value) : SqlExprString
{
	public string Value { get; } = value;
}
