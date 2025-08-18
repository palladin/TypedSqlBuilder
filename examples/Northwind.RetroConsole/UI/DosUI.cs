using Spectre.Console;
using Spectre.Console.Rendering;
using System.Linq;
using System.Text;

namespace Northwind.RetroConsole.UI;

public static class DosTheme
{
    // Classic MS-DOS colors (black background only)
    public static readonly Style Background = new(Color.White, Color.Black);
    public static readonly Style Text = new(Color.White, Color.Black);
    public static readonly Style Highlight = new(Color.White, Color.Black);
    public static readonly Style CursorBar = new(Color.White, Color.Black);
    public static readonly Style Header = new(Color.White, Color.Black);
    public static readonly Style Border = new(Color.White, Color.Black);
    public static readonly Style Status = new(Color.White, Color.Black);
    public static readonly Style Error = new(Color.White, Color.Black);
    public static readonly Style Success = new(Color.White, Color.Black);
    
    public static void InitializeConsole()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Clear();
        Console.CursorVisible = false;
    }
    
    public static void RestoreConsole()
    {
        Console.ResetColor();
        Console.Clear();
        Console.CursorVisible = true;
    }
}

public static class DosUI
{
    /// <summary>
    /// Gets the current console width, with a fallback for environments where this isn't available
    /// </summary>
    public static int GetConsoleWidth()
    {
        try
        {
            return Console.WindowWidth;
        }
        catch
        {
            return 80; // Fallback to classic 80-column width
        }
    }
    
    /// <summary>
    /// Gets the current console height, with a fallback for environments where this isn't available
    /// </summary>
    public static int GetConsoleHeight()
    {
        try
        {
            return Console.WindowHeight;
        }
        catch
        {
            return 25; // Fallback to classic 25-line height
        }
    }
    
    public static string CreateHeader(string title, int? width = null)
    {
        var actualWidth = width ?? GetConsoleWidth() - 2; // Leave 2 chars margin
        var padding = Math.Max(0, (actualWidth - title.Length) / 2);
        var paddedTitle = title.PadLeft(padding + title.Length).PadRight(actualWidth);
        
        return $"{new string(' ', actualWidth)}\n" +
               $"{paddedTitle}\n" +
               $"{new string(' ', actualWidth)}\n" +
               $"{new string('═', actualWidth)}\n" +
               $"{new string(' ', actualWidth)}";
    }
    
    public static string CreateTableHeader(string title, string[] columns, int? width = null)
    {
        var actualWidth = width ?? GetConsoleWidth() - 2;
        var header = CreateHeader(title, actualWidth);
        
        // Calculate column widths dynamically based on available space
        var availableWidth = actualWidth - 4; // Leave 4 chars for padding
        var baseColWidth = availableWidth / columns.Length;
        var lastColWidth = availableWidth - (baseColWidth * (columns.Length - 1));
        
        var columnHeader = "  " + string.Join(" ", columns.Select((col, i) => 
        {
            var colWidth = i == columns.Length - 1 ? lastColWidth : baseColWidth;
            return col.Length > colWidth ? col[..colWidth] : col.PadRight(colWidth);
        }));
        
        return header + 
               $"{columnHeader.PadRight(actualWidth)}\n" +
               $"{new string('─', actualWidth)}";
    }
    
    public static Panel CreateDialog(string title, string content, int? width = null)
    {
        var actualWidth = width ?? Math.Min(GetConsoleWidth() - 10, 60); // Max 60, or console width - 10
        
        return new Panel(content)
        {
            Header = new PanelHeader($"═══ {title} ═══"),
            Border = BoxBorder.Double,
            Padding = new Padding(2, 1),
            Width = actualWidth
        };
    }
    
    public static Panel CreateDialog(string title, IRenderable content)
    {
        return new Panel(content)
        {
            Header = new PanelHeader($"═══ {title} ═══"),
            Border = BoxBorder.Double,
            Padding = new Padding(2, 1)
        };
    }
    
    /// <summary>
    /// Shows a renderable object centered on the screen
    /// </summary>
    public static void ShowCentered(IRenderable renderable)
    {
        var layout = new Layout("Root")
            .SplitColumns(
                new Layout("Left"),
                new Layout("Center"), 
                new Layout("Right"));
            
        layout["Center"].Update(renderable);
        layout["Left"].Invisible();
        layout["Right"].Invisible();
        
        AnsiConsole.Write(layout);
    }
    
