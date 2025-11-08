using System.Collections.Generic;

namespace BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Abstract
{
    public interface ITodoListRepository
    {
        int GetNextId();
        List<string> GetAllCategories();
    }
}
