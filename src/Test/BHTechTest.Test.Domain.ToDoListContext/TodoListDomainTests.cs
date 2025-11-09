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
        public void HappyPath_example()
        {
            // Arrange
            var repoMock = CreateDefaultRepoMock(nextId: 1, categories: new[] { "Work" });
            var outputMock = CreateOutputMock();
            var todoListService = new TodoListService(repoMock.Object, outputMock.Object);

            //Act
            var addResult = todoListService.AddItem("Complete Project Report", "Finish the final report for the project", "Work");
            var id = addResult.Value;

            var r30 = todoListService.RegisterProgression(id, new DateTime(2025, 3, 18), 30m);
            var r50 = todoListService.RegisterProgression(id, new DateTime(2025, 3, 19), 50m);
            var r20 = todoListService.RegisterProgression(id, new DateTime(2025, 3, 20), 20m);

            var printResult = todoListService.PrintItems();

            var getResult = todoListService.GetItem(id);
            var item = getResult.Value;

            //Assert
            Assert.IsFalse(addResult.HasErrors, "AddItem should succeed");
            Assert.IsFalse(r30.HasErrors, "RegisterProgression +30% should succeed");
            Assert.IsFalse(r50.HasErrors, "RegisterProgression +50% should succeed");
            Assert.IsFalse(r20.HasErrors, "RegisterProgression +20% should succeed");
            Assert.IsFalse(getResult.HasErrors, "GetItem should succeed");
            Assert.IsNotNull(item);
            Assert.IsTrue(item.IsCompleted, "Item should be completed after 100%");
            Assert.AreEqual(3, item.Progressions.Count);
            Assert.AreEqual(100m, item.AcumulativeProgressPercent);
        }

        [TestMethod]
        public void Cannot_add_progression_with_lower_or_equal_date_but_greater_is_ok()
        {
            // Arrange
            var repoMock = CreateDefaultRepoMock(nextId: 1, categories: new[] { "Work" });
            var outputMock = CreateOutputMock();
            var todoListService = new TodoListService(repoMock.Object, outputMock.Object);

            //Act
            var addResult = todoListService.AddItem("Task", "Desc", "Work");
            var id = addResult.Value;

            var first = todoListService.RegisterProgression(id, new DateTime(2025, 1, 1), 10m);
            var sameDate = todoListService.RegisterProgression(id, new DateTime(2025, 1, 1), 20m);
            var olderDate = todoListService.RegisterProgression(id, new DateTime(2024, 12, 31), 20m);
            var newerDate = todoListService.RegisterProgression(id, new DateTime(2025, 1, 2), 20m);

            //Assert
            Assert.IsFalse(addResult.HasErrors);
            Assert.IsFalse(first.HasErrors);
            Assert.IsTrue(sameDate.HasErrors, "Registering a progression with the same date should produce an error");
            Assert.IsTrue(olderDate.HasErrors, "Registering a progression with an older date should produce an error");
            Assert.IsFalse(newerDate.HasErrors, "Registering a progression with an newer date should succeed");
        }

        [TestMethod]
        public void Cannot_register_progression_with_invalid_percent()
        {
            //Arrange
            var repoMock = CreateDefaultRepoMock(nextId: 1, categories: new[] { "Work" });
            var outputMock = CreateOutputMock();
            var todoListService = new TodoListService(repoMock.Object, outputMock.Object);

            //Act
            var addResult = todoListService.AddItem("Task", "Desc", "Work");
            var id = addResult.Value;

            var p0 = todoListService.RegisterProgression(id, DateTime.UtcNow.AddDays(1), 0m);
            var p120 = todoListService.RegisterProgression(id, DateTime.UtcNow.AddDays(1), 120m);
            var p30 = todoListService.RegisterProgression(id, DateTime.UtcNow.AddDays(1), 30m);

            //Assert
            Assert.IsFalse(addResult.HasErrors);
            Assert.IsTrue(p0.HasErrors, "Percent 0 should be invalid");
            Assert.IsTrue(p120.HasErrors, "Percent > 100 should be invalid");
            Assert.IsFalse(p30.HasErrors, "Valid progression 30% should succeed");
        }

        [TestMethod]
        public void Cannot_register_progression_with_acummulative_percent_greater_than_100percent()
        {
            //Arrange
            var repoMock = CreateDefaultRepoMock(nextId: 1, categories: new[] { "Work" });
            var outputMock = CreateOutputMock();
            var todoListService = new TodoListService(repoMock.Object, outputMock.Object);

            //Act
            var addResult = todoListService.AddItem("Task", "Desc", "Work");
            var id = addResult.Value;

            var rp1_30 = todoListService.RegisterProgression(id, DateTime.UtcNow.AddDays(1), 30m);
            var rp2_80 = todoListService.RegisterProgression(id, DateTime.UtcNow.AddDays(2), 80m);
            var rp2_70 = todoListService.RegisterProgression(id, DateTime.UtcNow.AddDays(2), 70m);

            //Assert
            Assert.IsFalse(addResult.HasErrors);
            Assert.IsFalse(rp1_30.HasErrors, "Progression 0 + 30% should succeed");
            Assert.IsTrue(rp2_80.HasErrors, "Progression 30 + 80 = 110% percent should be invalid");
            Assert.IsFalse(rp2_70.HasErrors, "Progression 30 + 70 = 100% percent should succeed");
        }

        [TestMethod]
        public void Cannot_update_or_delete_item_with_progress_more_than_50_percent()
        {
            //Arrange
            var repoMock = CreateDefaultRepoMock(nextId: 1, categories: new[] { "Work" });
            var outputMock = CreateOutputMock();
            var todoListService = new TodoListService(repoMock.Object, outputMock.Object);

            //Act
            var addResult = todoListService.AddItem("Task", "Desc", "Work");
            var id = addResult.Value;

            var r = todoListService.RegisterProgression(id, new DateTime(2025, 1, 1), 51m);
            var upd = todoListService.UpdateItem(id, "NewDesc");
            var rem = todoListService.RemoveItem(id);

            //Assert
            Assert.IsFalse(addResult.HasErrors);
            Assert.IsFalse(r.HasErrors, "Registering 51% should succeed");
            Assert.IsTrue(upd.HasErrors, "Updating an item with progress > 50% should error");
            Assert.IsTrue(rem.HasErrors, "Removing an item with progress > 50% should error");
        }
    }
}