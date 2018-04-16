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
            ListsViewModel vm = await BuildViewModel(list, task);

            //if (!string.IsNullOrWhiteSpace(list) && vm.SelectedList == null)
            //    return HttpNotFound();  // Don't do this as we refresh the demo data periodically

            //if (!string.IsNullOrWhiteSpace(task) && vm.SelectedTask == null)
            //    return HttpNotFound(); // Don't do this as we refresh the demo data periodically

            return View(vm);
        }

        public async Task<ActionResult> ThisWeek()
        {
            ListsViewModel vm = await BuildViewModel(listName:"This Week");
            return View("Index", vm);
        }
        public async Task<ActionResult> Important()
        {
            ListsViewModel vm = await BuildViewModel(listName: "Important");
            return View("Index", vm);
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


        private async Task<ListsViewModel> BuildViewModel(string list = null, string task = null, string listName = null)
        {
            var lists = await _listsService.GetAllLists();
            var vm = new ListsViewModel() { Lists = lists };

            //Set selected list
            if (vm.Lists.Any())
            {
                if (!string.IsNullOrWhiteSpace(list))
                    vm.SelectedList = vm.Lists.SingleOrDefault(x => x.Id == list);
                else if (!string.IsNullOrWhiteSpace(listName))
                    vm.SelectedList = vm.Lists.SingleOrDefault(x => x.Name == listName);

                if  (vm.SelectedList == null)
                    vm.SelectedList = vm.Lists[0];
            }

            //Set selected task view model
            if (!string.IsNullOrWhiteSpace(list) && vm.SelectedList != null && vm.SelectedList.Tasks != null && vm.SelectedList.Tasks.Any())
            {
                var t = vm.SelectedList.Tasks.FirstOrDefault(x => x.Id == task);
                if (t != null && vm.SelectedList != null)
                {
                    vm.SelectedTask.Task = t;
                    vm.SelectedTask.ListId = vm.SelectedList.Id;
                }
            }

            return vm;
        }


    }
}