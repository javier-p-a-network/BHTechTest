using BHTechTest.Domain.ShareKernel.Results;
using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Entities;
using System;

namespace BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Services
{
    public interface ITodoListService
    {
        Result<TodoItem> GetItem(int id);
        Result<int> AddItem(string title, string description, string category);
        Result UpdateItem(int id, string description);
        Result RemoveItem(int id);
        Result RegisterProgression(int id, DateTime dateTime, decimal percent);
        Result PrintItems();
    }
}
