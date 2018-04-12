using AzureLists.Website.Models;
using AzureLists.Website.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AzureLists.Website.Controllers
{
    public class ListsController : Controller
    {
        private ListsService _listsService;
        public ListsController()
        {
            _listsService = new ListsService(ConfigurationManager.AppSettings["ApiUri"]);
        }
        
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name")] Models.Api.List list)
        {
            if (ModelState.IsValid)
            {
                await _listsService.CreateList(list);
                return RedirectToAction("Index");
            }

            return View(list);
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