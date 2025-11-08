using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot;
using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Abstract;
using System;
using System.Linq;

namespace BHTechTest.Test.Domain.ToDoListContext
{
    [TestClass]
    public class TodoListDomainTests
    {
        [TestMethod]
        public void HappyPath_example_and_print_like_sample()
        {
            var repo = default(ITodoListRepository);// new InMemoryTodoListRepository(new List<string> { "Work" });
            var todoList = new TodoList(repo);

            var id = todoList.GetNextId();
            todoList.AddItem(id, "Complete Project Report", "Finish the final report for the project", "Work");

            todoList.RegisterProgression(id, new DateTime(2025, 3, 18), 30m);
            todoList.RegisterProgression(id, new DateTime(2025, 3, 19), 80m);
            todoList.RegisterProgression(id, new DateTime(2025, 3, 20), 100m);

            var item = todoList.Items.Single(i => i.Id == id);
            Assert.IsTrue(item.IsCompleted);
            Assert.AreEqual(3, item.Progressions.Count);
            Assert.AreEqual(100m, item.Progressions.Last().Percent);
        }

        [TestMethod]
        public void Cannot_add_progression_with_lower_or_equal_date()
        {
            var repo = default(ITodoListRepository); //new InMemoryTodoListRepository(new System.Collections.Generic.List<string> { "Work" });
            var todoList = new TodoList(repo);
            var id = todoList.GetNextId();
            todoList.AddItem(id, "Task", "Desc", "Work");
            todoList.RegisterProgression(id, new DateTime(2025, 1, 1), 10m);

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoList.RegisterProgression(id, new DateTime(2025, 1, 1), 20m));

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoList.RegisterProgression(id, new DateTime(2024, 12, 31), 20m));
        }

        [TestMethod]
        public void Cannot_register_progression_with_invalid_percent_or_non_increasing_percent()
        {
            var repo = default(ITodoListRepository); //new InMemoryTodoListRepository(new System.Collections.Generic.List<string> { "Work" });
            var todoList = new TodoList(repo);
            var id = todoList.GetNextId();
            todoList.AddItem(id, "Task", "Desc", "Work");

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoList.RegisterProgression(id, DateTime.UtcNow.AddDays(1), 0m));

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoList.RegisterProgression(id, DateTime.UtcNow.AddDays(1), 120m));

            todoList.RegisterProgression(id, DateTime.UtcNow.AddDays(1), 30m);

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoList.RegisterProgression(id, DateTime.UtcNow.AddDays(2), 20m));
        }

        [TestMethod]
        public void Cannot_update_or_delete_item_with_progress_more_than_50_percent()
        {
            var repo = default(ITodoListRepository); //new InMemoryTodoListRepository(new System.Collections.Generic.List<string> { "Work" });
            var todoList = new TodoList(repo);
            var id = todoList.GetNextId();
            todoList.AddItem(id, "Task", "Desc", "Work");

            todoList.RegisterProgression(id, new DateTime(2025, 1, 1), 51m);

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoList.UpdateItem(id, "NewDesc"));

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoList.RemoveItem(id));
        }
    }
}
