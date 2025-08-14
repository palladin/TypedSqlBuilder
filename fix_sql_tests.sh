#!/bin/bash

# Script to fix SQL test formatting

# Function to update projections - splits comma-separated projections into multiple lines
fix_projections() {
    local file="$1"
    
    # Handle three columns: Id, Age, Name  
    sed -i '' 's/a0\.Id AS Id, a0\.Age AS Age, a0\.Name AS Name/a0.Id AS Id,\
            a0.Age AS Age,\
            a0.Name AS Name/g' "$file"
            
    # Handle three columns: Id, Name, Age (different order)
    sed -i '' 's/a0\.Id AS Id, a0\.Name AS Name, a0\.Age AS Age/a0.Id AS Id,\
            a0.Name AS Name,\
            a0.Age AS Age/g' "$file"
    
    # Handle two columns: Id, Name
    sed -i '' 's/a0\.Id AS Id, a0\.Name AS Name/a0.Id AS Id,\
            a0.Name AS Name/g' "$file"
    
    # Handle two columns: Id, Age  
    sed -i '' 's/a0\.Id AS Id, a0\.Age AS Age/a0.Id AS Id,\
            a0.Age AS Age/g' "$file"
            
    # Handle other common patterns with a1 alias
    sed -i '' 's/a1\.Id AS Id, a1\.Name AS Name/a1.Id AS Id,\
            a1.Name AS Name/g' "$file"
            
    # Handle join patterns with multiple aliases
    sed -i '' 's/a0\.Id AS Id, a0\.Name AS Name, a1\./a0.Id AS Id,\
            a0.Name AS Name,\
            a1./g' "$file"
    
    sed -i '' 's/a1\.Id AS Id, a1\.Name AS Name, a1\./a1.Id AS Id,\
            a1.Name AS Name,\
            a1./g' "$file"
            
    # Handle aggregate functions with Age
    sed -i '' 's/a0\.Age AS Age, COUNT(\*) AS Count/a0.Age AS Age,\
            COUNT(*) AS Count/g' "$file"
            
    sed -i '' 's/a0\.Age AS Age, a0\.Name AS Name, COUNT(\*) AS Count/a0.Age AS Age,\
            a0.Name AS Name,\
            COUNT(*) AS Count/g' "$file"
}

# Function to fix FROM clauses
fix_from_clauses() {
    local file="$1"
    
    # Handle FROM customers a0
    sed -i '' 's/FROM customers a0/FROM \
            customers a0/g' "$file"
            
    # Handle FROM products a0  
    sed -i '' 's/FROM products a0/FROM \
            products a0/g' "$file"
}

# Apply fixes to both test files
fix_projections "/Users/nick.palladinos/Projects/TypedSqlBuilder/tests/TypedSqlBuilder.Tests/SqlServerQueryTests.cs"
fix_from_clauses "/Users/nick.palladinos/Projects/TypedSqlBuilder/tests/TypedSqlBuilder.Tests/SqlServerQueryTests.cs"

fix_projections "/Users/nick.palladinos/Projects/TypedSqlBuilder/tests/TypedSqlBuilder.Tests/SqliteQueryTests.cs"
fix_from_clauses "/Users/nick.palladinos/Projects/TypedSqlBuilder/tests/TypedSqlBuilder.Tests/SqliteQueryTests.cs"

echo "SQL formatting fixes applied!"
