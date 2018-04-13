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
                var result = await _listsService.CreateList(list);
                if (result.success)
                    return RedirectToAction("Index", "Home", new { list = result.newListId });
            }
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update([Bind(Include = "Id,Name")] Models.Api.List list)
        {
            if (ModelState.IsValid)
            {
                var existingList = await _listsService.GetList(list.Id); //Inlcudes all tasks which we are not changing
                if (existingList != null)
                {
                    existingList.Name = list.Name;
                    await _listsService.UpdateList(existingList);
                    return RedirectToAction("Index", "Home", new { list = existingList.Id });
                } 
            }
            return RedirectToAction("Index", "Home");
        }
    }
}