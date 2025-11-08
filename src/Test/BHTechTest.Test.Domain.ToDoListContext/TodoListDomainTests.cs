using BHTechTest.Domain.ShareKernel;
using BHTechTest.Domain.ShareKernel.Results;
using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Repositories;
using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BHTechTest.Test.Domain.ToDoListContext
{
    [TestClass]
    public class TodoListDomainTests
    {
        private Mock<ITodoListRepository> CreateDefaultRepoMock(int nextId = 1, IEnumerable<string>? categories = null)
        {
            var repoMock = new Mock<ITodoListRepository>();
            // For simple single-add tests, return a fixed nextId.
            repoMock.Setup(r => r.GetNextId()).Returns(nextId);
            repoMock.Setup(r => r.GetAllCategories()).Returns(new List<string>(categories ?? new[] { "Work", "Home", "Personal", "Other" }));
            return repoMock;
        }

        private Mock<IOutputService> CreateOutputMock()
        {
            var outputMock = new Mock<IOutputService>();
            // Allow WriteLine calls; we don't assert output content here.
            outputMock.Setup(o => o.WriteLine(It.IsAny<string>()));
            // Setup the non-generic Write(Result) overload that exists in IOutputService.
            outputMock.Setup(o => o.Write(It.IsAny<Result>()));
            // We avoid trying to setup the generic Write<T>(Result<T>) here; add a setup for a specific T if needed.
            return outputMock;
        }

        [TestMethod]
        public void HappyPath_example_and_print_like_sample()
        {
            // Arrange
            var repoMock = CreateDefaultRepoMock(nextId: 1, categories: new[] { "Work" });
            var outputMock = CreateOutputMock();
            var todoListService = new TodoListService(repoMock.Object, outputMock.Object);

            // Act - add item
            var addResult = todoListService.AddItem("Complete Project Report", "Finish the final report for the project", "Work");
            Assert.IsFalse(addResult.HasErrors, "AddItem should succeed");
            var id = addResult.Value;

            // Act - register progressions
            var r30 = todoListService.RegisterProgression(id, new DateTime(2025, 3, 18), 30m);
            Assert.IsFalse(r30.HasErrors, "RegisterProgression 30% should succeed");

            var r80 = todoListService.RegisterProgression(id, new DateTime(2025, 3, 19), 80m);
            Assert.IsFalse(r80.HasErrors, "RegisterProgression 80% should succeed");

            var r100 = todoListService.RegisterProgression(id, new DateTime(2025, 3, 20), 100m);
            Assert.IsFalse(r100.HasErrors, "RegisterProgression 100% should succeed");

            // Assert final state
            var getResult = todoListService.GetItem(id);
            Assert.IsFalse(getResult.HasErrors, "GetItem should succeed");
            var item = getResult.Value;
            Assert.IsNotNull(item);
            Assert.IsTrue(item.IsCompleted, "Item should be completed after 100%");
            Assert.AreEqual(3, item.Progressions.Count);
            Assert.AreEqual(100m, item.Progressions.Last().Percent);
        }

        [TestMethod]
        public void Cannot_add_progression_with_lower_or_equal_date()
        {
            // Arrange
            var repoMock = CreateDefaultRepoMock(nextId: 1, categories: new[] { "Work" });
            var outputMock = CreateOutputMock();
            var todoListService = new TodoListService(repoMock.Object, outputMock.Object);

            // Add item
            var addResult = todoListService.AddItem("Task", "Desc", "Work");
            Assert.IsFalse(addResult.HasErrors);
            var id = addResult.Value;

            // First progression OK
            var first = todoListService.RegisterProgression(id, new DateTime(2025, 1, 1), 10m);
            Assert.IsFalse(first.HasErrors);

            // Same date => should result in error (service wraps exceptions into Result)
            var sameDate = todoListService.RegisterProgression(id, new DateTime(2025, 1, 1), 20m);
            Assert.IsTrue(sameDate.HasErrors, "Registering a progression with the same date should produce an error");

            // Older date => should result in error
            var olderDate = todoListService.RegisterProgression(id, new DateTime(2024, 12, 31), 20m);
            Assert.IsTrue(olderDate.HasErrors, "Registering a progression with an older date should produce an error");
        }

        [TestMethod]
        public void Cannot_register_progression_with_invalid_percent_or_non_increasing_percent()
        {
            // Arrange
            var repoMock = CreateDefaultRepoMock(nextId: 1, categories: new[] { "Work" });
            var outputMock = CreateOutputMock();
            var todoListService = new TodoListService(repoMock.Object, outputMock.Object);

            // Add item
            var addResult = todoListService.AddItem("Task", "Desc", "Work");
            Assert.IsFalse(addResult.HasErrors);
            var id = addResult.Value;

            // Percent 0 => invalid
            var p0 = todoListService.RegisterProgression(id, DateTime.UtcNow.AddDays(1), 0m);
            Assert.IsTrue(p0.HasErrors, "Percent 0 should be invalid");

            // Percent > 100 => invalid
            var p120 = todoListService.RegisterProgression(id, DateTime.UtcNow.AddDays(1), 120m);
            Assert.IsTrue(p120.HasErrors, "Percent > 100 should be invalid");

            // Valid 30
            var p30 = todoListService.RegisterProgression(id, DateTime.UtcNow.AddDays(1), 30m);
            Assert.IsFalse(p30.HasErrors, "Valid progression 30% should succeed");

            // Non-increasing percent (20 after 30) => invalid
            var p20 = todoListService.RegisterProgression(id, DateTime.UtcNow.AddDays(2), 20m);
            Assert.IsTrue(p20.HasErrors, "Non-increasing percent should be invalid");
        }

        [TestMethod]
        public void Cannot_update_or_delete_item_with_progress_more_than_50_percent()
        {
            // Arrange
            var repoMock = CreateDefaultRepoMock(nextId: 1, categories: new[] { "Work" });
            var outputMock = CreateOutputMock();
            var todoListService = new TodoListService(repoMock.Object, outputMock.Object);

            // Add item
            var addResult = todoListService.AddItem("Task", "Desc", "Work");
            Assert.IsFalse(addResult.HasErrors);
            var id = addResult.Value;

            // Register progression > 50%
            var r = todoListService.RegisterProgression(id, new DateTime(2025, 1, 1), 51m);
            Assert.IsFalse(r.HasErrors, "Registering 51% should succeed");

            // Attempt update -> should report error via Result
            var upd = todoListService.UpdateItem(id, "NewDesc");
            Assert.IsTrue(upd.HasErrors, "Updating an item with progress > 50% should error");

            // Attempt remove -> should report error via Result
            var rem = todoListService.RemoveItem(id);
            Assert.IsTrue(rem.HasErrors, "Removing an item with progress > 50% should error");
        }
    }
}