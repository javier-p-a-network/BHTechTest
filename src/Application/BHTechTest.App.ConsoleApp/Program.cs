using BHTechTest.App.ConsoleApp;
using BHTechTest.Domain.ShareKernel;
using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Services;
using BHTechTest.Infrastructure.ToDoListContext;
using System.Globalization;

class Program
{
    private static void Main(string[] args)
    {
        // Initialize repository and aggregate
        var outputService = new OutputService();
        var repo = new MemoryTodoListRepository();
        var todoListService = new TodoListService(repo, outputService);


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
                        HandleAdd(todoListService, rest, outputService);
                        break;
                    case "update":
                        // format: update id |description|
                        HandleUpdate(todoListService, rest, outputService);
                        break;
                    case "remove":
                        // format: remove id
                        HandleRemove(todoListService, rest, outputService);
                        break;
                    case "progress":
                        // format: progress id yyyy-MM-ddTHH:mm percent
                        HandleProgress(todoListService, rest, outputService);
                        break;
                    case "print":
                        HandlePrintItems(todoListService);
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

    private static void HandleAdd(TodoListService todoListService, string args, IOutputService _outputService)
    {
        // We'll expect args in pipe separated: title|description|category
        var parts = args.Split('|', StringSplitOptions.TrimEntries);
        if (parts.Length < 3)
        {
            _outputService.WriteLine("Usage: add Title|Description|Category");
            return;
        }
        var result = todoListService.AddItem(parts[0], parts[1], parts[2]);
        if (result.HasErrors)
        {
            _outputService.Write(result);
            return;
        }

        var id = result.Value;
        _outputService.WriteLine($"Added item id {id}");
    }

    private static void HandleUpdate(TodoListService todoListService, string args, IOutputService _outputService)
    {
        var parts = args.Split(' ', 2, StringSplitOptions.TrimEntries);
        if (parts.Length < 2 || !int.TryParse(parts[0], out var id))
        {
            _outputService.WriteLine("Usage: update <id> New description");
            return;
        }
        todoListService.UpdateItem(id, parts[1]);
        _outputService.WriteLine($"Updated item {id}");
    }

    private static void HandleRemove(TodoListService todoListService, string args, IOutputService _outputService)
    {
        if (!int.TryParse(args.Trim(), out var id))
        {
            _outputService.WriteLine("Usage: remove <id>");
            return;
        }
        todoListService.RemoveItem(id);
        _outputService.WriteLine($"Removed item {id}");
    }

    private static void HandleProgress(TodoListService todoListService, string args, IOutputService _outputService)
    {
        // usage: progress id 2025-03-18T00:00 30
        var parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3 || !int.TryParse(parts[0], out var id))
        {
            _outputService.WriteLine("Usage: progress <id> <ISO-datetime> <percent>");
            return;
        }

        if (!DateTime.TryParse(parts[1], null, DateTimeStyles.RoundtripKind, out var dt))
        {
            _outputService.WriteLine("Invalid date. Use ISO format like 2025-03-18T00:00:00");
            return;
        }

        if (!decimal.TryParse(parts[2], out var percent))
        {
            Console.WriteLine("Invalid percent value.");
            return;
        }

        todoListService.RegisterProgression(id, dt, percent);
        _outputService.WriteLine($"Registered progress for {id}");
    }

    private static void HandlePrintItems(TodoListService todoListService) =>
        todoListService.PrintItems();
}