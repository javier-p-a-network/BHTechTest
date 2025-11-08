using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Abstract;
using System.Collections.Generic;

namespace BHTechTest.Infrastructure.ToDoListContext
{
    public class InMemoryTodoListRepository : ITodoListRepository
    {
        private int _current = 0;
        private readonly List<string> _categories;

        public InMemoryTodoListRepository(List<string>? categories = null)
        {
            // Default categories if not provided
            _categories = categories ?? new List<string> { "Work", "Home", "Personal", "Other" };
        }

        public int GetNextId()
        {
            _current++;
            return _current;
        }

        public List<string> GetAllCategories()
        {
            // return a copy to avoid external mutation
            return new List<string>(_categories);
        }

        // For tests or setup
        public void AddCategory(string category)
        {
            if (!_categories.Contains(category))
                _categories.Add(category);
        }
    }
}