    /// <summary>
    /// Shows a renderable object centered vertically and horizontally
    /// </summary>
    public static void ShowCenteredDialog(IRenderable renderable)
    {
        // Clear the screen efficiently
        Console.SetCursorPosition(0, 0);
        Console.Clear();
        
        var consoleHeight = GetConsoleHeight();
        var consoleWidth = GetConsoleWidth();
        
        // Calculate vertical padding to center the dialog
        var verticalPadding = Math.Max(1, (consoleHeight / 2) - 8);
        
        // Add vertical spacing
        for (int i = 0; i < verticalPadding; i++)
        {
            Console.WriteLine();
        }
        
        // Center horizontally and write in one operation
        AnsiConsole.Write(Align.Center(renderable));
        
        // Hide cursor to prevent blinking
        Console.CursorVisible = false;
    }
    
    public static void ShowCenteredDialogUp(IRenderable renderable)
    {
        // Clear the screen efficiently
        Console.SetCursorPosition(0, 0);
        Console.Clear();
        
        var consoleHeight = GetConsoleHeight();
        var consoleWidth = GetConsoleWidth();
        
        // Calculate vertical padding to position dialog higher up
        var verticalPadding = Math.Max(1, (consoleHeight / 3) - 6);
        
        // Add vertical spacing
        for (int i = 0; i < verticalPadding; i++)
        {
            Console.WriteLine();
        }
        
        // Center horizontally and write in one operation
        AnsiConsole.Write(Align.Center(renderable));
        
        // Hide cursor to prevent blinking
        Console.CursorVisible = false;
    }
    
    public static void ShowStatusBar(string message, int? width = null)
    {
        var actualWidth = width ?? GetConsoleWidth();
        var statusLine = message.Length > actualWidth ? message[..actualWidth] : message.PadRight(actualWidth);
        Console.SetCursorPosition(0, GetConsoleHeight() - 1);
        AnsiConsole.Write($"{statusLine}");
    }
    
    public static void ShowError(string title, string message)
    {
        var dialog = new Panel($"{message}\n\nPress any key to continue...")
        {
            Header = new PanelHeader($"═══ {title} ═══"),
            Border = BoxBorder.Double,
            Padding = new Padding(2, 1)
        };
        
        ShowCenteredDialog(dialog);
        Console.ReadKey(true);
    }
    
