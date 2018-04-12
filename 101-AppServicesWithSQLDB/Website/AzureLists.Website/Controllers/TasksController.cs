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

        public async Task<ActionResult> Index()
        {
            var vm = new ListsViewModel()
            {
                Lists = await _listsService.GetAllLists()
            };
            return View(vm);
        }

     
    }
}