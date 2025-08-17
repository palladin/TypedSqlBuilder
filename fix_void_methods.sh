#!/bin/bash

# Script to remove "return Task.CompletedTask;" from void methods in test files

for file in tests/TypedSqlBuilder.Tests/*.cs; do
    echo "Processing $file"
    
    # Use awk to remove return Task.CompletedTask; statements from void methods
    awk '
    BEGIN { in_void_method = 0 }
    
    # Check if we are entering a void method
    /public void/ { in_void_method = 1 }
    
    # Check if we are leaving a method (next method or class end)
    /public (Task|void)/ && !/public void/ && in_void_method { in_void_method = 0 }
    /^\s*}$/ && in_void_method { 
        # If this is the end of the void method, reset flag
        in_void_method = 0
    }
    
    # Skip return Task.CompletedTask; lines in void methods
    /return Task\.CompletedTask;/ && in_void_method { next }
    
    # Print all other lines
    { print }
    ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"
done

echo "Fixed all void methods"
