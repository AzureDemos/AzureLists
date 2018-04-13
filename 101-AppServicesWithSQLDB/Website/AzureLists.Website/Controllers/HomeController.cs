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
    public class HomeController : Controller
    {
        private ListsService _listsService;

        public HomeController()
        {
            _listsService = new ListsService(ConfigurationManager.AppSettings["ApiUri"]);
        }
        
        public async Task<ActionResult> Index(string list = null, string task = null)
        {
            var lists = await _listsService.GetAllLists();
            var vm = new ListsViewModel() { Lists = lists };

            //Set selected list
            vm.SelectedList = (!string.IsNullOrWhiteSpace(list) && vm.Lists.Any()) 
                ? vm.Lists.SingleOrDefault(x => x.Id == list)
                : vm.Lists[0];

            if (!string.IsNullOrWhiteSpace(list) && vm.SelectedList == null)
                return HttpNotFound();

            //Set selected task
            if (!string.IsNullOrWhiteSpace(list) && vm.SelectedList != null && vm.SelectedList.Tasks != null && vm.SelectedList.Tasks.Any())
                vm.SelectedTask = vm.SelectedList.Tasks.FirstOrDefault(x => x.Id == task);

            if (!string.IsNullOrWhiteSpace(task) && vm.SelectedTask == null)
                return HttpNotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateList([Bind(Include = "Id,Name")] Models.Api.List list)
        {
            if (ModelState.IsValid)
            {
                var updated = await _listsService.UpdateList(list);
                return RedirectToAction("Index", new { list = list.Id }); // todo: handle errors, show success messages, etc
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] Models.Api.List list)
        {
            if (ModelState.IsValid)
            {
                await _listsService.UpdateList(list);
                return RedirectToAction("Index");
            }

            return View(list);
        }



    }
}