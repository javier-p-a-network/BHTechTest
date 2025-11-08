using System.Collections.Generic;

namespace BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Repositories
{
    public interface ITodoListRepository
    {
        int GetNextId();
        List<string> GetAllCategories();
    }
}
