using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Entities;
using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Repositories;
using System;
using System.Collections.Generic;

namespace BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Services
{
    public class TodoListService : ITodoListService
    {
        private readonly ITodoListRepository _repository;
        private readonly TodoList _todoList = new();

        public TodoListService(ITodoListRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public TodoList TodoList => _todoList;

        public void AddItem(string title, string description, string category)
        {
            var isValid = ValidateCategory(category);
            if (!isValid)
                throw new InvalidOperationException($"Category '{category}' is not valid.");

            var id = _repository.GetNextId();

            _todoList.AddItem(id, title, description, category);
        }
        private bool ValidateCategory(string category)
        {
            var categories = _repository.GetAllCategories() ?? new List<string>();
            return categories.Contains(category);
        }

    }
}
