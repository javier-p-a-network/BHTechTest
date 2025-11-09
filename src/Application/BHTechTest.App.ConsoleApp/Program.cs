using BHTechTest.App.AppServices;
using BHTechTest.App.ConsoleApp;

class Program
{
    private static void Main(string[] args)
    {
        //TODO: Change to dependency injection
        var outputService = new ConsoleOutputService();
        var appService = new TodoListAppService(outputService);

        Console.WriteLine("Welcome to TodoList App");
        Console.WriteLine("Commands:");
        var commands = new[]
        {
            $"{ConsoleCommands.LIST_CATEGORIES}                     - List available categories",
            $"{ConsoleCommands.ADD} Title|Description|Category      - Add a new todo item",
            $"{ConsoleCommands.UPDATE} <id> New description         - Update an existing item's description",
            $"{ConsoleCommands.REMOVE} <id>                         - Remove an existing item",
            $"{ConsoleCommands.PROGRESS} <id> Date Percent          - Register progress for an item",
            $"{ConsoleCommands.PRINT}                               - Print all todo items",
            $"{ConsoleCommands.EXIT}                                - Exit the application"
        };
        foreach (var cmd in commands)
            Console.WriteLine(cmd);

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
                    case ConsoleCommands.LIST_CATEGORIES: appService.PrintCategories(); break;
                    case ConsoleCommands.ADD: appService.Add(rest); break;
                    case ConsoleCommands.UPDATE: appService.Update(rest); break;
                    case ConsoleCommands.REMOVE: appService.Remove(rest); break;
                    case ConsoleCommands.PROGRESS: appService.Progress(rest); break;
                    case ConsoleCommands.PRINT: appService.PrintItems(); break;
                    case ConsoleCommands.EXIT: return;
                    default: Console.WriteLine("Unknown command."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }
    }
}