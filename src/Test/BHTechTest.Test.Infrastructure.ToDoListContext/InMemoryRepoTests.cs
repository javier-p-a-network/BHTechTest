using BHTechTest.Infrastructure.ToDoListContext;
using System.Collections.Generic;

namespace BHTechTest.Test.Infrastructure.ToDoListContext
{
    [TestClass]
    public class InMemoryRepoTests
    {
        [TestMethod]
        public void GetNextId_increments()
        {
            var repo = new InMemoryTodoListRepository();
            var a = repo.GetNextId();
            var b = repo.GetNextId();
            Assert.AreEqual(a + 1, b);
        }

        [TestMethod]
        public void Default_categories_contain_work()
        {
            var repo = new InMemoryTodoListRepository();
            var cats = repo.GetAllCategories();
            CollectionAssert.Contains(cats, "Work");
        }

        [TestMethod]
        public void Can_supply_custom_categories()
        {
            var repo = new InMemoryTodoListRepository(new List<string> { "Custom" });
            var cats = repo.GetAllCategories();
            CollectionAssert.Contains(cats, "Custom");
            CollectionAssert.DoesNotContain(cats, "Work");
        }
    }
}
