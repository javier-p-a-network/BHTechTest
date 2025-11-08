using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Entities;

namespace BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Services
{
    public interface ITodoListService
    {
        void AddItem(string title, string description, string category);
        TodoList TodoList { get; }
    }
}
