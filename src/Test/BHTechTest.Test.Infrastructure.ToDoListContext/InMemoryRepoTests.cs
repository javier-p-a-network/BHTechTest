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
            //Arrange
            var repo = new MemoryTodoListRepository();

            //Act
            var a = repo.GetNextId();
            var b = repo.GetNextId();

            //Assert
            Assert.AreEqual(a + 1, b);
        }

        [TestMethod]
        public void Default_categories_contain_work()
        {
            //Arrange
            var repo = new MemoryTodoListRepository();

            //Act
            var cats = repo.GetAllCategories();

            //Assert
            CollectionAssert.Contains(cats, "Work");
        }

        [TestMethod]
        public void Can_supply_custom_categories()
        {
            //Arrange
            var repo = new MemoryTodoListRepository(new List<string> { "Custom" });

            //Act
            var cats = repo.GetAllCategories();

            //Assert
            CollectionAssert.Contains(cats, "Custom");
            CollectionAssert.DoesNotContain(cats, "Work");
        }
    }
}
