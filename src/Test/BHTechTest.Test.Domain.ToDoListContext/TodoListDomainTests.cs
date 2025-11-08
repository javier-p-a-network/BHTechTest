using BHTechTest.Domain.ShareKernel;
using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Repositories;
using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Services;
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
            var outputService = default(IOutputService); // new TestOutputService();
            var todoListService = new TodoListService(repo, outputService);

            var result = todoListService.AddItem("Complete Project Report", "Finish the final report for the project", "Work");
            var id = result.Value;

            todoListService.RegisterProgression(id, new DateTime(2025, 3, 18), 30m);
            todoListService.RegisterProgression(id, new DateTime(2025, 3, 19), 80m);
            todoListService.RegisterProgression(id, new DateTime(2025, 3, 20), 100m);

            var getResult = todoListService.GetItem(id);
            var item = getResult.Value;
            Assert.IsTrue(item?.IsCompleted);
            Assert.AreEqual(3, item?.Progressions.Count);
            Assert.AreEqual(100m, item?.Progressions.Last().Percent);
        }

        [TestMethod]
        public void Cannot_add_progression_with_lower_or_equal_date()
        {
            var repo = default(ITodoListRepository);// new InMemoryTodoListRepository(new List<string> { "Work" });
            var outputService = default(IOutputService); // new TestOutputService();
            var todoListService = new TodoListService(repo, outputService);
            var result = todoListService.AddItem("Task", "Desc", "Work");
            var id = result.Value;
            todoListService.RegisterProgression(id, new DateTime(2025, 1, 1), 10m);

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoListService.RegisterProgression(id, new DateTime(2025, 1, 1), 20m));

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoListService.RegisterProgression(id, new DateTime(2024, 12, 31), 20m));
        }

        [TestMethod]
        public void Cannot_register_progression_with_invalid_percent_or_non_increasing_percent()
        {
            var repo = default(ITodoListRepository);// new InMemoryTodoListRepository(new List<string> { "Work" });
            var outputService = default(IOutputService); // new TestOutputService();
            var todoListService = new TodoListService(repo, outputService);
            var result = todoListService.AddItem("Task", "Desc", "Work");
            var id = result.Value;

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoListService.RegisterProgression(id, DateTime.UtcNow.AddDays(1), 0m));

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoListService.RegisterProgression(id, DateTime.UtcNow.AddDays(1), 120m));

            todoListService.RegisterProgression(id, DateTime.UtcNow.AddDays(1), 30m);

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoListService.RegisterProgression(id, DateTime.UtcNow.AddDays(2), 20m));
        }

        [TestMethod]
        public void Cannot_update_or_delete_item_with_progress_more_than_50_percent()
        {
            var repo = default(ITodoListRepository);// new InMemoryTodoListRepository(new List<string> { "Work" });
            var outputService = default(IOutputService); // new TestOutputService();
            var todoListService = new TodoListService(repo, outputService);
            var result = todoListService.AddItem("Task", "Desc", "Work");
            var id = result.Value;

            todoListService.RegisterProgression(id, new DateTime(2025, 1, 1), 51m);

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoListService.UpdateItem(id, "NewDesc"));

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoListService.RemoveItem(id));
        }
    }
}
