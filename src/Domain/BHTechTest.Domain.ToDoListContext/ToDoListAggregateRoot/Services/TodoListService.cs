using BHTechTest.Domain.ShareKernel;
using BHTechTest.Domain.ShareKernel.Results;
using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Entities;
using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Entities.Abstract;
using BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BHTechTest.Domain.ToDoListContext.ToDoListAggregateRoot.Services
{
    public class TodoListService : ITodoListService
    {
        private readonly ITodoListRepository _repository;
        private readonly IOutputService _outputService;
        private readonly ITodoList _todoList;

        public TodoListService(ITodoListRepository repository, IOutputService outputService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _outputService = outputService ?? throw new ArgumentNullException(nameof(outputService));
            _todoList = TodoList.Create(outputService);
        }

        /// <summary>
        /// For tests and usage: ability to seed items or query items (internal)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<TodoItem> GetItem(int id)
        {
            try
            {
                var result = Result.CreateDefault<TodoItem>();
                var value = ((TodoList)_todoList).Items.Single(i => i.Id == id);
                result.AddValue(value);
                return result;
            }
            catch (Exception ex)
            {
                return Result.CreateError<TodoItem>(ex);
            }
        }

        public Result<int> AddItem(string title, string description, string category)
        {
            try
            {
                var result = Result.CreateDefault<int>();
                var validateResult = ValidateCategory(category);
                result.Add(validateResult);
                if (result.HasErrors) return result;

                var id = _repository.GetNextId();

                _todoList.AddItem(id, title, description, category);
                result.AddValue(id);
                return result;
            }
            catch (Exception ex)
            {
                return Result.CreateError<int>(ex);
            }
        }
        private Result ValidateCategory(string category)
        {
            try
            {
                var result = Result.CreateDefault();

                var listCategoriesResult = ListCategories();
                result.AddTyped(listCategoriesResult);
                if (result.HasErrors) return result;

                var categories = listCategoriesResult.Value ?? [];
                var isValid = categories.Contains(category);
                if (!isValid) result.AddError($"Category '{category}' is not valid.");

                return result;
            }
            catch (Exception ex)
            {
                return Result.CreateError(ex);
            }
        }

        public Result UpdateItem(int id, string description)
        {
            try
            {
                var result = Result.CreateDefault();

                _todoList.UpdateItem(id, description);

                return result;
            }
            catch (Exception ex)
            {
                return Result.CreateError(ex);
            }
        }

        public Result RemoveItem(int id)
        {
            try
            {
                var result = Result.CreateDefault();

                _todoList.RemoveItem(id);

                return result;
            }
            catch (Exception ex)
            {
                return Result.CreateError(ex);
            }
        }

        public Result RegisterProgression(int id, DateTime dateTime, decimal percent)
        {
            try
            {
                var result = Result.CreateDefault();

                _todoList.RegisterProgression(id, dateTime, percent);

                return result;
            }
            catch (Exception ex)
            {
                return Result.CreateError(ex);
            }
        }

        public Result PrintItems()
        {
            try
            {
                var result = Result.CreateDefault();

                _todoList.PrintItems();

                return result;
            }
            catch (Exception ex)
            {
                return Result.CreateError(ex);
            }
        }

        public Result<List<string>> ListCategories()
        {
            try
            {
                var result = Result.CreateDefault<List<string>>();

                var categories = _repository.GetAllCategories() ?? [];

                return result.AddValue(categories);
            }
            catch (Exception ex)
            {
                return Result.CreateError<List<string>>(ex);
            }
        }


    }
}
