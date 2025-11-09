using BHTechTest.App.AppServices;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BHTechTest.App.WebApi.Controllers
{
    public class TodoListController : Controller
    {
        private readonly ITodoListAppService _appService;

        //TODO: Change to dependency injection
        public TodoListController()
        {
            var outputService = new WebApiOutputService();
            _appService = new TodoListAppService(outputService);
        }

        //TODO
        [HttpPost]
        public IActionResult Add()
        {
            try
            {
                //_appService.Add();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        //TODO
        [HttpPut]
        public IActionResult Update()
        {
            try
            {
                //_appService.Update();
                return Update();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        //TODO
        [HttpDelete]
        public IActionResult Delete()
        {
            try
            {
                //_appService.Delete();
                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        //TODO
        [HttpPost]
        public IActionResult Progress()
        {
            try
            {
                //_appService.Progress();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        //TODO
        [HttpGet]
        public IActionResult PrintItems()
        {
            try
            {
                //_appService.PrintItems();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        //TODO
        [HttpGet]
        public IActionResult PrintCategories()
        {
            try
            {
                //_appService.PrintCategories();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
