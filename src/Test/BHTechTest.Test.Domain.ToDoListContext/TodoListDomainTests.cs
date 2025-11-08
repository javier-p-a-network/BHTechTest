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
            //Arrange
            var repo = default(ITodoListRepository);// new InMemoryTodoListRepository(new List<string> { "Work" });
            var outputService = default(IOutputService); // new TestOutputService();
            var todoListService = new TodoListService(repo, outputService);

            //Act
            var addItemResult = todoListService.AddItem("Complete Project Report", "Finish the final report for the project", "Work");
            var addItemHasErrors = addItemResult.HasErrors;
            var id = addItemResult.Value;

            var rprog30Result = todoListService.RegisterProgression(id, new DateTime(2025, 3, 18), 30m);
            var rprog80Result = todoListService.RegisterProgression(id, new DateTime(2025, 3, 19), 80m);
            var rprog100Result = todoListService.RegisterProgression(id, new DateTime(2025, 3, 20), 100m);

            var rprog30HasErrors = rprog30Result.HasErrors;
            var rprog80HasErrors = rprog80Result.HasErrors;
            var rprog100HasErrors = rprog80Result.HasErrors;

            var getResult = todoListService.GetItem(id);
            var getHasErrors = getResult.HasErrors;
            var item = getResult.Value;

            //Assert
            Assert.IsFalse(addItemHasErrors);
            Assert.IsFalse(rprog30HasErrors);
            Assert.IsFalse(rprog80HasErrors);
            Assert.IsFalse(rprog100HasErrors);
            Assert.IsFalse(getHasErrors);
            Assert.IsNotNull(item);
            Assert.IsTrue(item?.IsCompleted);
            Assert.AreEqual(3, item?.Progressions.Count);
            Assert.AreEqual(100m, item?.Progressions.Last().Percent);
        }

        [TestMethod]
        public void Cannot_add_progression_with_lower_or_equal_date()
        {
            //Arrange
            var repo = default(ITodoListRepository);// new InMemoryTodoListRepository(new List<string> { "Work" });
            var outputService = default(IOutputService); // new TestOutputService();
            var todoListService = new TodoListService(repo, outputService);

            //Act
            var addItemResult = todoListService.AddItem("Task", "Desc", "Work");
            var addItemHasErrors = addItemResult.HasErrors;
            var id = addItemResult.Value;

            var rprogResult = todoListService.RegisterProgression(id, new DateTime(2025, 1, 1), 10m);
            var rprogHasErrors = rprogResult.HasErrors;

            //Assert
            Assert.IsFalse(addItemHasErrors);
            Assert.IsFalse(rprogHasErrors);

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoListService.RegisterProgression(id, new DateTime(2025, 1, 1), 20m));

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoListService.RegisterProgression(id, new DateTime(2024, 12, 31), 20m));
        }

        [TestMethod]
        public void Cannot_register_progression_with_invalid_percent_or_non_increasing_percent()
        {
            //Arrange
            var repo = default(ITodoListRepository);// new InMemoryTodoListRepository(new List<string> { "Work" });
            var outputService = default(IOutputService); // new TestOutputService();
            var todoListService = new TodoListService(repo, outputService);

            //Act
            var addItemResult = todoListService.AddItem("Task", "Desc", "Work");
            var addItemHasErrors = addItemResult.HasErrors;
            var id = addItemResult.Value;

            //Assert
            Assert.IsFalse(addItemHasErrors);

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
            //Arrange
            var repo = default(ITodoListRepository);// new InMemoryTodoListRepository(new List<string> { "Work" });
            var outputService = default(IOutputService); // new TestOutputService();
            var todoListService = new TodoListService(repo, outputService);

            //Act
            var addItemresult = todoListService.AddItem("Task", "Desc", "Work");
            var addItemHasErrors = addItemresult.HasErrors;
            var id = addItemresult.Value;

            var rprogResult = todoListService.RegisterProgression(id, new DateTime(2025, 1, 1), 51m);
            var rprogHasErrors = rprogResult.HasErrors;

            //Assert
            Assert.IsFalse(addItemHasErrors);
            Assert.IsFalse(rprogHasErrors);

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoListService.UpdateItem(id, "NewDesc"));

            Assert.ThrowsException<InvalidOperationException>(() =>
                todoListService.RemoveItem(id));
        }
    }
}
