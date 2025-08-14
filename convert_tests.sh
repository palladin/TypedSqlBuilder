#!/bin/bash

# Function to convert SQL test expectation from single line to pretty format
convert_sql_test() {
    local file="$1"
    
    # Read the file content
    local content=$(cat "$file")
    
    # Replace Assert.Equal("SELECT...", sql); with pretty format
    # This is a very basic conversion - we'll need to refine it
    
    # Basic SELECT pattern: SELECT fields FROM table
    content=$(echo "$content" | perl -0777 -pe 's/Assert\.Equal\("(SELECT [^"]*?FROM [^"]*?)"(?!, sql\);)/var expectedSql = """\n        $1\n        """;\n        Assert.Equal(expectedSql/g')
    
    echo "$content" > "$file"
}

# Process both test files
convert_sql_test "/Users/nick.palladinos/Projects/TypedSqlBuilder/tests/TypedSqlBuilder.Tests/SqlServerQueryTests.cs"
convert_sql_test "/Users/nick.palladinos/Projects/TypedSqlBuilder/tests/TypedSqlBuilder.Tests/SqliteQueryTests.cs"

echo "Conversion complete!"
