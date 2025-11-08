using BHTechTest.Domain.ShareKernel.BHTechTest.Domain.SharedKernel;
using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Abstract;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot
{
    public class TodoListAggregate : AggregateRoot, ITodoList
    {
        private readonly List<TodoItem> _items = new();
        private readonly ITodoListRepository _repository;

        public TodoListAggregate(ITodoListRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void AddItem(int id, string title, string description, string category)
        {
            // Validate category exists in repository
            var categories = _repository.GetAllCategories() ?? new List<string>();
            if (!categories.Contains(category))
                throw new InvalidOperationException($"Category '{category}' is not valid.");

            if (_items.Any(i => i.Id == id))
                throw new InvalidOperationException($"An item with id {id} already exists.");

            var item = new TodoItem(id, title, description, category);
            _items.Add(item);
        }

        public void UpdateItem(int id, string description)
        {
            var item = FindItemOrThrow(id);
            // Not allowed to update if >50% completed
            if (item.LatestProgressPercent > 50m)
                throw new InvalidOperationException("Cannot update an item with progress greater than 50%.");

            item.UpdateDescription(description);
        }

        public void RemoveItem(int id)
        {
            var item = FindItemOrThrow(id);
            if (item.LatestProgressPercent > 50m)
                throw new InvalidOperationException("Cannot remove an item with progress greater than 50%.");
            _items.Remove(item);
        }

        public void RegisterProgression(int id, DateTime dateTime, decimal percent)
        {
            var item = FindItemOrThrow(id);
            item.AddProgression(new Progression(dateTime, percent));
        }

        public void PrintItems()
        {
            var ordered = _items.OrderBy(i => i.Id).ToList();
            var culture = CultureInfo.CreateSpecificCulture("en-US");

            foreach (var item in ordered)
            {
                Console.WriteLine($"{item.Id}) {item.Title} - {item.Description} ({item.Category}) Completed:{item.IsCompleted}.");
                foreach (var p in item.Progressions)
                {
                    var pct = p.Percent;
                    var bar = BuildProgressBar(pct);
                    // Example date format: 3/18/2025 12:00:00 AM
                    Console.WriteLine($"{p.DateTime.ToString(culture)} - {Decimal.ToInt32(pct)}% {bar}");
                }
            }
        }

        private string BuildProgressBar(decimal percent)
        {
            // Build a visual bar similar to example.
            // We'll use a fixed max width (50) and fill proportionally with 'O'.
            int maxWidth = 50;
            int filled = (int)Math.Round((double)(percent / 100m * maxWidth));
            if (filled < 0) filled = 0;
            if (filled > maxWidth) filled = maxWidth;
            var bar = new string('O', filled).PadRight(maxWidth, ' ');
            return $"|{bar}|";
        }

        private TodoItem FindItemOrThrow(int id)
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item == null) throw new InvalidOperationException($"Item with id {id} not found.");
            return item;
        }

        // Helper to create new id via repository
        public int GetNextId() => _repository.GetNextId();

        // For tests and usage: ability to seed items or query items (internal)
        public IReadOnlyList<TodoItem> Items => _items.AsReadOnly();
    }
}
