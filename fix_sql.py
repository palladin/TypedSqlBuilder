#!/usr/bin/env python3

import re
import sys

def fix_sql_formatting(content):
    """Fix SQL formatting in C# test files"""
    
    # Pattern to match SQL strings in the expected format
    # This matches: """<newline>        SELECT <newline>            <projections><newline>        FROM <table><newline>        """;
    pattern = r'("""\s*\n\s*SELECT\s*\n\s*)(.*?)(\s*\n\s*FROM\s+)([^"]*?)(\s*\n\s*""")'
    
    def replace_projections(match):
        select_part = match.group(1)
        projections = match.group(2).strip()
        from_part = match.group(3)
        table_part = match.group(4).strip()
        end_part = match.group(5)
        
        # Split projections by comma and format each on its own line
        proj_list = [p.strip() for p in projections.split(',')]
        formatted_projections = ',\n            '.join(proj_list)
        
        # Ensure table is properly indented
        formatted_table_part = f"\n            {table_part}"
        
        return f"{select_part}            {formatted_projections}{from_part}{formatted_table_part}{end_part}"
    
    # Apply the transformation
    result = re.sub(pattern, replace_projections, content, flags=re.MULTILINE | re.DOTALL)
    
    return result

def main():
    if len(sys.argv) != 2:
        print("Usage: python3 fix_sql.py <filename>")
        sys.exit(1)
    
    filename = sys.argv[1]
    
    try:
        with open(filename, 'r') as f:
            content = f.read()
        
        fixed_content = fix_sql_formatting(content)
        
        with open(filename, 'w') as f:
            f.write(fixed_content)
        
        print(f"Fixed SQL formatting in {filename}")
    
    except Exception as e:
        print(f"Error: {e}")
        sys.exit(1)

if __name__ == "__main__":
    main()