    public static bool Confirm(string title, string message)
    {
        var dialog = new Panel($"{message}\n\nPress Y to confirm, N to cancel")
        {
            Header = new PanelHeader($"═══ {title} ═══"),
            Border = BoxBorder.Double,
            Padding = new Padding(2, 1)
        };
        
        DosTheme.InitializeConsole(); // Clear screen
        ShowCenteredDialog(dialog);
        
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Y) return true;
            if (key.Key == ConsoleKey.N || key.Key == ConsoleKey.Escape) return false;
        }
    }

    public static string AskForInput(string title, string prompt, bool allowEmpty = false)
    {
        var input = new StringBuilder();
        
        void RenderDialog()
        {
            var content = $"{prompt}\n\n> {input}█";
            var dialog = new Panel(content)
            {
                Header = new PanelHeader($"═══ {title} ═══"),
                Border = BoxBorder.Double,
                Padding = new Padding(2, 1)
            };
            
            DosTheme.InitializeConsole(); // Clear screen
            ShowCenteredDialog(dialog);
        }
        
        RenderDialog();
        
        while (true)
        {
            var key = Console.ReadKey(true);
            
            if (key.Key == ConsoleKey.Enter)
            {
                if (allowEmpty || input.Length > 0)
                    return input.ToString();
                // If empty and not allowed, just continue
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                return string.Empty;
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (input.Length > 0)
                {
                    input.Length--;
                    RenderDialog();
                }
            }
            else if (key.KeyChar >= 32 && key.KeyChar < 127) // Printable ASCII
            {
                input.Append(key.KeyChar);
                RenderDialog();
            }
        }
    }

    public static Dictionary<string, string> ShowFormDialog(string title, List<FormField> fields)
    {
        var values = new Dictionary<string, string>();
        foreach (var field in fields)
        {
            values[field.Key] = field.DefaultValue ?? "";
        }

        int currentFieldIndex = 0;
        bool isEditing = false;
        var editBuffer = new StringBuilder();
        string lastRenderedContent = "";
        
        // Add submit and cancel as virtual fields
        var totalItems = fields.Count + 2; // fields + submit + cancel
        
        void RenderForm()
        {
            var content = new StringBuilder();
            content.AppendLine("↑↓ Navigate    ENTER/TAB Next    Type to edit");
            content.AppendLine("ESC=Cancel");
            content.AppendLine();
            
            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                var value = values[field.Key];
                var isSelected = i == currentFieldIndex;
                
                var prefix = isSelected ? "► " : "  ";
                var displayValue = string.IsNullOrEmpty(value) ? "" : value;
                
                if (isSelected && isEditing)
                {
                    content.AppendLine($"{prefix}{field.Label}:");
                    content.AppendLine($"   {editBuffer}█");
                }
                else if (isSelected)
                {
                    content.AppendLine($"{prefix}{field.Label}:");
                    content.AppendLine($"   {displayValue}█");
                }
                else
                {
                    content.AppendLine($"{prefix}{field.Label}:");
                    content.AppendLine($"   {displayValue}");
                }
                
                if (i < fields.Count - 1) content.AppendLine();
            }
            
            // Add submit and cancel buttons
            content.AppendLine();
            content.AppendLine("═══════════════════════════════");
            
            var submitSelected = currentFieldIndex == fields.Count;
            var cancelSelected = currentFieldIndex == fields.Count + 1;
            
            var submitPrefix = submitSelected ? "► " : "  ";
            var cancelPrefix = cancelSelected ? "► " : "  ";
            
            if (submitSelected)
            {
                content.AppendLine($"{submitPrefix}« SUBMIT FORM »█");
            }
            else
            {
                content.AppendLine($"{submitPrefix}« SUBMIT FORM »");
            }
            
            if (cancelSelected)
            {
                content.AppendLine($"{cancelPrefix}« CANCEL »█");
            }
            else
            {
                content.AppendLine($"{cancelPrefix}« CANCEL »");
            }
            
            var newContent = content.ToString().TrimEnd();
            
            // Only redraw if content actually changed
            if (newContent != lastRenderedContent)
            {
                var dialog = new Panel(newContent)
                {
                    Header = new PanelHeader($"═══ {title} ═══"),
                    Border = BoxBorder.Double,
                    Padding = new Padding(2, 1)
                };
                
                DosTheme.InitializeConsole();
                ShowCenteredDialog(dialog);
                lastRenderedContent = newContent;
                
                // Small delay to prevent rapid flickering
                Thread.Sleep(15);
            }
        }
        
        RenderForm();
        
        while (true)
        {
            var key = Console.ReadKey(true);
            
            if (isEditing)
            {
                // Handle editing mode
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        values[fields[currentFieldIndex].Key] = editBuffer.ToString();
                        isEditing = false;
                        // Auto-advance to next field after editing
                        currentFieldIndex = (currentFieldIndex + 1) % totalItems;
                        RenderForm();
                        break;
                        
                    case ConsoleKey.Escape:
                        isEditing = false;
                        RenderForm();
                        break;
                        
                    case ConsoleKey.Backspace:
                        if (editBuffer.Length > 0)
                        {
                            editBuffer.Length--;
                            RenderForm();
                        }
                        break;
                        
                    default:
                        if (key.KeyChar >= 32 && key.KeyChar < 127) // Printable ASCII
                        {
                            editBuffer.Append(key.KeyChar);
                            RenderForm();
                        }
                        break;
                }
            }
            else
            {
                // Handle navigation mode
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        currentFieldIndex = (currentFieldIndex - 1 + totalItems) % totalItems;
                        RenderForm();
                        break;
                        
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.Tab:
                        currentFieldIndex = (currentFieldIndex + 1) % totalItems;
                        RenderForm();
                        break;
                        
                    case ConsoleKey.Enter:
                        if (currentFieldIndex < fields.Count)
                        {
                            // Move to next field instead of editing
                            currentFieldIndex = (currentFieldIndex + 1) % totalItems;
                            RenderForm();
                        }
                        else if (currentFieldIndex == fields.Count)
                        {
                            // Submit button pressed
                            var missingRequired = fields
                                .Where(f => f.Required && string.IsNullOrWhiteSpace(values[f.Key]))
                                .ToList();
                            
                            if (missingRequired.Any())
                            {
                                var missing = string.Join(", ", missingRequired.Select(f => f.Label));
                                ShowError("VALIDATION ERROR", $"Required fields missing: {missing}");
                                RenderForm();
                            }
                            else
                            {
                                return values;
                            }
                        }
                        else if (currentFieldIndex == fields.Count + 1)
                        {
                            // Cancel button pressed
                            return new Dictionary<string, string>(); // Empty = cancelled
                        }
                        break;
                        
                    case ConsoleKey.Backspace:
                        // Start editing with backspace if on a field
                        if (currentFieldIndex < fields.Count)
                        {
                            editBuffer.Clear();
                            var currentValue = values[fields[currentFieldIndex].Key];
                            if (!string.IsNullOrEmpty(currentValue))
                            {
                                editBuffer.Append(currentValue[..^1]); // Remove last character
                            }
                            isEditing = true;
                            RenderForm();
                        }
                        break;
                        
                    case ConsoleKey.F1:
                        // Keep F1 as shortcut for submit
                        var missingRequiredF1 = fields
                            .Where(f => f.Required && string.IsNullOrWhiteSpace(values[f.Key]))
                            .ToList();
                        
                        if (missingRequiredF1.Any())
                        {
                            var missing = string.Join(", ", missingRequiredF1.Select(f => f.Label));
                            ShowError("VALIDATION ERROR", $"Required fields missing: {missing}");
                            RenderForm();
                        }
                        else
                        {
                            return values;
                        }
                        break;
                        
                    case ConsoleKey.Escape:
                        return new Dictionary<string, string>(); // Empty = cancelled
                        
                    default:
                        // Auto-start editing when typing any printable character on a field
                        if (currentFieldIndex < fields.Count && key.KeyChar >= 32 && key.KeyChar < 127)
                        {
                            editBuffer.Clear();
                            editBuffer.Append(key.KeyChar);
                            isEditing = true;
                            RenderForm();
                        }
                        break;
                }
            }
        }
    }
}

