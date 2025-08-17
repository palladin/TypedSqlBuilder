#!/bin/bash

# Script to convert all void methods to Task methods and add return statements

file="tests/TypedSqlBuilder.Tests/SqliteQueryTests.cs"

echo "Converting void methods to Task methods in $file"

# First, replace all "public void" with "public Task"
sed -i '' 's/public void \([A-Za-z_]*GeneratesCorrectSql\|[A-Za-z_]*WorksCorrectly\|[A-Za-z_]*ProducesCorrectResults\|[A-Za-z_]*GeneratesExpectedSql\|Sqlite_UsesColonPrefix\)(/public Task \1(/g' "$file"

# Add return Task.CompletedTask; before the closing braces of each method
# This is a bit trickier, so we'll use awk
awk '
BEGIN { in_method = 0; method_braces = 0 }

# Track when we enter a Task method
/public Task/ { in_method = 1; method_braces = 0; print; next }

# Track braces when in a method
in_method && /{/ { method_braces++; print; next }
in_method && /}/ { 
    method_braces--
    if (method_braces == 0) {
        # This is the closing brace of the method
        print "        return Task.CompletedTask;"
        print $0
        in_method = 0
    } else {
        print $0
    }
    next
}

# Print all other lines as-is
{ print }
' "$file" > "$file.tmp" && mv "$file.tmp" "$file"

echo "Conversion completed!"
