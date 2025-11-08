using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot;
using BHTechTest.Infrastructure.ToDoListContext;
using System.Globalization;

class Program
{
    static void Main(string[] args)
    {
        // Initialize repository and aggregate
        var repo = new MemoryTodoListRepository();
        var todoList = new TodoList(repo);

        Console.WriteLine("Welcome to TodoList App");
        Console.WriteLine("Commands: add, update, remove, progress, print, exit");
        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;
            var parts = input.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLowerInvariant();
            var rest = parts.Length > 1 ? parts[1] : string.Empty;

            try
            {
                switch (cmd)
                {
                    case "add":
                        // format: add |title| |description| |category|
                        HandleAdd(todoList, rest);
                        break;
                    case "update":
                        // format: update id |description|
                        HandleUpdate(todoList, rest);
                        break;
                    case "remove":
                        // format: remove id
                        HandleRemove(todoList, rest);
                        break;
                    case "progress":
                        // format: progress id yyyy-MM-ddTHH:mm percent
                        HandleProgress(todoList, rest);
                        break;
                    case "print":
                        todoList.PrintItems();
                        break;
                    case "exit":
                        return;
                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }
    }

    static void HandleAdd(TodoList todoList, string args)
    {
        // We'll expect args in pipe separated: title|description|category
        var parts = args.Split('|', StringSplitOptions.TrimEntries);
        if (parts.Length < 3)
        {
            Console.WriteLine("Usage: add Title|Description|Category");
            return;
        }
        var id = todoList.GetNextId();
        todoList.AddItem(id, parts[0], parts[1], parts[2]);
        Console.WriteLine($"Added item id {id}");
    }

    static void HandleUpdate(TodoList todoList, string args)
    {
        var parts = args.Split(' ', 2, StringSplitOptions.TrimEntries);
        if (parts.Length < 2 || !int.TryParse(parts[0], out var id))
        {
            Console.WriteLine("Usage: update <id> New description");
            return;
        }
        todoList.UpdateItem(id, parts[1]);
        Console.WriteLine($"Updated item {id}");
    }

    static void HandleRemove(TodoList todoList, string args)
    {
        if (!int.TryParse(args.Trim(), out var id))
        {
            Console.WriteLine("Usage: remove <id>");
            return;
        }
        todoList.RemoveItem(id);
        Console.WriteLine($"Removed item {id}");
    }

    static void HandleProgress(TodoList todoList, string args)
    {
        // usage: progress id 2025-03-18T00:00 30
        var parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3 || !int.TryParse(parts[0], out var id))
        {
            Console.WriteLine("Usage: progress <id> <ISO-datetime> <percent>");
            return;
        }

        if (!DateTime.TryParse(parts[1], null, DateTimeStyles.RoundtripKind, out var dt))
        {
            Console.WriteLine("Invalid date. Use ISO format like 2025-03-18T00:00:00");
            return;
        }

        if (!decimal.TryParse(parts[2], out var percent))
        {
            Console.WriteLine("Invalid percent value.");
            return;
        }

        todoList.RegisterProgression(id, dt, percent);
        Console.WriteLine($"Registered progress for {id}");
    }
}