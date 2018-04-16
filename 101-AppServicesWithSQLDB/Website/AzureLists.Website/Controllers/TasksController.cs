using AzureLists.Website.Models;
using AzureLists.Website.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AzureLists.Website.Controllers
{
    public class TasksController : Controller
    {
        private ListsService _listsService;

        public TasksController()
        {
            _listsService = new ListsService(ConfigurationManager.AppSettings["ApiUri"]);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string listId, [Bind(Include = "Title, Important")] Models.Api.Task task)
        {
            if (ModelState.IsValid)
            {
                var result = await _listsService.AddTaskToList(listId, task);
                if (result.success)
                    return RedirectToAction("Index", "Home", new { list = listId });
            }
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(string listId, [Bind(Include = "Id,Title,DueDate,Notes,Important")] Models.Api.Task task)
        {
            if (ModelState.IsValid)
            {
                var result = await _listsService.UpdateTask(listId, task);
                if (result)
                    return RedirectToAction("Index", "Home", new { list = listId });
            }
            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        public async Task<JsonResult> ToggleImportant(string listId, string taskId)
        {
            var task = await _listsService.GetTask(listId, taskId);
            task.Important = !task.Important;
            var result = await _listsService.UpdateTask(listId, task);
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> ToggleComplete(string listId, string taskId)
        {
            var task = await _listsService.GetTask(listId, taskId);
            if (task.CompletedDate != null && task.CompletedDate.HasValue)
                task.CompletedDate = null;
            else
                task.CompletedDate = DateTime.Now;
            var result = await _listsService.UpdateTask(listId, task);
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);

        }
    }
}