public class FormField
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public bool Required { get; set; } = false;
    public string? DefaultValue { get; set; }
}

public class MenuOption
{
    public string Key { get; set; } = string.Empty;
    public string Display { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Action? Action { get; set; }
}

public static class DosMenu
{
    public static T SelectFromList<T>(string title, List<T> items, Func<T, string> formatter, 
        string instructions = "↑↓ Navigate    ENTER Select    ESC Back") where T : notnull
    {
        var selectedIndex = 0;
        bool needsRedraw = true;
        
        while (true)
        {
            if (needsRedraw)
            {
                // Build the menu content
                var menuItems = new List<string>();
                for (int i = 0; i < items.Count; i++)
                {
                    var item = formatter(items[i]);
                    if (i == selectedIndex)
                    {
                        menuItems.Add($"> {item}");
                    }
                    else
                    {
                        menuItems.Add($"  {item}");
                    }
                }
                
                var content = string.Join("\n", menuItems) + $"\n\n{instructions}";
                var dialog = DosUI.CreateDialog(title, content);
                DosUI.ShowCenteredDialog(dialog);
                needsRedraw = false;
            }
            
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = selectedIndex == 0 ? items.Count - 1 : selectedIndex - 1;
                    needsRedraw = true;
                    Thread.Sleep(10); // Small delay to prevent rapid navigation flicker
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = selectedIndex == items.Count - 1 ? 0 : selectedIndex + 1;
                    needsRedraw = true;
                    Thread.Sleep(10); // Small delay to prevent rapid navigation flicker
                    break;
                case ConsoleKey.Enter:
                    return items[selectedIndex];
                case ConsoleKey.Escape:
                    throw new OperationCanceledException("User cancelled selection");
            }
        }
    }
    
    public static string ShowMainMenu()
    {
        var options = new[]
        {
            "CUSTOMER RECORDS",
            "PRODUCT CATALOG",
            "ORDER MANAGEMENT", 
            "EMPLOYEE DATABASE",
            "SALES REPORTS",
            "INVENTORY STATUS",
            "SYSTEM UTILITIES",
            "EXIT PROGRAM"
        };
        
        return SelectFromList("PALLADIN SHOP v1.0", options.ToList(), option => option);
    }
    
    public static MenuOption ShowMenuWithOptions(string title, MenuOption[] options)
    {
        return SelectFromList(title, options.ToList(), option => $"{option.Display,-4} {option.Description}");
    }
}
