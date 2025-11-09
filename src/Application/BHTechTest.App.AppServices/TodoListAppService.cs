using BHTechTest.Domain.ShareKernel;
using BHTechTest.Domain.ShareKernel.Results;
using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Services;
using BHTechTest.Infrastructure.ToDoListContext;
using System;
using System.Globalization;

namespace BHTechTest.App.AppServices
{
    public class TodoListAppService : ITodoListAppService
    {
        private readonly TodoListService _todoListService;
        private readonly IOutputService _outputService;

        public TodoListAppService(IOutputService outputService)
        {
            _outputService = outputService;
            var repo = new MemoryTodoListRepository();
            _todoListService = new TodoListService(repo, _outputService);
        }

        public void Add(string args)
        {
            try
            {
                var parts = args.Split('|', StringSplitOptions.TrimEntries);
                if (parts.Length < 3)
                {
                    _outputService.WriteLine("Usage: add Title|Description|Category");
                    return;
                }

                var result = _todoListService.AddItem(parts[0], parts[1], parts[2]);
                if (result.HasErrors)
                {
                    _outputService.Write(result);
                    return;
                }

                var id = result.Value;
                _outputService.WriteLine($"Added item id {id}");
            }
            catch (Exception ex)
            {
                var exResult = Result.CreateError(ex);
                _outputService.Write(exResult);
            }
        }

        public void Update(string args)
        {
            try
            {
                var parts = args.Split(' ', 2, StringSplitOptions.TrimEntries);
                if (parts.Length < 2 || !int.TryParse(parts[0], out var id))
                {
                    _outputService.WriteLine("Usage: update <id> New description");
                    return;
                }

                var updateResult = _todoListService.UpdateItem(id, parts[1]);
                if(updateResult.HasErrors)
                {
                    _outputService.Write(updateResult);
                    return;
                }

                _outputService.WriteLine($"Updated item {id}");
            }
            catch (Exception ex)
            {
                var exResult = Result.CreateError(ex);
                _outputService.Write(exResult);
            }
        }

        public void Remove(string args)
        {
            try
            {
                if (!int.TryParse(args.Trim(), out var id))
                {
                    _outputService.WriteLine("Usage: remove <id>");
                    return;
                }

                var removeResult = _todoListService.RemoveItem(id);
                if(removeResult.HasErrors)
                {
                    _outputService.Write(removeResult);
                    return;
                }

                _outputService.WriteLine($"Removed item {id}");
            }
            catch (Exception ex)
            {
                var exResult = Result.CreateError(ex);
                _outputService.Write(exResult);
            }
        }

        public void Progress(string args)
        {
            try
            {
                var parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3 || !int.TryParse(parts[0], out var id))
                {
                    _outputService.WriteLine("Usage: progress <id> <ISO-datetime> <percent> (en-US dot format)");
                    return;
                }

                if (!DateTime.TryParse(parts[1], null, DateTimeStyles.RoundtripKind, out var dt))
                {
                    _outputService.WriteLine("Invalid date. Use ISO format like 2025-03-18T00:00:00");
                    return;
                }

                if (!decimal.TryParse(parts[2], NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out var percent))
                {
                    _outputService.WriteLine("Invalid percent value. Use en-US format (e.g., 30.5)");
                    return;
                }

                var rpResult = _todoListService.RegisterProgression(id, dt, percent);
                if(rpResult.HasErrors)
                {
                    _outputService.Write(rpResult);
                    return;
                }

                _outputService.WriteLine($"Registered progress for {id}");
            }
            catch (Exception ex)
            {
                var exResult = Result.CreateError(ex);
                _outputService.Write(exResult);
            }
        }

        public void PrintItems()
        {
            try
            {
                var printResult = _todoListService.PrintItems();
                if(printResult.HasErrors)
                {
                    _outputService.Write(printResult);
                    return;
                }
            }
            catch (Exception ex)
            {
                var exResult = Result.CreateError(ex);
                _outputService.Write(exResult);
            }
        }

        public void PrintCategories()
        {
            try
            {
                var listResult = _todoListService.ListCategories();
                if (listResult.HasErrors)
                {
                    _outputService.Write(listResult);
                    return;
                }

                var categories = listResult.Value ?? [];
                foreach (var category in categories)
                    _outputService.WriteLine($"Category: {category}");
            }
            catch (Exception ex)
            {
                var exResult = Result.CreateError(ex);
                _outputService.Write(exResult);
            }
        }
    }
}

