#!/bin/bash

# Script to systematically fix SQL test formatting patterns

FILE1="/Users/nick.palladinos/Projects/TypedSqlBuilder/tests/TypedSqlBuilder.Tests/SqlServerQueryTests.cs"
FILE2="/Users/nick.palladinos/Projects/TypedSqlBuilder/tests/TypedSqlBuilder.Tests/SqliteQueryTests.cs"

fix_file() {
    local file="$1"
    echo "Fixing patterns in $file..."
    
    # Fix FROM clauses - table name should be on next line with indentation
    perl -i -pe 's/FROM customers a0/FROM \n            customers a0/g' "$file"
    perl -i -pe 's/FROM products a0/FROM \n            products a0/g' "$file"
    
    # Fix three-column projections (Id, Age, Name)
    perl -i -pe 's/a0\.Id AS Id, a0\.Age AS Age, a0\.Name AS Name/a0.Id AS Id,\n            a0.Age AS Age,\n            a0.Name AS Name/g' "$file"
    
    # Fix three-column projections (Id, Name, Age) - different order  
    perl -i -pe 's/a0\.Id AS Id, a0\.Name AS Name, a0\.Age AS Age/a0.Id AS Id,\n            a0.Name AS Name,\n            a0.Age AS Age/g' "$file"
    
    # Fix two-column projections (Id, Name)
    perl -i -pe 's/a0\.Id AS Id, a0\.Name AS Name/a0.Id AS Id,\n            a0.Name AS Name/g' "$file"
    
    # Fix two-column projections (Id, Age)
    perl -i -pe 's/a0\.Id AS Id, a0\.Age AS Age/a0.Id AS Id,\n            a0.Age AS Age/g' "$file"
    
    # Fix two-column projections (Age, Name)
    perl -i -pe 's/a0\.Age AS Age, a0\.Name AS Name/a0.Age AS Age,\n            a0.Name AS Name/g' "$file"
    
    # Fix a1 alias patterns
    perl -i -pe 's/a1\.Id AS Id, a1\.Name AS Name/a1.Id AS Id,\n            a1.Name AS Name/g' "$file"
    perl -i -pe 's/a1\.Id AS Id, a1\.Age AS Age/a1.Id AS Id,\n            a1.Age AS Age/g' "$file"
    
    # Fix join projections with multiple aliases - these are trickier
    # Pattern: a0.Id AS Id, a0.Name AS Name, a1.something
    perl -i -pe 's/a0\.Id AS Id, a0\.Name AS Name, (a1\.[^,\n]+)/a0.Id AS Id,\n            a0.Name AS Name,\n            $1/g' "$file"
    perl -i -pe 's/a1\.Id AS Id, a1\.Name AS Name, (a1\.[^,\n]+)/a1.Id AS Id,\n            a1.Name AS Name,\n            $1/g' "$file"
    
    # Fix aggregation patterns
    perl -i -pe 's/a0\.Age AS Age, COUNT\(\*\) AS Count/a0.Age AS Age,\n            COUNT(*) AS Count/g' "$file"
    perl -i -pe 's/a0\.Age AS Age, a0\.Name AS Name, COUNT\(\*\) AS Count/a0.Age AS Age,\n            a0.Name AS Name,\n            COUNT(*) AS Count/g' "$file"
    
    # Fix custom named projections
    perl -i -pe 's/a0\.Id AS CustomerId, a0\.Name AS CustomerName/a0.Id AS CustomerId,\n            a0.Name AS CustomerName/g' "$file"
    perl -i -pe 's/a0\.ProductId AS ProductId, a0\.ProductName AS ProductName/a0.ProductId AS ProductId,\n            a0.ProductName AS ProductName/g' "$file"
    
    # Fix expression projections
    perl -i -pe 's/([^,\n]+) AS prj0, ([^,\n]+) AS prj1/$1 AS prj0,\n            $2 AS prj1/g' "$file"
    
    echo "Fixed $file"
}

# Apply fixes to both test files
fix_file "$FILE1" 
fix_file "$FILE2"

echo "All SQL formatting fixes applied!"
